using Newtonsoft.Json;
using libflist.Util.Converters;

namespace libflist.FChat.Commands.Client.Global
{
	[Command("ACB", MinRight = UserRight.ChatOP)]
	public class BanCharacterCommand : Command
	{
		[JsonProperty(PropertyName = "character", Required = Required.DisallowNull)]
		public string Character { get; set; }
	}

	[Command("AOP", MinRight = UserRight.Admin)]
	public class GiveOPCommand : Command
	{
		[JsonProperty(PropertyName = "character", Required = Required.DisallowNull)]
		public string Character { get; set; }
	}

	[Command("AWC", MinRight = UserRight.ChatOP)]
	public class GetAltsCommand : Command
	{
		[JsonProperty(PropertyName = "character", Required = Required.DisallowNull)]
		public string Character { get; set; }
	}

	[Command("BRO", MinRight = UserRight.Admin)]
	public class BroadcastMessageCommand : Command
	{
		[JsonProperty(PropertyName = "message", Required = Required.DisallowNull)]
		public string Message { get; set; }
	}

	[Command("CHA")]
	public class GetPublicChannelsCommand : Command
	{

	}

	[Command("DOP", MinRight = UserRight.Admin)]
	public class TakeOPCommand : Command
	{
		[JsonProperty(PropertyName = "character", Required = Required.DisallowNull)]
		public string Character { get; set; }
	}

	[Command("FKS")]
	public class FindCharactersCommand : Command
	{
		[JsonProperty(PropertyName = "kinks", Required = Required.DisallowNull)]
		public int[] Kinks { get; set; }
		[JsonProperty(PropertyName = "genders", Required = Required.AllowNull,
		              ItemConverterType = typeof(JsonEnumConverter))]
		public CharacterGender[] Genders { get; set; }
		[JsonProperty(PropertyName = "orientations", Required = Required.AllowNull,
		              ItemConverterType = typeof(JsonEnumConverter))]
		public CharacterOrientation[] Orientations { get; set; }
		[JsonProperty(PropertyName = "languages", Required = Required.AllowNull,
		              ItemConverterType = typeof(JsonEnumConverter))]
		public CharacterLanguage[] Languages { get; set; }
		[JsonProperty(PropertyName = "preferences", Required = Required.AllowNull,
		              ItemConverterType = typeof(JsonEnumConverter))]
		public CharacterPreference[] Preferences { get; set; }
		[JsonProperty(PropertyName = "roles", Required = Required.AllowNull,
		              ItemConverterType = typeof(JsonEnumConverter))]
		public CharacterRole[] Roles { get; set; }
	}

	[Command("IGN")]
	public class IgnoreCommand : Command
	{
		public enum IgnoreAction
		{
			Add,
			Delete,
			List,
			Notify
		}

		[JsonProperty(PropertyName = "method",
		              Required = Required.Always,
		              ItemConverterType = typeof(JsonEnumConverter),
		              ItemConverterParameters = new object[]{ JsonEnumConverter.EnumHandling.Lowercase })]
		public IgnoreAction Action { get; set; }

		[JsonProperty(PropertyName = "character",
		              Required = Required.AllowNull,
		              DefaultValueHandling = DefaultValueHandling.Ignore)]
		public string Character { get; set; }
	}

	[Command("KIK", MinRight = UserRight.Admin)]
	public class KickCharacterCommand : Command
	{
		[JsonProperty(PropertyName = "character", Required = Required.DisallowNull)]
		public string Character { get; set; }
	}

	[Command("ORS")]
	public class GetPrivateChannelsCommand : Command
	{

	}

	[Command("RWD", MinRight = UserRight.Admin)]
	public class RewardCommand : Command
	{
		[JsonProperty(PropertyName = "character", Required = Required.DisallowNull)]
		public string Character { get; set; }
	}

	[Command("SFC", MinRight = UserRight.Admin)]
	public class ReportCharacterCommand : Command
	{
		[JsonProperty(PropertyName = "action")]
		public string Action { get; set; } = "report";

		[JsonProperty(PropertyName = "character", Required = Required.DisallowNull)]
		public string Character { get; set; }

		[JsonProperty(PropertyName = "report", Required = Required.DisallowNull)]
		public string Message { get; set; }
	}

	[Command("TMO", MinRight = UserRight.Admin)]
	public class TimeoutCharacterCommand : Command
	{
		[JsonProperty(PropertyName = "character", Required = Required.DisallowNull)]
		public string Character { get; set; }

		[JsonProperty(PropertyName = "time")]
		public int Minutes { get; set; }

		[JsonProperty(PropertyName = "reason", Required = Required.AllowNull)]
		public string Reason { get; set; }
	}

	[Command("UBN", MinRight = UserRight.ChatOP)]
	public class UnbanCharacterCommand : Command
	{
		[JsonProperty(PropertyName = "character", Required = Required.DisallowNull)]
		public string Character { get; set; }
	}
}

