public static class GUIMethodExtension
{
	public static string GetMethodSetProgress(this Godot.Object gdObj)
	{
		return SET_PROGRESS;
	}	


	private const string SET_PROGRESS = "SetProgress";
}
