using System;
using libflist.FChat.Commands;

namespace libflist.FChat.Events
{
	public sealed class CommandEventArgs : EventArgs
	{
		public Command Command { get; private set; }

		public CommandEventArgs(Command cmd)
		{
			Command = cmd;
		}
	}
}
