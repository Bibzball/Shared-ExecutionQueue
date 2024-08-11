using System;
using System.Threading.Tasks;

namespace WhiteSparrow.Shared.Queue.Items
{
	public class QueueAsyncTaskItem : AbstractQueueItem
	{
		private Func<Task> m_executeCall;
		private Task m_executingTask;
		
		public QueueAsyncTaskItem(Func<Task> call)
		{
			m_executeCall = call;
		}
		
		protected override async void Execute()
		{
			
			if (m_executeCall != null)
				m_executingTask = Task.Run(m_executeCall);

			if (m_executingTask == null)
			{
				End();
				return;
			}
			
			await m_executingTask;
			End();
		}
		
		public static implicit operator QueueAsyncTaskItem(Func<Task> call)
		{
			return new QueueAsyncTaskItem(call);
		}

	}
}