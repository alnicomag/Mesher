using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MeshContainer
{
	/// <summary>
	/// FEMにおけるエッジを表すクラス．
	/// Sort,BinarySearchの為にIComparableインターフェースを継承する．
	/// </summary>
	public class Edge : IComparable
	{
		public Edge(int start = -256, int end = -255, int id = -256)
		{
			ID = id;
			StartNodeId = start;
			EndNodeId = end;
			edge_force = new List<double>();
		}
		public Edge(Edge source)
		{
			ID = source.ID;
			StartNodeId = source.StartNodeId;
			EndNodeId = source.EndNodeId;
			Length = source.Length;
			edge_force = new List<double>();
			for (int i = 0; i < source.edge_force.Count; i++)
			{
				edge_force.Add(source.edge_force[i]);
			}
		}

		/// <summary>
		/// 同じエッジか判定（向きは問わない）
		/// </summary>
		/// <param name="a"></param>
		/// <param name="b"></param>
		/// <returns></returns>
		public static bool operator ==(Edge a, Edge b)
		{
			return ((a.StartNodeId == b.StartNodeId) && (a.EndNodeId == b.EndNodeId) || (a.StartNodeId == b.EndNodeId) && (a.EndNodeId == b.StartNodeId));
		}
		public static bool operator !=(Edge a, Edge b)	//同じエッジか判定（向きは問わない）
		{
			return !(a == b);
		}
		public override bool Equals(object obj)
		{
			if (obj == null || this.GetType() != obj.GetType())		//オブジェクトがnullもしくは型が異なる場合は不等号判定とする．
			{
				return false;
			}
			return ((this.StartNodeId == ((Edge)obj).StartNodeId) && (this.EndNodeId == ((Edge)obj).EndNodeId) ||
					(this.StartNodeId == ((Edge)obj).EndNodeId) && (this.EndNodeId == ((Edge)obj).StartNodeId)
					);
		}
		public override int GetHashCode()
		{
			return ID ^ StartNodeId ^ EndNodeId;
		}

		/// <summary>
		/// 大小比較
		/// </summary>
		/// <param name="obj">比較対象となるobject</param>
		/// <returns>自分自身がobjより小さいときはマイナスの数、大きいときはプラスの数、同じときは0を返す</returns>
		public int CompareTo(object obj)
		{
			if (obj == null)
			{
				throw new ArgumentException("オブジェクトがnullです．", "obj");
			}
			else if (this.GetType() != obj.GetType())
			{
				throw new ArgumentException("別の型とは比較できません．", "obj");
			}
			return this.StartNodeId.CompareTo(((Edge)obj).StartNodeId);
		}

		/// <summary>
		/// エッジID
		/// </summary>
		public int ID { get { return id; } set { this.id = value; } }
		/// <summary>
		/// 一方の節点ID
		/// </summary>
		public int StartNodeId { get { return start_id; } set { this.start_id = value; } }
		/// <summary>
		/// 他方の節点ID
		/// </summary>
		public int EndNodeId { get { return end_id; } set { this.end_id = value; } }
		/// <summary>
		/// エッジの長さ
		/// </summary>
		public double Length { get { return length; } set { this.length = value; } }

		/// <summary>
		/// 引数によって与えられたエッジが自身のエッジの次にあたるかどうかチェックする．
		/// </summary>
		/// <param name="tested">チェックされるエッジ</param>
		/// <returns>次のエッジであればtrueを返す</returns>
		public bool IsNext(Edge tested)
		{
			return (this.EndNodeId == tested.StartNodeId);
		}

		private int id;
		private int start_id;
		private int end_id;
		private double length;
		private List<double> edge_force;
	}

	/// <summary>
	/// FEMにおけるエッジを表すクラス．
	/// 2次元Delaunay分割アルゴリズムを用いた要素分割にも対応．
	/// Sort,BinarySearchの為にIComparableインターフェースを継承する．
	/// </summary>
	[Obsolete]
	public class Edge2D : IComparable
	{
		/// <summary>
		/// Edge2Dクラスの新規インスタンスを初期化する．
		/// </summary>
		public Edge2D(int start = -256, int end = -255, int id = -256)
		{
			Id = id;
			StartNodeId = start;
			EndNodeId = end;
			edge_force = new double[] { 0, 0 };
		}
		public Edge2D(Edge2D source)
		{
			Id = source.Id;
			StartNodeId = source.StartNodeId;
			EndNodeId = source.EndNodeId;
			Length = source.Length;
			edge_force = new double[] { 0, 0 };
			for (int i = 0; i < 2; i++)
			{
				edge_force[i] = source.edge_force[i];
			}
		}

		public static bool operator ==(Edge2D a, Edge2D b)	//同じエッジか判定（向きは問わない）
		{
			return ((a.StartNodeId == b.StartNodeId) && (a.EndNodeId == b.EndNodeId) || (a.StartNodeId == b.EndNodeId) && (a.EndNodeId == b.StartNodeId));
		}
		public static bool operator !=(Edge2D a, Edge2D b)	//同じエッジか判定（向きは問わない）
		{
			return !(a == b);
		}
		public override bool Equals(object obj)
		{
			if (obj == null || this.GetType() != obj.GetType())		//オブジェクトがnullもしくは型が異なる場合は不等号判定とする．
			{
				return false;
			}
			return ((this.StartNodeId == ((Edge2D)obj).StartNodeId) && (this.EndNodeId == ((Edge2D)obj).EndNodeId) ||
					(this.StartNodeId == ((Edge2D)obj).EndNodeId) && (this.EndNodeId == ((Edge2D)obj).StartNodeId)
					);
		}
		public override int GetHashCode()
		{
			return Id ^ StartNodeId ^ EndNodeId;
		}
		/// <summary>
		/// 大小比較
		/// </summary>
		/// <param name="obj">比較対象となるobject</param>
		/// <returns>自分自身がobjより小さいときはマイナスの数、大きいときはプラスの数、同じときは0を返す</returns>
		public int CompareTo(object obj)
		{
			if (obj == null)
			{
				throw new ArgumentException("オブジェクトがnullです．", "obj");
			}
			else if (this.GetType() != obj.GetType())
			{
				throw new ArgumentException("別の型とは比較できません．", "obj");
			}
			return this.StartNodeId.CompareTo(((Edge2D)obj).StartNodeId);
		}

		/// <summary>
		/// エッジID
		/// </summary>
		public int Id { get { return id; } set { this.id = value; } }
		/// <summary>
		/// 一方の節点ID
		/// </summary>
		public int StartNodeId { get { return start_id; } set { this.start_id = value; } }
		/// <summary>
		/// 他方の節点ID
		/// </summary>
		public int EndNodeId { get { return end_id; } set { this.end_id = value; } }
		/// <summary>
		/// エッジの長さ
		/// </summary>
		public double Length { get { return length; } set { this.length = value; } }
		/// <summary>
		/// 引数によって与えられたエッジが自身のエッジの次にあたるかどうかチェックする．
		/// </summary>
		/// <param name="tested">チェックされるエッジ</param>
		/// <returns>次のエッジであればtrueを返す</returns>
		public bool IsNext(Edge2D tested)
		{
			return (this.EndNodeId == tested.StartNodeId);
		}

		private int id;
		private int start_id;
		private int end_id;
		private double length;
		private double[] edge_force;
	}

	/// <summary>
	/// FEMにおけるエッジを表すクラス．
	/// 3次元Delaunay分割アルゴリズムを用いた要素分割にも対応．
	/// Sort,BinarySearchの為にIComparableインターフェースを継承する．
	/// </summary>
	[Obsolete]
	public class Edge3D : System.IComparable
	{
		/// <summary>
		/// Edge3Dクラスの新規インスタンスを初期化する．
		/// </summary>
		public Edge3D()
			: this(-256, -256, -255)
		{
		}
		public Edge3D(int start, int end)
			: this(-256, start, end)
		{
		}
		public Edge3D(int id, int start, int end)
		{
			Id = id;
			StartNodeId = start;
			EndNodeId = end;
			edge_force = new double[] { 0, 0, 0 };
		}
		public Edge3D(Edge3D source)
		{
			Id = source.Id;
			StartNodeId = source.StartNodeId;
			EndNodeId = source.EndNodeId;
			for (int i = 0; i < 3; i++)
			{
				edge_force[i] = source.edge_force[i];
			}

		}

        /// <summary>
        /// 同じエッジかどうか判定する（向きは問わない）．判定基準には節点番号を採用する．
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
		public static bool operator ==(Edge3D a, Edge3D b)
		{
			return ((a.StartNodeId == b.StartNodeId) && (a.EndNodeId == b.EndNodeId) || (a.StartNodeId == b.EndNodeId) && (a.EndNodeId == b.StartNodeId));
		}
        /// <summary>
        /// 異なるエッジかどうか判定する（向きは問わない）．判定基準には節点番号を採用する．
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
		public static bool operator !=(Edge3D a, Edge3D b)
		{
			return !(a == b);
		}
		public override bool Equals(object obj)
		{
			if (obj == null || this.GetType() != obj.GetType())		//オブジェクトがnullもしくは型が異なる場合は不等号判定とする．
			{
				return false;
			}
			return ((this.StartNodeId == ((Edge3D)obj).StartNodeId) && (this.EndNodeId == ((Edge3D)obj).EndNodeId) ||
					(this.StartNodeId == ((Edge3D)obj).EndNodeId) && (this.EndNodeId == ((Edge3D)obj).StartNodeId)
					);
		}
		public override int GetHashCode()
		{
			return Id ^ StartNodeId ^ EndNodeId;
		}
		/// <summary>
		/// 大小比較
		/// </summary>
		/// <param name="obj">比較対象となるobject</param>
		/// <returns>自分自身がobjより小さいときはマイナスの数、大きいときはプラスの数、同じときは0を返す</returns>
		public int CompareTo(object obj)
		{
			if (obj == null)
			{
				throw new ArgumentException("オブジェクトがnullです．", "obj");
			}
			else if (this.GetType() != obj.GetType())
			{
				throw new ArgumentException("別の型とは比較できません．", "obj");
			}
			return this.StartNodeId.CompareTo(((Edge3D)obj).StartNodeId);
		}

		/// <summary>
		/// エッジID
		/// </summary>
		public int Id { get { return id; } set { this.id = value; } }
		/// <summary>
		/// 一方の節点ID
		/// </summary>
		public int StartNodeId { get { return start_id; } set { this.start_id = value; } }
		/// <summary>
		/// 他方の節点ID
		/// </summary>
		public int EndNodeId { get { return end_id; } set { this.end_id = value; } }

        private int id;
        private int start_id;
        private int end_id;
        private double[] edge_force;
	}
}
