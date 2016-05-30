using System;
using libflist.FChat;

namespace libflist.Events
{
	public class ChannelAdminActionEventArgs : EventArgs
	{
		public Channel Channel { get; private set; }
		public FChat.Character Character { get; private set; }
		public FChat.Character Admin { get; private set; }
		public Command Command { get; private set; }

		public ChannelAdminActionEventArgs(Channel channel, FChat.Character character, FChat.Character admin, Command command)
		{
			Channel = channel;
			Character = character;
			Admin = admin;
			Command = command;
		}
	}
}
