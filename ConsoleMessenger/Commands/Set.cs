using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleMessenger.Commands
{
	[Command("set", Description = "Reads or sets the value of a configurable setting.")]
	class Set : Command
	{
		public void Call()
		{
			// TODO: Get all settings for the current context
		}

		public void Call(string name)
		{
			// TODO: Get the value of the given setting
		}

		public void Call(string name, params string[] values)
		{
			// TODO: Set a setting
		}
	}
}
