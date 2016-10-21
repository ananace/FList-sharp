using libflist.FChat;
using libflist.Message;
using libflist.Message.Nodes;
using System;
using System.Linq;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;
using System.Windows.Media.Effects;
using System.Windows.Media.Imaging;

namespace XAMLMessenger.Message
{
	static class NodeExtensions
	{
		public static Block ToBlock(this IParagraphNode node, Channel _chan = null)
		{
			var type = node.GetType();

			var member = type.GetMethod("ToBlock");
			if (member == null)
				member = type.GetExtensionMethod("ToBlock");

			if (member == null)
				return null;

			return member.Invoke(null, new object[] { node, _chan }) as Block;
		}

		public static Inline ToInline(this INode node, Channel _chan = null)
		{
			var type = node.GetType();

			var member = type.GetMethod("ToInline");
			if (member == null)
				member = type.GetExtensionMethod("ToInline");

			if (member == null)
				return null;

			return member.Invoke(null, new object[] { node, _chan }) as Inline;
		}

		public static Inline ToInline(this ITextNode node, Channel _chan = null)
		{
			return new Run(node.Text);
		}

		public static Inline ToInline(this IContentNode node, Channel _chan = null, Span _span = null)
		{
			Span inline = _span;
			if (inline == null)
			{
				if (node.Content.Count == 1)
					return node.Content.First().ToInline(_chan);

				inline = new Span();
			}

			foreach (var child in node.Content)
				inline.Inlines.Add(child.ToInline(_chan));
			return inline;
		}


		public static Block ToBlock(this CenterNode node, Channel _chan = null)
		{
			return new Paragraph(node.ToInline())
			{
				TextAlignment = TextAlignment.Center
			};
		}
		public static Block ToBlock(this CollapseNode node, Channel _chan = null)
		{
			return new Paragraph(node.ToInline());
		}
		public static Block ToBlock(this HeadingNode node, Channel _chan = null)
		{
			return new Paragraph(node.ToInline())
			{
				FontSize = 14,
				KeepTogether = true,
				KeepWithNext = false,
				Margin = new Thickness(0, 10, 0, 10)
			};
		}
		public static Block ToBlock(this HorizontalRuleNode node, Channel _chan = null)
		{
			return new Paragraph
			{
				BorderBrush = Brushes.Black
			};
		}
		public static Block ToBlock(this IndentNode node, Channel _chan = null)
		{
			return new Paragraph(node.ToInline())
			{
				Margin = new Thickness(36, 0, 0, 0)
			};
		}
		public static Block ToBlock(this JustifyNode node, Channel _chan = null)
		{
			return new Paragraph(node.ToInline())
			{
				TextAlignment = TextAlignment.Justify
			};
		}
		public static Block ToBlock(this LeftNode node, Channel _chan = null)
		{
			return new Paragraph(node.ToInline())
			{
				TextAlignment = TextAlignment.Left
			};
		}
		public static Block ToBlock(this QuoteNode node, Channel _chan = null)
		{
			return new Paragraph(node.ToInline())
			{
				BorderBrush = Brushes.Black,
				BorderThickness = new Thickness(1),

				Margin = new Thickness(10)
			};
		}
		public static Block ToBlock(this RightNode node, Channel _chan = null)
		{
			return new Paragraph(node.ToInline())
			{
				TextAlignment = TextAlignment.Right
			};
		}

