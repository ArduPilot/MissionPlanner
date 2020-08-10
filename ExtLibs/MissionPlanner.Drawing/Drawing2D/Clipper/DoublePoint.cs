/*******************************************************************************
*                                                                              *
* Author    :  Angus Johnson                                                   *
* Version   :  6.0.0                                                           *
* Date      :  11 September 2013                                               *
* Website   :  http://www.angusj.com                                           *
* Copyright :  Angus Johnson 2010-2013                                         *
*                                                                              *
* License:                                                                     *
* Use, modification & distribution is subject to Boost Software License Ver 1. *
* http://www.boost.org/LICENSE_1_0.txt                                         *
*                                                                              *
* Attributions:                                                                *
* The code in this library is an extension of Bala Vatti's clipping algorithm: *
* "A generic solution to polygon clipping"                                     *
* Communications of the ACM, Vol 35, Issue 7 (July 1992) pp 56-63.             *
* http://portal.acm.org/citation.cfm?id=129906                                 *
*                                                                              *
* Computer graphics and geometric modeling: implementation and algorithms      *
* By Max K. Agoston                                                            *
* Springer; 1 edition (January 4, 2005)                                        *
* http://books.google.com/books?q=vatti+clipping+agoston                       *
*                                                                              *
* See also:                                                                    *
* "Polygon Offsetting by Computing Winding Numbers"                            *
* Paper no. DETC2005-85513 pp. 565-575                                         *
* ASME 2005 International Design Engineering Technical Conferences             *
* and Computers and Information in Engineering Conference (IDETC/CIE2005)      *
* September 24-28, 2005 , Long Beach, California, USA                          *
* http://www.me.berkeley.edu/~mcmains/pubs/DAC05OffsetPolygon.pdf              *
*                                                                              *
*******************************************************************************/

/*******************************************************************************
*                                                                              *
* This is a translation of the Delphi Clipper library and the naming style     *
* used has retained a Delphi flavour.                                          *
*                                                                              *
*******************************************************************************/

//use_int32: When enabled 32bit ints are used instead of 64bit ints. This
//improve performance but coordinate values are limited to the range +/- 46340
//#define use_int32

//use_xyz: adds a Z member to IntPoint. Adds a minor cost to perfomance.
//#define use_xyz

//UseLines: Enables line clipping. Adds a very minor cost to performance.
//#define use_lines

//When enabled, code developed with earlier versions of Clipper 
//(ie prior to ver 6) should compile without changes. 
//In a future update, this compatability code will be removed.

#define use_deprecated


using System;
using System.Collections.Generic;

//using System.Text; //for Int128.AsString() & StringBuilder
//using System.IO; //streamReader & StreamWriter

namespace ClipperLib
{
#if use_int32
  using cInt = Int32;
#else
#endif
#if use_deprecated
    using Polygon = List<IntPoint>;

#endif

    internal class DoublePoint
    {
        public double X { get; set; }
        public double Y { get; set; }

        public DoublePoint(double x = 0, double y = 0)
        {
            this.X = x;
            this.Y = y;
        }

        public DoublePoint(DoublePoint dp)
        {
            this.X = dp.X;
            this.Y = dp.Y;
        }

        public DoublePoint(IntPoint ip)
        {
            this.X = ip.X;
            this.Y = ip.Y;
        }
    };


    //------------------------------------------------------------------------------
    // PolyTree & PolyNode classes
    //------------------------------------------------------------------------------


    //------------------------------------------------------------------------------
    // Int128 struct (enables safe math on signed 64bit integers)
    // eg Int128 val1((Int64)9223372036854775807); //ie 2^63 -1
    //    Int128 val2((Int64)9223372036854775807);
    //    Int128 val3 = val1 * val2;
    //    val3.ToString => "85070591730234615847396907784232501249" (8.5e+37)
    //------------------------------------------------------------------------------

    //------------------------------------------------------------------------------
    //------------------------------------------------------------------------------


    //By far the most widely used winding rules for polygon filling are
    //EvenOdd & NonZero (GDI, GDI+, XLib, OpenGL, Cairo, AGG, Quartz, SVG, Gr32)
    //Others rules include Positive, Negative and ABS_GTR_EQ_TWO (only in OpenGL)
    //see http://glprogramming.com/red/chapter11.html

    //end ClipperBase

    //end Clipper

    //------------------------------------------------------------------------------
} //end ClipperLib namespace