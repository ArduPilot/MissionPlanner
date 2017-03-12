using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using OpenTK;
using OpenTK.Graphics.OpenGL;

namespace MissionPlanner.Controls
{
    public class Sphere: OpenTK.GLControl
    {
        const double rad2deg = (180 / Math.PI);
        const double deg2rad = (1.0 / rad2deg);

        List<Vector3> points = new List<Vector3>();

        List<Vector3> aimpoints = new List<Vector3>();

        public Vector3 CenterPoint = new Vector3();

        public float scale = 300;

        Vector3 eye = new Vector3(1,1,1);

        float minx, maxx, miny, maxy, minz, maxz;
        private double yaw;
        private double pitch;

        public bool rotatewithdata { get; set; }

        public void Clear()
        {
            lock (points)
            {
                points.Clear();
            }
        }

        public void AimClear()
        {
            lock (aimpoints)
            {
                aimpoints.Clear();
            }
        }

        public void AimFor(Vector3 point)
        {
            lock (aimpoints)
            {
                //point.Normalize();

                //point *= scale;

                aimpoints.Add(point);
            }
        }

        public void AddPoint(Vector3 point)
        {
            minx = (float)Math.Min(minx, point.X);
            maxx = (float)Math.Max(maxx, point.X);

            miny = (float)Math.Min(miny, point.Y);
            maxy = (float)Math.Max(maxy, point.Y);

            minz = (float)Math.Min(minz, point.Z);
            maxz = (float)Math.Max(maxz, point.Z);

            lock (points)
            {
                //point.Normalize();

                //point *= scale;

                points.Add(point);
            }

            this.Invalidate();
        }

        protected override bool ProcessCmdKey(ref System.Windows.Forms.Message msg, System.Windows.Forms.Keys keyData)
        {
            if (keyData == System.Windows.Forms.Keys.Left)
            {
                yaw += -5 * deg2rad;
                this.Invalidate();
                return true;
            } 
            if (keyData == System.Windows.Forms.Keys.Right)
            {
                yaw += 5 * deg2rad;
                this.Invalidate();
                return true;
            } 
            if (keyData == System.Windows.Forms.Keys.Up)
            {
                pitch += 5 * deg2rad;
                this.Invalidate();
                return true;
            } 
            if (keyData == System.Windows.Forms.Keys.Down)
            {
                pitch += -5 * deg2rad;
                this.Invalidate();
                return true;
            }



            return base.ProcessCmdKey(ref msg, keyData);
        }


