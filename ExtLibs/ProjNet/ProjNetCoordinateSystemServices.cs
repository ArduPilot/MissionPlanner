// Copyright 2015 - Spartaco Giubbolini, Felix Obermaier (www.ivv-aachen.de)
//
// This file is part of ProjNet.
// ProjNet is free software; you can redistribute it and/or modify
// it under the terms of the GNU Lesser General Public License as published by
// the Free Software Foundation; either version 2 of the License, or
// (at your option) any later version.
// 
// ProjNet is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU Lesser General Public License for more details.
//
// You should have received a copy of the GNU Lesser General Public License
// along with SharpMap; if not, write to the Free Software
// Foundation, Inc., 59 Temple Place, Suite 330, Boston, MA  02111-1307  USA 
    
using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Threading;
using GeoAPI;
using GeoAPI.CoordinateSystems;
using GeoAPI.CoordinateSystems.Transformations;
using ProjNet.CoordinateSystems;

namespace ProjNet
{
    /// <summary>
    /// A coordinate system services class
    /// </summary>
    public class CoordinateSystemServices : ICoordinateSystemServices
    {
        private readonly Dictionary<int, ICoordinateSystem> _csBySrid;
        private readonly Dictionary<IInfo, int> _sridByCs;

        private readonly ICoordinateSystemFactory _coordinateSystemFactory;
        private readonly ICoordinateTransformationFactory _ctFactory;
        
        private readonly ManualResetEvent _initialization = new ManualResetEvent(false);

        #region CsEqualityComparer class
        private class CsEqualityComparer : EqualityComparer<IInfo>
        {
            public override bool Equals(IInfo x, IInfo y)
            {
                return x.AuthorityCode == y.AuthorityCode &&
                    string.Compare(x.Authority, y.Authority, StringComparison.OrdinalIgnoreCase) == 0;
            }

            public override int GetHashCode(IInfo obj)
            {
                if (obj == null) return 0;
                return Convert.ToInt32(obj.AuthorityCode) + (obj.Authority != null ? obj.Authority.GetHashCode() : 0);
            }
        }
        #endregion

        #region CoordinateSystemKey class

        private class CoordinateSystemKey : IInfo
        {
            public CoordinateSystemKey(string authority, long authorityCode)
            {
                Authority = authority;
                AuthorityCode = authorityCode;
            }

            public bool EqualParams(object obj)
            {
                throw new NotSupportedException();
            }

            public string Name { get { return null; } }
            public string Authority { get; private set; }
            public long AuthorityCode { get; private set; }
            public string Alias { get { return null; } }
            public string Abbreviation { get { return null; } }
            public string Remarks { get { return null; } }
            public string WKT { get { return null; } }
            public string XML { get { return null; } }
        }

        #endregion

        public CoordinateSystemServices(ICoordinateSystemFactory coordinateSystemFactory,
            ICoordinateTransformationFactory coordinateTransformationFactory)
        {
            if (coordinateSystemFactory == null)
                throw new ArgumentNullException("coordinateSystemFactory");
            _coordinateSystemFactory = coordinateSystemFactory;

            if (coordinateTransformationFactory == null)
                throw new ArgumentNullException("coordinateTransformationFactory");
            _ctFactory = coordinateTransformationFactory;

            _csBySrid = new Dictionary<int, ICoordinateSystem>();
            _sridByCs = new Dictionary<IInfo, int>(new CsEqualityComparer());

            FromEnumeration(new object[] { this, DefaultInitialization() });
        }

        //public Func<string, long, string> GetDefinition { get; set; }

        /*
        public static string GetFromSpatialReferenceOrg(string authority, long code)
        {
            var url = string.Format("http://spatialreference.org/ref/{0}/{1}/ogcwkt/", 
                authority.ToLowerInvariant(),
                code);
            var req = (HttpWebRequest) WebRequest.Create(url);
            using (var resp = req.GetResponse())
            {
                using (var resps = resp.GetResponseStream())
                {
                    if (resps != null)
                    {
                        using (var sr = new StreamReader(resps))
                            return sr.ReadToEnd();
                    }
                }
            }
            return null;
        }
         */

        public CoordinateSystemServices(ICoordinateSystemFactory coordinateSystemFactory,
            ICoordinateTransformationFactory coordinateTransformationFactory,
            IEnumerable<KeyValuePair<int, string>> enumeration)
            :this(coordinateSystemFactory, coordinateTransformationFactory)
        {
            var enumObj = (object)enumeration ?? DefaultInitialization();
            _initialization = new ManualResetEvent(false);
#if HAS_SYSTEM_THREADING_TASKS_TASK_RUN
            System.Threading.Tasks.Task.Run(() => FromEnumeration((new[] { this, enumObj })));
#elif HAS_SYSTEM_THREADING_THREADPOOL
            System.Threading.ThreadPool.QueueUserWorkItem(FromEnumeration, new[] { this, enumObj });
#else
#error Must have one or the other
#endif
        }

