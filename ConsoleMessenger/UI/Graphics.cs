using System;
using ConsoleMessenger.Types;

namespace ConsoleMessenger.UI
{
	public static class Graphics
	{
		public static void DrawChar(Point p, char c, ConsoleColor Background = ConsoleColor.Black, ConsoleColor Foreground = ConsoleColor.White)
		{
			var oldPos = new Point(Console.CursorLeft, Console.CursorTop);
			var oldBack = Console.BackgroundColor;
			var oldFore = Console.ForegroundColor;

			Console.BackgroundColor = Background;
			Console.ForegroundColor = Foreground;
			Console.SetCursorPosition(p.X, p.Y);

			Console.Write(c);

			Console.SetCursorPosition(oldPos.X, oldPos.Y);
			Console.BackgroundColor = oldBack;
			Console.ForegroundColor = oldFore;
		}

		public static void DrawLine(Point a, Point b, ConsoleColor Background = ConsoleColor.White, ConsoleColor Foreground = ConsoleColor.White, char Character = ' ')
		{
			var oldPos = new Point(Console.CursorLeft, Console.CursorTop);
			var oldBack = Console.BackgroundColor;
			var oldFore = Console.ForegroundColor;

			Console.BackgroundColor = Background;
			Console.ForegroundColor = Foreground;

			Line(a.X, a.Y, b.X, b.Y, (x, y) => {
				Console.SetCursorPosition(x, y);
				Console.Write(Character);
				return true;
			});

			Console.SetCursorPosition(oldPos.X, oldPos.Y);
			Console.BackgroundColor = oldBack;
			Console.ForegroundColor = oldFore;
		}

		public static void DrawFilledBox(Point p1, Point p2, char c, ConsoleColor Background = ConsoleColor.Black, ConsoleColor Foreground = ConsoleColor.White)
		{
			var oldPos = new Point(Console.CursorLeft, Console.CursorTop);
			var oldBack = Console.BackgroundColor;
			var oldFore = Console.ForegroundColor;

			var tmp = new string(c, p2.X - p1.X);

			for (int y = p1.Y; y != p2.Y; y += Math.Sign(p2.Y - p1.Y))
			{
				Console.SetCursorPosition(p1.X, y);
				Console.Write(tmp);
			}
			
			Console.SetCursorPosition(oldPos.X, oldPos.Y);
			Console.BackgroundColor = oldBack;
			Console.ForegroundColor = oldFore;
		}

		public static void DrawBox(Point pos, Point size, string BoxBrush, ConsoleColor Background = ConsoleColor.White, ConsoleColor Foreground = ConsoleColor.White)
		{
			if (BoxBrush == null)
				throw new ArgumentNullException(nameof(BoxBrush));
			if (BoxBrush.Length != 9)
				throw new ArgumentOutOfRangeException(nameof(BoxBrush));

			var oldPos = new Point(Console.CursorLeft, Console.CursorTop);
			var oldBack = Console.BackgroundColor;
			var oldFore = Console.ForegroundColor;

			DrawChar(pos, BoxBrush[0], Background, Foreground);
			DrawChar(pos + new Point(size.X, 0), BoxBrush[2], Background, Foreground);
			DrawChar(pos + new Point(size.X, size.Y), BoxBrush[8], Background, Foreground);
			DrawChar(pos + new Point(0, size.Y), BoxBrush[6], Background, Foreground);

			DrawLine(pos + new Point(1, 0), pos + new Point(size.X - 1, 0), Background, Foreground, BoxBrush[1]);
			DrawLine(pos + new Point(0, 1), pos + new Point(0, size.Y - 1), Background, Foreground, BoxBrush[3]);
			DrawLine(pos + new Point(size.X, size.Y - 1), pos + new Point(size.X, 1), Background, Foreground, BoxBrush[5]);
			DrawLine(pos + new Point(size.X - 1, size.Y), pos + new Point(1, size.Y), Background, Foreground, BoxBrush[7]);
			
			Console.SetCursorPosition(oldPos.X, oldPos.Y);
			Console.BackgroundColor = oldBack;
			Console.ForegroundColor = oldFore;
			
			DrawFilledBox(pos + new Point(1, 1), pos + size - new Point(1, 1), BoxBrush[4], Background, Foreground);
		}

		#region internal
		private static void Swap<T>(ref T lhs, ref T rhs) { T temp; temp = lhs; lhs = rhs; rhs = temp; }

		public delegate bool PlotFunction(int x, int y);

		public static void Line(int x0, int y0, int x1, int y1, PlotFunction plot)
		{
			bool steep = Math.Abs(y1 - y0) > Math.Abs(x1 - x0);
			if (steep) { Swap<int>(ref x0, ref y0); Swap<int>(ref x1, ref y1); }
			if (x0 > x1) { Swap<int>(ref x0, ref x1); Swap<int>(ref y0, ref y1); }
			int dX = (x1 - x0), dY = Math.Abs(y1 - y0), err = (dX / 2), ystep = (y0 < y1 ? 1 : -1), y = y0;

			for (int x = x0; x <= x1; ++x)
			{
				if (!(steep ? plot(y, x) : plot(x, y))) return;
				err = err - dY;
				if (err < 0) { y += ystep;  err += dX; }
			}
		}
		#endregion
	}
}

