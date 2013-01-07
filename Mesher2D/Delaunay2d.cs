using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Serialization;
using System.IO;

using MeshContainer;

namespace Mesher2D
{
	/// <summary>
	/// 2次元Delaunay分割のためのクラス．
	/// </summary>
    public class Delaunay2d
    {
		/// <summary>
		/// Delaunay2dクラスの新規インスタンスを初期化し，givenな節点座標と新たに振り直した節点番号をセットする．
		/// </summary>
		/// <param name="p">節点リスト</param>
		public Delaunay2d(List<Node> p)
		{
			Complete = false;
			given_node = new List<Node>();
			for (int i = 0; i < p.Count; i++)
			{
				given_node.Add(new Node(i, p[i].OnBoundary, p[i].X, p[i].Y));
			}
			bracket_node = new List<Node>();
			delaunay_elements = new List<DelaunayElement2D>();
			delaunay_edges = new List<Edge>();
			SetBracketTriangle();

            SerializeContainer = new XmlSerializerContainer4Mesh();
		}

		private List<Node> given_node;		// Delaunay分割に際して与えられたノード
		private List<Node> bracket_node;		//ブラケットノード
		private List<DelaunayElement2D> delaunay_elements;	// Delaunay分割によって作られたメッシュ（要素）を格納する．
		private List<Edge> delaunay_edges;				//Delaunay分割によって作られたメッシュのうち，エッジ情報のみ

		/// <summary>
		/// XMLファイル出力用
		/// </summary>
		public XmlSerializerContainer4Mesh SerializeContainer;

		private double givennode_min_x = 0;
		private double givennode_max_x = 0;
		private double givennode_min_y = 0;
		private double givennode_max_y = 0;
		private int step = 0;
		private bool complete = false;

		/// <summary>
		/// メッシュ生成処理が完了したノード数を取得する．
		/// </summary>
		public int Step { get { return step; } private set { step = value; } }

		public bool Complete { get { return complete; } private set { complete = value; } }

		/// <summary>
		/// Delaunay分割により作成されたメッシュを取得する．
		/// </summary>
		/// <param name="mesh"></param>
		public void GetRawMesh(List<DelaunayElement2D> mesh)
		{
			//mesh = new List<DelaunayElement2D>(delaunay_elements.Count);
			mesh.Clear();
			for (int i = 0; i < delaunay_elements.Count; i++)
			{
			//	mesh.Add(new DelaunayElement2D(delaunay_elements[i]));
				mesh.Add(delaunay_elements[i]);
			//	mesh[i] = new DelaunayElement2D(delaunay_elements[i]);
			}
		}
		/*
		/// <summary>
		/// 正規化されたDelaunay分割により作成されたメッシュを取得する．
		/// </summary>
		/// <param name="mesh"></param>
		public void GetNormalizedMesh(List<DelaunayElement2D> mesh)
		{
			double size = Math.Max(givennode_max_x - givennode_min_x, givennode_max_y - givennode_min_y) * 0.5 * 1.1;	//余白で1.1倍
			double center_x = (givennode_min_x + givennode_max_x) * 0.5;
			double center_y = (givennode_min_y + givennode_max_y) * 0.5;
		//	mesh = new List<DelaunayElement2D>();
			mesh.Clear();
			for (int i = 0; i < delaunay_elements.Count; i++)
			{
				mesh.Add(
					new DelaunayElement2D(
						new Node(delaunay_elements[i].GetPoint(0).ID, (delaunay_elements[i].GetPoint(0).X - center_x) / size, (delaunay_elements[i].GetPoint(0).Y - center_y) / size),
						new Node(delaunay_elements[i].GetPoint(1).ID, (delaunay_elements[i].GetPoint(1).X - center_x) / size, (delaunay_elements[i].GetPoint(1).Y - center_y) / size),
						new Node(delaunay_elements[i].GetPoint(2).ID, (delaunay_elements[i].GetPoint(2).X - center_x) / size, (delaunay_elements[i].GetPoint(2).Y - center_y) / size)
					)
				);
			}
		}
		*/
		/// <summary>
		/// Delaunay分割を全ステップ実行する．
		/// </summary>
		public void GetDelaunay2d()
		{
			List<Edge> rediv_edge;					//再分割領域を囲むエッジ

			for (int i = 0; i < given_node.Count; i++)
			{
				SearchRedivisionMesh(given_node[i], out rediv_edge);
			//	Circulation(rediv_edge);
				Redivision(given_node[i], rediv_edge);
				delaunay_elements.RemoveAll(mesh => mesh.DeleteFlag == true);
			}
			RemoveBracket();
			ExtractEdge();
			Complete = true;
		}

