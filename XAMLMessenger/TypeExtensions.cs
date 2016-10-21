using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace XAMLMessenger
{
	static class TypeExtensions
	{
		static List<MethodInfo> extensionTypes = new List<MethodInfo>();

		static TypeExtensions()
		{
			extensionTypes.AddRange(Assembly.GetExecutingAssembly().GetTypes()
				.Where(t => t.IsClass && !t.IsGenericType && !t.IsNested)
				.SelectMany(t => t.GetMethods(BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic))
				.Where(m => m.IsDefined(typeof(ExtensionAttribute), false)));
		}

		public static IEnumerable<MethodInfo> GetExtensionMethods(this Type _t)
		{
			return extensionTypes
				.Where(m => m.GetParameters()[0].ParameterType == _t);
		}

		public static MethodInfo GetExtensionMethod(this Type _t, string MethodName)
		{
			var em = _t.GetExtensionMethods();
			return em.FirstOrDefault(m => m.Name.Equals(MethodName, StringComparison.CurrentCulture));
		}
	}
}
