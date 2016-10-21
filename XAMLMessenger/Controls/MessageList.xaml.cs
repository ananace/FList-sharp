using libflist.Message;
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
using XAMLMessenger.Message;

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
		
		void routeHyperlinks(Inline inl)
		{
			if (inl is Hyperlink)
				(inl as Hyperlink).RequestNavigate += (s, e) => App.Current.OnRequestNavigate(s, e);

			if (inl is Span)
				foreach (var sub in (inl as Span).Inlines)
					routeHyperlinks(sub);
		}

		public void AddMessageParagraph(Paragraph inp)
		{
			foreach (var inl in inp.Inlines)
				routeHyperlinks(inl);

			_document.Blocks.Add(inp);
		}

        public void AddMessage(string message)
        {
            var par = new Paragraph();

            par.KeepWithNext = true;
            par.KeepTogether = true;
            par.LineHeight = 1;

            par.BorderBrush = new LinearGradientBrush(Colors.Black, Colors.White, 90.0)
            {
                Opacity = 0.1
            };
            par.BorderThickness = new Thickness(0, 1, 0, 1);

            par.FontSize = 11;
            par.FontStyle = FontStyles.Italic;
            par.Background = new SolidColorBrush
            {
                Color = Colors.Black,
                Opacity = 0.25
            };
            par.Foreground = Brushes.LightGray;

            par.Inlines.Add(new DateNode().ToInline(null));
            par.Inlines.AddRange(new Parser { Validity = NodeValidity.FChat }.ParseMessage(message).Select(n => {
				try
				{
					return n.ToInline(null);
				}
				catch (Exception ex)
				{
					return new Run(n.ToString(NodeStringType.BBCode))
					{
						TextDecorations = TextDecorations.Baseline,
						ToolTip = ex.ToString()
					};
				}
			}));

			AddMessageParagraph(par);
        }
    }
}
