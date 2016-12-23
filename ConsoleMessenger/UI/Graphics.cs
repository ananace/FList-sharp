using System;
using System.Linq;
using System.Collections.Generic;
using ConsoleMessenger.Types;

namespace ConsoleMessenger.UI
{
	public static class StringHelper
	{
        public static string ToPlainString(this string String)
        {
            var data = String.Split('\x1b');
            return data.First() + data.Skip(1).Select(s =>
            {
                int i = s.IndexOf('m');
                return i >= 0 ? s.Substring(i + 1) : s;
            }).ToString(" ");
        }

		public static int ANSILength(this string String)
		{
            return String.ToPlainString().Length;
		}

		public static string ANSIColor(this ConsoleColor color, bool background = false)
		{
			int c = 0;
			bool b = false;
			switch (color)
			{
				case ConsoleColor.Gray: c = 30; b = true; break;
				case ConsoleColor.Black: c = 30; break;
				case ConsoleColor.Red: c = 31; b = true; break;
				case ConsoleColor.DarkRed: c = 31; break;
				case ConsoleColor.Green: c = 32; b = true; break;
				case ConsoleColor.DarkGreen: c = 32; break;
				case ConsoleColor.Yellow: c = 33; b = true; break;
				case ConsoleColor.DarkYellow: c = 33; break;
				case ConsoleColor.Blue: c = 34; b = true; break;
				case ConsoleColor.DarkBlue: c = 34; break;
				case ConsoleColor.Magenta: c = 35; b = true; break;
				case ConsoleColor.DarkMagenta: c = 35; break;
				case ConsoleColor.Cyan: c = 36; b = true; break;
				case ConsoleColor.DarkCyan: c = 36; break;
				case ConsoleColor.White: c = 37; b = true; break;
				case ConsoleColor.DarkGray: c = 37; break;
			}

			return string.Format("\x1b[{0}{1}m", background ? c + 10 : c, b ? ";1" : "");
		}

		public static ANSIString BackgroundColor(this string String, ConsoleColor color)
		{
            var ret = new ANSIString(String);
            ret.BackgroundColor = color;
            return ret;
		}
		public static ANSIString Color(this string String, ConsoleColor color)
		{
            var ret = new ANSIString(String);
            ret.ForegroundColor = color;
            return ret;
		}
	}

	class ColorChanger : IDisposable
	{
		readonly ConsoleColor Background;
		readonly ConsoleColor Foreground;

		public ColorChanger(ConsoleColor? background = null, ConsoleColor? foreground = null)
		{
			Background = Console.BackgroundColor;
			Foreground = Console.ForegroundColor;

			if (background.HasValue)
				Console.BackgroundColor = background.Value;
			if (foreground.HasValue)
				Console.ForegroundColor = foreground.Value;
		}

		public void Dispose()
		{
			Console.BackgroundColor = Background;
			Console.ForegroundColor = Foreground;
		}
	}
	class CursorChanger : IDisposable
	{
		readonly Point Cursor;
		readonly bool Visible;

		public CursorChanger(Point? newPos = null, bool? visible = null)
		{
			Cursor = new Point(Console.CursorLeft, Console.CursorTop);
			Visible = Console.CursorVisible;

			if (newPos.HasValue)
				Console.SetCursorPosition(newPos.Value.X, newPos.Value.Y);
			if (visible.HasValue)
				Console.CursorVisible = visible.Value;
		}

		public void Dispose()
		{
			Console.SetCursorPosition(Cursor.X, Cursor.Y);
			Console.CursorVisible = Visible;
		}
	}

	public static class Graphics
	{

		public static Size AvailableSize
		{
			get
			{
				return ConsoleHelper.Size - new Size(1, 1);
			}
		}

