using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Media;

namespace XAMLMessenger.Message.Nodes
{
    class TextEffectNode : IContentNode
    {
        public string Name { get; set; }
        public ICollection<INode> Content { get; } = new List<INode>();

        public string Color { get; set; } = null;
        public FontWeight? FontWeight { get; set; } = null;
        public FontStyle? FontStyle { get; set; } = null;
        public BaselineAlignment? BaselineAlignment { get; set; } = null;
        public TextDecorationCollection TextDecorations { get; set; } = null;

        public Inline ToInline(libflist.FChat.Channel _chan)
        {
            try
            {
                var span = new Span();

                if (!string.IsNullOrEmpty(Color))
                    span.Foreground = _GetBrush(Color);
                if (BaselineAlignment.HasValue)
                {
                    span.BaselineAlignment = BaselineAlignment.Value;
                    span.FontSize -= 2;
                }
                if (FontWeight.HasValue)
                    span.FontWeight = FontWeight.Value;
                if (FontStyle.HasValue)
                    span.FontStyle = FontStyle.Value;
                if (TextDecorations != null)
                    span.TextDecorations = TextDecorations;

                foreach (var node in Content.Select(c => c.ToInline(_chan)))
                    span.Inlines.Add(node);

                return span;
            }
            catch(Exception ex)
            {
                return new Run($"{(this as IContentNode).ToNodeString()}") { ToolTip = ex.ToString() };
            }
        }

        Brush _GetBrush(string name)
        {
			// No need to use a lookup table when there's reflection
			var prop = typeof(Brushes)
				.GetProperties(BindingFlags.Static | BindingFlags.Public)
				.FirstOrDefault(p => p.Name.Equals(name, StringComparison.CurrentCultureIgnoreCase));
			if (prop == null)
				return Brushes.White;

			return prop.GetMethod.Invoke(null, null) as Brush;
        }
    }
}
