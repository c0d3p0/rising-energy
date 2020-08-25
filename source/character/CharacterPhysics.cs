using Godot;


public class CharacterPhysics : Node
{
	public void IncreaseVelocity(Vector3 desiredVelocity)
	{
		this.desiredVelocity += desiredVelocity;
	}

	public void IncreaseVelocity(float x, float y, float z)
	{
		this.desiredVelocity.x = x;
		this.desiredVelocity.y = y;
		this.desiredVelocity.z = z;
	}

	public void IncreaseTranslation(Vector3 desiredTranslation)
	{
		this.desiredTranslation += desiredTranslation;
	}

	public void IncreaseTranslation(float x, float y, float z)
	{
		this.desiredTranslation.x = x;
		this.desiredTranslation.y = y;
		this.desiredTranslation.z = z;
	}

	private void FixPositionZ()
	{
		if(kinematicBody.Translation.z != lockZPosition)
		{
			desiredTranslation.z = lockZPosition -
					kinematicBody.GlobalTransform.origin.z;
		}
	}

	private void ExecuteMoveAndSlide()
	{
		if(desiredVelocity.Length() > 0)
		{
			velocity = kinematicBody.MoveAndSlide(desiredVelocity,
					Vector3.Up, true, 4, Mathf.Deg2Rad(0.1f), false);
			desiredVelocity = Vector3.Zero;
		}
	}

	private void ExecuteMoveAndCollide()
	{
		if(desiredTranslation.Length() > 0)
		{
			kinematicBody.MoveAndCollide(desiredTranslation);
			desiredTranslation = Vector3.Zero;
		}
	}

	private void ApplyGravity(float delta)
	{
		if(gravityEnabled)
			desiredVelocity.y += velocity.y + (gravity * delta);
	}

	private void ResetCurrentVelocity()
	{
		velocity.x = 0;
		velocity.y = 0;
		velocity.z = 0;
	}

	public override void _PhysicsProcess(float physicsDelta)
	{
		ApplyGravity(physicsDelta);
		ExecuteMoveAndSlide();
		ExecuteMoveAndCollide();
	}

	public override void _EnterTree()
	{
		kinematicBody = GetNode<KinematicBody>(kinematicBodyNP);
	}

	
	[Export]
	public float gravity = -19.61f;

	[Export]
	public bool gravityEnabled = true;

	[Export]
	public float lockZPosition = 0f;

	[Export]
	public NodePath kinematicBodyNP;


	private KinematicBody kinematicBody;

	private Vector3 desiredVelocity;
	private Vector3 desiredTranslation;
	private Vector3 velocity;
}
