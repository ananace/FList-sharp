﻿using System;
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
				if (att != null)
					return att.Token;

				att = GetType().GetCustomAttribute<ReplyAttribute>();
				if (att != null)
					return att.Token;

				return null;
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

