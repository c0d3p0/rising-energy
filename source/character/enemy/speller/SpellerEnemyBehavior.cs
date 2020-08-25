using Godot;
using Godot.Collections;


public class SpellerEnemyBehavior : BaseEnemyBehavior
{
	public void Spell()
	{
		if(IsInsideTree() && ! IsQueuedForDeletion())
		{
			currentProjectile.Translation = projectileSpot.GlobalTransform.origin;
			currentProjectile.Call(this.GetMethodSetDirection(), direction);
			currentProjectile.Call(this.GetMethodTransitTo(), "spawn");
			activeProjectileMap.Add(currentProjectile.GetInstanceId(), currentProjectile);
			currentProjectile = null;
			requestedProjectile = false;
		}
	}

	public void SetProjectileSpawnData(Node projectile, Dictionary paramMap)	// Called by NodeFactor
	{
		if(IsInsideTree() && !IsQueuedForDeletion())
		{
			Spatial s = projectile as Spatial;
			s.Translation = new Vector3(0f, 0f, -1000f);
			s.Connect(this.GetGDSignalTreeExited(), this,
					this.GetMethodOnKinDestroyed(), this.CreateSingleBind(s));
			currentProjectile = s;
		}
	}

	public void OnDirectionTimeout()
	{
		SetDirectionToTarget();
	}

	public void OnKinDestroyed(Spatial projectile)
	{
		activeProjectileMap.Remove(projectile.GetInstanceId());
	}

	private void HandleSwitchSide()
	{
		if(!downRayCast.IsColliding() ||
				(frontRayCast.GetCollider() as Spatial) != null)
		{
			direction.x *= -1;
		}
	}

	private void TryToRequestProjectile()
	{
		if(ShouldSpell())
		{
			requestedProjectile = true;
			nodeFactory.Call(this.GetMethodPut(), this, projectilePrefabKey,
					this.GetMethodSetProjectileSpawnData(), null);
		}	
	}

	private bool TryToSpell()
	{
		if(currentProjectile != null && currentProjectile.IsInsideTree())
		{
			if(this.EmitSignal<bool>(this, this.GetSignalAttack(), direction))
			{
				SetProcessBehavior(false);
				return true;
			}
		}

		return false;
	}

	private bool TryToIdle()
	{
		if(ShouldIdle())
		{
			if(this.EmitSignal<bool>(this, this.GetSignalMove(), Vector3.Zero))
			{
				SetProcessBehavior(false);
				SetDirectionToTarget();
				return true;
			}		
		}

		return false;
	}

	private bool TryToMove()
	{
		return this.EmitSignal<bool>(this, this.GetSignalMove(), direction);
	}

	protected override void DecideWhatToDo()
	{
		AddCurrentProjectileToTheScene();
		TryToRequestProjectile();
		HandleSwitchSide();

		if(TryToSpell() || TryToIdle() || TryToMove())
			return;
	}

	private bool IsTargetInTheSameDirection()
	{
		float enemyX = enemyCharacter.GlobalTransform.origin.x;
		float targetX = target.GlobalTransform.origin.x;
		float targetDirection = (enemyX - targetX > 0f ? -1f : 1f);
		return direction.x == targetDirection;
	}

	private bool ShouldSpell()
	{
		return activeProjectileMap.Count < maxActiveProjectiles &&
				IsInsideTree() && !requestedProjectile &&
				IsTargetInTheSameDirection() && 
				this.RandiRange(rng, rngMinValue, rngMaxValue) <= spellTriggerRng;
	}

	private bool ShouldIdle()
	{
		return this.RandiRange(rng, rngMinValue, rngMaxValue)
				<= idleTriggerRng;
	}

	private void AddCurrentProjectileToTheScene()
	{
		if(currentProjectile != null && !currentProjectile.IsInsideTree())
			this.AddChildToEnemyContainer(this, currentProjectile);
	}

	protected override void Initialize()
	{
		base.Initialize();
		frontRayCast = GetNode<RayCast>(frontRayCastNP);
		downRayCast = GetNode<RayCast>(downRayCastNP);
		projectileSpot = GetNode<Spatial>(projectileSpotNP);
		activeProjectileMap = new Dictionary<ulong, Spatial>();
	}

	public override void _Ready()
	{
		SetDirectionToTarget();
		SetProcessBehavior(false);
	}


	[Export]
	public NodePath frontRayCastNP;

	[Export]
	public NodePath downRayCastNP;

	[Export]
	public NodePath projectileSpotNP;

	[Export]
	public string projectilePrefabKey;

	[Export]
	public int maxActiveProjectiles = 2;

	[Export]
	public int rngMinValue = 1;

	[Export]
	public int rngMaxValue = 10000;

	[Export]
	public int spellTriggerRng = 250;

	[Export]
	public int idleTriggerRng = 20;


	private RayCast frontRayCast;
	private RayCast downRayCast;
	private Spatial projectileSpot;
	private Spatial currentProjectile;
	private bool requestedProjectile;
	private Dictionary<ulong, Spatial> activeProjectileMap;
}
