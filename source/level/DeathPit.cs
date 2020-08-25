using Godot;


public class DeathPit : VisibilityNotifier
{
	public void OnCameraEntered(Camera camera)
	{
		camera.EmitSignal(this.GetSignalAddDeathPit(), this);
	}

	public void OnCameraExited(Camera camera)
	{
		camera.EmitSignal(this.GetSignalRemoveDeathPit(), this);
	}
}
