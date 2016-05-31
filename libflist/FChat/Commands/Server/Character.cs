using libflist.Util.Converters;
using Newtonsoft.Json;

namespace libflist.FChat.Commands
{
	[Reply("FLN")]
	public sealed class Server_FLN_CharacterOffline : Command, Command.ICharacterCommand
	{
		[JsonProperty(PropertyName = "character")]
		public string Character { get; set; }
	}

	[Reply("KID")]
	public sealed class Server_KID_CharacterKinkList : Command
	{
		public enum MessageType
		{
			Start,
			Custom,
			End
		}

		[JsonProperty(PropertyName = "type")]
		[JsonConverter(typeof(JsonEnumConverter), JsonEnumConverter.EnumHandling.Lowercase)]
		public MessageType Type { get; set; }

		[JsonProperty(PropertyName = "message")]
		public string Message { get; set; }

		[JsonProperty(PropertyName = "key")]
		public int Key { get; set; }
		[JsonProperty(PropertyName = "Value")]
		public int Value { get; set; }
	}

	[Reply("NLN")]
	public sealed class Server_NLN_CharacterOnline : Command, Command.ICharacterCommand
	{
		[JsonIgnore]
		public string CharacterName { get { return Character ?? Identity; } }

		[JsonProperty(PropertyName = "character")]
		public string Character { get; set; }
		[JsonProperty(PropertyName = "identity")]
		public string Identity { get; set; }

		[JsonProperty(PropertyName = "gender")]
		[JsonConverter(typeof(JsonEnumConverter), JsonEnumConverter.EnumHandling.Default)]
		public CharacterGender Gender { get; set; }

		[JsonProperty(PropertyName = "status")]
		[JsonConverter(typeof(JsonEnumConverter), JsonEnumConverter.EnumHandling.Default)]
		public CharacterStatus Status { get; set; }
	}

	[Reply("PRI")]
	public sealed class Server_PRI_CharacterChatMessage : Command, Command.ICharacterCommand
	{
		[JsonProperty(PropertyName = "character", Required = Required.Always)]
		public string Character { get; set; }
		[JsonProperty(PropertyName = "message")]
		public string Message { get; set; }
	}

	[Reply("PRD")]
	public sealed class Server_PRD_CharacterGetTags : Command, Command.ICharacterCommand
	{
		public enum MessageType
		{
			Start,
			End
		}
		
		[JsonProperty(PropertyName = "character", Required = Required.Always)]
		public string Character { get; set; }


		[JsonProperty(PropertyName = "type")]
        [JsonConverter(typeof(JsonEnumConverter), JsonEnumConverter.EnumHandling.Lowercase)]
		public MessageType Type { get; set; }

		[JsonProperty(PropertyName = "key")]
		public string Key { get; set; }

		[JsonProperty(PropertyName = "value")]
		public string Value { get; set; }
	}

	[Reply("STA")]
	public sealed class Server_STA_CharacterStatus : Command, Command.ICharacterCommand
	{
		[JsonProperty(PropertyName = "character", Required = Required.Always)]
		public string Character { get; set; }

		[JsonProperty(PropertyName = "status")]
		[JsonConverter(typeof(JsonEnumConverter), JsonEnumConverter.EnumHandling.Lowercase)]
		public CharacterStatus Status { get; set; }

		[JsonProperty(PropertyName = "statusmessage")]
		public string Message { get; set; }
	}

	[Reply("TPN")]
	public sealed class Server_TPN_CharacterTypingStatus : Command, Command.ICharacterCommand
	{
		[JsonProperty(PropertyName = "character", Required = Required.Always)]
		public string Character { get; set; }

		[JsonProperty(PropertyName = "status")]
		[JsonConverter(typeof(JsonEnumConverter), JsonEnumConverter.EnumHandling.Lowercase)]
		public TypingStatus Status { get; set; }
	}
}