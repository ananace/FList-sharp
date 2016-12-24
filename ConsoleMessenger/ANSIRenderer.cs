using System;
using System.Collections.Generic;
using ConsoleMessenger.Types;
using libflist.Message;
using libflist.Message.Nodes;

namespace ConsoleMessenger
{
    public class ANSIRenderer : IRenderer<ANSIString>
    {
        public ANSIString Render(IEnumerable<INode> nodes)
        {
            ANSIString ret = new ANSIString();
            foreach (var node in nodes)
                ret.Append(Render(node));

            return ret;
        }

        public ANSIString Render(INode node)
        {
            if (node is IPlainTextNode)
                return new ANSIString((node as IPlainTextNode).Text);

            ANSIString ret = new ANSIString();
            var special = GetType().GetMethod("Render", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic, Type.DefaultBinder, new[] { node.GetType() }, new System.Reflection.ParameterModifier[0]);
            if (special != null && special != System.Reflection.MethodBase.GetCurrentMethod())
                ret += (special.Invoke(this, new[] { node }) as ANSIString);
            else if (node is ITextNode)
                ret += (node as ITextNode).Text;
            else if (node is IContentNode)
            {
                foreach (var child in (node as IContentNode).Content)
                    ret += Render(child);
            }
            else
                return ret;

            return ret;
        }

        ANSIString Render(ColorNode node)
        {
            var _ColorLookup = new Dictionary<string, ConsoleColor>()
            {
                { "black", ConsoleColor.Black },
                { "blue", ConsoleColor.Blue },
                { "cyan", ConsoleColor.Cyan },
                { "purple", ConsoleColor.DarkMagenta },
                { "brown", ConsoleColor.DarkRed },
                { "orange", ConsoleColor.DarkYellow },
                { "gray", ConsoleColor.Gray },
                { "green", ConsoleColor.Green },
                { "pink", ConsoleColor.Magenta },
                { "red", ConsoleColor.Red },
                { "white", ConsoleColor.White },
                { "yellow", ConsoleColor.Yellow },
            };

            var content = Render(node.Content);
            content.ForegroundColor = _ColorLookup[node.Attribute];
            return content;
        }
    }
}

