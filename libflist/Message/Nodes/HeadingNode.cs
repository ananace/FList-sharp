using System.Collections.Generic;

namespace libflist.Message.Nodes
{
	[Node("heading", Valid = NodeValidity.FList)]
	public class HeadingNode : IContentNode, IParagraphNode
	{
		public ICollection<INode> Content { get; } = new List<INode>();
	}
}
