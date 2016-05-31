using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Net.Http;

namespace libflist
{
	public sealed class FListClientV2 : IFListClient
	{
		public IList<Character> Characters
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

		public Task<List<string>> GetBookmarks()
		{
			throw new NotImplementedException();
		}

		public Task<List<string>> GetAllCharacters()
		{
			throw new NotImplementedException();
		}

		public Task<Character> GetCustomKinks(string name)
		{
			throw new NotImplementedException();
		}

		public Task<string> GetDescription(string name)
		{
			throw new NotImplementedException();
		}

		public Task<List<string>> GetFriends(string source)
		{
			throw new NotImplementedException();
		}

		public Task<Character> GetImages(string name)
		{
			throw new NotImplementedException();
		}

		public Task<Character> GetInfo(string name)
		{
			throw new NotImplementedException();
		}

		public Task<Character> GetKinks(string name)
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
