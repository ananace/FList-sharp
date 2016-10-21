using System;

namespace libflist.Message
{
	[Flags]
	public enum NodeValidity
	{
		Invalid = 0,

		FChat = 1<<0,
		FList = 1<<1,
		Internal = 1<<2,

		Both = FChat | FList,
		All = Both | Internal
	}
}
