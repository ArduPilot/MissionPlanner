///////////////////////////////////////////////////////////////////////////////
//
//  Kalman3D.cs
//
//  By Philip R. Braica (HoshiKata@aol.com, VeryMadSci@gmail.com)
//
//  Distributed under the The Code Project Open License (CPOL)
//  http://www.codeproject.com/info/cpol10.aspx
///////////////////////////////////////////////////////////////////////////////

// Using.
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

// Namespace.
namespace MissionPlanner.Utilities
{
    /// <summary>
    /// Kalman 3D.
    /// </summary>
    public class Kalman3D
    {
        #region Protected data.
        /// <summary>
        /// State.
        /// </summary>
        Matrix m_x = new Matrix(1, 3);

        /// <summary>
        /// Covariance.
        /// </summary>
        Matrix m_p = new Matrix(3, 3);

        /// <summary>
        /// Minimal covariance.
        /// </summary>
        Matrix m_q = new Matrix(3, 3);

        /// <summary>
        /// Minimal innovative covariance, keeps filter from locking in to a solution.
        /// </summary>
        double m_r;
        #endregion

        /// <summary>
        /// The last updated value, can also be set if filter gets
        /// sudden absolute measurement data for the latest update.
        /// </summary>
        public double Value
        {
            get { return m_x.Data[0]; }
            set { m_x.Data[0] = value; }
        }

        /// <summary>
        /// How fast the value is changing.
        /// </summary>
        public double Velocity
        {
            get { return m_x.Data[1]; }
        }

        /// <summary>
        /// Get the second derivative of the value.
        /// </summary>
        public double Acceleration
        {
            get { return m_x.Data[2]; }
        }

        /// <summary>
        /// The last kalman gain used, useful for debug.
        /// </summary>
        public double LastGain { get; protected set; }

        /// <summary>
        /// Last updated positional variance.
        /// </summary>
        /// <returns></returns>
        public double Variance() { return m_p.Data[0]; }

        /// <summary>
        /// Predict the value forward from last measurement time by dt.
        /// X = F*X + H*U        
        /// </summary>
        /// <param name="dt"></param>
        /// <returns></returns>
        public double Predicition(double dt)
        {
            return m_x.Data[0] + (dt * m_x.Data[1]) + (dt * dt * m_x.Data[2]);
        }

        /// <summary>
        /// Get the estimated covariance of position predicted 
        /// forward from last measurement time by dt.
        /// P = F*P*F^T + Q.
        /// </summary>
        /// <param name="dt"></param>
        /// <returns></returns>
        public double Variance(double dt)
        {
            Matrix f = new Matrix(3, 3)
            {
                Data = new double[] { 
                1, dt, dt * dt, 
                0, 1, dt,
                0, 0, 1}
            };
            Matrix pPredicted = Matrix.MultiplyABAT(f, m_p);
            pPredicted.Add(m_q);

            return pPredicted.Data[0];
        }

        /// <summary>
        /// Reset the filter.
        /// </summary>
        /// <param name="qx">Measurement to position state minimal variance.</param>
        /// <param name="qv">Measurement to velocity state minimal variance.</param>
        /// <param name="qa">Measurement to velocity state minimal variance.</param>
        /// <param name="r">Measurement covariance (sets minimal gain).</param>
        /// <param name="pd">Initial variance.</param>
        /// <param name="ix">Initial position.</param>
        public void Reset(double qx, double qv, double qa, double r, double pd, double ix)
        {
            m_q.Data[0] = qx * qx;
            m_q.Data[1] = qv * qx;
            m_q.Data[2] = qa * qx;
       
            m_q.Data[3] = qx * qv;
            m_q.Data[4] = qv * qv;
            m_q.Data[5] = qa * qv;
            
            m_q.Data[6] = qx * qa;
            m_q.Data[7] = qv * qa;
            m_q.Data[8] = qa * qa;

            m_r = r;

            // Diagonal to pd, others to zero.
            for (int i = 0; i < m_p.Data.Length; i++)
            {
                m_p.Data[i] = 0;
            }
            m_p.Data[0] = m_p.Data[4] = m_p.Data[8] = pd;

            // Initial position, velocity and acceleration.
            m_x.Data[0] = ix;  
            m_x.Data[1] = 0;
            m_x.Data[2] = 0;
        }

