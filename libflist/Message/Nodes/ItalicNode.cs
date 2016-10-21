using System.Collections.Generic;

namespace libflist.Message.Nodes
{
	[Node("i")]
	public class ItalicNode : IContentNode
	{
		public ICollection<INode> Content { get; } = new List<INode>();
	}
}
