using Godot;
using Godot.Collections;


public class LevelProgress : Node
{
	public void FinishLevel()
	{
		if(!this.Call<bool>(target, this.GetMethodIsDead()))
		{
			SaveUpdatedTotalTime();
			globalData.Call(this.GetMethodPut(), "level_progress", 0);
			globalData.Call(this.GetMethodPut(), "spawn_point", Vector3.Zero);
			globalData.Call(this.GetMethodPut(), this.Call<Dictionary>(target,
					this.GetMethodGetVitalityMap()));
			globalData.Call(this.GetMethodPut(), "scene_to_load",
					this.GetScenePath(nextLevelScenePath));
			GetTree().ChangeScene(this.GetScenePath(loadScreenScenePath));
		}
	}

	public void ReloadLevel()
	{
		SaveUpdatedTotalTime();
		int deaths = this.Call<int>(globalData, this.GetMethodGet(), "deaths") + 1;
		globalData.Call(this.GetMethodPut(), "deaths", deaths);
		GetTree().ChangeScene(this.GetScenePath(loadScreenScenePath));
	}

	public void DeactivateNodeFactory()	// Called by an animation
	{
		nodeFactory.Call(this.GetMethodSetActive(), false);
	}

	public void SaveLevelProgress(Spatial checkpoint)
	{
		if(currentLevelProgress < bossPositionList.Count)
			SaveProgress(checkpoint);
		else
			StartFinishLevel();		
	}

	public void OnKinDestroyed(Spatial fightArea)
	{
		currentLevelProgress++;
		SpawnLevelManagedObjects();
	}

	public void ActivateLevel(bool active)
	{
		target.Call(this.GetMethodActivateInput(), active);
		EmitSignal(this.GetSignalOnIntroFinished());
	}

	public void SetBossSpawnData(Node boss, Dictionary paramMap)	// Called by NodeFactor
	{
		if(IsInsideTree() && !IsQueuedForDeletion())
		{
			Spatial s = boss as Spatial;
			s.Translation = currentLevelProgress < bossPositionList.Count ?
					bossPositionList[currentLevelProgress] :
					bossPositionList[bossPositionList.Count - 1];
			s.Call(this.GetMethodSetTarget(), target);
			s.Call(this.GetMethodSetNodeFactory(), nodeFactory);
			s.Call(this.GetMethodSetKinSpawnPivot(),
					currentLevelProgress < fightAreaPositionList.Count ?
					fightAreaPositionList[currentLevelProgress] :
					fightAreaPositionList[fightAreaPositionList.Count - 1]);
			currentBoss = s;
		}
	}

	public void SetFightAreaSpawnData(Node fightArea, Dictionary paramMap)	// Called by NodeFactor
	{
		if(IsInsideTree() && !IsQueuedForDeletion())
		{
			Spatial fa = fightArea as Spatial;
			fa.Translation = currentLevelProgress < fightAreaPositionList.Count ?
					fightAreaPositionList[currentLevelProgress] :
					fightAreaPositionList[fightAreaPositionList.Count - 1];
			fa.Call(this.GetMethodSetNodeFactory(), nodeFactory);
			fa.Call(this.GetMethodSetTarget(), currentBoss);
			fa.Call(this.GetMethodSetTriggerPosition(),
					currentLevelProgress < fightAreaTriggerPositionList.Count ?
					fightAreaTriggerPositionList[currentLevelProgress] :
					fightAreaTriggerPositionList[fightAreaTriggerPositionList.Count - 1]);
			fa.Connect(this.GetGDSignalTreeExited(), this,
					this.GetMethodOnKinDestroyed(), this.CreateSingleBind(fa));

			if(checkpointPositionMap.ContainsKey(currentLevelProgress))
			{
				fa.Call(this.GetMethodSetCheckpointPosition(),
						checkpointPositionMap[currentLevelProgress]);
			}

			currentFightArea = fa;
		}
	}

	private void SaveProgress(Spatial checkpoint)
	{
		Vector3 pp = target.GlobalTransform.origin;
		Vector3 cp = checkpoint.GlobalTransform.origin;
		pp.y = checkpoint.GlobalTransform.origin.y;
		globalData.Call(this.GetMethodPut(), "level_progress", currentLevelProgress);
		globalData.Call(this.GetMethodPut(), "spawn_point", pp);
		globalData.Call(this.GetMethodPut(), "player_direction",
				pp.x < cp.x ? Vector3.Right : Vector3.Left);
		globalData.Call(this.GetMethodPut(), this.Call<Dictionary>(target,
				this.GetMethodGetVitalityMap()));
	}

	private void SpawnLevelManagedObjects()
	{
		if(IsInsideTree() && !IsQueuedForDeletion() &&
				currentLevelProgress < bossPositionList.Count)
		{
			string bossPrefabKey = currentLevelProgress <  bossPrefabKeyList.Count ?
					bossPrefabKeyList[currentLevelProgress] :
					bossPrefabKeyList[bossPrefabKeyList.Count - 1];
			nodeFactory.Call(this.GetMethodPut(), this, bossPrefabKey,
					"SetBossSpawnData", null);
			nodeFactory.Call(this.GetMethodPut(), this, fightAreaPrefabKey,
					"SetFightAreaSpawnData", null);
		}
	}

	private void StartFinishLevel()
	{
		target.Call(this.GetMethodActivateInput(), false);
		target.Call(this.GetMethodCheer());
		this.EmitSignal(this.GetSignalFinishLevel());
	}

