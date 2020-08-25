using Godot;


public class WalkerEnemyBehavior : BaseEnemyBehavior
{
	public void OnMoveTimerTimeout()
	{
		PrepareNextWalk();
	}

	private void HandleSwitchSide()
	{
		if(!moveTimer.IsStopped() && (!downRayCast.IsColliding() ||
				(frontRayCast.GetCollider() as Spatial) != null))
		{
			direction.x *= -1;
		}
	}

	private bool TryToMove()
	{
		if(!moveTimer.IsStopped())
			return this.EmitSignal<bool>(this, this.GetSignalMove(), direction);

		return false;
	}

	private bool TryToIdle()
	{
		return this.EmitSignal<bool>(this, this.GetSignalMove(), Vector3.Zero);
	}

	private void PrepareNextWalk()
	{
		moveTimer.WaitTime = this.RandfRange(rng, 5, 8);
		SetDirectionToTarget();
	}

	protected override void DecideWhatToDo()
	{
		HandleSwitchSide();

		if(TryToMove() || TryToIdle())
			return;
	}

	protected override void Initialize()
	{
		base.Initialize();
		moveTimer = GetNode<Timer>(moveTimerNP);
		frontRayCast = GetNode<RayCast>(frontRayCastNP);
		downRayCast = GetNode<RayCast>(downRayCastNP);
	}

	public override void _Ready()
	{
		PrepareNextWalk();
		SetProcessBehavior(false);
	}


	[Export]
	public NodePath moveTimerNP;

	[Export]
	public NodePath frontRayCastNP;

	[Export]
	public NodePath downRayCastNP;


	private Timer moveTimer;
	private RayCast frontRayCast;
	private RayCast downRayCast;
}
