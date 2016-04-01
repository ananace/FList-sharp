using System.Diagnostics;

namespace ConsoleMessenger.Commands
{
	[Command("info", Description = "Provides information about the connected server")]
	public class Info : Command
	{
		public void Call()
		{
			Debug.WriteLine("Connected server variables:");
			foreach (var v in Application.Connection.Variables)
				Debug.WriteLine($"  {v.Key}:\t{v.Value}");
		}
	}
}

