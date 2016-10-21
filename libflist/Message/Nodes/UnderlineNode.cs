using System.Collections.Generic;

namespace libflist.Message.Nodes
{
	[Node("u")]
	public class UnderlineNode : IContentNode
	{
		public ICollection<INode> Content { get; } = new List<INode>();
	}
}
