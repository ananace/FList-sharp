using System;

namespace ConsoleMessenger.UI.FChat
{
	class StatusBar : Control
	{
		libflist.FChat Chat { get; set; }



		public override void Render()
		{
			Console.ForegroundColor = ConsoleColor.DarkCyan;
			Console.Write("[");
			Console.ForegroundColor = ConsoleColor.Gray;
			Console.Write(DateTime.Now.ToShortTimeString());
			Console.ForegroundColor = ConsoleColor.DarkCyan;
			Console.Write("]");

			Console.CursorLeft++;

			if (Application.Connection.LocalCharacter != null)
			{
				Console.Write("[");
				Console.ForegroundColor = Application.Connection.LocalCharacter.GenderColor;
				Console.Write(Application.Connection.LocalCharacter.Name);
				Console.ForegroundColor = ConsoleColor.DarkCyan;
				Console.Write("]");

				Console.CursorLeft++;
			}

			Console.Write("[");

			Console.Write("]");
		}
	}
}
