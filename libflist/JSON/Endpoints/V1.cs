using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Net.Http;
using Newtonsoft.Json;

namespace libflist.JSON
{
	public sealed class FListClientV1 : IFListClient, IInternalFListClient
	{
		public bool HasTicket { get { return Ticket != null; } }
		public AuthTicket Ticket { get; set; }

		public async Task<bool> Authenticate(string username, string password, bool isApiKey = false)
		{
			if (isApiKey)
				throw new Exception("V1 doesn't support api keys.");

			AuthTicket.TicketResponse response = null;
			using (var http = new HttpClient())
			{
				var msg = await RunQuery(http, Path.Ticket, new Dictionary<string,string> {
					{ "account", username },
					{ "password", password }
				});

				if (!msg.IsSuccessStatusCode)
					throw new Exception("HTTP request failed.");

				var content = await msg.Content.ReadAsStringAsync();
				response = Response.Create<AuthTicket.TicketResponse>(content);
			}

			if (response.Successful)
			{
				Ticket = new AuthTicket(response);
				Ticket.Account = username;

				return true;
			}
			else
				return false;
		}

		public Task<List<Character>> GetCharacters()
		{
			throw new NotImplementedException();
		}

		public async Task AddBookmark(Character character)
		{
			if (!HasTicket)
				throw new Exception();

			using (var http = new HttpClient())
			{
				var msg = await RunQuery(http, Path.BookmarkAdd, new Dictionary<string,string> {
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
		}

		public async Task<List<Character>> GetBookmarks()
		{
			if (!HasTicket)
				throw new Exception();

			BookmarkListResponse resp = null;
			using (var http = new HttpClient())
			{
				var msg = await RunQuery(http, Path.BookmarkList, new Dictionary<string,string> {
					{ "account", Ticket.Account },
					{ "ticket", Ticket.Ticket }
				});

				if (!msg.IsSuccessStatusCode)
					throw new Exception("HTTP request failed.");

				resp = Response.Create<BookmarkListResponse>(await msg.Content.ReadAsStringAsync());
				if (!resp.Successful)
					throw new Exception("failed");
			}

			return new List<Character>();
		}

		public async Task RemoveBookmark(Character character)
		{
			if (!HasTicket)
				throw new Exception();

			using (var http = new HttpClient())
			{
				var msg = await RunQuery(http, Path.BookmarkRemove, new Dictionary<string,string> {
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
		}

		public async Task AddFriend(Character source, Character target)
		{
			if (!HasTicket)
				throw new Exception();

			using (var http = new HttpClient())
			{
				var msg = await RunQuery(http, Path.RequestSend, new Dictionary<string,string> {
					{ "account", Ticket.Account },
					{ "ticket", Ticket.Ticket },
					{ "source_name", source.Name },
					{ "dest_name", target.Name }
				});

				if (!msg.IsSuccessStatusCode)
					throw new Exception("HTTP request failed.");

				var resp = Response.Create<Response>(await msg.Content.ReadAsStringAsync());
				if (!resp.Successful)
					throw new Exception("failed");
			}
		}

		public async Task<List<Character>> GetFriends(Character source)
		{
			if (!HasTicket)
				throw new Exception();

			FriendListResponse resp = null;
			using (var http = new HttpClient())
			{
				var msg = await RunQuery(http, Path.BookmarkList, new Dictionary<string,string> {
					{ "account", Ticket.Account },
					{ "ticket", Ticket.Ticket }
				});

				if (!msg.IsSuccessStatusCode)
					throw new Exception("HTTP request failed.");

				resp = Response.Create<FriendListResponse>(await msg.Content.ReadAsStringAsync());
				if (!resp.Successful)
					throw new Exception("failed");
			}

			return new List<Character>();
		}

		public async Task RemoveFriend(Character source, Character target)
		{
			if (!HasTicket)
				throw new Exception();

			using (var http = new HttpClient())
			{
				var msg = await RunQuery(http, Path.RequestSend, new Dictionary<string,string> {
					{ "account", Ticket.Account },
					{ "ticket", Ticket.Ticket },
					{ "source_name", source.Name },
					{ "dest_name", target.Name }
				});

				if (!msg.IsSuccessStatusCode)
					throw new Exception("HTTP request failed.");

				var resp = Response.Create<Response>(await msg.Content.ReadAsStringAsync());
				if (!resp.Successful)
					throw new Exception("failed");
			}
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
			case Path.RequestList:
			case Path.RequestPendingList:
				if (Arguments == null || !Arguments.ContainsKey("account") || !Arguments.ContainsKey("ticket"))
					throw new ArgumentException($"{p} requires 'account' and 'ticket'");
				break;

			case Path.BookmarkAdd:
			case Path.BookmarkRemove:
			case Path.CharacterGetCustomkinks:
			case Path.CharacterGetDescription:
			case Path.CharacterGetImages:
			case Path.CharacterGetInfo:
			case Path.CharacterGetKinks:
				if (Arguments == null
				    || !Arguments.ContainsKey("account")
				    || !Arguments.ContainsKey("ticket")
				    || !Arguments.ContainsKey("name"))
					throw new ArgumentException($"{p} requires 'account', 'ticket', and 'name'");
				break;

			case Path.RequestSend:
			case Path.FriendRemove:
				if (Arguments == null
					|| !Arguments.ContainsKey("account")
					|| !Arguments.ContainsKey("ticket")
					|| !Arguments.ContainsKey("source_name")
					|| !Arguments.ContainsKey("dest_name"))
					throw new ArgumentException($"{p} requires 'account', 'ticket', 'source_name', and 'dest_name'");
				break;

			case Path.RequestAccept:
			case Path.RequestCancel:
			case Path.RequestDeny:
				if (Arguments == null
					|| !Arguments.ContainsKey("account")
					|| !Arguments.ContainsKey("ticket")
					|| !Arguments.ContainsKey("request_id"))
					throw new ArgumentException($"{p} requires 'account', 'ticket', and 'request_id'");
				break;
			}
		}

		static Uri BuildURI(Path p)
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

		static Task<HttpResponseMessage> RunQuery(HttpClient http, Path p, IReadOnlyDictionary<string, string> Arguments)
		{
			_Verify(p, Arguments);

			return Arguments == null
				? http.PostAsync(BuildURI(p), null)
				: http.PostAsync(BuildURI(p), new FormUrlEncodedContent(Arguments));
		}

		sealed class BookmarkListResponse : Response
		{
			// TODO
		}
		sealed class FriendListResponse : Response
		{
			// TODO
		}
	}
}
