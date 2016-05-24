using libflist.Util.Converters;
using Newtonsoft.Json;

namespace libflist.FChat.Commands
{
	public sealed class User
	{
		[JsonProperty(PropertyName = "identity")]
		public string Identity { get; set; }
	}

	[Reply("CDS")]
	public sealed class Server_CDS_ChannelChangeDescription : Command, Command.IChannelCommand
	{
		[JsonProperty(PropertyName = "channel")]
		public string Channel { get; set; }
		[JsonProperty(PropertyName = "description")]
		public string Description { get; set; }
	}

	[Reply("CBU")]
	public sealed class Server_CBU_ChannelBanCharacter : Command, Command.IChannelCommand
	{
		[JsonProperty(PropertyName = "operator")]
		public string OP { get; set; }

		[JsonProperty(PropertyName = "channel")]
		public string Channel { get; set; }

		[JsonProperty(PropertyName = "character")]
		public string Character { get; set; }
	}

	[Reply("CKU")]
	public sealed class Server_CKU_ChannelKickCharacter : Command, Command.IChannelCommand
	{
		[JsonProperty(PropertyName = "operator")]
		public string OP { get; set; }

		[JsonProperty(PropertyName = "channel")]
		public string Channel { get; set; }

		[JsonProperty(PropertyName = "character")]
		public string Character { get; set; }
	}

	[Reply("COA")]
	public sealed class Server_COA_ChannelMakeOP : Command, Command.IChannelCommand
	{
		[JsonProperty(PropertyName = "channel")]
		public string Channel { get; set; }

		[JsonProperty(PropertyName = "character")]
		public string Character { get; set; }
	}

	[Reply("COL")]
	public sealed class Server_COL_ChannelGetOPs : Command, Command.IChannelCommand
	{
		[JsonProperty(PropertyName = "channel")]
		public string Channel { get; set; }

		[JsonProperty(PropertyName = "oplist")]
		public string[] OPs { get; set; }
	}

	[Reply("COR")]
	public sealed class Server_COR_ChannelRemoveOP : Command, Command.IChannelCommand
	{
		[JsonProperty(PropertyName = "channel")]
		public string Channel { get; set; }

		[JsonProperty(PropertyName = "character")]
		public string Character { get; set; }
	}

	[Reply("CSO")]
	public sealed class Server_CSO_ChannelSetOwner : Command, Command.IChannelCommand
	{
		[JsonProperty(PropertyName = "channel")]
		public string Channel { get; set; }

		[JsonProperty(PropertyName = "character")]
		public string Character { get; set; }
	}

	[Reply("CTU")]
	public sealed class Server_CTU_ChannelTimeoutCharacter : Command, Command.IChannelCommand
	{
		[JsonProperty(PropertyName = "operator")]
		public string OP { get; set; }

		[JsonProperty(PropertyName = "channel")]
		public string Channel { get; set; }

		[JsonProperty(PropertyName = "character")]
		public string Character { get; set; }

		[JsonProperty(PropertyName = "length")]
		public double Minutes { get; set; }
	}

	[Reply("CUB")]
	public sealed class Server_CUB_ChannelUnbanCharacter : Command, Command.IChannelCommand
	{
		[JsonProperty(PropertyName = "operator")]
		public string OP { get; set; }

		[JsonProperty(PropertyName = "channel")]
		public string Channel { get; set; }

		[JsonProperty(PropertyName = "character")]
		public string Character { get; set; }
	}

	[Reply("ICH")]
	public sealed class Server_ICH_ChannelInitialData : Command, Command.IChannelCommand
	{
		[JsonProperty(PropertyName = "channel")]
		public string Channel { get; set; }

		[JsonProperty(PropertyName = "users")]
		public User[] Users { get; set; }

		[JsonProperty(PropertyName = "mode",
			ItemConverterType = typeof(JsonEnumConverter),
			ItemConverterParameters = new object[]{ JsonEnumConverter.EnumHandling.Lowercase })]
		public ChannelMode Mode { get; set; }
	}

	[Reply("JCH")]
	public sealed class Server_JCH_ChannelJoin : Command, Command.IChannelCommand
	{
		[JsonProperty(PropertyName = "channel", Required = Required.Always)]
		public string Channel { get; set; }

		[JsonProperty(PropertyName = "character")]
		public User Character { get; set; }

		[JsonProperty(PropertyName = "title")]
		public string Title { get; set; }
	}

	[Reply("LCH")]
	public sealed class Server_LCH_ChannelLeave : Command, Command.IChannelCommand
	{
		[JsonProperty(PropertyName = "channel", Required = Required.Always)]
		public string Channel { get; set; }

		[JsonProperty(PropertyName = "character")]
		public string Character { get; set; }
	}

	[Reply("LRP")]
	public sealed class Server_LRP_ChannelLFRPMessage : Command, Command.IChannelCommand
	{
		[JsonProperty(PropertyName = "channel", Required = Required.Always)]
		public string Channel { get; set;  }

		[JsonProperty(PropertyName = "message")]
		public string AD { get; set; }

		[JsonProperty(PropertyName = "character")]
		public string Character { get; set; }
	}

	[Reply("MSG")]
	public sealed class Server_MSG_ChannelChatMessage : Command, Command.IChannelCommand
	{
		[JsonProperty(PropertyName = "channel", Required = Required.Always)]
		public string Channel { get; set;  }

		[JsonProperty(PropertyName = "message")]
		public string Message { get; set; }

		[JsonProperty(PropertyName = "character")]
		public string Character { get; set; }
	}

	[Reply("RLL")]
	public sealed class Server_RLL_ChannelRollMessage : Command, Command.IChannelCommand
	{
		public enum RollType
		{
			Dice,
			Bottle
		}

		[JsonProperty(PropertyName = "channel", Required = Required.Always)]
		public string Channel { get; set;  }

		[JsonProperty(PropertyName = "type", Required = Required.Always)]
		[JsonConverter(typeof(JsonEnumConverter), JsonEnumConverter.EnumHandling.Lowercase)]
		public RollType Type { get; set; }

		[JsonProperty(PropertyName = "message")]
		public string Message { get; set; }

		[JsonProperty(PropertyName = "rolls")]
		public string[] Rolls { get; set; }
		[JsonProperty(PropertyName = "results")]
		public int[] Results { get; set; }
		[JsonProperty(PropertyName = "endresult")]
		public int EndResult { get; set; }

		[JsonProperty(PropertyName = "character")]
		public string Character { get; set; }
	}

	[Reply("RMO")]
	public sealed class Server_RMO_ChannelSetMode : Command, Command.IChannelCommand
	{
		[JsonProperty(PropertyName = "channel", Required = Required.Always)]
		public string Channel { get; set; }

		[JsonProperty(PropertyName = "mode", Required = Required.Always)]
		[JsonConverter(typeof(JsonEnumConverter), JsonEnumConverter.EnumHandling.Lowercase)]
		public ChannelMode Mode { get; set; }
	}
	[Reply("RST")]
	public sealed class Server_RST_ChannelSetStatus : Command, Command.IChannelCommand
	{
		[JsonProperty(PropertyName = "channel", Required = Required.Always)]
		public string Channel { get; set; }

		[JsonProperty(PropertyName = "status", Required = Required.Always)]
		[JsonConverter(typeof(JsonEnumConverter), JsonEnumConverter.EnumHandling.Lowercase)]
		public ChannelStatus Status { get; set; }
	}
}

