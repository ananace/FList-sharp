using libflist.Message;

namespace XAMLMessenger.Message
{
	[Node("IE", Valid = NodeValidity.Internal)]
	class InternalEndNode : INode
	{
		public InternalEndNode(Parser p)
		{
			p.Validity &= ~NodeValidity.Internal;
		}
	}
}
