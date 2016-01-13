using System;
using System.Linq;

namespace ConsoleMessenger.Commands
{
	[Command("list", Description = "List available channels, either public or private")]
	public class ListChats : Command
	{
		public enum ChatType
		{
			Public,
			Private
		}

		public ListChats()
		{
		}

		public override bool TabComplete(string input, out string[] possibilities)
		{
			string[] Possible = { "public", "private" };
			
			possibilities = Possible
				.Where(p => p.StartsWith(input, StringComparison.OrdinalIgnoreCase))
				.ToArray();
			return possibilities.Any();
		}

		public void Call()
		{
			Call(ChatType.Public);
		}

		public void Call(ChatType type)
		{
			// TODO: Only update if data is stale.
			if (type == ChatType.Public)
				Application.Connection.SendCommand(new libflist.Connection.Commands.Client.Global.GetPublicChannelsCommand());
			else
				Application.Connection.SendCommand(new libflist.Connection.Commands.Client.Global.GetPrivateChannelsCommand());
		}
	}
}