		/// <summary>
		/// 節点をひとつ追加してDelaunay分割を1ステップ進める．
		/// </summary>
		public void NextStep()
		{
			List<Edge> rediv_edge;					//再分割領域を囲むエッジ

			if (step < given_node.Count)
			{
				SearchRedivisionMesh(given_node[step], out rediv_edge);
			//	Circulation(rediv_edge);
				Redivision(given_node[step], rediv_edge);
				delaunay_elements.RemoveAll(mesh => mesh.DeleteFlag == true);
				step++;
			}
			else if (step == given_node.Count)
			{
				RemoveBracket();
				ExtractEdge();
				Complete = true;
			}
			else
			{

			}
		}

		public void LaplacianSmoothing()
		{
			List<Node> NewNode = new List<Node>();

			for (int i = 0; i < given_node.Count; ++i)
			{
				if (!given_node[i].OnBoundary)
				{
					double x_sum=0;
					double y_sum=0;
					List<DelaunayElement2D> around_ele = new List<DelaunayElement2D>();
					around_ele = delaunay_elements.FindAll(ls => ls.Nodes.Exists(ls2 => ls2.ID == given_node[i].ID));
					for (int j = 0; j < around_ele.Count; ++j)
					{
						for(int k=0;k<3;++k)
						{
							if (around_ele[j].Nodes[k].ID != given_node[i].ID)
							{
								x_sum += around_ele[j].Nodes[k].X;
								y_sum += around_ele[j].Nodes[k].Y;
							}
						}
					}
					NewNode.Add(new Node(given_node[i].ID,false,x_sum/2.0/around_ele.Count,y_sum/2.0/around_ele.Count));
				}
				else
				{
					NewNode.Add(given_node[i]);
				}
			}

			Complete = false;
			given_node.Clear();
			for (int i = 0; i < NewNode.Count; i++)
			{
				given_node.Add(NewNode[i]);
			}
			bracket_node = new List<Node>();
			delaunay_elements = new List<DelaunayElement2D>();
			delaunay_edges = new List<Edge>();
			SetBracketTriangle();

			SerializeContainer = new XmlSerializerContainer4Mesh();
		
		}

		/// <summary>
		/// Delaunay分割されたメッシュの座標をコンソールに出力する．
		/// </summary>
		public void Print()
		{
			for (int i = 0; i < delaunay_elements.Count; i++)
			{
				Console.WriteLine(delaunay_elements[i].ToString());
			}
		}

