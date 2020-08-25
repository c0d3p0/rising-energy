using Godot;


public class StandardBossBehavior : BaseBossBehavior
{
	protected virtual bool TryToIdle()
	{
		if(ShouldIdle() && 
				this.EmitSignal<bool>(this, this.GetSignalMove(), Vector3.Zero))
		{
			SetProcessBehavior(false);
			SetDirectionToTarget();
			return true;
		}

		return false;
	}

	protected void HandleSwitchSide()
	{
		if(!downRayCast.IsColliding() ||
				(frontRayCast.GetCollider() as Spatial) != null ||
				!IsTargetInTheSameDirection())
		{
			direction.x *= -1;
		}
	}

	protected override void DecideWhatToDo()
	{
		AddKinToTheScene();
		TryToRequestKin();
		HandleSwitchSide();

		if(TryToSpell() || TryToIdle() || TryToMove())
			return;
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


	[Export]
	public int idleTriggerRng = 20;
	
	[Export]
	public NodePath frontRayCastNP;

	[Export]
	public NodePath downRayCastNP;



	private RayCast frontRayCast;
	private RayCast downRayCast;
}
