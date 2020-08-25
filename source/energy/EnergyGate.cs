using Godot;


public class EnergyGate : StaticBody
{
	public void SelfDestroy()
	{
		this.GetParent().CallDeferred(this.GetGDMethodRemoveChild(), this);
	}

	public void OnHurtAreaEntered(Area area)
	{
		if(area.IsInGroup(unlockStrikeGroup))
			animationPlayer.Play("vanish");
	}

	private void Initialize()
	{
		animationPlayer = GetNode<AnimationPlayer>(animationPlayerNP);
	}

	public override void _EnterTree()
	{
		Initialize();
	}

	public override void _ExitTree()
	{
		QueueFree();
	}


	[Export]
	public string unlockStrikeGroup;

	[Export]
	public NodePath animationPlayerNP;


	private AnimationPlayer animationPlayer;
}
