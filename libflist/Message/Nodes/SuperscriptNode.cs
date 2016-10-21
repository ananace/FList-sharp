using System.Collections.Generic;

namespace libflist.Message.Nodes
{
	[Node("sup")]
	public class SuperscriptNode : IContentNode
	{
		public ICollection<INode> Content { get; } = new List<INode>();
	}
}
