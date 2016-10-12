namespace ConsoleMessenger.Commands
{
	[Command("clear", Description = "Clears the log of the current chat")]
	class Clear : Command
	{
		public void Call()
		{
			Application.CurrentChannelBuffer.ChatBuf.Clear();
		}
	}
}
