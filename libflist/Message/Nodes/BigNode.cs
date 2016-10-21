using System.Collections.Generic;

namespace libflist.Message.Nodes
{
	[Node("big", Valid = NodeValidity.FList)]
	public class BigNode : IContentNode
	{
		public ICollection<INode> Content { get; } = new List<INode>();
	}
}
