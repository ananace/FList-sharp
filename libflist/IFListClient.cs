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

		ICollection<Character> Characters { get; }
		ICollection<Info.KinkInfo> GlobalKinks { get; }

		Character GetOrCreateCharacter(string name);
		Character GetOrCreateCharacter(string name, int id);

		Task<bool> Authenticate(string Username, string PasswordOrApiKey, AuthMethod Method = AuthMethod.Password);

		Task<List<string>> GetAllCharacters();
		Task<List<Info.KinkInfo>> GetAllKinks();

		Task<bool> AddBookmark(string name);
		Task<List<string>> GetBookmarks();
		Task RemoveBookmark(string name);

		Task<bool> AddFriend(string source, string target);
		Task<List<string>> GetFriends(string source);
		Task RemoveFriend(string source, string target);

		Task<List<Info.KinkInfo>> GetCustomKinks(string name);
		Task<string> GetDescription(string name);
		Task<List<Info.ImageInfo>> GetImages(string name);
		Task<Info.ProfileInfo> GetInfo(string name);
		Task<Dictionary<Info.KinkInfo, Info.KinkChoice>> GetKinks(string name);
	}

}
