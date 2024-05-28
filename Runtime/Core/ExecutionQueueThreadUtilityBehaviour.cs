using System;
using System.Threading;
using UnityEngine;

namespace WhiteSparrow.Shared.Queue
{
	internal static class ExecutionQueueThreadUtility
	{
		private static ExecutionQueueThreadUtilityBehaviour s_Instance;
		private static int s_MainThread;
		
		public static bool IsMainThread
		{
			get => Thread.CurrentThread.ManagedThreadId == s_MainThread;
		}

		private static Action s_CallOnMainThread;
		public static event Action CallOnMainThread
		{
			add
			{
				if (IsMainThread)
					value?.Invoke();
				else
				{
					s_CallOnMainThread += value;
					s_Instance.OnUpdate += MainThreadCallback;
				}
			}
			remove
			{
				s_CallOnMainThread -= value;
				if(s_CallOnMainThread == null)
					s_Instance.OnUpdate -= MainThreadCallback;
			}
		}

		private static void MainThreadCallback()
		{
			s_Instance.OnUpdate -= MainThreadCallback;
            
			if (s_CallOnMainThread == null)
				return;

			var call = s_CallOnMainThread;
			s_CallOnMainThread = null;
			call?.Invoke();
			
			
		}

		
		[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterAssembliesLoaded)]
		private static void Initialize()
		{
			Debug.Log("ExecutionQueueThreadUtility initialize");
			s_MainThread = Thread.CurrentThread.ManagedThreadId;
			GameObject go = new GameObject("ExecutionQueue Utility");
			GameObject.DontDestroyOnLoad(go);
			// go.hideFlags = HideFlags.HideAndDontSave;
			s_Instance = go.AddComponent<ExecutionQueueThreadUtilityBehaviour>();
		}
	}
	internal class ExecutionQueueThreadUtilityBehaviour : MonoBehaviour
	{
		private Action m_OnUpdate;
		public event Action OnUpdate
		{
			add
			{
				m_OnUpdate += value;
			}
			remove
			{
				m_OnUpdate -= value;
			}
		}

		private void Update()
		{
			m_OnUpdate?.Invoke();
		}

		private void LateUpdate()
		{
			m_OnUpdate?.Invoke();
		}
		
				
	
	}
}