        /// <summary>
        /// Update the state by measurement m at dt time from last measurement.
        /// Requires only X position, sythesizes velocity and acceleration
        /// as internal states. 
        /// </summary>
        /// <param name="mx">Measured position.</param>
        /// <param name="dt">Time delta.</param>
        /// <returns>Updated position.</returns>
        public double Update(double mx, double dt)
        {
            double mv = dt != 0 ? (mx - m_x.Data[0]) / dt : 0;
            double ma = dt != 0 ? (mv - m_x.Data[1]) / dt : 0;
            return Update(mx, mv, ma, dt);
        }


        /// <summary>
        /// Update the state by measurement m at dt time from last measurement.
        /// Requires only X position and velocity, sythesizes acceleration
        /// as internal state. 
        /// </summary>
        /// <param name="mx">Measured position.</param>
        /// <param name="mv">Measured velocity.</param>
        /// <param name="dt">Time delta.</param>
        /// <returns>Updated position.</returns>
        public double Update(double mx, double mv, double dt)
        {
            double ma = dt != 0 ? (mv - m_x.Data[1]) / dt : 0;
            return Update(mx, mv, ma, dt);
        }


        /// <summary>
        /// Update the state by measurement m at dt time from last measurement.
        /// </summary>
        /// <param name="mx">Measured position.</param>
        /// <param name="mv">Measured velocity.</param>
        /// <param name="ma">Measured acceleration.</param>
        /// <param name="dt">Time delta.</param>
        /// <returns>Updated position.</returns>
        public double Update(double mx, double mv, double ma, double dt)
        {
            // Predict to now, then update.
            // Predict:
            //   X = F*X + H*U
            //   P = F*P*F^T + Q.
            // Update:
            //   Y = M – H*X          Called the innovation = measurement – state transformed by H.	
            //   S = H*P*H^T + R      S= Residual covariance = covariane transformed by H + R
            //   K = P * H^T *S^-1    K = Kalman gain = variance / residual covariance.
            //   X = X + K*Y          Update with gain the new measurement
            //   P = (I – K * H) * P  Update covariance to this time.
            //
            // Same as 1D but mv is used instead of delta m_x[0], and H = [1,1].

            // X = F*X + H*U
            Matrix f = new Matrix(3, 3) { Data = new double[] { 
                1, dt, dt * dt, 
                0, 1, dt,
                0, 0, 1} };
            Matrix h = Matrix.MakeIdentity(3);
            Matrix ht = Matrix.MakeIdentity(3); // h is identity, so ht is also identity.

            // U = {0,0};
            m_x = Matrix.Multiply(f, m_x);

            // P = F*P*F^T + Q
            m_p = Matrix.MultiplyABAT(f, m_p);
            m_p.Add(m_q);

            // Y = M – H*X  
            Matrix y = new Matrix(1, 3) { Data = new double[] { 
                mx - m_x.Data[0], 
                mv - m_x.Data[1],
                ma - m_x.Data[2]} };

            // S = H*P*H^T + R 
            Matrix s = Matrix.MultiplyABAT(h, m_p);

            // R is added across the diagnal, reducing by 10% for velocity and accel.,
            // which is often good for motion, and allows us 
            // to work off of a single m_r value.
            s.Data[0] += m_r;
            s.Data[4] += m_r;
            s.Data[8] += m_r;

            // K = P * H^T *S^-1 
            Matrix tmp = Matrix.Multiply(m_p, ht);
            Matrix sinv = Matrix.Invert(s);
            Matrix k = new Matrix(3, 3); // inited to zero.

            if (sinv != null)
            {
                k = Matrix.Multiply(tmp, sinv);
            }
            // Measurements are unstable so make the gain always be between 0 and 1.
            for (int i = 0; i < k.Data.Length; i++)
            {
                double gain = k.Data[i];
                gain = gain < 0 ? 0 : gain > 0.25 ? 0.25 : gain;
                k.Data[i] = gain;
            }

            LastGain = k.Determinant;

            // X = X + K*Y
            m_x.Add(Matrix.Multiply(k, y));

            // P = (I – K * H) * P
            Matrix kh = Matrix.Multiply(k, h);
            Matrix id = Matrix.MakeIdentity(3);
            kh.Multiply(-1);
            id.Add(kh);
            id.Multiply(m_p);
            m_p.Set(id);

            // Return latest estimate.
            return m_x.Data[0];
        }


    }
}
