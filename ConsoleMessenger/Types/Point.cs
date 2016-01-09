using System;
using System.Diagnostics;

namespace ConsoleMessenger.Types
{
	[DebuggerDisplay("({X}, {Y})")]
	public struct Point : IEquatable<Point>
	{
		public int X;
		public int Y;

		public Point(int x, int y)
		{
			X = x;
			Y = y;
		}

		public static Point operator+(Point a, Point b)
		{
			return new Point(a.X + b.X, a.Y + b.Y);
		}
		public static Point operator-(Point a, Point b)
		{
			return new Point(a.X - b.X, a.Y - b.Y);
		}

		public static Point Constrain(Point a, Point b)
		{
			return new Point(a.X > b.X ? b.X : a.X, a.Y > b.Y ? b.Y : a.Y);
		}

		public override int GetHashCode()
		{
			return X.GetHashCode() ^ Y.GetHashCode();
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