using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace XAMLMessenger.Message.Nodes
{
    class CharacterNode : ITextNode
    {
        public string Name { get; } = "character";
        public string Text { get; set; }

        public Inline ToInline(libflist.FChat.Channel chan)
        {
            var ch = App.Current.FChatClient.GetCharacter(Text);
            var sym = _CharacterSymbol(ch, chan);
            var span = new Span
            {
                Inlines = {
                    new InlineUIContainer(new Image {
                        Source = new CroppedBitmap(App.Current.CombinedImageResource, _CharacterStatus(ch.Status)),
                        Margin = new Thickness(5, 0, 5, 0)
                    }),
                    new Run(ch.Name) {
                        Foreground = _CharacterBrush(ch.Gender)
                    }
                }
            };
            if (sym.HasValue)
                span.Inlines.InsertBefore(span.Inlines.LastInline, new InlineUIContainer(new Image
                {
                    Source = new CroppedBitmap(App.Current.CombinedImageResource, sym.Value),
                    Width = 24,
                    Height = 24,
                }));

            return new Hyperlink(span)
            {
				Foreground = Brushes.White,
				TextDecorations = null,

                NavigateUri = new Uri($"flist://character/{Uri.EscapeUriString(ch.Name.ToLower())}"),
                ToolTip = ch.Name
            };
        }

        Int32Rect? _CharacterSymbol(libflist.FChat.Character ch, libflist.FChat.Channel chan)
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

        Int32Rect _CharacterStatus(libflist.FChat.CharacterStatus status)
        {
            switch (status)
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

        Brush _CharacterBrush(libflist.Info.Genders gender)
        {
            switch (gender)
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
