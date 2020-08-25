using Godot;


public class EnemySpawnSpot : VisibilityNotifier
{
	public void OnCameraEntered(Camera camera)
	{
		camera.EmitSignal(this.GetSignalAddEnemySpawnSpot(), this);
	}

	public void OnCameraExited(Camera camera)
	{
		camera.EmitSignal(this.GetSignalRemoveEnemySpawnSpot(), this);
	}
}
