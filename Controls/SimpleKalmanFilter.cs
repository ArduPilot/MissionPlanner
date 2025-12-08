using System;

namespace MissionPlanner.Controls
{
    /// <summary>
    /// Position-Velocity Kalman filter for ultra-smooth interpolation.
    /// Tracks both position and velocity to predict motion between updates,
    /// resulting in much smoother movement than a simple position-only filter.
    /// </summary>
    public class SimpleKalmanFilter
    {
        // State vector: [position, velocity]
        private double _x;  // estimated position
        private double _v;  // estimated velocity

        // Covariance matrix (2x2, stored as individual elements)
        private double _p00; // position variance
        private double _p01; // position-velocity covariance
        private double _p10; // velocity-position covariance
        private double _p11; // velocity variance

        // Noise parameters
        private double _processNoise;      // process noise for position
        private double _velocityNoise;     // process noise for velocity
        private double _measurementNoise;  // measurement noise

        // Timing for prediction
        private DateTime _lastUpdate;
        private bool _hasLastUpdate;

        // Smoothing factor (0-1, higher = more responsive, lower = smoother)
        private double _smoothingFactor;

        public SimpleKalmanFilter(double q = 0.1, double r = 1.0, double initialValue = 0)
        {
            // Map old parameters to new system with much smoother defaults
            _processNoise = q * 0.5;       // reduced process noise for smoother tracking
            _velocityNoise = q * 2.0;      // velocity can change more freely
            _measurementNoise = r * 2.0;   // trust measurements less for smoother output
            _smoothingFactor = 0.15;       // low smoothing factor for very smooth output

            _x = initialValue;
            _v = 0;

            // Initialize covariance matrix with moderate uncertainty
            _p00 = 1.0;
            _p01 = 0;
            _p10 = 0;
            _p11 = 1.0;

            _hasLastUpdate = false;
        }

        public double Update(double measurement)
        {
            DateTime now = DateTime.Now;
            double dt = 0.033; // default ~30fps

            if (_hasLastUpdate)
            {
                dt = Math.Max(0.001, Math.Min(0.5, (now - _lastUpdate).TotalSeconds));
            }
            _lastUpdate = now;
            _hasLastUpdate = true;

            // === PREDICTION STEP ===
            // Predict new position based on velocity: x_pred = x + v * dt
            double x_pred = _x + _v * dt;
            double v_pred = _v; // assume constant velocity model

            // Predict covariance: P = F * P * F' + Q
            // where F = [1, dt; 0, 1]
            double p00_pred = _p00 + dt * (_p10 + _p01) + dt * dt * _p11 + _processNoise * dt;
            double p01_pred = _p01 + dt * _p11;
            double p10_pred = _p10 + dt * _p11;
            double p11_pred = _p11 + _velocityNoise * dt;

            // === UPDATE STEP ===
            // Kalman gain: K = P * H' * inv(H * P * H' + R)
            // where H = [1, 0] (we only measure position)
            double S = p00_pred + _measurementNoise; // innovation covariance
            double k0 = p00_pred / S; // Kalman gain for position
            double k1 = p10_pred / S; // Kalman gain for velocity

            // Apply adaptive smoothing - reduce gain when prediction is close to measurement
            double innovation = measurement - x_pred;
            double adaptiveFactor = 1.0;

            // If innovation is small, we can trust prediction more (smoother)
            // If innovation is large, trust measurement more (responsive)
            double innovationMagnitude = Math.Abs(innovation);
            if (innovationMagnitude < 1.0)
            {
                adaptiveFactor = _smoothingFactor + (1.0 - _smoothingFactor) * innovationMagnitude;
            }

            k0 *= adaptiveFactor;
            k1 *= adaptiveFactor;

            // Update state: x = x_pred + K * (z - H * x_pred)
            _x = x_pred + k0 * innovation;
            _v = v_pred + k1 * innovation;

            // Limit velocity to reasonable values to prevent overshooting
            double maxVelocity = 500.0; // m/s, reasonable for aircraft
            _v = Math.Max(-maxVelocity, Math.Min(maxVelocity, _v));

            // Update covariance: P = (I - K * H) * P
            _p00 = (1 - k0) * p00_pred;
            _p01 = (1 - k0) * p01_pred;
            _p10 = p10_pred - k1 * p00_pred;
            _p11 = p11_pred - k1 * p01_pred;

            return _x;
        }

        public double Value => _x;

        public double Velocity => _v;

        /// <summary>
        /// Update the filter with an angular measurement (handles 0/360 wraparound).
        /// Returns the filtered angle normalized to 0-360 range.
        /// </summary>
        public double UpdateAngle(double measurement)
        {
            // Normalize measurement to 0-360
            measurement = NormalizeAngle(measurement);

            // Normalize current state to 0-360 for comparison
            double currentNormalized = NormalizeAngle(_x);

            // Find shortest angular distance
            double diff = measurement - currentNormalized;
            if (diff > 180) diff -= 360;
            else if (diff < -180) diff += 360;

            // Create an adjusted measurement that's close to current state
            // This prevents the filter from seeing large jumps
            double adjustedMeasurement = _x + diff;

            // Run normal update with adjusted measurement
            double result = Update(adjustedMeasurement);

            // Normalize the output to 0-360
            return NormalizeAngle(result);
        }

        private static double NormalizeAngle(double angle)
        {
            angle = angle % 360;
            if (angle < 0) angle += 360;
            return angle;
        }

        public void Reset(double value)
        {
            _x = value;
            _v = 0;
            _p00 = 1.0;
            _p01 = 0;
            _p10 = 0;
            _p11 = 1.0;
            _hasLastUpdate = false;
        }
    }
}
