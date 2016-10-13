using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;

namespace ConsoleMessenger
{
	public abstract class Command
	{
		public sealed class CompleteResult
		{
			public string Prefix { get; set; }
			public string TruePrefix { get; set; }
			public string[] Found { get; set; }
		}

		object _Caller;
		protected object Caller { get { return _Caller; } }
		public string Name { get { return GetType().GetCustomAttributes<CommandAttribute>().First().Name; } }
		public string CalledName { get; set; }

		public virtual bool TabComplete(string input, out string[] possibilities)
		{
			possibilities = null;
			return false;
		}

		static Array CastArray(Type type, IEnumerable<string> Inp)
		{
			var conv = TypeDescriptor.GetConverter(type);
			//var temp = Array.CreateInstance(type, Inp.Count());
			return Inp.Select(v => conv.ConvertFromString(v)).ToArray();
		}

		public void Invoke(IEnumerable<string> args, object sender = null)
		{
			var method = GetType()
				.GetMethods()
				.FirstOrDefault(m => m.Name == "Call"
					&& m.GetParameters().Length == args.Count()
					|| (m.GetParameters().Any()
						&& m.GetParameters().Last().ParameterType.IsArray));

			if (method == null)
				throw new ArgumentException(string.Format("No function with given arguments found, possible functions are:\n- /{0}",
					GetType().GetMethods().Where(m => m.Name == "Call").Select(m =>
					{
						return string.Format("{0} {1}", Name, m.GetParameters().Select(p => "<" + p.Name + ">").ToString(" "));
					}).ToString("\n- /")));

			ParameterInfo[] parameters = method.GetParameters();
			bool hasParams = false;
			if (parameters.Length > 0)
			{
				hasParams = parameters.Last().GetCustomAttribute<ParamArrayAttribute>() != null || parameters.Last().ParameterType.IsArray;
			}

			object[] input = args.ToArray();
			var realParams = new object[parameters.Length];
			
			int lastParamPosition = parameters.Length - 1;

			int i = 0;
			foreach (var arg in input.Zip(method.GetParameters(), (Arg, Type) => new { Arg, Type.ParameterType }))
			{
				if (hasParams && i >= lastParamPosition)
					break;

				var conv = TypeDescriptor.GetConverter(arg.ParameterType);
				realParams[i] = conv.ConvertFromString(input[i++] as string);
			}

			if (hasParams)
			{
				var arr = input;

				Type paramsType = parameters.Last().ParameterType.GetElementType();
				Array extra = Array.CreateInstance(paramsType, arr.Length - lastParamPosition);
				var conv = TypeDescriptor.GetConverter(paramsType);
				i = 0;
				foreach (var toCopy in arr.Skip(lastParamPosition))
					extra.SetValue(conv.ConvertFromString(toCopy as string), i++);

				realParams[lastParamPosition] = extra;
			}

			input = realParams;

			_Caller = sender;
			method.Invoke(this, input);
			_Caller = null;
		}

		static readonly Dictionary<string, Type> _CommandTypes = new Dictionary<string, Type>();
		public static IEnumerable<string> Names { get { return _CommandTypes.Keys; } }
		public static IEnumerable<KeyValuePair<string, CommandAttribute>> Available
		{
			get
			{
				foreach (var cmd in _CommandTypes)
				{
					yield return new KeyValuePair<string, CommandAttribute>(cmd.Key, cmd.Value
						.GetCustomAttributes<CommandAttribute>()
						.First());
				}
			}
		}

		public static CommandAttribute GetCommand(string name)
		{
			if (!_CommandTypes.ContainsKey(name))
				return null;

			return _CommandTypes[name].GetCustomAttributes<CommandAttribute>().First(n => n.Name.Equals(name, StringComparison.OrdinalIgnoreCase));
		}

		static Command()
		{
			foreach (var cmd in Assembly.GetExecutingAssembly().GetTypes().Where(t => t.BaseType == typeof(Command)))
			{
				foreach (var att in cmd.GetCustomAttributes().Where(a => a.GetType() == typeof(CommandAttribute)).Select(a => a as CommandAttribute))
				{
					var wrong = cmd.GetMethods()
						.Where(m => m.Name == "Call")
						.FirstOrDefault(m => m.GetParameters()
							.Count(p => p.ParameterType.IsArray) > 1 
								|| (m.GetParameters()
									.Count(p => p.ParameterType.IsArray) == 1
								&& !m.GetParameters()
									.Last().ParameterType.IsArray));

					if (wrong != null)
						throw new Exception(string.Format("Command {0} ({1}) has invalid call function; {2}", att.Name, cmd.FullName,
							string.Format("{0}({1})", wrong.Name, wrong.GetParameters().Select(p => p.ParameterType + " " + p.Name))));

					_CommandTypes[att.Name.ToLower()] = cmd;
				}
			}
		}

		public static Command Create(string name)
		{
			var cmd = _CommandTypes.ContainsKey(name.ToLower()) 
				? Assembly.GetExecutingAssembly().CreateInstance(_CommandTypes[name.ToLower()].FullName) as Command 
				: null;

			if (cmd != null)
				cmd.CalledName = name;

			return cmd;
		}
	}

	[AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
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

