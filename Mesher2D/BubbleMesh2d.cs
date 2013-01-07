using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Serialization;
using System.IO;

using Geometry;

namespace Mesher2D
{
	/// <summary>
	/// バブルメッシュ法におけるノード粒子を表すクラス
	/// </summary>
    public class Bubble : Point
    {
        /// <summary>
        /// 中心座標配列及び境界上のバブルかどうかを元にBubbleクラスを初期化する．
        /// </summary>
        /// <param name="onboundary"></param>
        /// <param name="x"></param>
        public Bubble(bool onboundary, params double[] x)
            : base(x)
        {
            OnBoundary = onboundary;
        }
        /// <summary>
        /// Pointクラスによる中心座標及び境界上のバブルかどうかを元にBubbleクラスを初期化する．
        /// </summary>
        /// <param name="onboundary"></param>
        /// <param name="p"></param>
        public Bubble(bool onboundary, Point p)
            : base(p)
        {
            OnBoundary = onboundary;
        }
        /// <summary>
        /// コピーコンストラクタ
        /// </summary>
        /// <param name="source"></param>
        public Bubble(Bubble source)
            : base(source.x)
        {
            OnBoundary = source.OnBoundary;
            Diam = source.Diam;
            M = source.M;
            C = source.C;
        }

        public bool OnBoundary { get; private set; }
        public double Diam { get; set; }
        public double M { get { return m; } set { this.m = value; } }
        public double C { get { return c; } set { this.c = value; } }

        private double m;
        private double c;
    }

	/// <summary>
	/// 2次元バブルメッシュ法を提供するクラス
	/// </summary>
	public class BubbleMesh2D
	{
		/// <summary>
		/// FigurePathによる領域指定と基準バブルサイズによりBubbleMesh2Dクラスを初期化する．
		/// </summary>
		/// <param name="path"></param>
		/// <param name="size"></param>
		public BubbleMesh2D(FigurePath path, double size)
		{
			this.path = path;
			standard_diameter = size;
			bubbles = new List<Bubble>();
		}

		/// <summary>
		/// インデクサによるBubbleMesh2Dに格納されているBubbleへのアクセス．
		/// </summary>
		/// <param name="i"></param>
		/// <returns></returns>
		public Bubble this[int i]
		{
            get
            {
                if (i < Count) { return new Bubble(bubbles[i]); }
                else { throw new IndexOutOfRangeException(); }
            }
		}

		[Obsolete]
		public Bubble GetBubble(int i)
		{
			return bubbles[i];
		}
		
		/// <summary>
		/// BubbleMesh2dに格納されているBubbleの数を取得する．
		/// </summary>
		public int Count { get { return bubbles.Count; } }

		public FigurePath GetPath()
		{
			return path;
		}

		public void AddBubble(Bubble bbl)
		{
			bubbles.Add(bbl);
		}

		public void MakeBubble()
		{
			SetBoundaryBubble();
		}

		private FigurePath path;
		private List<Bubble> bubbles;
		private double standard_diameter;		//メッシュサイズ指標

		private void SetBoundaryBubble()
		{
			for (int i = 0; i < path.Count; ++i)
			{
				int div = (int)(path[i].Length / standard_diameter);
				for (int j = 0; j < div; ++j)
				{
					double x = path[i][0].X + (path[i][1].X - path[i][0].X) * j / div;
					double y = path[i][0].Y + (path[i][1].Y - path[i][0].Y) * j / div;
					bubbles.Add(new Bubble(true, x, y) { Diam = standard_diameter });
				}
			}
		}

		private double LennardJonesPotential(double r, double r0, double e, double p = 12, double q = 6)
		{
			double sigma = r0 * Math.Pow(q / p, 1 / (p - q));
			double repulsion = Math.Pow(sigma / r, p);
			double attraction = Math.Pow(sigma / r, q);
			return 4 * e * (repulsion - attraction);
		}

		private double LennardJonesForce(double r, double r0, double e, double p = 12, double q = 6)
		{
			double sigma = r0 * Math.Pow(q / p, 1 / (p - q));
			double repulsion = p / r * Math.Pow(sigma / r, p);
			double attraction = q / r * Math.Pow(sigma / r, q);
			return 4 * e * (repulsion - attraction);
		}
	}
}