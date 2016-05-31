using System;
using libflist.FChat;

namespace libflist.Events
{
	public class ChannelUserEntryEventArgs : EventArgs
	{
		public Channel Channel { get; private set; }
		public FChat.Character Character { get; private set; }
		public Command Command { get; private set; }

		public ChannelUserEntryEventArgs(Channel channel, FChat.Character character, Command command)
		{
			Channel = channel;
			Character = character;
			Command = command;
		}
	}

	public class ChannelUserEntryEventArgs<T> : EventArgs
	{
		public Channel Channel { get; private set; }
		public FChat.Character Character { get; private set; }
		public T Data { get; private set; }
		public Command Command { get; private set; }

		public ChannelUserEntryEventArgs(Channel channel, FChat.Character character, T data, Command command)
		{
			Channel = channel;
			Character = character;
			Data = data;
			Command = command;
		}
	}
}
