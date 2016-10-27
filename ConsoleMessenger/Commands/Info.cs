using System.Diagnostics;

namespace ConsoleMessenger.Commands
{
	[Command("info", Description = "Provides information about the connected server")]
	public class Info : Command
	{
		public void Call()
		{
			Application.WriteLog("Connected server variables:");
			foreach (var v in Application.Connection.Variables)
				Application.WriteLog($"  {v.Key}:\t{v.Value}");
		}
	}
}

