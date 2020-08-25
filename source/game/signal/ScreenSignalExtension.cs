public static class ScreenSignalExtension
{
	public static string GetSignalSave(this Godot.Object gdObj)
	{
		return SAVE;
	}

	public static string GetSignalLoad(this Godot.Object gdObj)
	{
		return LOAD;
	}

	public static string GetSignalIsValidDataMap(this Godot.Object gdObj)
	{
		return IS_VALID_DATA_MAP;
	}


	private const string SAVE = "sav";
	private const string LOAD = "lod";
	private const string IS_VALID_DATA_MAP = "i_v_d_m";
}
