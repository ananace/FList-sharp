using System;
using libflist.FChat;
using libflist.FChat.Commands;

namespace libflist.Events
{
	public class AdminActionEventArgs : EventArgs
	{
		public Character Character { get; private set; }
		public Character Admin { get; private set; }
		public Command Command { get; private set; }

		public AdminActionEventArgs(Character character, Character admin, Command command)
		{
			Character = character;
			Admin = admin;
			Command = command;
		}
	}
}

