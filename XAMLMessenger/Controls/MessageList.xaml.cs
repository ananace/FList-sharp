using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace XAMLMessenger.Controls
{
    /// <summary>
    /// Interaction logic for MessageList.xaml
    /// </summary>
    public partial class MessageList : UserControl
    {
        public FlowDocument Document => _document;

        public MessageList()
        {
            InitializeComponent();
        }

        public void AddMessage(string message)
        {
            var par = new Paragraph();
            par.Background = Brushes.DarkGray;
            par.Foreground = Brushes.DarkGray;

            par.Inlines.Add(new Run()
            {
                Text = DateTime.Now.ToShortTimeString(),
                FontStyle = FontStyles.Italic,
                Foreground = Brushes.White
            });
            par.Inlines.AddRange(ParseMessage(message));

            _document.Blocks.Add(par);
        }

        public IEnumerable<Inline> ParseMessage(string message)
        {
            List<Inline> ret = new List<Inline>();

            var parsed = new Parser().Parse(message);
            foreach (var node in parsed)
            {
                if (node is Parser.DataNode)
                {
                    switch (node.Name)
                    {
                        case "url":
                        case "user":
                        case "icon":
                        case "session":
                            {
                                string url = null;
                                string content = node.Content;

                                if (node.Name == "url")
                                {
                                    url = (node as Parser.LinkNode).URL;
                                    if (string.IsNullOrEmpty(url))
                                        url = node.Content;
                                }
                                else if (node.Name == "session")
                                {
                                    var room = (node as Parser.SessionNode).Room;
                                    if (string.IsNullOrEmpty(room))
                                        room = node.Content;

                                    content = room;
                                    url = node.Content;
                                }
                                else
                                {
                                    url = $"https://www.f-list.net/c/{content}";
                                }

                                var link = new Hyperlink();
                                if (node.Name == "session")
                                {
                                    link.ToolTip = content;
                                    link.Click += (s, e) =>
                                    {
                                        // Join the channel
                                        var chanId = url;
                                    };
                                }
                                else
                                {
                                    link.ToolTip = url;
                                    link.Click += (s, e) =>
                                    {
                                        System.Diagnostics.Process.Start(url);
                                    };
                                }

                                if (node.Name == "icon")
                                {
                                    link.Inlines.Add(new InlineUIContainer(new Image()
                                    {
                                        Source = new BitmapImage(new Uri($"https://static.f-list.net/images/avatar/{content}.png"))
                                    }));
                                }
                                else
                                {
                                    link.Inlines.Add(new Run()
                                    {
                                        Text = content
                                    });
                                }

                                ret.Add(link);
                            }
                            break;

                        case "eicon":
                            {
                                ret.Add(new InlineUIContainer(new Image()
                                {
                                    Source = new BitmapImage(new Uri($"https://static.f-list.net/images/icons/{node.Content}.png"))
                                }));
                            }
                            break;
                    }

                    continue;
                }

                Run run = new Run() { Text = node.Content };

                switch (node.Name)
                {
                    case "b":
                        run.FontWeight = FontWeights.Bold;
                        break;
                    case "i":
                        run.FontStyle = FontStyles.Italic;
                        break;
                    case "u":
                        run.TextDecorations = TextDecorations.Underline;
                        break;
                    case "s":
                        run.TextDecorations = TextDecorations.Strikethrough;
                        break;
                    case "sup":
                        run.BaselineAlignment = BaselineAlignment.Superscript;
                        break;
                    case "sub":
                        run.BaselineAlignment = BaselineAlignment.Subscript;
                        break;
                    case "color":
                        switch ((node as Parser.ColorNode).Color)
                        {
                            case "red": run.Foreground = Brushes.Red; break;
                            case "blue": run.Foreground = Brushes.Blue; break;
                            case "white": run.Foreground = Brushes.White; break;
                            case "yellow": run.Foreground = Brushes.Yellow; break;
                            case "pink": run.Foreground = Brushes.Pink; break;
                            case "gray": run.Foreground = Brushes.Gray; break;
                            case "green": run.Foreground = Brushes.Green; break;
                            case "orange": run.Foreground = Brushes.Orange; break;
                            case "purple": run.Foreground = Brushes.Purple; break;
                            case "black": run.Foreground = Brushes.Black; break;
                            case "brown": run.Foreground = Brushes.Brown; break;
                            case "cyan": run.Foreground = Brushes.Cyan; break;
                        }
                        break;
                }

                ret.Add(run);
            }

            return ret;
        }


        internal class Parser
        {
            internal abstract class Node
            {
                public string Content { get; set; }

                public abstract string Name { get; }
            }
            internal abstract class DataNode : Node { }

            internal class PlainNode : Node { public override string Name => ""; }

            internal class BoldNode : Node { public override string Name => "b"; }
            internal class ItalicNode : Node { public override string Name => "i"; }
            internal class UnderlineNode : Node { public override string Name => "u"; }
            internal class StrikethroughNode : Node { public override string Name => "s"; }
            internal class SuperscriptNode : Node { public override string Name => "sup"; }
            internal class SubscriptNode : Node { public override string Name => "sub"; }

            internal class IconNode : DataNode { public override string Name => "icon"; }
            internal class EIconNode : DataNode { public override string Name => "eicon"; }
            internal class UserNode : DataNode { public override string Name => "user"; }

            internal class ColorNode : Node
            {
                public string Color { get; set; }
                public override string Name => "color";
            }
            internal class LinkNode : DataNode
            {
                public string URL { get; set; }
                public override string Name => "url";
            }
            internal class SessionNode : DataNode
            {
                public string Room { get; set; }
                public override string Name => "session";
            }

            internal IEnumerable<Node> Parse(string text)
            {
                Stack<Node> ret = new Stack<Node>();

                for (int i = 0; i < text.Length; ++i)
                {
                    if (text[i] != '[')
                    {
                        StringBuilder plainText = new StringBuilder();

                        while (i < text.Length && text[i] != '[')
                            plainText.Append(text[i++]);

                        AddText(ret, plainText.ToString());
                    }
                    else
                    {
                        i++;

                        if (i == text.Length)
                        {
                            AddText(ret, "[");
                            break;
                        }

                        bool closing = (text[i] == '/');

                        if (closing)
                            ++i;

                        StringBuilder tagName = new StringBuilder();
                        while (i < text.Length && char.IsLetter(text[i]))
                            tagName.Append(text[i]);

                        if (i == text.Length)
                            break;

                        if (!closing && (text[i] == '=' || text[i] == ']'))
                        {
                            Node tag = null;
                            switch (tagName.ToString())
                            {
                                case "b": tag = new BoldNode(); break;
                                case "i": tag = new ItalicNode(); break;
                                case "u": tag = new UnderlineNode(); break;
                                case "s": tag = new StrikethroughNode(); break;
                                case "sup": tag = new SuperscriptNode(); break;
                                case "sub": tag = new SubscriptNode(); break;
                                case "icon": tag = new IconNode(); break;
                                case "eicon": tag = new EIconNode(); break;
                                case "user": tag = new UserNode(); break;
                                case "color": tag = new ColorNode(); break;
                                case "url": tag = new LinkNode(); break;
                                case "session": tag = new SessionNode(); break;

                                default: tag = new PlainNode(); break;
                            }
                            ret.Push(tag);

                            if (text[i] == ']')
                                continue;

                            if (text[i] == '=')
                                ++i;

                            StringBuilder attrib = new StringBuilder();
                            while (i < text.Length && text[i] != ']')
                                attrib.Append(text[i++]);

                            switch (tagName.ToString())
                            {
                                case "color":
                                    (tag as ColorNode).Color = attrib.ToString(); break;
                                case "url":
                                    (tag as LinkNode).URL = attrib.ToString(); break;
                                case "session":
                                    (tag as SessionNode).Room = attrib.ToString(); break;
                            }
                        }
                        else if (closing && text[i] == ']')
                        {
                            if (ret.Any() && ret.Peek().Name != tagName.ToString())
                            {
                                // Unmatched tag
                                AddText(ret, $"[/{tagName.ToString()}]");
                                continue;
                            }

                            ret.Push(new PlainNode());
                        }
                        else
                        {
                            if (text[i] == '[')
                                --i;

                            AddText(ret, $"[{tagName.ToString()}");
                        }
                    }
                }

                return ret.Where((n) => { return !string.IsNullOrEmpty(n.Content); });
            }

            void AddText(Stack<Node> stack, string text)
            {
                if (!stack.Any())
                    stack.Push(new PlainNode() { Content = text });
                else
                    stack.Peek().Content += text;
            }
        }
    }
}
