using System;
using System.Linq;

namespace ConsoleMessenger.Commands
{
	[Command("me", Description = "Send an emote to the current channel")]
	[Command("em", Description = "Send an emote to the current channel")]
	public class Me : Command
	{
		public void Call(params string[] msg)
		{
			Application.WriteMessage("/me " + msg.ToString(" "));
		}
	}
}

