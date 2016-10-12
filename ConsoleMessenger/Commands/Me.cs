using System;
using System.Linq;

namespace ConsoleMessenger.Commands
{
	[Command("me", Description = "Send an emote to the current channel")]
	[Command("me's", Description = "Send an emote to the current channel")]
	[Command("em", Description = "Send an emote to the current channel")]
	[Command("em's", Description = "Send an emote to the current channel")]
	public class Me : Command
	{
		public void Call(params string[] msg)
		{
			if (CalledName.EndsWith("'s", StringComparison.OrdinalIgnoreCase))
				Application.WriteMessage($"/me's {msg.ToString(" ")}");
			else
				Application.WriteMessage($"/me {msg.ToString(" ")}");
		}
	}
}

