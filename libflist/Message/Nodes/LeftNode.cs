using System.Collections.Generic;

namespace libflist.Message.Nodes
{
	[Node("left", Valid = NodeValidity.FList)]
	public class LeftNode : IContentNode, IParagraphNode
	{
		public ICollection<INode> Content { get; } = new List<INode>();
	}
}
