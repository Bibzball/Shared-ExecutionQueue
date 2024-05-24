using System.Threading.Tasks;
using UnityEngine.SceneManagement;
using WhiteSparrow.Shared.Queue.Items;

namespace Plugins.WhiteSparrow.Queue.LoadQueue
{
	public delegate void LoadSceneQueueItemDelegate(IQueueItem item);
	public abstract class AbstractSceneLoadQueueItem : AbstractLoadQueueItem
	{

		private Scene m_Scene;
		public Scene scene => m_Scene;
		
		protected override async void ExecuteLoad()
		{
			SceneManager.sceneLoaded += OnSceneLoaded;
			await LoadScene();
			SceneManager.sceneLoaded -= OnSceneLoaded;
			
			End(ValidateLoadResult() ? QueueResult.Success : QueueResult.Fail);
		}

		protected override async void ExecuteUnload()
		{
			await UnloadScene();
			
			End(QueueResult.Success);
		}

		protected abstract Task LoadScene();
		protected abstract Task UnloadScene();


		private void OnSceneLoaded(Scene scene, LoadSceneMode loadSceneMode)
		{
			if (!IsLoadedSceneTarget(scene, loadSceneMode))
				return;

			m_Scene = scene;
		}

		protected bool ValidateLoadResult()
		{
			return m_Scene.IsValid() && m_Scene.isLoaded;
		}

		protected abstract bool IsLoadedSceneTarget(Scene scene, LoadSceneMode loadSceneMode);
		
		
		private LoadSceneQueueItemDelegate m_OnComplete;
		public event LoadSceneQueueItemDelegate OnComplete
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
			base.InvokeOnComplete();
			m_OnComplete?.Invoke(this);
		}
	}
}