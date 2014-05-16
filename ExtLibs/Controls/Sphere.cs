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
        const float rad2deg = (float)(180 / Math.PI);
        const float deg2rad = (float)(1.0 / rad2deg);

        List<Vector3> points = new List<Vector3>();

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

        public void AddPoint(Vector3 point)
        {
            lock (points)
            {
                point.Normalize();

                point *= scale;

                points.Add(point);
            }

            minx = (float)Math.Min(minx, point.X);
            maxx = (float)Math.Max(maxx, point.X);

            miny = (float)Math.Min(miny, point.Y);
            maxy = (float)Math.Max(maxy, point.Y);

            minz = (float)Math.Min(minz, point.Z);
            maxz = (float)Math.Max(maxz, point.Z);

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

           // radians += 5 * deg2rad;

            if (rotatewithdata)
                yaw += 5 * deg2rad;

            MakeCurrent();

            GL.MatrixMode(MatrixMode.Projection);

            double max = Math.Max(maxx, scale);
            max = Math.Max(maxy, max);
            max = Math.Max(maxz, max);

            if (points.Count > 0)
            {
                Vector3 current = new Vector3(points[points.Count - 1].X, points[points.Count - 1].Y, points[points.Count - 1].Z);

                //yaw = Math.Atan2(points[points.Count - 1].X, points[points.Count - 1].Y);
            }


            OpenTK.Matrix4 projection = OpenTK.Matrix4.CreatePerspectiveFieldOfView(45 * deg2rad, 1f, 0.00001f, 5000.0f);
            GL.LoadMatrix(ref projection);

            float eyedist = (float)max * 3;

            // Z X Y

            //eye = Vector3.Transform(eye, Matrix4.RotateZ((float)yaw));

            //eye = Vector3.TransformPerspective(eye, Matrix4.CreateRotationX((float)pitch));

            eye = Vector3.TransformPosition(eye, Matrix4.CreateRotationZ((float)yaw));

            //get_pos(eyedist, yaw * rad2deg, pitch * rad2deg, ref eye);

            //eye = Vector3.TransformPosition(eye, Matrix4.CreateRotationY((float)Math.Sin(pitch)));
            //eye = Vector3.TransformPosition(eye, Matrix4.CreateRotationX((float)Math.Cos(pitch)));

            yaw = 0;
            pitch = 0;

           // eye = Vector3.Transform(eye, Matrix4.CreateRotationY((float)-pitch));



            if (float.IsNaN(eye.X))
                eye = new Vector3(1, 1, 1);

            eye.Normalize();

            eye *= eyedist;

            Console.WriteLine("eye "+ eye.ToString());
            //(maxx + minx) / 2, (maxy + miny) / 2, (maxz + minz) / 2
            Matrix4 modelview = Matrix4.LookAt(eye.X, eye.Y, eye.Z, 0,0,0, 0, 0, 1);
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
            GL.Vertex3(0, 0, 300);

            GL.Color3(Color.FromArgb(0, 255, 0));
            GL.Vertex3(0, 0, 0);
            GL.Vertex3(0, 300, 0);

            GL.Color3(Color.FromArgb(255, 0, 0));
            GL.Vertex3(0, 0, 0);
            GL.Vertex3(300, 0, 0);

            // -atives
            GL.Color3(Color.FromArgb(255, 255, 0));
            GL.Vertex3(0, 0, 0);
            GL.Vertex3(0, 0, -300);

            GL.Color3(Color.FromArgb(255, 0, 255));
            GL.Vertex3(0, 0, 0);
            GL.Vertex3(0, -300, 0);

            GL.Color3(Color.FromArgb(0, 255, 255));
            GL.Vertex3(0, 0, 0);
            GL.Vertex3(-300, 0, 0);


            GL.End();

            //GL.Rotate(Pitch, 0, 0, 0);
            //GL.Rotate(Roll, 0, 0, 0);
           // GL.Rotate(Yaw, 0, 0, 0);

            GL.Begin(PrimitiveType.Points);


            lock (points)
            {
                foreach (var item in points)
                {
                    float rangex = scale * 2;
                    float rangey = scale * 2;
                    float rangez = scale * 2;

                    int valuex = (int)Math.Abs((((item.X + scale) / rangex) * 255) % 255);
                    int valuey = (int)Math.Abs((((item.Y + scale) / rangey) * 255) % 255);
                    int valuez = (int)Math.Abs((((item.Z + scale) / rangez) * 255) % 255);

                    Color col = Color.FromArgb(valuex, valuey, valuez);

                    GL.Color3(col);

                    Vector3 vec = new Vector3(item.X, item.Y, item.Z);

                    GL.Vertex3(vec);
                }

                GL.End();

                // 8
                GL.PointSize(12);

                GL.Begin(PrimitiveType.Points);

                GL.Color3(Color.Red);

                if (points.Count > 0)
                    GL.Vertex3(points[points.Count - 1].X, points[points.Count - 1].Y, points[points.Count - 1].Z);

            }

            GL.End();

            this.SwapBuffers();
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
