//
// Copyright 2014 LusoVU. All rights reserved.
//
// This library is free software; you can redistribute it and/or
// modify it under the terms of the GNU Lesser General Public
// License as published by the Free Software Foundation; either
// version 3 of the License, or (at your option) any later version.
//
// This library is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU
// Lesser General Public License for more details.
//
// You should have received a copy of the GNU Lesser General Public
// License along with this library; if not, write to the Free Software
// Foundation, Inc., 51 Franklin Street, Fifth Floor, Boston, MA  02110-1301,
// USA.
//
// Project home page: https://bitbucket.com/lusovu/xamarinusbserial
//

using System;
using System.Threading;

namespace Hoho.Android.UsbSerial
{
    internal static class EventHandlerExtensions
    {
        public static void Raise(this EventHandler handler, object sender, EventArgs e)
        {
            var temp = Volatile.Read(ref handler);
            if (temp != null)
                temp(sender, e);
        }

        public static void Raise<T>(this EventHandler<T> handler, object sender, T e) where T : EventArgs
        {
            var temp = Volatile.Read(ref handler);
            if (temp != null)
                temp(sender, e);
        }
    }
}