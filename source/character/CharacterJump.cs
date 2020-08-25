using Godot;


public class CharacterJump : Node
{
	public void PrepareJump(Vector3 desiredDirection)
	{
		direction.x = desiredDirection.x;
		direction.y = 1;
		direction.z = 0;
	}

	public void ApplyJump()
	{
		ApplyJumpMove();
		direction.y = 0;
		jumping = true;
	}

	public void Stop()
	{
		jumping = false;
	}

	private void ApplyJumpMove()
	{
		velocity.z = 0;
		velocityLimit.z = 0;
		velocityLimit.x = direction.x * jumpSpeed.x;
		velocityLimit.y = direction.y * jumpSpeed.y;
		float interpolateTime = GetPhysicsProcessDeltaTime() * jumpAcceleration;
		velocity = velocity.LinearInterpolate(velocityLimit, interpolateTime);
		EmitSignal(this.GetSignalIncreaseVelocity(), velocity);
	}

	private void FixDirectionWhenOnGroundCorner()
	{
		if(this.EmitSignal<bool>(this, this.GetSignalIsOnGround(),
				direction.x < 0, false, direction.x > 0))
		{
			direction.x = 0;
		}
	}

	private void HandleJumping()
	{
		if(jumping && this.EmitSignal<bool>(this, this.GetSignalIsOnGround()))
			jumping = false;
		else if(jumping)
		{
			FixDirectionWhenOnGroundCorner();
			ApplyJumpMove();
		}
	}

	public override void _PhysicsProcess(float delta)
	{
		HandleJumping();
	}


	[Export]
	public NodePath playerCharacterNP;

	[Export]
	public Vector3 jumpSpeed = new Vector3(6f, 10f, 0f);

	[Export]
	public float jumpAcceleration = 60f;


	private Vector3 velocity;
	private Vector3 velocityLimit;
	private Vector3 direction;

	private bool jumping;
}

