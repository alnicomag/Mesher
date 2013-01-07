using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Geometry
{

	/// <summary>
	/// 点
	/// </summary>
	public class Point
	{
		public Point(params double[] x)
		{
			this.x = new double[x.Length];
			for (int i = 0; i < this.x.Length; ++i)
			{
				this.x[i] = x[i];
			}
		}
		public Point(Point source)
		{
			x = new double[source.x.Length];
			for (int i = 0; i < source.x.Length; ++i)
			{
				x[i] = source.x[i];
			}
		}

		public double this[int i]
		{
			get
			{
				if (i < x.Length) { return x[i]; } else { throw new IndexOutOfRangeException(); }
			}
			set
			{
				if (i < x.Length) { x[i] = value; } else { throw new IndexOutOfRangeException(); }
			}
		}

		public virtual int Dimension { get { return x.Length; } }

		protected double[] x;
	}

	public class Point2D : Point
	{
		public Point2D(double x, double y)
			: base(x, y)
		{
		}
        public Point2D(Point2D source)
            : base(source.X,source.Y)
        {
        }
		public static bool operator ==(Point2D p1, Point2D p2)
		{
			return (p1.X == p2.X) && (p1.Y == p2.Y);
		}
		public static bool operator !=(Point2D p1, Point2D p2)
		{
			return !(p1 == p2);
		}

        public override int Dimension { get { return 2; } }

		public double X { get { return x[0]; } set { x[0] = value; } }
		public double Y { get { return x[1]; } set { x[1] = value; } }
	}

	public class Point3D : Point
	{
		public Point3D(double x, double y, double z)
			: base(x, y, z)
		{
		}
		public static bool operator ==(Point3D p1, Point3D p2)
		{
			return (p1.X == p2.X) && (p1.Y == p2.Y) && (p1.Z == p2.Z);
		}
		public static bool operator !=(Point3D p1, Point3D p2)
		{
			return !(p1 == p2);
		}

        public override int Dimension
        {
            get { return 3; }
        }

		public double X { get { return x[0]; } set { x[0] = value; } }
		public double Y { get { return x[1]; } set { x[1] = value; } }
		public double Z { get { return x[2]; } set { x[2] = value; } }
	}

	public class Vector
	{
		public Vector(params double[] x)
		{
			this.x = new double[x.Length];
			for (int i = 0; i < this.x.Length; ++i)
			{
				this.x[i] = x[i];
			}
		}
		public Vector(Vector source)
		{
			x = new double[source.x.Length];
			for (int i = 0; i < source.x.Length; ++i)
			{
				x[i] = source.x[i];
			}
		}

		public static bool operator ==(Vector a, Vector b)
		{
			if (a.Dimension != b.Dimension)
			{
				throw new ArgumentException();
			}
			else
			{
				for (int i = 0; i < a.Dimension; ++i)
				{
					if (a[i] != b[i]) { return false; }
				}
				return true;
			}
		}
		public static bool operator !=(Vector a, Vector b)
		{
			try
			{
				return !(a == b);
			}
			catch(ArgumentException)
			{
				throw new ArgumentException();
			}
		}

		/// <summary>
		/// インデクサによる<c>Vector</c>の各成分へのアクセス．
		/// </summary>
		/// <param name="i"></param>
		/// <returns></returns>
		public double this[int i]
		{
			get
			{
				if (i < Dimension) { return x[i]; } else { throw new IndexOutOfRangeException(); }
			}
			set
			{
				if (i < Dimension) { x[i] = value; } else { throw new IndexOutOfRangeException(); }
			}
		}
		public virtual int Dimension { get { return x.Length; } }
		protected double[] x = new double[3];
	}

    public class Vector2D : Vector
    {
        /// <summary>
		/// ベクトルの各成分を用いて<c>Vector2D</c>クラスを初期化する．
		/// </summary>
		/// <param name="x"></param>
		/// <param name="y"></param>
		/// <param name="z"></param>
		public Vector2D(double x, double y)
			: base(x, y)
		{
		}

		public static bool operator ==(Vector2D a, Vector2D b)
		{
			return (a.X == b.X) && (a.Y == b.Y);
		}
		public static bool operator !=(Vector2D a, Vector2D b)
		{
			return !(a == b);
		}

		/// <summary>
		/// ベクトルの加算
		/// </summary>
		/// <param name="a"></param>
		/// <param name="b"></param>
		/// <returns></returns>
		public static Vector2D operator +(Vector2D a, Vector2D b)
		{
			return new Vector2D(a.X + b.X, a.Y + b.Y);
		}

		/// <summary>
		/// ベクトルの反転
		/// </summary>
		/// <param name="a"></param>
		/// <returns></returns>
		public static Vector2D operator -(Vector2D a)
		{
			return new Vector2D(-a.X, -a.Y);
		}

		/// <summary>
		/// ベクトルの減算
		/// </summary>
		/// <param name="a"></param>
		/// <param name="b"></param>
		/// <returns></returns>
		public static Vector2D operator -(Vector2D a, Vector2D b)
		{
			return new Vector2D(a.X - b.X, a.Y - b.Y);
		}

		/// <summary>
		/// ベクトルのスカラー乗算（スカラーが先）
		/// </summary>
		/// <param name="a"></param>
		/// <param name="v"></param>
		/// <returns></returns>
		public static Vector2D operator *(double a, Vector2D v)
		{
			return new Vector2D(a * v.X, a * v.Y);
		}

		/// <summary>
		/// ベクトルのスカラー乗算（ベクトルが先）
		/// </summary>
		/// <param name="v"></param>
		/// <param name="a"></param>
		/// <returns></returns>
		public static Vector2D operator *(Vector2D v, double a)
		{
			return a * v;
		}

		/// <summary>
		/// ベクトルのスカラー除算
		/// </summary>
		/// <param name="v"></param>
		/// <param name="a"></param>
		/// <returns></returns>
		public static Vector2D operator /(Vector2D v, double a)
		{
			if (a == 0)
			{
				throw new DivideByZeroException();	//ゼロ割り例外
			}
			else
			{
				return (1 / a) * v;
			}

		}

		/// <summary>
		/// ベクトルのドット積
		/// </summary>
		/// <param name="a"></param>
		/// <param name="b"></param>
		/// <returns></returns>
		public static double operator *(Vector2D a, Vector2D b)
		{
			return a.X * b.X + a.Y * b.Y;
		}

		

		public static explicit operator Vector2D(Point2D p)
		{
			return new Vector2D(p.X, p.Y);
		}

		/// <summary>
		/// ベクトルのクロス積
		/// </summary>
		/// <param name="a"></param>
		/// <param name="b"></param>
		/// <returns></returns>
		public static Vector3D CrossProduct(Vector2D a, Vector2D b)
		{
			return new Vector3D(0, 0, a.X * b.Y - a.Y * b.X);
		}

        public static double Angle(Vector2D a, Vector2D b)
        {
            return Math.Acos(a * b / a.Length / b.Length);
        }

        public override int Dimension { get { return 2; } }

		/// <summary>
		/// <c>Vector2D</c>のx成分
		/// </summary>
		public double X { get { return x[0]; } set { this.x[0] = value; } }
		/// <summary>
		/// <c>Vector2D</c>のy成分
		/// </summary>
		public double Y { get { return x[1]; } set { this.x[1] = value; } }

		/// <summary>
		/// <c>Vector2D</c>の長さを取得する．
		/// </summary>
		public double Length { get { return Math.Sqrt(X * X + Y * Y); } }

		/// <summary>
		/// <c>Vector2D</c>の長さの自乗を取得する．
		/// </summary>
		public double LengthSquared { get { return X * X + Y * Y; } }
    }

	/// <summary>
	/// 3次元デカルト座標系におけるベクトルを表すクラス
	/// </summary>
	public class Vector3D : Vector
	{
		/// <summary>
		/// ベクトルの各成分を用いて<c>Vector3D</c>クラスを初期化する．
		/// </summary>
		/// <param name="x"></param>
		/// <param name="y"></param>
		/// <param name="z"></param>
		public Vector3D(double x, double y, double z)
			: base(x, y, z)
		{
		}

		public static bool operator ==(Vector3D a, Vector3D b)
		{
			return (a.X == b.X) && (a.Y == b.Y) && (a.Z == b.Z);
		}
		public static bool operator !=(Vector3D a, Vector3D b)
		{
			return !(a == b);
		}

		/// <summary>
		/// ベクトルの加算
		/// </summary>
		/// <param name="a"></param>
		/// <param name="b"></param>
		/// <returns></returns>
		public static Vector3D operator +(Vector3D a, Vector3D b)
		{
			return new Vector3D(a.X + b.X, a.Y + b.Y, a.Z + b.Z);
		}

		/// <summary>
		/// ベクトルの反転
		/// </summary>
		/// <param name="a"></param>
		/// <returns></returns>
		public static Vector3D operator -(Vector3D a)
		{
			return new Vector3D(-a.X, -a.Y, -a.Z);
		}

		/// <summary>
		/// ベクトルの減算
		/// </summary>
		/// <param name="a"></param>
		/// <param name="b"></param>
		/// <returns></returns>
		public static Vector3D operator -(Vector3D a, Vector3D b)
		{
			return new Vector3D(a.X - b.X, a.Y - b.Y, a.Z - b.Z);
		}

		/// <summary>
		/// ベクトルのスカラー乗算（スカラーが先）
		/// </summary>
		/// <param name="a"></param>
		/// <param name="v"></param>
		/// <returns></returns>
		public static Vector3D operator *(double a, Vector3D v)
		{
			return new Vector3D(a * v.X, a * v.Y, a * v.Z);
		}

		/// <summary>
		/// ベクトルのスカラー乗算（ベクトルが先）
		/// </summary>
		/// <param name="v"></param>
		/// <param name="a"></param>
		/// <returns></returns>
		public static Vector3D operator *(Vector3D v, double a)
		{
			return a * v;
		}

		/// <summary>
		/// ベクトルのスカラー除算
		/// </summary>
		/// <param name="v"></param>
		/// <param name="a"></param>
		/// <returns></returns>
		public static Vector3D operator /(Vector3D v, double a)
		{
			if (a == 0)
			{
				throw new DivideByZeroException();	//ゼロ割り例外
			}
			else
			{
				return (1 / a) * v;
			}

		}

		/// <summary>
		/// ベクトルのドット積
		/// </summary>
		/// <param name="a"></param>
		/// <param name="b"></param>
		/// <returns></returns>
		public static double operator *(Vector3D a, Vector3D b)
		{
			return a.X * b.X + a.Y * b.Y + a.Z * b.Z;
		}

		

		public static explicit operator Vector3D(Point3D p)
		{
			return new Vector3D(p.X, p.Y, p.Z);
		}

		/// <summary>
		/// ベクトルのクロス積
		/// </summary>
		/// <param name="a"></param>
		/// <param name="b"></param>
		/// <returns></returns>
		public static Vector3D CrossProduct(Vector3D a, Vector3D b)
		{
			return new Vector3D(a.Y * b.Z - a.Z * b.Y, a.Z * b.X - a.X * b.Z, a.X * b.Y - a.Y * b.X);
		}

        public override int Dimension { get { return 3; } }

		/// <summary>
		/// <c>Vector3D</c>のx成分
		/// </summary>
		public double X { get { return x[0]; } set { this.x[0] = value; } }
		/// <summary>
		/// <c>Vector3D</c>のy成分
		/// </summary>
		public double Y { get { return x[1]; } set { this.x[1] = value; } }
		/// <summary>
		/// <c>Vector3D</c>のz成分
		/// </summary>
		public double Z { get { return x[2]; } set { this.x[2] = value; } }

		/// <summary>
		/// <c>Vector3D</c>の長さを取得する．
		/// </summary>
		public double Length { get { return Math.Sqrt(X * X + Y * Y + Z * Z); } }

		/// <summary>
		/// <c>Vector3D</c>の長さの自乗を取得する．
		/// </summary>
		public double LengthSquared { get { return X * X + Y * Y + Z * Z; } }
	}

    [Obsolete]
    public class Circle
    {
        public Point2D Center { get; set; }
        public double Radius { get; set; }
    }

    /// <summary>
    /// 長方形
    /// </summary>
    public class RegularRectangle
    {
        public RegularRectangle(Point2D p1, Point2D p2)
        {
            starting_point = new Point2D(p1.X < p2.X ? p1.X : p2.X, p1.Y < p2.Y ? p1.Y : p2.Y);
            diagonal = new Vector3D(Math.Abs(p1.X - p2.X), Math.Abs(p1.Y - p2.Y), 0);
        }
        public RegularRectangle(double top, double bottom, double right, double left)
        {
            starting_point = new Point2D(left, bottom);
            diagonal = new Vector3D(right - left, top - bottom, 0);
        }
        public RegularRectangle(RegularRectangle source)
        {
            starting_point = new Point2D(source.starting_point.X, source.starting_point.Y);
            diagonal = new Vector3D(source.diagonal.X, source.diagonal.Y, 0);
        }

        /// <summary>
        /// 高さ
        /// </summary>
        public double Height { get { return Top - Bottom; } }
        /// <summary>
        /// 幅
        /// </summary>
        public double Width { get { return Right - Left; } }
        /// <summary>
        /// 上辺
        /// </summary>
        public double Top
        {
            get { return starting_point.Y + diagonal.Y; }
            set
            {
                if (value <= Bottom) { throw new ArgumentException(); }
                else { diagonal.Y = value - Bottom; }
            }
        }
        public double Bottom
        {
            get { return starting_point.Y; }
            set
            {
                if (value >= Top) { throw new ArgumentException(); }
                else
                {
                    diagonal.Y = Top - value;
                    starting_point.Y = value;
                }
            }
        }
        public double Right
        {
            get { return starting_point.X + diagonal.X; }
            set
            {
                if (value <= Left) { throw new ArgumentException(); }
                else { diagonal.X = value - Left; }
            }
        }
        public double Left
        {
            get { return starting_point.X; }
            set
            {
                if (value <= Right) { throw new ArgumentException(); }
                else
                {
                    diagonal.X = Right - value;
                    starting_point.X = value;
                }
            }
        }
        public double DiagonalLength { get { return diagonal.Length; } }

        private Point2D starting_point;
        private Vector3D diagonal;
    }

    public class polygon
    {
        protected List<Point> vertex;
    }

    public class Rectangle
    {
        public Rectangle(Point2D center, double h, double w, double angle)
        {
            this.center = center;
            height = h;
            width = w;
            this.angle = angle;
        }

        public double Top { get; set; }
        public double Bottom { get; set; }
        public double Right { get; set; }
        public double Left { get; set; }
        public double DiagonalLength { get { return Math.Sqrt(height * height + width * width); } }

        private Point2D center;
        private double height;
        private double width;
        private double angle;
    }


	public enum LineSegmentCrossingStatement : int
	{
		Parallel,
		NoneCrossing,
		Crossing,
		Same,
		Overlapping,
		ParallelInLine,
		ParallelInContact,
		ContactEachOther,
		ContactT
	}

	/// <summary>
	/// 線分を表すクラス
	/// </summary>
	public class LineSegment2D
	{
		/// <summary>
		/// 線分の始点と終点を用いてLineSegmentクラスを初期化する．
		/// </summary>
		/// <param name="p1"></param>
		/// <param name="p2"></param>
		public LineSegment2D(Point2D p1, Point2D p2)
		{
			this.p1 = p1;
			this.p2 = p2;
			Length = Math.Sqrt((p1.X - p2.X) * (p1.X - p2.X) + (p1.Y - p2.Y) * (p1.Y - p2.Y));
		}

		/// <summary>
		/// インデクサによる線分の始点と終点に対するアクセス．
		/// </summary>
		/// <param name="i"></param>
		/// <returns></returns>
		public Point2D this[int i]
		{
			get
			{
				if (i == 0)
				{
					return new Point2D(p1);
				}
				else if (i == 1)
				{
					return new Point2D(p2);
				}
				else
				{
					throw new IndexOutOfRangeException();
				}
			}
		}

		/// <summary>
		/// <c>LineSegment2D</c>の長さを取得する．
		/// </summary>
		public double Length { get { return length; } private set { length = value; } }

		/// <summary>
        /// <c>Point2D p</c>がLineSegmentから見て右半面に存在するかどうか．
		/// </summary>
		/// <param name="p"></param>
		/// <returns></returns>
		public bool IsPointRightSide(Point2D p)
		{
            var vect1 = (Vector2D)p1 - (Vector2D)p;
            var vect2 = (Vector2D)p2 - (Vector2D)p;

			return Vector2D.CrossProduct(vect1, vect2).Z < 0 ? true : false;
		}

		/// <summary>
        /// <c>Point2D p</c>がLineSegmentで表される線分とその延長上に存在するかどうか．
		/// </summary>
		/// <param name="p"></param>
		/// <returns></returns>
		public bool IsPointOnLine(Point2D p)
		{
            var vect1 = (Vector2D)p1 - (Vector2D)p;
            var vect2 = (Vector2D)p2 - (Vector2D)p;

			return Vector2D.CrossProduct(vect1, vect2).Z == 0 ? true : false;
		}

		/// <summary>
		/// Node p がLineSegmentで表される線分上に存在するかどうか．（端点含む）
		/// </summary>
		/// <param name="p"></param>
		/// <returns></returns>
		public bool IsPointOnLineSegment(Point2D p)
		{
            if (IsPointOnLine(p))		//直線上
            {
                var vect1 = (Vector2D)p1 - (Vector2D)p;
                var vect2 = (Vector2D)p2 - (Vector2D)p;
                return vect1 * vect2 <= 0 ? true : false;   //0で端点上，負で線分内部
            }
			else
			{
				return false;
			}
		}

		/// <summary>
		/// 交点を有するかどうか
		/// </summary>
		/// <param name="ls"></param>
		/// <returns></returns>
		public bool Intersects(LineSegment2D ls)
		{
			double temp = (p2.X - p1.X) * (ls.p2.Y - ls.p1.Y) - (p2.Y - p1.Y) * (ls.p2.X - ls.p1.X);
			double temp_r = (ls.p2.Y - ls.p1.Y) * (ls.p1.X - p1.X) - (ls.p2.X - ls.p1.X) * (ls.p1.Y - p1.Y);
			double temp_s = (p2.Y - p1.Y) * (ls.p1.X - p1.X) - (p2.X - p1.X) * (ls.p1.Y - p1.Y);

			if (temp == 0)      //並行
			{
				if (temp_r != 0)
				{
					return false;
				}
				else        //同一直線上
				{
					if (ls.IsPointOnLineSegment(p1) | ls.IsPointOnLineSegment(p2))
					{
						return true;
					}
					else
					{
						return false;
					}
				}
			}
			else
			{
				double r = temp_r / temp;
				double s = temp_s / temp;
				if ((r < 0) | (1 < r) | (s < 0) | (1 < s))
				{
					return false;
				}
				else
				{
					return true;
				}
			}
		}


        public double DistanceTo(Point2D p)
        {
            var temp1 = (Vector2D)p - (Vector2D)p1;
            var temp2 = (Vector2D)p2 - (Vector2D)p1;

            return temp1.Length * Math.Sin(Vector2D.Angle(temp1, temp2));
        }

		private Point2D p1;
		private Point2D p2;
		private double length;
	}



	/// <summary>
	/// 図形パスを表すクラス
	/// </summary>
	public class FigurePath
	{
		/// <summary>
		/// 図形パスの開始Nodeを用いてFigurePathクラスを初期化する．
		/// </summary>
		/// <param name="start_point"></param>
		public FigurePath(Point2D start_point)
		{
			points = new List<Point2D>();
			lines = new List<LineSegment2D>();
			min_x = max_x = start_point.X;
			min_y = max_y = start_point.Y;
            envelope = new Rectangle(start_point, 0, 0, 0);
			points.Add(start_point);
		}

		/// <summary>
		/// インデクサによる図形パスを構成する線分へのアクセス．
		/// </summary>
		/// <param name="i"></param>
		/// <returns></returns>
		public LineSegment2D this[int i]
		{
			get
			{
#if DEBUG
				if (i >= lines.Count)
				{
					throw new IndexOutOfRangeException();
				}
#endif
				return this.lines[i];
			}
		}

		/// <summary>
		/// 図形パスを構成する<c>Point2D</c>を追加する．
		/// </summary>
		/// <param name="p"></param>
		public void AddPoint(Point2D p)
		{
			points.Add(p);
			if (p.X < min_x)
			{
				min_x = p.X;
			}
			else if (p.X > max_x)
			{
				max_x = p.X;
			}
			if (p.Y < min_y)
			{
				min_y = p.Y;
			}
			else if (p.Y > max_y)
			{
				max_y = p.Y;
			}

			if (points.Count == 2)
			{
				lines.Add(new LineSegment2D(points[0], points[1]));
			}
			else if (points.Count == 3)
			{
				lines.Add(new LineSegment2D(points[1], points[2]));
				lines.Add(new LineSegment2D(points[2], points[0]));
			}
			else
			{
				lines.RemoveAt(lines.Count - 1);
				lines.Add(new LineSegment2D(points[points.Count - 2], points[points.Count - 1]));
				lines.Add(new LineSegment2D(points[points.Count - 1], points[0]));
			}
		}

		/// <summary>
		/// 検査点が<c>FigurePath</c>で表された図形パスの内部に存在するかどうか．閉じられた図形パスが存在しない場合にはnullを返す．
		/// </summary>
		/// <param name="p">検査点</param>
		/// <returns></returns>
		public bool? IsInside(Point2D p)
		{
			if (Count >= 3)
			{
				if ((p.X < min_x) | (p.X > max_x) | (p.Y < min_y) | (p.Y > max_y))		//パス包含長方形の外部
				{
					return false;
				}
				else
				{
					bool temp2 = false;
					foreach (LineSegment2D line in lines)
					{
						temp2 |= line.IsPointOnLineSegment(p);
					}
					if (temp2)	//パス境界上
					{
						return false;
					}
					else
					{
						double size = Math.Sqrt((max_x - min_x) * (max_x - min_x) + (max_y - min_y) * (max_y - min_y));
						Random rnd = new Random();
						LineSegment2D ray;
						bool ray_is_crossing_on_point;
						do
						{
							double theta = rnd.NextDouble() * 2.0 * Math.PI;
							ray = new LineSegment2D(p, new Point2D(p.X + size * Math.Cos(theta), p.Y + size * Math.Sin(theta)));
							ray_is_crossing_on_point = false;
							foreach (Point2D tested_point in points)
							{
								ray_is_crossing_on_point |= ray.IsPointOnLineSegment(tested_point);
							}
						}
						while (ray_is_crossing_on_point);	//パスのノード上を通過することのないレイを選択
						int crossing_count = 0;
						foreach (LineSegment2D select_ls in lines)
						{
							if (select_ls.Intersects(ray))
							{
								crossing_count++;
							}
						}
						return crossing_count % 2 == 0 ? false : true;
					}
				}
			}
			else
			{
				return null;
			}
		}

		/// <summary>
		/// FigurePathで表された図形パスを構成する線分の数を取得する．
		/// </summary>
		public int Count { get { return lines.Count; } }

		/// <summary>
		/// FigurePathで表された図形パスが正常かどうかを取得する．
		/// </summary>
		/// <returns></returns>
		public bool IsCorrect()
		{
			return path_state;
		}

		private List<Point2D> points;
		private List<LineSegment2D> lines;
		private bool isBoss;	//生成パスor切取りパス
		private bool path_state;
		private double min_x;
		private double min_y;
		private double max_x;
		private double max_y;
        private Rectangle envelope;	//パスを取り囲む最小の（傾いていない）長方形
	}

}
