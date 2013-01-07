using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Diagnostics;
using MeshContainer;
using Mesher2D;

using Tao.OpenGl;
using Tao.Platform.Windows;

namespace Visualyzer2D
{
	public partial class Form1 : Form
	{
		public Form1()
		{
			InitializeComponent();
		}
		Delaunay2d mesh;
		List<DelaunayElement2D> result;
		Stopwatch sw = new Stopwatch();

		private void Form1_Load(object sender, EventArgs e)
		{
			textBox1.ReadOnly = true;

			var givenpoint1 = new List<Node>();
			var givenpoint2 = new List<Node>();
			var givenpoint3 = new List<Node>();

			MeshSample_Circle_01(givenpoint1);
			MeshSample_Circle_02(givenpoint2);
			MeshSample_Square(givenpoint3);


			mesh = new Delaunay2d(givenpoint2);
			result = new List<DelaunayElement2D>();
			mesh.GetRawMesh(result);
			timer1.Interval = 50;
		}

		private static void MeshSample_Circle_01(List<Node> givenpoint1)
		{
			double unitl = 0.05;
			double diam = 1;
			int r_div = (int)(diam / 2 / unitl);
			int theta_div;
			givenpoint1.Add(new Node(0, false, 0.5, 0.5));	//内部の点
			for (int i = 1; i <= r_div; i++)
			{
				double r = diam / 2 * i / r_div;
				theta_div = (int)(r * 2 * Math.PI / unitl);
				for (int j = 0; j < theta_div; j++)
				{
					double theta = 2 * Math.PI * j / theta_div;
					if (i != r_div)
					{
						givenpoint1.Add(new Node(0, false, 0.5 + r * Math.Cos(theta), 0.5 + r * Math.Sin(theta)));	//内部の点
					}
					else
					{
						givenpoint1.Add(new Node(0, true, 0.5 + r * Math.Cos(theta), 0.5 + r * Math.Sin(theta)));	//境界の点
					}
				}
			}
		}

		private static void MeshSample_Circle_02(List<Node> givenpoint2)
		{
			Random rnd = new Random();
			int size = 200;
			for (int i = 0; i < size; ++i)		//境界
			{
				double theta = 2 * Math.PI * i / size;
				givenpoint2.Add(new Node(0, true, 0.5 + 0.5 * Math.Cos(theta), 0.5 + 0.5 * Math.Sin(theta)));
			}
			for (int i = 0; i < size*size/Math.Sqrt(3)/Math.PI/2-size; ++i)		//ランダム内部
			{
				double x;
				double y;
				do
				{
					x = rnd.NextDouble();
					y = rnd.NextDouble();
				}
				while ((x - 0.5) * (x - 0.5) + (y - 0.5) * (y - 0.5) > 0.5 * 0.5);
				givenpoint2.Add(new Node(0, false, x, y));
			}
		}

		private static void MeshSample_Square(List<Node> givenpoint3)
		{
			Random rnd = new Random();

			for (int i = 0; i <= 15; ++i)		//境界
			{
				for (int j = 0; j <= 15; ++j)
				{
					if (((i == 0) | (i == 15)) | ((j == 0) | (j == 15)))
					{
						givenpoint3.Add(new Node(0, true, i / 15.0, j / 15.0));
					}
				}
			}
			for (int i = 0; i < 300; ++i)		//ランダム内部
			{
				givenpoint3.Add(new Node(0, false, rnd.NextDouble(), rnd.NextDouble()));
			}
		}

		protected override void OnPaint(PaintEventArgs e)
		{
			glpaint();
		}

		private void glpaint()
		{
			Wgl.wglMakeCurrent(glControl1.HDC, glControl1.HRC);

			Gl.glClear(Gl.GL_COLOR_BUFFER_BIT);

			Gl.glMatrixMode(Gl.GL_PROJECTION);
			Gl.glLoadIdentity();
			Gl.glViewport(0, 0, glControl1.ClientRectangle.Width, glControl1.ClientRectangle.Height);
			Gl.glOrtho(-0.1f, 1.1f, -0.1f, 1.1f, -10f, 50f);

			Gl.glMatrixMode(Gl.GL_MODELVIEW);
			Gl.glLoadIdentity();
			Gl.glRotatef(0f, 0f, 0f, 1f);

			for (int i = 0; i < result.Count; i++)
			{
            	Gl.glBegin(Gl.GL_LINE_LOOP);
			//	Gl.glBegin(Gl.GL_TRIANGLES);

                Gl.glColor4f(0f, 1f, 1f, 1f);
                Gl.glVertex2f((float)(result[i].GetPoint(0).X), (float)(result[i].GetPoint(0).Y));
			//	Gl.glColor4f(1f, 0f, 1f, 1f);
                Gl.glVertex2f((float)(result[i].GetPoint(1).X), (float)(result[i].GetPoint(1).Y));
			//	Gl.glColor4f(1f, 1f, 0f, 1f);
                Gl.glVertex2f((float)(result[i].GetPoint(2).X), (float)(result[i].GetPoint(2).Y));

                Gl.glEnd();
			}

			Wgl.wglSwapBuffers(glControl1.HDC);
		}
		
		private void Form1_Paint(object sender, PaintEventArgs e)
		{
			Invalidate();
		}

		private void button1_Click(object sender, EventArgs e)
		{
			timer1.Start();
			sw.Start();
		}

		private void timer1_Tick(object sender, EventArgs e)
		{
			mesh.NextStep();
			mesh.GetRawMesh(result);
			Invalidate();
			if (mesh.Complete)
			{
				timer1.Stop();
				sw.Stop();
				textBox1.Text = sw.ElapsedMilliseconds.ToString();
			}
		}

		private void button2_Click(object sender, EventArgs e)
		{
			mesh.PrintXML();

		}

		private void button3_Click(object sender, EventArgs e)
		{
			for (int i = 0; i < 1; ++i)
			{
				mesh.LaplacianSmoothing();
				mesh.GetDelaunay2d();
				mesh.GetRawMesh(result);
				Invalidate();
			}
		}

		private void Form1_SizeChanged(object sender, EventArgs e)
		{
			glControl1.Height = (this.Width - 42) > (this.Height - 93) ? (this.Height - 93) : (this.Width - 42);
			glControl1.Width = (this.Width - 42) > (this.Height - 93) ? (this.Height - 93) : (this.Width - 42);
            if ((this.Width - 42) > (this.Height - 93))
            {
                glControl1.Location = new System.Drawing.Point(((this.Width - 42) - (this.Height - 93)) / 2 + 13, 42);
            }
            else
            {
                glControl1.Location = new System.Drawing.Point(13, 42);
            }
			Invalidate();
		}

		private void button4_Click(object sender, EventArgs e)
		{
			if ((this.FormBorderStyle == System.Windows.Forms.FormBorderStyle.None) & (this.WindowState == FormWindowState.Maximized))
			{
				this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.Sizable;
				this.WindowState = FormWindowState.Normal;
			}
			else
			{
				this.WindowState = FormWindowState.Normal;
				this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
				this.WindowState = FormWindowState.Maximized;
			}
		}
	}
}
