using UnityEngine;

namespace WhiteSparrow.Shared.Queue.Items
{
	public abstract class AbstractQueueItem : IQueueItem
	{
		internal QueueResult m_Result = QueueResult.None;
		public QueueResult Result => m_Result;

		internal QueueState m_State = QueueState.None;
		public virtual QueueState State => m_State;

		public object UserData { get; set; }

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

		public virtual bool ForceCompleteOnMainThread { get; set; } = false;
        
		protected void End()
		{
			End(QueueResult.Success);
		}

		protected void End(QueueResult result)
		{
			if (m_PendingResult != QueueResult.None)
				return;
            
			if (ForceCompleteOnMainThread)
				EndOnMainThread(result);
			else
				EndContinuation();
		}

		protected void EndOnMainThread() => EndOnMainThread(QueueResult.Success);
		protected void EndOnMainThread(QueueResult result)
		{
			if (m_PendingResult != QueueResult.None)
				return;
			m_PendingResult = result;
			ExecutionQueueThreadUtility.CallOnMainThread += EndContinuation;
		}

		private QueueResult m_PendingResult;
		private void EndContinuation()
		{
			m_State = QueueState.Completed;
            m_Result = m_PendingResult;
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