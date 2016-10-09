using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media.Imaging;

namespace XAMLMessenger.Message.Nodes
{
    class IconNode : ITextNode
    {
        public string Name { get; } = "icon";
        public string Text { get; set; }

        public Inline ToInline(libflist.FChat.Channel _chan)
        {
            return new Hyperlink(new InlineUIContainer(new Image
            {
                Width = 30,
                Height = 30,
                Source = new BitmapImage(new Uri("$https://static.f-list.net/images/avatar/{Uri.EscapeUriString(Text.ToLower())}.png"))
            }))
            {
                NavigateUri = new Uri($"flist://character/{Uri.EscapeUriString(Text.ToLower())}"),
                ToolTip = Text
            };
        }
    }
}
