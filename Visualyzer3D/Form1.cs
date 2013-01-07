using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using MeshContainer;
using Mesher3D;

using Tao.OpenGl;
using Tao.Platform.Windows;

namespace Visualyzer3D
{
	public partial class Form1 : Form
	{
		public Form1()
		{
			InitializeComponent();
		}

		Delaunay3d mesh;
		List<Element3D> result;
		List<Node> givenpoint = new List<Node>();
		double unitl = 1.5;
		double diam = 10;
		double length = 5;

        bool l_down = false;
        bool m_down = false;
        bool r_down = false;
        System.Drawing.Point MousePositon;     //マウスポインタの位置
        System.Drawing.Point MouseTravel = new System.Drawing.Point(0, 0);      //マウスポインタの移動量
        float gl_rotate_x_base = 0f;
        float gl_rotate_y_base = 0f;
        float gl_rotate_z_base = 0f;
     //   float gl_rotate_x = 0f;
     //   float gl_rotate_y = 0f;
     //  float gl_rotate_z = 1f;

        System.Timers.Timer timer;

		private void Form1_Load(object sender, EventArgs e)
		{
            timer = new System.Timers.Timer();
            timer.Enabled = true;
            timer.AutoReset = true;
            timer.Interval = 100;
            timer.Elapsed += new System.Timers.ElapsedEventHandler(timer_Elapsed);

			int r_div = (int)(diam / 2 / unitl);
			int theta_div;
			int length_div = (int)(length / unitl);
			
			for (int k = 0; k <= length_div; k++)
			{
				double z = length * k / length_div;
                givenpoint.Add(new Node(-256, false, 0.0, 0.0, z));
				for (int i = 1; i <= r_div; i++)
				{
					double r = diam / 2 * i / r_div;
					theta_div = (int)(r * 2 * Math.PI / unitl);
					for (int j = 0; j < theta_div; j++)
					{
						double theta = 2 * Math.PI * j / theta_div;
						givenpoint.Add(new Node(-256, false, r * Math.Cos(theta), r * Math.Sin(theta), z));
					}
				}
			}
			Invalidate();

			mesh = new Delaunay3d(givenpoint);
		//	mesh.NextStep();
		//	mesh.NextStep();
			mesh.GetRawMesh(out result);
		//	timer1.Interval = 10;
		}

        void timer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
           // throw new NotImplementedException();

            if (m_down)
            {
                MouseTravel.X = Cursor.Position.X - MousePositon.X;
                MouseTravel.Y = Cursor.Position.Y - MousePositon.Y;
             //   MousePositon.X = Cursor.Position.X;
             //   MousePositon.Y = Cursor.Position.Y;

            }
            else
            {
                MouseTravel.X = 0;
                MouseTravel.Y = 0;
            }
            Invalidate();
        }

		protected override void OnPaint(PaintEventArgs e)
		{
			glpaint(MouseTravel);
		}

