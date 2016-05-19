using System;
using libflist.Connection.Types;
using libflist.Util.Converters;
using Newtonsoft.Json;

namespace libflist.Connection.Commands.Server.Character
{
	[Reply("FLN")]
	public class OfflineReply : Command, Command.ICharacterCommand
	{
		[JsonProperty(PropertyName = "character")]
		public string Character { get; set; }
	}

	[Reply("KID")]
	public class GetKinksReply : Command
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
	public class OnlineReply : Command, Command.ICharacterCommand
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
	public class SendMessageReply : Command, Command.ICharacterCommand
	{
		[JsonProperty(PropertyName = "character", Required = Required.Always)]
		public string Character { get; set; }
		[JsonProperty(PropertyName = "message")]
		public string Message { get; set; }
	}

	[Reply("PRD")]
	public class GetTagsReply : Command, Command.ICharacterCommand
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
	public class StatusReply : Command, Command.ICharacterCommand
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
	public class TypingReply : Command, Command.ICharacterCommand
	{
		[JsonProperty(PropertyName = "character", Required = Required.Always)]
		public string Character { get; set; }

		[JsonProperty(PropertyName = "status")]
		[JsonConverter(typeof(JsonEnumConverter), JsonEnumConverter.EnumHandling.Lowercase)]
		public TypingStatus Status { get; set; }
	}
}