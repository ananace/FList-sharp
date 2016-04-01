namespace ConsoleMessenger.Commands
{
	[Command("quit")]
	class Quit : Command
	{
		public void Call()
		{
			Application.Running = false;
		}
	}
}
