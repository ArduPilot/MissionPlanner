using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MissionPlanner.Maps
{
    using System;
    using GMap.NET.Projections;
    using System.Globalization;
    using GMap.NET.MapProviders;
    using GMap.NET;
    using System.Reflection;
    using GMap.NET.Core.GMap.NET.Projections;

    /// <summary>
    /// Custom
    /// </summary>
    public class NoMap : GMapProvider
    {
        public static readonly NoMap Instance;

        NoMap()
        {
            MaxZoom = 22;
        }

        static NoMap()
        {
            Instance = new NoMap();

            Type mytype = typeof (GMapProviders);
            FieldInfo field = mytype.GetField("DbHash", BindingFlags.Static | BindingFlags.NonPublic);
            Dictionary<int, GMapProvider> list = (Dictionary<int, GMapProvider>) field.GetValue(Instance);

            list.Add(Instance.DbId, Instance);

            
        }

        #region GMapProvider Members

        readonly Guid id = new Guid("4574228D-B552-4CAF-89AE-F20BADBBDB2B");

        public override Guid Id
        {
            get { return id; }
        }

        readonly string name = "NoMap";

        public override string Name
        {
            get { return name; }
        }

        GMapProvider[] overlays;

        public override GMapProvider[] Overlays
        {
            get
            {
                if (overlays == null)
                {
                    overlays = new GMapProvider[] {this};
                }
                return overlays;
            }
        }

        public override PureProjection Projection
        {
            get { return NoProjection.Instance; }
        }

        public override PureImage GetTileImage(GPoint pos, int zoom)
        {
            return null;
        }

        #endregion
    }
}