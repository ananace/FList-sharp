namespace libflist.Message.Nodes
{
	[Node("user")]
    public class UserNode : ITextNode
    {
        public string Text { get; set; }

		public Character GetCharacter(IFListClient client)
		{
			return client.GetOrCreateCharacter(Text);
		}
		public FChat.Character GetCharacter(FChat.FChatConnection connection)
		{
			return connection.GetOrCreateCharacter(Text);
		}
    }
}
