using System.Text;

namespace libflist.Message.Renderers
{
    public class Plain : IRenderer<string>
    {
        public string Render(INode node)
        {
            if (node is IPlainTextNode)
                return (node as IPlainTextNode).Text;

            var build = new StringBuilder();

            var special = GetType().GetMethod("Render", new[] { node.GetType() });
            if (special != System.Reflection.MethodBase.GetCurrentMethod())
                build.Append(special.Invoke(this, new[] { node }) as string);
            else if (node is ITextNode)
                build.Append((node as ITextNode).Text);
            else if (node is IContentNode)
            {
                foreach (var child in (node as IContentNode).Content)
                    build.Append(Render(child));
            }
            else
                return build.ToString();

            return build.ToString();
        }

        string Render(Nodes.EIconNode node)
        {
            return $":{node.Text}:";
        }
        string Render(Nodes.IconNode node)
        {
            return $":{node.Text}:";
        }
        string Render(Nodes.URLNode node)
        {
            if (string.IsNullOrEmpty(node.Attribute))
                return node.Uri.ToString();
            return $"{this.Render(node.Content)} ({node.Uri})";
        }
    }
}

