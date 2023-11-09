using System;
using UnityEngine;
using WhiteSparrow.Shared.Queue.Items;

namespace WhiteSparrow.Shared.Queue
{
	public abstract class AbstractComplexExecutionQueue<T, TOperation> : AbstractExecutionQueue<T>, IComplexQueueItem<TOperation>
		where T : class, IQueueItem, IComplexQueueItem<TOperation>
		where TOperation : struct, IConvertible
	{
		private TOperation m_Operation;

		protected virtual void Start(TOperation operation)
		{
			SetOperation(operation);
			Start();
		}
		
		public void SetOperation(TOperation operation)
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

			ChangeItemsOperation();
		}

		private void ChangeItemsOperation()
		{
			var allItems = GetItems();
			foreach (var item in allItems)
			{
				item.SetOperation(m_Operation);
			}
		}
		
		
		protected override void StartItem(T item)
		{
			item.SetOperation(m_Operation);
			base.StartItem(item);
		}
	}
}