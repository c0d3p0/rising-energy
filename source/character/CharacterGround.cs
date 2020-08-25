using Godot;


public class CharacterGround : Node
{
	private void FixPosition(RayCast rayCast, bool left, bool collisionToCheck)
	{
		if(collisionToCheck && IsFalling() && !IsOnGround())
		{
			Vector3 t = new Vector3(left ? slideDistance : -slideDistance, 0f, 0f);
			EmitSignal(this.GetSignalIncreaseTranslation(), t);
		}
	}

	private void UpdateCollisionAttributes()
	{
		onGround = groundRayCast.IsColliding();
		onGroundRight = groundRightRayCast.IsColliding();
		onGroundLeft = groundLeftRayCast.IsColliding();
	}

	private bool IsFalling()
	{
		return position.y <= lastPosition.y;
	}

	private void Initialize()
	{
		playerCharacter = GetNode<KinematicBody>(playerCharacterNP);
		groundRayCast = GetNode<RayCast>(groundRayCastNP);
		groundLeftRayCast = GetNode<RayCast>(groundLeftRayCastNP);
		groundRightRayCast = GetNode<RayCast>(groundRightRayCastNP);
	}

	public override void _EnterTree()
	{
		Initialize();		
	}

	private void HandleFixPosition()
	{
		position = playerCharacter.GlobalTransform.origin;
		FixPosition(groundLeftRayCast, true, onGroundLeft);
		FixPosition(groundRightRayCast, false, onGroundRight);
		lastPosition = position;
	}

	public override void _PhysicsProcess(float delta)
	{
		UpdateCollisionAttributes();
		HandleFixPosition();
	}

	public bool IsOnGround()
	{
		return onGround || (onGroundLeft && onGroundRight);
	}

	public bool IsOnGround(bool left, bool mid, bool right)
	{
		return (left & onGroundLeft) | (mid & onGround) | (right & onGroundRight);
	}


	[Export]
	public NodePath playerCharacterNP;

	[Export]
	public NodePath groundRayCastNP;

	[Export]
	public NodePath groundLeftRayCastNP;

	[Export]
	public NodePath groundRightRayCastNP;

	[Export]
	public float slideDistance = 0.1f;

	
	private KinematicBody playerCharacter;
	private RayCast groundRayCast;
	private RayCast groundLeftRayCast;
	private RayCast groundRightRayCast;

	private Vector3 position;
	private Vector3 lastPosition;
	private bool onGround;
	private bool onGroundRight;
	private bool onGroundLeft;
}
