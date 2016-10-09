using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XAMLMessenger.Message
{
    interface IAttributeNode : INode
    {
        string Attribute { get; set; }
    }

    static class AttributeNodeExtensions
    {
        public static string ToHeader(this IAttributeNode self)
        {
            return $"{self.Name}={self.Attribute}";
        }
    }
}
