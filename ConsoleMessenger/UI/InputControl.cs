using System;

namespace ConsoleMessenger.UI
{
	class InputControl : ContentControl
	{
		public event EventHandler<string> OnTextEntered;

		public InputControl()
		{
			Background = ConsoleColor.Black;
			Content = "";
		}

		public override void PushInput(ConsoleKeyInfo key)
		{
			if (key.Modifiers.HasFlag(ConsoleModifiers.Alt) || key.Modifiers.HasFlag(ConsoleModifiers.Control))
				return;

			var buf = Content as string;

			try
			{
				switch (key.Key)
				{
				case ConsoleKey.Backspace:
					{
						buf = buf.Substring(0, buf.Length - 1);

						Console.Write("\b \b");
					}
					break;

				case ConsoleKey.Enter:
					{
						var cmd = buf;
						buf = "";
							
						Clear();
						Console.CursorLeft -= cmd.Length;
							
						Content = buf;

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
							buf += key.KeyChar;
							Console.Write(key.KeyChar);
							break;
						}
					}
					break;
				}
			}
			finally
			{
				Content = buf;
			}
		}

		public override void Focus()
		{
			base.Focus();

			Console.SetCursorPosition(DisplayPosition.X + ContentDrawable.ToString().Length, DisplayPosition.Y);
		}
	}
}
