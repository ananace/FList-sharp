using System;

namespace ConsoleMessenger.Settings
{
	[AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
	class SettingAttribute : Attribute
	{
		public string Name { get; private set; }
		public string Description { get; set; }

		public object DefaultValue { get; set; }

		public SettingAttribute(string Name)
		{
			this.Name = Name;
		}
	}
}
