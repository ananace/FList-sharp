namespace libflist.Message.Nodes
{
	[Node("channel", Valid = NodeValidity.FChat)]
	[Node("session", Valid = NodeValidity.FChat)]
	public class SessionNode : ITextNode, IAttributeNode
	{
		public string Text { get; set; }
		public string Attribute { get; set; }

		public string SessionADH { get { return Text ?? Attribute; } }
		public string SessionName { get { return Attribute ?? Text; } }

		public bool IsOfficial { get { return SessionName == SessionADH || string.IsNullOrEmpty(SessionADH); } }
	}
}