        private void glpaint(System.Drawing.Point Travel)
		{
            float r = (float)(Math.Sqrt(Travel.X * Travel.X + Travel.Y * Travel.Y) / 10.0);
            if (Travel.X != 0)
            {
                float theta = (float)(Math.Atan(Travel.Y / Travel.X));
            }
            else
            {
                float theta = 0;
            }
            
            float gl_rotate_x = gl_rotate_x_base + r;

       //     float angle_x = gl_rotate_angle * (gl_rotate_x/(gl_rotate_x+gl_rotate_y+gl_rotate_z));
        //    float angle_y = gl_rotate_angle * (gl_rotate_y/(gl_rotate_x+gl_rotate_y+gl_rotate_z));
       //     float angle_z = gl_rotate_angle * (gl_rotate_z/(gl_rotate_x+gl_rotate_y+gl_rotate_z));
            
          //  gl_rotate_angle = (float)(Travel.X / 10.0);
            

         //   gl_rotate_x = 1f;
         //   gl_rotate_y = 0f;
          //  gl_rotate_z = 0f;

			Wgl.wglMakeCurrent(glControl1.HDC, glControl1.HRC);

			Gl.glClear(Gl.GL_COLOR_BUFFER_BIT);

			Gl.glMatrixMode(Gl.GL_PROJECTION);		//射影変換行列の設定
			Gl.glLoadIdentity();
			Gl.glViewport(0, 0, glControl1.ClientRectangle.Width, glControl1.ClientRectangle.Height);
			Gl.glOrtho(-10f, 10f, -10f, 10f, -10f, 50f);
            Gl.glRotatef(0, 1f, 0f, 0f);
            Gl.glRotatef(0, 0f, 1f, 0f);
            Gl.glRotatef(gl_rotate_x, 0f, 0f, 1f);
            
            

         //   Gl.glRotatef(gl_rotate_angle_diff, gl_rotate_x, gl_rotate_y, 0);

			Gl.glMatrixMode(Gl.GL_MODELVIEW);   //モデル変換
			Gl.glLoadIdentity();
            
			
			
			for (int i = 0; i < givenpoint.Count; i++)
			{
				Gl.glBegin(Gl.GL_LINE_LOOP);
				Gl.glColor4f(0f, 1f, 1f, 1f);
				Gl.glVertex3f((float)(givenpoint[i].X + unitl * 0.1), (float)(givenpoint[i].Y + unitl * 0.1), (float)(givenpoint[i].Z));
				Gl.glVertex3f((float)(givenpoint[i].X - unitl * 0.1), (float)(givenpoint[i].Y + unitl * 0.1), (float)(givenpoint[i].Z));
				Gl.glVertex3f((float)(givenpoint[i].X - unitl * 0.1), (float)(givenpoint[i].Y - unitl * 0.1), (float)(givenpoint[i].Z));
				Gl.glVertex3f((float)(givenpoint[i].X + unitl * 0.1), (float)(givenpoint[i].Y - unitl * 0.1), (float)(givenpoint[i].Z));
				Gl.glEnd();
			}
			for (int i = 0; i < result.Count; i++)
			{
				Gl.glBegin(Gl.GL_LINE_LOOP);
				Gl.glColor4f(0f, 1f, 1f, 1f);
				Gl.glVertex3f((float)(result[i].GetPoint(0).X), (float)(result[i].GetPoint(0).Y), (float)(result[i].GetPoint(0).Z));
				Gl.glVertex3f((float)(result[i].GetPoint(1).X), (float)(result[i].GetPoint(1).Y), (float)(result[i].GetPoint(1).Z));
				Gl.glVertex3f((float)(result[i].GetPoint(2).X), (float)(result[i].GetPoint(2).Y), (float)(result[i].GetPoint(2).Z));
				Gl.glEnd();

				Gl.glBegin(Gl.GL_LINES);
				Gl.glColor4f(0f, 1f, 1f, 1f);
				Gl.glVertex3f((float)(result[i].GetPoint(0).X), (float)(result[i].GetPoint(0).Y), (float)(result[i].GetPoint(0).Z));
				Gl.glVertex3f((float)(result[i].GetPoint(3).X), (float)(result[i].GetPoint(3).Y), (float)(result[i].GetPoint(3).Z));
				Gl.glEnd();

				Gl.glBegin(Gl.GL_LINES);
				Gl.glColor4f(0f, 1f, 1f, 1f);
				Gl.glVertex3f((float)(result[i].GetPoint(1).X), (float)(result[i].GetPoint(1).Y), (float)(result[i].GetPoint(1).Z));
				Gl.glVertex3f((float)(result[i].GetPoint(3).X), (float)(result[i].GetPoint(3).Y), (float)(result[i].GetPoint(3).Z));
				Gl.glEnd();

				Gl.glBegin(Gl.GL_LINES);
				Gl.glColor4f(0f, 1f, 1f, 1f);
				Gl.glVertex3f((float)(result[i].GetPoint(2).X), (float)(result[i].GetPoint(2).Y), (float)(result[i].GetPoint(2).Z));
				Gl.glVertex3f((float)(result[i].GetPoint(3).X), (float)(result[i].GetPoint(3).Y), (float)(result[i].GetPoint(3).Z));
				Gl.glEnd();
				
			}

			Wgl.wglSwapBuffers(glControl1.HDC);
		}

        private void glControl1_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == System.Windows.Forms.MouseButtons.Left)
            {
                l_down = true;
            }
            else if (e.Button == System.Windows.Forms.MouseButtons.Middle)
            {
                m_down = true;
                MousePositon = new System.Drawing.Point(Cursor.Position.X, Cursor.Position.Y);
            }
            else if (e.Button == System.Windows.Forms.MouseButtons.Right)
            {
                r_down = true;
            }
        }

        private void glControl1_MouseUp(object sender, MouseEventArgs e)
        {
            if (e.Button == System.Windows.Forms.MouseButtons.Left)
            {
                l_down = false;
            }
            else if (e.Button == System.Windows.Forms.MouseButtons.Middle)
            {
                m_down = false;
                gl_rotate_x_base += (float)(Math.Sqrt(MouseTravel.X * MouseTravel.X + MouseTravel.Y * MouseTravel.Y) / 10.0);
            }
            else if (e.Button == System.Windows.Forms.MouseButtons.Right)
            {
                r_down = false;
            }
        }

        private void glControl1_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Down) 
            {
                
            }
            else if (e.KeyCode == Keys.Up)
            {

            }
            else if (e.KeyCode == Keys.Left)
            {

            }
            else if (e.KeyCode == Keys.Right)
            {

            }
        }

        private void Form1_SizeChanged(object sender, EventArgs e)
        {
            glControl1.Width = (this.Size.Width - 40) < (this.Size.Height - 90) ? (this.Size.Width - 40) : (this.Size.Height - 90);
            glControl1.Height = (this.Size.Width - 40) < (this.Size.Height - 90) ? (this.Size.Width - 40) : (this.Size.Height - 90);
        }
	}

    public class ViewSet
    {


        private System.Drawing.Point MousePoint;

        
    }
}
