using System.Collections.Generic;

namespace libflist.Message.Nodes
{
	[Node("s")]
	public class StrikethroughNode : IContentNode
	{
		public ICollection<INode> Content { get; } = new List<INode>();
	}
}
