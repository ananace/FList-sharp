﻿using System;
using libflist.FChat;
using libflist.FChat.Commands;

namespace libflist.Events
{
	public class ChannelUserMessageEventArgs : EventArgs
	{
		public Channel Channel { get; private set; }
		public Character Character { get; private set; }
		public string Message { get; private set; }
		public Command Command { get; private set; }

		public ChannelUserMessageEventArgs(Channel channel, Character character, string message, Command command)
		{
			Channel = channel;
			Character = character;
			Message = message;
			Command = command;
		}
	}
}
