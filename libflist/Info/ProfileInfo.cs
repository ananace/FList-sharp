using System.Collections.Generic;

namespace libflist.Info
{

	public class ProfileInfo
	{
		public class GeneralDetail
		{
			public string Occupation { get; set; }
			// public Orientations? Orientation { get; set; }
			public string Species { get; set; }
			public string Height { get; set; }
			// public Builds? Build { get; set; }
			public string Age { get; set; }
			public string EyeColor { get; set; }
			public string Hair { get; set; }
			public string Weight { get; set; }
			public string Location { get; set; }
			// public BodyTypes? BodyType { get; set; }
			public string Personality { get; set; }
			public string Partner { get; set; }
			public string Pets { get; set; }
			public string Master { get; set; }
			// public Relationships? Relationship { get; set; }
			// public Genders? Gender { get; set; }
			// public BodyModifications? BodyModification { get; set; }
			public string SkinColor { get; set; }
			public string ApparentAge { get; set; }
		}
		public class RPingDetail
		{
			// public PostPerspectives? PostPerspective { get; set; }
			// public FurryPreferences? FurryPreference { get; set; }
			// public DesiredRPMethods? DesiredRPMethod { get; set; }
			public string CurrentlyLooking { get; set; }
			// public DesiredRPLengths? DesiredRPLength { get; set; }
			// public LanguagePreferences? LanguagePreference { get; set; }
			// public PostLengthPreferences? PostLengthPreference { get; set; }
			// public GrammarCompetences? GrammarCompetence { get; set; }
			// public GrammarCompetences? GrammarCompetencePreference { get; set; }
		}
		public class SexualDetail
		{
			public string Measurements { get; set; }
			public string NippleColor { get; set; }
			// public VulvaTypes? VulvaType { get; set; }
			// public CockShapes? CockShape { get; set; }
			// public Maybe? Uncut { get; set; }
			public string CockDiameter { get; set; }
			public string CockLength { get; set; }
			// public CockShapes? CockShape { get; set; }
			// public PublicHairs? PubicHair { get; set; }
			// public CumshotSizes? CumshotSize { get; set; }
			// public CockColors? CockColor { get; set; }
			public string KnotDiameter { get; set; }
			// public Sheaths? Sheath { get; set; }
			// public Maybe? Barbed { get; set; }
			public string BreastSize { get; set; }
			// public BallSizes? BallSize { get; set; }
			// public Maybe? Knotted { get; set; }
			// public Positions? Position { get; set; }
			// public DomRoles? DomRole { get; set; }
		}

		public Dictionary<string, string> ContactDetails { get; set; } = new Dictionary<string, string>();
		public GeneralDetail GeneralDetails { get; set; } = new GeneralDetail();
		public RPingDetail RPingDetails { get; set; } = new RPingDetail();
		public SexualDetail SexualDetails { get; set; } = new SexualDetail();
	}

}
