using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Net.Http;
using libflist.JSON.Responses;

namespace libflist.JSON
{
	public sealed class FListClientV1 : IFListClient
	{
		public bool HasTicket { get { return Ticket != null && Ticket.Successful; } }
		public TicketResponse Ticket { get; set; }

		public async Task<bool> Authenticate(string username, string password, bool isApiKey = false)
		{
			if (isApiKey)
				throw new Exception("V1 doesn't support api keys.");

			var msg = await RunQuery(Path.Ticket, new Dictionary<string,string>{
				{ "account", username},
				{ "password", password }
			});

			if (!msg.IsSuccessStatusCode)
				throw new Exception("HTTP request failed.");

			var content = await msg.Content.ReadAsStringAsync();

			var response = Response.Create<Responses.TicketResponse>(content);
			if (response.Successful)
			{
				Ticket = response;
				Ticket.Account = username;

				return true;
			}
			else
				return false;
		}

		public async Task AddBookmark(Character character)
		{
			if (!HasTicket)
				throw new Exception();

			var msg = await RunQuery(Path.BookmarkAdd, new Dictionary<string,string> {
				{ "account", Ticket.Account },
				{ "ticket", Ticket.Ticket },
				{ "name", character.Name }
			});

			if (!msg.IsSuccessStatusCode)
				throw new Exception("HTTP request failed.");

			var resp = Response.Create<Response>(await msg.Content.ReadAsStringAsync());
			if (!resp.Successful)
				throw new Exception("failed");
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

		static void _Verify(Path p, IReadOnlyDictionary<string, string> Arguments)
		{
			switch (p)
			{
			case Path.Ticket:
				if (Arguments == null || !Arguments.ContainsKey("account") || !Arguments.ContainsKey("password"))
					throw new ArgumentException("Ticket requires 'account' and 'password'.");
				break;

			case Path.BookmarkList:
			case Path.CharacterList:
				if (Arguments == null || !Arguments.ContainsKey("account") || !Arguments.ContainsKey("ticket"))
					throw new ArgumentException($"{p} requires 'account' and 'ticket'");
				break;

			case Path.BookmarkAdd:
			case Path.BookmarkRemove:
			case Path.CharacterCustomkinks:
			case Path.CharacterGet:
			case Path.CharacterImages:
			case Path.CharacterInfo:
			case Path.CharacterKinks:
				if (Arguments == null
					|| !Arguments.ContainsKey("account")
					|| !Arguments.ContainsKey("ticket")
					|| !Arguments.ContainsKey("name"))
					throw new ArgumentException($"{p} requires 'account', 'ticket', and 'name'");
				break;
			}
		}

		Uri BuildURI(Path p)
		{
			var build = new UriBuilder("https://www.f-list.net/json/api/");

			switch (p)
			{
			case Path.Ticket:
				build.Path = "/json/getApiTicket.php";
				break;

			case Path.BookmarkAdd: build.Path += "bookmark-add.php"; break;
			case Path.BookmarkList: build.Path += "bookmark-list.php"; break;
			case Path.BookmarkRemove: build.Path += "bookmark-remove.php"; break;
				
			default:
				throw new NotImplementedException();
			}

			return build.Uri;
		}

		Task<HttpResponseMessage> RunQuery(Path p, IReadOnlyDictionary<string, string> Arguments)
		{
			_Verify(p, Arguments);

			using (var http = new HttpClient())
				return Arguments == null
					? http.PostAsync(BuildURI(p), null)
					: http.PostAsync(BuildURI(p), new FormUrlEncodedContent(Arguments));
		}
	}
}

