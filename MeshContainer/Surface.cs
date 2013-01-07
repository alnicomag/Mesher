using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MeshContainer
{
	public class Surface3D
	{
		public Surface3D(params int[] nodeid)
		{
			ID = -256;
			node_id = new int[nodeid.Length];

			for (int i = 0; i < nodeid.Length; i++)
			{
				node_id[i] = nodeid[i];
			}
			surface_force = new double[3] { 0, 0, 0 };
		}
		public Surface3D(Surface3D source)
		{
			ID = source.ID;
			node_id = new int[source.node_id.Length];
			for (int i = 0; i < source.node_id.Length; ++i)
			{
				node_id[i] = source.node_id[i];
			}
			surface_force = new double[source.surface_force.Length];
			for (int i = 0; i < source.surface_force.Length; ++i)
			{
				surface_force[i] = source.surface_force[i];
			}
		}

		public static bool operator ==(Surface3D a, Surface3D b)
		{
			return ((a.node_id[0] == b.node_id[0]) && (a.node_id[1] == b.node_id[1]) && (a.node_id[2] == b.node_id[2]))
				|| ((a.node_id[0] == b.node_id[1]) && (a.node_id[1] == b.node_id[2]) && (a.node_id[2] == b.node_id[0]))
				|| ((a.node_id[0] == b.node_id[2]) && (a.node_id[1] == b.node_id[0]) && (a.node_id[2] == b.node_id[1]))
				|| ((a.node_id[0] == b.node_id[0]) && (a.node_id[1] == b.node_id[2]) && (a.node_id[2] == b.node_id[1]))
				|| ((a.node_id[0] == b.node_id[2]) && (a.node_id[1] == b.node_id[1]) && (a.node_id[2] == b.node_id[0]))
				|| ((a.node_id[0] == b.node_id[1]) && (a.node_id[1] == b.node_id[0]) && (a.node_id[2] == b.node_id[2]));
		}
		public static bool operator !=(Surface3D a, Surface3D b)
		{
			return !(a == b);
		}
		public override bool Equals(object obj)
		{
			if (obj == null || this.GetType() != obj.GetType())		//オブジェクトがnullもしくは型が異なる場合は不等号判定とする．
			{
				return false;
			}
			return ((this.node_id[0] == ((Surface3D)obj).node_id[0]) && (this.node_id[1] == ((Surface3D)obj).node_id[1]) && (this.node_id[2] == ((Surface3D)obj).node_id[2]))
				|| ((this.node_id[0] == ((Surface3D)obj).node_id[1]) && (this.node_id[1] == ((Surface3D)obj).node_id[2]) && (this.node_id[2] == ((Surface3D)obj).node_id[0]))
				|| ((this.node_id[0] == ((Surface3D)obj).node_id[2]) && (this.node_id[1] == ((Surface3D)obj).node_id[0]) && (this.node_id[2] == ((Surface3D)obj).node_id[1]))
				|| ((this.node_id[0] == ((Surface3D)obj).node_id[0]) && (this.node_id[1] == ((Surface3D)obj).node_id[2]) && (this.node_id[2] == ((Surface3D)obj).node_id[1]))
				|| ((this.node_id[0] == ((Surface3D)obj).node_id[2]) && (this.node_id[1] == ((Surface3D)obj).node_id[1]) && (this.node_id[2] == ((Surface3D)obj).node_id[0]))
				|| ((this.node_id[0] == ((Surface3D)obj).node_id[1]) && (this.node_id[1] == ((Surface3D)obj).node_id[0]) && (this.node_id[2] == ((Surface3D)obj).node_id[2]));
		}
		public override int GetHashCode()
		{
			return ID ^ node_id[0] ^ node_id[1] ^ node_id[2];
		}

		/// <summary>
		/// サーフェス番号
		/// </summary>
        public int ID { get { return id; } set { this.id = value; } }

		public int GetNodeID(int a)
		{
			return node_id[a];
		}

		public bool PositiveSide(Node p)
		{

			return true;
		}

		private int id;
		private int[] node_id;
		private double[] surface_force;
        
	}
}
