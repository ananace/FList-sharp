namespace ConsoleMessenger.Commands
{
	[Command("clearall", Description = "Clears the log of all chats")]
	class ClearAll : Command
	{
		public void Call()
		{
			foreach (var buf in Application.Buffers)
				buf.ChatBuf.Clear();
		}
	}
}
