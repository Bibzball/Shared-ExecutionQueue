using System;
using System.Collections;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using WhiteSparrow.Shared.Queue.Items;

namespace WhiteSparrow.Shared.Queue.Tests
{
	public class ExecutionQueueTests
	{
		[UnityTest]
		public IEnumerator TaskItems()
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


		[UnityTest]
		public IEnumerator UniTaskItems()
		{
			ExecutionQueue queue = new ExecutionQueue();
			queue.Add(UniTaskItem);
			queue.Add(UniTaskItem);
			queue.Add(UniTaskItem);
			queue.Add(UniTaskItem);
			queue.Add(UniTaskItem);
			queue.Add(UniTaskItem);
			queue.Add(UniTaskItem);
			queue.Start();
			
			while(!queue.IsDone)
				yield return new WaitForSeconds(1);
			
			Debug.Log("Completion");
		}

		private async UniTask UniTaskItem()
		{
			Debug.Log("UniTask item Start");
			await UniTask.WaitForSeconds(1);
			Debug.Log("UniTask item Complete");
		}

		[UnityTest]
		public IEnumerator StoppingUniTaskQueue()
		{
			ExecutionQueue queue = new ExecutionQueue();
			queue.MaxConcurrentItems = 2;
			queue.Add(UniTaskItem);
			queue.Add(UniTaskItem);
			queue.Add(UniTaskItem);
			queue.Add(UniTaskItem);
			queue.Add(UniTaskItem);
			queue.Add(UniTaskItem);
			queue.Add(UniTaskItem);
			queue.Add(UniTaskItem);
			queue.Add(UniTaskItem);
			queue.Add(UniTaskItem);
			queue.Add(UniTaskItem);
			queue.Add(UniTaskItem);
			queue.Start();
			
			yield return new WaitForSeconds(1.1f);
			
			Debug.Log("Stop");
			queue.Stop();
			
			while (!queue.IsDone)
			{
				Debug.Log("Delaying to wait until isDone");
				yield return new WaitForSeconds(0.4f);
			}
			
			GetItemStateCounts(queue, out int none, out int running, out int stopped, out int completed);

			Assert.IsTrue(completed == 2);
			Assert.IsTrue(running == 0);
			Assert.IsTrue(stopped == 2);
			Assert.IsTrue(none == 8);
		}

		private void GetItemStateCounts(ExecutionQueue queue, out int none, out int running, out int stopped, out int completed)
		{
			none = running = stopped = completed = 0;
			var items = queue.GetItems();
			foreach (var item in items)
			{
				switch (item.State)
				{
					case QueueState.None:
						none++;
						break;
					case QueueState.Running:
						running++;
						break;
					case QueueState.Stopping:
					case QueueState.Stopped:
						stopped++;
						break;
					case QueueState.Completed:
						completed++;
						break;
				}
			}
		}

	}

}
