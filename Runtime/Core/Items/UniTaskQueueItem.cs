#if EXECUTIONQUEUE_UNITASK
using System;
using Cysharp.Threading.Tasks;

namespace WhiteSparrow.Shared.Queue.Items
{
	public class UniTaskQueueItem : AbstractQueueItem
	{
		protected override bool forceMainThread => true;

		private Func<UniTask> m_ExecuteCall;
		private UniTask? m_UniTask;

		public UniTaskQueueItem(Func<UniTask> call)
		{
			m_ExecuteCall = call;
		}
		

		public UniTaskQueueItem(UniTask call)
		{
			m_UniTask = call;
			
		}
		protected override async void Execute()
		{
			if (m_ExecuteCall != null)
				m_UniTask = UniTask.Create(m_ExecuteCall);

			if (m_UniTask is null)
			{
				End();
				return;
			}
			
			await m_UniTask.Value;
			
			End();
		}

		public static implicit operator UniTaskQueueItem(Func<UniTask> call)
		{
			return new UniTaskQueueItem(call);
		}
		public static implicit operator UniTaskQueueItem(UniTask task)
		{
			return new UniTaskQueueItem(task);
		}

		protected override void OnDispose()
		{
			base.OnDispose();

			m_ExecuteCall = null;
			m_UniTask = null;
		}
	}
}
#endif
