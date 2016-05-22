using System;
using libflist.FChat;
using libflist.FChat.Commands;

namespace libflist.Events
{
	public class CharacterEntryEventArgs : EventArgs
	{
		public Character Character { get; private set; }
		public Command Command { get; private set; }

		public CharacterEntryEventArgs(Character character, Command command)
		{
			Character = character;
			Command = command;
		}
	}

	public class CharacterEntryEventArgs<T> : EventArgs
	{
		public Character Character { get; private set; }
		public T Data { get; private set; }
		public Command Command { get; private set; }

		public CharacterEntryEventArgs(Character character, T data, Command command)
		{
			Character = character;
			Data = data;
			Command = command;
		}
	}
}

