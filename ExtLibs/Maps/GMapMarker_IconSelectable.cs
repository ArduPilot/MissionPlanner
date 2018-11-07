using System;
using System.Drawing;
using GMap.NET;
using GMap.NET.WindowsForms;
using MissionPlanner.Utilities;
using SvgNet.SvgGdi;

namespace MissionPlanner.Maps
{
    [Serializable]
    public abstract class GMapMarker_IconSelectable : GMapMarker
    {

        private static System.Collections.Generic.List<Bitmap> _frame_icon_list = new System.Collections.Generic.List<Bitmap>();
        private static String _frame_icon_list_str = "";


        /// <summary>
        /// Constructor
        /// </summary>
        public GMapMarker_IconSelectable(PointLatLng p)
            : base(p)
        {
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="icon_index">index of icon. usage will define by child class.</param>
        /// <returns>Icon bitmap</returns>
        public System.Drawing.Bitmap FrameIcon(int icon_index = 0 )
        {
            Bitmap icon = null;

            // Check whether the setting has been changed.
            if (_frame_icon_list_str != Settings.Instance.FrameShapeFiles)
            {
                Load_Icon_List(Settings.Instance.FrameShapeFiles);
                _frame_icon_list_str = Settings.Instance.FrameShapeFiles;

            }

            if (0 <= icon_index && icon_index < _frame_icon_list.Count)
                icon = _frame_icon_list[icon_index];

            return icon ?? DefaultIcon(icon_index);
        }


        /// <summary>
        /// return default icon when use didn't set icon file.
        /// please override this method in child class for set to default frame icon.
        /// </summary>
        /// <param name="icon_index">index of icon. usage will define by child class.</param>
        /// <returns>Icon bitmap</returns>
        protected abstract Bitmap DefaultIcon(int icon_index);


        /// <summary>
        /// Load icon list from file.
        /// </summary>
        /// <param name="icon_list_str">selected filename list( separated by ';')</param>
        private static void Load_Icon_List(string icon_list_str)
        {
            // clear old icon list
            if (_frame_icon_list != null)
            {
                foreach (Bitmap bmp in _frame_icon_list)
                {
                    if (bmp != null) bmp.Dispose();
                }
                _frame_icon_list.Clear();
            }

            //
            // Load new bitmaps
            String[] strs = icon_list_str.Split(';');
            _frame_icon_list.Capacity = strs.Length;
            foreach (String fn in strs)
            {
                System.Drawing.Bitmap bmp = null;
                try
                {
                    bmp = (System.Drawing.Bitmap)System.Drawing.Image.FromFile(fn);
                }
                catch
                {
                    bmp = null;
                }
                _frame_icon_list.Add(bmp);
            }
        }


    }
}
