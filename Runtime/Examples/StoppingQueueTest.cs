using System;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using UnityEngine;
using WhiteSparrow.Shared.Queue;
using WhiteSparrow.Shared.Queue.Items;

namespace Plugins.WhiteSparrow.Queue.Examples
{
	public class StoppingQueueTest : MonoBehaviour
	{
		private int delayTaskCount = 0;
		private System.Random r;
		private void Start()
		{
			r = new System.Random();
			RunQueueTest();
		}

		private async void RunQueueTest()
		{
			delayTaskCount = 0;
			
			var q = new ExecutionQueue();
			for (int i = 0; i < 2; i++)
				q.Add(ExampleDelayTask);
			q.Add(new StoppableTask());
			for (int i = 0; i < 2; i++)
				q.Add(ExampleDelayTask);
			q.Start();

			await Task.Delay(4000);
			
			Debug.Log("Stop");
			q.Stop();
			
			await q;
			
			Debug.Log($"Completed queue Test {q.State} {q.Result}");
		}

		private async Task ExampleDelayTask()
		{
			int taskId = delayTaskCount++;
			int randomDelayMs = 1000; // Mathf.RoundToInt(200 + r.Next(1600));
			
			
			Debug.Log($"Starting Delay Task: {taskId}: {randomDelayMs}");
			await Task.Delay(randomDelayMs);
			Debug.Log($"Completed Delay Task: {taskId}");
		}

		private async UniTask ExampleUniTaskItem()
		{
			
		}
	}

	public class StoppableTask : AbstractQueueItem
	{
		protected override UniTask Execute() => UniTask.CompletedTask;
	}
}