using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace MeshContainer
{
    [XmlRoot("mesh", Namespace = "urn:Handcraft:XmlSC4Mesh")]
	public class XmlSerializerContainer4Mesh
	{
		public XmlSerializerContainer4Mesh()
		{
			NumberOfElements = 0;
			NumberOfNodes = 0;
			NumberOfEdges = 0;
			ElemPart = new XmlSerializerContainer4ElementPart();
			EdgePart = new XmlSerializerContainer4EdgePart();
			NodePart = new XmlSerializerContainer4NodePart();
			SurfacePart = new XmlSerializerContainer4SurfacePart();
		}
		public XmlSerializerContainer4Mesh(int elemnum, int inelem_nodenum, int edgenum, int nodenum, int surfacenum)
		{
			NumberOfElements = elemnum;
			NumberOfNodes = nodenum;
			NumberOfEdges = edgenum;

			ElemPart = new XmlSerializerContainer4ElementPart(elemnum, inelem_nodenum);
			EdgePart = new XmlSerializerContainer4EdgePart(edgenum);
			NodePart = new XmlSerializerContainer4NodePart(nodenum);
			SurfacePart = new XmlSerializerContainer4SurfacePart(surfacenum);
		}
        [XmlElement("elemnum")]
		public int NumberOfElements { get; set; }
		[XmlElement("edgenum")]
		public int NumberOfEdges { get; set; }
        [XmlElement("nodenum")]
		public int NumberOfNodes { get; set; }
		[XmlElement("elempart")]
		public XmlSerializerContainer4ElementPart ElemPart;
		[XmlElement("nodepart")]
		public XmlSerializerContainer4NodePart NodePart;
		[XmlElement("edgepart")]
		public XmlSerializerContainer4EdgePart EdgePart;
		[XmlElement("surfacepart")]
		public XmlSerializerContainer4SurfacePart SurfacePart;
	}

	[XmlRoot("elementpart", Namespace = "urn:Handcraft:XmlSC4ElementPart")]
	public class XmlSerializerContainer4ElementPart
	{
		public XmlSerializerContainer4ElementPart()
		{
			Elements = new List<XmlSerializerContainer4Element>();
		}
		public XmlSerializerContainer4ElementPart(int elemnum, int inelem_nodenum)
		{
			Elements = new List<XmlSerializerContainer4Element>();
			for (int i = 0; i < elemnum; i++)
			{
				Elements.Add(new XmlSerializerContainer4Element(inelem_nodenum));
			}
		}

		[XmlElement("element")]
		public List<XmlSerializerContainer4Element> Elements;
	}

	[XmlRoot("element", Namespace = "urn:Handcraft:XmlSC4Element")]
	public class XmlSerializerContainer4Element
	{
		public XmlSerializerContainer4Element()
		{
			ID = 0;
			ElementCode = "Sample";
			Young = 0;
			Poisson = 0;
			Density = 0;
			Nodes = new List<XmlSerializerContainer4InElemNode>();
		}
		public XmlSerializerContainer4Element(int inelem_nodenum)
		{
			ID = 0;
			ElementCode = "Sample";
			Young = 0;
			Poisson = 0;
			Density = 0;
			Nodes = new List<XmlSerializerContainer4InElemNode>();
			for (int i = 0; i < inelem_nodenum; i++)
			{
				Nodes.Add(new XmlSerializerContainer4InElemNode(3));
			}
		}
		[XmlAttribute("elemid")]
		public int ID { get; set; }
		[XmlAttribute("code")]
		public string ElementCode { get; set; }
		[XmlElement("matename")]
		public string MaterialName { get; set; }
		[XmlElement("e")]
		public double Young { get; set; }
		[XmlElement("nu")]
		public double Poisson { get; set; }
		[XmlElement("rho")]
		public double Density { get; set; }
		[XmlElement("node")]
		public List<XmlSerializerContainer4InElemNode> Nodes;
	}

	[XmlRoot("inelemnode", Namespace = "urn:Handcraft:XmlSC4Node")]
	public class XmlSerializerContainer4InElemNode
	{
		public XmlSerializerContainer4InElemNode()
		{
			InternalID = 0;
			ID = 0;
		//	Coordinate = new XmlSerializerContainer4Coordinate();
			Coordinates = new List<XmlSerializerContainer4Coordinate>();
		}
		public XmlSerializerContainer4InElemNode(int freedeg)
		{
			InternalID = 0;
			ID = 0;
			Coordinates = new List<XmlSerializerContainer4Coordinate>();
			for (int i = 0; i < freedeg; i++)
			{
				Coordinates.Add(new XmlSerializerContainer4Coordinate());
			}
		}
		[XmlAttribute("internal_id")]
		public int InternalID { get; set; }
		[XmlElement("nodeid")]
		public int ID { get; set; }
	//	[XmlElement("coordinate")]
	//	public XmlSerializerContainer4Coordinate Coordinate;
		[XmlElement("coordinate")]
		public List<XmlSerializerContainer4Coordinate> Coordinates;
	}

	[XmlRoot("coordinate", Namespace = "urn:Handcraft:XmlSC4Coordinate")]
	public class XmlSerializerContainer4Coordinate
	{
		public XmlSerializerContainer4Coordinate()
		{
			Sou = "mm";
			CoordinateAxis = "x";
			Value = 0;
		//	X = 0;
		//	Y = 0;
		//	Z = 0;
		}
		[XmlAttribute("sou")]
		public string Sou { get; set; }
		[XmlAttribute("axis")]
		public string CoordinateAxis { get; set; }
		[XmlElement("value")]
		public double Value { get; set; }
		
	//	public double X { get; set; }
		
	//	public double Y { get; set; }
		
	//	public double Z { get; set; }
	}

	[XmlRoot("nodepart", Namespace = "urn:Handcraft:XmlSC4NodePart")]
	public class XmlSerializerContainer4NodePart
	{
		public XmlSerializerContainer4NodePart()
		{
			Nodes = new List<XmlSerializerContainer4Node>();
		}
		public XmlSerializerContainer4NodePart(int nodenum)
		{
			Nodes = new List<XmlSerializerContainer4Node>();
			for (int i = 0; i < nodenum; i++)
			{
				Nodes.Add(new XmlSerializerContainer4Node(3));
			}
		}
		[XmlElement("node")]
		public List<XmlSerializerContainer4Node> Nodes;
	}

	[XmlRoot("node", Namespace = "urn:Handcraft:XmlSC4Node")]
	public class XmlSerializerContainer4Node
	{
		public XmlSerializerContainer4Node()
		{
			ID = 0;
			Coordinates = new List<XmlSerializerContainer4Coordinate>();
		}
		public XmlSerializerContainer4Node(int freedeg)
		{
			ID = 0;
			Coordinates = new List<XmlSerializerContainer4Coordinate>();
			for (int i = 0; i < freedeg; i++)
			{
				Coordinates.Add(new XmlSerializerContainer4Coordinate());
			}
		}
		[XmlElement("nodeid")]
		public int ID { get; set; }
		[XmlElement("coordinate")]
		public List<XmlSerializerContainer4Coordinate> Coordinates;
	}

	[XmlRoot("edgepart", Namespace = "urn:Handcraft:XmlSC4EdgePart")]
	public class XmlSerializerContainer4EdgePart
	{
		public XmlSerializerContainer4EdgePart()
		{
			Edges = new List<XmlSerializerContainer4Edge>();
		}
		public XmlSerializerContainer4EdgePart(int edgenum)
		{
			Edges = new List<XmlSerializerContainer4Edge>();
			for (int i = 0; i < edgenum; i++)
			{
				Edges.Add(new XmlSerializerContainer4Edge());
			}
		}
		[XmlElement("edge")]
		public List<XmlSerializerContainer4Edge> Edges;
	}

	[XmlRoot("edge", Namespace = "urn:Handcraft:XmlSC4Edge")]
	public class XmlSerializerContainer4Edge
	{
		public XmlSerializerContainer4Edge()
		{
			ID = 0;
			NodeID = new int[2] { 0, 0 };
		}
		[XmlAttribute("edgeid")]
		public int ID { get; set; }
		[XmlElement("nodeid")]
		public int[] NodeID;
	}

	[XmlRoot("surfacepart", Namespace = "urn:Handcraft:XmlSCSurfacePart")]
	public class XmlSerializerContainer4SurfacePart
	{
		public XmlSerializerContainer4SurfacePart()
		{
			Surface = new List<XmlSerializerContainer4Surface>();
		}
		public XmlSerializerContainer4SurfacePart(int surfacenum)
		{
			Surface = new List<XmlSerializerContainer4Surface>();
			for (int i = 0; i < surfacenum; i++)
			{
				Surface.Add(new XmlSerializerContainer4Surface());
			}
		}
		[XmlElement("surface")]
		public List<XmlSerializerContainer4Surface> Surface;
	}

	[XmlRoot("surface", Namespace = "urn:Handcraft:XmlSC4Surface")]
	public class XmlSerializerContainer4Surface
	{
		public XmlSerializerContainer4Surface()
		{
			ID = 0;
		}
		[XmlAttribute("surfaceid")]
		public int ID { get; set; }
	}
}
