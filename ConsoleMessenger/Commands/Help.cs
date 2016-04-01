using System;
using System.Diagnostics;
using System.Linq;

namespace ConsoleMessenger.Commands
{
	[Command("help", Description = "Provides help for the available commands")]
	public class Help : Command
	{
		public override bool TabComplete(string input, out string[] possibilities)
		{
			possibilities = Command.Available
				.Where(t => t.Key.StartsWith(input, StringComparison.OrdinalIgnoreCase))
				.Select(t => t.Key)
				.ToArray();
			return possibilities.Any();
		}

		public void Call()
		{
			Debug.WriteLine("Available commands are:");
			Debug.WriteLine("  " + string.Join(", ", Command.Available.Select(t => t.Key).OrderBy(t => t)));
			Debug.WriteLine("More help with: /help <command>");
		}

		public void Call(string command)
		{
			if (command[0] == '/')
				command = command.Substring(1);

			var cmd = Command.GetCommand(command);
			if (cmd == null)
				Debug.WriteLine($"No such command as '{command}'.");
			else
			{
				Debug.WriteLine($"{cmd.Name}: {cmd.Description}");
			}
		}
	}
}