        protected override void OnPaint(System.Windows.Forms.PaintEventArgs e)
        {
            if (this.DesignMode)
            {
                e.Graphics.Clear(Color.Black);
                return;
            }

            GL.Viewport(0, 0, this.Width, this.Height);

            // radians += 5 * deg2rad;

            if (rotatewithdata)
                yaw += 5 * deg2rad;

            MakeCurrent();

            GL.MatrixMode(MatrixMode.Projection);

            double max = Math.Max(Math.Max((maxx - minx)/2,(maxy - miny)/2),(maxz - minz)/2);

            if (max < 300)
                max = 400;

            max *= 1.3;

            if (points.Count > 0)
            {
                Vector3 current = new Vector3(points[points.Count - 1].X, points[points.Count - 1].Y, points[points.Count - 1].Z);

                //yaw = Math.Atan2(points[points.Count - 1].X, points[points.Count - 1].Y);
            }


            OpenTK.Matrix4 projection = OpenTK.Matrix4.CreatePerspectiveFieldOfView((float)(45 * deg2rad), 1f, 0.00001f, 5000.0f);
            GL.LoadMatrix(ref projection);

            float eyedist = (float)max * 3;

            // Z X Y

            eye = Vector3.TransformPosition(eye, Matrix4.CreateRotationZ((float)yaw));

            yaw = 0;
            pitch = 0;

            if (float.IsNaN(eye.X))
                eye = new Vector3(1, 1, 1);

            eye.Normalize();

            eye *= eyedist;

            //Console.WriteLine("eye "+ eye.ToString());
            //(maxx + minx) / 2, (maxy + miny) / 2, (maxz + minz) / 2
            Matrix4 modelview = Matrix4.LookAt(eye.X, eye.Y, eye.Z, 0,0,0, 0, 0, 1); // CenterPoint.X, CenterPoint.Y, CenterPoint.Z
            GL.MatrixMode(MatrixMode.Modelview);

            GL.LoadMatrix(ref modelview);

            GL.ClearColor(Color.Black);

            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

            // 3
            GL.PointSize(8);

            GL.Begin(PrimitiveType.Lines);

            // +tivs
            GL.Color3(Color.FromArgb(0,0,255));
            GL.Vertex3(0, 0, 0);
            GL.Vertex3(0, 0, max);

            GL.Color3(Color.FromArgb(0, 255, 0));
            GL.Vertex3(0, 0, 0);
            GL.Vertex3(0, max, 0);

            GL.Color3(Color.FromArgb(255, 0, 0));
            GL.Vertex3(0, 0, 0);
            GL.Vertex3(max, 0, 0);

            // -atives
            GL.Color3(Color.FromArgb(255, 255, 0));
            GL.Vertex3(0, 0, 0);
            GL.Vertex3(0, 0, -max);

            GL.Color3(Color.FromArgb(255, 0, 255));
            GL.Vertex3(0, 0, 0);
            GL.Vertex3(0, -max, 0);

            GL.Color3(Color.FromArgb(0, 255, 255));
            GL.Vertex3(0, 0, 0);
            GL.Vertex3(-max, 0, 0);


            GL.End();

            //GL.Rotate(Pitch, 0, 0, 0);
            //GL.Rotate(Roll, 0, 0, 0);
           // GL.Rotate(Yaw, 0, 0, 0);

            GL.Begin(PrimitiveType.Points);


            lock (points)
            {
                foreach (var item in points)
                {
                    float rangex = maxx - minx;
                    float rangey = maxy - miny;
                    float rangez = maxz - minz;

                    int valuex = (int)Math.Abs((((item.X ) / rangex) * 254)) & 0xff;
                    int valuey = (int)Math.Abs((((item.Y) / rangey) * 254)) & 0xff;
                    int valuez = (int)Math.Abs((((item.Z) / rangez) * 254)) & 0xff;

                    Color col = Color.FromArgb(valuex, valuey, valuez);

                    GL.Color3(col);

                    Vector3 vec = new Vector3(item.X, item.Y, item.Z) + CenterPoint;

                    GL.Vertex3(vec);
                }

                lock (aimpoints)
                {
                    foreach (var aim in aimpoints)
                    {
                        GL.PointSize(8);
                        GL.Color3(Color.White);
                        GL.Vertex3(new Vector3(aim.X, aim.Y, aim.Z) + CenterPoint);
                    }
                }

                GL.End();

                // 8
                GL.PointSize(12);

                GL.Begin(PrimitiveType.Points);

                GL.Color3(Color.Red);

                if (points.Count > 0)
                    GL.Vertex3(new Vector3(points[points.Count - 1].X, points[points.Count - 1].Y, points[points.Count - 1].Z) + CenterPoint);

            }

            GL.End();

            Console.WriteLine(Math.Atan2(eye.Y, eye.X));

            //float newyaw = 0 * deg2rad;

            //DrawCircle(CenterPoint.X, CenterPoint.Y, CenterPoint.Z, newyaw, (float)(max), 60);

            this.SwapBuffers();
        }

        void DrawCircle(float cx, float cy, float cz, float yaw, float r, int num_segments)
        {
            GL.Begin(PrimitiveType.LineLoop);
            for (int ii = 0; ii < num_segments; ii++)
            {
                double theta = 2.0f * 3.1415926f * (double)ii / (double)num_segments;//get the current angle 

                double x = r * Math.Cos(theta);//calculate the x component 
                double y = 0;//r * Math.Sin(theta);//calculate the y component 
                double z = r * Math.Sin(theta);//calculate the y component 

                //x = x * Math.Cos(yaw) - y * Math.Sin(yaw);
                y = x * Math.Sin(yaw) + y * Math.Cos(yaw);
                //z = z;

                GL.Vertex3(x + cx, y + cy, z + cz);//output vertex 

            }
            GL.End();
        }

        void get_pos(double radius, double theta, double phi, ref Vector3 camera_pos)
        {
            /* get angles in radians */
            double th = theta * deg2rad;
            double ph = phi * deg2rad;
            camera_pos.X = (float)(radius * Math.Sin(th) * Math.Sin(ph));
            camera_pos.Y = (float)(radius * Math.Cos(th));
            camera_pos.Z = (float)(radius * Math.Sin(th) * Math.Cos(ph));
        }
    }
}
