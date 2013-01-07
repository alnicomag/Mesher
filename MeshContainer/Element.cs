using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace MeshContainer
{
	public struct Material
	{
		public Material(string name = "A2017", double density = 2800, double young = 6.9E10, double poisson = 0.34, double tr_elasticity = 2.6E10, double cte = 2.35E-5)
		{
			this.name = name;
			this.density = density;
			this.young = young;
			this.poisson = poisson;
			this.transverse_elasticity = tr_elasticity;
			this.cte = cte;
		}
		public string name;						//Material name
		public double density;					//Material density
		public double young;					//縦弾性係数
		public double poisson;					//ポアソン比
		public double transverse_elasticity;	//横弾性係数
		public double cte;						//線膨張係数
	}

	public enum ElementCode : int
	{
		Tria03 = 0,
		Tetra04,
		Tetra10,
		Hexa08,
		Hexa20,
		Hexa32,
		Hexa44
	}

	public class ElementParameter
	{
		public ElementParameter(ElementCode elementcode, int dim, int nodenum, int edgenum, int surfacenum, int d, int k)
		{
			ELEMENTCODE = elementcode;
			DIM = dim;
			NODENUM = nodenum;
			EDGENUM = edgenum;
			SURFACENUM = surfacenum;
			DMAT = new int[2] { d, d };
			BMAT = new int[2] { d, k };
			KMAT = new int[2] { k, k };
		}
		public readonly ElementCode ELEMENTCODE;
		public readonly int DIM;
		public readonly int NODENUM;
		public readonly int EDGENUM;
		public readonly int SURFACENUM;
		public readonly int[] DMAT;
		public readonly int[] BMAT;
		public readonly int[] KMAT;
	}

	public class ElementList
	{
		static ElementList()
		{
			Serendipity = new List<ElementParameter>();
			Serendipity.Add(new ElementParameter(ElementCode.Tria03, 2, 3, 3, 1, 3, 6));
			Serendipity.Add(new ElementParameter(ElementCode.Tetra04, 3, 4, 6, 4, 6, 12));
			Serendipity.Add(new ElementParameter(ElementCode.Tetra10, 3, 10, 6, 4, 6, 30));
			Serendipity.Add(new ElementParameter(ElementCode.Hexa08, 3, 8, 12, 6, 6, 24));
			Serendipity.Add(new ElementParameter(ElementCode.Hexa20, 3, 20, 12, 6, 6, 60));
			Serendipity.Add(new ElementParameter(ElementCode.Hexa32, 3, 32, 12, 6, 6, 96));
			Serendipity.Add(new ElementParameter(ElementCode.Hexa44, 3, 44, 12, 6, 6, 132));
		}
		public static List<ElementParameter> Serendipity;
	}

	public class Element
	{
		public Element()
		{

		}

		public Element(ElementCode elementcode, Material material)
		{
			this.elementcode = elementcode;
			this.material = material;
			ElementParameter ElePara = ElementList.Serendipity.Find(l => l.ELEMENTCODE == elementcode);
		}

		public List<Node> Nodes = new List<Node>();			//要素に含まれる節点
		public List<Edge> Edges = new List<Edge>();			//要素に含まれる稜

		/// <summary>
		/// 要素コード．使用している要素の種類を指定する名称．
		/// </summary>
		public ElementCode Elementcode { get { return this.elementcode; } protected set { this.elementcode = value; } }

		/// <summary>
		/// 材料パラメータ
		/// </summary>
		public Material Material { get { return this.material; } private set { this.material = value; } }

		private ElementCode elementcode;	//Element code
		private Material material;			//材料
	}
	

	/// <summary>
	/// FEMにおいて2次元直交座標系での多角形（要素）を表すクラス．
	/// </summary>
	public class Element2D
	{
		public Element2D()
		{
		}
		
		public Element2D(ElementCode elementcode, Material material, params Node[] nodes)
		{
			this.elementcode = elementcode;
			this.material = material;
			ElementParameter ElePara = ElementList.Serendipity.Find(l => l.ELEMENTCODE == elementcode);

			if ((ElePara.DIM == 2) & (ElePara.NODENUM == nodes.Length))
			{
				for (int i = 0; i < ElePara.NODENUM; ++i)
				{
					Nodes.Add(nodes[i]);
				}
				for (int i = 0; i < ElePara.NODENUM; ++i)
				{
					Edges.Add(new Edge(nodes[i].ID, nodes[(i + 1) % ElePara.NODENUM].ID));
				}
			}
			else
			{
				throw new FormatException();
			}
		}

		/// <summary>
		/// 要素に含まれる節点の座標列を等価な文字列に変換する．
		/// </summary>
		/// <returns></returns>
		public override string ToString()
		{
			string tmp = "[";
			foreach (Node i in Nodes)
			{
				tmp += (i.ToString() + ", ");
			}
			tmp.Remove(tmp.Length - 2);
			tmp += "]";

			return tmp;
		}

		public List<Node> Nodes = new List<Node>();			//要素に含まれる節点
		public List<Edge> Edges = new List<Edge>();			//要素に含まれる稜

		/// <summary>
		/// 要素コード．使用している要素の種類を指定する名称．
		/// </summary>
		public ElementCode Elementcode { get { return this.elementcode; } protected set { this.elementcode = value; } }

		/// <summary>
		/// 材料パラメータ
		/// </summary>
		public Material Material { get { return this.material; } private set { this.material = value; } }

		public double Area { get { return this.area; } protected set { this.area = value; } }

		public Edge GetEdge(int a)
		{
			if ((0 <= a) && (a < Edges.Count))
			{
				return new Edge(Edges[a]);
			}
			else
			{
				throw new IndexOutOfRangeException();
			}
		}

		public Node GetPoint(int a)
		{
			if ((0 <= a) && (a < Nodes.Count))
			{
				return new Node(Nodes[a]);
			}
			else
			{
				throw new IndexOutOfRangeException();
			}
		}

		/// <summary>
		/// 要素に含まれる節点のうち，最も節点番号の小さいものを探索し，その番号を返す．
		/// </summary>
		/// <returns>最小節点番号</returns>
		public int MinNodeID()
		{
			int temp_min = Nodes[0].ID;
			for (int i = 1; i < Nodes.Count; i++)
			{
				if (Nodes[i].ID < temp_min)
				{
					temp_min = Nodes[i].ID;
				}
			}
			return temp_min;
		}

		/// <summary>
		/// 要素に含まれる節点のうち，最も節点番号の大きいものを探索し，その番号を返す．
		/// </summary>
		/// <returns>最大節点番号</returns>
		public int MaxNodeID()
		{
			int temp_max = Nodes[0].ID;
			for (int i = 1; i < Nodes.Count; i++)
			{
				if (Nodes[i].ID > temp_max)
				{
					temp_max = Nodes[i].ID;
				}
			}
			return temp_max;
		}
		
		private ElementCode elementcode;	//Element code
		private Material material;			//材料
		private double area;				// 要素の面積
		private double[] volume_force;		//体積力荷重3成分

		private double CalcArea()		// メッシュの面積
		{
			return 0; 
		}
	}

	/// <summary>
	/// 2次元Delaunay分割の要素を表すクラス．
	/// </summary>
	public class DelaunayElement2D : Element2D
	{
		/// <summary>
		/// DelaunayElement2Dクラスの新規インスタンスを初期化し，各頂点情報（座標，ID）をセットする．
		/// </summary>
		/// <param name="a"></param>
		/// <param name="b"></param>
		/// <param name="c"></param>
		public DelaunayElement2D(Node a, Node b, Node c)
		{
			Elementcode = ElementCode.Tria03;
			Nodes = new List<Node>() { a, b, c };
			Edges = new List<Edge>() { new Edge(a.ID, b.ID), new Edge(b.ID, c.ID), new Edge(c.ID, a.ID) };
			
			center = new Node(-256, false, 0.0, 0.0);
			radius = 0;
			SetSideLength();
			Area = CalcArea();
			SetCircumscribedCircle();
			DeleteFlag = false;
		}
		/// <summary>
		/// コピーコンストラクタ
		/// </summary>
		/// <param name="source">コピーされるobject</param>
		public DelaunayElement2D(DelaunayElement2D source)
		{
			Elementcode = source.Elementcode;
			Nodes = new List<Node>() { source.Nodes[0], source.Nodes[1], source.Nodes[2] };
			Edges = new List<Edge>() { source.Edges[0], source.Edges[1], source.Edges[2] };
			Area = source.Area;
			center = new Node(source.center);
			radius = source.radius;
			DeleteFlag = source.DeleteFlag;
		}

		public static bool operator ==(DelaunayElement2D elem1, DelaunayElement2D elem2)
		{
			return ((elem1.Nodes[0] == elem2.Nodes[0]) && (elem1.Nodes[1] == elem2.Nodes[1]) && (elem1.Nodes[2] == elem2.Nodes[2])) ||
					((elem1.Nodes[0] == elem2.Nodes[1]) && (elem1.Nodes[1] == elem2.Nodes[2]) && (elem1.Nodes[2] == elem2.Nodes[0])) ||
					((elem1.Nodes[0] == elem2.Nodes[2]) && (elem1.Nodes[1] == elem2.Nodes[0]) && (elem1.Nodes[2] == elem2.Nodes[1]));
		}
		public static bool operator !=(DelaunayElement2D elem1, DelaunayElement2D elem2)
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

		public static bool operator <(DelaunayElement2D elem1, DelaunayElement2D elem2)		//大小判定
		{
			return !(elem1.MinPoint() == elem2.MinPoint()) ? (elem1.MinPoint() < elem2.MinPoint()) :
					(!(elem1.MidPoint() == elem2.MidPoint()) ? (elem1.MidPoint() < elem2.MidPoint()) :
					(elem1.MaxPoint() < elem2.MaxPoint()));
		}
		public static bool operator >(DelaunayElement2D elem1, DelaunayElement2D elem2)
		{
			return !(elem1.MinPoint() == elem2.MinPoint()) ? (elem1.MinPoint() > elem2.MinPoint()) :
					(!(elem1.MidPoint() == elem2.MidPoint()) ? (elem1.MidPoint() > elem2.MidPoint()) :
					(elem1.MaxPoint() > elem2.MaxPoint()));
		}

		private Node center;			// 外接円の中心
		private double radius;			// 外接円半径
		private bool flag_delete;		// Delaunay分割の際，再分割メッシュの対象になっているかどうか．trueの時削除される．

		/// <summary>
		/// 再分割メッシュの対象になっているかどうか．trueの時削除される．
		/// </summary>
		public bool DeleteFlag { get { return flag_delete; } set { this.flag_delete = value; } }

		/// <summary>
		/// 与えられた点の座標が円の内部に含まれるかどうか判定する．
		/// </summary>
		/// <param name="p">点</param>
		/// <returns>判定結果</returns>
		public bool IsInside(Node p)
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

		private double CalcArea()		// 三角形メッシュの面積を求める．
		{
			return Math.Abs	(0.5 *	(
										(Nodes[1].X - Nodes[0].X) * (Nodes[2].Y - Nodes[0].Y)
										- (Nodes[2].X - Nodes[0].X) * (Nodes[1].Y - Nodes[0].Y)
									)
							);
		}

		private void SetSideLength()		// 三角形メッシュの3辺の長さを求める．
		{
			Edges[0].Length = (Nodes[0].X - Nodes[1].X) * (Nodes[0].X - Nodes[1].X) + (Nodes[0].Y - Nodes[1].Y) * (Nodes[0].Y - Nodes[1].Y);
			Edges[1].Length = (Nodes[1].X - Nodes[2].X) * (Nodes[1].X - Nodes[2].X) + (Nodes[1].Y - Nodes[2].Y) * (Nodes[1].Y - Nodes[2].Y);
			Edges[2].Length = (Nodes[2].X - Nodes[0].X) * (Nodes[2].X - Nodes[0].X) + (Nodes[2].Y - Nodes[0].Y) * (Nodes[2].Y - Nodes[0].Y);
		}

		private void SetCircumscribedCircle()		// 三角形メッシュの外接円を求め，center,radiusにセットする．
		{
			double temp_val0 = Edges[1].Length * (Edges[2].Length + Edges[0].Length - Edges[1].Length);
			double temp_val1 = Edges[2].Length * (Edges[0].Length + Edges[1].Length - Edges[2].Length);
			double temp_val2 = Edges[0].Length * (Edges[1].Length + Edges[2].Length - Edges[0].Length);

			center.X = (temp_val0 * Nodes[0].X + temp_val1 * Nodes[1].X + temp_val2 * Nodes[2].X) / 16 / Area / Area;
			center.Y = (temp_val0 * Nodes[0].Y + temp_val1 * Nodes[1].Y + temp_val2 * Nodes[2].Y) / 16 / Area / Area;
			radius = 0.25 * Math.Sqrt(Edges[0].Length * Edges[1].Length * Edges[2].Length) / Area;
		}

		/// <summary>
		/// 要素内の3ノードをノードIDで辞書順ソートした時の最小のノードを返す
		/// </summary>
		/// <returns>最小ノード</returns>
		public Node MinPoint()
		{
			return ((Nodes[0] < Nodes[1]) && (Nodes[0] < Nodes[2])) ? Nodes[0] : ((Nodes[1] < Nodes[2]) ? Nodes[1] : Nodes[2]);
		}
		/// <summary>
		/// 要素内の3ノードをノードIDで辞書順ソートした時の真ん中のノードを返す
		/// </summary>
		/// <returns>真ん中のノード</returns>
		private Node MidPoint()
		{
			return (((Nodes[0] < Nodes[1]) && (Nodes[1] < Nodes[2])) || ((Nodes[2] < Nodes[1]) && (Nodes[1] < Nodes[0]))) ? Nodes[1] :
					((((Nodes[0] < Nodes[2]) && (Nodes[2] < Nodes[1])) || ((Nodes[1] < Nodes[2]) && (Nodes[2] < Nodes[0]))) ? Nodes[2] : Nodes[0]);
		}
		/// <summary>
		/// 要素内の3ノードをノードIDで辞書順ソートした時の最大のノードを返す
		/// </summary>
		/// <returns>最大のノード</returns>
		private Node MaxPoint()
		{
			return ((Nodes[1] < Nodes[0]) && (Nodes[2] < Nodes[0])) ? Nodes[0] : ((Nodes[2] < Nodes[1]) ? Nodes[1] : Nodes[2]);
		}
	}

	public class Element3D
	{
		/// <summary>
		/// Element3Dクラスの新規インスタンスを初期化する．
		/// </summary>
		public Element3D()
		{

		}

		public Element3D(Node a, Node b, Node c, Node d)
		{
			Nodes = new List<Node>() { a, b, c, d };
			Edges = new List<Edge>();
			Surfaces = new List<Surface3D>() { new Surface3D(a.ID, b.ID, c.ID), new Surface3D(a.ID, d.ID, b.ID), new Surface3D(b.ID, d.ID, c.ID), new Surface3D(a.ID, c.ID, d.ID) };

            center = new Node(0, false, 0.0, 0.0, 0.0);
            SetCircumscribedSphere();
            DeleteFlag = false;
		}
        /// <summary>
        /// コピーコンストラクタ
        /// </summary>
        /// <param name="source"></param>
		public Element3D(Element3D source)
		{
			elementcode = source.elementcode;
			material = source.material;
			Nodes = new List<Node>();
			for (int i = 0; i < source.Nodes.Count; ++i)
			{
				Nodes.Add(new Node(source.Nodes[i]));
			}
			Edges = new List<Edge>();
			for (int i = 0; i < source.Edges.Count; ++i)
			{
				Edges.Add(new Edge(source.Edges[i]));
			}
			Surfaces = new List<Surface3D>();
			for (int i = 0; i < source.Surfaces.Count; ++i)
			{
				Surfaces.Add(new Surface3D(source.Surfaces[i]));
			}
			center = new Node(source.center);
			radius = source.radius;
			DeleteFlag = source.DeleteFlag;

		}

        public override bool Equals(object obj)
        {
            return base.Equals(obj);
        }
        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public static bool operator <(Element3D a, Element3D b)
        {
            return true;
        }
        public static bool operator >(Element3D a, Element3D b)
        {
            return false;
        }

		private string elementcode;		//Element code
		private Material material;		//材料

		private List<Node> Nodes;			//要素に含まれる節点
		private List<Edge> Edges;			//要素に含まれる稜
		private List<Surface3D> Surfaces;	//要素に含まれる面

		private double[] volume_force;		//体積力荷重3成分

//		private Circle CircumscribedSphere;
		private Node center;			// 外接球の中心
        private double radius = 0;			// 外接球半径
        public bool DeleteFlag { get; set; }        // Delaunay分割の際，再分割メッシュの対象になっているかどうか．trueの時削除される．

		/// <summary>
		/// 要素コード．使用している要素の種類を指定する名称．
		/// </summary>
		public string Elementcode { get { return this.elementcode; } private set { this.elementcode = value; } }
		/// <summary>
		/// 材料パラメータ
		/// </summary>
		public Material Material { get { return this.material; } private set { this.material = value; } }

        /// <summary>
        /// 要素に含まれる節点（Node3Dクラスインスタンス）を取得する．
        /// </summary>
        /// <param name="a">内部節点番号</param>
        /// <returns>節点</returns>
		public Node GetPoint(int a)
		{
			if ((0 <= a) && (a < Nodes.Count))
			{
				return new Node(Nodes[a]);
			}
			else
			{
				throw new IndexOutOfRangeException();
			}
		}

		public Edge GetEdge(int a)
		{
			if ((0 <= a) && (a < Edges.Count))
			{
				return new Edge(Edges[a]);
			}
			else
			{
				throw new IndexOutOfRangeException();
			}
		}

		public Surface3D GetSurface(int a)
		{
			if ((0 <= a) && (a < Surfaces.Count))
			{
				return new Surface3D(Surfaces[a]);
			}
			else
			{
				throw new IndexOutOfRangeException();
			}
		}

		/// <summary>
		/// （要素が四面体の場合のみ）与えられた点の座標が外接球の内部に含まれるかどうか判定する．
		/// </summary>
		/// <param name="p">点</param>
		/// <returns>判定結果</returns>
		public bool IsInside(Node p)
		{
			if (Nodes.Count == 4)
			{
				double distance_sq = ((p.X - center.X) * (p.X - center.X) + (p.Y - center.Y) * (p.Y - center.Y) + (p.Z - center.Z) * (p.Z - center.Z));
				if (distance_sq < radius * radius)
				{
					return true;
				}
				else
				{
					return false;
				}
			}
			else
			{
				return false;
			}
			
		}

		/// <summary>
		/// 要素に含まれるノードの座標をコンソールに出力する．
		/// </summary>
		public void Print()
		{
			if (Nodes.Count > 0)
			{
				Console.Write("[{0}", Nodes[0].ToString());
			}
			for (int i = 1; i < Nodes.Count; i++)
			{
				Console.Write(", {0}", Nodes[i].ToString());
			}
			Console.WriteLine("]");
		}

        /// <summary>
        /// （要素が四面体の場合のみ）四面体の外接円を求め，center,radiusにセットする．
        /// </summary>
        private void SetCircumscribedSphere()
        {
			if (Nodes.Count == 4)
			{
				var vector_y = new double[3]
                {
                    Nodes[0].X * Nodes[0].X + Nodes[0].Y * Nodes[0].Y + Nodes[0].Z * Nodes[0].Z - Nodes[1].X * Nodes[1].X - Nodes[1].Y * Nodes[1].Y - Nodes[1].Z * Nodes[1].Z,
                    Nodes[1].X * Nodes[1].X + Nodes[1].Y * Nodes[1].Y + Nodes[1].Z * Nodes[1].Z - Nodes[2].X * Nodes[2].X - Nodes[2].Y * Nodes[2].Y - Nodes[2].Z * Nodes[2].Z,
                    Nodes[2].X * Nodes[2].X + Nodes[2].Y * Nodes[2].Y + Nodes[2].Z * Nodes[2].Z - Nodes[3].X * Nodes[3].X - Nodes[3].Y * Nodes[3].Y - Nodes[3].Z * Nodes[3].Z
                };
				var mat_a = new double[3, 3]
                {   {Nodes[0].X-Nodes[1].X,Nodes[0].Y-Nodes[1].Y,Nodes[0].Z-Nodes[1].Z},
                    {Nodes[1].X-Nodes[2].X,Nodes[1].Y-Nodes[2].Y,Nodes[1].Z-Nodes[2].Z},
                    {Nodes[2].X-Nodes[3].X,Nodes[2].Y-Nodes[3].Y,Nodes[2].Z-Nodes[3].Z}
                };
				double[,] inv_mat_a;
				InverseMatrix(mat_a, out inv_mat_a);
				center.X = 0.5 * (inv_mat_a[0, 0] * vector_y[0] + inv_mat_a[0, 1] * vector_y[1] + inv_mat_a[0, 2] * vector_y[2]);
				center.Y = 0.5 * (inv_mat_a[1, 0] * vector_y[0] + inv_mat_a[1, 1] * vector_y[1] + inv_mat_a[1, 2] * vector_y[2]);
				center.Z = 0.5 * (inv_mat_a[2, 0] * vector_y[0] + inv_mat_a[2, 1] * vector_y[1] + inv_mat_a[2, 2] * vector_y[2]);

				radius = Math.Sqrt((center.X - Nodes[0].X) * (center.X - Nodes[0].X) + (center.Y - Nodes[0].Y) * (center.Y - Nodes[0].Y) + (center.Z - Nodes[0].Z) * (center.Z - Nodes[0].Z));
			}
			else
			{
			//	throw new 
			}
        }

		/// <summary>
		/// 要素に含まれる節点のうち，最も節点番号の小さいものを探索し，その番号を返す．
		/// </summary>
		/// <returns>最小節点番号</returns>
		public int MinNodeID()
		{
			int temp_min = Nodes[0].ID;
			for (int i = 1; i < Nodes.Count; i++)
			{
				if (Nodes[i].ID < temp_min)
				{
					temp_min = Nodes[i].ID;
				}
			}
			return temp_min;
		}

		/// <summary>
		/// 要素に含まれる節点のうち，最も節点番号の大きいものを探索し，その番号を返す．
		/// </summary>
		/// <returns>最大節点番号</returns>
		public int MaxNodeID()
		{
			int temp_max = Nodes[0].ID;
			for (int i = 1; i < Nodes.Count; i++)
			{
				if (Nodes[i].ID > temp_max)
				{
					temp_max = Nodes[i].ID;
				}
			}
			return temp_max;
		}

       
        private static bool InverseMatrix(double[,] m, out double[,] inv)
        {
            inv = new double[3, 3];
            if ((m.GetLength(0) == 3) && (m.GetLength(1) == 3))
            {
                double det = m[0, 0] * (m[1, 1] * m[2, 2] - m[1, 2] * m[2, 1]) + m[1, 0] * (m[2, 1] * m[0, 2] - m[2, 2] * m[0, 1]) + m[2, 0] * (m[0, 1] * m[1, 2] - m[0, 2] * m[1, 1]);
                for (int i = 0; i < 3; i++)
                {
                    for (int j = 0; j < 3; j++)
                    {
                        inv[i, j] = (m[(j + 1) % 3, (i + 1) % 3] * m[(j + 2) % 3, (i + 2) % 3] - m[(j + 1) % 3, (i + 2) % 3] * m[(j + 2) % 3, (i + 1) % 3]) / det;
                    }
                }
                return true;
            }
            else
            {
                return false;
            }
        }
        


	}
}
