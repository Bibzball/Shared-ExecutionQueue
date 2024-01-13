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
		
		protected override void ExecuteLoad()
		{
			ResourceRequest request = Resources.LoadAsync<T>(m_Path);
			request.completed += LoadOperationComplete;
		}

		private void LoadOperationComplete(AsyncOperation op)
		{
			op.completed -= LoadOperationComplete;
			if (op is ResourceRequest request)
			{
				if (request.asset is T assetT)
					m_Asset = assetT;
			}
			End(m_Asset != null ? QueueResult.Success : QueueResult.Fail);
		}

		protected override void ExecuteUnload()
		{
			if (m_Asset != null)
			{
				Resources.UnloadAsset(m_Asset);
				m_Asset = null;
			}
			End();
		}

		object ILoadAssetQueueItem.Asset => this.Asset;
	}
}