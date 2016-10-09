using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Documents;

namespace XAMLMessenger.Message.Nodes
{
    class URLNode : IContentNode, IAttributeNode
    {
        public string Name { get; } = "url";
        public string Attribute { get; set; }
        public Stack<INode> Content { get; } = new Stack<INode>();

        public string Uri { get
            {
                return Attribute ?? Content.Where(n => n is ITextNode).Select(n => (n as ITextNode).Text).FirstOrDefault();
            } }

        public Inline ToInline(libflist.FChat.Channel _chan)
        {
            var link = new Hyperlink()
            {
                NavigateUri = new Uri(Uri),
                ToolTip = Uri
            };

            foreach (var node in Content.Select(n => n.ToInline(_chan)))
                link.Inlines.Add(node);

            return link;
        }
    }
}
