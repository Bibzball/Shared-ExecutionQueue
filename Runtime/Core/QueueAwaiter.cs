using System;
using System.Runtime.CompilerServices;
using WhiteSparrow.Shared.Queue.Items;

namespace WhiteSparrow.Shared.Queue
{
	public class QueueAwaiter : INotifyCompletion
	{
		private IQueueItem m_Item;
		
		public QueueAwaiter(IQueueItem item)
		{
			if (item == null)
				return;
			
			m_Item = item;
			if (!m_Item.IsDone)
				m_Item.OnComplete += OnItemComplete;
		}

		protected Action m_continuation;

		public bool IsCompleted => m_Item?.IsDone ?? true;

		private void OnItemComplete(IQueueItem item)
		{
			item.OnComplete -= OnItemComplete;
			RunContinuation();
		}
		
		public void OnCompleted(Action continuation)
		{
			if (m_Item == null || m_Item.IsDone)
			{
				continuation?.Invoke();
				return;
			}
			m_continuation = continuation;
			if (m_Item.IsDone)
				RunContinuation();
		}

		public void GetResult()
		{
		}
		
		public void RunContinuation()
		{
			m_continuation();
		}
	}

	public static class QueueAwaitExtensions
	{
		public static QueueAwaiter GetAwaiter(this IQueueItem instance)
		{
			return new QueueAwaiter(instance);
		}
	}
	
}