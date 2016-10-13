using System;
namespace libflist.FChat.Events
{
	public class ServerVariableEventArgs : EventArgs
	{
		public string Name { get; set; }
		public object Value { get; set; }

		public ServerVariableEventArgs() { }
		public ServerVariableEventArgs(string Name, object Value)
		{
			this.Name = Name;
			this.Value = Value;
		}
	}
}

