using System;
using System.Threading;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using WhiteSparrow.Shared.Queue.Items;

namespace WhiteSparrow.Shared.Queue
{
	public class ExecutionQueue : AbstractExecutionQueue<IQueueItem>
	{
		public IQueueItem Add(Func<Task> item)
		{
			var wrapper = new QueueAsyncTaskItem(item);
			base.Add(wrapper);
			return wrapper;
		}
		
		public IQueueItem Add(Func<UniTask> item)
		{
			var wrapper = new UniTaskQueueItem(item);
			base.Add(wrapper);
			return wrapper;
		}

		public IQueueItem Add(Func<CancellationToken, UniTask> item)
		{
			var wrapper = new UniTaskQueueItem(item);
			base.Add(wrapper);
			return wrapper;
		}
	}
}