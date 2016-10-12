using System;
using System.Linq;
using libflist.FChat.Commands;

namespace ConsoleMessenger.Commands
{
	[Command("joinp", Description = "Joins an available channel")]
	public class JoinPrivate : Command
	{
		public override bool TabComplete(string input, out string[] possibilities)
		{
			if (input.StartsWith("ADH-", StringComparison.CurrentCulture))
				possibilities = Application.Connection.PrivateChannels
					.Where(p => p.ID.StartsWith(input, StringComparison.OrdinalIgnoreCase))
					.Select(p => p.ID)
					.ToArray();
			else
				possibilities = Application.Connection.PrivateChannels
					.Where(p => p.Title.StartsWith(input, StringComparison.OrdinalIgnoreCase))
					.Select(p => p.Title)
					.ToArray();
			return possibilities.Any();
		}

		public void Call(string name)
		{
			var channel = Application.Connection.PrivateChannels.FirstOrDefault(c => c.Title == name || c.ID == name);

			if (channel == null && !name.StartsWith("ADH-", StringComparison.CurrentCulture))
				throw new ArgumentException(string.Format("Unknown channel {0}", name), nameof(name));

			Application.JoinChannel(channel != null ? channel.ID : name);
		}
	}
}