		public static Inline ToInline(this BigNode node, Channel _chan = null)
		{
			var span = (node as IContentNode).ToInline();
			span.FontSize = 16;

			return span;
		}
		public static Inline ToInline(this BoldNode node, Channel chan = null)
		{
			var span = (node as IContentNode).ToInline(chan);
			span.FontWeight = FontWeights.Bold;

			return span;
		}
		public static Inline ToInline(this ColorNode node, Channel chan = null)
		{
			var span = (node as IContentNode).ToInline(chan);
			var prop = typeof(Brushes)
				.GetProperties(BindingFlags.Static | BindingFlags.Public)
				.FirstOrDefault(p => p.Name.Equals(node.Attribute, StringComparison.CurrentCultureIgnoreCase));
			if (prop != null)
			{ 
				span.Foreground = prop.GetValue(null) as Brush;
				// TODO: Text shadow?
			}
			return span;
		}
		public static Inline ToInline(this EIconNode node, Channel chan = null)
		{
			return new InlineUIContainer(new Image
			{
				Source = new BitmapImage(node.IconUri),

				Width = 100,
				Height = 100,
			})
			{
				ToolTip = node.Text
			};
		}
		public static Inline ToInline(this IconNode node, Channel chan = null)
		{
			return new Hyperlink
			{
				Inlines =
				{
					new InlineUIContainer(new Image
					{
						Source = new BitmapImage(node.AvatarUri),

						Width = 32,
						Height = 32,
					})
				},

				NavigateUri = new Uri($"flist://character/{node.Text}"),
				ToolTip = node.Text
			};
		}
		public static Inline ToInline(this ItalicNode node, Channel chan = null)
		{
			var span = (node as IContentNode).ToInline(chan);
			span.FontStyle = FontStyles.Italic;

			return span;
		}
		public static Inline ToInline(this NoparseNode node, Channel chan = null)
		{
			return new Run(node.ToString(NodeStringType.BBCode));
		}
		public static Inline ToInline(this SessionNode node, Channel chan = null)
		{
			return new Hyperlink
			{
				Inlines =
				{
					new InlineUIContainer(new Image
					{
						Source = new CroppedBitmap(App.Current.CombinedImageResource, node.IsOfficial ? new Int32Rect(96, 0, 24, 24) : new Int32Rect(0, 24, 24, 24)),
						Width = 16,
						Height = 16,

						Margin = new Thickness(4)
					}),
					new Run(node.SessionName)
					{
						FontWeight = FontWeights.Bold
					}
				},

				Foreground = Brushes.White,
				TextDecorations = null,

				NavigateUri = new Uri($"flist://session/{node.SessionADH}"),
				ToolTip = node.SessionName
			};
		}
		public static Inline ToInline(this SmallNode node, Channel _chan = null)
		{
			var span = (node as IContentNode).ToInline();
			span.FontSize = 6;

			return span;
		}
		public static Inline ToInline(this StrikethroughNode node, Channel chan = null)
		{
			var span = (node as IContentNode).ToInline(chan);
			span.TextDecorations = TextDecorations.Strikethrough;

			return span;
		}
		public static Inline ToInline(this SubscriptNode node, Channel chan = null)
		{
			var span = (node as IContentNode).ToInline(chan);
			span.BaselineAlignment = BaselineAlignment.Subscript;
			span.FontSize = 9;
			return span;
		}
		public static Inline ToInline(this SuperscriptNode node, Channel chan = null)
		{
			var span = (node as IContentNode).ToInline(chan);
			span.BaselineAlignment = BaselineAlignment.Superscript;
			span.FontSize = 9;
			return span;
		}
		public static Inline ToInline(this TextNode node, Channel chan = null)
		{
			return new Run(node.Text);
		}
		public static Inline ToInline(this UnderlineNode node, Channel chan = null)
		{
			var span = (node as IContentNode).ToInline(chan);
			span.TextDecorations = TextDecorations.Underline;
			return span;
		}
		public static Inline ToInline(this URLNode node, Channel chan = null)
		{
			return new Hyperlink
			{
				Inlines =
				{
					new InlineUIContainer(new Image
					{
						Source = new CroppedBitmap(App.Current.CombinedImageResource, new Int32Rect(72, 96, 24, 24)),
						Width = 16,
						Height = 16,

						Margin = new Thickness(4)
					}),
					(node as IContentNode).ToInline(chan, new Span { TextDecorations = TextDecorations.Underline })
				},

				TextDecorations = null,
				Foreground = Brushes.White,

				NavigateUri = node.Uri,
				ToolTip = node.Uri.ToString()
			};
		}
		public static Inline ToInline(this UserNode node, Channel chan = null)
		{
			var character = node.GetCharacter(App.Current.FChatClient);

			return new Hyperlink
			{
				Inlines =
				{
					(chan == null || character.CharacterSymbolRect(chan) == null ? null : new Image {
						Source = new CroppedBitmap(App.Current.CombinedImageResource, character.CharacterSymbolRect(chan).Value),
						Width = 16,
						Height = 16,

						Margin = new Thickness(2)
					}),
					new InlineUIContainer(new Image
					{
						Source = new CroppedBitmap(App.Current.CombinedImageResource, character.CharacterStatusRect()),
						Width = 16,
						Height = 16,

						Margin = new Thickness(4)
					}),
				},

				TextDecorations = null,
				Foreground = character.CharacterGenderBrush()
			};
		}
	}
}
