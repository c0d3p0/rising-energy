using Godot;
using Godot.Collections;


public class SurvivalLevelProgress : EnemySpawer
{
	public void FinishLevel()
	{
		SetProcess(false);
		globalData.Call(this.GetMethodPut(), "game_type", "survivalGame");
		globalData.Call(this.GetMethodPut(), "gameplay_result", true);
		globalData.Call(this.GetMethodPut(), "player_score",
				(scoreFromEnemies + scoreFromBosses).ToString());
		globalData.Call(this.GetMethodPut(), "scene_to_load",
				this.GetScenePath(nextLevelScenePath));
		GetTree().ChangeScene(this.GetScenePath(loadScreenScenePath));
	}

	public void DeactivateNodeFactory()	// Called by an animation
	{
		nodeFactory.Call(this.GetMethodSetActive(), false);
	}

	public void ActivateLevel(bool active)
	{
		target.Call(this.GetMethodActivateInput(), active);
		SetProcess(active);
	}

	public override void OnKinDestroyed(Spatial kin)
	{
		if(kin != null)
		{
			if(kin.IsInGroup("energy_source"))
				currentEnergySource = null;
			else if(this.Call<bool>(kin, this.GetMethodIsDead()))
			{
				string[] split = kin.Name.Split('_');

				if(split.Length > 0)
				{
					string enemyName = split[0].ToLower();

					if(bossKillScoreMap.ContainsKey(enemyName))
					{
						scoreFromBosses += bossKillScoreMap[enemyName];
						requestedBoss = false;
						currentBoss = null;
						SpawnEnergySource();
					}
					else if(enemyKillScoreMap.ContainsKey(enemyName))
						scoreFromEnemies += enemyKillScoreMap[enemyName];

					EmitScoreSignals();
				}
			}
		}	

		activeEnemyMap.Remove(kin.GetInstanceId());
	}

	public void SetBossEvilEnergySpawnData(Node evilEnergy, Dictionary paramMap)	// Called by NodeFactory
	{
		if(IsInsideTree() && !IsQueuedForDeletion() && 
				paramMap != null && paramMap.Contains(ENEMY_TRANSLATION) &&
				paramMap.Contains(EVIL_ENERGY_ROTATION))
		{
			Spatial ee = evilEnergy as Spatial;
			ee.Translation = (Vector3) paramMap[ENEMY_TRANSLATION];
			ee.RotationDegrees = (Vector3) paramMap[EVIL_ENERGY_ROTATION];
			currentBossEvilEnergy = ee;
		}
	}

	public void SetBossSpawnData(Node boss, Dictionary paramMap)	// Called by NodeFactory
	{
		if(IsInsideTree() && !IsQueuedForDeletion() && paramMap != null &&
				paramMap.Contains(ENEMY_TRANSLATION) && paramMap.Contains(DEFAULT_BOSS))
		{
			bool defaultBoss = (bool) paramMap[DEFAULT_BOSS];
			Spatial s = boss as Spatial;
			s.Translation = (Vector3) paramMap[ENEMY_TRANSLATION];
			s.Call(this.GetMethodSetTarget(), target);
			s.Call(this.GetMethodSetNodeFactory(), nodeFactory);
			s.Call(this.GetMethodSetKinSpawnPivot(), defaultBoss ?
					fightAreaPositionList[0] : fightAreaPositionList[1]);
			s.Connect(this.GetGDSignalTreeExited(), this,
					this.GetMethodOnKinDestroyed(), this.CreateSingleBind(boss));
			currentBoss = s;
		}
	}

	public void SetFightAreaSpawnData(Node fightArea, Dictionary paramMap)	// Called by NodeFactory
	{
		if(IsInsideTree() && !IsQueuedForDeletion() &&
				paramMap != null && paramMap.Contains(DEFAULT_BOSS))
		{
			bool defaultBoss = (bool) paramMap[DEFAULT_BOSS];
			Spatial fa = fightArea as Spatial;
			fa.Translation = defaultBoss ? fightAreaPositionList[0] : fightAreaPositionList[1];
			fa.Call(this.GetMethodSetNodeFactory(), nodeFactory);
			fa.Call(this.GetMethodSetTarget(), currentBoss);
			fa.Call(this.GetMethodSetTriggerPosition(), defaultBoss ?
					fightAreaTriggerPositionList[0] : fightAreaTriggerPositionList[1]);
			currentFightArea = fa;
		}
	}

