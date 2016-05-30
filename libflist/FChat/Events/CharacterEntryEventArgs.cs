using System;
using libflist.FChat;

namespace libflist.Events
{
	public class CharacterEntryEventArgs : EventArgs
	{
		public FChat.Character Character { get; private set; }
		public Command Command { get; private set; }

		public CharacterEntryEventArgs(FChat.Character character, Command command)
		{
			Character = character;
			Command = command;
		}
	}

	public class CharacterEntryEventArgs<T> : EventArgs
	{
		public FChat.Character Character { get; private set; }
		public T Data { get; private set; }
		public Command Command { get; private set; }

		public CharacterEntryEventArgs(FChat.Character character, T data, Command command)
		{
			Character = character;
			Data = data;
			Command = command;
		}
	}
}
