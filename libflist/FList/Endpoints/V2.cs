using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Net.Http;

namespace libflist.FList
{
	public sealed class FListClientV2 : IFListClient, IInternalFListClient
	{
		public bool HasTicket {
			get {
				throw new NotImplementedException();
			}
		}
		public AuthTicket Ticket {
			get {
				throw new NotImplementedException();
			}
			set {
				throw new NotImplementedException();
			}
		}

		public Character GetOrCreateCharacter(string name)
		{
			throw new NotImplementedException();
		}

		public Character GetOrCreateCharacter(string name, int id)
		{
			throw new NotImplementedException();
		}

		public Task<bool> Authenticate(string username, string password, bool isApiKey = false)
		{
			throw new NotImplementedException();
		}

		public Task<List<Character>> GetCharacters()
		{
			throw new NotImplementedException();
		}

		public Task AddBookmark(Character character)
		{
			throw new NotImplementedException();
		}
		public Task<List<Character>> GetBookmarks()
		{
			throw new NotImplementedException();
		}
		public Task RemoveBookmark(Character character)
		{
			throw new NotImplementedException();
		}
		public Task AddFriend(Character source, Character target)
		{
			throw new NotImplementedException();
		}
		public Task<List<Character>> GetFriends(Character source)
		{
			throw new NotImplementedException();
		}
		public Task RemoveFriend(Character source, Character target)
		{
			throw new NotImplementedException();
		}

		public Task GetCustomKinks(Character character)
		{
			throw new NotImplementedException();
		}

		public Task GetDescription(Character character)
		{
			throw new NotImplementedException();
		}

		public Task GetImages(Character character)
		{
			throw new NotImplementedException();
		}

		public Task GetInfo(Character character)
		{
			throw new NotImplementedException();
		}

		public Task GetInlines(Character character)
		{
			throw new NotImplementedException();
		}

		public Task GetKinks(Character character)
		{
			throw new NotImplementedException();
		}
	}
}
