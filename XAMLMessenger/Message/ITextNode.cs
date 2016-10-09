using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XAMLMessenger.Message
{
    interface ITextNode : INode
    {
        string Text { get; set; }
    }

    static class TextNodeExtensions
    {
        public static string ToNodeString(this ITextNode self)
        {
            return $"[{(self is IAttributeNode ? (self as IAttributeNode).ToHeader() : self.Name)}]{self.Text}[/{self.Name}]";
        }
    }
}
