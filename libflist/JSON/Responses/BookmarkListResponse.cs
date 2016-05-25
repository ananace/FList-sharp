using System;
using libflist.JSON;
using Newtonsoft.Json;

namespace libflist
{
	sealed class BookmarkListResponse : Response
	{
		[JsonProperty(PropertyName = "ticket")]
		public string Ticket { get; set; }

		[JsonProperty(PropertyName = "default_characters")]
		public string DefaultCharacter { get; set; }
		[JsonProperty(PropertyName = "characters")]
		public string[] Characters { get; set; }
	}
}

