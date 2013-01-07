using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MeshContainer;

namespace Mesher2D
{
	/// <summary>
	/// 2次元Delaunay分割の要素を表すクラス．
	/// </summary>
	public class DelaunayElement2d
    {
		/// <summary>
		/// Element2dクラスの新規インスタンスを初期化し，各頂点情報（座標，ID）をセットする．
		/// </summary>
		/// <param name="a"></param>
		/// <param name="b"></param>
		/// <param name="c"></param>
		public DelaunayElement2d(Node2D a, Node2D b, Node2D c)
		{
			p = new List<Node2D>() { a, b, c };
			edge = new List<Edge2D>() { new Edge2D(a.ID, b.ID), new Edge2D(b.ID, c.ID), new Edge2D(c.ID, a.ID) };
	//		l = new double[3] { 0, 0, 0 };
			length_sq = new double[3] { 0, 0, 0 };
			center = new Node2D(0.0, 0.0);
			radius = 0;
			SetSideLength();
			SetArea();
			SetCircumscribedCircle();
			DeleteFlag = false;
		}
		/// <summary>
		/// コピーコンストラクタ
		/// </summary>
		/// <param name="source">コピーされるobject</param>
		public DelaunayElement2d(DelaunayElement2d source)
		{
			p = new List<Node2D>() { source.p[0], source.p[1], source.p[2] };
			edge = new List<Edge2D>() { source.edge[0], source.edge[1], source.edge[2] };
	//		l = new double[3] { source.l[0], source.l[1], source.l[2] };
			length_sq = new double[3] { source.length_sq[0], source.length_sq[1], source.length_sq[2] };
			area = source.area;
			center = new Node2D(source.center);
			radius = source.radius;
			DeleteFlag = source.DeleteFlag;
		}
		[Obsolete("no need")]
		public static bool operator ==(DelaunayElement2d elem1, DelaunayElement2d elem2)
		{
			return	((elem1.p[0] == elem2.p[0]) && (elem1.p[1] == elem2.p[1]) && (elem1.p[2] == elem2.p[2])) ||
					((elem1.p[0] == elem2.p[1]) && (elem1.p[1] == elem2.p[2]) && (elem1.p[2] == elem2.p[0])) ||
					((elem1.p[0] == elem2.p[2]) && (elem1.p[1] == elem2.p[0]) && (elem1.p[2] == elem2.p[1]));
		}
		[Obsolete("no need",true)]
		public static bool operator !=(DelaunayElement2d elem1, DelaunayElement2d elem2)
		{
			return !(elem1 == elem2);
		}
		
		public override bool Equals(object obj)
		{
			return base.Equals(obj);
		}
		public override int GetHashCode()
		{
			return base.GetHashCode();
		}
		public static bool operator <(DelaunayElement2d elem1, DelaunayElement2d elem2)		//大小判定
		{
			return !(elem1.MinPoint() == elem2.MinPoint()) ? (elem1.MinPoint() < elem2.MinPoint()) :
					(!(elem1.MidPoint() == elem2.MidPoint()) ? (elem1.MidPoint() < elem2.MidPoint()) :
					(elem1.MaxPoint() < elem2.MaxPoint()));
		}
		public static bool operator >(DelaunayElement2d elem1, DelaunayElement2d elem2)
		{
			return !(elem1.MinPoint() == elem2.MinPoint()) ? (elem1.MinPoint() > elem2.MinPoint()) :
					(!(elem1.MidPoint() == elem2.MidPoint()) ? (elem1.MidPoint() > elem2.MidPoint()) :
					(elem1.MaxPoint() > elem2.MaxPoint()));
		}


		private List<Node2D> p;		// 三角形メッシュの3頂点をCCWの順に格納する．
		private List<Edge2D> edge;		// 三角形メッシュの3辺（3線分）をCCWの順に格納する．
	//	private double[] l;				// 辺の長さ
		private double[] length_sq;		// 辺の長さ
		private double area;			// 三角形メッシュの面積
		private Node2D center;			// 外接円の中心
		private double radius;			// 外接円半径
		private bool flag_delete;		// Delaunay分割の際，再分割メッシュの対象になっているかどうか．trueの時削除される．

		/// <summary>
		/// 再分割メッシュの対象になっているかどうか．trueの時削除される．
		/// </summary>
		public bool DeleteFlag { get { return flag_delete; } set { this.flag_delete = value; } }

