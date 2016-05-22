using System;

namespace libflist.FChat.Commands.Meta
{
	[Command("???")]
	public class UnknownCommand : Command
	{
		public string CMDToken { get; set; }
		public string Data { get; set; }
	}
	[Command("!!!")]
	public class FailedCommand : Command
	{
		public string CMDToken { get; set; }
		public string Data { get; set; }
		public Exception Exception { get; set; }
	}

	[Reply("???")]
	public class UnknownReply : Command
	{
		public string CMDToken { get; set; }
		public string Data { get; set; }
	}
	[Reply("!!!")]
	public class FailedReply : Command
	{
		public string CMDToken { get; set; }
		public string Data { get; set; }
		public Exception Exception { get; set; }
	}
}

