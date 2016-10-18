using System;
using libflist.FChat;
using libflist.Info;

namespace ConsoleMessenger.Types
{
	static class CharacterExtensions
	{

		public static char GetStatusChar(this Character character)
		{
				switch (character.Status)
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
		public static ConsoleColor GetStatusColor(this Character character)
		{
			switch (character.Status)
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
		public static ConsoleColor GetGenderColor(this Character character)
		{
			switch (character.Gender)
			{
				case Genders.Cuntboy:
					return ConsoleColor.Green;
				case Genders.Female:
					return ConsoleColor.Red;
				case Genders.Herm:
					return ConsoleColor.DarkMagenta;
				case Genders.Male:
					return ConsoleColor.DarkCyan;
				case Genders.MaleHerm:
					return ConsoleColor.Blue;
				case Genders.None:
					return ConsoleColor.Yellow;
				case Genders.Shemale:
					return ConsoleColor.Magenta;
				case Genders.Transgender:
					return ConsoleColor.DarkYellow;
			}

			return ConsoleColor.White;
		}

	}
}
