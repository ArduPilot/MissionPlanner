using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace System
{
    public static class CustomMessageBox
    {
        public delegate DialogResult ShowDelegate(string text, string caption = "",
            MessageBoxButtons buttons = MessageBoxButtons.OK, MessageBoxIcon icon = MessageBoxIcon.None);

        public static event ShowDelegate ShowEvent;

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

        public static DialogResult Show(string text, string caption = "", MessageBoxButtons MessageBoxButtons = MessageBoxButtons.OK, MessageBoxIcon MessageBoxIcon = MessageBoxIcon.None)
        {
            if (ShowEvent != null)
                return ShowEvent.Invoke(text, caption, MessageBoxButtons, MessageBoxIcon);

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
