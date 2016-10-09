using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Documents;

namespace XAMLMessenger.Message.Nodes
{
    class NoparseNode : IContentNode
    {
        public string Name { get; } = "noparse";
        public ICollection<INode> Content { get; } = new List<INode>();

        public Inline ToInline(libflist.FChat.Channel _chan)
        {
            return new Run(this.ToNodeString());
        }
    }
}
