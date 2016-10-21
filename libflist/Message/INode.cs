using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace libflist.Message
{
    public interface INode
    {
    }

	public static class INodeExtensions
	{
		public static string GetName(this INode node)
		{
			return (node.GetType().GetCustomAttribute<NodeAttribute>().Name);
		}

		public static IEnumerable<string> GetNames(this INode node)
		{
			return (node.GetType().GetCustomAttributes<NodeAttribute>().Select(n => n.Name));
		}

		public static string ToString(this INode node, NodeStringType type = NodeStringType.BBCode)
		{
			if (node is IPlainTextNode)
				return (node as IPlainTextNode).Text;

			StringBuilder build = new StringBuilder();
			if (type == NodeStringType.BBCode)
			{
				build.Append('[');
				build.Append(node.GetName());

				if (node is IAttributeNode && !string.IsNullOrEmpty((node as IAttributeNode).Attribute))
				{
					build.Append('=');
					build.Append((node as IAttributeNode).Attribute);
				}

				build.Append(']');
			}

			if (node is Nodes.URLNode && type == NodeStringType.Plain)
			{
				var url = node as Nodes.URLNode;
				if (!string.IsNullOrEmpty(url.Attribute))
				{
					build.Append((url as IContentNode).ToString(type));
					build.Append(" (");
				}

				build.Append((node as Nodes.URLNode).Uri.ToString());

				if (!string.IsNullOrEmpty(url.Attribute))
					build.Append(")");
			}
			else if ((node is Nodes.IconNode || node is Nodes.EIconNode) && type == NodeStringType.Plain)
			{
				build.Append(":");
				build.Append((node as ITextNode).ToString(type));
				build.Append(":");
			}
			else if (node is ITextNode)
				build.Append((node as ITextNode).Text);
			else if (node is IContentNode)
			{
				foreach (var child in (node as IContentNode).Content)
					build.Append(child.ToString(type));
			}
			else
				return build.ToString();

			if (type == NodeStringType.BBCode)
			{
				build.Append("[/");
				build.Append(node.GetName());
				build.Append(']');
			}

			return build.ToString();
		}
	}
}