		public static void WriteANSIString(ANSIString String, Point? p = null, ConsoleColor? background = null, ConsoleColor? foreground = null)
		{
			CursorChanger cursor = null;
			if (p.HasValue)
				cursor = new CursorChanger(p);

            using (var color = new ColorChanger(background, foreground))
            {
                var oldBack = Console.BackgroundColor;
                var oldFore = Console.ForegroundColor;

                ConsoleColor? lastBack = null, lastFore = null;

                foreach (var ansichar in String as IEnumerable<ANSIString.ANSIChar>)
                {
                    if (lastBack != ansichar.BackgroundColor)
                    {
                        lastBack = ansichar.BackgroundColor;
                        if (lastBack.HasValue)
                            Console.BackgroundColor = lastBack.Value;
                        else
                            Console.BackgroundColor = oldBack;
                    }
                    if (lastFore != ansichar.ForegroundColor)
                    {
                        lastFore = ansichar.ForegroundColor;
                        if (lastFore.HasValue)
                            Console.ForegroundColor = lastFore.Value;
                        else
                            Console.ForegroundColor = oldFore;
                    }

                    Console.Write(ansichar.UnicodeChar);
                }
            }

			if (cursor != null)
				cursor.Dispose();
		}

		public static void DrawChar(Point p, char c, ConsoleColor Background = ConsoleColor.Black, ConsoleColor Foreground = ConsoleColor.White)
		{
			using (new CursorChanger(p, false))
			using (new ColorChanger(Background, Foreground))
				Console.Write(c);
		}

        public static void DrawLine(Point a, Size b, char Character, ConsoleColor Background = ConsoleColor.Black, ConsoleColor Foreground = ConsoleColor.White)
        {
            DrawLine(a, a + b, Character, Background, Foreground);
        }
		public static void DrawLine(Point a, Point b, char Character, ConsoleColor Background = ConsoleColor.Black, ConsoleColor Foreground = ConsoleColor.White)
		{
			using (new CursorChanger(null, false))
			using (new ColorChanger(Background, Foreground))
				Line(a.X, a.Y, b.X, b.Y, (x, y) =>
				{
					Console.SetCursorPosition(x, y);
					Console.Write(Character);
					return true;
				});
		}

		public static void DrawFilledBox(Point p1, Size sz, char c, ConsoleColor Background = ConsoleColor.Black, ConsoleColor Foreground = ConsoleColor.White)
		{
			DrawFilledBox(p1, p1 + sz, c, Background, Foreground);
		}

		public static void DrawFilledBox(Point p1, Point p2, char c, ConsoleColor Background = ConsoleColor.Black, ConsoleColor Foreground = ConsoleColor.White)
		{
			using (new CursorChanger(null, false))
			using (new ColorChanger(Background, Foreground))
			{
				var tmp = new string(c, p2.X - p1.X);

				for (int y = p1.Y; y != p2.Y; y += Math.Sign(p2.Y - p1.Y))
				{
					Console.SetCursorPosition(p1.X, y);
					Console.Write(tmp);
				}
			}
		}

		public static void DrawBox(Point pos, Point size, string BoxBrush, ConsoleColor Background = ConsoleColor.White, ConsoleColor Foreground = ConsoleColor.White)
		{
			if (BoxBrush == null)
				throw new ArgumentNullException(nameof(BoxBrush));
			if (BoxBrush.Length != 9)
				throw new ArgumentOutOfRangeException(nameof(BoxBrush));

			using (new CursorChanger(pos, false))
			using (new ColorChanger(Background, Foreground))
			{
				DrawChar(pos, BoxBrush[0]);
				DrawChar(pos + new Point(size.X, 0), BoxBrush[2]);
				DrawChar(pos + new Point(size.X, size.Y), BoxBrush[8]);
				DrawChar(pos + new Point(0, size.Y), BoxBrush[6]);

				DrawLine(pos + new Point(1, 0), pos + new Point(size.X - 1, 0), BoxBrush[1]);
				DrawLine(pos + new Point(0, 1), pos + new Point(0, size.Y - 1), BoxBrush[3]);
				DrawLine(pos + new Point(size.X, size.Y - 1), pos + new Point(size.X, 1), BoxBrush[5]);
				DrawLine(pos + new Point(size.X - 1, size.Y), pos + new Point(1, size.Y), BoxBrush[7]);
			}

			DrawFilledBox(pos + new Point(1, 1), pos + size - new Point(1, 1), BoxBrush[4]);
		}

		#region internal
		static void Swap<T>(ref T lhs, ref T rhs) { T temp; temp = lhs; lhs = rhs; rhs = temp; }

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
				if (err < 0) { y += ystep; err += dX; }
			}
		}
		#endregion
	}
}

