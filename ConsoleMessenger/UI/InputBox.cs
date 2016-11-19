using ConsoleMessenger.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ConsoleMessenger.UI
{
	public class InputBox
	{
		public event EventHandler<string> OnTextEntered;

		List<string> _History = new List<string>();
		int _HistoryPtr = 0;
		string _HistoryCmd = null;

        public int HistoryLength { get; set; } = 5;
		public string PopHistory()
		{
            string ret = _History.Last();
            _History.RemoveAt(_History.Count - 1);
            return ret;
		}

		public int Cursor { get; set; }
		public bool NeedsRender { get; set; }

        StringBuilder _Content = new StringBuilder();
		public string Content
		{
			get
			{
				return (_Content as StringBuilder).ToString(); }
			set
			{
				var str = value.ToString();
				var sb = _Content as StringBuilder;

				if (sb.ToString() == str) return;

				sb.Clear();
				sb.Insert(0, str);

				if (Cursor > sb.Length)
					Cursor = sb.Length;

				NeedsRender = true;
			}
		}

		public void PushInput(ConsoleKeyInfo key)
		{
			if (key.Modifiers.HasFlag(ConsoleModifiers.Alt) || key.Modifiers.HasFlag(ConsoleModifiers.Control))
				return;

			var buf = _Content as StringBuilder;

			switch (key.Key)
			{
			case ConsoleKey.Backspace:
				{
					if (buf.Length == 0 || Cursor < 1)
						break;

					buf.Remove(--Cursor, 1);
                    Render();
				}
				break;

			case ConsoleKey.Delete:
				{
					if (Cursor >= buf.Length)
						break;

					buf.Remove(Cursor, 1);
                    Render(); // TODO: Do some speedup here?
				}
				break;

			case ConsoleKey.LeftArrow:
				if (Cursor > 0)
				{
					Cursor--;
					Console.CursorLeft--;
				}
				break;
			case ConsoleKey.RightArrow:
				if (Cursor < buf.Length)
				{
					Cursor++;
					Console.CursorLeft++;
				}
				break;
			case ConsoleKey.UpArrow:
				{
					if (_HistoryPtr + 1 >= _History.Count)
						break;

					if (_HistoryPtr < 0 && buf.Length > 0)
						_HistoryCmd = buf.ToString();

					++_HistoryPtr;
					buf.Clear();

					buf.Append(_History[_History.Count - _HistoryPtr - 1]);

					Cursor = buf.Length;
                    Render();
				}
				break;
			case ConsoleKey.DownArrow:
				{
					if (_HistoryPtr < 0)
						break;

					--_HistoryPtr;
					buf.Clear();

					if (_HistoryPtr >= 0)
						buf.Append(_History[_History.Count - _HistoryPtr - 1]);
					else
						buf.Append(_HistoryCmd);

					Cursor = buf.Length;
                    Render();
				}
				break;

			case ConsoleKey.Home:
				Console.CursorLeft -= Cursor;
				Cursor = 0;
				break;

			case ConsoleKey.End:
				Console.CursorLeft += buf.Length - Cursor;
				Cursor = buf.Length;
				break;

			case ConsoleKey.Enter:
				{
					var cmd = buf.ToString();

					var displayed = Content;
							
					buf.Clear();
					Cursor = 0;
                    Render();
					Focus();

					_HistoryCmd = null;
					_HistoryPtr = -1;
					_History.Add(cmd);
					if (_History.Count > HistoryLength)
						_History.RemoveAt(0);

                        OnTextEntered?.Invoke(this, cmd);
                    }
				break;

			default:
				{
					if (!char.IsControl(key.KeyChar) &&
					      !key.Modifiers.HasFlag(ConsoleModifiers.Alt) &&
					      !key.Modifiers.HasFlag(ConsoleModifiers.Control))
					{
						buf.Insert(Cursor++, key.KeyChar.ToString());
                        Render();
						break;
					}
				}
				break;
			}
		}

		public void Focus()
		{
			Console.SetCursorPosition(Cursor, ConsoleHelper.Size.Height - 1);
		}
		
		public void Render()
		{
            lock (Application.DrawLock)
            {
				NeedsRender = false;
                Graphics.DrawLine(new Point(0, ConsoleHelper.Size.Height - 1), new Size(ConsoleHelper.Size.Width - 1, 0), ' ');

                Console.SetCursorPosition(Cursor, ConsoleHelper.Size.Height - 1);
                using (var cur = new CursorChanger(new Point(0, ConsoleHelper.Size.Height - 1)))
                using (var col = new ColorChanger(foreground: ConsoleColor.White))
                {
                    Console.Write(new AutoSplitString(_Content.ToString()) { MaxLength = ConsoleHelper.Size.Width - 1 });
                }
				Console.ForegroundColor = ConsoleColor.Gray;
            }
		}
	}
}
