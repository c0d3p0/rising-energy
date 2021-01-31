using Godot;


public class BaseEnemyCharacterAction : BaseCharacterAction
{
	public virtual bool Move(Vector3 direction, Vector3 bodyDirection)
	{
		string t = CanTransitToMove();
		
		if(t != null)
		{
			EmitSignal(this.GetSignalApplyMove(), direction);
			FixBodyDirection(direction);

			if(direction.Length() > 0)
				animationStateMachine.Travel(t);
			else
				animationStateMachine.Travel("idle");

			return true;
		}

		return false;
	}

	public virtual bool Attack(Vector3 direction)
	{
		string t = CanTransitToAttack();
		
		if(t != null)
		{
			FixBodyDirection(direction);
			animationStateMachine.Travel(t);
			return true;
		}

		return false;
	}

	public virtual bool Attack(string attackAnimation, Vector3 direction)
	{
		string t = CanTransitToAttack();
		
		if(t != null)
		{
			FixBodyDirection(direction);
			animationStateMachine.Travel(attackAnimation);
			return true;
		}

		return false;
	}

	public void SelfDestroy()
	{
		enemyCharacter.GetParent().CallDeferred(
				this.GetGDMethodRemoveChild(), enemyCharacter);
	}

	public void StartDespawnTimer()	// Called by an animation
	{
		if(!despawnVisibilityNotifier.IsOnScreen())
			despawnTimer.Start(despawnTimeTolerance > 0 ? despawnTimeTolerance : -1f);
	}

	public void OnCameraEntered(Camera camera) // Used to control the character despawn
	{
		if(despawn)
			despawnTimer.Stop();
	}

	public void OnCameraExited(Camera camera) // Used to control the character despawn
	{
		if(despawn)
			despawnTimer.Start(despawnTimeTolerance > 0 ? despawnTimeTolerance : -1f);
	}

	public void OnDespawnTimeout() // Used to control the character despawn
	{
		if(despawn)
			SelfDestroy();
	}

	protected override void Initialize()
	{
		base.Initialize();
		enemyCharacter = GetNode<Spatial>(enemyCharacterNP);
		despawnVisibilityNotifier = GetNode<VisibilityNotifier>(
				despawnVisibilityNotifierNP);

		if(despawnTimerNP != null)
		{
			despawnTimer = GetNode<Timer>(despawnTimerNP);

			if(despawnTimeTolerance > 0f)
				despawnTimer.WaitTime = despawnTimeTolerance;
		}
	}

	public float DespawnTimeTolerance
	{
		set
		{
			despawnTimeTolerance = value;
		}
	}


	[Export]
	public NodePath enemyCharacterNP;

	[Export]
	public NodePath despawnTimerNP;

	[Export]
	public NodePath despawnVisibilityNotifierNP;

	[Export]
	public bool despawn = true;


	private float despawnTimeTolerance = -1f;

	protected Spatial enemyCharacter;
	protected Timer despawnTimer;
	protected VisibilityNotifier despawnVisibilityNotifier;
}
