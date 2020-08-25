using Godot;


public class CharacterMove : Node
{
	public void ApplyMove(Vector3 desiredDirection)
	{
		direction.x = moveInXEnabled ? desiredDirection.x : 0;
		direction.y = moveInYEnabled ? desiredDirection.y : 0;
		velocity.z = 0;
		velocityLimit.z = 0;
		velocityLimit.x = direction.x * moveSpeed.x;
		velocityLimit.y = direction.y * moveSpeed.y;
		float interpolateTime = GetPhysicsProcessDeltaTime() * GetAcceleration(direction);
		velocity = velocity.LinearInterpolate(velocityLimit, interpolateTime);
		EmitSignal(this.GetSignalIncreaseVelocity(), velocity);
	}

	private float GetAcceleration(Vector3 direction)
	{
		if(direction.Length() > 0)
			return acceleration;
		else
			return deacceleration;
	}


	[Export]
	public Vector3 moveSpeed = new Vector3(4.2f, 0f, 0f);

	[Export]
	public float acceleration = 25f;

	[Export]
	public float deacceleration = 45f;

	[Export]
	public bool moveInXEnabled = true;

	[Export]
	public bool moveInYEnabled = true;


	private Vector3 velocity;
	private Vector3 velocityLimit;
	private Vector3 direction;
}
