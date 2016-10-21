using System.Collections.Generic;

namespace libflist.Message.Nodes
{
	[Node("b")]
	public class BoldNode : IContentNode
	{
		public ICollection<INode> Content { get; } = new List<INode>();
	}
}
