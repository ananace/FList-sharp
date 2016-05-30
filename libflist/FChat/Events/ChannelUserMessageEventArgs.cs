using System;
using libflist.FChat;

namespace libflist.Events
{
	public class CharacterMessageEventArgs : EventArgs
	{
		public FChat.Character Character { get; private set; }
		public string Message { get; private set; }
		public Command Command { get; private set; }

		public CharacterMessageEventArgs(FChat.Character character, string message, Command command)
		{
			Character = character;
			Message = message;
			Command = command;
		}
	}
}
