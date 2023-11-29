using System.Threading.Tasks;
using UnityEngine.SceneManagement;
using WhiteSparrow.Shared.Queue.Items;

namespace Plugins.WhiteSparrow.Queue.LoadQueue
{
	public abstract class AbstractLoadQueueItem : AbstractComplexQueueItem<LoadQueueOperation>, ILoadQueueItem
	{
		
		protected override void ExecuteOperation(LoadQueueOperation operation)
		{
			switch (operation)
			{
				case LoadQueueOperation.Load:
					ExecuteLoad();
					break;
				case LoadQueueOperation.Unload:
					ExecuteUnload();
					break;
			}
		}
		
		public void Load()
		{
			SetOperation(LoadQueueOperation.Load);
			base.Start();
		}

		public void Unload()
		{
			SetOperation(LoadQueueOperation.Unload);
			Start();
		}
		

		protected abstract void ExecuteLoad();
		
		protected abstract void ExecuteUnload();
	}

}