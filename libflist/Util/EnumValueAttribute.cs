using System;

namespace libflist.Util
{
	public sealed class EnumValueAttribute : Attribute
	{
		public string Name { get; set; }

		public EnumValueAttribute(string Name)
		{
			this.Name = Name;
		}
	}
}

