using System.Collections.Generic;

namespace libflist.Message.Nodes
{
	[Node("center", Valid = NodeValidity.FList)]
	public class CenterNode : IContentNode, IParagraphNode
	{
		public ICollection<INode> Content { get; } = new List<INode>();
	}
}
