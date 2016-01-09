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
		}
	}
}
