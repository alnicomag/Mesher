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

namespace Delaunay3D
{
	public partial class Form1 : Form
	{
		public Form1()
		{
			InitializeComponent();
		}

		Delaunay3d mesh;
		List<Element3D> result;

		float rotation=0;

		private void Form1_Load(object sender, EventArgs e)
		{
			var givenpoint2 = new List<Node>();
			var givenpoint3 = new List<Node>();

			Random rnd = new Random();
			for (int i = 0; i < 10; ++i)
			{
				givenpoint3.Add(new Node(0, false, rnd.NextDouble() * 1, rnd.NextDouble() * 1, rnd.NextDouble() * 1));
			}

			for (int i = 0; i < 2; ++i)
			{
				for (int j = 0; j < 2; ++j)
				{
					for (int k = 0; k < 2; ++k)
					{
						givenpoint2.Add(new Node(0, false, i * 0.1, j * 0.1, k * 0.1));
					}
				}
			}

			Invalidate();

			mesh = new Delaunay3d(givenpoint2);
			mesh.GetNormalizedMesh(out result);
			mesh.PrintXML();

			timer1.Interval = 10;
			timer2.Interval = 100;
			timer2.Start();
		}


		protected override void OnPaint(PaintEventArgs e)
		{
			glpaint(rotation);
		}

		private void glpaint(float aa)
		{
			Wgl.wglMakeCurrent(glControl1.HDC, glControl1.HRC);

			Gl.glClear(Gl.GL_COLOR_BUFFER_BIT);

			Gl.glMatrixMode(Gl.GL_PROJECTION);
			Gl.glLoadIdentity();
			Gl.glViewport(0, 0, glControl1.ClientRectangle.Width, glControl1.ClientRectangle.Height);
			Gl.glOrtho(-3f, 3f, -3f, 3f, -10f, 50f);

			Gl.glMatrixMode(Gl.GL_MODELVIEW);
			Gl.glLoadIdentity();
			Gl.glRotatef(aa, 1f, 0.5f, 0.3f);

			for (int i = 0; i < result.Count; i++)
			{
				Gl.glBegin(Gl.GL_LINE_LOOP);
				Gl.glColor4f(0f, 1f, 1f, 1f);
				Gl.glVertex3f((float)(result[i].GetPoint(0).X), (float)(result[i].GetPoint(0).Y), (float)(result[i].GetPoint(0).Z));
				Gl.glVertex3f((float)(result[i].GetPoint(1).X), (float)(result[i].GetPoint(1).Y), (float)(result[i].GetPoint(1).Z));
				Gl.glVertex3f((float)(result[i].GetPoint(2).X), (float)(result[i].GetPoint(2).Y), (float)(result[i].GetPoint(2).Z));
				Gl.glEnd();
				Gl.glBegin(Gl.GL_LINE_STRIP);
				Gl.glColor4f(0f, 1f, 1f, 1f);
				Gl.glVertex3f((float)(result[i].GetPoint(0).X), (float)(result[i].GetPoint(0).Y), (float)(result[i].GetPoint(0).Z));
				Gl.glVertex3f((float)(result[i].GetPoint(3).X), (float)(result[i].GetPoint(3).Y), (float)(result[i].GetPoint(3).Z));
				Gl.glVertex3f((float)(result[i].GetPoint(1).X), (float)(result[i].GetPoint(1).Y), (float)(result[i].GetPoint(1).Z));
				Gl.glEnd();
				Gl.glBegin(Gl.GL_LINES);
				Gl.glColor4f(0f, 1f, 1f, 1f);
				Gl.glVertex3f((float)(result[i].GetPoint(2).X), (float)(result[i].GetPoint(2).Y), (float)(result[i].GetPoint(2).Z));
				Gl.glVertex3f((float)(result[i].GetPoint(3).X), (float)(result[i].GetPoint(3).Y), (float)(result[i].GetPoint(3).Z));
				Gl.glEnd();
			}

			Wgl.wglSwapBuffers(glControl1.HDC);
		}

		private void button1_Click(object sender, EventArgs e)
		{
			timer1.Start();
		}

		private void timer1_Tick(object sender, EventArgs e)
		{
			if (!mesh.Complete)
			{
				mesh.NextStep();
				mesh.GetNormalizedMesh(out result);
				mesh.PrintXML();
				//	pictureBox1.Invalidate();
				Invalidate();
			}
			else
			{
				timer1.Stop();
				//	mesh.PrintXML();
			}
		}

		private void timer2_Tick(object sender, EventArgs e)
		{
			rotation += 1;
			Invalidate();
		}

		private void button2_Click(object sender, EventArgs e)
		{
			mesh.NextStep();
			mesh.GetNormalizedMesh(out result);
			//	pictureBox1.Invalidate();
			Invalidate();
		}

		
	}
}
