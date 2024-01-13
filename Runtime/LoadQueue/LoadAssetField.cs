using System;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace Plugins.WhiteSparrow.Queue.LoadQueue
{
	[Serializable]
	public abstract class AbstractLoadAssetField
	{
		public enum LoadType
		{
			Resource = 1,
			Addressable = 2
		}

		[SerializeField]
		private LoadType m_LoadType;

		public virtual LoadType Type => m_LoadType;

		public string ResourcePath;
		
		public abstract ILoadAssetQueueItem GetLoadQueueItem();

		public abstract bool IsLoadTypeSupported(LoadType candidate);
	}

	[Serializable]
	public abstract class AbstractLoadAssetField<T> : AbstractLoadAssetField
		where T : UnityEngine.Object
	{
		protected abstract AssetReferenceT<T> TypedAssetReference { get; }

		public override ILoadAssetQueueItem GetLoadQueueItem()
		{
			switch (Type)
			{
				case LoadType.Resource:
					return new LoadResourceQueueItem<T>(ResourcePath);
				case LoadType.Addressable:
					return new LoadAddressableQueueItem<T>(TypedAssetReference);
			}

			return null;
		}
	}

	[Serializable]
	public class GameObjectAssetField : AbstractLoadAssetField<GameObject>
	{
		[SerializeField] 
		private AssetReferenceGameObject AssetReference;

		protected override AssetReferenceT<GameObject> TypedAssetReference => AssetReference;
		public override bool IsLoadTypeSupported(LoadType candidate)
		{
			return true;
		}
	}
	
	[Serializable]
	public class SpriteAssetField : AbstractLoadAssetField<Sprite>
	{
		[SerializeField] 
		private AssetReferenceSprite AssetReference;
		
		protected override AssetReferenceT<Sprite> TypedAssetReference => AssetReference;
		public override bool IsLoadTypeSupported(LoadType candidate)
		{
			return true;
		}
	}
	
	
	
}