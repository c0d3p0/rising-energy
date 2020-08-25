using Godot;


public class WaverEnemyBehavior : BaseEnemyBehavior
{
	private void Move()
	{
		FixDirection();
		HandleChangeDirection();
		directionPercentage.x = horizontalSpeedPercentage * direction.x;
		directionPercentage.y = direction.y;
		this.EmitSignal<bool>(this, this.GetSignalMove(), directionPercentage);		
	}	

	private void UpdateMovementValues()
	{
		if(target != null)
			sourcePoint = target.GlobalTransform.origin.y;
		else
			sourcePoint = 0f;
		
		maxLimit = sourcePoint + this.RandiRange(rng,
				(int) Mathf.Round(topLimitRange.x),
				(int) Mathf.Round(topLimitRange.y));
		minLimit = sourcePoint + this.RandiRange(rng,
				(int) Mathf.Round(bottomLimitRange.x),
				(int) Mathf.Round(bottomLimitRange.y));
		horizontalSpeedPercentage = this.RandfRange(rng,
				horizontalSpeedPercentageRange.x,
				horizontalSpeedPercentageRange.y);
	}

	private void FixDirection()
	{
		if(direction.y == 0)
			direction.y = (this.RandiRange(rng, 0, 1) * 2) - 1;
	}

	private void HandleChangeDirection()
	{
		float posY = enemyCharacter.GlobalTransform.origin.y;

		if(direction.y == 1 && posY >= maxLimit)
		{	
			direction.y = -1;
			UpdateMovementValues();
		}
		else if(direction.y == -1 && posY <= minLimit)
		{
			direction.y = 1;
			UpdateMovementValues();
		}
	}
	
	protected override void DecideWhatToDo()
	{
		Move();
	}

	public override void _Ready()
	{
		SetDirectionToTarget();
		UpdateMovementValues();
	}


	[Export]
	public Vector2 topLimitRange = new Vector2(2f, 4f);
	
	[Export]
	public Vector2 bottomLimitRange = new Vector2(-1f, 1f);

	[Export]
	public Vector2 horizontalSpeedPercentageRange = new Vector2(0.1f, 1f);


	private float sourcePoint;
	private float horizontalSpeedPercentage;
	private float maxLimit;
	private float minLimit;
	private Vector3 directionPercentage;
}