		public Edge2D GetEdge(int a)
		{
			if (!(a < 0) && (a < edge.Count))
			{
				return new Edge2D(edge[a]);
			}
			else
			{
				return null;
			}
		}

		public Node2D GetPoint(int a)
		{
			if (!(a < 0) && (a < p.Count))
			{
				return new Node2D(p[a]);
			}
			else
			{
				return null;
			}
		}

		/// <summary>
		/// 与えられた点の座標が円の内部に含まれるかどうか判定する．
		/// </summary>
		/// <param name="p">点</param>
		/// <returns>判定結果</returns>
		public bool Inside(Node2D p)
		{
			double distance_sq = ((p.X - center.X) * (p.X - center.X) + (p.Y - center.Y) * (p.Y - center.Y));
			if (distance_sq < radius * radius)
			{
				return true;
			}
			else
			{
				return false;
			}
		}

		/// <summary>
		/// 要素に含まれる3ノードの座標をコンソールに出力する．
		/// </summary>
		public void Print()
		{
			Console.WriteLine("[{0}, {1}, {2}]", p[0].ToString(), p[1].ToString(), p[2].ToString());
		}

		/// <summary>
		/// 三角形メッシュの面積を求める．
		/// </summary>
		private void SetArea()
		{
			area = Math.Abs(0.5 * ((p[1].X - p[0].X) * (p[2].Y - p[0].Y) - (p[2].X - p[0].X) * (p[1].Y - p[0].Y)));
		}

		/// <summary>
		/// 三角形メッシュの3辺の長さを求める．
		/// </summary>
		private void SetSideLength()
		{
			length_sq[0] = (p[0].X - p[1].X) * (p[0].X - p[1].X) + (p[0].Y - p[1].Y) * (p[0].Y - p[1].Y);
			length_sq[1] = (p[1].X - p[2].X) * (p[1].X - p[2].X) + (p[1].Y - p[2].Y) * (p[1].Y - p[2].Y);
			length_sq[2] = (p[2].X - p[0].X) * (p[2].X - p[0].X) + (p[2].Y - p[0].Y) * (p[2].Y - p[0].Y);
		}

		/// <summary>
		/// 三角形メッシュの外接円を求め，center,radiusにセットする．
		/// </summary>
		private void SetCircumscribedCircle()
		{
			double temp_val0 = length_sq[1] * (length_sq[2] + length_sq[0] - length_sq[1]);
			double temp_val1 = length_sq[2] * (length_sq[0] + length_sq[1] - length_sq[2]);
			double temp_val2 = length_sq[0] * (length_sq[1] + length_sq[2] - length_sq[0]);

			center.X = (temp_val0 * p[0].X + temp_val1 * p[1].X + temp_val2 * p[2].X) / 16 / area / area;
			center.Y = (temp_val0 * p[0].Y + temp_val1 * p[1].Y + temp_val2 * p[2].Y) / 16 / area / area;
			radius = 0.25 * Math.Sqrt(length_sq[0] * length_sq[1] * length_sq[2]) / area;
		}

		/// <summary>
		/// 要素内の3ノードをノードIDで辞書順ソートした時の最小のノードを返す
		/// </summary>
		/// <returns>最小ノード</returns>
		public Node2D MinPoint()
		{
			return ((p[0] < p[1]) && (p[0] < p[2])) ? p[0] : ((p[1] < p[2]) ? p[1] : p[2]);
		}
		/// <summary>
		/// 要素内の3ノードをノードIDで辞書順ソートした時の真ん中のノードを返す
		/// </summary>
		/// <returns>真ん中のノード</returns>
		private Node2D MidPoint()
		{
			return (((p[0] < p[1]) && (p[1] < p[2])) || ((p[2] < p[1]) && (p[1] < p[0]))) ? p[1] :
					((((p[0] < p[2]) && (p[2] < p[1])) || ((p[1] < p[2]) && (p[2] < p[0]))) ? p[2] : p[0]);
		}
		/// <summary>
		/// 要素内の3ノードをノードIDで辞書順ソートした時の最大のノードを返す
		/// </summary>
		/// <returns>最大のノード</returns>
		private Node2D MaxPoint()
		{
			return ((p[1] < p[0]) && (p[2] < p[0])) ? p[0] : ((p[2] < p[1]) ? p[1] : p[2]);
		}
    }
}