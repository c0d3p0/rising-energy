using Godot;


public class AnimationRandomizer : Node
{
	private void Initialize()
	{
		animationPlayer = GetNode<AnimationPlayer>(animationPlayerNP);
	}

	private void PlayRandomAnimation()
	{
		RandomNumberGenerator rng = new RandomNumberGenerator();
		string[] animations = animationPlayer.GetAnimationList();
		animationPlayer.Play(animations[this.RandiRange(rng, 0, animations.Length - 1)]);
	}

	public override void _EnterTree()
	{
		Initialize();
	}

	public override void _Ready()
	{
		PlayRandomAnimation();
	}


	[Export]
	public NodePath animationPlayerNP;


	private AnimationPlayer animationPlayer;
}
