# Shared-ExecutionQueue
The Execution Queue system is meant to add a simple, unified system to queue actions one after another, with simple parallelism and completion reporting.

The abstract classes can be extended to customise the system for specific use cases, and example is the LoadQueue which can handle Resource or Addressable load queueing and simple parallelism.

# Installation
In package manager choose add from `git repo Url` and paste this repository Url.

# Execution Queue Usage

```csharp
public async void Start()
{
    ExecutionQueue queue = new ExecutionQueue();
    queue.Add(new CustomQueueItem());
    queue.Add(SystemTaskQueueItem);
    queue.Add(UniTaskQueueItem);
    
    queue.Start();
    await queue;
    
    Debug.Log("Queue Complete");
}

public async Task SystemTaskQueueItem()
{
    // logic
}

public async UniTask UniTaskQueueItem()
{
    // logic
}

public class CustomQueueItem : AbstractQueueItem
{
    protected override void Execute()
    {
        // logic
        // ...
        
        End();
    }
}
```


There are multiple ways to define queue items.
- Create custom Queue Item and execute logic within them
- Pass C# async / Task methods as Queue Item
- Pass a UniTask async method as Queue Item

Built in converters will take care of wrapping all items in the ExecutionQueueItem object to handle state and callback processing.

# Custom Queue Items

Creating custom Queue Items can be quite beneficial when you're reusing a set of sequences across different places in your game.
Extending the `AbstractQueueItem` gives you a few options to utilise the built in features.

```csharp
public class CustomQueueItem : AbstractQueueItem
{
    protected override void Execute()
    {
        // logic
        // ...
        
        End();
    }
}
```

All items need to call `End()` to notify about finishing its execution, this can be done after an async callback is finished ot when your task completes in the same logic.

Failed status can be propagated by calling `End(QueueResult.Fail)`.

## Enforcing Main Thread
In unity quite often you're required to execute on the main thread, it is possible to force a Queue Item execution on main thread by 

```csharp
public class CustomQueueItem : AbstractQueueItem
{
    protected override bool forceMainThread => true;
    protected override bool forceCompleteOnMainThread => true;

    protected override void Execute()
    {
        // logic
        // ...
        
        End();
    }
}
```

- `forceMainThread` - will force both execution and completion to be performed on the main thread.
- `forceCompleteOnMainThread` - will force completion to be performed on main thread.

Alternatively you can use `EndOnMainThread()` to invoke end on main thread.
```csharp
public class CustomQueueItem : AbstractQueueItem
{
    protected override void Execute()
    {
        // logic
        // ...
        
        EndOnMainThread();
        EndOnMainThread(QueueResult.Fail);
    }
}
```
## Async execution
It is perfectly fine to use `async void` output from the `Execute()` method. 
```csharp
public class CustomQueueItem : AbstractQueueItem
{
    protected override async void Execute()
    {
        // logic
        // ...
        
        End();
    }
}
```
> [!NOTE]
> `async Task Execute()` methods are not supported in the `AbstractQueueItem` at the moment.

You can also use any asynchronous process callback to finalise your item. 
```csharp
public class CreateInstances : AbstractQueueItem
{
    private GameObject m_Prefab;
    protected override bool forceMainThread => true;

    public CreateInstances(GameObject prefab)
    {
        m_Prefab = prefab;
    }
    
    protected override async void Execute()
    {
        var asynOp = GameObject.InstantiateAsync(m_Prefab, 10);
        asynOp.completed += OnInstantiateComplete;
    }

    private void OnInstantiateComplete(AsyncOperation op)
    {
        End();
    }
}
```

# Load Queue Usage

```csharp
LoadQueue queue = new LoadQueue();
queue.MaxConcurrentItems = 3;

var gameplaySceneLoad = queue.Add(new LoadSceneQueueItem("Gameplay", LoadSceneMode.Single));
var environmentSceneLoad = queue.Add(new LoadSceneQueueItem("Environment", LoadSceneMode.Additive));
var heroImageLoad = queue.Add(new LoadResourceQueueItem<Sprite>("Images/HeroImage"));

await queue.Load();

var gameplayRootObjects = gameplaySceneLoad.scene.GetRootGameObjects();
var environmentRootObjects = environmentSceneLoad.scene.GetRootGameObjects();
var heroImage = heroImageLoad.Asset;

await queue.Unload();
```
The LoadQueue and Load Items have two distinct operations, Load and Unload. After loading you can re-attach any complete callbacks and call unload to perform a unload on all queued items.