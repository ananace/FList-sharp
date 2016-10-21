using System.Windows;
using System.Windows.Media;

namespace XAMLMessenger.Message
{
	static class CharacterExtensions
	{
        public static Int32Rect? CharacterSymbolRect(this libflist.FChat.Character ch, libflist.FChat.Channel chan)
        {
            if (ch.IsChatOp)
                return new Int32Rect(216, 72, 24, 24);

            if (chan != null)
            {
                if (chan.Owner == ch)
                    return new Int32Rect(288, 74, 24, 24);

                if (ch.IsOPInChannel(chan))
                    return new Int32Rect(264, 72, 24, 24);
            }

            return null;
        }

        public static Int32Rect CharacterStatusRect(this libflist.FChat.Character character)
        {
            switch (character.Status)
            {
                case libflist.FChat.CharacterStatus.Away: return new Int32Rect(24, 72, 24, 24);
                case libflist.FChat.CharacterStatus.Busy: return new Int32Rect(72, 72, 24, 24);
                case libflist.FChat.CharacterStatus.DND: return new Int32Rect(168, 72, 24, 24);
                case libflist.FChat.CharacterStatus.Idle: return new Int32Rect(0, 72, 24, 24);
                case libflist.FChat.CharacterStatus.Looking: return new Int32Rect(48, 72, 24, 24);
                case libflist.FChat.CharacterStatus.Offline: return new Int32Rect(120, 72, 24, 24);
                case libflist.FChat.CharacterStatus.Online: return new Int32Rect(144, 72, 24, 24);
                case libflist.FChat.CharacterStatus.Rewarded: return new Int32Rect(168, 24, 24, 24);
            }
            return new Int32Rect(96, 72, 24, 24);
        }

        public static Brush CharacterGenderBrush(this libflist.FChat.Character character)
        {
            switch (character.Gender)
            {
                case libflist.Info.Genders.Cuntboy: return Brushes.Green;
                case libflist.Info.Genders.Female: return Brushes.Pink;
                case libflist.Info.Genders.Herm: return Brushes.Purple;
                case libflist.Info.Genders.Male: return Brushes.LightBlue;
                case libflist.Info.Genders.MaleHerm: return Brushes.DarkBlue;
                case libflist.Info.Genders.None: return Brushes.Yellow;
                case libflist.Info.Genders.Shemale: return Brushes.MediumPurple;
                case libflist.Info.Genders.Transgender: return Brushes.Orange;
            }
            return Brushes.White;
        }
	}
}
