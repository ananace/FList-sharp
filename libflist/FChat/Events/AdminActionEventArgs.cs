using System;
using libflist.FChat;

namespace libflist.Events
{
	public class AdminActionEventArgs : EventArgs
	{
		public FChat.Character Character { get; private set; }
		public FChat.Character Admin { get; private set; }
		public Command Command { get; private set; }

		public AdminActionEventArgs(FChat.Character character, FChat.Character admin, Command command)
		{
			Character = character;
			Admin = admin;
			Command = command;
		}
	}
}
