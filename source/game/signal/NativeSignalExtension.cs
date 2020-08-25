public static class NativeSignalExtension
{
	public static string GetGDSignalAreaEntered(this Godot.Object gdObj)
	{
		return AREA_ENTERED;
	}

	public static string GetGDSignalReady(this Godot.Object gdObj)
	{
		return READY;
	}

	public static string GetGDSignalTreeEntered(this Godot.Object gdObj)
	{
		return TREE_ENTERED;
	}

	public static string GetGDSignalTreeExited(this Godot.Object gdObj)
	{
		return TREE_EXITED;
	}


	private const string AREA_ENTERED = "area_entered";
	private const string TREE_ENTERED = "tree_entered";
	private const string TREE_EXITED = "tree_exited";
	private const string READY = "ready";
}
