using System;
using Newtonsoft.Json;

namespace libflist.JSON.Responses
{
	public sealed class TicketResponse : Response
	{
		[JsonProperty(PropertyName = "ticket")]
		public string Ticket { get; set; }

		[JsonProperty(PropertyName = "default_characters")]
		public string DefaultCharacter { get; set; }
		[JsonProperty(PropertyName = "characters")]
		public string[] Characters { get; set; }
		/*
		public sealed class Friend
		{
			
		}

		public sealed class Bookmark
		{
			
		}

		[JsonProperty(PropertyName = "friends")]
		public Friend[] Friends { get; set; }
		[JsonProperty(PropertyName = "bookmarks")]
		public Bookmark[] Bookmarks { get; set; }
		*/
	}
}

