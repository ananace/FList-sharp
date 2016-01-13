using System;
using libflist.Connection.Commands;

namespace libflist.Events
{
	public class CharacterEntryEventArgs : EventArgs
	{
		public Channel Channel { get; private set; }
		public Character Character { get; private set; }
		public Command Command { get; private set; }

		public CharacterEntryEventArgs(Channel channel, Character character, Command command)
		{
			Channel = channel;
			Character = character;
			Command = command;
		}

		public CharacterEntryEventArgs(Character character, Command command)
		{
			Character = character;
			Command = command;
		}
	}

	public class CharacterEntryEventArgs<T> : EventArgs
	{
		public Channel Channel { get; private set; }
		public Character Character { get; private set; }
		public T Data { get; private set; }
		public T Old { get; set; }
		public Command Command { get; private set; }

		public CharacterEntryEventArgs(Channel channel, Character character, T data, Command command)
		{
			Channel = channel;
			Character = character;
			Data = data;
			Command = command;
		}

		public CharacterEntryEventArgs(Character character, T data, Command command)
		{
			Character = character;
			Data = data;
			Command = command;
		}
	}
}

