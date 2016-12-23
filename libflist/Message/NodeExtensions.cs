using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace libflist.Message
{
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
    }

    public static class IRendererExtensions
    {
        public static string Render(this IRenderer<string> renderer, IEnumerable<INode> nodes)
        {
            var build = new StringBuilder();
            foreach (var node in nodes)
                build.Append(renderer.Render(node));
            return build.ToString();
        }
    }
}

