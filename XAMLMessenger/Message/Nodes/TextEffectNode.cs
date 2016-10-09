using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Media;

namespace XAMLMessenger.Message.Nodes
{
    class TextEffectNode : IContentNode
    {
        public string Name { get; set; }
        public Stack<INode> Content { get; } = new Stack<INode>();

        public string Color { get; set; } = "White";
        public FontWeight FontWeight { get; set; } = FontWeights.Regular;
        public FontStyle FontStyle { get; set; } = FontStyles.Normal;
        public BaselineAlignment BaselineAlignment { get; set; } = BaselineAlignment.Baseline;
        public TextDecorationCollection TextDecorations { get; set; } = System.Windows.TextDecorations.Baseline;

        public Inline ToInline(libflist.FChat.Channel _chan)
        {
            var span = new Span
            {
                Foreground = _GetBrush(Color),
                FontWeight = FontWeight,
                FontStyle = FontStyle,
                BaselineAlignment = BaselineAlignment,
                TextDecorations = TextDecorations
            };

            foreach (var node in Content.Select(c => c.ToInline(_chan)))
                span.Inlines.Add(node);

            return span;
        }

        Brush _GetBrush(string name)
        {
            switch (name.ToLower())
            {
                case "red": return Brushes.Red;
                case "blue": return Brushes.Blue;
                case "white": return Brushes.White;
                case "yellow": return Brushes.Yellow;
                case "pink": return Brushes.Pink;
                case "gray": return Brushes.Gray;
                case "green": return Brushes.Green;
                case "orange": return Brushes.Orange;
                case "purple": return Brushes.Purple;
                case "black": return Brushes.Black;
                case "brown": return Brushes.Brown;
                case "cyan": return Brushes.Cyan;
            }
            return Brushes.White;
        }
    }
}
