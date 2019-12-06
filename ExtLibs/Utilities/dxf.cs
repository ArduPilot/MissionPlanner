using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using netDxf;
using netDxf.Entities;

namespace MissionPlanner.Utilities
{
    public class dxf
    {
        public delegate void LineEventHandler(dxf sender, Line line);
        public delegate void PolyLineEventHandler(dxf sender, Polyline pline);
        public delegate void LwPolylineEventHandler(dxf sender, LwPolyline pline);
        public delegate void MLineEventHandler(dxf sender, MLine pline);

        public event LineEventHandler newLine;
        public event PolyLineEventHandler newPolyLine;
        public event LwPolylineEventHandler newLwPolyline;
        public event MLineEventHandler newMLine;

        public object Tag;

        public void Read(string filename)
        {
            var dxfDocument = DxfDocument.Load(filename);

            foreach (var line in dxfDocument.Lines)
            {
                if(line.IsVisible)
                    newLine?.Invoke(this, line);
            }

            foreach (var pline in dxfDocument.Polylines)
            {
                if (pline.IsVisible)
                    newPolyLine?.Invoke(this, pline);
            }

            foreach (var lwpline in dxfDocument.LwPolylines)
            {
                if (lwpline.IsVisible)
                    newLwPolyline?.Invoke(this, lwpline);
            }

            foreach (var mline in dxfDocument.MLines)
            {
                if (mline.IsVisible)
                    newMLine?.Invoke(this, mline);
            }
        }
    }
}
