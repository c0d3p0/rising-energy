public static class GameSignalExtension
{
	public static string GetSignalGet(this Godot.Object gdObj)
	{
		return GET;
	}

	public static string GetSignalPut(this Godot.Object gdObj)
	{
		return PUT;
	}

	public static string GetSignalRemove(this Godot.Object gdObj)
	{
		return REMOVE;
	}


	private const string GET = "get";
	private const string PUT = "put";
	private const string REMOVE = "del";
}

