using System;

namespace libflist.Connection.Commands
{
	[AttributeUsage(AttributeTargets.Class)]
	public sealed class ReplyAttribute : Attribute
	{
		public string Token { get; set; }

		public ReplyAttribute(string token)
		{
			Token = token;
		}
	}
}

