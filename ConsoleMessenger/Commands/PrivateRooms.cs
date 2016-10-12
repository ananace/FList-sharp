using libflist.FChat.Commands;

namespace ConsoleMessenger.Commands
{
	[Command("prooms", Description = "List available private rooms")]
	public class PrivateRooms : Command
	{
		public void Call()
		{
			// TODO: Only update if data is stale.
			Application.Connection.SendCommand(new Client_ORS_ChatListPrivateChannels());
		}
	}
}

