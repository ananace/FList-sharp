using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
