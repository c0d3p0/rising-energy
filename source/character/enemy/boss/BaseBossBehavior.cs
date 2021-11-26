using SCG = System.Collections.Generic;

using Godot;
using Godot.Collections;


public abstract class BaseBossBehavior : BaseEnemyBehavior
{
	public virtual void Spell()	// Called by an animation
	{
		if(IsInsideTree() && ! IsQueuedForDeletion())
		{
			if(projectilePrefabKeyMap.ContainsKey(kinKeys[kinId]))
				SpellProjectile();
			else if(kinPrefabKeyMap.ContainsKey(kinKeys[kinId]))
				SpellKin();
		}
	}

	public virtual void SetProjectileSpawnData(Node projectile, Dictionary paramMap)	// Called by NodeFactory
	{
		SetDefaultSpawnData(projectile, paramMap);
	}

	public virtual void SetKinSpawnData(Node enemy, Dictionary paramMap)	// Called by NodeFactory
	{
		enemy.Call(this.GetMethodSetNodeFactory(), nodeFactory);
		enemy.Call(this.GetMethodSetDespawnTimeTolerance(), kinDespawnTimeTolerance);
		SetDefaultSpawnData(enemy, paramMap);
	}

	public virtual void SetDefaultSpawnData(Node kin, Dictionary paramMap)
	{
		if(IsInsideTree() && !IsQueuedForDeletion())
		{
			Spatial s = kin as Spatial;
			s.Translation = new Vector3(-10000f, -10000f, -5f);
			s.Call(this.GetMethodSetTarget(), target);
			s.Connect(this.GetGDSignalTreeExited(), this,
					this.GetMethodOnKinDestroyed(), this.CreateSingleBind(s));
			currentKin = s;
		}
	}

	public void OnKinDestroyed(Spatial kin)
	{
		activeKinMap.Remove(kin.GetInstanceId().ToString());
	}

	protected void TryToRequestKin()
	{
		if(ShouldSpell())
		{
			requestedKin = true;
			
			if(projectilePrefabKeyMap.ContainsKey(kinKeys[kinId]))
			{
				nodeFactory.Call(this.GetMethodPut(), this, kinKeys[kinId],
						this.GetMethodSetProjectileSpawnData(), null);
			}
			else if(kinPrefabKeyMap.ContainsKey(kinKeys[kinId]))
			{
				nodeFactory.Call(this.GetMethodPut(), this, kinKeys[kinId],
						this.GetMethodSetKinSpawnData(), null);
			}
		}	
	}

	protected virtual bool TryToSpell()
	{
		if(currentKin != null && currentKin.IsInsideTree())
		{
			if(this.EmitSignal<bool>(this, this.GetSignalAttack(), direction))
			{
				SetProcessBehavior(false);
				return true;
			}
		}

		return false;
	}

	protected bool TryToMove()
	{
		return this.EmitSignal<bool>(this, this.GetSignalMove(), direction);
	}

	protected virtual void SpellKin()
	{
		float posX = this.RandfRange(rng, kinSpawnPivot.x - kinSpawnRange.x,
				kinSpawnPivot.x + kinSpawnRange.x);
		float posY = this.RandfRange(rng, kinSpawnPivot.y,
				kinSpawnPivot.y + kinSpawnRange.y);
		currentKin.Translation = new Vector3(posX, posY, kinSpawnPivot.z);
		currentKin.Call(this.GetMethodTransitTo(), "spawn");
		activeKinMap.Add(currentKin.GetInstanceId().ToString(), currentKin);
		currentKin = null;
		requestedKin = false;
	}

	protected virtual void SpellProjectile()
	{
		currentKin.Translation = projectileSpotMap[kinKeys[kinId]].GlobalTransform.origin;
		currentKin.Call(this.GetMethodSetDirection(), direction);
		currentKin.Call(this.GetMethodSetRepositionPivot(), kinSpawnPivot);
		currentKin.Call(this.GetMethodTransitTo(), "spawn");
		activeKinMap.Add(currentKin.GetInstanceId().ToString(), currentKin);
		currentKin = null;
		requestedKin = false;
	}

