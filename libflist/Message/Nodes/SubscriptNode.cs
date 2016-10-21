using System.Collections.Generic;

namespace libflist.Message.Nodes
{
	[Node("sub")]
	public class SubscriptNode : IContentNode
	{
		public ICollection<INode> Content { get; } = new List<INode>();
	}
}
