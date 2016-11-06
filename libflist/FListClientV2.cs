using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Net.Http;

namespace libflist
{
	public sealed class FListClientV2 : IFListClient
	{
		public ICollection<Character> Characters
		{
			get
			{
				throw new NotImplementedException();
			}
		}

		public ICollection<Info.KinkInfo> GlobalKinks
		{
			get
			{
				throw new NotImplementedException();
			}
		}

		public bool HasTicket
		{
			get
			{
				throw new NotImplementedException();
			}
		}

		public AuthTicket Ticket
		{
			get
			{
				throw new NotImplementedException();
			}

			set
			{
				throw new NotImplementedException();
			}
		}

		public Task<bool> AddBookmark(string name)
		{
			throw new NotImplementedException();
		}

		public Task<bool> AddFriend(string source, string target)
		{
			throw new NotImplementedException();
		}

		public Task<bool> Authenticate(string Username, string PasswordOrApiKey, AuthMethod Method = AuthMethod.Password)
		{
			throw new NotImplementedException();
		}

		public Task<IReadOnlyCollection<string>> GetAllCharacters()
		{
			throw new NotImplementedException();
		}

		public Task<IReadOnlyCollection<Info.KinkInfo>> GetAllKinks()
		{
			throw new NotImplementedException();
		}

		public Task<IReadOnlyCollection<string>> GetBookmarks()
		{
			throw new NotImplementedException();
		}

		public Task<IReadOnlyCollection<Info.KinkInfo>> GetCustomKinks(string name)
		{
			throw new NotImplementedException();
		}

		public Task<string> GetDescription(string name)
		{
			throw new NotImplementedException();
		}

		public Task<IReadOnlyCollection<string>> GetFriends(string source)
		{
			throw new NotImplementedException();
		}

		public Task<IReadOnlyCollection<Info.ImageInfo>> GetImages(string name)
		{
			throw new NotImplementedException();
		}

		public Task<Info.ProfileInfo> GetInfo(string name)
		{
			throw new NotImplementedException();
		}

		public Task<IReadOnlyDictionary<Info.KinkInfo, Info.KinkChoice>> GetKinks(string name)
		{
			throw new NotImplementedException();
		}

		public Character GetOrCreateCharacter(string name)
		{
			throw new NotImplementedException();
		}

		public Character GetOrCreateCharacter(string name, int id)
		{
			throw new NotImplementedException();
		}

		public Task RemoveBookmark(string name)
		{
			throw new NotImplementedException();
		}

		public Task RemoveFriend(string source, string target)
		{
			throw new NotImplementedException();
		}
	}
}
