using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Documents;

namespace XAMLMessenger.Message.Nodes
{
    class TextNode : ITextNode
    {
        public string Name { get; } = "text";
        public string Text { get; set; }

        public Inline ToInline(libflist.FChat.Channel _chan)
        {
            return new Run(Text);
        }
    }
}
