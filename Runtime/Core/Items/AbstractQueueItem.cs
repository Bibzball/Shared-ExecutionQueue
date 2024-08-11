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
		public bool ForceMainThread { get; set; } = false;
		protected virtual bool forceMainThread => false;
		protected bool _useMainThread => ForceMainThread || forceMainThread;
		
		public bool ForceCompleteOnMainThread { get; set; } = false;
		protected virtual bool forceCompleteOnMainThread => false;
		protected bool _useCompleteOnMainThread => _useMainThread || ForceCompleteOnMainThread || forceCompleteOnMainThread;


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
		
		public IQueueItem Start()
		{
			if (State != QueueState.None)
			{
				Debug.Log($"QueueItem {this.GetType().FullName} already started");
				return this;
			}

			m_State = QueueState.Running;

			if (_useMainThread)
				ExecutionQueueThreadUtility.ExecuteOnMainThread(Execute);
			else
				Execute();
			
			return this;
		}

		protected abstract void Execute();

        
		protected void End()
		{
			End(QueueResult.Success);
		}

		protected void End(QueueResult result)
		{
			if (m_PendingResult != QueueResult.None)
				return;
            
			if (_useCompleteOnMainThread)
				EndOnMainThread(result);
			else
			{
				m_PendingResult = result;
				EndContinuation();
			}
		}

		protected void EndOnMainThread() => EndOnMainThread(QueueResult.Success);
		protected void EndOnMainThread(QueueResult result)
		{
			if (m_PendingResult != QueueResult.None)
				return;
			m_PendingResult = result;
			ExecutionQueueThreadUtility.ExecuteOnMainThread(EndContinuation);
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

		~AbstractQueueItem()
		{
			Dispose();
		}
		
		private bool m_Dispose;
		public void Dispose()
		{
			if (m_Dispose)
				return;
			m_Dispose = true;
			OnDispose();
			
			UserData = null;
			m_OnComplete = null;
		}

		protected virtual void OnDispose()
		{
			
		}
	}
}