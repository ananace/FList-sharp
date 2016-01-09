using System;
using System.Linq;
using System.Reflection;
using Newtonsoft.Json;

namespace libflist.Connection.Commands
{
	public abstract class Command
	{
		[JsonIgnore]
		public string Token
		{
			get
			{
				dynamic att = GetType().GetCustomAttribute<CommandAttribute>();

				if (att == null)
					att = GetType().GetCustomAttribute<ReplyAttribute>();
				
				return att.Token;
			}
		}

		[JsonIgnore]
		public bool IsCommand
		{
			get
			{
				return GetType().GetCustomAttribute<CommandAttribute>() != null;	
			}
		}

		[JsonIgnore]
		public bool IsReply
		{
			get
			{
				return GetType().GetCustomAttribute<ReplyAttribute>() != null;	
			}
		}

		public override string ToString()
		{
			var type = GetType();
			var mmb = type.GetProperties().Where(p => p.GetCustomAttribute<JsonIgnoreAttribute>() == null);
			if (!mmb.Any())
				return Token;

			return string.Format("{0} {1}", Token, JsonConvert.SerializeObject(this, Formatting.None));
		}

		public interface ICharacterCommand
		{
			string Character { get; }
		}
		public interface IChannelCommand
		{
			string Channel { get; }
		}
	}
}

