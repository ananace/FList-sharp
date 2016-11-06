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

		Task<IReadOnlyCollection<string>> GetAllCharacters();
		Task<IReadOnlyCollection<Info.KinkInfo>> GetAllKinks();

		Task<bool> AddBookmark(string name);
		Task<IReadOnlyCollection<string>> GetBookmarks();
		Task RemoveBookmark(string name);

		Task<bool> AddFriend(string source, string target);
		Task<IReadOnlyCollection<string>> GetFriends(string source);
		Task RemoveFriend(string source, string target);

		Task<IReadOnlyCollection<Info.KinkInfo>> GetCustomKinks(string name);
		Task<string> GetDescription(string name);
		Task<IReadOnlyCollection<Info.ImageInfo>> GetImages(string name);
		Task<Info.ProfileInfo> GetInfo(string name);
		Task<IReadOnlyDictionary<Info.KinkInfo, Info.KinkChoice>> GetKinks(string name);
	}

}
