using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Serialization;
using System.IO;

using MeshContainer;

namespace Mesher3D
{
	/// <summary>
	/// 3次元Delaunay分割のためのクラス．
	/// </summary>
	public class Delaunay3d
	{
		/// <summary>
		/// Delaunay3dクラスの新規インスタンスを初期化し，givenな節点座標と新たに振り直した節点番号をセットする．
		/// </summary>
		/// <param name="p">節点リスト</param>
		public Delaunay3d(List<Node> p)
		{
			given_node = new List<Node>();
			for (int i = 0; i < p.Count; i++)
			{
				given_node.Add(new Node(i, false, p[i].X, p[i].Y, p[i].Z));
			}
			bracket_node = new List<Node>();
			delaunay_elements = new List<Element3D>();
			delaunay_edges = new List<Edge>();
			SetBracketTriangle();
            SerializeContainer = new XmlSerializerContainer4Mesh();
		}

		private List<Node> given_node;			// Delaunay分割に際して与えられたノード
		private List<Node> bracket_node;			//ブラケットノード
		private List<Element3D> delaunay_elements;	// Delaunay分割によって作られたメッシュ（要素）を格納する．
		private List<Edge> delaunay_edges;		//Delaunay分割によって作られたメッシュのうち，エッジ情報のみ

		/// <summary>
		/// XMLファイル出力用
		/// </summary>
		public XmlSerializerContainer4Mesh SerializeContainer;

		private double givennode_min_x = 0;
		private double givennode_max_x = 0;
		private double givennode_min_y = 0;
		private double givennode_max_y = 0;
		private double givennode_min_z = 0;
		private double givennode_max_z = 0;
		private int step = 0;
		private bool complete;

		/// <summary>
		/// メッシュ生成処理が完了したノード数を取得する．
		/// </summary>
		public int Step { get { return step; } private set { step = value; } }
		public bool Complete { get { return complete; } private set { complete = value; } }

		/// <summary>
		/// Delaunay分割により作成されたメッシュを取得する．
		/// </summary>
		/// <param name="mesh"></param>
		public void GetRawMesh(out List<Element3D> mesh)
		{
			mesh = new List<Element3D>();
			for (int i = 0; i < delaunay_elements.Count; i++)
			{
				mesh.Add(new Element3D(delaunay_elements[i]));
			}
		}

		/// <summary>
		/// 正規化されたDelaunay分割により作成されたメッシュを取得する．
		/// </summary>
		/// <param name="mesh"></param>
		public void GetNormalizedMesh(out List<Element3D> mesh)
		{
			double size = Math.Max(Math.Max(givennode_max_x - givennode_min_x, givennode_max_y - givennode_min_y), givennode_max_z - givennode_min_z)* 0.5;	//余白なし
			double center_x = (givennode_min_x + givennode_max_x) * 0.5;
			double center_y = (givennode_min_y + givennode_max_y) * 0.5;
			double center_z = (givennode_min_z + givennode_max_z) * 0.5;
			mesh = new List<Element3D>();
			for (int i = 0; i < delaunay_elements.Count; i++)
			{
				mesh.Add(new Element3D(new Node(delaunay_elements[i].GetPoint(0).ID, false, (delaunay_elements[i].GetPoint(0).X - center_x) / size, (delaunay_elements[i].GetPoint(0).Y - center_y) / size, (delaunay_elements[i].GetPoint(0).Z - center_z) / size),
										new Node(delaunay_elements[i].GetPoint(1).ID, false, (delaunay_elements[i].GetPoint(1).X - center_x) / size, (delaunay_elements[i].GetPoint(1).Y - center_y) / size, (delaunay_elements[i].GetPoint(1).Z - center_z) / size),
										new Node(delaunay_elements[i].GetPoint(2).ID, false, (delaunay_elements[i].GetPoint(2).X - center_x) / size, (delaunay_elements[i].GetPoint(2).Y - center_y) / size, (delaunay_elements[i].GetPoint(2).Z - center_z) / size),
										new Node(delaunay_elements[i].GetPoint(3).ID, false, (delaunay_elements[i].GetPoint(3).X - center_x) / size, (delaunay_elements[i].GetPoint(3).Y - center_y) / size, (delaunay_elements[i].GetPoint(3).Z - center_z) / size)
										));
			}
		}

