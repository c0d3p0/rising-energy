using Godot;


public class RhombusProjectileBehavior : BaseEnemyBehavior
{
	private void HandleMoveStep()
	{
		float posY = enemyCharacter.GlobalTransform.origin.y;

		if(desiredDirection.y == -1 && posY <= currentStepOriginY + yMoveRange.x ||
				desiredDirection.y == 1 && posY >= currentStepOriginY + yMoveRange.y)
		{
			currentIndex++;
			currentStepOriginY = enemyCharacter.Transform.origin.y;
			UpdateRhombusData();
		}

		desiredDirection.y = currentDirections[currentIndex];
	}

	private void UpdateRhombusData()
	{
		if(currentIndex < 0 || currentIndex >= currentDirections.Length)
		{
			currentIndex = 0;
			currentStepOriginY = enemyCharacter.Transform.origin.y;
			SetRhombusDirectionsByStyle(this.RandiRange(rng, 0, 3));
			desiredDirection.x = direction.x * this.RandfRange(rng, xSpeedFactorRange.x,
					xSpeedFactorRange.y);
		}
		else if(currentIndex == 2 || currentIndex == 4)
			desiredDirection.x *= -1;
	}

	private void SetRhombusDirectionsByStyle(int style)
	{
		switch(style)
		{
			case 0:
				SetCurrentDirections(1, -1, -1, 1, 1, -1);
				break;
			case 1:
				SetCurrentDirections(-1, 1, 1, -1, -1, 1);
				break;
			case 2:
				SetCurrentDirections(1, -1, -1, 1, -1, 1);
				break;
			case 3:
				SetCurrentDirections(-1, 1, 1, -1, 1, -1);
				break;
			default:
				SetCurrentDirections(1, -1, -1, 1, 1, -1);
				break;
		}	
	}

	private void SetCurrentDirections(sbyte a, sbyte b, sbyte c,
			sbyte d, sbyte e, sbyte f)
	{
		currentDirections[0] = a;
		currentDirections[1] = b;
		currentDirections[2] = c;
		currentDirections[3] = d;
		currentDirections[4] = e;
		currentDirections[5] = f;
	}

	private bool Move()
	{
		return this.EmitSignal<bool>(this, this.GetSignalMove(),
				desiredDirection);
	}

	private void ForceDefaultDirection()
	{
		if(direction.Length() == 0)
			direction.x = -1;
	}

	protected override void DecideWhatToDo()
	{
		HandleMoveStep();
		Move();
	}

	protected override void Initialize()
	{
		base.Initialize();
		currentIndex = -1;
		currentDirections = new sbyte[6];
	}

	public override void _Ready() {}

	public override Vector3 Direction
	{
		set
		{
			direction = value;
			currentIndex = -1;
			ForceDefaultDirection();
			UpdateRhombusData();
		}
	}

	
	[Export]
	public Vector2 xSpeedFactorRange = new Vector2(0.4f, 1f);

	[Export]
	public Vector2 yMoveRange = new Vector2(-1.5f, 1.5f);


	private float currentStepOriginY;
	private sbyte currentIndex;
	private Vector3 desiredDirection;

	private sbyte[] currentDirections;
}
