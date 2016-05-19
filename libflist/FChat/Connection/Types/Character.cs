using System;
using libflist.Util;

namespace libflist.Connection.Types
{
	public enum CharacterGender
	{
		Male,
		Female,
		Transgender,
		Herm,
		Shemale,
		[EnumValue("Male-Herm")]
		MaleHerm,
		[EnumValue("Cunt-boy")]
		Cuntboy,
		None
	}

	public enum CharacterOrientation
	{
		Straight,
		Gay,
		Bisexual,
		Asexual,
		Unsure,
		[EnumValue("Bi - male preference")]
		Bi_Male,
		[EnumValue("Bi - female preference")]
		Bi_Female,
		Pansexual,
		[EnumValue("Bi-curious")]
		Bi_Curious
	}

	public enum CharacterLanguage
	{
		Dutch,
		English,
		French,
		Spanish,
		German,
		Russian,
		Chinese,
		Japanese,
		Portugese,
		Korean,
		Arabic,
		Italian,
		Swedish,

		Other
	}

	public enum CharacterPreference
	{
		[EnumValue("No furry characters, just humans")]
		OnlyHumans,
		[EnumValue("No humans, just furry characters")]
		OnlyFurs,
		[EnumValue("Humans ok, Furries Preferred")]
		FursPreferred,
		[EnumValue("Furries ok, Humans Preferred")]
		HumansPreferred,
		[EnumValue("Furs and / or humans")]
		Both
	}

	public enum CharacterRole
	{
		[EnumValue("Always dominant")]
		Dom,
		[EnumValue("Usually dominant")]
		MostlyDom,
		Switch,
		[EnumValue("Usually submissive")]
		MostlySub,
		[EnumValue("Always submissive")]
		Sub,

		None
	}

	public enum CharacterStatus
	{
		Offline,
		Online,
		Looking,
		Busy,
		DND,
		Idle,
		Away,
		[EnumValue("crown")]
		Rewarded
	}

	public enum TypingStatus
	{
		Clear,
		Typing,
		Paused
	}
}

