using Godot;
using Godot.Collections;


public class AirSpellerEnemyBehavior : BaseEnemyBehavior
{
	public void Spell()
	{
		if(IsInsideTree() && ! IsQueuedForDeletion())
		{
			currentProjectile.Translation = projectileSpot.GlobalTransform.origin;
			currentProjectile.Call(this.GetMethodSetDirection(), Vector3.Up);
			currentProjectile.Call(this.GetMethodTransitTo(), "spawn");
			projectileAmount--;
			spellTimer.Start(this.RandfRange(rng, spellTimeRange.x, spellTimeRange.y));
			currentProjectile = null;
			requestedProjectile = false;
		}
	}

	public void SetProjectileSpawnData(Node projectile, Dictionary paramMap)	// Called by NodeFactor
	{
		if(IsInsideTree() && ! IsQueuedForDeletion())
		{
			Spatial s = projectile as Spatial;
			s.Translation = new Vector3(0f, 0f, -1000f);
			currentProjectile = s;
		}
	}

	private bool TryToGiveUp()
	{
		if(projectileAmount <= 0)
			return this.EmitSignal<bool>(this, this.GetSignalMove(), Vector3.Up);
		
		return false;
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

	private bool TryToMove()
	{
		Vector3 tp = target.GlobalTransform.origin;
		Vector3 ep = enemyCharacter.GlobalTransform.origin;
		float h = tp.y + heightOverTarget;
		direction.x = tp.x < ep.x ? -1f : tp.x > ep.x ? 1 : -0.0001f;
		direction.y = h < ep.y ? -1f : h > ep.y ? 1 : 0f;
		return this.EmitSignal<bool>(this, this.GetSignalMove(), direction);
	}

	private bool ShouldSpell()
	{
		return !requestedProjectile && spellTimer.IsStopped() &&
				projectileAmount > 0;
	}

	protected override void DecideWhatToDo()
	{
		AddCurrentProjectileToTheScene();
		TryToRequestProjectile();

		if(TryToGiveUp() || TryToSpell() || TryToMove())
			return;
	}

	private void AddCurrentProjectileToTheScene()
	{
		if(currentProjectile != null && !currentProjectile.IsInsideTree())
			this.AddChildToEnemyContainer(this, currentProjectile);
	}

	protected override void Initialize()
	{
		base.Initialize();
		projectileSpot = GetNode<Spatial>(projectileSpotNP);
		spellTimer = GetNode<Timer>(spellTimerNP);
		projectileAmount = (byte) this.RandiRange(rng,
				(int) projectileAmountRange.x, (int) projectileAmountRange.y);
		spellTimer.Start(this.RandfRange(rng, spellTimeRange.x, spellTimeRange.y));
	}

	public override void _Ready()
	{
		SetDirectionToTarget();
		SetProcessBehavior(false);
	}
	

	[Export]
	public NodePath spellTimerNP;

	[Export]
	public NodePath projectileSpotNP;

	[Export]
	public string projectilePrefabKey;

	[Export]
	public float heightOverTarget = 5f;

	[Export]
	public Vector2 spellTimeRange = new Vector2(3f, 5f);

	[Export]
	public Vector2 projectileAmountRange = new Vector2(2f, 4f);


	private Timer spellTimer;
	private Spatial projectileSpot;
	private byte projectileAmount;

	private Spatial currentProjectile;
	private bool requestedProjectile;
}
