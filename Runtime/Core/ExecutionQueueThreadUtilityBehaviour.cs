using System;
using System.Threading;
using UnityEngine;

namespace WhiteSparrow.Shared.Queue
{
	internal static class ExecutionQueueThreadUtility
	{
		private static int s_ThreadId;
		
		[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
		private static void Initialize()
		{
			s_ThreadId = Thread.CurrentThread.ManagedThreadId;
			ExecutionQueueThreadUtilityBehaviour.GetInstance();
		}

		private static Action s_PendingActions;
		public static void ExecuteOnMainThread(Action action)
		{
			if (action == null)
				throw new ArgumentNullException(nameof(action), "ExecuteOnMainThread requires an action");
			
			if (Thread.CurrentThread.ManagedThreadId != s_ThreadId)
			{
				if (s_PendingActions == null)
					ExecutionQueueThreadUtilityBehaviour.OnUpdate += OnUnityUpdate;
				s_PendingActions += action;
				return;
			}
			
			action.Invoke();
		}
		
		public static void WaitAndExecuteOnMainThread(Action action)
		{
			if (action == null)
				throw new ArgumentNullException(nameof(action), "ExecuteOnMainThread requires an action");

			if (Thread.CurrentThread.ManagedThreadId == s_ThreadId)
			{
				action.Invoke();
				return;
			}

			bool complete = false;
			ExecutionQueueThreadUtilityBehaviour.OnUpdate += () =>
			{
				action.Invoke();
				complete = true;
			};
			while (!complete)
			{
			}
		}
		public static void WaitAndExecuteOnMainThread<T>(Action<T> action, T data)
		{
			if (action == null)
				throw new ArgumentNullException(nameof(action), "ExecuteOnMainThread requires an action");

			if (Thread.CurrentThread.ManagedThreadId == s_ThreadId)
			{
				action.Invoke(data);
				return;
			}

			bool complete = false;
			ExecutionQueueThreadUtilityBehaviour.OnUpdate += () =>
			{
				action.Invoke(data);
				complete = true;
			};
			while (!complete)
			{
			}
		}

		private static void OnUnityUpdate()
		{
			var c = s_PendingActions;
			s_PendingActions = null;
			c?.Invoke();
		}
	}
	
	public class ExecutionQueueThreadUtilityBehaviour : MonoBehaviour
	{
		private static ExecutionQueueThreadUtilityBehaviour s_Instance;
		internal static ExecutionQueueThreadUtilityBehaviour GetInstance()
		{
			if (s_Instance != null)
				return s_Instance;

			var go = new GameObject("Corvus Callback Utility");
			DontDestroyOnLoad(go);
			s_Instance = go.AddComponent<ExecutionQueueThreadUtilityBehaviour>();
			return s_Instance;
		}
		
		public static event Action OnUpdate
		{
			add => GetInstance().m_OnUpdate += value;
			remove => GetInstance().m_OnUpdate -= value;
		}
		public static event Action OnLateUpdate
		{
			add => GetInstance().m_OnLateUpdate += value;
			remove => GetInstance().m_OnLateUpdate -= value;
		}
		
		private Action m_OnUpdate;
		private Action m_OnLateUpdate;

		private void Update()
		{
			var c = m_OnUpdate;
			m_OnUpdate = null;
			c?.Invoke();
		}

		private void LateUpdate()
		{
			var c = m_OnLateUpdate;
			m_OnLateUpdate = null;
			c?.Invoke();
		}

		
	}
}