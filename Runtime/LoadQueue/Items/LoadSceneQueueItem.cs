using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Plugins.WhiteSparrow.Queue.LoadQueue
{
	public class LoadSceneQueueItem : AbstractSceneLoadQueueItem
	{
		private enum LoadSceneMethod
		{
			SceneName = 0,
			SceneIndex = 1
		}
		
		private LoadSceneMethod m_LoadSceneMethod;
		private string m_SceneName;

		
		private int m_SceneBuildIndex;
		private LoadSceneMode m_LoadSceneMode;

		private AsyncOperation m_LoadOperation;
		
		
		public LoadSceneQueueItem(string sceneName, LoadSceneMode loadSceneMode = LoadSceneMode.Single)
		{
			m_LoadSceneMethod = LoadSceneMethod.SceneName;
			m_SceneName = sceneName;
			m_LoadSceneMode = loadSceneMode;
		}

		public LoadSceneQueueItem(int sceneIndex, LoadSceneMode loadSceneMode = LoadSceneMode.Single)
		{
			m_LoadSceneMethod = LoadSceneMethod.SceneIndex;
			m_SceneBuildIndex = sceneIndex;
			m_LoadSceneMode = loadSceneMode;
		}
		
		protected override async Task LoadScene()
		{
			switch (m_LoadSceneMethod)
			{
				case LoadSceneMethod.SceneName:
					m_LoadOperation = SceneManager.LoadSceneAsync(m_SceneName, m_LoadSceneMode);
					break;
				case LoadSceneMethod.SceneIndex:
					m_LoadOperation = SceneManager.LoadSceneAsync(m_SceneBuildIndex, m_LoadSceneMode);
					break;
			}

			await m_LoadOperation;
		}

		protected override async Task UnloadScene()
		{
			m_LoadOperation = SceneManager.UnloadSceneAsync(scene);
			await m_LoadOperation;
		}

		protected override bool IsLoadedSceneTarget(Scene scene, LoadSceneMode loadSceneMode)
		{
			if (m_LoadSceneMode != loadSceneMode)
				return false;

			switch (m_LoadSceneMethod)
			{
				case LoadSceneMethod.SceneName:
					if (scene.name != m_SceneName)
						return false;
					break;
				case LoadSceneMethod.SceneIndex:
					if (scene.buildIndex != m_SceneBuildIndex)
						return false;
					break;
			}

			return true;
		}
	}
}