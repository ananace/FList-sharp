using libflist.FChat.Commands;

namespace ConsoleMessenger.Commands
{
	[Command("channels", Description = "List available public channels")]
	public class Channels : Command
	{
		public void Call()
		{
			// TODO: Only update if data is stale.
			Application.SendCommand(new Client_CHA_ChatListOfficialChannels());
		}
	}
}

