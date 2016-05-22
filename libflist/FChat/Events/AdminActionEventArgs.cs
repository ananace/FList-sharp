using System;
using libflist.FChat;
using libflist.FChat.Commands;

namespace libflist.Events
{
	public class AdminActionEventArgs : EventArgs
	{
		public Channel Channel { get; private set; }
		public Character Character { get; private set; }
		public Character Admin { get; private set; }
		public Command Command { get; private set; }

		public AdminActionEventArgs(Channel channel, Character character, Character admin, Command command)
		{
			Channel = channel;
			Character = character;
			Admin = admin;
			Command = command;
		}
	}
}