	private void SaveUpdatedTotalTime()
	{
		int totalTime = this.Call<int>(globalData, this.GetMethodGet(), "total_time");

		if(totalTime >= 0)
		{
			if((long) (OS.GetTicksMsec() - startTime) < (long) int.MaxValue)
				totalTime += ((int) OS.GetTicksMsec()) - startTime;
			else
				totalTime = int.MinValue;
				
			globalData.Call(this.GetMethodPut(), "total_time", totalTime);
		}
	}

	private void AddRequestedNodesToTheScene()
	{
		if(currentBoss != null && !currentBoss.IsInsideTree())
		{
			this.AddChildToEnemyContainer(this, currentBoss);
			currentBoss = null;
		}

		if(currentFightArea != null && !currentFightArea.IsInsideTree())
		{
			this.AddChildToEnemyContainer(this, currentFightArea);
			currentFightArea = null;
		}
	}

	private void InitializePlayerCharacter()
	{
		int swordId = this.Call<int>(globalData,
				this.GetMethodGet(), "sword_id");
		// int shieldId = this.Call<int>(globalData,
		// 		this.GetMethodGet(), "shield_id");
		float maxHealth = this.Call<float>(globalData,
				this.GetMethodGet(), "max_health");
		float currentHealth = this.Call<float>(globalData,
					this.GetMethodGet(), "current_health");
		float maxEnergy = this.Call<float>(globalData,
					this.GetMethodGet(), "max_energy");
		float currentEnergy = this.Call<float>(globalData,
					this.GetMethodGet(), "current_energy");
		Spatial sword = GetItem(swordPrefabKeyList, swordId);
		// Spatial shield = GetItem(shieldPrefabKeyList, shieldId);

		if(maxHealth == 0)
			maxHealth = 1;
		
		if(currentHealth == 0)
			currentHealth = 1;
				
		if(this.Call<int>(globalData, this.GetMethodGet(),
				"level_progress") > 0f)
		{
			target.Translation = this.Call<Vector3>(globalData,
					this.GetMethodGet(), "spawn_point");
			target.Call(this.GetMethodFixBodyDirection(), this.Call<Vector3>(
					globalData, this.GetMethodGet(), "player_direction"));
		}
		else
			target.Call(this.GetMethodFixBodyDirection(), playerStartingDirection);

		currentHealth += 300f;	// Health recover that must happen at each checkpoing.
		currentEnergy += 300f;	// Energy recover that must happen at each checkpoing.
		target.Call(this.GetMethodSetVitality(), sword, null,
				maxHealth, currentHealth, maxEnergy, currentEnergy);
	}

	private void InitializeLevel()
	{
		nodeFactory.Call(this.GetMethodSetActive(), true);
		startTime = (int) OS.GetTicksMsec();
		globalData.Call(this.GetMethodPut(), "scene_to_load",
				this.GetScenePath(currentLevelScenePath));
		currentLevelProgress = this.Call<int>(globalData, this.GetMethodGet(),
				"level_progress");
	}

	private Spatial GetItem(Array<string> itemPrefabKeyList, int index)
	{
		if(itemPrefabKeyList != null && index > -1 &&
				index < itemPrefabKeyList.Count)
		{
			return this.Call<Spatial>(nodeFactory,
					this.GetMethodGet(), itemPrefabKeyList[index]);
		}

		return null;
	}

	public override void _Ready()
	{
		InitializeLevel();
		InitializePlayerCharacter();
		SpawnLevelManagedObjects();
		ActivateLevel(false);
	}

	public override void _Process(float delta)
	{
		AddRequestedNodesToTheScene();
	}

	public Node GlobalData
	{
		set
		{
			globalData = value;
		}
	}

	public Node NodeFactory
	{
		set
		{
			nodeFactory = value;
		}
	}

	public Spatial Target
	{
		set
		{
			target = value;
		}
	}

	public Vector3 PlayerStartingDirection
	{
		set
		{
			playerStartingDirection = value;
		}
	}

	public string CurrentLevelScenePath
	{
		set
		{
			currentLevelScenePath = value;
		}
	}

	public string NextLevelScenePath
	{
		set
		{
			nextLevelScenePath = value;
		}
	}

	public Array<string> BossPrefabKeyList
	{
		set
		{
			bossPrefabKeyList = value;
		}
	}

	public Array<Vector3> BossPositionList
	{
		set
		{
			bossPositionList = value;
		}
	}

	public Array<Vector3> FightAreaPositionList
	{
		set
		{
			fightAreaPositionList = value;
		}
	}

	public Array<Vector3> FightAreaTriggerPositionList
	{
		set
		{
			fightAreaTriggerPositionList = value;
		}
	}

	public Dictionary<int, Vector3> CheckpointPositionMap
	{
		set
		{
			checkpointPositionMap = value;
		}
	}


	[Export]
	public string loadScreenScenePath;

	[Export]
	public string fightAreaPrefabKey;

	[Export]
	public Array<string> swordPrefabKeyList;


	private Node globalData;
	private Node nodeFactory;
	private string currentLevelScenePath;
	private string nextLevelScenePath;
	private Spatial target;
	private Vector3 playerStartingDirection;
	private Array<string> bossPrefabKeyList;
	private Array<Vector3> bossPositionList;
	private Array<Vector3> fightAreaPositionList;
	private Array<Vector3> fightAreaTriggerPositionList;
	private Dictionary<int, Vector3> checkpointPositionMap;

	private int currentLevelProgress;
	private int startTime;

	private Spatial currentBoss;
	private Spatial currentFightArea;
}
