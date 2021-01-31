using Godot;


public class FloaterEnemyBehavior : BaseEnemyBehavior
{
	public void OnSpawn()	// Called by an animation
	{
		SetDirectionToTarget();
		enemyCharacter.Call(this.GetMethodFixBodyDirection(), direction);
	}

	public void PrepareMove()	// Called by an animation
	{
		Vector3 position = enemyCharacter.GlobalTransform.origin;

		if(this.RandiRange(rng, 0, 1) == 0)
			PrepareHorizontalMove(position);
		else
			PrepareVerticalMove(position);
	}

	private void PrepareVerticalMove(Vector3 position)
	{
		Vector3 targetPosition = target.GlobalTransform.origin;
		float bottomLimit = targetPosition.y + verticalMoveLimit.x;
		float topLimit = targetPosition.y + verticalMoveLimit.y;
		direction.x = 0;

		if(position.y <= bottomLimit)
			direction.y = 1;
		else if(position.y >= topLimit)
			direction.y = -1;
		else
			direction.y = (this.RandiRange(rng, 0, 1) * 2) - 1;

		float limitDistance = direction.y == -1 ? bottomLimit : topLimit;
		limitDistance = Mathf.Abs(position.y - limitDistance);
		desiredPoint.x = 0;
		desiredPoint.y = position.y + (direction.y * 
				this.RandfRange(rng, Mathf.Min(1f, limitDistance), limitDistance));
	}

	private void PrepareHorizontalMove(Vector3 position)
	{
		direction.x = (this.RandiRange(rng, 0, 1) * 2) - 1;
		direction.y = 0;
		desiredPoint.x = position.x + (direction.x *
				this.RandfRange(rng, horizontalMoveRange.x, horizontalMoveRange.y));
		desiredPoint.y = 0;
	}

	private bool TryToIdle()
	{
		Vector3 position = enemyCharacter.GlobalTransform.origin;

		if((direction.x == 1 && position.x >= desiredPoint.x) ||
				(direction.x == -1 && position.x <= desiredPoint.x) ||
				(direction.y == 1 && position.y >= desiredPoint.y) ||
				(direction.y == -1 && position.y <= desiredPoint.y))
		{
			direction.x = 0;
			direction.y = 0;
			return this.EmitSignal<bool>(this, this.GetSignalMove(), direction);
		}
		
		return false;
	}

	private bool Move()
	{
		return this.EmitSignal<bool>(this, this.GetSignalMove(), direction);
	}	

	protected override void DecideWhatToDo()
	{
		if(TryToIdle() || Move())
			return;
	}


	[Export]
	public Vector2 verticalMoveLimit = new Vector2(0f, 3f);

	[Export]
	public Vector2 horizontalMoveRange = new Vector2(1f, 5f);


	private Vector2 desiredPoint;
}