		/// <summary>
		/// Delaunay分割されたメッシュの座標をXMLファイルに出力する．
		/// </summary>
		public void PrintXML()
		{
			SerializeContainer = new XmlSerializerContainer4Mesh(delaunay_elements.Count, 3, delaunay_edges.Count, given_node.Count, 0);
            for (int i = 0; i < delaunay_elements.Count; i++)
            {
                SerializeContainer.ElemPart.Elements[i].ID = i;
				SerializeContainer.ElemPart.Elements[i].ElementCode = "Tria03";
				for (int j = 0; j < 3; j++)
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
					SerializeContainer.ElemPart.Elements[i].Nodes[j].Coordinates[2].Value = 0;
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
				SerializeContainer.NodePart.Nodes[i].Coordinates[2].Value = 0;
			}
			for (int i = 0; i < delaunay_edges.Count; i++)
			{
				SerializeContainer.EdgePart.Edges[i].ID = delaunay_edges[i].ID;
				SerializeContainer.EdgePart.Edges[i].NodeID[0] = delaunay_edges[i].StartNodeId;
				SerializeContainer.EdgePart.Edges[i].NodeID[1] = delaunay_edges[i].EndNodeId;
			}
            
			var serializer = new XmlSerializer(typeof(MeshContainer.XmlSerializerContainer4Mesh));
			var fs = new FileStream("testxml.xml", FileMode.Create);
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
			double min_x = given_node.Min(ls => ls.X);
			double max_x = given_node.Max(ls => ls.X);
			double min_y = given_node.Min(ls => ls.Y);
			double max_y = given_node.Max(ls => ls.Y);

			givennode_min_x = given_node.Min(ls => ls.X);
			givennode_max_x = given_node.Max(ls => ls.X);
			givennode_min_y = given_node.Min(ls => ls.Y);
			givennode_max_y = given_node.Max(ls => ls.Y);

		//	double center_x = (max_x + min_x) /2;
		//	double center_y = (max_y + min_y) /2;
		//	double r = Math.Sqrt((max_x - min_x) * (max_x - min_x) + (max_y - min_y) * (max_y - min_y)) / 2;
			/*
			bracket_node.Add(new Node(-3, center_x, center_y + 2 * r));						//ブラケット三角形の上の点（ノードID:-3）追加
			bracket_node.Add(new Node(-2, center_x - Math.Sqrt(3) * r, center_y - r));		//ブラケット三角形の左下の点（ノードID:-2）追加
			bracket_node.Add(new Node(-1, center_x + Math.Sqrt(3) * r, center_y - r));		//ブラケット三角形の右下の点（ノードID:-1）追加

			delaunay_elements.Add(new DelaunayElement2D(bracket_node[0], bracket_node[1], bracket_node[2]));
			*/

			bracket_node.Add(new Node(-4, false, min_x - (max_x - min_x) * 0.05, max_y + (max_y - min_y) * 0.05));			//ブラケット三角形の左上の点（ノードID:-4）追加
			bracket_node.Add(new Node(-3, false, min_x - (max_x - min_x) * 0.05, min_y - (max_y - min_y) * 0.05));			//ブラケット三角形の左下の点（ノードID:-3）追加
			bracket_node.Add(new Node(-2, false, max_x + (max_x - min_x) * 0.05, min_y - (max_y - min_y) * 0.05));			//ブラケット三角形の右下の点（ノードID:-2）追加
			bracket_node.Add(new Node(-1, false, max_x + (max_x - min_x) * 0.05, max_y + (max_y - min_y) * 0.05));			//ブラケット三角形の右上の点（ノードID:-1）追加

			delaunay_elements.Add(new DelaunayElement2D(bracket_node[0], bracket_node[1], bracket_node[2]));
			delaunay_elements.Add(new DelaunayElement2D(bracket_node[3], bracket_node[0], bracket_node[2]));

			
		}

		/// <summary>
		/// 追加ノードに対して，そのノードを外接円内部に含むような要素をすべて探索し，該当領域のメッシュ・エッジ・ノードを抽出する．
		/// </summary>
		/// <param name="add_node">追加ノード</param>
		/// <param name="rediv_edge">再分割要素を囲むエッジを返す</param>
		private void SearchRedivisionMesh(Node add_node, out List<Edge> rediv_edge)
		{
			rediv_edge = new List<Edge>();
			for (int i = 0; i < delaunay_elements.Count; i++)	
			{
				if (delaunay_elements[i].IsInside(add_node))		//ノードを外接円内部に含むような要素の探索
				{
					for (int j = 0; j < 3; j++)
					{
						var find_edge = delaunay_elements[i].GetEdge(j);
						if (rediv_edge.Exists(ls => ls == find_edge))
						{
							rediv_edge.RemoveAll(ls => ls == find_edge);	//エッジの重複は必ず2回なので重複が検地された場合除去
						}
						else
						{
							rediv_edge.Add(find_edge);	//重複のない場合は追加
						}
					}
					delaunay_elements[i].DeleteFlag = true;		//削除用フラグのセット
				}
			}
		}

