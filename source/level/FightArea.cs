using Godot;
using Godot.Collections;


public class FightArea : Spatial
{
	public void OnCameraEntered(Camera camera)
	{
		if(!active)
		{	
			wallLeftCollisionShape.Disabled = false;
			wallRightCollisionShape.Disabled = false;
			target.Call(this.GetMethodTransitTo(), "idle");
			camera.EmitSignal(this.GetSignalSetFightArea(), this);
			camera.EmitSignal(this.GetSignalOnFightAreaEntered());
			active = true;
			this.camera = camera;
		}
	}

	public void SetCheckpointSpawnData(Node checkpoint, Dictionary paramMap)
	{
		VisibilityNotifier vn = checkpoint as VisibilityNotifier;
		vn.Translation = checkpointPosition;
		currentCheckpoint = vn;
	}

	public void OnKinDestroyed(Spatial enemy)
	{
		if(enemy != null && this.Call<bool>(enemy, this.GetMethodIsDead()))
		{
			ActivateCheckpoint();

			if(camera != null)
				camera.EmitSignal(this.GetSignalOnFightAreaExited());
		}
		
		this.GetParent().CallDeferred(this.GetGDMethodRemoveChild(), this);
	}

	private void ActivateCheckpoint()
	{
		if(currentCheckpoint != null)
			this.AddChildToEnemyContainer(this, currentCheckpoint);
	}

	private void SpawnCheckpoint()
	{
		if(currentCheckpoint == null && !requestedCheckpoint)
		{
			requestedCheckpoint = true;
			nodeFactory.Call(this.GetMethodPut(), this, checkpointPrefabKey,
					this.GetMethodSetCheckpointSpawnData(), null);
		}
	}
	
	private void Initialize()
	{
		wallLeftCollisionShape = GetNode<CollisionShape>(
				wallLeftCollisionShapeNP);
		wallRightCollisionShape = GetNode<CollisionShape>(
				wallRightCollisionShapeNP);
		fightTriggerVisibilityNotifier = GetNode<VisibilityNotifier>(
				fightTriggerVisibilityNotifierNP);
		fightTriggerVisibilityNotifier.Translation = triggerPosition;
		wallLeftCollisionShape.Disabled = true;
		wallRightCollisionShape.Disabled = true;

		if(targetNP != null)
			target = GetNode<Spatial>(targetNP);

		if(shouldCreateCheckpoint)
			SpawnCheckpoint();
	}

	public override void _EnterTree()
	{
		Initialize();
	}

	public override void _ExitTree()
	{
		QueueFree();
	}

	public void SetNodeFactory(Node nodeFactory)
	{
		this.nodeFactory = nodeFactory;
	}

	public void SetTarget(Spatial target)
	{
		this.target = target;
		target.Connect(this.GetGDSignalTreeExited(), this,
				this.GetMethodOnKinDestroyed(), this.CreateSingleBind(target));
	}

	public void SetTriggerPosition(Vector3 position)
	{
		triggerPosition = position;
	}

	public void SetCheckpointPosition(Vector3 position)
	{
		checkpointPosition = position;
		SpawnCheckpoint();
	}


	[Export]
	public NodePath targetNP;

	[Export]
	public string checkpointPrefabKey;

	[Export]
	public NodePath wallLeftCollisionShapeNP;

	[Export]
	public NodePath wallRightCollisionShapeNP;

	[Export]
	public NodePath fightTriggerVisibilityNotifierNP;

	[Export]
	public Vector3 triggerPosition = new Vector3(-6f, 0f, 0f);

	[Export]
	public Vector3 checkpointPosition = new Vector3(0f, 0f, 0f);

	[Export]
	public bool shouldCreateCheckpoint;


	private Spatial target;
	private Node nodeFactory;

	private Camera camera;
	private VisibilityNotifier currentCheckpoint;
	private CollisionShape wallLeftCollisionShape;
	private CollisionShape wallRightCollisionShape;
	private VisibilityNotifier fightTriggerVisibilityNotifier;

	private bool active;
	private bool requestedCheckpoint;
}
