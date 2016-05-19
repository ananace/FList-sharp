using System;
using libflist.FChat;

namespace libflist.Events
{
	public class CharacterMessageEventArgs : EventArgs
	{
		public IChannel Channel { get; private set; }
		public ICharacter Character { get; private set; }
		public string Message { get; private set; }
		public ICommand Command { get; private set; }

		public CharacterMessageEventArgs(IChannel channel, ICharacter character, string message, ICommand command)
		{
			Channel = channel;
			Character = character;
			Message = message;
			Command = command;
		}

		public CharacterMessageEventArgs(ICharacter character, string message, ICommand command)
		{
			Character = character;
			Message = message;
			Command = command;
		}
	}
}

