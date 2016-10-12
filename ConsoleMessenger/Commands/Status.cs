using libflist.FChat;
using libflist.FChat.Commands;
using System;
using System.Linq;

namespace ConsoleMessenger.Commands
{
	[Command("status", Description = "Sets your status, with an optional message")]
	class Status : Command
	{
		public override bool TabComplete(string input, out string[] possibilities)
		{
			CharacterStatus[] possible = {
				CharacterStatus.Away, CharacterStatus.Busy,
				CharacterStatus.DND, CharacterStatus.Looking,
				CharacterStatus.Online
			};

			possibilities = possible.Select(e => e.ToString())
				.Where(p => p.StartsWith(input, StringComparison.OrdinalIgnoreCase))
				.ToArray();
			return possibilities.Any();
		}

		public void Call(CharacterStatus status, string message)
		{
			Application.Connection.SendCommand(new Client_STA_ChatSetStatus { Message = message, Status = status });
		}

		public void Call(CharacterStatus status)
		{
			Application.Connection.SendCommand(new Client_STA_ChatSetStatus { Status = status });
		}
	}
}
