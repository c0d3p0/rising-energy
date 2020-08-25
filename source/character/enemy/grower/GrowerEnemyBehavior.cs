using Godot;


public class GrowerEnemyBehavior : BaseEnemyBehavior
{
	private void HandleSwitchSide()
	{
		if(!downRayCast.IsColliding() ||
				(frontRayCast.GetCollider() as Spatial) != null)
		{
			direction.x *= -1;
		}
	}

	private bool TryToGrow()
	{
		if(ShouldGrow())
		{
			if(this.EmitSignal<bool>(this, this.GetSignalAttack(), direction))
			{
				SetProcessBehavior(false);
				SetDirectionToTarget();
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
		HandleSwitchSide();

		if(TryToGrow() || TryToIdle() || TryToMove())
			return;
	}

	private bool ShouldGrow()
	{
		float distanceToPlayer = Mathf.Abs(target.GlobalTransform.origin.x -
				enemyCharacter.GlobalTransform.origin.x);

		return distanceToPlayer <=  growTriggerDistance &&
				this.RandiRange(rng, rngMinValue, rngMaxValue) <= growTriggerRng;
	}

	private bool ShouldIdle()
	{
		return this.RandiRange(rng, rngMinValue, rngMaxValue)
				<= idleTriggerRng;
	}

	protected override void Initialize()
	{
		base.Initialize();
		frontRayCast = GetNode<RayCast>(frontRayCastNP);
		downRayCast = GetNode<RayCast>(downRayCastNP);
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
	public float growTriggerDistance = 2f;

	[Export]
	public int rngMinValue = 1;

	[Export]
	public int rngMaxValue = 10000;

	[Export]
	public int growTriggerRng = 250;

	[Export]
	public int idleTriggerRng = 20;


	private RayCast frontRayCast;
	private RayCast downRayCast;
}

