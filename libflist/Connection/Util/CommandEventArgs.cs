using System;
using libflist.Connection.Commands;

namespace libflist.Connection.Util
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

