namespace libflist.Message.Nodes
{
	[Node("text")]
    public class TextNode : IPlainTextNode
    {
        public string Text { get; set; }
    }
}
