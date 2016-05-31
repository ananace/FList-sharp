using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Net.Http;
using Newtonsoft.Json;
using System.Linq;
using libflist.Util;

namespace libflist
{
	public sealed class FListClientV1 : IFListClient
	{
		enum Path
		{
			Ticket,

			BookmarkAdd,
			BookmarkList,
			BookmarkRemove,

			CharacterGetCustomkinks,
			CharacterGetDescription,
			CharacterGetImages,
			CharacterGetInfo,
			CharacterGetKinks,
			CharacterList,

			FriendList,
			FriendRemove,

			RequestAccept,
			RequestCancel,
			RequestDeny,
			RequestList,
			RequestPendingList,
			RequestSend,

			GroupList,
			IgnoreList,
			InfoList,
			KinkList
		}

		List<Character> _Characters = new List<Character>();
		List<Info.KinkInfo> _Kinks = new List<Info.KinkInfo>();

		public bool HasTicket { get { return Ticket != null; } }
		public AuthTicket Ticket { get; set; }
		public ICollection<Character> Characters { get { return _Characters; } }
		public ICollection<Info.KinkInfo> GlobalKinks { get { return _Kinks; } }

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

		public Character GetOrCreateCharacter(string name, int _)
		{
			return GetOrCreateCharacter(name);
		}

		public async Task<bool> Authenticate(string username, string password, AuthMethod method)
		{
			if (method == AuthMethod.APIKey)
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

		public async Task<List<string>> GetAllCharacters()
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

			var list = new List<string>();
			foreach (var character in resp.Characters)
				list.Add(character);
			return list;
		}

		public async Task<List<Info.KinkInfo>> GetAllKinks()
		{
			if (!HasTicket)
				throw new Exception();

			CharacterListResponse resp;
			using (var http = new HttpClient())
			{
				var msg = await RunQuery(http, Path.KinkList, new Dictionary<string, string> {
					{ "account", Ticket.Account },
					{ "ticket", Ticket.Ticket }
				});

				if (!msg.IsSuccessStatusCode)
					throw new Exception("HTTP request failed.");

				resp = Response.Create<CharacterListResponse>(await msg.Content.ReadAsStringAsync());
				if (!resp.Successful)
					throw new Exception("failed");
			}

			var list = new List<Info.KinkInfo>();


			_Kinks.Clear();
			_Kinks.AddRange(list);

			return list;
		}

		public async Task<bool> AddBookmark(string character)
		{
			if (!HasTicket)
				throw new Exception();

			using (var http = new HttpClient())
			{
				var msg = await RunQuery(http, Path.BookmarkAdd, new Dictionary<string,string> {
					{ "account", Ticket.Account },
					{ "ticket", Ticket.Ticket },
					{ "name", character }
				});

				if (!msg.IsSuccessStatusCode)
					return false;

				var resp = Response.Create<Response>(await msg.Content.ReadAsStringAsync());
				if (!resp.Successful)
					return false;
			}

			return true;
		}

		public async Task<List<string>> GetBookmarks()
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

			var list = new List<string>();
			foreach (var character in resp.Characters)
				list.Add(character);
			return list;
		}

		public async Task RemoveBookmark(string character)
		{
			if (!HasTicket)
				throw new Exception();

			using (var http = new HttpClient())
			{
				var msg = await RunQuery(http, Path.BookmarkRemove, new Dictionary<string,string> {
					{ "account", Ticket.Account },
					{ "ticket", Ticket.Ticket },
					{ "name", character }
				});

				if (!msg.IsSuccessStatusCode)
					throw new Exception("HTTP request failed.");

				var resp = Response.Create<Response>(await msg.Content.ReadAsStringAsync());
				if (!resp.Successful)
					throw new Exception("failed");
			}
		}

