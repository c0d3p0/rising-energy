public static class LevelSignalExtension
{
	public static string GetSignalAddDeathPit(this Godot.Object gdObj)
	{
		return ADD_DEATH_PIT;
	}

	public static string GetSignalRemoveDeathPit(this Godot.Object gdObj)
	{
		return REMOVE_DEATH_PIT;
	}

	public static string GetSignalSetFightArea(this Godot.Object gdObj)
	{
		return SET_FIGHT_AREA;
	}

	public static string GetSignalAddEnemySpawnSpot(this Godot.Object gdObj)
	{
		return ADD_ENEMY_SPAWN_SPOT;
	}

	public static string GetSignalRemoveEnemySpawnSpot(this Godot.Object gdObj)
	{
		return REMOVE_ENEMY_SPAWN_SPOT;
	}

	public static string GetSignalSaveLevelProgress(this Godot.Object gdObj)
	{
		return SAVE_LEVEL_PROGRESS;
	}

	public static string GetSignalPlayerScoreChanged(this Godot.Object gdObj)
	{
		return PLAYER_SCORE_CHANGED;
	}

	public static string GetSignalNextBossScoreChanged(this Godot.Object gdObj)
	{
		return NEXT_BOSS_SCORE_CHANGED;
	}

	public static string GetSignalFinishLevel(this Godot.Object gdObj)
	{
		return FINISH_LEVEL;
	}

	public static string GetSignalOnIntroFinished(this Godot.Object gdObj)
	{
		return ON_INTRO_FINISH;
	}

	public static string GetSignalOnFightAreaEntered(this Godot.Object gdObj)
	{
		return ON_FIGHT_AREA_ENTERED;
	}

	public static string GetSignalOnFightAreaExited(this Godot.Object gdObj)
	{
		return ON_FIGHT_AREA_EXITED;
	}


	private const string ADD_DEATH_PIT = "a_d_p";
	private const string REMOVE_DEATH_PIT = "r_d_p";
	private const string SET_FIGHT_AREA = "a_f_a";	
	private const string ADD_ENEMY_SPAWN_SPOT = "a_e_s_s";
	private const string REMOVE_ENEMY_SPAWN_SPOT = "r_e_s_s";
	private const string SAVE_LEVEL_PROGRESS = "s_l_p";
	private const string PLAYER_SCORE_CHANGED = "p_s_c";
	private const string NEXT_BOSS_SCORE_CHANGED = "n_b_s_c";
	private const string FINISH_LEVEL = "f_l";
	private const string ON_FIGHT_AREA_ENTERED = "o_f_a_en";
	private const string ON_FIGHT_AREA_EXITED = "o_f_a_ex";
	private const string ON_INTRO_FINISH = "o_i_f";
}
