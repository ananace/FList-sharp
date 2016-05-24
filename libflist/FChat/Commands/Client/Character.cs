using Newtonsoft.Json;
using libflist.Util.Converters;

namespace libflist.FChat.Commands
{
	[Command("KIN", Response = ResponseType.Multiple, ResponseToken = "KID")]
	public sealed class Client_KIN_CharacterGetKinks : Command
	{
		[JsonProperty(PropertyName = "character", Required = Required.DisallowNull)]
		public string Character { get; set; }
	}

	[Command("PRI")]
	public sealed class Client_PRI_CharacterSendMessage : Command
	{
		[JsonProperty(PropertyName = "recipient", Required = Required.DisallowNull)]
		public string Character { get; set; }
		[JsonProperty(PropertyName = "message", Required = Required.DisallowNull)]
		public string Message { get; set; }
	}

	[Command("PRO")]
	public sealed class Client_PRO_CharacterGetTags : Command
	{
		[JsonProperty(PropertyName = "character", Required = Required.DisallowNull)]
		public string Character { get; set; }
	}

	[Command("STA")]
	public sealed class Client_STA_ChatSetStatus : Command
	{
		[JsonProperty(PropertyName = "status",
			ItemConverterType = typeof(JsonEnumConverter),
			ItemConverterParameters = new object[]{ JsonEnumConverter.EnumHandling.Lowercase })]
		public CharacterStatus Status { get; set; }

		[JsonProperty(PropertyName = "statusmsg", Required = Required.AllowNull)]
		public string Message { get; set; }
	}

	[Command("TPN")]
	public sealed class Client_TPN_ChatSetTyping : Command
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

