
#region BSD License
/*
    Copyright (c) 2010 Miguel Angel Guirado López

    This file is part of CsAssortedWidgets.

    All rights reserved.
 
    This file is a C# port of AssortedWidgets project. Original authors see readme.txt file.

    Redistribution and use in source and binary forms, with or without modification, are permitted provided that the following conditions are met:

    Redistributions of source code must retain the above copyright notice, this list of conditions and the following disclaimer.
    Redistributions in binary form must reproduce the above copyright notice, this list of conditions and the following disclaimer in the documentation and/or other materials provided with the distribution.
    Neither the name of the <ORGANIZATION> nor the names of its contributors may be used to endorse or promote products derived from this software without specific prior written permission.
    THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT HOLDER OR CONTRIBUTORS BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.
*/
#endregion

using System;

using AssortedWidgets.Util;

namespace AssortedWidgets.Widgets
{
    public class ProgressBar : Component, IElement
    {
        float Value_;
        float min;
        float max;

        public ProgressBar()
            : this(ETypeOrientation.Horizontal)
        {
        }
        public ProgressBar(ETypeOrientation orientation)
            : this(0, 100f, orientation)
        {
        }
        public ProgressBar(float min, float max, ETypeOrientation orientation)
            : this(min, max, 0, orientation)
        {
        }
        public ProgressBar(float min, float max, float value, ETypeOrientation orientation)
        {
            this.min = min;
            this.max = max;
            Type = orientation;
            Size = GetPreferedSize();
            Value = value;

            if (orientation == ETypeOrientation.Horizontal)
            {
                HorizontalStyle = EElementStyle.Stretch;
                VerticalStyle = EElementStyle.Fit;
            }
            else
            {
                HorizontalStyle = EElementStyle.Fit;
                VerticalStyle = EElementStyle.Stretch;
            }
            Pack();
        }
		public Container Parent {
			get;
			set;
		}
    	
		public EElementStyle HorizontalStyle {
			get;
			set;
		}
    	
		public EElementStyle VerticalStyle {
			get;
			set;
		}
        public uint POfSlider
        {
            get;
            private set;
        }
        public float Value
        {
            get { return min + (max - min) * Value_; }
            set
            {
                if (value >= min && value <= max)
                {
                    Value_ = (value - min) / (max - min);
                    if (Type == ETypeOrientation.Horizontal)
                    {
                        POfSlider = (uint)(Value_ * (Size.width - 4));
                    }
                    else if (Type == ETypeOrientation.Vertical)
                    {
                        POfSlider = (uint)(Value_ * Size.height);
                    }
                }
            }
        }
        public ETypeOrientation Type
        {
            get;
            private set;
        }
        public override Size GetPreferedSize()
        {
            if (Type == ETypeOrientation.Horizontal)
            {
                return new Size(10, 20);
            }
            else
            {
                return new Size(20, 10);
            }
        }
        public override void Paint()
        {
            UI.Instance.CurrentTheme.PaintProgressBar(this);
        }
        public override void Pack()
        {
            base.Pack();

            if (Type == ETypeOrientation.Horizontal)
            {
                POfSlider = (uint)(Value_ * (Size.width - 4));
            }
            else if (Type == ETypeOrientation.Vertical)
            {
                POfSlider = (uint)(Value_ * Size.height);
            }			
        }
    }
    public enum ETypeOrientation
    {
        Horizontal,
        Vertical
    }
}
