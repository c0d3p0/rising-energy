using Godot;


// All public methods in this class are called by
// animations in the editor.
public class PlayerCharacterPhysicsObjects : Node
{
	public void SetObjectsForAttackState()
	{
		SetDefaultGroundRayCastsPosition();
		SetAttackStrikeAreaPosition();
	}

	public void SetObjectsForAirState()
	{	
		SetCollisionShapePosition(airCollisionShape, airCollisionShapeTranslation);
		SetAirGroundRayCastsPosition();
	}

	public void DisableGroundRayCasts()
	{
		SetGroundRayCastsEnabled(false, false, false);
	}

	public void EnableGroundRayCasts()
	{
		SetGroundRayCastsEnabled(true, true, true);
	}

	public void SetDefaultGroundRayCastsPosition()
	{
		groundRayCast.Translation = new Vector3(0f,
				standGroundSideRayCastTranslation.y,
				standGroundSideRayCastTranslation.z);
		groundLeftRayCast.Translation = new Vector3(
				-standGroundSideRayCastTranslation.x,
				standGroundSideRayCastTranslation.y,
				standGroundSideRayCastTranslation.z);
		groundRightRayCast.Translation = standGroundSideRayCastTranslation;
	}

	public void SetAirHurtGroundRayCastsPosition()
	{
		groundRayCast.Translation = new Vector3(0f,
				airHurtGroundSideRayCastTranslation.y,
				airHurtGroundSideRayCastTranslation.z);
		groundLeftRayCast.Translation = new Vector3(
				-airHurtGroundSideRayCastTranslation.x,
				airHurtGroundSideRayCastTranslation.y,
				airHurtGroundSideRayCastTranslation.z);
		groundRightRayCast.Translation = airHurtGroundSideRayCastTranslation;
	}

	private void SetAirGroundRayCastsPosition()
	{
		if(IsLookingBackward())
		{
			groundRightRayCast.Translation = new Vector3(
					-airGroundBackRayCastTranslation.x,
					airGroundBackRayCastTranslation.y,
					airGroundBackRayCastTranslation.z);
			groundLeftRayCast.Translation = new Vector3(
					-standGroundSideRayCastTranslation.x,
					standGroundSideRayCastTranslation.y,
					standGroundSideRayCastTranslation.z);
		}
		else
		{
			groundRightRayCast.Translation = standGroundSideRayCastTranslation;
			groundLeftRayCast.Translation = airGroundBackRayCastTranslation;
		}

		groundRayCast.Translation = new Vector3(0f,
					standGroundSideRayCastTranslation.y,
					standGroundSideRayCastTranslation.z);
	}

	private void SetCollisionShapePosition(CollisionShape shape, Vector3 position)
	{
		if(IsLookingBackward())
			shape.Translation = new Vector3(-position.x, position.y, position.z);
		else
			shape.Translation = position;
	}

	private void SetGroundRayCastsEnabled(bool left, bool mid, bool right)
	{
		groundLeftRayCast.Enabled = left;
		groundRayCast.Enabled = mid;
		groundRightRayCast.Enabled = right;
	}

	private void SetAttackStrikeAreaPosition()
	{
		if(IsLookingBackward())
		{
			strikeArea.Translation = new Vector3(-strikeAreaTranslation.x,
					strikeAreaTranslation.y, strikeAreaTranslation.z);
		}
		else
			strikeArea.Translation = strikeAreaTranslation;
	}

	private bool IsLookingBackward()
	{
		return body.RotationDegrees.y == bodyDirectionAngle.x;
	}

	private bool IsLookingForward()
	{
		return body.RotationDegrees.y == bodyDirectionAngle.y;
	}

	private void InitializeRayCasts()
	{
		RayCast[] rayCasts = new RayCast[]{
					groundLeftRayCast, groundRayCast, groundRightRayCast};
		
		for(int i = 0; i < rayCasts.Length; i++)
		{
			rayCasts[i].AddException(standCollisionShape);
			rayCasts[i].AddException(crouchCollisionShape);
			rayCasts[i].AddException(airCollisionShape);
			rayCasts[i].AddException(airHurtTopCollisionShape);
			rayCasts[i].AddException(airHurtBottomCollisionShape);
		}	
	}

	private void Initialize()
	{
		body = GetNode<Spatial>(bodyNP);
		standCollisionShape = GetNode<CollisionShape>(standCollisionShapeNP);
		crouchCollisionShape = GetNode<CollisionShape>(crouchCollisionShapeNP);
		airCollisionShape = GetNode<CollisionShape>(airCollisionShapeNP);
		airHurtTopCollisionShape = GetNode<CollisionShape>(airHurtTopCollisionShapeNP);
		airHurtBottomCollisionShape = GetNode<CollisionShape>(airHurtBottomCollisionShapeNP);
		groundRayCast = GetNode<RayCast>(groundRayCastNP);
		groundLeftRayCast = GetNode<RayCast>(groundLeftRayCastNP);
		groundRightRayCast = GetNode<RayCast>(groundRightRayCastNP);
		strikeArea = GetNode<Area>(strikeAreaNP);
	}

	public override void _EnterTree()
	{
		Initialize();
		InitializeRayCasts();
	}


	[Export]
	public NodePath bodyNP;

	[Export]
	public NodePath standCollisionShapeNP;

	[Export]
	public NodePath crouchCollisionShapeNP;

	[Export]
	public NodePath airCollisionShapeNP;

	[Export]
	public NodePath airHurtTopCollisionShapeNP;

	[Export]
	public NodePath airHurtBottomCollisionShapeNP;

	[Export]
	public NodePath groundRayCastNP;

	[Export]
	public NodePath groundLeftRayCastNP;

	[Export]
	public NodePath groundRightRayCastNP;

	[Export]
	public NodePath strikeAreaNP;

	[Export]
	public Vector2 bodyDirectionAngle = new Vector2(-180f, 0f);

	[Export]
	public Vector3 standGroundSideRayCastTranslation = new Vector3(0.6f, 0.5f, 0f);

	[Export]
	public Vector3 airGroundBackRayCastTranslation = new Vector3(-0.2f, 0.5f, 0f);

	[Export]
	public Vector3 airHurtGroundSideRayCastTranslation = new Vector3(0.9f, 0.5f, 0f);

	[Export]
	public Vector3 airCollisionShapeTranslation = new Vector3(0.2f, 1f, 0f);

	[Export]
	public Vector3 strikeAreaTranslation = new Vector3(1.29f, 1.12f, 0f);


	private Spatial body;
	private CollisionShape standCollisionShape;
	private CollisionShape crouchCollisionShape;
	private CollisionShape airCollisionShape;
	private CollisionShape airHurtTopCollisionShape;
	private CollisionShape airHurtBottomCollisionShape;
	private RayCast groundRayCast;
	private RayCast groundLeftRayCast;
	private RayCast groundRightRayCast;
	private Area strikeArea;
}
