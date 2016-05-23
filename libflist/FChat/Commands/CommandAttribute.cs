using System;

namespace libflist.FChat.Commands
{
	public enum ResponseType
	{
		Default,
		None,
		Multiple
	}

	public enum UserRight
	{
		Disconnected = -1,

		User = 0,
		ChatOP,
		Admin
	}

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

