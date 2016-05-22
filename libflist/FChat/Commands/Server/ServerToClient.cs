using System;
using Newtonsoft.Json;
using libflist.Util;
using libflist.Util.Converters;
using libflist.Connection.Types;

namespace libflist.FChat.Commands.Server
{
	public class User
	{
		[JsonProperty(PropertyName = "identity")]
		public string Identity { get; set; }
	}

	[Reply("ADL")]
	public class ChatOPList : Command
	{
		[JsonProperty(PropertyName = "ops")]
		public string[] OPs { get; set; }
	}

	[Reply("AOP")]
	public class ChatMakeOP : Command
	{
		[JsonProperty(PropertyName = "character")]
		public string Character { get; set; }
	}

	[Reply("BRO")]
	public class ChatBroadcast : Command
	{
		[JsonProperty(PropertyName = "message")]
		public string Message { get; set; }
	}

	[Reply("CHA")]
	public class ChatGetPublicChannels : Command
	{
		public class Channel
		{
			[JsonProperty(PropertyName = "name")]
			public string Name { get; set; }
			[JsonProperty(PropertyName = "characters")]
			public int Count { get; set; }
			[JsonProperty(PropertyName = "mode")]
			[JsonConverter(typeof(JsonEnumConverter), JsonEnumConverter.EnumHandling.Lowercase)]
			public ChannelMode Mode { get; set; }
		}

		[JsonProperty(PropertyName = "channels")]
		public Channel[] Channels { get; set; }
	}

	[Reply("CIU")]
	public class InviteCharacterReply : Command
	{
		[JsonProperty(PropertyName = "sender")]
		public string Sender { get; set; }

		[JsonProperty(PropertyName = "title")]
		public string Title { get; set; }
		[JsonProperty(PropertyName = "name")]
		public string Name { get; set; }
	}

	[Reply("CON")]
	public class ServerConnected : Command
	{
		[JsonProperty(PropertyName = "count")]
		public int ConnectedUsers { get; set; }
	}


	[Reply("DOP")]
	public class ChatRemoveOP : Command
	{
		[JsonProperty(PropertyName = "character")]
		public string Character { get; set; }
	}

	[Reply("ERR")]
	public class ChatError : Command
	{
		[JsonProperty(PropertyName = "message")]
		public string Error { get; set; }
		[JsonProperty(PropertyName = "number")]
		public int ErrorID { get; set; }
	}

	[Reply("FKS")]
	public class ChatFindCharacters : Command
	{
		[JsonProperty(PropertyName = "characters")]
		public string[] Characters { get; set; }
		[JsonProperty(PropertyName = "kinks")]
		public int[] Kinks { get; set; }
	}

	[Reply("FRL")]
	public class FriendListReply : Command
	{
		[JsonProperty(PropertyName = "characters")]
		public string[] FriendsAndBookmarks { get; set; }
	}

	[Reply("HLO")]
	public class ServerHello : Command
	{
		[JsonProperty(PropertyName = "message")]
		public string Message { get; set; }
	}

	[Reply("IDN")]
	public class ServerIdentify : Command
	{
		[JsonProperty(PropertyName = "character")]
		public string Character { get; set; }
	}

	[Reply("IGN")]
	public class IgnoreListReply : Command
	{
		public enum IgnoreAction
		{
			Init,

			Add,
			Delete,
			List,
			Notify
		}

		[JsonProperty(PropertyName = "action")]
		[JsonConverter(typeof(JsonEnumConverter), JsonEnumConverter.EnumHandling.Lowercase)]
		public IgnoreAction Action { get; set; }

		[JsonProperty(PropertyName = "characters")]
		public string[] Characters { get; set; }
		[JsonProperty(PropertyName = "character")]
		public string Character { get; set; }
	}

	[Reply("LIS")]
	public class UserListReply : Command
	{
		[JsonProperty(PropertyName = "characters")]
		public string[][] CharacterData { get; set; }
	}

	[Reply("ORS")]
	public class PrivateChannelListReply : Command
	{
		public class Channel
		{
			[JsonProperty(PropertyName = "name")]
			public string ID { get; set; }
			[JsonProperty(PropertyName = "characters")]
			public int Count { get; set; }
			[JsonProperty(PropertyName = "title")]
			public string Title { get; set; }
		}

		[JsonProperty(PropertyName = "channels")]
		public Channel[] Channels { get; set; }
	}

	[Reply("PIN")]
	public class ServerPing : Command
	{
		
	}

	[Reply("RTB")]
	public class FListMessageReply : Command, Command.ICharacterCommand
	{
		[JsonProperty(PropertyName = "type")]
		public string Type { get; set; }

		[JsonProperty(PropertyName = "character")]
		public string Character { get; set; }
	}

	[Reply("SFC")]
	public class IssueReportReply : Command
	{
		public enum ReportType
		{
			Report,
			Confirm
		}

		[JsonProperty(PropertyName = "action")]
		public ReportType Type { get; set; }

		[JsonProperty(PropertyName = "timestamp")]
		public string Timestamp { get; set;}
		[JsonProperty(PropertyName = "character")]
		public string Character { get; set;}

		// Report data;
		[JsonProperty(PropertyName = "callid")]
		public int AlertID { get; set; }
		[JsonProperty(PropertyName = "logid")]
		public int LogID { get; set; }
		[JsonProperty(PropertyName = "report")]
		public string Report { get; set; }

		// Confirm data
		[JsonProperty(PropertyName = "moderator")]
		public string Moderator { get; set; }
	}

	[Reply("SYS")]
	public class SysReply : Command, Command.IChannelCommand
	{
		[JsonProperty(PropertyName = "channel")]
		public string Channel { get; set; }

		[JsonProperty(PropertyName = "message")]
		public string Message { get; set; }
	}

	[Reply("UPT")]
	public class ServerUptime : Command
	{
		[JsonProperty(PropertyName = "time")]
		[JsonConverter(typeof(JsonDateTimeConverter), JsonDateTimeConverter.TimestampPrecision.Seconds)]
		public DateTime Timestamp { get; set; }

		[JsonProperty(PropertyName = "starttime")]
		[JsonConverter(typeof(JsonDateTimeConverter), JsonDateTimeConverter.TimestampPrecision.Seconds)]
		public DateTime StartTime { get; set; }

		[JsonProperty(PropertyName = "startstring")]
		public string StartTimeString { get; set; }

		[JsonProperty(PropertyName = "accepted")]
		public int AcceptedConnections { get; set; }

		[JsonProperty(PropertyName = "channels")]
		public int Channels { get; set; }

		[JsonProperty(PropertyName = "users")]
		public int CurrentUsers { get; set; }

		[JsonProperty(PropertyName = "maxusers")]
		public int PeakUsers { get; set; }
	}

	[Reply("VAR")]
	public class ServerVariable : Command
	{
		[JsonProperty(PropertyName = "variable")]
		public string Name { get; set; }

		[JsonProperty(PropertyName = "value")]
		public object Value { get; set; }
	}
}