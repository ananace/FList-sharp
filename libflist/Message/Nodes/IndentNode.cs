using System.Collections.Generic;

namespace libflist.Message.Nodes
{
	[Node("indent", Valid = NodeValidity.FList)]
	public class IndentNode : IContentNode, IParagraphNode
	{
		public ICollection<INode> Content { get; } = new List<INode>();
	}
}
