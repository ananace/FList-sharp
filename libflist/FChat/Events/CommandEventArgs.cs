using System;

namespace libflist.FChat.Events
{
	public sealed class CommandEventArgs : EventArgs
	{
		public Commands.Command Command { get; private set; }

		public CommandEventArgs(Command cmd)
		{
			Command = cmd;
		}
	}
}
