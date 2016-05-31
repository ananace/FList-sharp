using libflist.Util;

namespace libflist.Info
{

	public enum KinkChoice
	{
		Fave,
		Yes,
		Maybe,
		No
	}

	public enum KinkGroup
	{
		[EnumValue("Anal Sex")]
		AnalSex,
		[EnumValue("BDSM &amp; Related")]
		BDSMRelated,
		[EnumValue("Blood &amp; Gore / Torture / Death")]
		GoreRelated,
		[EnumValue("Body preferences")]
		BodyPreferences,
		[EnumValue("Cum-related")]
		CumRelated,
		[EnumValue("Genders")]
		GenderPreferences,
		General,
		Hardcore,
		[EnumValue("Inflation, growth, shrinking")]
		InflationRelated,
		Kinky,
		[EnumValue("Oral Sex")]
		OralSex,
		[EnumValue("Roleplay Specifics")]
		RoleplaySpecifics,
		Species,
		[EnumValue("Themes and Scenery")]
		ThemeRelated,
		[EnumValue("Vaginal / Straight")]
		VaginalSex,
		[EnumValue("Vore / Unbirth")]
		VoreRelated,
		[EnumValue("Watersports / Scat")]
		ToiletRelated,

		Custom
	}

	public sealed class KinkInfo
	{
		public string Name { get; set; }
		public string Description { get; set; }
		public KinkGroup Group { get; set; }
	}

}
