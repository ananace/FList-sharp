using System.Collections.Generic;

namespace libflist.Message.Nodes
{
	[Node("noparse")]
    public class NoparseNode : IContentNode
    {
        public ICollection<INode> Content { get; } = new List<INode>();
    }
}
