using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace libflist.FChat.Util
{
	sealed class AsyncAutoResetEvent
	{
		readonly static Task _completedTask = Task.FromResult(true);
		readonly Queue<TaskCompletionSource<bool>> _waiting = new Queue<TaskCompletionSource<bool>>();

		bool _event;

		public AsyncAutoResetEvent(bool set = false)
		{
			_event = set;
		}

		public Task WaitAsync(CancellationToken cancellationToken = new CancellationToken())
		{
			lock (_waiting)
			{
				if (_event)
				{
					_event = false;
					return _completedTask;
				}
				else
				{
					var t = new TaskCompletionSource<bool>();
					_waiting.Enqueue(t);
					return t.Task;
				}
			}
		}
		
		public void Set()
		{
			TaskCompletionSource<bool> toRelease = null;
			lock (_waiting)
			{
				if (_waiting.Any())
					toRelease = _waiting.Dequeue();
				else if (!_event)
					_event = true;
			}

			toRelease?.SetResult(true);
		}
	}
}
