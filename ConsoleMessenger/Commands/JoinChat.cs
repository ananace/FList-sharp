using System;
using System.Linq;

namespace ConsoleMessenger.Commands
{
	[Command("join", Description = "Joins an available channel")]
	public class Join : Command
	{
		public override bool TabComplete(string input, out string[] possibilities)
		{
			possibilities = Application.Connection.OfficialChannels
				.Where(p => p.Title.StartsWith(input, StringComparison.OrdinalIgnoreCase))
				.Select(p => p.Title)
				.ToArray();
			return possibilities.Any();
		}

		public void Call(string name)
		{
			Application.Connection.SendCommand(new libflist.FChat.Commands.Client.Channel.JoinCommand
			{
				Channel = name
			});
		}
	}
}