	public void SetEnergySourceSpawnData(Node energySource, Dictionary paramMap)	// Called by NodeFactory
	{
		if(IsInsideTree() && !IsQueuedForDeletion() && currentEnergySource == null)
		{
			Spatial s = energySource as Spatial;
			s.Translation = energySourceSpawnPosition;
			s.Connect(this.GetGDSignalTreeExited(), this,
					this.GetMethodOnKinDestroyed(), this.CreateSingleBind(energySource));
			currentEnergySource = s;
		}
	}

	private void SpawnBoss()
	{
		if(ShouldSpawnBoss())
		{
			int bossIndex = this.RandiRange(rng, 0, bossPrefabKeyList.Count - 1);
			bool defaultBoss = bossIndex < bossPrefabKeyList.Count - 1;
			Vector3 faPos = defaultBoss ? fightAreaPositionList[0] : fightAreaPositionList[1];
			bossSpawned++;
			requestedBoss = true;

			Dictionary paramMap = new Dictionary();
			paramMap.Add(ENEMY_TRANSLATION, defaultBoss ? bossPositionList[0] : bossPositionList[1]);
			paramMap.Add(EVIL_ENERGY_ROTATION, Vector3.Zero);
			paramMap.Add(DEFAULT_BOSS, defaultBoss);

			nodeFactory.Call(this.GetMethodPut(), this, evilEnergyPrefabKey,
					this.GetMethodSetBossEvilEnergySpawnData(), paramMap);
			nodeFactory.Call(this.GetMethodPut(), this, bossPrefabKeyList[bossIndex],
					this.GetMethodSetBossSpawnData(), paramMap);
			nodeFactory.Call(this.GetMethodPut(), this, fightAreaPrefabKey,
					this.GetMethodSetFightAreaSpawnData(), paramMap);

			SpawnEnergySource();
		}
	}

	private void SpawnEnergySource()
	{
		if(currentEnergySource == null || !currentEnergySource.IsInsideTree())
		{
			int index = this.RandiRange(rng, 0, energySourcePrefabKeyList.Count - 1);
			nodeFactory.Call(this.GetMethodPut(), this, energySourcePrefabKeyList[index],
					this.GetMethodSetEnergySourceSpawnData(), null);
		}
	}

	private void EmitScoreSignals()
	{
		int totalScore = scoreFromEnemies + scoreFromBosses;
		int nextBoss = bossSpawnScoreFromEnemyInterval -
				(scoreFromEnemies % bossSpawnScoreFromEnemyInterval);
		this.EmitSignal(this.GetSignalPlayerScoreChanged(),
				"Score: " + totalScore.ToString());
		this.EmitSignal(this.GetSignalNextBossScoreChanged(),
				GetNextBossLabelText(nextBoss));
	}

	protected override bool ShouldSpawnEnemy()
	{
		return activeEnemyMap.Count < maximumActiveEnemies &&
				enemySpawnTimer.IsStopped() && spawnSpotMap.Count > 0 &&
				currentBoss == null;
	}

	private bool ShouldSpawnBoss()
	{
		return !requestedBoss && currentBoss == null &&
				(scoreFromEnemies / bossSpawnScoreFromEnemyInterval) > bossSpawned;
	}

	protected override void AddRequestedNodesToTheScene()
	{
		base.AddRequestedNodesToTheScene();

		if(currentEnergySource != null && !currentEnergySource.IsInsideTree())
			this.AddChildToEnemyContainer(this, currentEnergySource);

		if(currentBoss != null && !currentBoss.IsInsideTree())
		{
			if(currentBossEvilEnergy != null && !currentBossEvilEnergy.IsInsideTree())
			{
				this.AddChildToEnemyContainer(this, currentBossEvilEnergy);
				currentBossEvilEnergy = null;
			}

			this.AddChildToEnemyContainer(this, currentBoss);
		}

		if(currentFightArea != null && !currentFightArea.IsInsideTree())
		{
			this.AddChildToEnemyContainer(this, currentFightArea);
			currentFightArea = null;
		}
	}

