using UnityEngine;
using UnityEngine.AddressableAssets;
using WhiteSparrow.Shared.Queue.Items;

namespace Plugins.WhiteSparrow.Queue.LoadQueue
{
	public interface ILoadQueueItem : IQueueItem, IStopQueueItem, IComplexQueueItem<LoadQueueOperation>
	{
		void Load();
		void Unload();
	}

	public interface ILoadAssetQueueItem : ILoadQueueItem
	{
		object Asset { get; }
	}

	public interface ILoadAddressableAssetQueueItem : ILoadAssetQueueItem
	{
		AssetReference AssetReference { get; }
	}

}