using libflist.FChat;
using libflist.Info;
using System;
using ConsoleMessenger.Types;

namespace ConsoleMessenger.UI
{
    static class CharacterExtensions
    {
		// TODO: Rework to be closer to the official FChat icons
		// TODO: Status icon
		public static ANSIString GetChanIcons(this Character ch, Channel chan = null)
		{
			if (ch.IsChatOp)
				return "▼".Color(ConsoleColor.Yellow);

			if (chan != null)
			{
				if (chan.Official)
				{
					if (chan.Owner == ch || ch.IsOPInChannel(chan))
						return "▼".Color(ConsoleColor.Red);
				}
				else
				{
					if (chan.Owner == ch)
						return "►".Color(ConsoleColor.Cyan);
					else if (ch.IsOPInChannel(chan))
						return "►".Color(ConsoleColor.Red);
				}
			}

			return null;
		}

        public static ANSIString ToANSIString(this Character ch, Channel chan = null, bool full = false)
        {
            return (ch.GetChanIcons(chan) ?? new ANSIString(" ")) + (full ? ch.Status.ToANSIString() : new ANSIString()) + ch.Name.Color(ch.Gender.ToColor());
        }

		public static string ToSortable(this Character ch, Channel chan)
		{
            int value = 10;
            if (ch.IsFriend || ch.IsBookmark)
                value = 1;
            else if (ch.IsChatOp || ch.IsOPInChannel(chan))
                value = 0;
            else if (ch.Status == CharacterStatus.Looking)
                value = 2;

			return $"{value}{ch.Name}";
		}

        public static ANSIString ToANSIString(this CharacterStatus status)
        {
            string icon;
            switch (status)
            {
                case CharacterStatus.Rewarded: icon = "♥"; break;
                default: icon = "●"; break;
            }

            return icon.Color(status.ToColor());
        }

        public static ConsoleColor ToColor(this CharacterStatus status)
        {
            switch (status)
            {
                case CharacterStatus.Away: return ConsoleColor.DarkBlue;
                case CharacterStatus.Busy: return ConsoleColor.DarkCyan;
                case CharacterStatus.DND: return ConsoleColor.DarkRed;
                case CharacterStatus.Idle: return ConsoleColor.DarkYellow;
                case CharacterStatus.Looking: return ConsoleColor.DarkGreen;
                case CharacterStatus.Offline: return ConsoleColor.Gray;
                case CharacterStatus.Online: return ConsoleColor.Black;

                case CharacterStatus.Rewarded: return ConsoleColor.White;
            }
            return ConsoleColor.White;
        }

        public static ConsoleColor ToColor(this Genders gender)
        {
            switch (gender)
            {
                case Genders.Cuntboy: return ConsoleColor.Green;
                case Genders.Female: return ConsoleColor.Red;
                case Genders.Herm: return ConsoleColor.DarkMagenta;
                case Genders.Male: return ConsoleColor.Blue;
                case Genders.MaleHerm: return ConsoleColor.DarkBlue;
                case Genders.None: return ConsoleColor.Yellow;
                case Genders.Shemale: return ConsoleColor.Magenta;
                case Genders.Transgender: return ConsoleColor.DarkYellow;
            }
            return ConsoleColor.White;
        }
    }
}
