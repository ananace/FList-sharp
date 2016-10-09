using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XAMLMessenger.Message
{
    interface IContentNode : INode
    {
        ICollection<INode> Content { get; }
    }

    static class ContentNodeExtensions
    {
        public static string ToNodeString(this IContentNode self)
        {
            var content = self.Content.Select(n =>
            {
                if (n is Nodes.TextNode)
                    return (n as Nodes.TextNode).ToNodeString();
                else if (n is IContentNode)
                    return (n as IContentNode).ToNodeString();
                else if (n is ITextNode)
                    return (n as ITextNode).ToNodeString();
                else
                    return "";
            });
            return $"[{(self is IAttributeNode ? (self as IAttributeNode).ToHeader() : self.Name)}]{string.Join("", content)}[/{self.Name}]";
        }
    }
}
