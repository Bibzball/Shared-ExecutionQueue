using System;
using System.Threading.Tasks;
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
	}
}