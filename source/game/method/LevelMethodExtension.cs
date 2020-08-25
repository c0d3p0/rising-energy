public static class LevelMethodExtension
{
	public static string GetMethodAddDeathPit(this Godot.Object gdObj)
	{
		return ADD_DEATH_PIT;
	}

	public static string GetMethodRemoveDeathPit(this Godot.Object gdObj)
	{
		return REMOVE_DEATH_PIT;
	}

	public static string GetMethodSetFightArea(this Godot.Object gdObj)
	{
		return SET_FIGHT_AREA;
	}

	public static string GetMethodAddEnemySpawnSpot(this Godot.Object gdObj)
	{
		return ADD_ENEMY_SPAWN_SPOT;
	}

	public static string GetMethodRemoveEnemySpawnSpot(this Godot.Object gdObj)
	{
		return REMOVE_ENEMY_SPAWN_SPOT;
	}

	public static string GetMethodOnKinDestroyed(this Godot.Object gdObj)
	{
		return ON_KIN_DESTROYED;
	}
	
	public static string GetMethodSaveLevelProgress(this Godot.Object gdObj)
	{
		return SAVE_LEVEL_PROGRESS;
	}

	public static string GetMethodSetTriggerPosition(this Godot.Object gdObj)
	{
		return SET_TRIGGER_POSITION;
	}

	public static string GetMethodSetCheckpointPosition(this Godot.Object gdObj)
	{
		return SET_CHECKPOINT_POSITION;
	}


	public static string GetMethodSetEnemySpawnData(this Godot.Object gdObj)
	{
		return SET_ENEMY_SPAWN_DATA;
	}
	
	public static string GetMethodSetEvilEnergySpawnData(this Godot.Object gdObj)
	{
		return SET_EVIL_ENERGY_SPAWN_DATA;
	}

	public static string GetMethodSetBossEvilEnergySpawnData(this Godot.Object gdObj)
	{
		return SET_BOSS_EVIL_ENERGY_SPAWN_DATA;
	}

	public static string GetMethodSetEnergySourceSpawnData(this Godot.Object gdObj)
	{
		return SET_ENERGY_SOURCE_SPAWN_DATA;
	}

	public static string GetMethodSetFightAreaSpawnData(this Godot.Object gdObj)
	{
		return SET_FIGHT_AREA_SPAWN_DATA;
	}

	public static string GetMethodSetBossSpawnData(this Godot.Object gdObj)
	{
		return SET_BOSS_SPAWN_DATA;
	}

	public static string GetMethodSetCheckpointSpawnData(this Godot.Object gdObj)
	{
		return SET_CHECKPOINT_SPAWN_DATA;
	}

	public static string GetMethodSetActive(this Godot.Object gdObj)
	{
		return SET_ACTIVE;
	}


	private const string ADD_DEATH_PIT = "AddDeathPit";
	private const string REMOVE_DEATH_PIT = "RemoveDeathPit";
	private const string SET_FIGHT_AREA = "SetFightArea";
	private const string ADD_ENEMY_SPAWN_SPOT = "AddEnemySpawnSpot";
	private const string REMOVE_ENEMY_SPAWN_SPOT = "RemoveEnemySpawnSpot";
	private const string ON_KIN_DESTROYED = "OnKinDestroyed";
	private const string SAVE_LEVEL_PROGRESS = "SaveLevelProgress";
	private const string SET_TRIGGER_POSITION = "SetTriggerPosition";
	private const string SET_CHECKPOINT_POSITION = "SetCheckpointPosition";

	private const string SET_ENERGY_SOURCE_SPAWN_DATA = "SetEnergySourceSpawnData";
	private const string SET_ENEMY_SPAWN_DATA = "SetEnemySpawnData";
	private const string SET_BOSS_SPAWN_DATA = "SetBossSpawnData";
	private const string SET_FIGHT_AREA_SPAWN_DATA = "SetFightAreaSpawnData";
	private const string SET_EVIL_ENERGY_SPAWN_DATA = "SetEvilEnergySpawnData";
	private const string SET_BOSS_EVIL_ENERGY_SPAWN_DATA = "SetBossEvilEnergySpawnData";
	private const string SET_CHECKPOINT_SPAWN_DATA = "SetCheckpointSpawnData";

	private const string SET_ACTIVE = "SetActive";
}
