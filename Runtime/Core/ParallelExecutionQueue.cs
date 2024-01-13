namespace WhiteSparrow.Shared.Queue
{
	public class ParallelExecutionQueue : ExecutionQueue
	{
		protected override bool CanStartItem()
		{
			return true;
		}
	}
}