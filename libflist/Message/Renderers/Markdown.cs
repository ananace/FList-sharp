using System.Text;

namespace libflist.Message.Renderers
{
    public class Markdown : IRenderer<string>
    {
        bool _MarkdownToggle = false;
        char _MarkdownChar { get { return (_MarkdownToggle = !_MarkdownToggle) ? '*' : '_'; } }

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
        string ToMarkdown(Nodes.BoldNode node)
        {
            char c = _MarkdownChar;
            return $"{c}{c}{this.Render(node.Content)}{c}{c}";
        }
        string ToMarkdown(Nodes.EIconNode node)
        {
            return $"![:{node.Text}:]({node.IconUri})";
        }
        string ToMarkdown(Nodes.HeadingNode node)
        {
            return "## " + this.Render(node.Content);
        }
        string ToMarkdown(Nodes.IconNode node)
        {
            return $"![{node.Text}]({node.AvatarUri})";
        }
        string ToMarkdown(Nodes.ItalicNode node)
        {
            char c = _MarkdownChar;
            return "{c}" + this.Render(node.Content) + "{c}";
        }
        string ToMarkdown(Nodes.NoparseNode node)
        {
            return new Plain().Render(node.Content);
        }
        string ToMarkdown(Nodes.QuoteNode node)
        {
            return "> " + this.Render(node.Content).Replace("\n", "\n> ");
        }
        string ToMarkdown(Nodes.StrikethroughNode node)
        {
            return "~~" + this.Render(node.Content) + "~~";
        }
        string ToMarkdown(Nodes.URLNode node)
        {
            if (string.IsNullOrEmpty(node.Attribute))
                return node.Uri.ToString();
            return $"[{this.Render(node.Content)}]({node.Uri})";
        }
    }
}

