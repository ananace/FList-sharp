using System;
namespace libflist.FChat.Events
{
	public class ErrorEventArgs : EventArgs
	{
		public Exception InnerException { get; set; }
		public string Message { get; set; }

		public ErrorEventArgs() { }
		public ErrorEventArgs(Exception Inner, string Message)
		{
			InnerException = Inner;
			this.Message = Message;
		}
	}
}

