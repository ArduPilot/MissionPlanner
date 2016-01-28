using System;
using System.ComponentModel;
using System.Reflection;
using GMap.NET;
using GMap.NET.Internals;
using GMap.NET.MapProviders;
using MissionPlanner.Utilities;

namespace MissionPlanner
{
    public class GMapCoreWrapper
    {
        private static readonly Type CoreType;
        private static readonly PropertyInfo PositionPropertyInfo;
        private static readonly PropertyInfo ProviderPropertyInfo;
        private static readonly PropertyInfo ZoomPropertyInfo;
        private static readonly FieldInfo MatrixFieldInfo;
        private static readonly FieldInfo TileDrawingListLockFieldInfo;
        private static readonly MethodInfo OnMapOpenMethodInfo;
        private static readonly MethodInfo OnMapSizeChangedMethodInfo;

        private readonly object _core;

        static GMapCoreWrapper()
        {
            Assembly asm = typeof(GMap.NET.Internals.Tile).Assembly;

            CoreType = asm.GetType("GMap.NET.Internals.Core");

            PositionPropertyInfo = CoreType.GetProperty("Position", typeof(GMap.NET.PointLatLng));
            ProviderPropertyInfo = CoreType.GetProperty("Provider", typeof(GMapProvider));
            ZoomPropertyInfo = CoreType.GetProperty("Zoom", typeof(int));

            MatrixFieldInfo = CoreType.GetField("Matrix", BindingFlags.Instance | BindingFlags.Public);
            TileDrawingListLockFieldInfo = CoreType.GetField("tileDrawingListLock");

            OnMapOpenMethodInfo = CoreType.GetMethod("OnMapOpen", BindingFlags.Instance | BindingFlags.Public);
            OnMapSizeChangedMethodInfo = CoreType.GetMethod("OnMapSizeChanged", BindingFlags.Instance | BindingFlags.Public);
        }

        public GMapCoreWrapper()
        {
            this._core = Activator.CreateInstance(CoreType);
        }

        public BackgroundWorker OnMapOpen()
        {
            return (BackgroundWorker)OnMapOpenMethodInfo.Invoke(this._core, null);
        }

        public void OnMapSizeChanged(int width, int height)
        {
            OnMapSizeChangedMethodInfo.Invoke(this._core, new object[] {width, height});
        }

        public void AcquireReadLocks()
        {
            FastReaderWriterLock tileDrawingListLock = (FastReaderWriterLock)TileDrawingListLockFieldInfo.GetValue(this._core);
            tileDrawingListLock.AcquireReaderLock();

            var matrix = MatrixFieldInfo.GetValue(this._core);
            MethodInfo enterReadLockInfo = matrix.GetType().GetMethod("EnterReadLock", BindingFlags.Instance | BindingFlags.Public);
            enterReadLockInfo.Invoke(matrix, null);
        }

        public void ReleaseReadLocks()
        {
            var matrix = MatrixFieldInfo.GetValue(this._core);
            MethodInfo enterReadLockInfo = matrix.GetType().GetMethod("LeaveReadLock", BindingFlags.Instance | BindingFlags.Public);
            enterReadLockInfo.Invoke(matrix, null);

            FastReaderWriterLock tileDrawingListLock = (FastReaderWriterLock)TileDrawingListLockFieldInfo.GetValue(this._core);
            tileDrawingListLock.ReleaseReaderLock();
        }

        public Tile GetTileWithNoLock(int zoom, GPoint p)
        {
            object matrix = MatrixFieldInfo.GetValue(this._core);
            MethodInfo getTileWithNoLockMethodInfo = matrix.GetType().GetMethod("GetTileWithNoLock", BindingFlags.Instance | BindingFlags.Public);
            return (Tile)getTileWithNoLockMethodInfo.Invoke(matrix, new object[] {zoom, p});
        }

        public GMap.NET.PointLatLng Position
        {
            get
            {
                return (GMap.NET.PointLatLng)PositionPropertyInfo.GetValue(this._core, null);
            }
            set
            {
                PositionPropertyInfo.SetValue(this._core, value, null);
            }
        }

        public GMapProvider Provider
        {
            get
            {
                return (GMapProvider)ProviderPropertyInfo.GetValue(this._core, null);
            }
            set
            {
                ProviderPropertyInfo.SetValue(this._core, value, null);
            }
        }

        public int Zoom
        {
            get
            {
                return (int)ZoomPropertyInfo.GetValue(this._core, null);
            }
            set
            {
                ZoomPropertyInfo.SetValue(this._core, value, null);
            }
        }
    }
}