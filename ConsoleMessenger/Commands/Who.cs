using System;
using System.Diagnostics;
using System.Linq;
using ConsoleMessenger.UI;

namespace ConsoleMessenger.Commands
{
	[Command("who", Description = "List the users in the current channel")]
	public class Who : Command
	{
		public void Call()
		{
			var buf = Application.CurrentChannelBuffer.ChatBuf;
			if (buf is ConsoleChatBuffer)
				Debug.WriteLine("No users in console");
			else if (buf is CharacterChatBuffer)
				Debug.WriteLine((buf as CharacterChatBuffer).Character.Name);
			else
			{
				var chan = (buf as ChannelChatBuffer).Channel;
				Debug.WriteLine($"{chan.Characters.Count} characters in {chan.Title}:");
				Debug.WriteLine(chan.Characters.OrderBy(c => c.ToSortable(chan)).Select(c => c.ToANSIString()).ToString(", "));
			}
		}
	}
}