		/// <summary>
		/// エッジが格納されたListを環状にソートする．
		/// </summary>
		/// <param name="unsort">ソートされるList</param>
		/// <returns>正常にソートが終了すればtrueを返す</returns>
		private bool Circulation(List<Edge> unsort)
		{
			var lsnum = unsort.Count;			//エッジ数
			var sorted = new List<Edge>();
			sorted.Add(new Edge(unsort[0]));		//取り敢えず最初のエッジを初期エッジにしておく．
			for (int i = 0; i < lsnum - 1; i++)
			{
				for (int j = 1; j < unsort.Count; j++)
				{
					if (sorted[sorted.Count - 1].IsNext(unsort[j]))
					{
						sorted.Add(unsort[j]);
					}
				}
			}

			if ((sorted.Count != lsnum) || (sorted[0].StartNodeId != sorted[sorted.Count - 1].EndNodeId))
			{
				return false;
			}
			else
			{
				unsort.Clear();
				for (int i = 0; i < sorted.Count; i++)
				{
					unsort.Add(sorted[i]);
				}
				return true;
			}
		}

		/// <summary>
		/// 再分割領域を分割しメッシュを追加する．
		/// </summary>
		/// <param name="adding_node">追加ノード</param>
		/// <param name="rediv_edge">再分割領域を囲むエッジ</param>
		private void Redivision(Node adding_node, List<Edge> rediv_edge)
		{
			for (int j = 0; j < rediv_edge.Count; j++)
			{
				double start_coordi_x = 0;
				double start_coordi_y = 0;
				double end_coordi_x = 0;
				double end_coordi_y = 0;
				bool start_onboundary = false;
				bool end_onboundary = false;
				if (rediv_edge[j].StartNodeId < 0)		//ノードインデックス負数（ブラケット節点）の場合
				{
					//
					start_coordi_x = bracket_node[bracket_node.Count + rediv_edge[j].StartNodeId].X;
					start_coordi_y = bracket_node[bracket_node.Count + rediv_edge[j].StartNodeId].Y;
					start_onboundary = bracket_node[bracket_node.Count + rediv_edge[j].StartNodeId].OnBoundary;
				}
				else
				{
					start_coordi_x = given_node[rediv_edge[j].StartNodeId].X;
					start_coordi_y = given_node[rediv_edge[j].StartNodeId].Y;
					start_onboundary = given_node[rediv_edge[j].StartNodeId].OnBoundary;
				}
				if (rediv_edge[j].EndNodeId < 0)		//ノードインデックス負数（ブラケット節点）の場合
				{
					end_coordi_x = bracket_node[bracket_node.Count + rediv_edge[j].EndNodeId].X;
					end_coordi_y = bracket_node[bracket_node.Count + rediv_edge[j].EndNodeId].Y;
					end_onboundary = bracket_node[bracket_node.Count + rediv_edge[j].EndNodeId].OnBoundary;
				}
				else
				{
					end_coordi_x = given_node[rediv_edge[j].EndNodeId].X;
					end_coordi_y = given_node[rediv_edge[j].EndNodeId].Y;
					end_onboundary = given_node[rediv_edge[j].EndNodeId].OnBoundary;
				}
				delaunay_elements.Add(new DelaunayElement2D(adding_node,
													new Node(rediv_edge[j].StartNodeId, start_onboundary, start_coordi_x, start_coordi_y),
													new Node(rediv_edge[j].EndNodeId, end_onboundary, end_coordi_x, end_coordi_y)
													)
										);
			}
		}

		
		private void RemoveBracket()	// ブラケット接点を含む不要なメッシュを削除する．
		{
			delaunay_elements.RemoveAll(el => el.MinPoint().ID < 0);
		}

		private void ExtractEdge()		//エッジ情報のみ抽出する．
		{
			for (int i = 0; i < delaunay_elements.Count; i++)
			{
				for (int j = 0; j < 3; j++)
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

	public class LaplacianSmoothing
	{
		public LaplacianSmoothing(List<Node> p, List<DelaunayElement2D> mesh)
		{

		}


	}

	
}