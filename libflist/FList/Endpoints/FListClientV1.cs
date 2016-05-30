﻿using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Net.Http;
using Newtonsoft.Json;
using System.Linq;

namespace libflist.FList
{
	public sealed class FListClientV1 : IFListClient, IInternalFListClient
	{
		List<Character> _Characters = new List<Character>();

		public bool HasTicket { get { return Ticket != null; } }
		public AuthTicket Ticket { get; set; }

		public Character GetOrCreateCharacter(string name)
		{
			lock (_Characters)
			{
				var character = _Characters.FirstOrDefault(c => c.Name.Equals(name, StringComparison.CurrentCultureIgnoreCase));
				if (character == null)
				{
					character = new Character(this, name);
					_Characters.Add(character);
				}
				return character;
			}
		}
		public Character GetOrCreateCharacter(string name, int id)
		{
			lock (_Characters)
			{
				var character = _Characters.FirstOrDefault(c => c.Name.Equals(name, StringComparison.CurrentCultureIgnoreCase));
				if (character == null)
				{
					character = new Character(this, name, id);
					_Characters.Add(character);
				}
				return character;
			}
		}

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

		public async Task<List<Character>> GetCharacters()
		{
			if (!HasTicket)
				throw new Exception();

			CharacterListResponse resp;
			using (var http = new HttpClient())
			{
				var msg = await RunQuery(http, Path.CharacterList, new Dictionary<string,string> {
					{ "account", Ticket.Account },
					{ "ticket", Ticket.Ticket }
				});

				if (!msg.IsSuccessStatusCode)
					throw new Exception("HTTP request failed.");

				resp = Response.Create<CharacterListResponse>(await msg.Content.ReadAsStringAsync());
				if (!resp.Successful)
					throw new Exception("failed");
			}

			var list = new List<Character>();
			foreach (var character in resp.Characters)
				list.Add(GetOrCreateCharacter(character));
			return list;
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

			CharacterListResponse resp;
			using (var http = new HttpClient())
			{
				var msg = await RunQuery(http, Path.BookmarkList, new Dictionary<string,string> {
					{ "account", Ticket.Account },
					{ "ticket", Ticket.Ticket }
				});

				if (!msg.IsSuccessStatusCode)
					throw new Exception("HTTP request failed.");

				resp = Response.Create<CharacterListResponse>(await msg.Content.ReadAsStringAsync());
				if (!resp.Successful)
					throw new Exception("failed");
			}

			var list = new List<Character>();
			foreach (var character in resp.Characters)
				list.Add(GetOrCreateCharacter(character));
			return list;
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

			var list = new List<Character>();
			foreach (var character in resp.Friends.Where(f => f.Character.Equals(source.Name, StringComparison.CurrentCultureIgnoreCase)))
				list.Add(GetOrCreateCharacter(character.Friend));
			return list;
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

		async Task<CharacterResponse> UpdateCharacter(Path p, Character character)
		{
			if (!HasTicket)
				throw new Exception();
			
			CharacterResponse resp;
			using (var http = new HttpClient())
			{
				var msg = await RunQuery(http, p, new Dictionary<string,string> {
					{ "account", Ticket.Account },
					{ "ticket", Ticket.Ticket },
					{ "name", character.Name }
				});

				if (!msg.IsSuccessStatusCode)
					throw new Exception("HTTP request failed.");

				resp = Response.Create<CharacterResponse>(await msg.Content.ReadAsStringAsync());
				if (!resp.Successful)
					throw new Exception("failed");
			}

			return resp;
		}

		public async Task GetCustomKinks(Character character)
		{
			var data = await UpdateCharacter(Path.CharacterGetCustomkinks, character);
			// TODO
		}

		public async Task GetDescription(Character character)
		{
			var data = await UpdateCharacter(Path.CharacterGetDescription, character);
			character._Description = data.Character.Description;
		}

		public async Task GetImages(Character character)
		{
			var data = await UpdateCharacter(Path.CharacterGetImages, character);
			// TODO
		}

		public async Task GetInfo(Character character)
		{
			var data = await UpdateCharacter(Path.CharacterGetInfo, character);
			// TODO
		}

		public async Task GetKinks(Character character)
		{
			var data = await UpdateCharacter(Path.CharacterGetKinks, character);
			// TODO
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

			case Path.CharacterList: build.Path += "character-list.php"; break;

			case Path.BookmarkAdd: build.Path += "bookmark-add.php"; break;
			case Path.BookmarkList: build.Path += "bookmark-list.php"; break;
			case Path.BookmarkRemove: build.Path += "bookmark-remove.php"; break;

			case Path.CharacterGetCustomkinks: build.Path += "character-customkinks.php"; break;
			case Path.CharacterGetDescription: build.Path += "character-get.php"; break;
			case Path.CharacterGetImages: build.Path += "character-images.php"; break;
			case Path.CharacterGetInfo: build.Path += "character-info.php"; break;
			case Path.CharacterGetKinks: build.Path += "character-kinks.php"; break;
				
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

		sealed class CharacterListResponse : Response
		{
			[JsonProperty("characters")]
			public string[] Characters { get; set; }
		}
		sealed class FriendListResponse : Response
		{
			public class FriendMapping
			{
				[JsonProperty("source")]
				public string Character { get; set; }
				[JsonProperty("dest")]
				public string Friend { get; set; }
				[JsonProperty("last_online")]
				public int LastOnline { get; set; }
			}

			[JsonProperty("friends")]
			public FriendMapping[] Friends { get; set; }
		}
		sealed class CharacterData
		{
			[JsonProperty("name")]
			public string Name { get; set; }

			[JsonProperty("description")]
			public string Description { get; set; }
			[JsonProperty("datetime_created")]
			public string CreatedAt { get; set; }
			[JsonProperty("datetime_changed")]
			public string UpdatedAt { get; set; }
			[JsonProperty("pageviews")]
			public string Views { get; set; }
		}

		sealed class CharacterResponse : Response
		{
			[JsonProperty("character")]
			public CharacterData Character { get; set; }
		}
	}
}
