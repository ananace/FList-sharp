using System.Collections.Generic;

namespace libflist.Message.Nodes
{
	[Node("quote", Valid = NodeValidity.FList)]
	public class QuoteNode : IContentNode, IParagraphNode
	{
		public ICollection<INode> Content { get; } = new List<INode>();
	}
}
