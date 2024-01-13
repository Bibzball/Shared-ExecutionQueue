using System;
using System.Threading.Tasks;
using NUnit.Framework;
using UnityEngine;
using WhiteSparrow.Shared.Queue;

namespace Plugins.Repositories.Shared_ExecutionQueue.Tests
{
	public class ExecutionQueueTests
	{
		[Test]
		public async void AsyncItems()
		{
			ExecutionQueue queue = new ExecutionQueue();
			queue.Add(AsyncItemExample1);
			queue.Start();
			
			await queue;
			Debug.Log("Completion");
		}

		public async Task AsyncItemExample1()
		{
			Debug.Log("Example item Start");
			await Task.Delay(TimeSpan.FromSeconds(2));
			Debug.Log("Example item Complete");

		}
	}

}
