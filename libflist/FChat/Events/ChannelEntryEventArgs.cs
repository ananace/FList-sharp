using System;
using libflist.FChat;
using libflist.FChat.Commands;

namespace libflist.Events
{
	public class ChannelEntryEventArgs : EventArgs
	{
		public Channel Channel { get; private set; }
		public Command Command { get; private set; }

		public ChannelEntryEventArgs(Channel channel, Command command)
		{
			Channel = channel;
			Command = command;
		}
	}

	public class ChannelEntryEventArgs<T> : EventArgs
	{
		public Channel Channel { get; private set; }
		public T Data { get; private set; }
		public Command Command { get; private set; }

		public ChannelEntryEventArgs(Channel channel, T data, Command command)
		{
			Channel = channel;
			Data = data;
			Command = command;
		}
	}
}

