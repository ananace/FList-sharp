using System.Collections.Generic;

namespace libflist.Message.Nodes
{
	[Node("small", Valid = NodeValidity.FList)]
	public class SmallNode : IContentNode
	{
		public ICollection<INode> Content { get; } = new List<INode>();
	}
}
