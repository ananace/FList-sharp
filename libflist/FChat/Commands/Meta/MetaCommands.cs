using System;

namespace libflist.FChat.Commands
{
	[Command("???")]
	public sealed class Client_Meta_Unknown : Command
	{
		public string CMDToken { get; set; }
		public string Data { get; set; }
	}
	[Command("!!!")]
	public sealed class Client_Meta_Failed : Command
	{
		public string CMDToken { get; set; }
		public string Data { get; set; }
		public Exception Exception { get; set; }
	}

	[Reply("???")]
	public sealed class Server_Meta_Unknown : Command
	{
		public string CMDToken { get; set; }
		public string Data { get; set; }
	}
	[Reply("!!!")]
	public sealed class Server_Meta_Failed : Command
	{
		public string CMDToken { get; set; }
		public string Data { get; set; }
		public Exception Exception { get; set; }
	}
}

