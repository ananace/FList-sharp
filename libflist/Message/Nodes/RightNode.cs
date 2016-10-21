using System.Collections.Generic;

namespace libflist.Message.Nodes
{
	[Node("right", Valid = NodeValidity.FList)]
	public class RightNode : IContentNode, IParagraphNode
	{
		public ICollection<INode> Content { get; } = new List<INode>();
	}
}
