using System;
using System.Linq;

namespace ConsoleMessenger.Commands
{
	[Command("join", Description = "List available channels, either public or private")]
	public class Join : Command
	{
		public void Call(string name)
		{
			Application.Connection.SendCommand(new libflist.Connection.Commands.Client.Channel.JoinCommand
			{
				Channel = name
			});
		}
	}
}

