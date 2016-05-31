using Newtonsoft.Json;
using libflist.Util.Converters;

namespace libflist.FChat.Commands
{
	/// <summary>
	/// Gets the banlist for a channel
	/// </summary>
	[Command("CBL", ResponseToken = "SYS")]
	public sealed class Client_CBL_GetBanlist : Command
	{
		[JsonProperty(PropertyName = "channel", Required = Required.DisallowNull)]
		public string Channel { get; set; }
	}

	/// <summary>
	/// Ban a character from a channel
	/// </summary>
	[Command("CBU")]
	public sealed class Client_CBU_ChannelBanCharacter : Command
	{
		[JsonProperty(PropertyName = "channel", Required = Required.DisallowNull)]
		public string Channel { get; set; }

		[JsonProperty(PropertyName = "character", Required = Required.DisallowNull)]
		public string Character { get; set; }
	}

	/// <summary>
	/// Create a private channel
	/// </summary>
	[Command("CCR", Response = ResponseType.None)]
	public sealed class Client_CCR_ChannelCreatePrivate : Command
	{
		[JsonProperty(PropertyName = "channel", Required = Required.DisallowNull)]
		public string Channel { get; set; }
	}

	/// <summary>
	/// Set the description for a channel
	/// </summary>
	[Command("CDS")]
	public sealed class Client_CDS_ChannelSetDescription : Command
	{
		[JsonProperty(PropertyName = "channel", Required = Required.DisallowNull)]
		public string Channel { get; set; }

		[JsonProperty(PropertyName = "description")]
		public string Description { get; set; }
	}

	/// <summary>
	/// Invite a character to a channel
	/// </summary>
	[Command("CIU", ResponseToken = "SYS")]
	public sealed class Client_CIU_ChannelInviteCharacter : Command
	{
		[JsonProperty(PropertyName = "channel", Required = Required.DisallowNull)]
		public string Channel { get; set; }

		[JsonProperty(PropertyName = "character", Required = Required.DisallowNull)]
		public string Character { get; set; }
	}

	/// <summary>
	/// Kick a user from a channel
	/// </summary>
	[Command("CKU")]
	public sealed class Client_CKU_ChannelKickCharacter : Command
	{
		[JsonProperty(PropertyName = "channel", Required = Required.DisallowNull)]
		public string Channel { get; set; }

		[JsonProperty(PropertyName = "character", Required = Required.DisallowNull)]
		public string Character { get; set; }
	}

	/// <summary>
	/// Give OP to a character in a channel
	/// </summary>
	[Command("COA")]
	public sealed class Client_COA_ChannelAddOP : Command
	{
		[JsonProperty(PropertyName = "channel", Required = Required.DisallowNull)]
		public string Channel { get; set; }

		[JsonProperty(PropertyName = "character", Required = Required.DisallowNull)]
		public string Character { get; set; }
	}

	/// <summary>
	/// Get the OP list for a channel
	/// </summary>
	[Command("COL", ResponseToken = "SYS")]
	public sealed class Client_COL_ChannelGetOPs : Command
	{
		[JsonProperty(PropertyName = "channel", Required = Required.DisallowNull)]
		public string Channel { get; set; }
	}

	/// <summary>
	/// Remove OP from a character in a channel
	/// </summary>
	[Command("COR")]
	public sealed class Client_COR_ChannelRemoveOP : Command
	{
		[JsonProperty(PropertyName = "channel", Required = Required.DisallowNull)]
		public string Channel { get; set; }

		[JsonProperty(PropertyName = "character", Required = Required.DisallowNull)]
		public string Character { get; set; }
	}

	/// <summary>
	/// Create an official channel
	/// </summary>
	[Command("CRC", MinRight = UserRight.Admin)]
	public sealed class Client_CRC_ChannelCreateOfficial : Command
	{
		[JsonProperty(PropertyName = "channel", Required = Required.DisallowNull)]
		public string Channel { get; set; }
	}