	protected bool IsTargetInTheSameDirection()
	{
		float enemyX = enemyCharacter.GlobalTransform.origin.x;
		float targetX = target.GlobalTransform.origin.x;
		float targetDirection = (enemyX - targetX > 0f ? -1f : 1f);
		return direction.x == targetDirection;
	}

	protected bool ShouldSpell()
	{
		if(currentKin == null && !requestedKin &&
				activeKinMap.Count < maxActiveKins &&
				IsTargetInTheSameDirection() &&
				this.RandiRange(rng, rngMinValue, rngMaxValue) <= spellTriggerRng)
		{
			kinId = this.RandiRange(rng, 0, kinKeys.Length - 1);
			return this.EmitSignal<bool>(this, this.GetSignalCanAttack(), kinKeys[kinId]);
		}

		return false;
	}

	protected void AddKinToTheScene()
	{
		if(currentKin != null && !currentKin.IsInsideTree())
			this.AddChildToEnemyContainer(this, currentKin);
	}
	
	protected void InitializeKinData()
	{
		AddPrefabKeyListToMap(kinPrefabKeyList, kinPrefabKeyMap);
		AddPrefabKeyListToMap(projectilePrefabKeyList, projectilePrefabKeyMap);
		CreateKinKeys();
	}

	private void AddPrefabKeyListToMap(Array<string> keyList,
			Dictionary<string, object> prefabKeyMap)
	{
		if(keyList != null)
		{
			SCG.IEnumerator<string> it = keyList.GetEnumerator();

			while(it.MoveNext())
				prefabKeyMap.Add(it.Current, null);
		}
	}

	private void CreateKinKeys()
	{
		Array<string> keyList = new Array<string>();
		SCG.IEnumerator<string> it = kinPrefabKeyMap.Keys.GetEnumerator();

		while(it.MoveNext())
			keyList.Add(it.Current);

		it = projectilePrefabKeyMap.Keys.GetEnumerator();

		while(it.MoveNext())
			keyList.Add(it.Current);

		kinKeys = new string[keyList.Count];

		for(int i = 0; i < keyList.Count; i++)
			kinKeys[i] = keyList[i];
	}

	protected override void Initialize()
	{
		base.Initialize();
		kinPrefabKeyMap = new Dictionary<string, object>();
		projectilePrefabKeyMap = new Dictionary<string, object>();
		activeKinMap = new Dictionary<string, Spatial>();
		projectileSpotMap = this.GetNodeMap<string, Spatial>(
					this, projectileSpotNPMap);
		InitializeKinData();
	}

	public override void _Ready()
	{
		SetDirectionToTarget();
		SetProcessBehavior(false);
	}

	public Vector3 KinSpawnPivot
	{
		set
		{
			kinSpawnPivot = value;
		}	
	}


	[Export]
	public Array<string> kinPrefabKeyList;

	[Export]
	public Array<string> projectilePrefabKeyList;

	[Export]
	public Dictionary<string, NodePath> projectileSpotNPMap;

	[Export]
	public Vector2 kinSpawnRange = new Vector2(6f, 0f);

	[Export]
	public int maxActiveKins = 2;

	[Export]
	public int rngMinValue = 1;

	[Export]
	public int rngMaxValue = 10000;

	[Export]
	public int spellTriggerRng = 350;

	[Export]
	public float kinDespawnTimeTolerance = 2f;


	protected Dictionary<string, Spatial> projectileSpotMap;
	protected Dictionary<string, object> kinPrefabKeyMap;
	protected Dictionary<string, object> projectilePrefabKeyMap;
	protected Dictionary<string, Spatial> activeKinMap;
	protected string[] kinKeys;
	protected Vector3 kinSpawnPivot;
	protected int kinId;
	protected Spatial currentKin;
	protected bool requestedKin;
}
