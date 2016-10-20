using ConsoleMessenger.Settings;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleMessenger.Commands
{
	[Command("set", Description = "Reads or sets the value of a configurable setting.")]
	class Set : Command
	{
		class PropertyKV
		{
			public object Class;
			public PropertyInfo Property;
		}

		public void Call()
		{
			var settings = typeof(Application).GetProperties(BindingFlags.Static | BindingFlags.Public).Where(p => p.GetCustomAttribute<SettingAttribute>() != null).Select(p => new PropertyKV{ Property = p, Class = null });

			if (Application.CurrentChannelBuffer != null)
				settings = settings.Concat(Application.CurrentChannelBuffer.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance).Where(p => p.GetCustomAttribute<SettingAttribute>() != null).Select( p=> new PropertyKV{ Class = Application.CurrentChannelBuffer, Property = p }));

			Debug.WriteLine("Available settings:");
			foreach (var prop in settings)
			{
				var set = prop.Property.GetCustomAttribute<SettingAttribute>();
				Debug.WriteLine($"- {set.Name} = {prop.Property.GetMethod.Invoke(prop.Class, null)} - {set.Description}");
			}	
		}

		public void Call(string name)
		{
			// TODO: Get the value of the given setting
		}

		public void Call(string name, params string[] values)
		{
			// TODO: Set a setting
		}
	}
}
