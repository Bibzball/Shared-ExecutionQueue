using System;
using System.Collections;
using System.Threading.Tasks;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using WhiteSparrow.Shared.Queue;

namespace Plugins.Repositories.Shared_ExecutionQueue.Tests
{
	public class ExecutionQueueTests
	{
		[UnityTest]
		public IEnumerator AsyncItems()
		{
			ExecutionQueue queue = new ExecutionQueue();
			queue.Add(AsyncItemExample1);
			queue.Add(AsyncItemExample1);
			queue.Add(AsyncItemExample1);
			queue.Add(AsyncItemExample1);
			queue.Add(AsyncItemExample1);
			queue.Add(AsyncItemExample1);
			queue.Start();
			
			while(!queue.IsDone)
				yield return new WaitForSeconds(1);
			
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
