using System;
using libflist.FChat;
using libflist.FChat.Commands;

namespace libflist.Events
{
	public class CharacterMessageEventArgs : EventArgs
	{
		public Character Character { get; private set; }
		public string Message { get; private set; }
		public Command Command { get; private set; }

		public CharacterMessageEventArgs(Character character, string message, Command command)
		{
			Character = character;
			Message = message;
			Command = command;
		}
	}
}

