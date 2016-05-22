using System;
using libflist.FChat;
using libflist.FChat.Commands;

namespace libflist.Events
{
	public class ChannelUserEntryEventArgs : EventArgs
	{
		public Channel Channel { get; private set; }
		public Character Character { get; private set; }
		public Command Command { get; private set; }

		public ChannelUserEntryEventArgs(Channel channel, Character character, Command command)
		{
			Channel = channel;
			Character = character;
			Command = command;
		}
	}

	public class ChannelUserEntryEventArgs<T> : EventArgs
	{
		public Channel Channel { get; private set; }
		public Character Character { get; private set; }
		public T Data { get; private set; }
		public Command Command { get; private set; }

		public ChannelUserEntryEventArgs(Channel channel, Character character, T data, Command command)
		{
			Channel = channel;
			Character = character;
			Data = data;
			Command = command;
		}
	}
}

