using System;

namespace libCBUI
{
	public struct Thickness
	{
		readonly int _Top;
		readonly int _Left;
		readonly int _Right;
		readonly int _Bottom;

		public int Top => _Top;
		public int Left => _Left;
		public int Right => _Right;
		public int Bottom => _Bottom;

		public Thickness(int uniform)
		{
			_Top = _Left = _Right = _Bottom = uniform;
		}

		public Thickness(int horiz, int vert)
		{
			_Top = _Bottom = vert;
			_Left = _Right = horiz;
		}

		public Thickness(int left, int top, int right, int bottom)
		{
			_Top = top;
			_Left = left;
			_Right = right;
			_Bottom = bottom;
		}

		public bool IsEmpty => Top == 0 && Right == 0 && Bottom == 0 && Left == 0;

		public static Thickness operator+(Thickness lhs, Thickness rhs)
		{
			return new Thickness(
				lhs.Left + rhs.Left,
				lhs.Top + rhs.Top,
				lhs.Right + rhs.Right,
				lhs.Bottom + rhs.Bottom);
		}

		public static Size operator+(Size sz, Thickness th)
		{
			return new Size(
				sz.Width + th.Left + th.Right,
				sz.Height + th.Top + th.Bottom);
		}

		public static Size operator-(Size sz, Thickness th)
		{
			return new Size(
				sz.Width - (th.Left + th.Right),
				sz.Height - (th.Top + th.Bottom));
		}
	}
}
