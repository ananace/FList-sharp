using System.Collections.Generic;

namespace libflist.Message.Nodes
{
	[Node("collapse", Valid = NodeValidity.FList)]
	public class CollapseNode : IContentNode, IAttributeNode, IParagraphNode
	{
		public ICollection<INode> Content { get; } = new List<INode>();
		public string Attribute { get; set; }
	}
}
