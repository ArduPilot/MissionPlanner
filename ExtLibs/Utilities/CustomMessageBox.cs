using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace System
{
    public static class CustomMessageBox
    {
        public delegate DialogResult ShowDelegate(string text, string caption = "",
            MessageBoxButtons buttons = MessageBoxButtons.OK, MessageBoxIcon icon = MessageBoxIcon.None, string YesText = "Yes", string NoText = "No");

        public static event ShowDelegate ShowEvent;

        static readonly Text.RegularExpressions.Regex linkMarkupStart =
            new Text.RegularExpressions.Regex(@"\[link;", Text.RegularExpressions.RegexOptions.IgnoreCase);

        /// <summary>
        /// Stop text from another source (e.g. a vehicle's STATUSTEXT) from being read as
        /// [link;target;label] markup when it is shown in a message box. Only Mission Planner's own
        /// strings should be able to create a link.
        /// </summary>
        public static string NeutraliseLinkMarkup(string text)
        {
            if (string.IsNullOrEmpty(text))
                return text;
            return linkMarkupStart.Replace(text, "[link ");
        }

        /// <summary>
        /// The text to show for a [link;target;label] link: the label, followed by the host the link
        /// really opens, so a label cannot pass itself off as a different site.
        /// </summary>
        public static string LinkDisplayText(string link, string label)
        {
            Uri uri;
            if (link == null || !Uri.TryCreate(link, UriKind.Absolute, out uri) || string.IsNullOrEmpty(uri.Host))
                return label;

            if (string.Equals(label, link, StringComparison.OrdinalIgnoreCase))
                return label;

            return label + " (" + uri.Host + ")";
        }

        public static DialogResult Show(string text)
        {
            return Show(text, "", MessageBoxButtons.OK, MessageBoxIcon.None);
        }

        public static DialogResult Show(string text, string caption)
        {
            return Show(text, caption, MessageBoxButtons.OK, MessageBoxIcon.None);
        }

        public static int Show(string text, string caption = "", object MessageBoxButtons = null,
            object MessageBoxIcon = null)
        {
            if (MessageBoxButtons == null)
                MessageBoxButtons = CustomMessageBox.MessageBoxButtons.OK;
            if (MessageBoxIcon == null)
                MessageBoxIcon = CustomMessageBox.MessageBoxIcon.None;

            return (int)Show(text, caption, (MessageBoxButtons)(int)MessageBoxButtons, (MessageBoxIcon)(int)MessageBoxIcon);
        }

        public static DialogResult Show(string text, string caption = "", MessageBoxButtons MessageBoxButtons = MessageBoxButtons.OK, MessageBoxIcon MessageBoxIcon = MessageBoxIcon.None, string YesText = "Yes", string NoText = "No")
        {
            Console.WriteLine("CustomMessageBox.Show");

            if (ShowEvent != null)
                return ShowEvent.Invoke(text, caption, MessageBoxButtons, MessageBoxIcon, YesText, NoText);

            throw new Exception("ShowEvent Not Set");
        }

        public enum DialogResult
        {
            /// <summary>Nothing is returned from the dialog box. This means that the modal dialog continues running.</summary>
            None,
            /// <summary>The dialog box return value is OK (usually sent from a button labeled OK).</summary>
            OK,
            /// <summary>The dialog box return value is Cancel (usually sent from a button labeled Cancel).</summary>
            Cancel,
            /// <summary>The dialog box return value is Abort (usually sent from a button labeled Abort).</summary>
            Abort,
            /// <summary>The dialog box return value is Retry (usually sent from a button labeled Retry).</summary>
            Retry,
            /// <summary>The dialog box return value is Ignore (usually sent from a button labeled Ignore).</summary>
            Ignore,
            /// <summary>The dialog box return value is Yes (usually sent from a button labeled Yes).</summary>
            Yes,
            /// <summary>The dialog box return value is No (usually sent from a button labeled No).</summary>
            No
        }

        public enum MessageBoxButtons
        {
            /// <summary>The message box contains an OK button.</summary>
            OK,
            /// <summary>The message box contains OK and Cancel buttons.</summary>
            OKCancel,
            /// <summary>The message box contains Abort, Retry, and Ignore buttons.</summary>
            AbortRetryIgnore,
            /// <summary>The message box contains Yes, No, and Cancel buttons.</summary>
            YesNoCancel,
            /// <summary>The message box contains Yes and No buttons.</summary>
            YesNo,
            /// <summary>The message box contains Retry and Cancel buttons.</summary>
            RetryCancel
        }

        public enum MessageBoxIcon
        {
            /// <summary>The message box contain no symbols.</summary>
            None,
            /// <summary>The message box contains a symbol consisting of a white X in a circle with a red background.</summary>
            Hand = 16,
            /// <summary>The message box contains a symbol consisting of a question mark in a circle. The question-mark message icon is no longer recommended because it does not clearly represent a specific type of message and because the phrasing of a message as a question could apply to any message type. In addition, users can confuse the message symbol question mark with Help information. Therefore, do not use this question mark message symbol in your message boxes. The system continues to support its inclusion only for backward compatibility.</summary>
            Question = 32,
            /// <summary>The message box contains a symbol consisting of an exclamation point in a triangle with a yellow background.</summary>
            Exclamation = 48,
            /// <summary>The message box contains a symbol consisting of a lowercase letter i in a circle.</summary>
            Asterisk = 64,
            /// <summary>The message box contains a symbol consisting of white X in a circle with a red background.</summary>
            Stop = 16,
            /// <summary>The message box contains a symbol consisting of white X in a circle with a red background.</summary>
            Error = 16,
            /// <summary>The message box contains a symbol consisting of an exclamation point in a triangle with a yellow background.</summary>
            Warning = 48,
            /// <summary>The message box contains a symbol consisting of a lowercase letter i in a circle.</summary>
            Information = 64
        }
    }
}
