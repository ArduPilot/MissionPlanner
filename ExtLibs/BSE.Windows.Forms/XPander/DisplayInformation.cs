using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Win32;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using System.Windows.Forms.VisualStyles;
using System.IO;
using System.Text.RegularExpressions;

namespace BSE.Windows.Forms
{
	static class DisplayInformation
	{
		#region FieldsPrivate

		[ThreadStatic]
        private static bool m_bIsThemed;
        private const string m_strRegExpression = @".*\.msstyles$";

		#endregion

		#region Properties

        internal static bool IsThemed
        {
            get { return m_bIsThemed; }
        }
		#endregion

		#region MethodsPrivate

		static DisplayInformation()
		{
			SystemEvents.UserPreferenceChanged += new UserPreferenceChangedEventHandler(DisplayInformation.OnUserPreferenceChanged);
			DisplayInformation.SetScheme();
		}

		private static void OnUserPreferenceChanged(object sender, UserPreferenceChangedEventArgs e)
		{
			DisplayInformation.SetScheme();
		}

		private static void SetScheme()
		{
			if (VisualStyleRenderer.IsSupported)
			{
				if (!VisualStyleInformation.IsEnabledByUser)
				{
					return;
				}
				StringBuilder stringBuilder = new StringBuilder(0x200);
				int iResult = NativeMethods.GetCurrentThemeName(stringBuilder, stringBuilder.Capacity, null, 0, null, 0);
                if (iResult == 0)
                {
                    Regex regex = new Regex(m_strRegExpression);
                    m_bIsThemed = regex.IsMatch(Path.GetFileName(stringBuilder.ToString()));
                }
			}
		}
		#endregion

		static class NativeMethods
		{
			[DllImport("uxtheme.dll", CharSet = CharSet.Unicode)]
			public static extern int GetCurrentThemeName(StringBuilder pszThemeFileName, int dwMaxNameChars, StringBuilder pszColorBuff, int dwMaxColorChars, StringBuilder pszSizeBuff, int cchMaxSizeChars);
		}
	}
}
