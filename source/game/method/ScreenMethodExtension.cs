public static class ScreenMethodExtension
{
	public static string GetMethodSave(this Godot.Object gdObj)
	{
		return SAVE;
	}

	public static string GetMethodLoad(this Godot.Object gdObj)
	{
		return LOAD;
	}

	public static string GetMethodIsValidDataMap(this Godot.Object gdObj)
	{
		return IS_VALID_DATA_MAP;
	}

	public static string GetMethodOnToggleCheatCode(this Godot.Object gdObj)
	{
		return ON_TOGGLE_CHEAT_CODE;
	}


	private const string SAVE = "Save";
	private const string LOAD = "Load";
	private const string IS_VALID_DATA_MAP = "IsValidDataMap";
	private const string ON_TOGGLE_CHEAT_CODE = "OnToggleCheatCode";
}