        //private CoordinateSystemServices(ICoordinateSystemFactory coordinateSystemFactory,
        //    ICoordinateTransformationFactory coordinateTransformationFactory,
        //    IEnumerable<KeyValuePair<int, ICoordinateSystem>> enumeration)
        //    : this(coordinateSystemFactory, coordinateTransformationFactory)
        //{
        //    var enumObj = (object)enumeration ?? DefaultInitialization();
        //    _initialization = new ManualResetEvent(false);
        //    ThreadPool.QueueUserWorkItem(FromEnumeration, new[] { this, enumObj });
        //}

        private static ICoordinateSystem CreateCoordinateSystem(ICoordinateSystemFactory coordinateSystemFactory, string wkt)
        {
            try
            {
                return coordinateSystemFactory.CreateFromWkt(wkt.Replace("ELLIPSOID", "SPHEROID"));
            }
            catch (Exception)
            {
                // as a fallback we ignore projections not supported
                return null;
            }
        }

        private static IEnumerable<KeyValuePair<int, ICoordinateSystem>> DefaultInitialization()
        {
            yield return new KeyValuePair<int, ICoordinateSystem>(4326, GeographicCoordinateSystem.WGS84);
            yield return new KeyValuePair<int, ICoordinateSystem>(3857, ProjectedCoordinateSystem.WebMercator);
        }

        private static void FromEnumeration(CoordinateSystemServices css,
            IEnumerable<KeyValuePair<int, ICoordinateSystem>> enumeration)
        {
            foreach (var sridCs in enumeration)
            {
                css.AddCoordinateSystem(sridCs.Key, sridCs.Value);
            }
        }

        private static IEnumerable<KeyValuePair<int, ICoordinateSystem>> CreateCoordinateSystems(
            ICoordinateSystemFactory factory,
            IEnumerable<KeyValuePair<int, string>> enumeration)
        {
            foreach (var sridWkt in enumeration)
            {
                var cs = CreateCoordinateSystem(factory, sridWkt.Value);
                if (cs != null)
                    yield return new KeyValuePair<int, ICoordinateSystem>(sridWkt.Key, cs);
            }
        }

        private static void FromEnumeration(CoordinateSystemServices css,
            IEnumerable<KeyValuePair<int, string>> enumeration)
        {
            FromEnumeration(css, CreateCoordinateSystems(css._coordinateSystemFactory, enumeration));
        }

        private static void FromEnumeration(object parameter)
        {
            var paras = (object[]) parameter;
            var css = (CoordinateSystemServices) paras[0];

            if (paras[1] is IEnumerable<KeyValuePair<int, string>>)
                FromEnumeration(css, (IEnumerable<KeyValuePair<int, string>>) paras[1]);
            else
                FromEnumeration(css, (IEnumerable<KeyValuePair<int, ICoordinateSystem>>)paras[1]);

            css._initialization.Set();
        }

        public ICoordinateSystem GetCoordinateSystem(int srid)
        {
            ICoordinateSystem cs;
            _initialization.WaitOne();
            return _csBySrid.TryGetValue(srid, out cs) ? cs : null;
        }

        public ICoordinateSystem GetCoordinateSystem(string authority, long code)
        {
            var srid = GetSRID(authority, code);
            if (srid.HasValue)
                return GetCoordinateSystem(srid.Value);
            return null;
        }

        public int? GetSRID(string authority, long authorityCode)
        {
            var key = new CoordinateSystemKey(authority, authorityCode);
            int srid;
            _initialization.WaitOne();
            if (_sridByCs.TryGetValue(key, out srid))
                return srid;

            return null;
        }

        public ICoordinateTransformation CreateTransformation(int sourceSrid, int targetSrid)
        {
            return CreateTransformation(GetCoordinateSystem(sourceSrid),
                GetCoordinateSystem(targetSrid));
        }

        public ICoordinateTransformation CreateTransformation(ICoordinateSystem src, ICoordinateSystem tgt)
        {
            return _ctFactory.CreateFromCoordinateSystems(src, tgt);
        }

        protected void AddCoordinateSystem(int srid, ICoordinateSystem coordinateSystem)
        {
            lock (((IDictionary) _csBySrid).SyncRoot)
            {
                lock (((IDictionary) _sridByCs).SyncRoot)
                {
                    if (_csBySrid.ContainsKey(srid))
                    {
                        if (ReferenceEquals(coordinateSystem, _csBySrid[srid]))
                            return;

                        _sridByCs.Remove(_csBySrid[srid]);
                        _csBySrid[srid] = coordinateSystem;
                        _sridByCs.Add(coordinateSystem, srid);
                    }
                    else
                    {
                        _csBySrid.Add(srid, coordinateSystem);
                        _sridByCs.Add(coordinateSystem, srid);
                    }
                }
            }
        }

        protected virtual int AddCoordinateSystem(ICoordinateSystem coordinateSystem)
        {
            var srid = (int) coordinateSystem.AuthorityCode;
            AddCoordinateSystem(srid, coordinateSystem);

            return srid;
        }

        protected void Clear()
        {
            _csBySrid.Clear();
        }

        protected int Count
        {
            get
            {
                _initialization.WaitOne();
                return _sridByCs.Count;
            }
        }

        public bool RemoveCoordinateSystem(int srid)
        {
            throw new NotSupportedException();
        }

        public IEnumerator<KeyValuePair<int, ICoordinateSystem>> GetEnumerator()
        {
            _initialization.WaitOne();
            return _csBySrid.GetEnumerator();
        }
    }
}