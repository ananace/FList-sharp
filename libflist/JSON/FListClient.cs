using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace libflist.JSON
{
	internal enum Path
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

	public interface IFListClient
	{
		bool HasTicket { get; }
		Responses.TicketResponse Ticket { get; set; }

		Task<bool> Authenticate(string username, string password, bool isApiKey = false);

		Task AddBookmark(Character character);
		Task<List<Character>> GetBookmarks();
		Task RemoveBookmark(Character character);
		Task AddFriend(Character source, Character target);
		Task<List<Character>> GetFriends(Character source);
		Task RemoveFriend(Character source, Character target);
	}
}

