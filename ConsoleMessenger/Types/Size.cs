using System;
using System.Diagnostics;

namespace ConsoleMessenger.Types
{
	[DebuggerDisplay("({Width}x{Height})")]
	public struct Size : IEquatable<Size>
	{
		public int Width { get; set; }
		public int Height { get; set; }

		public Size(int width, int height)
		{
			Width = width;
			Height = height;
		}

		public Size(Point pt)
		{
			Width = pt.X;
			Height = pt.Y;
		}

		public static implicit operator Point(Size sz)
		{
			return new Point(sz.Width, sz.Height);
		}

		public static Size operator +(Size a, Size b)
		{
			return new Size(a.Width + b.Width, a.Height + b.Height);
		}
		public static Size operator -(Size a, Size b)
		{
			return new Size(a.Width - b.Width, a.Height - b.Height);
		}

		public static bool operator >(Size a, Size b)
		{
			return a.Width > b.Width && a.Height > b.Height;
		}
		public static bool operator >=(Size a, Size b)
		{
			return a.Width >= b.Width && a.Height >= b.Height;
		}
		public static bool operator <(Size a, Size b)
		{
			return a.Width < b.Width && a.Height < b.Height;
		}
		public static bool operator <=(Size a, Size b)
		{
			return a.Width <= b.Width && a.Height <= b.Height;
		}
		public static bool operator ==(Size a, Size b)
		{
			return a.Equals(b);
		}
		public static bool operator !=(Size a, Size b)
		{
			return !a.Equals(b);
		}

		public static Size Constrain(Size a, Size b)
		{
			return new Size(a.Width > b.Width ? b.Width : a.Width, a.Height > b.Height ? b.Height : a.Height);
		}

		public override int GetHashCode()
		{
			return Width ^ Height;
		}
		public override bool Equals(object obj)
		{
			if (obj is Size)
				return Equals((Size)obj);
			return false;
		}
		public bool Equals(Size other)
		{
			return Width == other.Width && Height == other.Height;
		}
	}
}