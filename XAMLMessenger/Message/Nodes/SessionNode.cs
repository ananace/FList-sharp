using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media.Imaging;

namespace XAMLMessenger.Message.Nodes
{
    class SessionNode : ITextNode, IAttributeNode
    {
        public string Name { get; set; } = "session";

        public string Text { get; set; }
        public string Attribute { get; set; }

        public string SessionADH { get { return Text ?? Attribute; } }
        public string SessionName { get { return Attribute ?? Text; } }

        public bool IsOfficial { get { return SessionName == SessionADH || string.IsNullOrEmpty(SessionADH); } }
        Int32Rect ImageRect { get
            {
                if (IsOfficial)
                    return new Int32Rect(72, 24, 24, 24);
                return new Int32Rect(0, 24, 24, 24);
            } }

        public Inline ToInline(libflist.FChat.Channel _chan)
        {
            return new Hyperlink(new Span
            {
                Inlines =
                {
                    new InlineUIContainer(new Image
                    {
                        Source = new CroppedBitmap(App.Current.StaticImageResource, ImageRect),
                        Width = 24,
                        Height = 24
                    }),
                    new Run(SessionName)
                }
            })
            {
                NavigateUri = new Uri($"flist://session/{Uri.EscapeUriString(SessionADH.ToLower())}"),
                ToolTip = SessionName
            };
        }
    }
}
