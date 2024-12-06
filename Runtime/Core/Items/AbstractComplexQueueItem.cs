using System;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace WhiteSparrow.Shared.Queue.Items
{
	public abstract class AbstractComplexQueueItem<T> : AbstractQueueItem
		where T : struct, IConvertible
	{
		private T m_Operation;

		protected void Start(T operation)
		{
			SetOperation(operation);
			Start();
		}
		
		public void SetOperation(T operation)
		{
			if (m_Operation.Equals(operation))
				return;
			
			if (State == QueueState.Running || State == QueueState.Stopping)
			{
				Debug.LogError("You cannot change the state of a Queue Item while it's running. Stop and wait for completion.");
				return;
			}

			m_Operation = operation;
			m_State = QueueState.None;
			m_Result = QueueResult.None;
		}

		protected sealed override UniTask Execute() => ExecuteOperation(m_Operation);

		protected abstract UniTask ExecuteOperation(T operation);
	}
}