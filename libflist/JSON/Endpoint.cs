using System;
using System.Collections.Generic;

namespace libflist.JSON
{
	public abstract class Endpoint
	{
		public enum Path
		{
			Ticket,

			BookmarkAdd,
			BookmarkList,
			BookmarkRemove,

			CharacterCustomkinks,
			CharacterGet,
			CharacterImages,
			CharacterInfo,
			CharacterKinks,
			CharacterList,

			FriendList,
			FriendRemove,

			RequestAccept,
			RequestCancel,
			RequestDeny,
			RequestList,
			RequestPending,
			RequestSend,

			GroupList,
			IgnoreList,
			InfoList,
			KinkList
		}

		public static string BaseURI { get; protected set; }
		public static IReadOnlyDictionary<Path, string> Paths { get; protected set; }

		public Uri BuildURI(Path p)
		{
			if (!Paths.ContainsKey(p))
				throw new NotImplementedException();

			return new Uri(BaseURI + Paths[p]);
		}
	}
}