	/// <summary>
	/// Set the owner of a channel
	/// </summary>
	[Command("CSO")]
	public sealed class Client_CSO_ChannelSetOwner : Command
	{
		[JsonProperty(PropertyName = "channel", Required = Required.DisallowNull)]
		public string Channel { get; set; }

		[JsonProperty(PropertyName = "character", Required = Required.DisallowNull)]
		public string Character { get; set; }
	}

	/// <summary>
	/// Time out a character from a channel
	/// </summary>
	[Command("CTU")]
	public sealed class Client_CTU_ChannelTimeoutCharacter : Command
	{
		[JsonProperty(PropertyName = "channel", Required = Required.DisallowNull)]
		public string Channel { get; set; }

		[JsonProperty(PropertyName = "character", Required = Required.DisallowNull)]
		public string Character { get; set; }

		[JsonProperty(PropertyName = "timeout", Required = Required.Always)]
		public double Minutes { get; set; }
	}

	/// <summary>
	/// Unbans a character from a channel
	/// </summary>
	[Command("CUB", ResponseToken = "SYS")]
	public sealed class Client_CUB_ChannelUnbanCharacter : Command
	{
		[JsonProperty(PropertyName = "channel", Required = Required.DisallowNull)]
		public string Channel { get; set; }

		[JsonProperty(PropertyName = "character", Required = Required.DisallowNull)]
		public string Character { get; set; }
	}

	/// <summary>
	/// Joins a channel
	/// </summary>
	[Command("JCH")]
	public sealed class Client_JCH_ChannelJoin : Command
	{
		[JsonProperty(PropertyName = "channel", Required = Required.DisallowNull)]
		public string Channel { get; set; }
	}

	/// <summary>
	/// Leaves a channel
	/// </summary>
	[Command("LCH")]
	public sealed class Client_LCH_ChannelLeave : Command
	{
		[JsonProperty(PropertyName = "channel", Required = Required.DisallowNull)]
		public string Channel { get; set; }
	}

	/// <summary>
	/// Sends a LFRP message to a channel
	/// </summary>
	[Command("LRP")]
	public sealed class Client_LRP_ChannelLFRPMessage : Command
	{
		[JsonProperty(PropertyName = "channel", Required = Required.DisallowNull)]
		public string Channel { get; set; }

		[JsonProperty(PropertyName = "message", Required = Required.DisallowNull)]
		public string Message { get; set; }
	}

	/// <summary>
	/// Sends a chat message to a channel
	/// </summary>
	[Command("MSG")]
	public sealed class Client_MSG_ChannelChatMessage : Command
	{
		[JsonProperty(PropertyName = "channel", Required = Required.DisallowNull)]
		public string Channel { get; set; }

		[JsonProperty(PropertyName = "message", Required = Required.DisallowNull)]
		public string Message { get; set; }
	}

	[Command("RLL")]
	public sealed class Client_RLL_ChannelRollMessage : Command
	{
		[JsonProperty(PropertyName = "channel", Required = Required.DisallowNull)]
		public string Channel { get; set; }

		[JsonProperty(PropertyName = "dice", Required = Required.DisallowNull)]
		public string Roll { get; set; }
	}

	[Command("RMO")]
	public sealed class Client_RMO_ChannelSetMode : Command
	{
		[JsonProperty(PropertyName = "channel", Required = Required.DisallowNull)]
		public string Channel { get; set; }

		[JsonProperty(PropertyName = "mode", Required = Required.DisallowNull,
			ItemConverterType = typeof(JsonEnumConverter),
			ItemConverterParameters = new object[]{ JsonEnumConverter.EnumHandling.Lowercase })]
		public ChannelMode Mode { get; set; }
	}

	[Command("RST")]
	public sealed class Client_RST_ChannelSetStatus : Command
	{
		[JsonProperty(PropertyName = "channel", Required = Required.DisallowNull)]
		public string Channel { get; set; }

		[JsonProperty(PropertyName = "status", Required = Required.DisallowNull,
			ItemConverterType = typeof(JsonEnumConverter),
			ItemConverterParameters = new object[]{ JsonEnumConverter.EnumHandling.Lowercase })]
		public ChannelStatus Status { get; set; }
	}
}
