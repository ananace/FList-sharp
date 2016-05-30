using System;
using libflist.FChat;

namespace libflist.Events
{
	public class ChannelUserMessageEventArgs : EventArgs
	{
		public Channel Channel { get; private set; }
		public FChat.Character Character { get; private set; }
		public string Message { get; private set; }
		public Command Command { get; private set; }

		public ChannelUserMessageEventArgs(Channel channel, FChat.Character character, string message, Command command)
		{
			Channel = channel;
			Character = character;
			Message = message;
			Command = command;
		}
	}
}
