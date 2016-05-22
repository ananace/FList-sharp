using System;
using libflist.FChat.Commands;

namespace libflist.Connection.Util
{
	public sealed class CommandEventArgs : EventArgs
	{
		public string Command { get; private set; }

		public CommandEventArgs(string cmd)
		{
			Command = cmd;
		}
	}
}

