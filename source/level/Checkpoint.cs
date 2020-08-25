using Godot;


public class Checkpoint : VisibilityNotifier
{
	public void OnCameraEntered(Camera camera)
	{
		camera.EmitSignal(this.GetSignalSaveLevelProgress(), this);
		this.GetParent().CallDeferred(this.GetGDMethodRemoveChild(), this);
	}

	public override void _ExitTree()
	{
		QueueFree();
	}
}
