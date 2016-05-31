using Newtonsoft.Json;

namespace libflist
{
	public class AuthTicket
	{
		internal sealed class TicketResponse : Response
		{
			[JsonIgnore]
			public string Account { get; set; }
			[JsonProperty(PropertyName = "ticket")]
			public string Ticket { get; set; }

			[JsonProperty(PropertyName = "default_characters")]
			public string DefaultCharacter { get; set; }
			[JsonProperty(PropertyName = "characters")]
			public string[] Characters { get; set; }
		}

		internal AuthTicket(TicketResponse Response)
		{
			Account = Response.Account;
			Ticket = Response.Ticket;
			DefaultCharacter = Response.DefaultCharacter;
			Characters = Response.Characters;
		}
		public AuthTicket() { }

		public string Account { get; set; }
		public string Ticket { get; set; }
		public string DefaultCharacter { get; set; }
		public string[] Characters { get; set; }
	}
}
