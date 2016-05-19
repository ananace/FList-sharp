using System;
using libflist.Connection.Commands;
using libflist.FChat;

namespace libflist.Events
{
	public class CharacterEntryEventArgs : EventArgs
	{
		public IChannel Channel { get; private set; }
		public ICharacter Character { get; private set; }
		public Command Command { get; private set; }

		public CharacterEntryEventArgs(IChannel channel, ICharacter character, Command command)
		{
			Channel = channel;
			Character = character;
			Command = command;
		}

		public CharacterEntryEventArgs(ICharacter character, Command command)
		{
			Character = character;
			Command = command;
		}
	}

	public class CharacterEntryEventArgs<T> : EventArgs
	{
		public IChannel Channel { get; private set; }
		public ICharacter Character { get; private set; }
		public T Data { get; private set; }
		public T Old { get; set; }
		public Command Command { get; private set; }

		public CharacterEntryEventArgs(IChannel channel, ICharacter character, T data, Command command)
		{
			Channel = channel;
			Character = character;
			Data = data;
			Command = command;
		}

		public CharacterEntryEventArgs(ICharacter character, T data, Command command)
		{
			Character = character;
			Data = data;
			Command = command;
		}
	}
}

