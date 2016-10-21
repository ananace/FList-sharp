using System.Collections.Generic;

namespace libflist.Message.Nodes
{
	[Node("color")]
	public class ColorNode : IContentNode, IAttributeNode
	{
		public ICollection<INode> Content { get; } = new List<INode>();
		public string Attribute { get; set; }

		public bool IsValid
		{
			get
			{
				return _ValidColors.Contains(Attribute);
			}
		}

		static readonly ICollection<string> _ValidColors = new string[]
		{
			"red", "blue", "white", "yellow", "pink", "gray", "green", "orange", "purple",
			"black", "brown", "cyan"
		};
	}
}
