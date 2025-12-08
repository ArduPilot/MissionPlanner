using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Forms;

namespace MissionPlanner.Controls
{
    /// <summary>
    /// Handles loading and parsing STL model files for 3D visualization.
    /// Supports both embedded resources based on MAV_TYPE and custom file paths.
    /// </summary>
    public class STLModelLoader
    {
        private List<float> _vertices;
        private List<float> _normals;
        private bool _isLoaded = false;
        private MAVLink.MAV_TYPE? _loadedMavType = null;
        private string _customSTLPath = "";

        public List<float> Vertices => _vertices;
        public List<float> Normals => _normals;
        public int VertexCount => _vertices?.Count / 3 ?? 0;
        public bool IsLoaded => _isLoaded;
        public MAVLink.MAV_TYPE? LoadedMavType => _loadedMavType;

        public string CustomSTLPath
        {
            get => _customSTLPath;
            set
            {
                if (_customSTLPath != value)
                {
                    _customSTLPath = value;
                    _isLoaded = false; // Force reload when path changes
                }
            }
        }

        /// <summary>
        /// Loads the STL model if needed, based on current MAV_TYPE or custom path.
        /// Returns true if the model is ready to render.
        /// </summary>
        public bool EnsureLoaded()
        {
            // Get current MAV_TYPE
            MAVLink.MAV_TYPE currentMavType = MAVLink.MAV_TYPE.FIXED_WING;
            try
            {
                currentMavType = MainV2.comPort?.MAV?.aptype ?? MAVLink.MAV_TYPE.FIXED_WING;
            }
            catch { }

            // Check if we need to reload due to vehicle type change (only when using default STL)
            bool usingCustomSTL = !string.IsNullOrEmpty(_customSTLPath) && File.Exists(_customSTLPath);
            if (_isLoaded && !usingCustomSTL && _loadedMavType.HasValue && _loadedMavType.Value != currentMavType)
            {
                // Vehicle type changed, force reload
                _isLoaded = false;
            }

            if (_isLoaded) return true;

            try
            {
                string stlContent;
                if (usingCustomSTL)
                {
                    stlContent = File.ReadAllText(_customSTLPath);
                }
                else
                {
                    stlContent = GetDefaultSTLForMavType(currentMavType);
                    _loadedMavType = currentMavType;
                }

                if (string.IsNullOrEmpty(stlContent))
                {
                    MessageBox.Show("STL resource not found for vehicle type", "STL Load Error");
                    return false;
                }

                ParseSTL(stlContent);
                _isLoaded = true;
                return true;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error loading STL: " + ex.Message, "STL Load Error");
                return false;
            }
        }

        /// <summary>
        /// Forces a reload of the STL model on next EnsureLoaded() call.
        /// </summary>
        public void Invalidate()
        {
            _isLoaded = false;
        }

        /// <summary>
        /// Checks if the current vehicle is a quadplane via Q_ENABLE parameter.
        /// Some quadplanes report as FIXED_WING but have Q_ENABLE=1.
        /// </summary>
        private bool IsQuadPlaneByParam()
        {
            try
            {
                var param = MainV2.comPort?.MAV?.param;
                if (param != null && param.ContainsKey("Q_ENABLE"))
                {
                    return param["Q_ENABLE"].Value == 1.0;
                }
            }
            catch { }
            return false;
        }

