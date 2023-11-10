using UnityEngine;

namespace WhiteSparrow.Shared.Queue.Items
{
	public abstract class AbstractQueueItem : IQueueItem
	{
		internal QueueResult m_Result = QueueResult.None;
		public QueueResult Result => m_Result;

		internal QueueState m_State = QueueState.None;
		public virtual QueueState State => m_State;


		public virtual bool IsRunning => State == QueueState.Running || State == QueueState.Stopping;
		public virtual bool IsDone => State == QueueState.Completed || State == QueueState.Stopped;

		private QueueItemDelegate m_OnComplete;
		public event QueueItemDelegate OnComplete
		{
			add
			{
				if (IsDone)
					value(this);
				else
					m_OnComplete += value;
			}
			remove => m_OnComplete -= value;
		}
		
		public void Start()
		{
			if (State != QueueState.None)
			{
				Debug.Log("Already started");
				return;
			}

			m_State = QueueState.Running;
			Execute();
		}

		protected abstract void Execute();

		protected void End()
		{
			End(QueueResult.Success);
		}

		protected void End(QueueResult result)
		{
			if (m_Result != QueueResult.None)
			{
				// TODO Already completed
				return;
			}

			if (m_State == QueueState.Stopping)
			{
				m_State = QueueState.Stopped;
				m_Result = result == QueueResult.Success ? QueueResult.Stop : result;
			}
			else
			{
				m_State = QueueState.Completed;
				m_Result = result;
			}

			InvokeOnComplete();
		}

		protected virtual void InvokeOnComplete()
		{
			m_OnComplete?.Invoke(this);
			m_OnComplete = null;
		}

		public void Stop()
		{
			if (m_State != QueueState.Running)
				return;

			m_State = QueueState.Stopping;

			ExecuteStop();
		}

		protected virtual void ExecuteStop()
		{
			Debug.Log("Stop requested");
		}
	}
}