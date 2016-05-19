using System;
using System.Reflection;
using Newtonsoft.Json;
using libflist.Connection.Types;

namespace libflist.Connection.Commands.Client.Connection
{
	[Command("IDN", MinRight = UserRight.Disconnected)]
	public class IdentifyCommand : Command
	{
		[JsonProperty(PropertyName = "method")]
		public string Method { get; set; } = "ticket";

		[JsonProperty(PropertyName = "account")]
		public string Account { get; set; }
		[JsonProperty(PropertyName = "ticket")]
		public string Ticket { get; set; }
		[JsonProperty(PropertyName = "character")]
		public string Character { get; set; }

		[JsonProperty(PropertyName = "cname")]
		public string ClientName { get; set; }
		[JsonProperty(PropertyName = "cversion")]
		public string ClientVersion { get; set; }

		public IdentifyCommand()
		{
			var app = Assembly.GetEntryAssembly();
			var appname = app.GetName();

			var title = Attribute.GetCustomAttribute(app, typeof(AssemblyTitleAttribute), false) as AssemblyTitleAttribute;

			ClientName = title.Title;
			ClientVersion = string.Format("v{0}", appname.Version);
		}
	}
}