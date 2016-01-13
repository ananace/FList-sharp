using ConsoleMessenger.Types;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace ConsoleMessenger.UI.FChat
{
	public class ChannelBuffer : Control
	{
		class DebugTracer : TraceListener
		{
			ChannelBuffer _Buf;
			public DebugTracer(ChannelBuffer buf)
			{
				_Buf = buf;
			}

			string partial;

			public override void Write(string message)
			{
				if (partial == null)
					partial = message;
				else
					partial += message;

				if (partial.Contains("\n"))
				{
					_Buf.PushMessage(partial.Substring(0, partial.IndexOf('\n')));
					partial = partial.Remove(0, partial.IndexOf('\n') + 1);
				}
			}

			public override void WriteLine(string message)
			{
				_Buf.PushMessage(message);
			}
		}

		public class Rendered
		{
			string _Message;
			public string Message
			{
				get { return _Message; }
				set
				{
					_Message = value;

					var data = Message.Split('\n');
					var height = data.Length;
					Size = new Size(data.OrderByDescending(s =>
					{
						var len = s.ANSILength();
						if (len > Size.Width && Size.Width > 0)
						{
							height += len / Size.Width;
							return len % Size.Width;
						}
						return len;
					}).First().ANSILength(), height);
				}
			}
			public DateTime Timestamp { get; set; }
			public Size Size { get; set; }
			public int WrapPoint { get; set; }
		}

		DebugTracer _Tracer;
		public TraceListener TraceListener { get { if (_Tracer == null) _Tracer = new DebugTracer(this); return _Tracer; } }
		List<Rendered> _RenderedMessages = new List<Rendered>();

		public override void InvalidateLayout()
		{
			// Size changed probably, recalculate message sizes.
			foreach (var msg in _RenderedMessages)
				msg.Message = msg.Message;

			base.InvalidateLayout();
		}

		public void PushMessage(Rendered msg)
		{
			_RenderedMessages.Add(msg);

			if (_RenderedMessages.Count > 1000)
				_RenderedMessages.RemoveAt(0);

			InvalidateVisual();
		}

		public void PushMessage(string msg)
		{
			// TODO: Better pre-rendering of messages; Colors, etc
			PushMessage(new Rendered
			{
				Message = msg,
				Timestamp = DateTime.Now
			});
		}

		public override void Render()
		{
			int totalHeight = 0;
			foreach (var msg in (_RenderedMessages as IEnumerable<Rendered>).Reverse().TakeWhile(c => (totalHeight += c.Size.Height) <= Size.Height).Reverse())
			{
				// TODO: If message wraps around, indent to the wrap point.
				Console.Write(msg.Timestamp.ToShortTimeString());
				Console.CursorLeft++;
				Graphics.WriteANSIString(msg.Message + "\n");
			}
		}
	}
}
