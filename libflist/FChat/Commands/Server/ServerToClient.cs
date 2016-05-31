using System;
using Newtonsoft.Json;
using libflist.Util.Converters;

namespace libflist.FChat.Commands
{

	[Reply("ADL")]
	public sealed class Server_ADL_ChatListOPs : Command
	{
		[JsonProperty(PropertyName = "ops")]
		public string[] OPs { get; set; }
	}

	[Reply("AOP")]
	public sealed class Server_AOP_ChatAddOP : Command
	{
		[JsonProperty(PropertyName = "character")]
		public string Character { get; set; }
	}

	[Reply("BRO")]
	public sealed class Server_BRO_ChatBroadcastMessage : Command
	{
		[JsonProperty(PropertyName = "message")]
		public string Message { get; set; }
	}

	[Reply("CHA")]
	public sealed class Server_CHA_ChatListPublicChannels : Command
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
	public sealed class Server_CIU_ChatChannelInvite : Command
	{
		[JsonProperty(PropertyName = "sender")]
		public string Sender { get; set; }

		[JsonProperty(PropertyName = "title")]
		public string Title { get; set; }
		[JsonProperty(PropertyName = "name")]
		public string Channel { get; set; }
	}

	[Reply("CON")]
	public sealed class Server_CON_ChatConnectedUsers : Command
	{
		[JsonProperty(PropertyName = "count")]
		public int ConnectedUsers { get; set; }
	}


	[Reply("DOP")]
	public sealed class Server_DOP_ChatRemoveOP : Command
	{
		[JsonProperty(PropertyName = "character")]
		public string Character { get; set; }
	}

	[Reply("ERR")]
	public sealed class Server_ERR_ChatError : Command
	{
		[JsonProperty(PropertyName = "message")]
		public string Error { get; set; }
		[JsonProperty(PropertyName = "number")]
		public int ErrorID { get; set; }
	}

	[Reply("FKS")]
	public sealed class Server_FKS_ChatFindCharacters : Command
	{
		[JsonProperty(PropertyName = "characters")]
		public string[] Characters { get; set; }
		[JsonProperty(PropertyName = "kinks")]
		public int[] Kinks { get; set; }
	}

	[Reply("FRL")]
	public sealed class Server_FRL_ChatListFriends : Command
	{
		[JsonProperty(PropertyName = "characters")]
		public string[] FriendsAndBookmarks { get; set; }
	}

	[Reply("HLO")]
	public sealed class Server_HLO_ChatHello : Command
	{
		[JsonProperty(PropertyName = "message")]
		public string Message { get; set; }
	}

	[Reply("IDN")]
	public sealed class Server_IDN_ChatIdentify : Command
	{
		[JsonProperty(PropertyName = "character")]
		public string Character { get; set; }
	}

	[Reply("IGN")]
	public sealed class Server_IGN_ChatListIgnores : Command
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
	public sealed class Server_LIS_ChatListUsers : Command
	{
		[JsonProperty(PropertyName = "characters")]
		public string[][] CharacterData { get; set; }
	}

	[Reply("ORS")]
	public sealed class Server_ORS_ChatListPrivateChannels : Command
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
	public sealed class Server_PIN_ChatPing : Command
	{
		
	}

	[Reply("RTB")]
	public sealed class Server_RTB_FListMessage : Command, Command.ICharacterCommand
	{
		[JsonProperty(PropertyName = "type")]
		public string Type { get; set; }

		[JsonProperty(PropertyName = "character")]
		public string Character { get; set; }
	}

	[Reply("SFC")]
	public sealed class Server_SFC_ChatIssueReport : Command
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
	public sealed class Server_SYS_ChatSYSMessage : Command, Command.IChannelCommand
	{
		[JsonProperty(PropertyName = "channel")]
		public string Channel { get; set; }

		[JsonProperty(PropertyName = "message")]
		public string Message { get; set; }
	}

	[Reply("UPT")]
	public sealed class Server_UPT_ChatUptime : Command
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
	public sealed class Server_VAR_ChatVariable : Command
	{
		[JsonProperty(PropertyName = "variable")]
		public string Name { get; set; }

		[JsonProperty(PropertyName = "value")]
		public object Value { get; set; }
	}
}