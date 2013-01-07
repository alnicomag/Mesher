using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MeshContainer
{
	public class Mesh3D
	{
		public Mesh3D()
		{

		}
		public Mesh3D(int dim_, int partelementnum_, int partnodenum_)		//デフォルトのパート解析モデルを生成する
		{

		}
		public Mesh3D(int dim_, int partelementnum_, int partnodenum_, string elementcode)
		{

		}
		public Mesh3D(int dim_, int partelementnum_, int partnodenum_, string elementcode, string materialcode, double e_, double nu_)		//材料定数データも含めてパート解析モデルを生成する
		{

		}

		private string meshname;	//メッシュ名称
		private int freedeg;		//自由度
		private int bandwidth;		//バンド幅

		public string Meshname { get { return this.meshname; } set { this.meshname = value; } }
		public int Freedeg { get { return this.freedeg; } }
		public int BandWidth { get { return this.bandwidth; } private set { this.bandwidth = value; } }
		
		public List<Element3D> Inelem;		//要素並びのデータ
		public List<Node> InNode;			//節点並びのデータ
		public List<Edge> InEdge;			//稜並びのデータ
		public List<Surface3D> InSurf;		//面並びのデータ

		public void Resize(int dim, int partelementnum, int partnodenum) { }
		public void Resize(int dim, int partelementnum, int partnodenum, string elementcode) { }
		private void Resize(int dim, int partnodenum) { }
		private double Distance3D(double[] p, double[] q) { return 0; }


		public void SetTempNodeID() { }
		public void RemoveDupulication() { }

		void TranstoGlobalData() { }	//InelemからGlobalIndexデータに変換
		void TranstoLocalData() { }	//GlobalIndexデータからInelemに変換


		private void SetBandWidth()
		{
			int min = 0, max = 0;
			int temp_bw = 0;
			int bw_max = 0;

			for (int i = 0; i < Inelem.Count; ++i)
			{
				min = Inelem[i].MinNodeID();
				max = Inelem[i].MaxNodeID();
				temp_bw = max - min + 1;
				if (temp_bw > bw_max)
				{
					bw_max = temp_bw;
				}
			}
			this.BandWidth = (bw_max * 3);
		}


		public void PrintXML() { }
		public void PrintXML(string filename) { }

		public void PrintXML_NodeCoordinate() { }
		public void PrintXML_NodeCoordinate(string filename) { }

		void PrintXML_RestrictionLabel() { }
		void PrintXML_RestrictionLabel(string filename) { }
	

		//	Number of Element
		//	Number of Node
		//
		//	ElementID, "meshcode"
		//	GlobalNodeID(0), NodeCoordi_x, NodeCoordi_y, NodeCoordi_z
		//	GlobalNodeID(1), NodeCoordi_x, NodeCoordi_y, NodeCoordi_z
		//	・・・
		//	GlobalNodeID(7), NodeCoordi_x, NodeCoordi_y, NodeCoordi_z
		//	E, nu, rho,
		//
		//	GlobalNodeID, x_fixed?, y_fixed?, z_fixed?
		//
		//	GlobalNodeID, load_x, load_y, load_z
	}
}
