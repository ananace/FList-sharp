using Newtonsoft.Json;
using libflist.Util.Converters;
using libflist.Info;

namespace libflist.FChat.Commands
{
	[Command("ACB", MinRight = UserRight.ChatOP)]
	public sealed class Client_ACB_ChatBanCharacter : Command
	{
		[JsonProperty(PropertyName = "character", Required = Required.DisallowNull)]
		public string Character { get; set; }
	}

	[Command("AOP", MinRight = UserRight.Admin)]
	public sealed class Client_AOP_ChatAddOP : Command
	{
		[JsonProperty(PropertyName = "character", Required = Required.DisallowNull)]
		public string Character { get; set; }
	}

	[Command("AWC", MinRight = UserRight.ChatOP)]
	public sealed class Client_AWC_ChatListAlts : Command
	{
		[JsonProperty(PropertyName = "character", Required = Required.DisallowNull)]
		public string Character { get; set; }
	}

	[Command("BRO", MinRight = UserRight.Admin)]
	public sealed class Client_BRO_ChatBroadcastMessage : Command
	{
		[JsonProperty(PropertyName = "message", Required = Required.DisallowNull)]
		public string Message { get; set; }
	}

	[Command("CHA")]
	public sealed class Client_CHA_ChatListOfficialChannels : Command
	{

	}

	[Command("DOP", MinRight = UserRight.Admin)]
	public sealed class Client_DOP_ChatRemoveOP : Command
	{
		[JsonProperty(PropertyName = "character", Required = Required.DisallowNull)]
		public string Character { get; set; }
	}

	[Command("FKS")]
	public sealed class Client_FKS_ChatFindCharacters : Command
	{
		[JsonProperty(PropertyName = "kinks", Required = Required.DisallowNull)]
		public int[] Kinks { get; set; }
        [JsonProperty(PropertyName = "genders", Required = Required.AllowNull)]
        [JsonConverter(typeof(JsonEnumConverter), new object[] { JsonEnumConverter.EnumHandling.Lowercase })]
		public Genders[] Genders { get; set; }
        [JsonProperty(PropertyName = "orientations", Required = Required.AllowNull)]
        [JsonConverter(typeof(JsonEnumConverter), new object[] { JsonEnumConverter.EnumHandling.Lowercase })]
		public Orientations[] Orientations { get; set; }
        [JsonProperty(PropertyName = "languages", Required = Required.AllowNull)]
        [JsonConverter(typeof(JsonEnumConverter), new object[] { JsonEnumConverter.EnumHandling.Lowercase })]
		public LanguagePreferences[] Languages { get; set; }
        [JsonProperty(PropertyName = "preferences", Required = Required.AllowNull)]
        [JsonConverter(typeof(JsonEnumConverter), new object[] { JsonEnumConverter.EnumHandling.Lowercase })]
		public FurryPreferences[] Preferences { get; set; }
        [JsonProperty(PropertyName = "roles", Required = Required.AllowNull)]
        [JsonConverter(typeof(JsonEnumConverter), new object[] { JsonEnumConverter.EnumHandling.Lowercase })]
		public DomRoles[] Roles { get; set; }
	}

	[Command("IGN")]
	public sealed class Client_IGN_ChatIgnoreList : Command
	{
		public enum IgnoreAction
		{
			Add,
			Delete,
			List,
			Notify
		}

		[JsonProperty(PropertyName = "method", Required = Required.Always)]
        [JsonConverter(typeof(JsonEnumConverter), new object[] { JsonEnumConverter.EnumHandling.Lowercase })]
		public IgnoreAction Action { get; set; }

		[JsonProperty(PropertyName = "character",
		              Required = Required.AllowNull,
		              DefaultValueHandling = DefaultValueHandling.Ignore)]
		public string Character { get; set; }
	}

	[Command("KIK", MinRight = UserRight.Admin)]
	public sealed class Client_KIK_ChatKickCharacter : Command
	{
		[JsonProperty(PropertyName = "character", Required = Required.DisallowNull)]
		public string Character { get; set; }
	}

	[Command("ORS")]
	public sealed class Client_ORS_ChatListPrivateChannels : Command
	{

	}

	[Command("RWD", MinRight = UserRight.Admin)]
	public sealed class Client_RWD_ChatRewardCharacter : Command
	{
		[JsonProperty(PropertyName = "character", Required = Required.DisallowNull)]
		public string Character { get; set; }
	}

	[Command("SFC", MinRight = UserRight.Admin)]
	public sealed class Client_SFC_ChatReportCharacter : Command
	{
		[JsonProperty(PropertyName = "action")]
		public string Action { get; set; } = "report";

		[JsonProperty(PropertyName = "character", Required = Required.DisallowNull)]
		public string Character { get; set; }

		[JsonProperty(PropertyName = "report", Required = Required.DisallowNull)]
		public string Message { get; set; }
	}

	[Command("TMO", MinRight = UserRight.Admin)]
	public sealed class Client_TMO_ChatTimeoutCharacter : Command
	{
		[JsonProperty(PropertyName = "character", Required = Required.DisallowNull)]
		public string Character { get; set; }

		[JsonProperty(PropertyName = "time")]
		public int Minutes { get; set; }

		[JsonProperty(PropertyName = "reason", Required = Required.AllowNull)]
		public string Reason { get; set; }
	}

	[Command("UBN", MinRight = UserRight.ChatOP)]
	public sealed class Client_UBN_ChatUnbanCharacter : Command
	{
		[JsonProperty(PropertyName = "character", Required = Required.DisallowNull)]
		public string Character { get; set; }
	}
}

