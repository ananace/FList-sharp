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
    class URLNode : IContentNode, IAttributeNode
    {
        public string Name { get; } = "url";
        public string Attribute { get; set; }
        public ICollection<INode> Content { get; } = new List<INode>();

        public string Uri { get
            {
                return Attribute ?? Content.Where(n => n is ITextNode).Select(n => (n as ITextNode).Text).FirstOrDefault();
            } }

        public Inline ToInline(libflist.FChat.Channel _chan)
        {
            try
            {
                var link = new Hyperlink()
                {
                    NavigateUri = new Uri(Uri),
                    ToolTip = Uri
                };

                link.Inlines.Add(new InlineUIContainer(new Image
                {
                    Source = new CroppedBitmap(App.Current.StaticImageResource, new System.Windows.Int32Rect(72, 96, 24, 24)),
                    Width = 16,
                    Height = 16,
                })
                {
                    BaselineAlignment = System.Windows.BaselineAlignment.TextBottom
                });
                foreach (var node in Content.Select(n => n.ToInline(_chan)))
                    link.Inlines.Add(node);

                return link;
            }
            catch(Exception ex)
            {
                return new Run($"{(this as IContentNode).ToNodeString()}") { ToolTip = ex.ToString() };
            }
        }
    }
}
