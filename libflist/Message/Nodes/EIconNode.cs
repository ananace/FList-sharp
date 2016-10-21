using System;

namespace libflist.Message.Nodes
{
	[Node("eicon", Valid = NodeValidity.FChat)]
    public class EIconNode : ITextNode
    {
        public string Text { get; set; }

		public Uri IconUri
		{
			get
			{
				return new Uri($"https://static.f-list.net/images/icons/{Uri.EscapeUriString(Text)}.gif");
			}
		}
    }
}
