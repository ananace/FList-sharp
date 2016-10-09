using System;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media.Imaging;

namespace XAMLMessenger.Message.Nodes
{
    class EIconNode : ITextNode
    {
        public string Name { get; } = "eicon";
        public string Text { get; set; }

        public Inline ToInline(libflist.FChat.Channel _chan)
        {
            return new InlineUIContainer(new Image
            {
                Width = 30,
                Height = 30,
                Source = new BitmapImage(new Uri($"https://static.f-list.net/images/icons/{Text}.png")),
                ToolTip = Text
            });
        }
    }
}
