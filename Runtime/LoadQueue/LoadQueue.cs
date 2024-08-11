using WhiteSparrow.Shared.Queue;
using WhiteSparrow.Shared.Queue.Items;

namespace Plugins.WhiteSparrow.Queue.LoadQueue
{
	public class LoadQueue : AbstractComplexExecutionQueue<ILoadQueueItem, LoadQueueOperation>, ILoadQueueItem
	{
		public LoadQueue()
		{
			SetOperation(LoadQueueOperation.Load);
		}
		
		public ILoadQueueItem Load()
		{
			base.Start(LoadQueueOperation.Load);
			return this;
		}

		public ILoadQueueItem Unload()
		{
			base.Start(LoadQueueOperation.Unload);
			return this;
		}

	}
}