using System;
using System.Collections.Generic;
using System.Linq;

namespace libflist.Message.Nodes
{
	[Node("url")]
    public class URLNode : IContentNode, IAttributeNode
    {
        public string Attribute { get; set; }
        public ICollection<INode> Content { get; } = new List<INode>();

		public Uri Uri
		{
			get
			{
				if (!string.IsNullOrEmpty(Attribute))
					return new Uri(Attribute);
				else if (Content.Count == 1)
					return new Uri(Content.First().ToString(NodeStringType.Plain));

				return null;
			}
		}
    }
}
