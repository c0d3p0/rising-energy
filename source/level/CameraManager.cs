using Godot;
using Godot.Collections;


public class CameraManager : Node
{
	public void AddDeathPit(Spatial deathPit)
	{
		string iid = deathPit.GetInstanceId().ToString();

		if(!deathPitMap.ContainsKey(iid))
			deathPitMap.Add(iid, deathPit);
	}

	public void RemoveDeathPit(Spatial deathPit)
	{
		string iid = deathPit.GetInstanceId().ToString();
		
		if(deathPitMap.ContainsKey(iid))
			this.deathPitMap.Remove(iid);
	}

	public void SetFightArea(Spatial fightArea)
	{
		this.fightArea = fightArea;
		fightArea.Connect(this.GetGDSignalTreeExited(), this,
				this.GetMethodOnKinDestroyed(), this.CreateSingleBind(fightArea));
	}

	public void OnKinDestroyed(Spatial kin)
	{
		if(this.fightArea.GetInstanceId() == fightArea.GetInstanceId())
			this.fightArea = null;
	}

	private bool LockCameraBecauseDeathPit()
	{
		return deathPitMap.Count > 0;
	}

	private bool MoveCameraToFightArea(float delta)
	{
		if(fightArea != null && fightArea.IsInsideTree())
		{
			Vector3 faPos = fightArea.GlobalTransform.origin;
			Vector3 tp = target.GlobalTransform.origin;
			float moveX;

			if(tp.x < faPos.x - fightAreaMoveRange.x)
				moveX = faPos.x - fightAreaMoveRange.x;
			else if(tp.x > faPos.x + fightAreaMoveRange.x)
				moveX = faPos.x + fightAreaMoveRange.x;
			else
				moveX = tp.x;

			MoveCameraSmoothly(new Vector3(moveX, faPos.y, faPos.z),
					toFightAreaMoveSpeed, delta);
		}

		return fightArea != null;
	}

	private bool MoveCameraDefault(float delta)
	{
		if(target != null)
		{
			Vector3 cp = camera.GlobalTransform.origin - distance;
			Vector3 tp = target.GlobalTransform.origin;
			float moveY;
			bool targetNotOnGround = !this.Call<bool>(target, this.GetMethodIsOnGround());
			if(targetNotOnGround && tp.y >= targetLastPosition.y)
			{
				moveY = cp.y < tp.y - targetMoveYTolerance.y ?
						tp.y - targetMoveYTolerance.y : cp.y;
			}
			else if(targetNotOnGround && tp.y < targetLastPosition.y)
				moveY = tp.y - targetMoveYTolerance.x;
			else
				moveY = tp.y;

			MoveCameraSmoothly(new Vector3(tp.x, moveY, tp.z), toTargetMoveSpeed, delta);
		}
		else
			camera.Translation = distance;

		return true;
	}

	private void MoveCameraSmoothly(Vector3 position, Vector3 moveSpeed, float delta)
	{
		Vector3 cp = camera.GlobalTransform.origin;
		float moveX = Mathf.Lerp(cp.x, position.x + distance.x, delta * moveSpeed.x);
		float moveY = Mathf.Lerp(cp.y, position.y + distance.y, delta * moveSpeed.y);
		float moveZ = Mathf.Lerp(cp.z, position.z + distance.z, delta * moveSpeed.z);
		camera.Translation = new Vector3(moveX, moveY, moveZ);
	}

	private void HandleMoveCamera(float delta)
	{
		if(LockCameraBecauseDeathPit() ||
				MoveCameraToFightArea(delta) || MoveCameraDefault(delta));

		targetLastPosition = target.GlobalTransform.origin;
	}

	private void Initialize()
	{
		deathPitMap = new Dictionary<string, Spatial>();
	}

	public override void _EnterTree()
	{
		Initialize();
	}

	public override void _Ready()
	{
		camera.Translation = target.GlobalTransform.origin + distance;
		targetLastPosition = target.GlobalTransform.origin;
	}

	public override void _Process(float delta)
	{
		HandleMoveCamera(delta);
	}

	public Camera Camera
	{
		set
		{
			camera = value;
		}	
	}

	public Spatial Target
	{
		set
		{
			target = value;
		}	
	}
	
	public Vector3 Distance
	{
		set
		{
			distance = value;
		}	
	}


	[Export]
	public Vector2 targetMoveYTolerance = new Vector2(1.5f, 2f);

	[Export]
	public Vector3 toFightAreaMoveSpeed = new Vector3(1.2f, 2.0f, 2.0f);

	[Export]
	public Vector3 toTargetMoveSpeed = new Vector3(8.0f, 7.0f, 10.0f);

	[Export]
	public Vector3 fightAreaMoveRange = new Vector3(6f, 0f, 0f);


	private Camera camera;
	private Spatial target;
	private Vector3 distance;

	private Dictionary<string, Spatial> deathPitMap;
	private Spatial fightArea;
	private Vector3 targetLastPosition;
}
