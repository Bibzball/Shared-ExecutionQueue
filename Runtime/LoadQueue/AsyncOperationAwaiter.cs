using System;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace Plugins.WhiteSparrow.Queue.LoadQueue
{
	public class AsyncOperationAwaiter : INotifyCompletion
	{
		private AsyncOperation m_Operation;
		
		public AsyncOperationAwaiter(AsyncOperation operation)
		{
			m_Operation = operation;
			if (m_Operation != null && !m_Operation.isDone)
				m_Operation.completed += OnOperationCompleted;
		}

		private void OnOperationCompleted(AsyncOperation obj)
		{
			m_Operation.completed -= OnOperationCompleted;
			if(m_Operation.isDone)
				RunContinuation();
		}
		public bool IsCompleted => m_Operation == null || m_Operation.isDone;

		protected Action m_continuation;
		
		public void OnCompleted(Action continuation)
		{
			m_continuation = continuation;
			if (m_Operation.isDone)
				RunContinuation();
		}
		
		public void RunContinuation()
		{
			m_continuation();
		}
		
		public void GetResult()
		{
		}
	}
	
	public static class AsyncOperationAwaiterExtensions
	{
		public static AsyncOperationAwaiter GetAwaiter(this AsyncOperation instance)
		{
			return new AsyncOperationAwaiter(instance);
		}
	}
}