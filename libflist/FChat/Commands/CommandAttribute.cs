using System;
using libflist.Connection.Types;

namespace libflist.FChat.Commands
{
	[AttributeUsage(AttributeTargets.Class)]
	public sealed class CommandAttribute : Attribute
	{
		public string Token { get; set; }
		public UserRight MinRight { get; set; } = UserRight.User;
		public ResponseType Response { get; set; } = ResponseType.Default;
		public string ResponseToken { get; set; }

		public CommandAttribute(string token)
		{
			Token = token;
			ResponseToken = token;
		}
	}
}

