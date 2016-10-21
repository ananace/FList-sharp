using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using libflist.Message.Nodes;
using System.Reflection;
using System.Diagnostics;

namespace libflist.Message
{
    public class Parser
    {
        public class ParserException : Exception { }
		public NodeValidity Validity { get; set; } = NodeValidity.Both;

		static IDictionary<NodeAttribute, Type> _RegisteredNodeTypes = new Dictionary<NodeAttribute, Type>();
		static Parser()
		{
			RegisterNodeTypes(Assembly.GetAssembly(typeof(Parser)).GetTypes().Where(t => !t.IsInterface && t.GetInterface(typeof(INode).FullName) != null));
		}

		public static void RegisterNodeTypes(IEnumerable<Type> nodeTypes = null)
		{
			if (nodeTypes == null)
				nodeTypes = Assembly.GetCallingAssembly().GetTypes().Where(t => !t.IsInterface && t.GetInterface(typeof(INode).FullName) != null);

			foreach (var type in nodeTypes)
			{
				var atts = type.GetCustomAttributes<NodeAttribute>();
				if (!atts.Any())
				{
					Debug.WriteLine("Tried to register {0} as node type, missing node attribute.", type.FullName);
					continue;
				}

				foreach (var node in atts)
				{
					if (_RegisteredNodeTypes.ContainsKey(node))
						Debug.WriteLine("Overriding node type {0} ({1}) with {2}", node.Name, _RegisteredNodeTypes[node].FullName, type.FullName);
					_RegisteredNodeTypes[node] = type;
				}
			}
		}

		IDictionary<string, Type> AvailableNodeTypes
		{
			get
			{
				return _RegisteredNodeTypes.Where(n => (n.Key.Valid & Validity) != 0).ToDictionary(k => k.Key.Name, v => v.Value);
			}
		}
        
        public IEnumerable<INode> ParseMessage(string Message)
        {
			Stack<INode> TagStack = new Stack<INode>();
			Stack<INode> TagContentStack = new Stack<INode>();
			List<INode> Parsed = new List<INode>();

            for (int i = 0; i < Message.Length;)
            {
                if (Message[i] == ']' && i > 0)
                    ++i;

                if (i == Message.Length)
                    break;

                if (Message[i] != '[')
                {
                    StringBuilder plainText = new StringBuilder();

                    while (i < Message.Length && Message[i] != '[')
                        plainText.Append(Message[i++]);

                    AddText(plainText.ToString(), TagContentStack, Parsed);
                }
                else
                {
                    i++;

                    if (i == Message.Length)
                    {
                        AddText("[", TagContentStack, Parsed);
                        break;
                    }

                    bool closing = (Message[i] == '/');

                    if (closing)
                        ++i;

                    StringBuilder tagName = new StringBuilder();
                    while (i < Message.Length && char.IsLetter(Message[i]))
                        tagName.Append(Message[i++]);

                    if (i == Message.Length)
                        break;

                    if (!closing && (Message[i] == '=' || Message[i] == ']'))
                    {
                        INode tag = null;
						Type tagType = AvailableNodeTypes.FirstOrDefault(n => n.Key.Equals(tagName.ToString(), StringComparison.CurrentCultureIgnoreCase)).Value;

						if (tagType != null)
						{
							var constructor = tagType.GetConstructor(new Type[] { typeof(Parser) });
							if (constructor == null)
							{
								constructor = tagType.GetConstructor(Type.EmptyTypes);
								tag = constructor.Invoke(null) as INode;
							}
							else
								tag = constructor.Invoke(new[] { this }) as INode;

							if (TagContentStack.Any() && TagContentStack.Peek() is ITextNode)
								TagContentStack.Pop();

							if (TagContentStack.Any() && TagContentStack.Peek() is IContentNode)
								(TagContentStack.Peek() as IContentNode).Content.Add(tag);
							else if (TagStack.Any() && TagStack.Peek() is IContentNode)
								(TagStack.Peek() as IContentNode).Content.Add(tag);
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
							}
						}
						else
						{
							AddText($"[{tagName.ToString()}", TagContentStack, Parsed);

							if (Message[i] == ']')
							{
								AddText("]", TagContentStack, Parsed);
								continue;
							}

							StringBuilder crap = new StringBuilder();
							while (i < Message.Length && Message[i] != ']')
								crap.Append(Message[i++]);

							if (Message[i] == ']')
								crap.Append(']');

							AddText(crap.ToString(), TagContentStack, Parsed);
						}
					}
                    else if (closing && Message[i] == ']')
                    {
                        bool topMost = TagStack.Peek().GetNames().FirstOrDefault(s => s.Equals(tagName.ToString(), StringComparison.CurrentCultureIgnoreCase)) != null;

                        if (!TagStack.Any() || !topMost)
                        {
                            // Unmatched tag
                            AddText($"[/{tagName.ToString()}]", TagContentStack, Parsed);
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

                        AddText($"[{tagName.ToString()}", TagContentStack, Parsed);
                    }
                }
            }

            return Parsed;
        }

        void AddText(string Text, Stack<INode> TagContentStack, List<INode> Parsed)
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
                    (top as IContentNode).Content.Add(text);
                    TagContentStack.Push(text);
                }
                else
                    throw new ParserException();
            }
        }
    }
}
