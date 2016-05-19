using System;
using libflist.Connection.Commands;
using libflist.FChat;

namespace libflist.Events
{
	public class AdminActionEventArgs : EventArgs
	{
		public IChannel Channel { get; private set; }
		public ICharacter Character { get; private set; }
		public ICharacter Admin { get; private set; }
		public Command Command { get; private set; }

		public AdminActionEventArgs(IChannel channel, ICharacter character, ICharacter admin, Command command)
		{
			Channel = channel;
			Character = character;
			Admin = admin;
			Command = command;
		}
	}
}

