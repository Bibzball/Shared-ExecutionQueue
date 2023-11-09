using WhiteSparrow.Shared.Queue;

namespace Plugins.WhiteSparrow.Queue.LoadQueue
{
	public class LoadQueue : AbstractComplexExecutionQueue<ILoadQueueItem, LoadQueueOperation>, ILoadQueueItem
	{
		public LoadQueue()
		{
			SetOperation(LoadQueueOperation.Load);
		}
		
		public void Load()
		{
			base.Start(LoadQueueOperation.Load);
		}

		public void Unload()
		{
			base.Start(LoadQueueOperation.Unload);
		}

	}
}