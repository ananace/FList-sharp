using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Net.Http;

namespace libflist.JSON
{
	public sealed class FListClientV2 : IFListClient
	{
		public Task<bool> Authenticate(string username, string password, bool isApiKey = false)
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
		public bool HasTicket {
			get {
				throw new NotImplementedException();
			}
		}
		public libflist.JSON.Responses.TicketResponse Ticket {
			get {
				throw new NotImplementedException();
			}
			set {
				throw new NotImplementedException();
			}
		}
	}
}

