using System;
using System.Collections.Generic;
using System.Text;

namespace ConsoleMessenger.UI
{
	public class InputControl : ContentControl
	{
		public event EventHandler<string> OnTextEntered;
		public override event EventHandler OnContentChanged;

		List<string> _History;
		int _HistoryPtr;
		string _HistoryCmd;

		public int HistoryLength { get; set; } = 5;
		public void PopHistory()
		{
			_History.RemoveAt(_History.Count - 1);
		}

		public int Cursor { get; set; }

		public override object Content
		{
			get
			{
				if (base.Content == null || !(base.Content is StringBuilder))
					base.Content = new StringBuilder();

				return (base.Content as StringBuilder).ToString(); }
			set
			{
				if (base.Content == null || !(base.Content is StringBuilder))
					base.Content = new StringBuilder();

				var str = value.ToString();
				var sb = base.Content as StringBuilder;

				if (sb.ToString() == str) return;

				sb.Clear();
				sb.Insert(0, str);

				if (Cursor > sb.Length)
					Cursor = sb.Length;

				if (OnContentChanged != null)
					OnContentChanged(this, EventArgs.Empty);

				InvalidateLayout();
			}
		}

		public InputControl()
		{
			Background = ConsoleColor.Black;
			base.Content = new StringBuilder();

			_History = new List<string>();
		}

		public override void PushInput(ConsoleKeyInfo key)
		{
			if (key.Modifiers.HasFlag(ConsoleModifiers.Alt) || key.Modifiers.HasFlag(ConsoleModifiers.Control))
				return;

			var buf = base.Content as StringBuilder;
			var before = buf.ToString();

			switch (key.Key)
			{
			case ConsoleKey.Backspace:
				{
					if (buf.Length == 0 || Cursor < 1)
						break;

					buf.Remove(--Cursor, 1);
					InvalidateVisual();
				}
				break;

			case ConsoleKey.Delete:
				{
					if (Cursor >= buf.Length)
						break;

					buf.Remove(Cursor, 1);
					InvalidateVisual();
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

					var displayed = ContentDrawable;
							
					Clear();
					buf.Clear();
					Cursor = 0;

					_HistoryCmd = null;
					_HistoryPtr = -1;
					_History.Add(cmd);
					if (_History.Count > HistoryLength)
						_History.RemoveAt(0);

					if (OnTextEntered != null)
						OnTextEntered(this, cmd);
				}
				break;

			default:
				{
					if (!char.IsControl(key.KeyChar) &&
					      !key.Modifiers.HasFlag(ConsoleModifiers.Alt) &&
					      !key.Modifiers.HasFlag(ConsoleModifiers.Control))
					{
						buf.Insert(Cursor++, key.KeyChar.ToString());
						Console.Write(key.KeyChar);
						break;
					}
				}
				break;
			}


			if (buf.ToString() != before)
			{
				if (OnContentChanged != null)
					OnContentChanged(this, EventArgs.Empty);

				InvalidateLayout();
			}
		}

		public override void Focus()
		{
			base.Focus();

			Console.SetCursorPosition(DisplayPosition.X + Cursor, DisplayPosition.Y);
		}
		
		public override void Render()
		{
			var toDraw = (base.Content as StringBuilder).ToString();
			Console.Write(toDraw);

			Console.SetCursorPosition(DisplayPosition.X + Cursor, DisplayPosition.Y);
		}
	}
}
