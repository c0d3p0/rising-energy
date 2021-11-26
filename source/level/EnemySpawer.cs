using SCG = System.Collections.Generic;

using Godot;
using Godot.Collections;


public class EnemySpawer : Node
{
	public void SetEnemySpawnData(Node enemy, Dictionary paramMap)	// Called by NodeFactory
	{
		if(IsInsideTree() && !IsQueuedForDeletion() &&
				paramMap != null && paramMap.Contains(ENEMY_TRANSLATION))
		{
			Spatial e = enemy as Spatial;
			e.Translation = ((Vector3) paramMap[ENEMY_TRANSLATION]) + enemySpawnOffset;
			e.Call(this.GetMethodSetTarget(), target);
			e.Call(this.GetMethodSetNodeFactory(), nodeFactory);
			e.Connect(this.GetGDSignalTreeExited(), this,
					this.GetMethodOnKinDestroyed(), this.CreateSingleBind(e));
			e.Connect(this.GetGDSignalReady(), e,
					this.GetMethodTransitTo(), this.CreateSingleBind("spawn"));
			activeEnemyMap.Add(e.GetInstanceId().ToString(), e);
			currentEnemy = e;
		}
	}
	
	public void SetEvilEnergySpawnData(Node evilEnergy, Dictionary paramMap)	// Called by NodeFactory
	{
		if(IsInsideTree() && !IsQueuedForDeletion() && 
				paramMap != null && paramMap.Contains(ENEMY_TRANSLATION) &&
				paramMap.Contains(EVIL_ENERGY_ROTATION))
		{
			Spatial ee = evilEnergy as Spatial;
			ee.Translation = (Vector3) paramMap[ENEMY_TRANSLATION];
			ee.RotationDegrees = (Vector3) paramMap[EVIL_ENERGY_ROTATION];
			currentEvilEnergy = ee;
		}
	}
	
	public void AddEnemySpawnSpot(VisibilityNotifier spawnSpot)
	{
		string iid = spawnSpot.GetInstanceId().ToString();

		if(!spawnSpotMap.ContainsKey(iid))
			spawnSpotMap.Add(iid, spawnSpot);
	}

	public void RemoveEnemySpawnSpot(VisibilityNotifier spawnSpot)
	{
		string iid = spawnSpot.GetInstanceId().ToString();

		if(spawnSpotMap.ContainsKey(iid))
			spawnSpotMap.Remove(iid);
	}

	public virtual void OnKinDestroyed(Spatial enemy)
	{
		activeEnemyMap.Remove(enemy.GetInstanceId().ToString());
	}

	protected void TryToSpawnEnemy()
	{
		if(ShouldSpawnEnemy())
		{
			UpdateClosestSpawnData();
			int min = IsNotEmptyList(landEnemyPrefabKeyList) ? 0 : 1;
			int max = IsNotEmptyList(airEnemyPrefabKeyList) ? 1 : 0;
			int enemyType = this.RandiRange(rng, min, max);
			enemySpawnTimer.Start(this.RandfRange(rng,
					spawnDelayRange.x, spawnDelayRange.y));

			if(enemyType == 0)
				SpawnLandEnemy();
			if(enemyType == 1)
				SpawnAirEnemy();
		}
	}

	protected void SpawnLandEnemy()
	{
		int enemyIndex = this.RandiRange(rng, 0, landEnemyPrefabKeyList.Count - 1);
		Vector3 ep = new Vector3(this.RandfRange(rng, spawnRangeX.x,
				spawnRangeX.y), spawnRangeY.x, 0f);

		Dictionary paramMap = new Dictionary();
		paramMap.Add(ENEMY_TRANSLATION, ep);
		paramMap.Add(EVIL_ENERGY_ROTATION, Vector3.Zero);

		nodeFactory.Call(this.GetMethodPut(), this, evilEnergyPrefabKey,
				this.GetMethodSetEvilEnergySpawnData(), paramMap);
		nodeFactory.Call(this.GetMethodPut(), this, landEnemyPrefabKeyList[enemyIndex],
				this.GetMethodSetEnemySpawnData(), paramMap);
	}

	protected void SpawnAirEnemy()
	{
		int enemyIndex = this.RandiRange(rng, 0, airEnemyPrefabKeyList.Count - 1);
		Vector3 ep = new Vector3(
				this.RandfRange(rng, spawnRangeX.x, spawnRangeX.y),
				this.RandfRange(rng, spawnRangeY.x, spawnRangeY.y), 0f);

		Dictionary paramMap = new Dictionary();
		paramMap.Add(ENEMY_TRANSLATION, ep);
		paramMap.Add(EVIL_ENERGY_ROTATION, new Vector3(90f, 0f, 0f));

		nodeFactory.Call(this.GetMethodPut(), this, evilEnergyPrefabKey,
				this.GetMethodSetEvilEnergySpawnData(), paramMap);
		nodeFactory.Call(this.GetMethodPut(), this, airEnemyPrefabKeyList[enemyIndex],
				this.GetMethodSetEnemySpawnData(), paramMap);
	}

