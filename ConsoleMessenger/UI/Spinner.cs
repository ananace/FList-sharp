using System;
using System.Threading;

namespace ConsoleMessenger.UI
{
	public class Spinner : IDisposable
	{
		static readonly char[] Frames = {
			'-',
			'\\',
			'|',
			'/'
		};
		static readonly uint RedrawTime = 100;

		bool _Drawing;
		int _Frame;
		Timer _Redraw;

		private Spinner()
		{
			_Frame = 0;
			_Drawing = true;
			_Redraw = new Timer(s => Draw(), null, 0, RedrawTime);
		}

		public void Dispose()
		{
			Stop();

			_Redraw = null;
		}

		public static Spinner Start()
		{
			return new Spinner();
		}

		public void Stop()
		{
			lock(_Redraw)
			{
				_Drawing = false;
				_Redraw.Change(Timeout.Infinite, Timeout.Infinite);
				Draw(true);
			}
		}
		public void Restart()
		{
			lock(_Redraw)
			{
				_Drawing = true;
				_Redraw.Change(0, RedrawTime);
			}
		}

		public void Draw(bool erase = false)
		{
			lock(_Redraw)
			{
				if (!_Drawing && !erase)
					return;

				var originalX = Console.CursorLeft;
				var originalY = Console.CursorTop;

				if (erase)
				{
					Console.CursorLeft += 2;
					Console.Write("\b\b");
				}
				else
				{
					Console.Write(Frames[_Frame]);
					_Frame = (_Frame + 1) % Frames.Length;
				}

				Console.SetCursorPosition(originalX, originalY);
			}
		}
	}
}

