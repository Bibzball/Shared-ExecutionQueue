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
			if (m_executeCall == null)
			{
				End();
				return;
			}

			await Task.Run(m_executeCall);

			End();
		}

	}
}