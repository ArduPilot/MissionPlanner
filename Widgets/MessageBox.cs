using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ArdupilotMega;
using ArdupilotMega.Utilities;
using AssortedWidgets.Layouts;
using AssortedWidgets.Widgets;
using System.Drawing;
using AssortedWidgets.Managers;

namespace ArdupilotMega.Widgets
{
    public class CustomMessageBox : MessageBox
    {
        public CustomMessageBox(string title)
            : base(title)
    {
    }
    }

    public class MessageBox : Dialog
    {
        // Summary:
        //     Specifies identifiers to indicate the return value of a dialog box.
        public enum DialogResult
        {
            // Summary:
            //     Nothing is returned from the dialog box. This means that the modal dialog
            //     continues running.
            None = 0,
            //
            // Summary:
            //     The dialog box return value is OK (usually sent from a button labeled OK).
            OK = 1,
            //
            // Summary:
            //     The dialog box return value is Cancel (usually sent from a button labeled
            //     Cancel).
            Cancel = 2,
            //
            // Summary:
            //     The dialog box return value is Abort (usually sent from a button labeled
            //     Abort).
            Abort = 3,
            //
            // Summary:
            //     The dialog box return value is Retry (usually sent from a button labeled
            //     Retry).
            Retry = 4,
            //
            // Summary:
            //     The dialog box return value is Ignore (usually sent from a button labeled
            //     Ignore).
            Ignore = 5,
            //
            // Summary:
            //     The dialog box return value is Yes (usually sent from a button labeled Yes).
            Yes = 6,
            //
            // Summary:
            //     The dialog box return value is No (usually sent from a button labeled No).
            No = 7,
        }

        // Summary:
        //     Specifies constants defining which buttons to display on a System.Windows.Forms.MessageBox.
        public enum MessageBoxButtons
        {
            // Summary:
            //     The message box contains an OK button.
            OK = 0,
            //
            // Summary:
            //     The message box contains OK and Cancel buttons.
            OKCancel = 1,
            //
            // Summary:
            //     The message box contains Abort, Retry, and Ignore buttons.
            AbortRetryIgnore = 2,
            //
            // Summary:
            //     The message box contains Yes, No, and Cancel buttons.
            YesNoCancel = 3,
            //
            // Summary:
            //     The message box contains Yes and No buttons.
            YesNo = 4,
            //
            // Summary:
            //     The message box contains Retry and Cancel buttons.
            RetryCancel = 5,
        }

        // Summary:
        //     Specifies constants defining which information to display.
        public enum MessageBoxIcon
        {
            // Summary:
            //     The message box contain no symbols.
            None = 0,
            //
            // Summary:
            //     The message box contains a symbol consisting of white X in a circle with
            //     a red background.
            Error = 16,
            //
            // Summary:
            //     The message box contains a symbol consisting of a white X in a circle with
            //     a red background.
            Hand = 16,
            //
            // Summary:
            //     The message box contains a symbol consisting of white X in a circle with
            //     a red background.
            Stop = 16,
            //
            // Summary:
            //     The message box contains a symbol consisting of a question mark in a circle.
            //     The question-mark message icon is no longer recommended because it does not
            //     clearly represent a specific type of message and because the phrasing of
            //     a message as a question could apply to any message type. In addition, users
            //     can confuse the message symbol question mark with Help information. Therefore,
            //     do not use this question mark message symbol in your message boxes. The system
            //     continues to support its inclusion only for backward compatibility.
            Question = 32,
            //
            // Summary:
            //     The message box contains a symbol consisting of an exclamation point in a
            //     triangle with a yellow background.
            Exclamation = 48,
            //
            // Summary:
            //     The message box contains a symbol consisting of an exclamation point in a
            //     triangle with a yellow background.
            Warning = 48,
            //
            // Summary:
            //     The message box contains a symbol consisting of a lowercase letter i in a
            //     circle.
            Information = 64,
            //
            // Summary:
            //     The message box contains a symbol consisting of a lowercase letter i in a
            //     circle.
            Asterisk = 64,
        }

        const int FORM_Y_MARGIN = 10;
        const int FORM_X_MARGIN = 16;

        static DialogResult _state = DialogResult.None;

        GirdLayout girdLayout;

        public MessageBox(string title)
            : base(title, 250, 250, 260, 180)
        {
            girdLayout = new GirdLayout(2, 2);

            // row. col
            girdLayout.SetHorizontalAlignment(0, 0, EHAlignment.HLeft);
            girdLayout.SetHorizontalAlignment(1, 0, EHAlignment.HRight);

            Layout = girdLayout;

            Resizable = false;
            Dragable = true;
        }

        public static DialogResult Show(string text)
        {
            return Show(text, string.Empty, MessageBoxButtons.OK, MessageBoxIcon.None);
        }

        public static DialogResult Show(string text, string caption)
        {
            return Show(text, caption, MessageBoxButtons.OK, MessageBoxIcon.None);
        }

        public static DialogResult Show(string text, string caption, MessageBoxButtons buttons)
        {
            return Show(text, caption, buttons, MessageBoxIcon.None);
        }

