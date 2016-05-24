using System;
using System.Linq;
using libflist.FChat.Commands;
using libflist.Util;

namespace libflist.FChat
{
	public enum CharacterGender
	{
		Male,
		Female,
		Transgender,
		Herm,
		Shemale,
		[EnumValue("Male-Herm")]
		MaleHerm,
		[EnumValue("Cunt-boy")]
		Cuntboy,
		None
	}

	public enum CharacterOrientation
	{
		Straight,
		Gay,
		Bisexual,
		Asexual,
		Unsure,
		[EnumValue("Bi - male preference")]
		Bi_Male,
		[EnumValue("Bi - female preference")]
		Bi_Female,
		Pansexual,
		[EnumValue("Bi-curious")]
		Bi_Curious
	}

	public enum CharacterLanguage
	{
		Dutch,
		English,
		French,
		Spanish,
		German,
		Russian,
		Chinese,
		Japanese,
		Portugese,
		Korean,
		Arabic,
		Italian,
		Swedish,

		Other
	}

	public enum CharacterPreference
	{
		[EnumValue("No furry characters, just humans")]
		OnlyHumans,
		[EnumValue("No humans, just furry characters")]
		OnlyFurries,
		[EnumValue("Humans ok, Furries Preferred")]
		FurriesPreferred,
		[EnumValue("Furries ok, Humans Preferred")]
		HumansPreferred,
		[EnumValue("Furs and / or humans")]
		Any
	}

	public enum CharacterRole
	{
		[EnumValue("Always dominant")]
		AlwaysDom,
		[EnumValue("Usually dominant")]
		MostlyDom,
		Switch,
		[EnumValue("Usually submissive")]
		MostlySub,
		[EnumValue("Always submissive")]
		AlwaysSub,

		None
	}

	public enum CharacterStatus
	{
		Offline,
		Online,
		Looking,
		Busy,
		DND,
		Idle,
		Away,

		[EnumValue("crown")]
		Rewarded
	}

	public enum TypingStatus
	{
		Clear,
		Typing,
		Paused
	}
	
	public sealed class Character : IDisposable
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
		public TypingStatus IsTyping { get; internal set; }

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
			if (c == null)
				return false;
			return c.OPs.Contains(this);
		}

		public void SendMessage(string message)
		{
			Connection.SendCommand(new Client_PRI_CharacterSendMessage {
				Character = Name,
				Message = message
			});
		}
	}
}

