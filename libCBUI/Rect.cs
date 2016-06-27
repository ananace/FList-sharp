using System;

namespace libCBUI
{
	public struct Rect : IEquatable<Rect>
	{
		public int Top { get; set; }
		public int Left { get; set; }
		public int Width { get; set; }
		public int Height { get; set; }

		public Point Position { get { return new Point(Left, Top); }
			set
			{
				Left = value.X;
				Top = value.Y;
			}
		}
		public Size Size { get { return new Size(Width, Height); }
			set
			{
				Width = value.Width;
				Height = value.Height;
			}
		}

		public Rect(int left, int top, int width, int height)
		{
			Top = top;
			Left = left;
			Width = width;
			Height = height;
		}

		public Rect(Point position, Size size)
		{
			Top = position.Y;
			Left = position.X;
			Width = size.Width;
			Height = size.Height;
		}


		public override int GetHashCode()
		{
			return Top ^ Left ^ Width ^ Height;
		}
		public override bool Equals(object obj)
		{
			if (obj is Rect)
				return Equals((Rect)obj);
			return false;
		}
		public bool Equals(Rect other)
		{
			return Top == other.Top &&
				Left == other.Left &&
				Width == other.Width &&
				Height == other.Height;
		}
		public static bool operator ==(Rect a, Rect b)
		{
			return a.Equals(b);
		}
		public static bool operator !=(Rect a, Rect b)
		{
			return !a.Equals(b);
		}
	}
}
