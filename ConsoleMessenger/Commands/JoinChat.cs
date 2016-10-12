using System;
using System.Linq;

namespace ConsoleMessenger.Commands
{
	[Command("join", Description = "Joins an available channel")]
	[Command("joino", Description = "Joins an official channel")]
	[Command("joinp", Description = "Joins a private channel")]
	public class Join : Command
	{
		public override bool TabComplete(string input, out string[] possibilities)
		{
			var possible = Application.Connection.AllKnownChannels;
			if (CalledName.EndsWith("o", StringComparison.OrdinalIgnoreCase))
				possible = Application.Connection.OfficialChannels;
			else if (CalledName.EndsWith("p", StringComparison.OrdinalIgnoreCase))
				possible = Application.Connection.PrivateChannels;

			if (!CalledName.EndsWith("p", StringComparison.OrdinalIgnoreCase) && input.StartsWith("ADH-", StringComparison.OrdinalIgnoreCase))
				possibilities = possible
					.Where(p => p.ID.StartsWith(input, StringComparison.OrdinalIgnoreCase))
					.Select(p => p.ID)
					.ToArray();
			else
				possibilities = possible
					.Where(p => p.Title.StartsWith(input, StringComparison.OrdinalIgnoreCase))
					.Select(p => p.Title)
					.ToArray();

			return possibilities.Any();
		}

		public void Call(string name)
		{
			if (!CalledName.EndsWith("o", StringComparison.OrdinalIgnoreCase) &&
				!name.StartsWith("ADH-", StringComparison.OrdinalIgnoreCase) &&
				!Application.Connection.OfficialChannels.Any(c => c.Title.Equals(name, StringComparison.OrdinalIgnoreCase)))
			{
				name = Application.Connection.PrivateChannels
					.First(c => c.Title.Equals(name, StringComparison.OrdinalIgnoreCase))
					.ID;
			}

			Application.JoinChannel(name);
		}
	}
}

