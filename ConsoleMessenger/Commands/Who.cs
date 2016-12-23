using System;
using System.Diagnostics;
using System.Linq;
using ConsoleMessenger.Types;
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
				buf.PushMessage(null, (buf as CharacterChatBuffer).Character.Name, MessageType.Preview);
			else
			{
				var chan = (buf as ChannelChatBuffer).Channel;
				buf.PushMessage(null, $"{chan.Characters.Count} characters in {chan.Title}:", MessageType.Preview);
                buf.PushMessage(null, ANSIString.Join(", ", chan.Characters.OrderBy(c => c.ToSortable(chan)).Select(c => c.ToANSIString(chan, true))).ToBBCode(), MessageType.Preview);
			}
		}
	}
}