        public static DialogResult Show(string text, string caption, MessageBoxButtons buttons, MessageBoxIcon icon)
        {
            if (text == null)
                text = "";

            if (caption == null)
                caption = "";

            MessageBox mymb = new MessageBox(caption);

            mymb.mymb = mymb;

            // ensure we are always in a known state
            _state = DialogResult.None;

            // convert to nice wrapped lines.
            text = AddNewLinesToText(text);
            // get pixel width and height

            Graphics graphics = Graphics.FromImage(new Bitmap(1, 1));

            SizeF textSize = graphics.MeasureString(text, SystemFonts.DefaultFont);

            textSize = new SizeF(textSize.Width * 3, textSize.Height * 1);
            // allow for icon
  //          if (icon != MessageBoxIcon.None)
//                textSize.Width += SystemIcons.Question.Width;

            mymb.Size = new AssortedWidgets.Util.Size(textSize.Width + 50, textSize.Height + 100);

            //Rectangle screenRectangle = msgBoxFrm.RectangleToScreen(msgBoxFrm.ClientRectangle);
            //int titleHeight = screenRectangle.Top - msgBoxFrm.Top;

            var lblMessage = new Label(text);
            
            var actualIcon = getMessageBoxIcon(icon);

            if (actualIcon == null)
            {
             //   lblMessage.Location = new Point(FORM_X_MARGIN, FORM_Y_MARGIN);
            }
            else
            {
             //   var iconPbox = new PictureBox
                {
              //      Image = actualIcon.ToBitmap(),
              //      Location = new Point(FORM_X_MARGIN, FORM_Y_MARGIN)
                };
              //  msgBoxFrm.Controls.Add(iconPbox);
            }

            mymb.Add(lblMessage);

            //mymb.Add(actualIcon);

            AddButtonsToForm(mymb, buttons);

            mymb.Pack();

            DialogManager.Instance.SetModalDialog(mymb);

            DialogResult answer = _state;

            return answer;
        }

        MessageBox mymb { get; set; }
        public int Width { get { return (int)mymb.Size.width; } }
        public int Height { get { return (int)mymb.Size.height; } }

        // from http://stackoverflow.com/questions/2512781/winforms-big-paragraph-tooltip/2512895#2512895
        private static int maximumSingleLineTooltipLength = 85;

        private static string AddNewLinesToText(string text)
        {
            if (text.Length < maximumSingleLineTooltipLength)
                return text;
            int lineLength = maximumSingleLineTooltipLength;
            StringBuilder sb = new StringBuilder();
            int currentLinePosition = 0;
            for (int textIndex = 0; textIndex < text.Length; textIndex++)
            {
                // If we have reached the target line length and the next      
                // character is whitespace then begin a new line.   
                if (currentLinePosition >= lineLength &&
                    char.IsWhiteSpace(text[textIndex]))
                {
                    sb.Append(Environment.NewLine);
                    currentLinePosition = 0;
                }
                // reset line lnegth counter on existing new line
                if (text[textIndex] == Environment.NewLine[Environment.NewLine.Length - 1])
                {
                    currentLinePosition = 1;
                }
                // If we have just started a new line, skip all the whitespace.    
                if (currentLinePosition == 0)
                    while (textIndex < text.Length && char.IsWhiteSpace(text[textIndex]))
                        textIndex++;
                // Append the next character.     
                if (textIndex < text.Length) sb.Append(text[textIndex]);
                currentLinePosition++;
            }
            return sb.ToString();
        }

        private static void AddButtonsToForm(MessageBox msgBoxFrm, MessageBoxButtons buttons)
        {
           // Rectangle screenRectangle = msgBoxFrm.RectangleToScreen(msgBoxFrm.ClientRectangle);
            int titleHeight = 0;// screenRectangle.Top - msgBoxFrm.Top;

           // var t = Type.GetType("Mono.Runtime");
           // if ((t != null))
           //     titleHeight = 25;

            switch (buttons)
            {
                case MessageBoxButtons.OK:
                    
                    var but = new Button("OK");

                    but.MousePressedEvent += delegate { _state = DialogResult.OK; msgBoxFrm.Close(); };
                    msgBoxFrm.Add(but);
                    break;

                case MessageBoxButtons.YesNo:
                    var butyes = new Button("Yes");

                    butyes.MousePressedEvent += delegate { _state = DialogResult.Yes; msgBoxFrm.Close(); };
                    msgBoxFrm.Add(butyes);

                    var butno = new Button("No");

                    butno.MousePressedEvent += delegate { _state = DialogResult.No; msgBoxFrm.Close(); };
                    msgBoxFrm.Add(butno);
                    break;

                default:
                    throw new NotImplementedException("Only MessageBoxButtons.OK and YesNo supported at this time");
            }
        }

        /// <summary>
        /// Get system icon for MessageBoxIcon.
        /// </summary>
        /// <param name="icon">The MessageBoxIcon value.</param>
        /// <returns>SystemIcon type Icon.</returns>
        private static Icon getMessageBoxIcon(MessageBoxIcon icon)
        {
            switch (icon)
            {
                case MessageBoxIcon.Asterisk:
                    return SystemIcons.Asterisk;
                case MessageBoxIcon.Error:
                    return SystemIcons.Error;
                case MessageBoxIcon.Exclamation:
                    return SystemIcons.Exclamation;
                case MessageBoxIcon.Question:
                    return SystemIcons.Question;
                default:
                    return null;
            }
        }
    }
}
