using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using WhiteSparrow.Shared.Queue.Items;

namespace Plugins.WhiteSparrow.Queue.LoadQueue
{
	public delegate void LoadQueueItemDelegate(IQueueItem item);
	public delegate void LoadQueueItemDelegate<T>(LoadAddressableQueueItem<T> item);

	public abstract class AbstractLoadAddressableQueueItem : AbstractLoadQueueItem, ILoadAssetQueueItem
	{
		private AssetReference m_AssetReference;
		public AssetReference AssetReference => m_AssetReference;

		private ICustomAddressableReference m_CustomAssetReference;
		public ICustomAddressableReference CustomAssetReference => m_CustomAssetReference;
		
		public AbstractLoadAddressableQueueItem(AssetReference assetReference)
		{
			m_AssetReference = assetReference;
		}

		public AbstractLoadAddressableQueueItem(ICustomAddressableReference assetReference)
		{
			m_AssetReference = assetReference.AssetReference;
			m_CustomAssetReference = assetReference;
		}

		object ILoadAssetQueueItem.Asset => ResolveAsset();
		public object Asset => ResolveAsset();
		protected abstract object ResolveAsset();
	}
	
	public class LoadAddressableQueueItem<T> : AbstractLoadAddressableQueueItem
	{
		private LoadQueueItemDelegate<T> m_OnComplete;
		public new event LoadQueueItemDelegate<T> OnComplete
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
		
		public LoadAddressableQueueItem(AssetReference assetReference) : base(assetReference)
		{
		}

		public LoadAddressableQueueItem(ICustomAddressableReference assetReference) : base(assetReference)
		{
		}

		public T Asset
		{
			get
			{
				return m_LoadOperation.IsValid() ? (T) m_LoadOperation.Result : default;
			}
		}

		protected override object ResolveAsset() => this.Asset;

		private AsyncOperationHandle m_LoadOperation;
		protected AsyncOperationHandle LoadOperation;
	
		
		

		protected override void ExecuteLoad()
		{
			if (AssetReference == null)
			{
				End(QueueResult.Fail);
				return;
			}

			if (AssetReference.OperationHandle.IsValid())
			{
				m_LoadOperation = AssetReference.OperationHandle;
			}
			else
			{
				m_LoadOperation = TriggerAssetLoad(AssetReference);
			}

			if (m_LoadOperation.OperationException != null)
			{
				OnLoadOperationFailed(m_LoadOperation);
				return;
			}

			if (m_LoadOperation.IsDone)
			{
				OnLoadOperationCompleted(m_LoadOperation);
			}
			else
			{
				m_LoadOperation.Completed += OnLoadOperationCompleted;
			}
		}

		private void OnLoadOperationCompleted(AsyncOperationHandle loadOperation)
		{
			loadOperation.Completed -= OnLoadOperationCompleted;

			if (loadOperation.OperationException != null)
			{
				OnLoadOperationFailed(loadOperation);
				return;
			}

			if (loadOperation.Status == AsyncOperationStatus.Failed)
			{
				
			}
			
			End(m_LoadOperation.Status == AsyncOperationStatus.Succeeded ? QueueResult.Success : QueueResult.Fail);
		}

		protected virtual void OnLoadOperationFailed(AsyncOperationHandle loadOperation)
		{
			loadOperation.Completed -= OnLoadOperationCompleted;
			
			
		}

		protected virtual AsyncOperationHandle<T> TriggerAssetLoad(AssetReference assetReference)
		{
			return assetReference.LoadAssetAsync<T>();
		}

		protected virtual void CancelLoad()
		{
			if (m_LoadOperation.IsValid())
			{
				m_LoadOperation.Completed -= OnLoadOperationCompleted;
				Addressables.Release(m_LoadOperation);
			}
			
		}
		
		protected override void ExecuteUnload()
		{
			CancelLoad();
			End(QueueResult.Success);
		}

	}

	public interface ICustomAddressableReference
	{
		AssetReference AssetReference { get; }
	}
}