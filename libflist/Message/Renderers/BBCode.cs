using System;
using System.Text;

namespace libflist.Message.Renderers
{
    public sealed class BBCode : IRenderer<string>
    {
        public string Render(INode node)
        {
            if (node is IPlainTextNode)
                return (node as IPlainTextNode).Text;

            StringBuilder build = new StringBuilder();

            build.Append('[');
            build.Append(node.GetName());

            if (node is IAttributeNode && !string.IsNullOrEmpty((node as IAttributeNode).Attribute))
            {
                build.Append('=');
                build.Append((node as IAttributeNode).Attribute);
            }

            build.Append(']');

            if (node is ITextNode)
                build.Append((node as ITextNode).Text);
            else if (node is IContentNode)
            {
                foreach (var child in (node as IContentNode).Content)
                    build.Append(Render(child));
            }
            else
                return build.ToString();

            build.Append("[/");
            build.Append(node.GetName());
            build.Append(']');

            return build.ToString();
        }
    }
}

