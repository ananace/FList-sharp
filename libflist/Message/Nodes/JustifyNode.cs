using System.Collections.Generic;

namespace libflist.Message.Nodes
{
	[Node("justify", Valid = NodeValidity.FList)]
	public class JustifyNode : IContentNode, IParagraphNode
	{
		public ICollection<INode> Content { get; } = new List<INode>();
	}
}
