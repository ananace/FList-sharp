using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Newtonsoft.Json;

namespace libflist.Connection.Commands
{
	public static class CommandParser
	{
		static Dictionary<string, Type> _CommandTypes = new Dictionary<string, Type>();
		static Dictionary<string, Type> _ReplyTypes = new Dictionary<string, Type>();
		public static bool AllowMeta { get; set; } = true;

		public static IEnumerable<string> ImplementedCommands
		{
			get
			{
				return _CommandTypes.Select(t => t.Key);
			}
		}
		public static IEnumerable<string> ImplementedReplies
		{
			get
			{
				return _ReplyTypes.Select(t => t.Key);
			}
		}

		static CommandParser()
		{
			AddCommandsFrom(Assembly.GetExecutingAssembly());
		}

		public static void AddCommandsFrom(Assembly assembly)
		{
			foreach (var cmd in assembly.GetTypes().Where(t => t.BaseType == typeof(Command)))
			{
				var att = cmd.GetCustomAttribute<CommandAttribute>();
				if (att == null)
				{
					var att2 = cmd.GetCustomAttribute<ReplyAttribute>();
					_ReplyTypes[att2.Token] = cmd;
				}
				else
					_CommandTypes[att.Token] = cmd;
			}
		}

		public static Command ParseCommand(string Token, string JSON, bool? AllowMeta = null)
		{
			if (!_CommandTypes.ContainsKey(Token))
			{
				if (CommandParser.AllowMeta && !(AllowMeta.HasValue && AllowMeta.Value == false) || (AllowMeta.HasValue && AllowMeta.Value))
					return new Meta.UnknownCommand { CMDToken = Token, Data = JSON };
			}
			else
			{
				var type = _CommandTypes[Token];
				if (!type.GetProperties().Where(p => p.GetCustomAttribute<JsonIgnoreAttribute>() == null).Any())
					return type.Assembly.CreateInstance(type.FullName) as Command;

				try
				{
					return JsonConvert.DeserializeObject(JSON, type) as Command;
				}
				catch (Exception ex)
				{
					if (CommandParser.AllowMeta && !(AllowMeta.HasValue && AllowMeta.Value == false) || (AllowMeta.HasValue && AllowMeta.Value))
						return new Meta.FailedCommand { CMDToken = Token, Data = JSON, Exception = ex };
				}
			}

			return null;
		}

		public static Command ParseReply(string Token, string JSON, bool? AllowMeta = null)
		{
			if (!_ReplyTypes.ContainsKey(Token))
			{
				if (CommandParser.AllowMeta && !(AllowMeta.HasValue && AllowMeta.Value == false) || (AllowMeta.HasValue && AllowMeta.Value))
					return new Meta.UnknownReply { CMDToken = Token, Data = JSON };
			}
			else
			{
				var type = _ReplyTypes[Token];
				if (!type.GetProperties().Where(p => p.GetCustomAttribute<JsonIgnoreAttribute>() == null).Any())
					return type.Assembly.CreateInstance(type.FullName) as Command;

				try
				{
					return JsonConvert.DeserializeObject(JSON, type) as Command;
				}
				catch (Exception ex)
				{
					if (CommandParser.AllowMeta && !(AllowMeta.HasValue && AllowMeta.Value == false) || (AllowMeta.HasValue && AllowMeta.Value))
						return new Meta.FailedReply { CMDToken = Token, Data = JSON, Exception = ex };
				}
			}

			return null;
		}
	}
}