	private Spatial GetItem(Array<string> itemPrefabKeyList, int index)
	{
		if(itemPrefabKeyList != null && index > -1 && index < itemPrefabKeyList.Count)
		{
			return this.Call<Node>(nodeFactory, this.GetMethodGet(),
					itemPrefabKeyList[index]) as Spatial;
		}

		return null;
	}

	private string GetNextBossLabelText(int nextBossPoints)
	{
		return this.CreateString("Next Boss: ", requestedBoss ||
				(scoreFromEnemies / bossSpawnScoreFromEnemyInterval) > bossSpawned ?
				"Active" : nextBossPoints.ToString());
	}
	
	private void InitializeLevel()
	{
		nodeFactory.Call(this.GetMethodSetActive(), true);
	}
	
	private void InitializePlayerCharacter()
	{
		float maxHealth = this.Call<float>(globalData,
				this.GetMethodGet(), "max_health");

		if(maxHealth == 0)
			maxHealth = 1;
		
		float currentHealth = maxHealth;
		int swordId = this.RandiRange(rng, 0, 2);
		float maxEnergy = 1000 + (swordId * 500);
		float currentEnergy = maxEnergy;
		Spatial sword = GetItem(swordPrefabKeyList, swordId);
		target.Call(this.GetMethodFixBodyDirection(), playerStartingDirection);
		target.Call(this.GetMethodSetVitality(), sword, null,
				maxHealth, currentHealth, maxEnergy, currentEnergy);
	}
	
	public override void _EnterTree()
	{
		Initialize();
	}

	public override void _Ready()
	{
		InitializeLevel();
		InitializePlayerCharacter();
		ActivateLevel(false);
		EmitScoreSignals();
	}

	public override void _Process(float delta)
	{
		AddRequestedNodesToTheScene();
		SpawnBoss();
		TryToSpawnEnemy();
	}

	public Node GlobalData
	{
		set
		{
			globalData = value;
		}
	}

	public string NextLevelScenePath
	{
		set
		{
			nextLevelScenePath = value;
		}
	}

	public Vector3 PlayerStartingDirection
	{
		set
		{
			playerStartingDirection = value;
		}
	}

	public Vector3 EnergySourceSpawnPosition
	{
		set
		{
			energySourceSpawnPosition = value;
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

	public Dictionary<string, int> EnemyKillScoreMap
	{
		set
		{
			enemyKillScoreMap = value;
		}
	}

	public Dictionary<string, int> BossKillScoreMap
	{
		set
		{
			bossKillScoreMap = value;
		}
	}

	public int BossSpawnScoreFromEnemyInterval
	{
		set
		{
			bossSpawnScoreFromEnemyInterval = value;
		}
	}


	[Export]
	public string loadScreenScenePath;

	[Export]
	public string fightAreaPrefabKey;

	[Export]
	public Array<string> swordPrefabKeyList;

	[Export]
	public Array<string> energySourcePrefabKeyList;


	private Node globalData;
	private string nextLevelScenePath;
	private Vector3 playerStartingDirection = new Vector3(1f, 0f, 0f);
	private Vector3 energySourceSpawnPosition = new Vector3(-21f, 2f, 0f);
	private Array<string> bossPrefabKeyList;	
	private Array<Vector3> bossPositionList;
	private Array<Vector3> fightAreaPositionList;
	private Array<Vector3> fightAreaTriggerPositionList;
	private Dictionary<string, int> enemyKillScoreMap;
	private Dictionary<string, int> bossKillScoreMap;
	private int bossSpawnScoreFromEnemyInterval;

	private int bossSpawned;
	private int scoreFromEnemies;
	private int scoreFromBosses;

	private bool requestedBoss;
	private Spatial currentBoss;
	private Spatial currentBossEvilEnergy;
	private Spatial currentFightArea;
	private Spatial currentEnergySource;


	private const int DEFAULT_BOSS = 2;
}
