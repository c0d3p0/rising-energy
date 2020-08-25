using Godot;


public class StrikeDispatcher : Area
{
	public void OnStrikeAreaEntered(Area area)
	{
		GD.PushWarning("StrikeDispatcher.OnStrikeAreaEntered()");
		area.EmitSignal("hit", this, area, 0, 50, 50);
	}
}
