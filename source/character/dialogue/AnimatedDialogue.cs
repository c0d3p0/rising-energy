using Godot;


public class AnimatedDialogue : Spatial
{
	public void Interact(Spatial playerCharacter)
	{
		this.playerCharacter = playerCharacter;
		playerCharacter.Call(this.GetMethodFixBodyDirection(), playerForcedDirection);
		animationStateMachine.Travel(animationName);
	}

	public void EndInteraction()
	{
		playerCharacter.Call(this.GetMethodSetToDialogueInteraction(), false);
		playerCharacter = null;
	}

	public void OnInteractAreaEntered(Area area)
	{
		hintAnimationPlayer.Play("hint_idle");
		area.EmitSignal(this.GetSignalSetInteractionObject(), this);
	}

	public void OnInteractAreaExited(Area area)
	{
		area.EmitSignal(this.GetSignalSetInteractionObject());

		// Force animation to go to the 0.0 avoiding the text to stuck on screen.
		hintAnimationPlayer.Seek(0, true);
		hintAnimationPlayer.Stop();
	}

	private void Initialize()
	{
		spirit = GetNode<Spatial>(spiritNP);
		spirit.Translation = spirit.Translation * playerForcedDirection;
		spirit.RotationDegrees = new Vector3(0f, (-1f + playerForcedDirection.x) * 90f, 0f);
		hintAnimationPlayer = GetNode<AnimationPlayer>(hintAnimationPlayerNP);
		animationStateMachine = GetNode<AnimationTree>(animationTreeNP).
				Get("parameters/playback") as AnimationNodeStateMachinePlayback;
	}

	public override void _EnterTree()
	{
		Initialize();
	}


	[Export]
	public NodePath spiritNP;

	[Export]
	public NodePath animationTreeNP;

	[Export]
	public NodePath hintAnimationPlayerNP;

	[Export]
	public string animationName;

	[Export]
	public Vector3 playerForcedDirection = new Vector3(1f, 0f, 0f);


	private Spatial spirit;
	private AnimationNodeStateMachinePlayback animationStateMachine;
	private AnimationPlayer hintAnimationPlayer;

	private Spatial playerCharacter;
}
