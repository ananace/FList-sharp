using System;

namespace libflist.Message.Nodes
{
	[Node("icon")]
    public class IconNode : ITextNode
    {
        public string Text { get; set; }

		public Uri AvatarUri
		{
			get
			{
				return new Uri($"https://static.f-list.net/images/avatar/{Uri.EscapeUriString(Text)}.png");
			}
		}
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
