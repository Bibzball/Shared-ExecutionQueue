using WhiteSparrow.Shared.Queue.Items;

namespace Plugins.WhiteSparrow.Queue.LoadQueue
{
	public abstract class AbstractLoadQueueItem : AbstractComplexQueueItem<LoadQueueOperation>, ILoadQueueItem
	{
		protected override void ExecuteOperation(LoadQueueOperation operation)
		{
			switch (operation)
			{
				case LoadQueueOperation.Load:
					ExecuteLoad();
					break;
				case LoadQueueOperation.Unload:
					ExecuteUnload();
					break;
			}
		}
		
		public void Load()
		{
			SetOperation(LoadQueueOperation.Load);
			base.Start();
		}

		public void Unload()
		{
			SetOperation(LoadQueueOperation.Unload);
			Start();
		}

		protected abstract void ExecuteLoad();
		
		protected abstract void ExecuteUnload();

		private LoadQueueItemDelegate m_OnComplete;
		public new event LoadQueueItemDelegate OnComplete
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

		protected override void InvokeOnComplete()
		{
			m_OnComplete?.Invoke(this);
			m_OnComplete = null;
			base.InvokeOnComplete();
		}
	}

}