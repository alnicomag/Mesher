using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;

using Tao.OpenGl;
using Tao.Platform.Windows;

namespace GLPictureBox
{
	public partial class GLPictureBox : UserControl
	{
		public GLPictureBox()
		{
			InitializeComponent();
		}

		IntPtr hRC;
		IntPtr hDC;

		public IntPtr HRC { get { return hRC; } }
		public IntPtr HDC { get { return hDC; } }

		protected override void OnHandleCreated(EventArgs e)
		{
			base.OnHandleCreated(e);

			this.SetStyle(ControlStyles.UserPaint, true);
			this.SetStyle(ControlStyles.AllPaintingInWmPaint, true);
			this.SetStyle(ControlStyles.DoubleBuffer, false);
			this.SetStyle(ControlStyles.Opaque, true);
			this.SetStyle(ControlStyles.ResizeRedraw, true);

			Gdi.PIXELFORMATDESCRIPTOR pfd = new Gdi.PIXELFORMATDESCRIPTOR();

            pfd.dwFlags = Gdi.PFD_SUPPORT_OPENGL | Gdi.PFD_DRAW_TO_WINDOW | Gdi.PFD_DOUBLEBUFFER;   // OpenGLをサポート | ウィンドウに描画 | ダブルバッファ
			pfd.iPixelType = Gdi.PFD_TYPE_RGBA; //RGBAフォーマット
			pfd.cColorBits = 32; // 32bit/pixel
			pfd.cAlphaBits = 8; // アルファチャンネル8bit (0にするとアルファチャンネル無しになる)
			pfd.cDepthBits = 16; // デプスバッファ16bit

            this.hDC = User.GetDC(this.Handle);     // デバイスコンテキストのハンドルを取得

            int pixFormat = Gdi.ChoosePixelFormat(this.hDC, ref pfd);   // ピクセルフォーマットを選択
            if (pixFormat <= 0)
            {
                throw new Exception("ChoosePixelFormat failed.");
            }

            Wgl.wglDescribePixelFormat(this.hDC, pixFormat, 40, ref pfd);   //ピクセルフォーマットの正確な設定を取得

            bool valid = Gdi.SetPixelFormat(this.hDC, pixFormat, ref pfd);  //デバイスコンテキストにピクセルフォーマットを設定
            if (!valid)
            {
                throw new Exception("SetPixelFormat failed");
            }

            this.hRC = Wgl.wglCreateContext(this.hDC);  //OpenGLのレンダリングコンテキストを作成
            if (this.hRC == IntPtr.Zero)
            {
                throw new Exception("wglCreateContext failed.");
            }
			
			Wgl.wglMakeCurrent(this.hDC, this.hRC);  //作成したコンテキストをカレントに設定

            //レンダリングコンテキストを作成、カレントに設定したら、１度だけこれを呼び出しておく
			//Tao.OpenGl.GL、Tao.Platform.Windows.WGLの仕様。
			Gl.ReloadFunctions();
			Wgl.ReloadFunctions();

			int err = Gl.glGetError();
			if (err != Gl.GL_NO_ERROR)
			{
				throw new Exception("Error code = " + err.ToString());
			}

			this.SetupGL();
		}


		/// <summary>
		/// レンダリングコンテキストを解放する。
		/// </summary>
		private void DeleteContext()
		{
			Wgl.wglDeleteContext(hRC);
		}


		private void SetupGL()
		{
			Gl.glClearColor(0f, 0f, 0f, 1f);

			Gl.glEnable(Gl.GL_BLEND);
			Gl.glBlendFunc(Gl.GL_SRC_ALPHA, Gl.GL_ONE_MINUS_SRC_ALPHA);

			Gl.glShadeModel(Gl.GL_SMOOTH);
		}


		protected override void OnLoad(System.EventArgs e)
		{

		}

		protected override void OnPaint(PaintEventArgs e)
		{
			
			Wgl.wglMakeCurrent(this.hDC, this.hRC);

			Gl.glClear(Gl.GL_COLOR_BUFFER_BIT);

			Gl.glMatrixMode(Gl.GL_PROJECTION);
			Gl.glLoadIdentity();
			Gl.glViewport(0, 0, this.ClientRectangle.Width, this.ClientRectangle.Height);

			Gl.glMatrixMode(Gl.GL_MODELVIEW);
			Gl.glLoadIdentity();
			Gl.glRotatef(20f, 0f, 0f, 1f);
			/*
			Gl.glBegin(Gl.GL_TRIANGLES);

			Gl.glColor3f(1f, 0f, 0f);
			Gl.glVertex2f(-0.8f, -0.6f);

			Gl.glColor3f(0f, 1f, 0f);
			Gl.glVertex2f(0.8f, -0.6f);

			Gl.glColor3f(0f, 0f, 1f);
			Gl.glVertex2f(0.0f, 0.8f);


			Gl.glColor4f(1f, 0f, 0f, 0.6f);
			Gl.glVertex2f(-0.8f, 0.6f);

			Gl.glColor4f(0f, 1f, 0f, 0.6f);
			Gl.glVertex2f(0.0f, -0.8f);

			Gl.glColor4f(0f, 0f, 1f, 0.6f);
			Gl.glVertex2f(0.8f, 0.6f);

			Gl.glEnd();
			*/
			Wgl.wglSwapBuffers(this.hDC);
			
		}

		private void GLControl_Load(object sender, System.EventArgs e)
		{

		}



	}
}
