using System;
using System.Drawing;

namespace System.Drawing.Drawing2D
{
    public sealed class GraphicsPathIterator : MarshalByRefObject, IDisposable
    {
        GraphicsPath path;
        int markerPosition = 0;
        int subpathPosition = 0;
        int pathTypePosition = 0;

        public GraphicsPathIterator(GraphicsPath path)
        {
            // We do not have to have a path
            if (path == null)
                path = new GraphicsPath();
            else
                // We will clone the path so if things change it will not effect the iterator
                this.path = (GraphicsPath) path.Clone();
        }

        // Public Properites

        public int Count
        {
            get { return path.PointCount; }
        }

        public int SubpathCount
        {
            get
            {
                int count = 0;
                byte current;
                byte start = (byte) PathPointType.Start;

                for (int i = 0; i < path.types.Count; i++)
                {
                    current = path.types[i];

                    // count only the starts
                    if (current == start)
                        count++;
                }

                return count;
            }
        }

        internal void Dispose(bool disposing)
        {
            path.Dispose();
        }

        // Public Methods.

        public int CopyData(ref PointF[] points, ref byte[] types, int startIndex, int endIndex)
        {
            // no null checks, MS throws a NullReferenceException here
            if (points.Length != types.Length)
                throw new ArgumentException("Invalid arguments passed. Both arrays should have the same length.");

            var resultCount = 0;

            var end = Math.Min(path.points.Count - 1, endIndex);

            for (int s = startIndex, i = 0; s <= end; s++, i++)
            {
                points[i] = path.points[s];
                types[i] = path.types[s];
                resultCount++;
            }

            return resultCount;
        }

        public void Dispose()
        {
            Dispose(true);
            System.GC.SuppressFinalize(this);
        }

        ~GraphicsPathIterator()
        {
            Dispose(false);
        }

        public int Enumerate(ref PointF[] points, ref byte[] types)
        {
            return CopyData(ref points, ref types, 0, Count - 1);
        }

        public bool HasCurve()
        {
            return GraphicsPath.PathHasCurve(path);
        }

        public int NextMarker(GraphicsPath path)
        {
            int resultCount = 0;

            int index = 0;
            byte type;
            PointF point;

            // There are no paths or markers or we are done with all the markers 
            if (path == null || (this.path.points.Count == 0) ||
                (markerPosition == this.path.points.Count))
            {
                return resultCount;
            }

            // Clear the existing values from path 
            if (path.points.Count > 0)
            {
                path.points.Clear();
                path.types.Clear();
            }

            for (index = markerPosition; index < this.path.points.Count; index++)
            {
                type = this.path.types[index];
                point = this.path.points[index];
                path.points.Add(point);
                path.types.Add(type);

                // Copy the marker and stop copying the points when we reach a marker type 
                if ((type & (byte) PathPointType.PathMarker) != 0)
                {
                    index++;
                    break;
                }
            }

            resultCount = index - markerPosition;
            markerPosition = index;

            return resultCount;
        }

        public int NextMarker(out int startIndex, out int endIndex)
        {
            int resultCount = 0;

            // We have to assign something to the following for out parameters
            startIndex = 0;
            endIndex = 0;

            // There are no markers or we are done with all the markers 
            if ((path.points.Count == 0) ||
                (markerPosition == path.points.Count))
            {
                return resultCount;
            }

            var index = 0;
            // Check for next marker 
            for (index = markerPosition; index < path.types.Count; index++)
            {
                var type = path.types[index];
                if ((type & (byte) PathPointType.PathMarker) != 0)
                {
                    index++;
                    break;
                }
            }

            startIndex = markerPosition;
            endIndex = index - 1;
            resultCount = endIndex - startIndex + 1;
            ;

            markerPosition = index;

            return resultCount;
        }

