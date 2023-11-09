using System;

namespace WhiteSparrow.Shared.Queue.Items
{
	public delegate void QueueItemDelegate(IQueueItem item);
	
	public interface IQueueItem
	{
		QueueResult Result { get; }
		QueueState State { get; }
		
		bool IsRunning { get; }
		bool IsDone { get; }

		event QueueItemDelegate OnComplete;

		void Start();
	}

	public interface IStopQueueItem
	{
		void Stop();
	}

	public interface IComplexQueueItem<T>
		where T : struct, IConvertible
	{
		void SetOperation(T operation);
	}

	public enum QueueResult
	{
		None = 0,
		Success = 1,
		Fail = 2,
		Stop = 3
	}

	public enum QueueState
	{
		None = 0,
		Running = 1,
		Completed = 2,
		Stopping = 3,
		Stopped = 4,
		
	}
}