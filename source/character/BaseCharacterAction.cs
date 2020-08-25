using Godot;
using Godot.Collections;


public class BaseCharacterAction : Node
{
	public void FixBodyDirection(Vector3 direction)
	{
		if(direction.x < 0)
			body.RotationDegrees = new Vector3(0f, directionXBodyRotation.x, 0f);
		else if(direction.x > 0)
			body.RotationDegrees = new Vector3(0f, directionXBodyRotation.y, 0f);
	}

	protected string CanTransitToBlock()
	{	
		return GetValue(toBlockMap, animationStateMachine.GetCurrentNode());
	}

	protected string CanTransitToAttack()
	{
		return GetValue(toAttackMap, animationStateMachine.GetCurrentNode());
	}

	protected string CanTransitToJump()
	{
		return GetValue(toJumpMap, animationStateMachine.GetCurrentNode());
	}

	protected string CanTransitToCrouch()
	{
		return GetValue(toCrouchMap, animationStateMachine.GetCurrentNode());
	}

	protected string CanTransitToMove()
	{
		return GetValue(toMoveMap, animationStateMachine.GetCurrentNode());
	}

	protected string GetValue(Dictionary<string, string> map, string key)
	{
		string value;

		if(map.TryGetValue(key, out value))
			return value;

		return null;
	}

	public void TransitTo(string actionName)
	{
		animationStateMachine.Travel(actionName);
	}

	protected virtual void Initialize()
	{
		body = GetNode<Spatial>(bodyNP);
		animationStateMachine = GetNode<AnimationTree>(animationTreeNP).Get(
				"parameters/playback") as AnimationNodeStateMachinePlayback;
	}

	public override void _EnterTree()
	{
		Initialize();
	}


	[Export]
	public Vector2 directionXBodyRotation = new Vector2(180f, 0f);

	[Export]
	public Dictionary<string, string> toBlockMap;

	[Export]
	public Dictionary<string, string> toAttackMap;

	[Export]
	public Dictionary<string, string> toJumpMap;

	[Export]
	public Dictionary<string, string> toCrouchMap;

	[Export]
	public Dictionary<string, string> toMoveMap;

	[Export]
	public NodePath bodyNP;

	[Export]
	public NodePath animationTreeNP;


	protected Spatial body;
	protected AnimationNodeStateMachinePlayback animationStateMachine;
}
