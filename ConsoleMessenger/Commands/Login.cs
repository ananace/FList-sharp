using System;
using System.Diagnostics;
using System.Linq;

namespace ConsoleMessenger.Commands
{
	[Command("login")]
	class Login : Command
	{
		public override bool TabComplete(string input, out string[] possibilities)
		{
			possibilities = Application.Connection.User.Characters
				.Where(p => p.StartsWith(input, StringComparison.OrdinalIgnoreCase))
				.ToArray();
			return possibilities.Any();
		}

		public void Call(string character)
		{
			Debug.WriteLine(string.Format("Logging in as character {0}", character));
			Application.Connection.Login(character);
		}
	}
}
