using System;
using System.Diagnostics;

namespace libCBUI
{
	[DebuggerDisplay("[{X},{Y}]")]
	public struct Point : IEquatable<Point>
	{
		public int X { get; set; }
		public int Y { get; set; }

		public Point(int x, int y)
		{
			X = x;
			Y = y;
		}

		public Point(Size sz)
		{
			X = sz.Width;
			Y = sz.Height;
		}

		public static implicit operator Size(Point p)
		{
			return new Size(p.X, p.Y);
		}

		public static Point operator +(Point a, Point b)
		{
			return new Point(a.X + b.X, a.Y + b.Y);
		}
		public static Point operator +(Point a, Size b)
		{
			return new Point(a.X + b.Width, a.Y + b.Height);
		}
		public static Point operator -(Point a, Point b)
		{
			return new Point(a.X - b.X, a.Y - b.Y);
		}
		public static Point operator -(Point a, Size b)
		{
			return new Point(a.X - b.Width, a.Y - b.Height);
		}

		public static Point Constrain(Point a, Point b)
		{
			return new Point(a.X > b.X ? b.X : a.X, a.Y > b.Y ? b.Y : a.Y);
		}

		public override int GetHashCode()
		{
			return X ^ Y;
		}
		public override bool Equals(object obj)
		{
			if (obj is Point)
				return Equals((Point)obj);
			return false;
		}
		public bool Equals(Point other)
		{
			return X == other.X && Y == other.Y;
		}
		public static bool operator==(Point a, Point b)
		{
			return a.Equals(b);
		}
		public static bool operator!=(Point a, Point b)
		{
			return !a.Equals(b);
		}
	}
}