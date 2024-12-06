using System.Collections;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.TestTools;
using WhiteSparrow.Shared.Queue.Items;

namespace WhiteSparrow.Shared.Queue.Tests
{
    public class ExecutionItemTests
    {
        [UnityTest]
        public IEnumerator CancellationToken()
        {
            UniTaskQueueItem qi = new UniTaskQueueItem(CancellableQueueItem);
            qi.Start();
            
            yield return new WaitForSeconds(1);
            
            qi.Stop();

            while (!qi.IsDone)
            {
                Debug.Log("Delaying to wait until isDone");
                yield return new WaitForSeconds(0.4f);
            }
        }

        private async UniTask CancellableQueueItem(CancellationToken token)
        {
            await UniTask.WaitForSeconds(3, false, PlayerLoopTiming.Update, token);
            
            Debug.Log("Item was not cancelled");
        }
    }
}