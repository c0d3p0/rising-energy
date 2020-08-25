using Godot;
using Godot.Collections;


public class LevelManager : Control
{
	private void Initialize()
	{
		camera = GetNode<Camera>(cameraNP);
		target = GetNode<Spatial>(targetNP);
		GetNode<Label>(levelNameLabelNP).Text = levelName;
	}

	public override void _EnterTree()
	{
		Initialize();
	}

	public Camera Camera
	{
		get
		{
			return camera;
		}
	}

	public Spatial Target
	{
		get
		{
			return target;
		}
	}


	[Export]
	public NodePath cameraNP;

	[Export]
	public NodePath targetNP;

	[Export]
	public NodePath levelNameLabelNP;

	[Export]
	public string currentLevelScenePath;

	[Export]
	public string nextLevelScenePath;

	[Export]
	public string levelName;

	[Export]
	public Vector3 cameraDistance = new Vector3(0f, 3.5f, 11f);

	[Export]
	public Vector3 playerStartingDirection = new Vector3(1f, 0f, 0f);

	[Export]
	public byte maximumActiveEnemies = 3;

	[Export]
	public Array<string> landEnemyPrefabKeyList;

	[Export]
	public Array<string> airEnemyPrefabKeyList;

	[Export]
	public Array<string> bossPrefabKeyList;

	[Export]
	public Array<Vector3> bossPositionList;

	[Export]
	public Array<Vector3> fightAreaPositionList;

	[Export]
	public Array<Vector3> fightAreaTriggerPositionList;

	[Export]
	public Dictionary<int, Vector3> checkpointPositionMap;

	[Export]
	public string mainBGMAnimationName;


	private Spatial target;
	private Camera camera;
}
