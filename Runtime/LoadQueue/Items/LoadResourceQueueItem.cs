using Cysharp.Threading.Tasks;
using UnityEngine;
using WhiteSparrow.Shared.Queue.Items;

namespace Plugins.WhiteSparrow.Queue.LoadQueue
{
	public class LoadResourceQueueItem<T> : AbstractLoadQueueItem, ILoadAssetQueueItem
		where T : Object
	{
		private string m_Path;
		private T m_Asset;
		public T Asset
		{
			get { return m_Asset; }
		}

		public LoadResourceQueueItem(string path)
		{
			m_Path = path;
		}
		
		protected override async UniTask ExecuteLoad()
		{
			ResourceRequest request = Resources.LoadAsync<T>(m_Path);
			await request.WithCancellation(CancellationToken);
			
			if (request.asset is T assetT)
				m_Asset = assetT;
			SetResult(m_Asset != null ? QueueResult.Success : QueueResult.Fail);

		}

		protected override UniTask ExecuteUnload()
		{
			if (m_Asset != null)
			{
				Resources.UnloadAsset(m_Asset);
				m_Asset = null;
			}
			return UniTask.CompletedTask;
		}

		object ILoadAssetQueueItem.Asset => this.Asset;
	}
}