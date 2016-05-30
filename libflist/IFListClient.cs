using System.Collections.Generic;
using System.Threading.Tasks;

namespace libflist
{

	public enum AuthMethod
	{
		Password,
		APIKey
	}

	public interface IFListClient
	{
		bool HasTicket { get; }
		AuthTicket Ticket { get; set; }
		IList<Character> Characters { get; }

		Character GetOrCreateCharacter(string name);
		Character GetOrCreateCharacter(string name, int id);

		Task<bool> Authenticate(string Username, string PasswordOrApiKey, AuthMethod Method = AuthMethod.Password);

		Task<List<string>> GetCharacters();

		Task<bool> AddBookmark(string name);
		Task<List<string>> GetBookmarks();
		Task RemoveBookmark(string name);
		Task<bool> AddFriend(string source, string target);
		Task<List<string>> GetFriends(string source);
		Task RemoveFriend(string source, string target);

		Task<Character> GetCustomKinks(string name);
		Task<string> GetDescription(string name);
		Task<Character> GetImages(string name);
		Task<Character> GetInfo(string name);
		Task<Character> GetKinks(string name);
	}

}
