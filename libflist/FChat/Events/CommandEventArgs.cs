using libflist.Connection.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
