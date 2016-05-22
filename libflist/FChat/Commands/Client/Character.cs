using Newtonsoft.Json;
using libflist.Connection.Types;
using libflist.Util.Converters;

namespace libflist.FChat.Commands.Client.Character
{
	[Command("KIN", Response = ResponseType.Multiple, ResponseToken = "KID")]
	public class GetKinksCommand : Command
	{
		[JsonProperty(PropertyName = "character", Required = Required.DisallowNull)]
		public string Character { get; set; }
	}

	[Command("PRI")]
	public class SendMessageCommand : Command
	{
		[JsonProperty(PropertyName = "recipient", Required = Required.DisallowNull)]
		public string Character { get; set; }
		[JsonProperty(PropertyName = "message", Required = Required.DisallowNull)]
		public string Message { get; set; }
	}

	[Command("PRO")]
	public class GetTagsCommand : Command
	{
		[JsonProperty(PropertyName = "character", Required = Required.DisallowNull)]
		public string Character { get; set; }
	}

	[Command("STA")]
	public class SetStatusCommand : Command
	{
		[JsonProperty(PropertyName = "status",
			ItemConverterType = typeof(JsonEnumConverter),
			ItemConverterParameters = new object[]{ JsonEnumConverter.EnumHandling.Lowercase })]
		public CharacterStatus Status { get; set; }

		[JsonProperty(PropertyName = "statusmsg", Required = Required.AllowNull)]
		public string Message { get; set; }
	}

	[Command("TPN")]
	public class SetTypingCommand : Command
	{
		public enum TypingStatus
		{
			Typing,
			Paused,
			Clear
		}

		[JsonProperty(PropertyName = "character", Required = Required.DisallowNull)]
		public string Character { get; set; }

		[JsonProperty(PropertyName = "status",
			ItemConverterType = typeof(JsonEnumConverter),
			ItemConverterParameters = new object[]{ JsonEnumConverter.EnumHandling.Lowercase })]
		public TypingStatus Status { get; set; }
	}
}