		public async Task<bool> AddFriend(string source, string target)
		{
			if (!HasTicket)
				throw new Exception();

			using (var http = new HttpClient())
			{
				var msg = await RunQuery(http, Path.RequestSend, new Dictionary<string,string> {
					{ "account", Ticket.Account },
					{ "ticket", Ticket.Ticket },
					{ "source_name", source },
					{ "dest_name", target }
				});

				if (!msg.IsSuccessStatusCode)
					return false;

				var resp = Response.Create<Response>(await msg.Content.ReadAsStringAsync());
				if (!resp.Successful)
					return false;
			}

			return true;
		}

		public async Task<List<string>> GetFriends(string source)
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

			var list = new List<string>();
			foreach (var character in resp.Friends.Where(f => f.Character.Equals(source, StringComparison.CurrentCultureIgnoreCase)))
				list.Add(character.Friend);
			return list;
		}

		public async Task RemoveFriend(string source, string target)
		{
			if (!HasTicket)
				throw new Exception();

			using (var http = new HttpClient())
			{
				var msg = await RunQuery(http, Path.RequestSend, new Dictionary<string,string> {
					{ "account", Ticket.Account },
					{ "ticket", Ticket.Ticket },
					{ "source_name", source },
					{ "dest_name", target }
				});

				if (!msg.IsSuccessStatusCode)
					throw new Exception("HTTP request failed.");

				var resp = Response.Create<Response>(await msg.Content.ReadAsStringAsync());
				if (!resp.Successful)
					throw new Exception("failed");
			}
		}

		async Task<T> UpdateCharacter<T>(Path p, string name) where T : Response
		{
			if (!HasTicket)
				throw new Exception();
			
			T resp;
			using (var http = new HttpClient())
			{
				var msg = await RunQuery(http, p, new Dictionary<string,string> {
					{ "account", Ticket.Account },
					{ "ticket", Ticket.Ticket },
					{ "name", name }
				});

				if (!msg.IsSuccessStatusCode)
					throw new Exception("HTTP request failed.");

				resp = Response.Create<T>(await msg.Content.ReadAsStringAsync());
				if (!resp.Successful)
					throw new Exception("failed");
			}

			return resp;
		}

		public async Task<List<Info.KinkInfo>> GetCustomKinks(string name)
		{
			var data = await UpdateCharacter<CustomKinkResponse>(Path.CharacterGetCustomkinks, name);
			
			return data.Kinks.Select(k => new Info.KinkInfo
			{
				Description = k.Description,
				Name = k.Name,
				Group = Info.KinkGroup.Custom
			}).ToList();
		}

		public async Task<string> GetDescription(string name)
		{
			var data = await UpdateCharacter<CharacterResponse>(Path.CharacterGetDescription, name);
			return data.Character.Description;
		}

		public async Task<List<Info.ImageInfo>> GetImages(string name)
		{
			var data = await UpdateCharacter<ImagesResponse>(Path.CharacterGetImages, name);
			
			return data.Images.Select(i => new Info.ImageInfo
			{
				Description = i.Description,
				Height = i.Height,
				Width = i.Width,
				Url = new Uri(i.Url)
			}).ToList();
		}

		public async Task<Info.ProfileInfo> GetInfo(string name)
		{
			var data = await UpdateCharacter<InfoResponse>(Path.CharacterGetInfo, name);

			var getValue = new Func<InfoResponse.InfoGroup, string, string>((g, n) => {
				var block = data.InfoBlocks.Values.FirstOrDefault(b => b.Group == g);
				if (block == null)
					return null;

				var item = block.Items.FirstOrDefault(i => i.Name.Equals(n, StringComparison.CurrentCultureIgnoreCase));
				if (item == null)
					return null;

				return item.Value;
			});

			var info = new Info.ProfileInfo
			{
				GeneralDetails =
				{
					Age = getValue(InfoResponse.InfoGroup.GeneralDetails, "age"),
					ApparentAge = getValue(InfoResponse.InfoGroup.GeneralDetails, "apparent age"),
					EyeColor = getValue(InfoResponse.InfoGroup.GeneralDetails, "eye color"),
					Hair = getValue(InfoResponse.InfoGroup.GeneralDetails, "hair"),
					Height = getValue(InfoResponse.InfoGroup.GeneralDetails, "height/length"),
					Location = getValue(InfoResponse.InfoGroup.GeneralDetails, "location"),
					Master = getValue(InfoResponse.InfoGroup.GeneralDetails, "master/mistress/owner"),
					Occupation = getValue(InfoResponse.InfoGroup.GeneralDetails, "occupation"),
					Partner = getValue(InfoResponse.InfoGroup.GeneralDetails, "partner/mate/lover"),
					Personality = getValue(InfoResponse.InfoGroup.GeneralDetails, "personality"),
					Pets = getValue(InfoResponse.InfoGroup.GeneralDetails, "pets/slaves"),
					SkinColor = getValue(InfoResponse.InfoGroup.GeneralDetails, "fur/scale/skin color"),
					Species = getValue(InfoResponse.InfoGroup.GeneralDetails, "species"),
					Weight = getValue(InfoResponse.InfoGroup.GeneralDetails, "weight")
				},
				RPingDetails =
				{
					CurrentlyLooking = getValue(InfoResponse.InfoGroup.RPingPreferences, "currently looking for")
				},
				SexualDetails =
				{
					BreastSize = getValue(InfoResponse.InfoGroup.SexualDetails, "breast size"),
					CockDiameter = getValue(InfoResponse.InfoGroup.SexualDetails, "cock diameter (inches)"),
					CockLength = getValue(InfoResponse.InfoGroup.SexualDetails, "cock length (inches)"),
					KnotDiameter = getValue(InfoResponse.InfoGroup.SexualDetails, "knot diameter (inches)"),
					Measurements = getValue(InfoResponse.InfoGroup.SexualDetails, "measurements"),
					NippleColor = getValue(InfoResponse.InfoGroup.SexualDetails, "nipple color")
				}
			};

			var contactBlock = data.InfoBlocks.Values.FirstOrDefault(b => b.Group == InfoResponse.InfoGroup.ContactDetails);
			if (contactBlock != null)
				info.ContactDetails = contactBlock.Items.ToDictionary(k => k.Name, v => v.Value);

			return info;
		}

		public async Task<Dictionary<Info.KinkInfo, Info.KinkChoice>> GetKinks(string name)
		{
			var data = await UpdateCharacter<KinksResponse>(Path.CharacterGetKinks, name);
			// TODO
			return null;
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
		sealed class CustomKinkResponse : Response
		{
			public class Kink
			{
				[JsonProperty("name")]
				public string Name { get; set; }
				[JsonProperty("description")]
				public string Description { get; set; }
			}

			[JsonProperty("kinks")]
			public List<Kink> Kinks { get; set; }
		}
		sealed class ImagesResponse : Response
		{
			public class ImageData
			{
				[JsonProperty("width")]
				public uint Width { get; set; }
				[JsonProperty("height")]
				public uint Height { get; set; }
				[JsonProperty("url")]
				public string Url { get; set; }
				[JsonProperty("description")]
				public string Description { get; set; }
			}

			[JsonProperty("images")]
			public List<ImageData> Images { get; set; }
		}
		sealed class InfoResponse : Response
		{
			public enum InfoGroup
			{
				[EnumValue("Contact details/Sites")]
				ContactDetails,
				[EnumValue("General details")]
				GeneralDetails,
				[EnumValue("RPing preferences")]
				RPingPreferences,
				[EnumValue("Sexual details")]
				SexualDetails
			}

			public class InfoBlock
			{
				public class Data
				{
					[JsonProperty("id")]
					public int ID { get; set; }
					[JsonProperty("name")]
					public string Name { get; set; }
					[JsonProperty("value")]
					public string Value { get; set; }
				}
				[JsonProperty("group")]
				public InfoGroup Group { get; set; }
				[JsonProperty("items")]
				public List<Data> Items { get; set; }
			}
			
			[JsonProperty("info")]
			public Dictionary<int, InfoBlock> InfoBlocks { get; set; }
		}
		sealed class KinksResponse : Response
		{

		}
	}
}
