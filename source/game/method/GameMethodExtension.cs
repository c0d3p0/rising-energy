public static class GameMethodExtension
{
	public static string GetMethodGet(this Godot.Object gdObj)
	{
		return GET;
	}

	public static string GetMethodPut(this Godot.Object gdObj)
	{
		return PUT;
	}

	public static string GetMethodPutAll(this Godot.Object gdObj)
	{
		return PUT_ALL;
	}

	public static string GetMethodRemove(this Godot.Object gdObj)
	{
		return REMOVE;
	}

	public static string GetMethodClear(this Godot.Object gdObj)
	{
		return CLEAR;
	}

	public static string GetMethodIncreaseInstanceCount(this Godot.Object gdObj)
	{
		return INCREASE_INSTANCE_COUNT;
	}


	private const string GET = "Get";
	private const string PUT = "Put";
	private const string PUT_ALL = "PutAll";
	private const string REMOVE = "Remove";
	private const string CLEAR = "Clear";
	private const string QUEUE_REMOVE_STRAY_NODE = "QueueRemoveStrayNode";
	private const string INCREASE_INSTANCE_COUNT = "IncreaseInstanceCount";
}