        /// <summary>
        /// Delaunay分割を全ステップ実行する．
        /// </summary>
        public void GetDelaunay3d()
        {
			if (!Complete)
			{
				List<Surface3D> rediv_surface;	//再分割領域を囲むサーフェス
				for (int i = 0; i < given_node.Count; i++)
				{
					SearchRedivisionMesh(given_node[i], out rediv_surface);
					Redivision(given_node[i], rediv_surface);
					delaunay_elements.RemoveAll(mesh => mesh.DeleteFlag == true);
				}
				RemoveBracket();
				//     ExtractEdge();
			}
			Complete = true;
        }

        /// <summary>
        /// 節点をひとつ追加してDelaunay分割を1ステップ進める．
        /// </summary>
        public void NextStep()
        {
			if (!Complete)
			{
				List<Surface3D> rediv_surface;	//再分割領域を囲むサーフェス
				if (step < given_node.Count)
				{
					SearchRedivisionMesh(given_node[step], out rediv_surface);
					Redivision(given_node[step], rediv_surface);
					delaunay_elements.RemoveAll(mesh => mesh.DeleteFlag == true);
					step++;
				}
				else if (step == given_node.Count)
				{
					RemoveBracket();
					//          ExtractEdge();
					Complete = true;
				}
			}
        }

		/// <summary>
		/// Delaunay分割されたメッシュの座標をコンソールに出力する．
		/// </summary>
		public void Print()
		{
			for (int i = 0; i < delaunay_elements.Count; i++)
			{
				delaunay_elements[i].Print();
			}
		}

		/// <summary>
		/// Delaunay分割されたメッシュの座標をXMLファイルに出力する．
		/// </summary>
		public void PrintXML()
		{
			SerializeContainer = new XmlSerializerContainer4Mesh(delaunay_elements.Count, 4, delaunay_edges.Count, given_node.Count, 0);
			for (int i = 0; i < delaunay_elements.Count; i++)
			{
				SerializeContainer.ElemPart.Elements[i].ID = i;
				SerializeContainer.ElemPart.Elements[i].ElementCode = "Tria03";
				for (int j = 0; j < 4; j++)
				{
					SerializeContainer.ElemPart.Elements[i].Nodes[j].InternalID = j;
					SerializeContainer.ElemPart.Elements[i].Nodes[j].ID = delaunay_elements[i].GetPoint(j).ID;

					SerializeContainer.ElemPart.Elements[i].Nodes[j].Coordinates[0].Sou = "mm";
					SerializeContainer.ElemPart.Elements[i].Nodes[j].Coordinates[0].CoordinateAxis = "x";
					SerializeContainer.ElemPart.Elements[i].Nodes[j].Coordinates[0].Value = delaunay_elements[i].GetPoint(j).X;
					SerializeContainer.ElemPart.Elements[i].Nodes[j].Coordinates[1].Sou = "mm";
					SerializeContainer.ElemPart.Elements[i].Nodes[j].Coordinates[1].CoordinateAxis = "y";
					SerializeContainer.ElemPart.Elements[i].Nodes[j].Coordinates[1].Value = delaunay_elements[i].GetPoint(j).Y;
					SerializeContainer.ElemPart.Elements[i].Nodes[j].Coordinates[2].Sou = "mm";
					SerializeContainer.ElemPart.Elements[i].Nodes[j].Coordinates[2].CoordinateAxis = "z";
					SerializeContainer.ElemPart.Elements[i].Nodes[j].Coordinates[2].Value = delaunay_elements[i].GetPoint(j).Z;
				}
				SerializeContainer.ElemPart.Elements[i].MaterialName = "Alminum";
				SerializeContainer.ElemPart.Elements[i].Density = 2700;
				SerializeContainer.ElemPart.Elements[i].Young = 200000;
				SerializeContainer.ElemPart.Elements[i].Poisson = 0.5;
			}
			for (int i = 0; i < given_node.Count; i++)
			{
				SerializeContainer.NodePart.Nodes[i].ID = given_node[i].ID;

				SerializeContainer.NodePart.Nodes[i].Coordinates[0].Sou = "mm";
				SerializeContainer.NodePart.Nodes[i].Coordinates[0].CoordinateAxis = "x";
				SerializeContainer.NodePart.Nodes[i].Coordinates[0].Value = given_node[i].X;
				SerializeContainer.NodePart.Nodes[i].Coordinates[1].Sou = "mm";
				SerializeContainer.NodePart.Nodes[i].Coordinates[1].CoordinateAxis = "y";
				SerializeContainer.NodePart.Nodes[i].Coordinates[1].Value = given_node[i].Y;
				SerializeContainer.NodePart.Nodes[i].Coordinates[2].Sou = "mm";
				SerializeContainer.NodePart.Nodes[i].Coordinates[2].CoordinateAxis = "z";
				SerializeContainer.NodePart.Nodes[i].Coordinates[2].Value = given_node[i].Z;
			}
			for (int i = 0; i < delaunay_edges.Count; i++)
			{
				SerializeContainer.EdgePart.Edges[i].ID = delaunay_edges[i].ID;
				SerializeContainer.EdgePart.Edges[i].NodeID[0] = delaunay_edges[i].StartNodeId;
				SerializeContainer.EdgePart.Edges[i].NodeID[1] = delaunay_edges[i].EndNodeId;
			}

			var serializer = new XmlSerializer(typeof(MeshContainer.XmlSerializerContainer4Mesh));

			var fs = new FileStream("mesh_"+DateTime.Now.ToString("yyyyMMddHHmmssfff")+".xml", FileMode.Create);
			var setting = new XmlWriterSettings();
			setting.Indent = true;
			setting.IndentChars = "  ";
			setting.OmitXmlDeclaration = false;

			serializer.Serialize(fs, SerializeContainer);
			fs.Close();
		}


