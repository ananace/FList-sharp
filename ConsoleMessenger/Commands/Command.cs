using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;

namespace ConsoleMessenger
{
	public abstract class Command
	{
		public string Name { get { return GetType().GetCustomAttribute<CommandAttribute>().Name; } }
		
		public virtual bool TabComplete(string input, out string[] possibilities)
		{
			possibilities = new string[0];
			return false;
		}

		public void Invoke(IEnumerable<string> args)
		{
			var method = GetType()
				.GetMethods()
				.Where(m => m.Name == "Call" &&
					m.GetParameters().Length == args.Count())
				.FirstOrDefault();

			if (method == null)
				throw new ArgumentException(string.Format("No function with given arguments found, possible functions are:\n- /{0}",
					GetType().GetMethods().Where(m => m.Name == "Call").Select(m =>
					{
						return string.Format("{0} {1}", Name, m.GetParameters().Select(p => "<" + p.Name + ">").ToString(" "));
					}).ToString("\n- /")));

			method.Invoke(this, args.Zip(method.GetParameters(), (arg, type) => new { type, arg }).Select(a =>
			{
				var conv = TypeDescriptor.GetConverter(a.type.ParameterType);
				return conv.ConvertFromString(a.arg);
			}).ToArray());
		}

		static Dictionary<string, Type> _CommandTypes;
		public static IEnumerable<string> Available { get { return _CommandTypes.Select(t => t.Key); } }

		static Command()
		{
			_CommandTypes = new Dictionary<string, Type>();

			foreach (var cmd in Assembly.GetExecutingAssembly().GetTypes().Where(t => t.BaseType == typeof(Command)))
			{
				var att = cmd.GetCustomAttribute<CommandAttribute>();
				if (att != null)
					_CommandTypes[att.Name.ToLower()] = cmd;
			}
		}

		public static Command Create(string name)
		{
			if (_CommandTypes.ContainsKey(name.ToLower()))
				return Assembly.GetExecutingAssembly().CreateInstance(_CommandTypes[name.ToLower()].FullName) as Command;

			return null;
		}
	}

	public class CommandAttribute : Attribute
	{
		public string Name { get; private set; }
		public string Description { get; set; }

		public CommandAttribute(string name)
		{
			Name = name;
		}
	}
}