        public int NextPathType(out byte pathType, out int startIndex, out int endIndex)
        {
            int resultCount = 0;

            // We need to initialize out parameters
            pathType = 0;
            startIndex = 0;
            endIndex = 0;


            int index;
            byte currentType;
            byte lastTypeSeen;

            // There are no subpaths or we are done with all the subpaths 
            if ((path.points.Count == 0) || (subpathPosition == 0))
            {
                return resultCount;
            }

            // Pathtype position lags behind subpath position 
            else if (pathTypePosition < subpathPosition)
            {
                lastTypeSeen = path.types[pathTypePosition + 1];
                // Mask the flags 
                lastTypeSeen &= (byte) PathPointType.PathTypeMask;

                // Check for the change in type 
                for (index = pathTypePosition + 2; index < subpathPosition; index++)
                {
                    currentType = path.types[index];
                    currentType &= (byte) PathPointType.PathTypeMask;

                    if (currentType != lastTypeSeen)
                        break;
                }

                startIndex = pathTypePosition;
                endIndex = index - 1;
                resultCount = endIndex - startIndex + 1;
                pathType = lastTypeSeen;

                // If lastTypeSeen is a line, it becomes the starting point for the next
                // path type. We get this when we have connected figures. We need to step
                // back in that case. We don't need to step back if we are finished with
                // current subpath.
                if ((lastTypeSeen == (byte) PathPointType.Line) && (index != subpathPosition))
                    pathTypePosition = index - 1;
                else
                    pathTypePosition = index;
            }

            // If pathtype position and subpath position coincide we return the resultCount = 0 
            else
                resultCount = 0;

            return resultCount;
        }

        public int NextSubpath(GraphicsPath path, out bool isClosed)
        {
            int resultCount = 0;

            // We have to initialize all out parameters
            isClosed = false;


            int index = 0;
            PointF point;
            byte currentType;

            // There are no subpaths or we are done with all the subpaths 
            if (path == null || this.path.points.Count == 0 ||
                (subpathPosition == this.path.points.Count))
            {
                isClosed = true;
                return resultCount;
            }

            // Clear the existing values from path 
            if (this.path.points.Count > 0)
            {
                path.points.Clear();
                path.types.Clear();
            }

            // Copy the starting point
            currentType = this.path.types[subpathPosition];
            point = this.path.points[subpathPosition];
            path.points.Add(point);
            path.types.Add(currentType);

            // Check for next start point 
            for (index = subpathPosition + 1; index < this.path.points.Count; index++)
            {
                currentType = this.path.types[index];

                // Copy the start point till next start point 
                if (currentType == (byte) PathPointType.Start)
                    break;

                point = this.path.points[index];
                path.points.Add(point);
                path.types.Add(currentType);
            }

            resultCount = index - subpathPosition;
            // set positions for next iteration
            pathTypePosition = subpathPosition;
            subpathPosition = index;

            // Check if last subpath was closed
            currentType = this.path.types[index - 1];
            if ((currentType & (byte) PathPointType.CloseSubpath) != 0)
                isClosed = true;
            else
                isClosed = false;

            return resultCount;
        }

        public int NextSubpath(out int startIndex, out int endIndex, out bool isClosed)
        {
            int resultCount = 0;

            // We have to initialize the out parameters
            startIndex = 0;
            endIndex = 0;
            isClosed = false;


            int index = 0;
            byte currentType;

            // There are no subpaths or we are done with all the subpaths 
            if ((path.types.Count == 0) ||
                (subpathPosition == path.types.Count))
            {
                // we don't touch startIndex and endIndex in this case 
                isClosed = true;
                return resultCount;
            }

            // Check for next start point 
            for (index = subpathPosition + 1; index < path.types.Count; index++)
            {
                currentType = path.types[index];
                if (currentType == (byte) PathPointType.Start)
                    break;
            }

            startIndex = subpathPosition;
            endIndex = index - 1;
            resultCount = endIndex - startIndex + 1;
            // set positions for next iteration 
            pathTypePosition = subpathPosition;
            subpathPosition = index;

            // check if last subpath was closed 
            currentType = path.types[index - 1];
            if ((currentType & (byte) PathPointType.CloseSubpath) != 0)
                isClosed = true;
            else
                isClosed = false;


            return resultCount;
        }

        public void Rewind()
        {
            subpathPosition = 0;
            pathTypePosition = 0;
            markerPosition = 0;
        }
    }
}