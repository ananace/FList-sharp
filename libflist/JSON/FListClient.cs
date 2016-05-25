using System.Collections.Generic;
using System.Threading.Tasks;

namespace libflist.JSON
{
	enum Path
	{
		Ticket,

		BookmarkAdd,
		BookmarkList,
		BookmarkRemove,

		CharacterGetCustomkinks,
		CharacterGetDescription,
		CharacterGetImages,
		CharacterGetInfo,
		CharacterGetInlines,
		CharacterGetKinks,
		CharacterList,

		FriendList,
		FriendRemove,

		RequestAccept,
		RequestCancel,
		RequestDeny,
		RequestList,
		RequestPendingList,
		RequestSend,

		GroupList,
		IgnoreList,
		InfoList,
		KinkList
	}

	public interface IFListClient
	{
		bool HasTicket { get; }
		AuthTicket Ticket { get; set; }

		Task<bool> Authenticate(string username, string password, bool isApiKey = false);

		Task<List<Character>> GetCharacters();

		Task AddBookmark(Character character);
		Task<List<Character>> GetBookmarks();
		Task RemoveBookmark(Character character);
		Task AddFriend(Character source, Character target);
		Task<List<Character>> GetFriends(Character source);
		Task RemoveFriend(Character source, Character target);
	}

	interface IInternalFListClient : IFListClient
	{
		Task GetCustomKinks(Character character);
		Task GetDescription(Character character);
		Task GetImages(Character character);
		Task GetInfo(Character character);
		Task GetInlines(Character character);
		Task GetKinks(Character character);
	}
}
