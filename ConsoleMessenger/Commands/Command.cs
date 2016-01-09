using System;

namespace ConsoleMessenger
{
	public class Command
	{
		public virtual bool TabComplete(string input, out string[] possibilities)
		{
			possibilities = new string[0];
			return false;
		}


	}

	public class CommandAttribute : Attribute
	{
		public string Name { get; private set; }

		public CommandAttribute(string name)
		{
			Name = name;
		}
	}
}

