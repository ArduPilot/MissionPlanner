#region netDxf library, Copyright (C) 2009-2016 Daniel Carvajal (haplokuon@gmail.com)

//                        netDxf library
// Copyright (C) 2009-2016 Daniel Carvajal (haplokuon@gmail.com)
// 
// This library is free software; you can redistribute it and/or
// modify it under the terms of the GNU Lesser General Public
// License as published by the Free Software Foundation; either
// version 2.1 of the License, or (at your option) any later version.
// 
// The above copyright notice and this permission notice shall be included in all
// copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS
// FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR
// COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER
// IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN
// CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.

#endregion

using System;

namespace netDxf.Units
{
    /// <summary>
    /// Utility functions to handle DateTime conversions.
    /// </summary>
    public static class DrawingTime
    {
        /// <summary>
        /// Calculates a date represented as &lt;Julian date&gt;.&lt;Fraction of day&gt; from a <see cref="DateTime">DateTime</see> instance.
        /// </summary>
        /// <param name="date"><see cref="DateTime">DateTime</see> instance.</param>
        /// <returns>The date represented as &lt;Julian date&gt;.&lt;Fraction of day&gt; equivalent to the <see cref="DateTime">DateTime</see> instance.</returns>
        public static double ToJulianCalendar(DateTime date)
        {
            int year = date.Year;
            int month = date.Month;
            int day = date.Day;
            double hour = date.Hour;
            double minute = date.Minute;
            double second = date.Second;
            double millisecond = date.Millisecond;
            double fraction = day + hour/24.0 + minute/1440.0 + (second + millisecond/1000)/86400.0;

            if (month < 3)
            {
                year = year - 1;
                month = month + 12;
            }

            int a = year/100;
            int b = 2 - a + a/4;
            int c;
            if (year < 0)
                c = (int) (365.25*year - 0.75);
            else
                c = (int) (365.25*year);

            int d = (int) (30.6001*(month + 1));
            return b + c + d + 1720995 + fraction;
        }

        /// <summary>
        /// Calculates the <see cref="DateTime">DateTime</see> from a date represented as &lt;Julian date&gt;&lt;.Fraction of day&gt;.
        /// </summary>
        /// <param name="date">A date represented as &lt;Julian date&gt;.&lt;Fraction of day&gt;.</param>
        /// <returns>The <see cref="DateTime">DateTime</see> equivalent to the Julian date.</returns>
        public static DateTime FromJulianCalendar(double date)
        {
            if (date < 1721426 || date > 5373484)
                throw new ArgumentOutOfRangeException(nameof(date), "The valid values range from 1721426 and 5373484 that correspond to January 1, 1 and December 31, 9999 respectively.");

            double julian = (int) date;
            double fraction = date - julian;

            int temp = (int) ((julian - 1867216.25)/36524.25);
            julian = julian + 1 + temp - (int) (temp/4.0);

            int a = (int) julian + 1524;
            int b = (int) ((a - 122.1)/365.25);
            int c = (int) (365.25*b);
            int d = (int) ((a - c)/30.6001);

            int months = d < 14 ? d - 1 : d - 13;
            int years = months > 2 ? b - 4716 : b - 4715;
            int days = a - c - (int) (30.6001*d);

            int hours = (int) (fraction*24);
            fraction -= hours/24.0;
            int minutes = (int) (fraction*1440);
            fraction -= minutes/1440.0;

            double decimalSeconds = fraction*86400;
            int seconds = (int) decimalSeconds;
            int milliseconds = (int) ((decimalSeconds - seconds)*1000);
            return new DateTime(years, months, days, hours, minutes, seconds, milliseconds);
        }

        /// <summary>
        /// Calculates the <see cref="TimeSpan">TimeSpan</see> from a elapsed time represented as &lt;Number of days&gt;.&lt;Fraction of day&gt;.
        /// </summary>
        /// <param name="elapsed">An elapsed time represented as &lt;Number of days&gt;.&lt;Fraction of day&gt;.</param>
        /// <returns>The <see cref="TimeSpan">TimeSpan</see> equivalent to the elapsed time.</returns>
        public static TimeSpan EditingTime(double elapsed)
        {
            int days = (int) elapsed;
            double fraction = elapsed - days;

            int hours = (int) (fraction*24);
            fraction -= hours/24.0;

            int minutes = (int) (fraction*1440);
            fraction -= minutes/1440.0;

            double decimalSeconds = fraction*86400;
            int seconds = (int) decimalSeconds;
            int milliseconds = (int) ((decimalSeconds - seconds)*1000);

            return new TimeSpan(days, hours, minutes, seconds, milliseconds);
        }
    }
}