using Cysharp.Threading.Tasks;
using WhiteSparrow.Shared.Queue.Items;

namespace Plugins.WhiteSparrow.Queue.LoadQueue
{
	public abstract class AbstractLoadQueueItem : AbstractComplexQueueItem<LoadQueueOperation>, ILoadQueueItem
	{
		
		
		public ILoadQueueItem Load()
		{
			SetOperation(LoadQueueOperation.Load);
			Start();
			return this;
		}

		public ILoadQueueItem Unload()
		{
			SetOperation(LoadQueueOperation.Unload);
			Start();
			return this;
		}
		
		protected override UniTask ExecuteOperation(LoadQueueOperation operation)
		{
			switch (operation)
			{
				case LoadQueueOperation.Load:
					return ExecuteLoad();
				case LoadQueueOperation.Unload:
					return ExecuteUnload();
			}
			return UniTask.CompletedTask;
		}

		protected abstract UniTask ExecuteLoad();
		
		protected abstract UniTask ExecuteUnload();

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