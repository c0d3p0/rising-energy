using Godot;


public class BoomerangProjetileBehavior : BaseAreaProjectileBehavior
{
	private void ChangeDirection()
	{
		if(!returning)
		{
			Vector3 p = enemyCharacter.GlobalTransform.origin;

			if(moveAxis.x != 0 && ((direction.x == -1 && p.x < limit.x) ||
					(direction.x == 1 && p.x > limit.x)))
			{
				direction.x *= -1f;
				returning = true;
			}
		
			if(moveAxis.y != 0 && (direction.y == -1 && p.y < limit.y) ||
					(direction.y == 1 && p.y > limit.y))
			{
				direction.y *= -1f;
				returning = true;
			}
		}
	}

	protected override void Reposition()
	{
		if(repositionEnabled)
		{
			Vector3 tp = target.GlobalTransform.origin;
			Vector3 pp = enemyCharacter.Translation;
			float min = tp.x - repositionTargetRange.x;
			min = min < repositionPivot.x - repositionRange.x ? 
					repositionPivot.x - repositionRange.x : min;
			float max = tp.x + repositionTargetRange.x;
			max = max > repositionPivot.x + repositionRange.x ? 
					repositionPivot.x + repositionRange.x : max;
			float rX = this.RandfRange(rng, min, max);
			float rY = this.RandfRange(rng, 0f, repositionRange.y);
			enemyCharacter.Translation = new Vector3(rX, pp.y, pp.z);
		}
	}

	private void UpdateDirection()
	{
		if(direction.x != 0)
			direction.x = moveAxis.x != 0f ? direction.x * moveAxis.x : direction.x;
		else
			direction.x = moveAxis.x != 0f ? moveAxis.x : -1f;

		if(direction.y != 0)
			direction.y = direction.y * moveAxis.y;
		else
			direction.y = moveAxis.y;
	}

	private bool Move()
	{
		if(this.EmitSignal<bool>(this, this.GetSignalMove(), direction))
		{
			ChangeDirection();
			return true;
		}

		return false;
	}

	private void DefineLimit()
	{
		float d = this.RandfRange(rng, moveToleranceRange.x, moveToleranceRange.y);
		limit = target.GlobalTransform.origin + (direction * new Vector3(d, d, 0f));
	}

	protected override void DecideWhatToDo()
	{
		Move();
	}

	public override void _Ready() {}

	public override Vector3 Direction
	{
		set
		{
			direction = value;
			UpdateDirection();
			DefineLimit();
		}
	}


	[Export]
	public Vector3 moveAxis = new Vector3(1.0f, 0.0f, 0.0f);

	[Export]
	public Vector3 repositionTargetRange = new Vector3(4.0f, 0.0f, 0.0f);

	[Export]
	public Vector2 moveToleranceRange = new Vector2(0f, 4f);


	private Vector3 limit;
	private bool returning;
}
