using Godot;


public class StrikeDispatcher : Area
{
	public void OnStrikeAreaEntered(Area area)
	{
		if(ShouldDispatchHit(area))
		{
			area.EmitSignal("hit", this, area, 0, damageTaken, energyTaken);
			
			if(OS.IsDebugBuild())
				GD.PushWarning("StrikeDispatcher.OnStrikeAreaEntered()");
		}
	}

	private bool ShouldDispatchHit(Area area)
	{
		Node node = area.GetParent();
		return node == null || !node.IsInGroup(ignoreGroup);
	}


	[Export]
	public float damageTaken = 50f;

	[Export]
	public float energyTaken = 50f;

	[Export]
	public string ignoreGroup = "none";
}
