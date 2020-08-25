public static class NativeMethodExtension
{
	public static string GetGDMethodAddChild(this Godot.Object gdObj)
	{
		return ADD_CHILD;
	}

	public static string GetGDMethodRemoveChild(this Godot.Object gdObj)
	{
		return REMOVE_CHILD;
	}

	public static string GetGDMethodQueueFree(this Godot.Object gdObj)
	{
		return QUEUE_FREE;
	}

	public static string GetGDMethodTravel(this Godot.Object gdObj)
	{
		return TRAVEL;
	}

	public static string GetGDMethodSetProcess(this Godot.Object gdObj)
	{
		return SET_PROCESS;
	}

	public static string GetGDMethodSetText(this Godot.Object gdObj)
	{
		return SET_TEXT;
	}

	public static string GetGDMethodPlay(this Godot.Object gdObj)
	{
		return PLAY;
	}


	private const string ADD_CHILD = "add_child";
	private const string REMOVE_CHILD = "remove_child";
	private const string QUEUE_FREE = "remove_child";
	private const string TRAVEL = "travel";
	private const string SET_PROCESS = "set_process";
	private const string SET_TEXT = "set_text";
	private const string PLAY = "play";
}
