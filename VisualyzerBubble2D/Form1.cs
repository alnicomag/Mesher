using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

using MeshContainer;
using Mesher2D;
using Geometry;

using Tao.OpenGl;
using Tao.Platform.Windows;

namespace VisualyzerBubble2D
{
	public partial class Form1 : Form
	{
		public Form1()
		{
			InitializeComponent();
		}
		
		BubbleMesh2D samplebubble;
		Random rnd = new Random();

		private void Form1_Load(object sender, EventArgs e)
		{
			timer1.Interval = 5;

			FigurePath path1 = new FigurePath(new Point2D(0.0, 0.0));
			path1.AddPoint(new Point2D(0.0, 0.3));
			path1.AddPoint(new Point2D(0.2, 0.8));
			path1.AddPoint(new Point2D(0.6, 0.5));
			path1.AddPoint(new Point2D(0.9, 1.0));
			path1.AddPoint(new Point2D(1.0, 0.7));
			path1.AddPoint(new Point2D(1.0, 0.3));
			path1.AddPoint(new Point2D(0.9, 0.2));
			path1.AddPoint(new Point2D(0.5, 0.0));
			samplebubble = new BubbleMesh2D(path1, 0.05);
			samplebubble.MakeBubble();
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

			for (int i = 0; i < samplebubble.Count; i++)
			{
				Gl.glBegin(Gl.GL_LINE_LOOP);

				Gl.glColor4f(0f, 1f, 1f, 1f);

				int resol=20;
				for (int j = 0; j < resol; ++j)
				{
					Gl.glVertex2f((float)(samplebubble[i][0] + samplebubble[i].Diam * 0.5 * Math.Cos(2.0 * Math.PI * j / resol)), (float)(samplebubble[i][1] + samplebubble[i].Diam * 0.5 * Math.Sin(2.0 * Math.PI * j / resol)));
				}
				
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
			
			double x = rnd.NextDouble();
			double y = rnd.NextDouble();
			Point2D newpoint = new Point2D(x, y);
			if (samplebubble.GetPath().IsInside(newpoint) == true)
			{
				samplebubble.AddBubble(new Bubble(false, x,y) { Diam = 0.05 });
			}
			if (samplebubble.Count >= 200)
			{
				timer1.Stop();
			}
			Invalidate();
		}

		private void Form1_Paint(object sender, PaintEventArgs e)
		{
			Invalidate();
		}

		
	}
}
