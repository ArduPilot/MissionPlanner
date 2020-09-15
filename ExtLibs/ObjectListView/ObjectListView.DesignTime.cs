/*
 * DesignSupport - Design time support for the various classes within ObjectListView
 *
 * Author: Phillip Piper
 * Date: 12/08/2009 8:36 PM
 *
 * Change log:
 * 2012-08-27   JPP  - Fall back to more specific type name for the ListViewDesigner if
 *                     the first GetType() fails.
 * v2.5.1
 * 2012-04-26   JPP  - Filter group events from TreeListView since it can't have groups
 * 2011-06-06   JPP  - Vastly improved ObjectListViewDesigner, based off information in
 *                     "'Inheriting' from an Internal WinForms Designer" on CodeProject.
 * v2.3
 * 2009-08-12   JPP  - Initial version
 *
 * To do:
 *
 * Copyright (C) 2009-2014 Phillip Piper
 *
 * This program is free software: you can redistribute it and/or modify
 * it under the terms of the GNU General Public License as published by
 * the Free Software Foundation, either version 3 of the License, or
 * (at your option) any later version.
 *
 * This program is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU General Public License for more details.
 *
 * You should have received a copy of the GNU General Public License
 * along with this program.  If not, see <http://www.gnu.org/licenses/>.
 *
 * If you wish to use this code in a closed source application, please contact phillip.piper@gmail.com.
 */

using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Diagnostics;
using System.Drawing;
using System.Reflection;
using System.Windows.Forms;
using System.Windows.Forms.Design;

namespace BrightIdeasSoftware.Design
{

    /// <summary>
    /// Control how the overlay is presented in the IDE
    /// </summary>
    internal class OverlayConverter : ExpandableObjectConverter
    {
        public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType) {
            return destinationType == typeof(string) || base.CanConvertTo(context, destinationType);
        }

        public override object ConvertTo(ITypeDescriptorContext context, System.Globalization.CultureInfo culture, object value, Type destinationType) {
            if (destinationType == typeof(string)) {
                ImageOverlay imageOverlay = value as ImageOverlay;
                if (imageOverlay != null) {
                    return imageOverlay.Image == null ? "(none)" : "(set)";
                }
                TextOverlay textOverlay = value as TextOverlay;
                if (textOverlay != null) {
                    return String.IsNullOrEmpty(textOverlay.Text) ? "(none)" : "(set)";
                }
            }

            return base.ConvertTo(context, culture, value, destinationType);
        }
    }
}