	protected void UpdateClosestSpawnData()
	{
		closestSpawn = null;
		VisibilityNotifier itSpot = null;
		Vector2 closestDistance = new Vector2();
		Vector2 itDistance;
		SCG.IEnumerator<SCG.KeyValuePair<string, VisibilityNotifier>> it =
				spawnSpotMap.GetEnumerator();

		if(it.MoveNext())
		{	
			closestSpawn = it.Current.Value;
			closestDistance = GetTargetDistance(closestSpawn);
		}

		while(it.MoveNext())
		{
			itSpot = it.Current.Value;
			itDistance = GetTargetDistance(itSpot);

			if(itDistance.x < closestDistance.x || itDistance.y < closestDistance.y)
			{
				closestSpawn = itSpot;
				closestDistance = itDistance;
			}	
		}

		UpdateSpawnRanges(closestSpawn);
	}

	protected Vector2 GetTargetDistance(VisibilityNotifier spawnSpot)
	{
		Vector3 p = spawnSpot.GlobalTransform.origin;
		Vector3 tp = target.GlobalTransform.origin;
		Vector3 size = spawnSpot.Aabb.Size;
		float hs = (size.x / 2f);
		return new Vector2(Mathf.Abs(tp.x - (p.x - hs)), Mathf.Abs(tp.x - (p.x + hs)));
	}

	protected void UpdateSpawnRanges(VisibilityNotifier spawnSpot)
	{
		Vector3 p = spawnSpot.GlobalTransform.origin;
		Vector3 size = spawnSpot.Aabb.Size;
		Vector3 tp = target.GlobalTransform.origin;
		float hsx = (size.x / 2f);
		float hsy = (size.y / 2f);
		float xMin = p.x - hsx;
		float xMax = p.x + hsx;
		float dMin = tp.x + spawnDistanceRange.x;
		float dMax = tp.x + spawnDistanceRange.y;

		if(dMin > xMax)
			xMin = xMax;
		else if(dMin > xMin)
			xMin = dMin;
		
		if(dMax < xMin)
			xMax = xMin;
		else if(dMax < xMax)
			xMax = dMax;

		spawnRangeX.x = xMin;
		spawnRangeX.y = xMax;

		spawnRangeY.x = p.y - hsy;
		spawnRangeY.y = p.y + hsy;
	}

	protected virtual bool ShouldSpawnEnemy()
	{
		return !IsQueuedForDeletion() && IsInsideTree() &&
				enemySpawnTimer.IsStopped() && spawnSpotMap.Count > 0 &&
				activeEnemyMap.Count < maximumActiveEnemies &&
				(landEnemyPrefabKeyList != null || airEnemyPrefabKeyList != null);
	}

	protected virtual void AddRequestedNodesToTheScene()
	{
		if(currentEvilEnergy != null && !currentEvilEnergy.IsInsideTree() &&
				currentEnemy != null && !currentEnemy.IsInsideTree())
		{
			this.AddChildToEnemyContainer(this, currentEvilEnergy);
			this.AddChildToEnemyContainer(this, currentEnemy);
			currentEvilEnergy = null;
			currentEnemy = null;
		}
	}

	private bool IsNotEmptyList(Array<string> list)
	{
		return list != null && list.Count > 0;
	}

	protected void Initialize()
	{
		spawnSpotMap = new Dictionary<string, VisibilityNotifier>();
		activeEnemyMap = new Dictionary<string, Spatial>();
		enemySpawnTimer = GetNode<Timer>(enemySpawnTimerNP);
		rng = new RandomNumberGenerator();
		enemySpawnTimer.WaitTime = this.RandfRange(rng,
				spawnDelayRange.x, spawnDelayRange.y);
	}

	public override void _EnterTree()
	{
		Initialize();
	}

	public override void _Process(float delta)
	{
		AddRequestedNodesToTheScene();
		TryToSpawnEnemy();
	}

	public Spatial Target
	{
		set
		{
			target = value;
		}
	}

	public Node NodeFactory
	{
		set
		{
			nodeFactory = value;
		}
	}

	public int MaximumActiveEnemies
	{
		set
		{
			maximumActiveEnemies = value;
		}
	}

	public Array<string> LandEnemyPrefabKeyList
	{
		set
		{
			landEnemyPrefabKeyList = value;
		}
	}

	public Array<string> AirEnemyPrefabKeyList
	{
		set
		{
			airEnemyPrefabKeyList = value;
		}
	}


	[Export]
	public NodePath enemySpawnTimerNP;

	[Export]
	public string evilEnergyPrefabKey;

	[Export]
	public Vector2 spawnDelayRange = new Vector2(2f, 4f);

	[Export]
	public Vector2 spawnDistanceRange = new Vector2(-4f, 8f);

	[Export]
	public Vector3 enemySpawnOffset = new Vector3(0f, 0f, 0f);


	protected Spatial target;
	protected Node nodeFactory;
	protected Array<string> landEnemyPrefabKeyList;
	protected Array<string> airEnemyPrefabKeyList;
	protected int maximumActiveEnemies = 3;
	protected Timer enemySpawnTimer;
	protected Dictionary<string, VisibilityNotifier> spawnSpotMap;
	protected Dictionary<string, Spatial> activeEnemyMap;
	protected RandomNumberGenerator rng;
	protected VisibilityNotifier closestSpawn;
	protected Vector2 spawnRangeX;
	protected Vector2 spawnRangeY;


	protected Spatial currentEnemy;
	protected Spatial currentEvilEnergy;

	protected const int ENEMY_TRANSLATION = 0;
	protected const int EVIL_ENERGY_ROTATION = 1;
}