		/// <summary>
		/// 与えられている全てのノード座標を読み取り，これらすべてを囲むようなブラケット三角形要素を生成し，elemに追加する．また，最大値最小値をセットする．
		/// </summary>
		private void SetBracketTriangle()
		{
			var min_x = givennode_min_x = given_node.Min(ls => ls.X);
			var max_x = givennode_max_x = given_node.Max(ls => ls.X);
			var min_y = givennode_min_y = given_node.Min(ls => ls.Y);
			var max_y = givennode_max_y = given_node.Max(ls => ls.Y);
			var min_z = givennode_min_z = given_node.Min(ls => ls.Z);
			var max_z = givennode_max_z = given_node.Max(ls => ls.Z);
			var cubecenter = new Node(0, false, (min_x + max_x) / 2, (min_y + max_y) / 2, (min_z + max_z) / 2);
			var cubulength = Math.Max(Math.Max((max_x - min_x), (max_y - min_y)), (max_z - min_z));	//与えられた全てのノードを内部に含み，各辺が座標軸に平行な最小の立方体の辺の長さ

			// cubelength = a とおく．一辺aの立方体の外接円の半径rはr=\sqrt{3}/2*a
			// 半径rの球の外接正四面体の一辺の長さLはL=3\sqrt{2}a
			// 球の中心から正四面体の頂点までの距離は3r = 3*\sqrt{3}/2*a <= 3a
			// 球の中心から正四面体の面までの距離はr = \sqrt{3}/2*a <= a
			bracket_node.Add(new Node(-4, false, cubecenter.X, cubecenter.Y + 2.5 * cubulength, cubecenter.Z - cubulength));		//上から見て底面上
			bracket_node.Add(new Node(-3, false, cubecenter.X - 2.2 * cubulength, cubecenter.Y - 1.3 * cubulength, cubecenter.Z - cubulength));		//上から見て底面左下
			bracket_node.Add(new Node(-2, false, cubecenter.X + 2.2 * cubulength, cubecenter.Y - 1.3 * cubulength, cubecenter.Z - cubulength));		//上から見て底面右下
			bracket_node.Add(new Node(-1, false, cubecenter.X, cubecenter.Y, cubecenter.Z + 3 * cubulength));		//真上
			
			delaunay_elements.Add(new Element3D(bracket_node[0], bracket_node[1], bracket_node[2], bracket_node[3]));
		}

