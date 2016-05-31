namespace libflist.FChat.Commands
{
	[Command("PIN")]
	public sealed class Client_PIN_ChatPing : Command
	{ 

	}
	
	[Command("RLD", MinRight = UserRight.ChatOP)]
	public sealed class Client_RLD_ChatReloadConfig : Command
	{
		
	}

	[Command("UPT")]
	public sealed class Client_UPT_ChatGetUptime : Command
	{

	}
}