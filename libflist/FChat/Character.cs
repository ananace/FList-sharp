using System;
using System.Linq;
using libflist.Connection.Types;

namespace libflist.FChat
{
	public class Character : IDisposable
	{
		public FChatConnection Connection { get; private set; }

		public string Name { get; private set; }
		public CharacterGender Gender { get; internal set; }
		public CharacterStatus Status { get; internal set; }
		public string StatusMessage { get; internal set; }

		public char StatusChar
		{
			get
			{
				switch (Status)
				{
				case CharacterStatus.Away: return '●';
				case CharacterStatus.Busy: return '●';
				case CharacterStatus.DND: return '●';
				case CharacterStatus.Idle: return '●';
				case CharacterStatus.Looking: return '●';
				case CharacterStatus.Rewarded: return '☺';
				}

				return '○';
			}
		}
		public ConsoleColor StatusColor
		{
			get
			{
				switch (Status)
				{
					case CharacterStatus.Away: return ConsoleColor.Gray;
					case CharacterStatus.Busy: return ConsoleColor.Cyan;
					case CharacterStatus.DND: return ConsoleColor.Red;
					case CharacterStatus.Idle: return ConsoleColor.Yellow;
					case CharacterStatus.Looking: return ConsoleColor.Green;
					case CharacterStatus.Rewarded: return ConsoleColor.DarkYellow;
				}

				return ConsoleColor.DarkGray;
			}
		}
		public ConsoleColor GenderColor
		{
			get
			{
				switch (Gender)
				{
					case CharacterGender.Cuntboy:
						return ConsoleColor.Green;
					case CharacterGender.Female:
						return ConsoleColor.Red;
					case CharacterGender.Herm:
						return ConsoleColor.DarkMagenta;
					case CharacterGender.Male:
						return ConsoleColor.Blue;
					case CharacterGender.MaleHerm:
						return ConsoleColor.DarkBlue;
					case CharacterGender.None:
						return ConsoleColor.Yellow;
					case CharacterGender.Shemale:
						return ConsoleColor.Magenta;
					case CharacterGender.Transgender:
						return ConsoleColor.DarkYellow;
				}

				return ConsoleColor.White;
			}
		}

		public bool IsDisposed { get; private set; }
		public bool IsTyping { get; internal set; }

		internal Character(FChatConnection Connection, string Name)
		{
			this.Connection = Connection;
			this.Name = Name;
		}

		public void Dispose()
		{
			Name = null;
			Connection = null;

			IsDisposed = true;
		}

		public bool IsOPInChannel(Channel c)
		{
			return c.OPs.Contains(this);
		}

		public void SendMessage(string message)
		{
			Connection.SendCommand(new Commands.Client.Character.SendMessageCommand {
				Character = Name,
				Message = message
			});
		}
	}
}