		/// <summary>
		/// 追加ノードに対して，そのノードを外接球内部に含むような要素をすべて探索し，該当領域のメッシュ・サーフェスを抽出する．
		/// </summary>
		/// <param name="adding_node">追加ノード</param>
		/// <param name="rediv_surface">再分割要素を囲むサーフェスを返す</param>
		private void SearchRedivisionMesh(Node adding_node, out List<Surface3D> rediv_surface)
		{
			rediv_surface = new List<Surface3D>();
			for (int j = 0; j < delaunay_elements.Count; j++)
			{
				if (delaunay_elements[j].IsInside(adding_node))	//ノードを外接球内部に含むような要素であれば
				{
					for (int k = 0; k < 4; k++)
					{
						var find_surface = delaunay_elements[j].GetSurface(k);
						if (rediv_surface.Exists(ls => ls == find_surface))
						{
							rediv_surface.RemoveAll(ls => ls == find_surface);	//サーフェスの重複は必ず2回なので重複が検知された場合除去
						}
						else
						{
							rediv_surface.Add(find_surface);	//重複のない場合は取り合えず追加
						}
					}
					delaunay_elements[j].DeleteFlag = true;		//削除用フラグのセット
				}
			}
		}

		/// <summary>
		/// 再分割領域を分割しメッシュを追加する．
		/// </summary>
		/// <param name="adding_node"></param>
		/// <param name="rediv_surface"></param>
		private void Redivision(Node adding_node, List<Surface3D> rediv_surface)
		{
			for (int i = 0; i < rediv_surface.Count; i++)
			{
				double[,] coordi = new double[3, 3];

				for (int j = 0; j < 3; j++)
				{
					if (rediv_surface[i].GetNodeID(j) < 0)		//ノードインデックス負数（ブラケット節点）の場合
					{
						coordi[j, 0] = bracket_node[4 + rediv_surface[i].GetNodeID(j)].X;
						coordi[j, 1] = bracket_node[4 + rediv_surface[i].GetNodeID(j)].Y;
						coordi[j, 2] = bracket_node[4 + rediv_surface[i].GetNodeID(j)].Z;
					}
					else
					{
						coordi[j, 0] = given_node[rediv_surface[i].GetNodeID(j)].X;
						coordi[j, 1] = given_node[rediv_surface[i].GetNodeID(j)].Y;
						coordi[j, 2] = given_node[rediv_surface[i].GetNodeID(j)].Z;
					}
				}

				delaunay_elements.Add(new Element3D(new Node(rediv_surface[i].GetNodeID(0), false, coordi[0, 0], coordi[0, 1], coordi[0, 2]),
													new Node(rediv_surface[i].GetNodeID(1), false, coordi[1, 0], coordi[1, 1], coordi[1, 2]),
													new Node(rediv_surface[i].GetNodeID(2), false, coordi[2, 0], coordi[2, 1], coordi[2, 2]),
													adding_node
													)
										);
			}
		}
		
		private void RemoveBracket()	// ブラケット接点を含む不要なメッシュを削除する．
		{
			delaunay_elements.RemoveAll(el => el.MinNodeID() < 0);
		}

		private void ExtractEdge()		//エッジ情報のみ抽出する．
		{
			for (int i = 0; i < delaunay_elements.Count; i++)
			{
				for (int j = 0; j < 4; j++)
				{
					if (!delaunay_edges.Exists(ls => ls == delaunay_elements[i].GetEdge(j)))
					{
						delaunay_edges.Add(delaunay_elements[i].GetEdge(j));
					}
				}
			}
			for (int i = 0; i < delaunay_edges.Count; i++)
			{
				delaunay_edges[i].ID = i;
			}
		}

	}


	

}
