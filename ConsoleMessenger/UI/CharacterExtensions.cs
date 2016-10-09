using libflist.FChat;
using libflist.Info;
using System;

namespace ConsoleMessenger.UI
{
    static class CharacterExtensions
    {
        public static string ToANSIString(this Character ch)
        {
            return ch.Name.Color(ch.Gender.ToColor());
        }

        public static string ToANSIString(this CharacterStatus status)
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
                case CharacterStatus.Away: return ConsoleColor.DarkGray;
                case CharacterStatus.Busy: return ConsoleColor.DarkYellow;
                case CharacterStatus.DND: return ConsoleColor.DarkRed;
                case CharacterStatus.Idle: return ConsoleColor.DarkCyan;
                case CharacterStatus.Looking: return ConsoleColor.DarkGreen;
                case CharacterStatus.Offline: return ConsoleColor.Gray;
                case CharacterStatus.Online: return ConsoleColor.Black;
                case CharacterStatus.Rewarded: return ConsoleColor.Yellow;
            }
            return ConsoleColor.White;
        }

        public static ConsoleColor ToColor(this Genders gender)
        {
            switch (gender)
            {
                case Genders.Cuntboy: return ConsoleColor.Green;
                case Genders.Female: return ConsoleColor.Red;
                case Genders.Herm: return ConsoleColor.Magenta;
                case Genders.Male: return ConsoleColor.Blue;
                case Genders.MaleHerm: return ConsoleColor.DarkBlue;
                case Genders.None: return ConsoleColor.Yellow;
                case Genders.Shemale: return ConsoleColor.DarkMagenta;
                case Genders.Transgender: return ConsoleColor.DarkYellow;
            }
            return ConsoleColor.White;
        }
    }
}
