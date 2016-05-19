using Newtonsoft.Json;
using libflist.Connection.Types;
using libflist.Util.Converters;

namespace libflist.Connection.Commands.Client.Channel
{
	[Command("CBL", ResponseToken = "SYS")]
	public class GetBanlistCommand : Command
	{
		[JsonProperty(PropertyName = "channel", Required = Required.DisallowNull)]
		public string Channel { get; set; }
	}

	[Command("CBU")]
	public class BanCharacterCommand : Command
	{
		[JsonProperty(PropertyName = "channel", Required = Required.DisallowNull)]
		public string Channel { get; set; }

		[JsonProperty(PropertyName = "character", Required = Required.DisallowNull)]
		public string Character { get; set; }
	}

	[Command("CCR")]
	public class CreatePrivateChannelCommand : Command
	{
		[JsonProperty(PropertyName = "channel", Required = Required.DisallowNull)]
		public string Channel { get; set; }
	}

	[Command("CDS")]
	public class SetDescriptionCommand : Command
	{
		[JsonProperty(PropertyName = "channel", Required = Required.DisallowNull)]
		public string Channel { get; set; }

		[JsonProperty(PropertyName = "description")]
		public string Description { get; set; }
	}

	[Command("CIU", ResponseToken = "SYS")]
	public class InviteCharacterCommand : Command
	{
		[JsonProperty(PropertyName = "channel", Required = Required.DisallowNull)]
		public string Channel { get; set; }

		[JsonProperty(PropertyName = "character", Required = Required.DisallowNull)]
		public string Character { get; set; }
	}

	[Command("CKU")]
	public class KickUserCommand : Command
	{
		[JsonProperty(PropertyName = "channel", Required = Required.DisallowNull)]
		public string Channel { get; set; }

		[JsonProperty(PropertyName = "character", Required = Required.DisallowNull)]
		public string Character { get; set; }
	}

	[Command("COA")]
	public class GiveOPCommand : Command
	{
		[JsonProperty(PropertyName = "channel", Required = Required.DisallowNull)]
		public string Channel { get; set; }

		[JsonProperty(PropertyName = "character", Required = Required.DisallowNull)]
		public string Character { get; set; }
	}

	[Command("COL", ResponseToken = "SYS")]
	public class GetOPsCommand : Command
	{
		[JsonProperty(PropertyName = "channel", Required = Required.DisallowNull)]
		public string Channel { get; set; }
	}

	[Command("COR")]
	public class ChannelRemoveOP : Command
	{
		[JsonProperty(PropertyName = "channel", Required = Required.DisallowNull)]
		public string Channel { get; set; }

		[JsonProperty(PropertyName = "character", Required = Required.DisallowNull)]
		public string Character { get; set; }
	}

	[Command("CRC", MinRight = UserRight.Admin)]
	public class CreateOfficialChannelCommand : Command
	{
		[JsonProperty(PropertyName = "channel", Required = Required.DisallowNull)]
		public string Channel { get; set; }
	}

	[Command("CSO")]
	public class SetOwnerCommand : Command
	{
		[JsonProperty(PropertyName = "channel", Required = Required.DisallowNull)]
		public string Channel { get; set; }

		[JsonProperty(PropertyName = "character", Required = Required.DisallowNull)]
		public string Character { get; set; }
	}

	[Command("CTU")]
	public class TimeoutCharacterCommand : Command
	{
		[JsonProperty(PropertyName = "channel", Required = Required.DisallowNull)]
		public string Channel { get; set; }

		[JsonProperty(PropertyName = "character", Required = Required.DisallowNull)]
		public string Character { get; set; }

		[JsonProperty(PropertyName = "timeout", Required = Required.Always)]
		public double Minutes { get; set; }
	}

	[Command("CUB", ResponseToken = "SYS")]
	public class UnbanCharacterCommand : Command
	{
		[JsonProperty(PropertyName = "channel", Required = Required.DisallowNull)]
		public string Channel { get; set; }

		[JsonProperty(PropertyName = "character", Required = Required.DisallowNull)]
		public string Character { get; set; }
	}

	[Command("JCH")]
	public class JoinCommand : Command
	{
		[JsonProperty(PropertyName = "channel", Required = Required.DisallowNull)]
		public string Channel { get; set; }
	}

	[Command("LCH")]
	public class LeaveCommand : Command
	{
		[JsonProperty(PropertyName = "channel", Required = Required.DisallowNull)]
		public string Channel { get; set; }
	}

	[Command("LRP")]
	public class SendLFRPCommand : Command
	{
		[JsonProperty(PropertyName = "channel", Required = Required.DisallowNull)]
		public string Channel { get; set; }

		[JsonProperty(PropertyName = "message", Required = Required.DisallowNull)]
		public string Message { get; set; }
	}

	[Command("MSG")]
	public class SendMessageCommand : Command
	{
		[JsonProperty(PropertyName = "channel", Required = Required.DisallowNull)]
		public string Channel { get; set; }

		[JsonProperty(PropertyName = "message", Required = Required.DisallowNull)]
		public string Message { get; set; }
	}

	[Command("RLL")]
	public class SendRollCommand : Command
	{
		[JsonProperty(PropertyName = "channel", Required = Required.DisallowNull)]
		public string Channel { get; set; }

		[JsonProperty(PropertyName = "dice", Required = Required.DisallowNull)]
		public string Roll { get; set; }
	}

	[Command("RMO")]
	public class SetModeCommand : Command
	{
		[JsonProperty(PropertyName = "channel", Required = Required.DisallowNull)]
		public string Channel { get; set; }

		[JsonProperty(PropertyName = "mode", Required = Required.DisallowNull,
			ItemConverterType = typeof(JsonEnumConverter),
			ItemConverterParameters = new object[]{ JsonEnumConverter.EnumHandling.Lowercase })]
		public ChannelMode Mode { get; set; }
	}

	[Command("RST")]
	public class SetStatusCommand : Command
	{
		[JsonProperty(PropertyName = "channel", Required = Required.DisallowNull)]
		public string Channel { get; set; }

		[JsonProperty(PropertyName = "status", Required = Required.DisallowNull,
			ItemConverterType = typeof(JsonEnumConverter),
			ItemConverterParameters = new object[]{ JsonEnumConverter.EnumHandling.Lowercase })]
		public ChannelStatus Status { get; set; }
	}
}

