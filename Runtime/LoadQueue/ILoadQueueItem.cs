#if EXECUTIONQUEUE_ADRESSABLES
using UnityEngine.AddressableAssets;
#endif
using WhiteSparrow.Shared.Queue.Items;

namespace Plugins.WhiteSparrow.Queue.LoadQueue
{
	public interface ILoadQueueItem : IQueueItem, IComplexQueueItem<LoadQueueOperation>
	{
		ILoadQueueItem Load();
		ILoadQueueItem Unload();
	}

	public interface ILoadAssetQueueItem : ILoadQueueItem
	{
		object Asset { get; }
	}
	
#if EXECUTIONQUEUE_ADRESSABLES
	public interface ILoadAddressableAssetQueueItem : ILoadAssetQueueItem
	{
		AssetReference AssetReference { get; }
	}
#endif
}