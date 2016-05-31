using System;
using libflist.FChat;

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
}
