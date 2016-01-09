using System;
using libflist.Connection.Commands;

namespace libflist.Events
{
	public class CharacterMessageEventArgs : EventArgs
	{
		public Channel Channel { get; private set; }
		public Character Character { get; private set; }
		public string Message { get; private set; }
		public Command Command { get; private set; }

		public CharacterMessageEventArgs(Channel channel, Character character, string message, Command command)
		{
			Channel = channel;
			Character = character;
			Message = message;
			Command = command;
		}

		public CharacterMessageEventArgs(Character character, string message, Command command)
		{
			Character = character;
			Message = message;
			Command = command;
		}
	}
}

