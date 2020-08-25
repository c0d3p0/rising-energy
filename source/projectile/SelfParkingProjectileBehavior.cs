using Godot;


public class SelfParkingProjectileBehavior : BaseEnemyBehavior
{
	public void OnParkingTimeout()
	{
		SetGoAwayDesiredSpot();
	}

	private bool TryToMove()
	{
		if(direction.Length() > 0.0f)
			return this.EmitSignal<bool>(this, this.GetSignalMove(), direction);

		return false;
	}

	private bool TryToPark()
	{
		if(direction.Length() == 0.0f)
		{
			if(parkingTimer.IsStopped())
				parkingTimer.Start();

			return this.EmitSignal<bool>(this, this.GetSignalMove(), Vector3.Zero);
		}

		return false;
	}

	private void RandomizeParkingSpotAndTime()
	{
		Vector3 p = enemyCharacter.GlobalTransform.origin;
		float spX = p.x + (direction.x * this.RandfRange(rng, xMoveRange.x, xMoveRange.y));
		float spY = p.y + this.RandfRange(rng, yMoveRange.x, yMoveRange.y);
		desiredSpot = new Vector3(spX, spY, 0f);
		direction.y = p.y > desiredSpot.y ? -1 : 1;
		parkingTimer.WaitTime = this.RandfRange(rng,
				parkingTimeRange.x, parkingTimeRange.y);
	}

	private void SetGoAwayDesiredSpot()
	{
		Vector3 p = enemyCharacter.GlobalTransform.origin;
		desiredSpot = new Vector3(p.x, 3000.0f, 0f);
		direction.y = 1f;		
	}

	private void FixMoveDirection()
	{
		Vector3 p = enemyCharacter.GlobalTransform.origin;

		if(direction.x == -1 && p.x <= desiredSpot.x)
			direction.x = 0;

		if(direction.x == 1 && p.x >= desiredSpot.x)
			direction.x = 0;

		if(direction.y == -1 && p.y <= desiredSpot.y)
			direction.y = 0;

		if(direction.y == 1 && p.y >= desiredSpot.y)
			direction.y = 0;
	}

	private void ForceDefaultDirection()
	{
		if(direction.Length() == 0)
			direction.x = -1;
	}

	protected override void DecideWhatToDo()
	{
		FixMoveDirection();

		if(TryToMove() || TryToPark())
			return;
	}

	protected override void Initialize()
	{
		base.Initialize();
		parkingTimer = GetNode<Timer>(parkingTimerNP);
	}

	public override void _Ready() {}

	public override Vector3 Direction
	{
		set
		{
			direction = value;
			ForceDefaultDirection();
			RandomizeParkingSpotAndTime();
		}
	}


	[Export]
	public NodePath parkingTimerNP;

	[Export]
	public Vector2 xMoveRange = new Vector2(2f, 6.0f);

	[Export]
	public Vector2 yMoveRange = new Vector2(-0.5f, 2.0f);

	[Export]
	public Vector2 parkingTimeRange = new Vector2(3.0f, 6.0f);

	
	private Timer parkingTimer;
	private Vector3 desiredSpot;
}
