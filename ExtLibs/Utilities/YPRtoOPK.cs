using System;
using System.Collections.Generic;
using System.Text;

namespace MissionPlanner.Utilities
{
    public class YPRtoOPK
    {
        //https://s3.amazonaws.com/mics.pix4d.com/KB/documents/New+Calibration+and+Computing+Method+for+Direct+Georeferencing+of+Image+and+Scanner+Data+Using.pdf
        //https://s3.amazonaws.com/mics.pix4d.com/KB/documents/Pix4D_Yaw_Pitch_Roll_Omega_to_Phi_Kappa_angles_and_conversion.pdf
        //https://github.com/nasa/georef_geocamutilweb/blob/master/geocamUtil/registration.py#L62
        //https://github.com/davdmaccartney/rpy_opk/blob/master/rpy_opk.py
        //https://github.com/OpenDroneMap/ODM/blob/0339e0108aa702177a8aca8eb6c1697c972c5602/opendm/photo.py#L804
        public static (double phi, double omega, double kappa) Convert(double roll, double pitch, double yaw, PointLatLngAlt plla)
        {
            Matrix3 yprMatrix = new Matrix3();

            yprMatrix.from_euler(roll * MathHelper.deg2rad, pitch * MathHelper.deg2rad, yaw * MathHelper.deg2rad);

            Matrix3 CBb = new Matrix3(0, 1, 0, 1, 0, 0, 0, 0, -1);

            var delta = 1e-7;

            double[] p1 = new double[3];
            double[] p2 = new double[3];
            Utilities.rtcm3.pos2ecef(
                new double[] { (plla.Lat + delta) * MathHelper.deg2rad, (plla.Lng) * MathHelper.deg2rad, plla.Alt },
                ref p1);
            Utilities.rtcm3.pos2ecef(
                new double[] { (plla.Lat - delta) * MathHelper.deg2rad, (plla.Lng) * MathHelper.deg2rad, plla.Alt },
                ref p2);
            var xnp = new Vector3(p1[0], p1[1], p1[2]) - new Vector3(p2[0], p2[1], p2[2]);
            xnp.normalize();

            var znp = new Vector3(0, 0, -1);
            var ynp = new Vector3(znp) % new Vector3(xnp);

            Matrix3 CEn = new Matrix3(xnp, ynp, znp);

            Matrix3 CEB = CEn * yprMatrix * CBb;

            var vec2 =  (Math.Asin(CEB[0,2]) * MathHelper.rad2deg, Math.Atan2(-CEB[1, 2] , CEB[2,2]) * MathHelper.rad2deg, Math.Atan2(-CEB[0, 1], CEB[0,0]) * MathHelper.rad2deg);
            
            return vec2;
        }
    }
}
