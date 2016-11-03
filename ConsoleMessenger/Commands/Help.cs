using System;
using System.Diagnostics;
using System.Linq;
using System.Reflection;

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
            Application.WriteLog("Request more help with: /help <command>");
			Application.WriteLog("Available commands are:");
			Application.WriteLog("  " + string.Join(", ", Command.Available.Select(t => t.Key).OrderBy(t => t)));
		}

		public void Call(string command)
		{
			if (command[0] == '/')
				command = command.Substring(1);

			var cmd = Command.GetCommand(command);
            if (cmd == null)
                Application.WriteLog($"No such command as '{command}'.");
            else
            {
                Application.WriteLog($"{cmd.Name}: {cmd.Description}");

                foreach (var m in Types[cmd.Name].GetMethods().Where(m => m.Name == "Call"))
                    Application.WriteLog(string.Format("- /{0} {1}", cmd.Name, m.GetParameters().Select(p => "<" + p.Name + ">").ToString(" ")));
            }
		}
	}
}
