using WhiteSparrow.Shared.Queue.Items;

namespace Plugins.WhiteSparrow.Queue.LoadQueue
{
	public interface ILoadQueueItem : IQueueItem, IStopQueueItem, IComplexQueueItem<LoadQueueOperation>
	{
		void Load();
		void Unload();
	}
}