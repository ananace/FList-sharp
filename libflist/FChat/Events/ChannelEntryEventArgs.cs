using System;
using libflist.Connection.Commands;
using libflist.FChat;

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

		public ChannelEntryEventArgs(Command command)
		{
			Command = command;
		}
	}

	public class ChannelEntryEventArgs<T> : EventArgs
	{
		public Channel Channel { get; private set; }
		public T Data { get; private set; }
		public T Old { get; set; }
		public Command Command { get; private set; }

		public ChannelEntryEventArgs(Channel channel, T data, Command command)
		{
			Channel = channel;
			Data = data;
			Command = command;
		}

		public ChannelEntryEventArgs(T data, Command command)
		{
			Data = data;
			Command = command;
		}
	}
}

