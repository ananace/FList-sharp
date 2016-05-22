using System;
using System.Linq;
using System.Reflection;
using Newtonsoft.Json;
using libflist.FChat;

namespace libflist.FChat.Commands
{
	public enum CommandSource
	{
		Client,
		Server
	}

	public abstract class Command
	{
		[JsonIgnore]
		public string Token
		{
			get
			{
				dynamic att = GetType().GetCustomAttribute<CommandAttribute>();
				if (att != null)
					return att.Token;

				att = GetType().GetCustomAttribute<ReplyAttribute>();
				if (att != null)
					return att.Token;

				return null;
			}
		}

		[JsonIgnore]
		public CommandSource Source { get { return GetType().GetCustomAttribute<CommandAttribute>() != null ? CommandSource.Client : CommandSource.Server; } }
		
		public string Serialize()
		{
			var type = GetType();
			var mmb = type.GetProperties().Where(p => p.GetCustomAttribute<JsonIgnoreAttribute>() == null);

			return !mmb.Any()
				? Token
				: string.Format("{0} {1}", Token, JsonConvert.SerializeObject(this, Formatting.None));
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

