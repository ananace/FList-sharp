using System;
using libflist.Connection.Types;

namespace libflist.FChat.Commands.Client.Server
{
	[Command("PIN")]
	public class PingCommand : Command
	{ 

	}
	
	[Command("RLD", MinRight = UserRight.ChatOP)]
	public class ReloadConfigCommand : Command
	{
		
	}

	[Command("UPT")]
	public class UptimeCommand : Command
	{

	}
}