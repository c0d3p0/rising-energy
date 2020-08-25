using Godot;


public class FollowerEnemyBehavior : BaseEnemyBehavior
{
	private bool TryToStopMoving()
	{
		return IsTargetInTheOppositeDirection() &&
				this.EmitSignal<bool>(this, this.GetSignalMove(), Vector3.Zero);
	}

	private bool TryToMove()
	{
		return this.EmitSignal<bool>(this,
				this.GetSignalMove(), direction);
	}

	protected override void DecideWhatToDo()
	{
		if(TryToStopMoving() || TryToMove())
			return;
	}

	private bool IsTargetInTheOppositeDirection()
	{
		float enemyX = enemyCharacter.GlobalTransform.origin.x;
		float targetX = target.GlobalTransform.origin.x;
		float targetDirection = (enemyX - targetX > 0f ? -1f : 1f);
		return direction.x != targetDirection;
	}

	public override void _Ready()
	{
		SetDirectionToTarget();
		SetProcessBehavior(false);
	}
}
