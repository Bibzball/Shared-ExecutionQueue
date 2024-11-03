#if EXECUTIONQUEUE_UNITASK
using System;
using System.Threading;
using Cysharp.Threading.Tasks;

namespace WhiteSparrow.Shared.Queue.Items
{
	public class UniTaskQueueItem : AbstractQueueItem
	{
		private Func<UniTask> m_ExecuteCall;
		private Func<CancellationToken, UniTask> m_ExecuteCallCancellation;
		private UniTask? m_UniTask;

		public UniTaskQueueItem(UniTask call)
		{
			m_UniTask = call;
		}
		
		public UniTaskQueueItem(Func<UniTask> call)
		{
			m_ExecuteCall = call;
		}

		public UniTaskQueueItem(Func<CancellationToken, UniTask> call)
		{
			m_ExecuteCallCancellation = call;
		}

	

		protected override async UniTask Execute()
		{
			if (m_UniTask is not null)
			{
				await m_UniTask.Value;
				return;
			}
			
			if (m_ExecuteCall != null)
				m_UniTask = UniTask.Create(m_ExecuteCall).AttachExternalCancellation(CancellationToken);
			else if (m_ExecuteCallCancellation != null)
				m_UniTask = UniTask.Create(m_ExecuteCallCancellation, CancellationToken).AttachExternalCancellation(CancellationToken);
			
			if (m_UniTask is null)
				return;
			
			await m_UniTask.Value;
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
