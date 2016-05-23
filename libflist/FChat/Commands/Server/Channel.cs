using libflist.Connection.Types;
using libflist.Util.Converters;
using Newtonsoft.Json;

namespace libflist.FChat.Commands.Server.Channel
{
	[Reply("CDS")]
	public class ChangeDescriptionReply : Command, Command.IChannelCommand
	{
		[JsonProperty(PropertyName = "channel")]
		public string Channel { get; set; }
		[JsonProperty(PropertyName = "description")]
		public string Description { get; set; }
	}

	[Reply("CBU")]
	public class BanCharacterReply : Command, Command.IChannelCommand
	{
		[JsonProperty(PropertyName = "operator")]
		public string OP { get; set; }

		[JsonProperty(PropertyName = "channel")]
		public string Channel { get; set; }

		[JsonProperty(PropertyName = "character")]
		public string Character { get; set; }
	}

	[Reply("CKU")]
	public class KickCharacterReply : Command, Command.IChannelCommand
	{
		[JsonProperty(PropertyName = "operator")]
		public string OP { get; set; }

		[JsonProperty(PropertyName = "channel")]
		public string Channel { get; set; }

		[JsonProperty(PropertyName = "character")]
		public string Character { get; set; }
	}

	[Reply("COA")]
	public class MakeOPReply : Command, Command.IChannelCommand
	{
		[JsonProperty(PropertyName = "channel")]
		public string Channel { get; set; }

		[JsonProperty(PropertyName = "character")]
		public string Character { get; set; }
	}

	[Reply("COL")]
	public class OPListReply : Command, Command.IChannelCommand
	{
		[JsonProperty(PropertyName = "channel")]
		public string Channel { get; set; }

		[JsonProperty(PropertyName = "oplist")]
		public string[] OPs { get; set; }
	}

	[Reply("COR")]
	public class RemoveOPReply : Command, Command.IChannelCommand
	{
		[JsonProperty(PropertyName = "channel")]
		public string Channel { get; set; }

		[JsonProperty(PropertyName = "character")]
		public string Character { get; set; }
	}

	[Reply("CSO")]
	public class SetOwnerReply : Command, Command.IChannelCommand
	{
		[JsonProperty(PropertyName = "channel")]
		public string Channel { get; set; }

		[JsonProperty(PropertyName = "character")]
		public string Character { get; set; }
	}

	[Reply("CTU")]
	public class TimeoutCharacterReply : Command, Command.IChannelCommand
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
	public class UnbanCharacterReply : Command, Command.IChannelCommand
	{
		[JsonProperty(PropertyName = "operator")]
		public string OP { get; set; }

		[JsonProperty(PropertyName = "channel")]
		public string Channel { get; set; }

		[JsonProperty(PropertyName = "character")]
		public string Character { get; set; }
	}

	[Reply("ICH")]
	public class InitialDataReply : Command, Command.IChannelCommand
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
	public class JoinReply : Command, Command.IChannelCommand
	{
		[JsonProperty(PropertyName = "channel", Required = Required.Always)]
		public string Channel { get; set; }

		[JsonProperty(PropertyName = "character")]
		public User Character { get; set; }

		[JsonProperty(PropertyName = "title")]
		public string Title { get; set; }
	}

	[Reply("LCH")]
	public class LeaveReply : Command, Command.IChannelCommand
	{
		[JsonProperty(PropertyName = "channel", Required = Required.Always)]
		public string Channel { get; set; }

		[JsonProperty(PropertyName = "character")]
		public string Character { get; set; }
	}

	[Reply("LRP")]
	public class SendLFRPReply : Command, Command.IChannelCommand
	{
		[JsonProperty(PropertyName = "channel", Required = Required.Always)]
		public string Channel { get; set;  }

		[JsonProperty(PropertyName = "message")]
		public string AD { get; set; }

		[JsonProperty(PropertyName = "character")]
		public string Character { get; set; }
	}

	[Reply("MSG")]
	public class SendMessageReply : Command, Command.IChannelCommand
	{
		[JsonProperty(PropertyName = "channel", Required = Required.Always)]
		public string Channel { get; set;  }

		[JsonProperty(PropertyName = "message")]
		public string Message { get; set; }

		[JsonProperty(PropertyName = "character")]
		public string Character { get; set; }
	}

	[Reply("RLL")]
	public class SendRollReply : Command, Command.IChannelCommand
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
	public class SetModeReply : Command, Command.IChannelCommand
	{
		[JsonProperty(PropertyName = "channel", Required = Required.Always)]
		public string Channel { get; set; }

		[JsonProperty(PropertyName = "mode", Required = Required.Always)]
		[JsonConverter(typeof(JsonEnumConverter), JsonEnumConverter.EnumHandling.Lowercase)]
		public ChannelMode Mode { get; set; }
	}
	[Reply("RST")]
	public class SetStatusReply : Command, Command.IChannelCommand
	{
		[JsonProperty(PropertyName = "channel", Required = Required.Always)]
		public string Channel { get; set; }

		[JsonProperty(PropertyName = "status", Required = Required.Always)]
		[JsonConverter(typeof(JsonEnumConverter), JsonEnumConverter.EnumHandling.Lowercase)]
		public ChannelStatus Status { get; set; }
	}
}

