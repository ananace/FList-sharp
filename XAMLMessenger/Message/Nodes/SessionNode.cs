using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;
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
                    return new Int32Rect(96, 0, 24, 24);
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
						Source = new CroppedBitmap(App.Current.CombinedImageResource, ImageRect),
						Width = 16,
						Height = 16
					}) {
						BaselineAlignment = BaselineAlignment.Bottom
					},
					new Run(SessionName)
				}
			})
			{
				Foreground = Brushes.White,
				FontWeight = FontWeights.Bold,
				TextDecorations = null,

                NavigateUri = new Uri($"flist://session/{Uri.EscapeUriString(SessionADH.ToLower())}"),
                ToolTip = SessionName
            };
        }
    }
}
