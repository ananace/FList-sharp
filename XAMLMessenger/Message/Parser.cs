using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using XAMLMessenger.Message.Nodes;

namespace XAMLMessenger.Message
{
    internal class Parser
    {
        public class ParserException : Exception { }

        Stack<INode> TagStack;
        Stack<INode> TagContentStack;
        List<INode> Parsed;
        
        public IEnumerable<INode> ParseMessage(string Message)
        {
            TagStack = new Stack<INode>();
            TagContentStack = new Stack<INode>();
            Parsed = new List<INode>();

            for (int i = 0; i < Message.Length; ++i)
            {
                if (Message[i] != '[')
                {
                    StringBuilder plainText = new StringBuilder();

                    while (i < Message.Length && Message[i] != '[')
                        plainText.Append(Message[i++]);

                    AddText(plainText.ToString());
                }
                else
                {
                    i++;

                    if (i == Message.Length)
                    {
                        AddText("[");
                        break;
                    }

                    bool closing = (Message[i] == '/');

                    if (closing)
                        ++i;

                    StringBuilder tagName = new StringBuilder();
                    while (i < Message.Length && char.IsLetter(Message[i]))
                        tagName.Append(Message[i]);

                    if (i == Message.Length)
                        break;

                    if (!closing && (Message[i] == '=' || Message[i] == ']'))
                    {
                        INode tag = null;
                        switch (tagName.ToString())
                        {
                            case "b": tag = new TextEffectNode { FontWeight = FontWeights.Bold }; break;
                            case "i": tag = new TextEffectNode { FontStyle = FontStyles.Italic }; break;
                            case "u": tag = new TextEffectNode { TextDecorations = TextDecorations.Underline }; break;
                            case "s": tag = new TextEffectNode { TextDecorations = TextDecorations.Strikethrough }; break;
                            case "sup": tag = new TextEffectNode { BaselineAlignment = BaselineAlignment.Superscript }; break;
                            case "sub": tag = new TextEffectNode { BaselineAlignment = BaselineAlignment.Subscript }; break;
                            case "icon": tag = new IconNode(); break;
                            case "eicon": tag = new EIconNode(); break;
                            case "user": tag = new CharacterNode(); break;
                            case "color": tag = new TextEffectNode(); break;
                            case "url": tag = new URLNode(); break;
                            case "session": tag = new SessionNode(); break;
                            case "channel": tag = new SessionNode() { Name = "channel" }; break;

                            default: tag = new TextNode(); break;
                        }

                        if (TagContentStack.Peek() is IContentNode)
                            (TagContentStack.Peek() as IContentNode).Content.Push(tag);
                        else if (TagStack.Peek() is IContentNode)
                            (TagStack.Peek() as IContentNode).Content.Push(tag);
                        else
                            Parsed.Add(tag);
                        
                        TagContentStack.Push(tag);
                        TagStack.Push(tag);

                        if (Message[i] == ']')
                            continue;

                        if (Message[i] == '=')
                            ++i;

                        StringBuilder attrib = new StringBuilder();
                        while (i < Message.Length && Message[i] != ']')
                            attrib.Append(Message[i++]);

                        if (!string.IsNullOrEmpty(attrib.ToString()))
                        {
                            if (tag is IAttributeNode)
                                (tag as IAttributeNode).Attribute = attrib.ToString();
                            else if (tagName.ToString() == "color")
                                (tag as TextEffectNode).Color = attrib.ToString();
                        }
                    }
                    else if (closing && Message[i] == ']')
                    {
                        bool topMost = TagStack.Peek().Name.Equals(tagName.ToString(), StringComparison.CurrentCultureIgnoreCase);

                        if (!TagStack.Any() || !topMost)
                        {
                            // Unmatched tag
                            AddText($"[/{tagName.ToString()}]");
                            continue;
                        }

                        while (TagContentStack.Peek() != TagStack.Peek())
                            TagContentStack.Pop();

                        TagContentStack.Pop();
                        TagStack.Pop();
                    }
                    else
                    {
                        if (Message[i] == '[')
                            --i;

                        AddText($"[{tagName.ToString()}");
                    }
                }
            }

            return Parsed;
        }

        void AddText(string Text)
        {
            if (!TagContentStack.Any())
            {
                var node = new TextNode { Text = Text };
                TagContentStack.Push(node);
                Parsed.Add(node);
            }
            else
            {
                var top = TagContentStack.Peek();
                if (top is ITextNode)
                    (top as ITextNode).Text += Text;
                else if (top is IContentNode)
                {
                    var text = new TextNode { Text = Text };
                    (top as IContentNode).Content.Push(text);
                    TagContentStack.Push(text);
                }
                else
                    throw new ParserException();
            }
        }
    }
}
