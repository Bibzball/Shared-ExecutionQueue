using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace WhiteSparrow.Shared.Queue.Items
{
	public abstract class AbstractQueueItem : IQueueItem
	{
		private CancellationTokenSource m_CancellationTokenSource;
		protected CancellationToken CancellationToken => m_CancellationTokenSource.Token;
		
		
		internal QueueResult m_Result = QueueResult.None;
		public QueueResult Result => m_Result;

		internal QueueState m_State = QueueState.None;
		public virtual QueueState State => m_State;

		public object UserData { get; set; }

		public virtual bool IsRunning => State == QueueState.Running || State == QueueState.Stopping;
		public virtual bool IsDone => State == QueueState.Completed || State == QueueState.Stopped;
		
		protected UniTask m_InternalTask;

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

		public AbstractQueueItem()
		{
			m_CancellationTokenSource = new CancellationTokenSource();
		}
		
		public IQueueItem Start()
		{
			if (State != QueueState.None)
			{
				Debug.Log($"QueueItem {this.GetType().FullName} already started");
				return this;
			}

			m_State = QueueState.Running;

			// we want to reset the cancellation token in case it was already triggered
			if (m_CancellationTokenSource.IsCancellationRequested)
			{
				m_CancellationTokenSource.Dispose();
				m_CancellationTokenSource = new CancellationTokenSource();
			}

			m_InternalTask = Execute().ContinueWith(OnCompletion).AttachExternalCancellation(m_CancellationTokenSource.Token);
			
			return this;
		}

		private void OnCompletion()
		{
			m_State = QueueState.Completed;
			SetResult(QueueResult.Success);
			InvokeOnComplete();
		}


		protected abstract UniTask Execute();

        
		protected void SetResult(QueueResult result)
		{
			if (m_Result == QueueResult.Fail)
				return;
			if (m_Result == QueueResult.Success && result == QueueResult.Stop)
				return;
			m_Result = result;
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
			
			if(!m_CancellationTokenSource.IsCancellationRequested)
				m_CancellationTokenSource.Cancel();
			m_CancellationTokenSource = new CancellationTokenSource();
			
			UniTask.Create(ExecuteStop).AttachExternalCancellation(m_CancellationTokenSource.Token).ContinueWith(StopContinuation);
		}

		private void StopContinuation()
		{
			m_State = QueueState.Stopped;
			InvokeOnComplete();
		}

		protected virtual UniTask ExecuteStop() => UniTask.CompletedTask;

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
			
			m_CancellationTokenSource.Cancel();
			m_CancellationTokenSource.Dispose();
			UserData = null;
			m_OnComplete = null;
		}

		protected virtual void OnDispose()
		{
			
		}

		public IQueueItem WithCancellation(CancellationToken token)
		{
			token.Register(OnExternalCancellationInvoked);
			return this;
		}

		private void OnExternalCancellationInvoked()
		{
			Stop();
		}
	}
}