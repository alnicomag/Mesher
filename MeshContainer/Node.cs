using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MeshContainer
{
	/// <summary>
	/// FEMにおける節点を表すクラス．
	/// </summary>
	public class Node
	{
		/// <summary>
		/// Nodeクラスの新規インスタンスを初期化する．
		/// </summary>
		public Node()
		{
			Dimension = 0;
		}
		/// <summary>
		/// Nodeクラスの新規インスタンスを初期化する．
		/// </summary>
		/// <param name="id"></param>
		/// <param name="onboundary"></param>
		/// <param name="x"></param>
		public Node(int id, bool onboundary, params double[] x)
		{
			Dimension = x.Length;
			ID = id;
			OnBoundary = onboundary;
			ConnectedNodeID = new List<int>();
			coordinates = new double[Dimension];
			rest = new bool[Dimension];
			point_force = new double[Dimension];
			point_force_exist = new bool[Dimension];
			for (int i = 0; i < Dimension; ++i)
			{
				coordinates[i] = x[i];
				rest[i] = false;
				point_force[i] = 0;
				point_force_exist[i] = false;
			}
		}
		/// <summary>
		/// コピーコンストラクタ
		/// </summary>
		/// <param name="source"></param>
		public Node(Node source)
		{
			Dimension = source.Dimension;
			ID = source.ID;
			OnBoundary = source.OnBoundary;
			ConnectedNodeID = new List<int>();
			for (int i = 0; i < source.ConnectedNodeID.Count; ++i)
			{
				ConnectedNodeID.Add(source.ConnectedNodeID[i]);
			}
			coordinates = new double[Dimension];
			rest = new bool[Dimension];
			point_force = new double[Dimension];
			point_force_exist = new bool[Dimension];
			for (int i = 0; i < Dimension; ++i)
			{
				coordinates[i] = source.coordinates[i];
				rest[i] = source.rest[i];
				point_force[i] = source.point_force[i];
				point_force_exist[i] = source.point_force_exist[i];
			}
		}

		/// <summary>
		/// 節点番号と節点座標を比較要素に選び，等号演算子をオーバーロードする．
		/// </summary>
		/// <param name="p1">オペランド1</param>
		/// <param name="p2">オペランド2</param>
		/// <returns>等号判定</returns>
		public static bool operator ==(Node p1, Node p2)
		{
			if (p1.Dimension != p2.Dimension)
			{
				return false;
			}
			if(!(p1.ID == p2.ID))
			{
				return false;
			}
			if (p1.OnBoundary != p2.OnBoundary)
			{
				return false;
			}

			//ConnectedNodeIDの比較コードをここに書く

			bool equal = true;
			for (int i = 0; i < p1.Dimension; ++i)
			{
				equal &= (p1[i] == p2[i]);
			}
			return equal;
		}
		/// <summary>
		/// 節点番号と節点座標を比較要素に選び，不等号演算子をオーバーロードする．
		/// </summary>
		/// <param name="p1">オペランド1</param>
		/// <param name="p2">オペランド2</param>
		/// <returns>不等号判定</returns>
		public static bool operator !=(Node p1, Node p2)
		{
			return !(p1.ID == p2.ID);
		}
		/// <summary>
		/// 等号演算子のオーバーロードに合わせてオーバーライドしたEqualsメソッド．
		/// </summary>
		/// <param name="obj">オペランド</param>
		/// <returns>等号判定</returns>
		public override bool Equals(object obj)
		{
			if (obj == null || this.GetType() != obj.GetType())		//オブジェクトがnullもしくは型が異なる場合は不等号判定とする．
			{
				return false;
			}
			Node c = (Node)obj;
			if (this.Dimension != c.Dimension)
			{
				return false;
			}
			if (!(this.ID == c.ID))
			{
				return false;
			}
			if (this.OnBoundary != c.OnBoundary)
			{
				return false;
			}

			//ConnectedNodeIDの比較コードをここに書く

			bool equal = true;
			for (int i = 0; i < this.Dimension; ++i)
			{
				equal &= (this[i] == c[i]);
			}
			return equal;
		}
		/// <summary>
		/// ハッシュコードを返す
		/// </summary>
		/// <returns>ハッシュコード</returns>
		public override int GetHashCode()
		{
			int hash = ID;
			for (int i = 0; i < Dimension; ++i)
			{
				hash ^= coordinates[i].GetHashCode();
			}
			return hash;
		}
		/// <summary>
		/// 節点番号を比較要素に選び，小なり演算子をオーバーロードする．
		/// </summary>
		/// <param name="p1">オペランド1</param>
		/// <param name="p2">オペランド2</param>
		/// <returns>少なり判定</returns>
		public static bool operator <(Node p1, Node p2)
		{
			return (p1.ID < p2.ID);
		}
		/// <summary>
		/// 節点番号を比較要素に選び，大なり演算子をオーバーロードする．
		/// </summary>
		/// <param name="p1">オペランド1</param>
		/// <param name="p2">オペランド2</param>
		/// <returns>大なり判定</returns>
		public static bool operator >(Node p1, Node p2)
		{
			return (p1.ID > p2.ID);
		}
		/// <summary>
		/// 自身の座標を（x1,x2,x3,...）の形式で表した文字列に変換する．
		/// </summary>
		/// <returns></returns>
		public override string ToString()
		{
			string tmp = "(";
			for (int i = 0; i < Dimension - 1; ++i)
			{
				tmp += (coordinates[i].ToString() + ", ");
			}
			tmp += (coordinates[Dimension - 1].ToString() + ")");
			return tmp;
		}

		/// <summary>
		/// ノード番号
		/// </summary>
		public int ID { get { return id; } set { this.id = value; } }
		/// <summary>
		/// 次元
		/// </summary>
		public int Dimension { get { return dimension; } private set { this.dimension = value; } }
		/// <summary>
		/// 境界上のノードかどうか
		/// </summary>
		public bool OnBoundary { get; set; }
		/// <summary>
		/// このNodeと接続されているNodeのIDのList
		/// </summary>
		public List<int> ConnectedNodeID;

		/// <summary>
		/// インデクサによる座標へのアクセス
		/// </summary>
		/// <param name="i">成分の指定</param>
		/// <returns>座標</returns>
		public double this[int i]
		{
			get
			{
#if DEBUG
				if (i >= Dimension)
				{
					throw new IndexOutOfRangeException();
				}
#endif
				return this.coordinates[i];
			}
			set
			{
#if DEBUG
				if (i >= Dimension)
				{
					throw new IndexOutOfRangeException();
				}
#endif
				this.coordinates[i] = value;
			}
		}

		/// <summary>
		/// x座標
		/// </summary>
		public double X
		{
			get
			{
#if DEBUG
				if (1 > Dimension)
				{
					throw new IndexOutOfRangeException();
				}
#endif
				return coordinates[0]; 
			} 
			set 
			{
#if DEBUG
				if (1 > Dimension)
				{
					throw new IndexOutOfRangeException();
				}
#endif
				coordinates[0] = value; 
			}
		}

		/// <summary>
		/// y座標
		/// </summary>
		public double Y
		{
			get
			{
#if DEBUG
				if (2 > Dimension)
				{
					throw new IndexOutOfRangeException();
				}
#endif
				return coordinates[1];
			}
			set
			{
#if DEBUG
				if (2 > Dimension)
				{
					throw new IndexOutOfRangeException();
				}
#endif
				coordinates[1] = value;
			}
		}

		/// <summary>
		/// z座標
		/// </summary>
		public double Z
		{
			get
			{
#if DEBUG
				if (3 > Dimension)
				{
					throw new IndexOutOfRangeException();
				}
#endif
				return coordinates[2];
			}
			set
			{
#if DEBUG
				if (3 > Dimension)
				{
					throw new IndexOutOfRangeException();
				}
#endif
				coordinates[2] = value;
			}
		}

		

		private int dimension;					//次元
		private int id;							//節点ID
		
		/// <summary>
		/// 節点座標
		/// </summary>
		protected double[] coordinates;
		/// <summary>
		/// 節点が拘束されているかどうか
		/// </summary>
		protected bool[] rest;
		protected bool[] point_force_exist;		//節点への点荷重が存在するかどうか
		/// <summary>
		/// 節点に作用する点荷重
		/// </summary>
		protected double[] point_force;
	}

	/*
	/// <summary>
	/// FEMにおいて2次元直交座標系での節点を表すクラス．
	/// Delaunay分割アルゴリズムを用いた要素分割にも対応．
	/// </summary>
	[Obsolete]
	public class Node2D : Node
	{
		/// <summary>
		/// Node2Dクラスの新規インスタンスを初期化する．
		/// </summary>
		/// <param name="id">節点番号</param>
		/// <param name="x">x座標</param>
		/// <param name="y">y座標</param>
		public Node2D(double x=0.0, double y=0.0, int id=-256)
			: base(id, false, x, y)
		{
		}
		/// <summary>
		/// コピーコンストラクタ
		/// </summary>
		/// <param name="source">コピーされるインスタンス</param>
		public Node2D(Node2D source)
		{
			ID = source.ID;
			coordinates = new double[] { source.X, source.Y };
			rest = new bool[] { source.rest[0], source.rest[1] };
			point_force = new double[] { source.point_force[0], source.point_force[1] };
			point_force_exist = new bool[] { source.point_force_exist[0], source.point_force_exist[1] };
		}

		/// <summary>
		/// 節点番号と節点座標を比較要素に選び，等号演算子をオーバーロードする．
		/// </summary>
		/// <param name="p1">オペランド1</param>
		/// <param name="p2">オペランド2</param>
		/// <returns>等号判定</returns>
		public static bool operator ==(Node2D p1, Node2D p2)
		{
			return ((p1.ID == p2.ID) && (p1.X == p2.X) && (p1.Y == p2.Y));
		}
		/// <summary>
		/// 節点番号と節点座標を比較要素に選び，不等号演算子をオーバーロードする．
		/// </summary>
		/// <param name="p1">オペランド1</param>
		/// <param name="p2">オペランド2</param>
		/// <returns>不等号判定</returns>
		public static bool operator !=(Node2D p1, Node2D p2)
		{
			return !(p1.ID == p2.ID);
		}
		/// <summary>
		/// 等号演算子のオーバーロードに合わせてオーバーライドしたEqualsメソッド．
		/// </summary>
		/// <param name="obj">オペランド</param>
		/// <returns>等号判定</returns>
		public override bool Equals(object obj)
		{
			if (obj == null || this.GetType() != obj.GetType())		//オブジェクトがnullもしくは型が異なる場合は不等号判定とする．
			{
				return false;
			}
			Node2D c = (Node2D)obj;
			return ((this.ID == c.ID) && (this.X == c.X) && (this.Y == c.Y));
		}
		/// <summary>
		/// ハッシュコードを返す
		/// </summary>
		/// <returns>ハッシュコード</returns>
		public override int GetHashCode()
		{
			return ID ^ X.GetHashCode() ^ Y.GetHashCode();
		}
		/// <summary>
		/// 節点番号と節点座標を比較要素に選び，小なり演算子をオーバーロードする．
		/// </summary>
		/// <param name="p1">オペランド1</param>
		/// <param name="p2">オペランド2</param>
		/// <returns>少なり判定</returns>
		public static bool operator <(Node2D p1, Node2D p2)
		{
			return (p1.ID < p2.ID);
		//	return !(p1.ID == p2.ID) ? (p1.ID < p2.ID) : (!(p1.X == p2.X) ? (p1.X < p2.X) : (p1.Y < p2.Y));
		}
		/// <summary>
		/// 節点番号と節点座標を比較要素に選び，大なり演算子をオーバーロードする．
		/// </summary>
		/// <param name="p1">オペランド1</param>
		/// <param name="p2">オペランド2</param>
		/// <returns>大なり判定</returns>
		public static bool operator >(Node2D p1, Node2D p2)
		{
			return (p1.ID > p2.ID);
		}
	}

	/// <summary>
	/// FEMにおいて3次元直交座標系での節点を表すクラス．
	/// Delaunay分割アルゴリズムを用いた要素分割にも対応．
	/// </summary>
	[Obsolete]
	public class Node3D : Node
	{
		/// <summary>
		/// Node3Dクラスの新規インスタンスを初期化する．
		/// </summary>
		public Node3D()
			: base(-256, false, 0.0, 0.0, 0.0)
		{
		}
		/// <summary>
		/// Node3Dクラスの新規インスタンスを初期化し，(x,y,z)座標をセットする．
		/// </summary>
		/// <param name="x">x座標</param>
		/// <param name="y">y座標</param>
		/// <param name="z">z座標</param>
		public Node3D(double x, double y, double z)
			: base(-256, false, x, y, z)
		{
		}
		/// <summary>
		/// Node3Dクラスの新規インスタンスを初期化し，節点番号と(x,y,z)座標をセットする．
		/// </summary>
		/// <param name="id">節点番号</param>
		/// <param name="x">x座標</param>
		/// <param name="y">y座標</param>
		/// <param name="z">z座標</param>
		public Node3D(int id, double x, double y, double z)
			: base(id, false, x, y, z)
		{
		}
		/// <summary>
		/// コピーコンストラクタ
		/// </summary>
		/// <param name="source">コピーされるインスタンス</param>
		public Node3D(Node3D source)
		{
			ID = source.ID;
			coordinates = new double[] { source.X, source.Y, source.Z };
			rest = new bool[] { source.rest[0], source.rest[1], source.rest[2] };
			point_force = new double[] { source.point_force[0], source.point_force[1], source.point_force[2] };
			point_force_exist = new bool[] { source.point_force_exist[0], source.point_force_exist[1], source.point_force_exist[2] };
		}

		/// <summary>
		/// 節点番号と節点座標を比較要素に選び，等号演算子をオーバーロードする．
		/// </summary>
		/// <param name="p1">オペランド1</param>
		/// <param name="p2">オペランド2</param>
		/// <returns>等号判定</returns>
		public static bool operator ==(Node3D p1, Node3D p2)
		{
			return ((p1.ID == p2.ID) && (p1.X == p2.X) && (p1.Y == p2.Y) && (p1.Z == p2.Z));
		}
		/// <summary>
		/// 節点番号と節点座標を比較要素に選び，不等号演算子をオーバーロードする．
		/// </summary>
		/// <param name="p1">オペランド1</param>
		/// <param name="p2">オペランド2</param>
		/// <returns>不等号判定</returns>
		public static bool operator !=(Node3D p1, Node3D p2)
		{
			return !(p1.ID == p2.ID);
		}
		/// <summary>
		/// 等号演算子のオーバーロードに合わせてオーバーライドしたEqualsメソッド．
		/// </summary>
		/// <param name="obj">オペランド</param>
		/// <returns>等号判定</returns>
		public override bool Equals(object obj)
		{
			if (obj == null || this.GetType() != obj.GetType())		//オブジェクトがnullもしくは型が異なる場合は不等号判定とする．
			{
				return false;
			}
			Node3D c = (Node3D)obj;
			return ((this.ID == c.ID) && (this.X == c.X) && (this.Y == c.Y) && (this.Z == c.Z));
		}
		/// <summary>
		/// ハッシュコードを返す
		/// </summary>
		/// <returns>ハッシュコード</returns>
		public override int GetHashCode()
		{
			return ID ^ X.GetHashCode() ^ Y.GetHashCode() ^ Z.GetHashCode();
		}
		/// <summary>
		/// 節点番号と節点座標を比較要素に選び，小なり演算子をオーバーロードする．
		/// </summary>
		/// <param name="p1">オペランド1</param>
		/// <param name="p2">オペランド2</param>
		/// <returns>少なり判定</returns>
		public static bool operator <(Node3D p1, Node3D p2)
		{
			return (p1.ID < p2.ID);
		//	return !(p1.ID == p2.ID) ? (p1.ID < p2.ID) : (!(p1.X == p2.X) ? (p1.X < p2.X) : (!(p1.Y == p2.Y) ? (p1.Y < p2.Y) : (p1.Z < p2.Z)));
		}
		/// <summary>
		/// 節点番号と節点座標を比較要素に選び，大なり演算子をオーバーロードする．
		/// </summary>
		/// <param name="p1">オペランド1</param>
		/// <param name="p2">オペランド2</param>
		/// <returns>大なり判定</returns>
		public static bool operator >(Node3D p1, Node3D p2)
		{
			return (p1.ID > p2.ID);
			//return (!(p1.ID < p2.ID) && !(p1.ID == p2.ID));
		}
		

	}
	 * */
}