        /// <summary>
        /// Gets the default embedded STL content based on MAV_TYPE.
        /// Uses specific models for different vehicle types, with fallbacks.
        /// Also checks Q_ENABLE parameter for quadplanes that report as FIXED_WING.
        /// </summary>
        private string GetDefaultSTLForMavType(MAVLink.MAV_TYPE mavType)
        {
            string stl = null;

            // Check for quadplane via Q_ENABLE parameter (some quadplanes report as FIXED_WING)
            if (mavType == MAVLink.MAV_TYPE.FIXED_WING && IsQuadPlaneByParam())
            {
                stl = MissionPlanner.Properties.Resources.quadplane_stl;
                if (string.IsNullOrEmpty(stl)) stl = MissionPlanner.Properties.Resources.plane_stl;
                return stl;
            }

            switch (mavType)
            {
                // Quadrotor (most common copter)
                case MAVLink.MAV_TYPE.QUADROTOR:
                    stl = MissionPlanner.Properties.Resources.quadrotor_stl;
                    if (string.IsNullOrEmpty(stl)) stl = MissionPlanner.Properties.Resources.copter_stl;
                    break;

                // Hexarotor
                case MAVLink.MAV_TYPE.HEXAROTOR:
                    stl = MissionPlanner.Properties.Resources.hexarotor_stl;
                    if (string.IsNullOrEmpty(stl)) stl = MissionPlanner.Properties.Resources.copter_stl;
                    break;

                // Octorotor
                case MAVLink.MAV_TYPE.OCTOROTOR:
                    stl = MissionPlanner.Properties.Resources.octorotor_stl;
                    if (string.IsNullOrEmpty(stl)) stl = MissionPlanner.Properties.Resources.copter_stl;
                    break;

                // Tricopter
                case MAVLink.MAV_TYPE.TRICOPTER:
                    stl = MissionPlanner.Properties.Resources.tricopter_stl;
                    if (string.IsNullOrEmpty(stl)) stl = MissionPlanner.Properties.Resources.copter_stl;
                    break;

                // Coaxial
                case MAVLink.MAV_TYPE.COAXIAL:
                    stl = MissionPlanner.Properties.Resources.coaxial_stl;
                    if (string.IsNullOrEmpty(stl)) stl = MissionPlanner.Properties.Resources.copter_stl;
                    break;

                // Helicopter
                case MAVLink.MAV_TYPE.HELICOPTER:
                    stl = MissionPlanner.Properties.Resources.helicopter_stl;
                    if (string.IsNullOrEmpty(stl)) stl = MissionPlanner.Properties.Resources.copter_stl;
                    break;

                // Dodecarotor (12 motors)
                case MAVLink.MAV_TYPE.DODECAROTOR:
                    stl = MissionPlanner.Properties.Resources.dodecarotor_stl;
                    if (string.IsNullOrEmpty(stl)) stl = MissionPlanner.Properties.Resources.copter_stl;
                    break;

                // VTOL / Quadplane types (via MAV_TYPE)
                case MAVLink.MAV_TYPE.VTOL_DUOROTOR:
                case MAVLink.MAV_TYPE.VTOL_QUADROTOR:
                case MAVLink.MAV_TYPE.VTOL_TILTROTOR:
                case MAVLink.MAV_TYPE.VTOL_RESERVED2:
                case MAVLink.MAV_TYPE.VTOL_RESERVED3:
                case MAVLink.MAV_TYPE.VTOL_RESERVED4:
                case MAVLink.MAV_TYPE.VTOL_RESERVED5:
                    stl = MissionPlanner.Properties.Resources.quadplane_stl;
                    if (string.IsNullOrEmpty(stl)) stl = MissionPlanner.Properties.Resources.plane_stl;
                    break;

                // Ground rover (car)
                case MAVLink.MAV_TYPE.GROUND_ROVER:
                    stl = MissionPlanner.Properties.Resources.rover_stl;
                    break;

                // Surface boat
                case MAVLink.MAV_TYPE.SURFACE_BOAT:
                    stl = MissionPlanner.Properties.Resources.boat_stl;
                    break;

                // Submarine
                case MAVLink.MAV_TYPE.SUBMARINE:
                    stl = MissionPlanner.Properties.Resources.sub_stl;
                    break;
            }

            // Final fallback to plane
            if (string.IsNullOrEmpty(stl))
                stl = MissionPlanner.Properties.Resources.plane_stl;

            return stl;
        }

        /// <summary>
        /// Parses ASCII STL content into vertices and normals.
        /// Keeps the STL’s original origin and applies a -90 degree Z rotation.
        /// </summary>
        private void ParseSTL(string stlContent)
        {
            _vertices = new List<float>();
            _normals = new List<float>();

            var lines = stlContent.Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);
            float nx = 0, ny = 0, nz = 0;

            foreach (var line in lines)
            {
                var trimmed = line.Trim();
                if (trimmed.StartsWith("facet normal"))
                {
                    var parts = trimmed.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                    nx = float.Parse(parts[2], System.Globalization.CultureInfo.InvariantCulture);
                    ny = float.Parse(parts[3], System.Globalization.CultureInfo.InvariantCulture);
                    nz = float.Parse(parts[4], System.Globalization.CultureInfo.InvariantCulture);
                }
                else if (trimmed.StartsWith("vertex"))
                {
                    var parts = trimmed.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                    float vx = float.Parse(parts[1], System.Globalization.CultureInfo.InvariantCulture);
                    float vy = float.Parse(parts[2], System.Globalization.CultureInfo.InvariantCulture);
                    float vz = float.Parse(parts[3], System.Globalization.CultureInfo.InvariantCulture);

                    _vertices.Add(vx);
                    _vertices.Add(vy);
                    _vertices.Add(vz);

                    _normals.Add(nx);
                    _normals.Add(ny);
                    _normals.Add(nz);
                }
            }

            // STL is in millimeters - conversion to meters happens in Map3D.DrawPlane()

            // Rotate STL -90 degrees around Z axis (swap X and Y, negate new Y)
            RotateModel();
        }

        /// <summary>
        /// Rotates the model -90 degrees around Z axis.
        /// </summary>
        private void RotateModel()
        {
            for (int i = 0; i < _vertices.Count; i += 3)
            {
                float x = _vertices[i];
                float y = _vertices[i + 1];
                _vertices[i] = y;
                _vertices[i + 1] = -x;
            }

            for (int i = 0; i < _normals.Count; i += 3)
            {
                float normX = _normals[i];
                float normY = _normals[i + 1];
                _normals[i] = normY;
                _normals[i + 1] = -normX;
            }
        }
    }
}
