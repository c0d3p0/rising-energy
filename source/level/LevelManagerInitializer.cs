using Godot;


public class LevelManagerInitializer : Node
{
	private void Initialize()
	{
		nodeFactory = GetNode(nodeFactoryNodePath);
		levelManager = GetNode<LevelManager>(levelManagerNP);
		cameraManager = GetNode<CameraManager>(cameraManagerNP);
		enemySpawner = GetNode<EnemySpawer>(enemySpawnerNP);
		levelProgress = GetNode<LevelProgress>(levelProgressNP);
		camera = levelManager.Camera;
		animationStateMachine = GetNode<AnimationTree>(animationTreeNP).
				Get("parameters/playback") as AnimationNodeStateMachinePlayback;	
	}

	private void InitalizeCameraManager()
	{
		cameraManager.Camera = levelManager.Camera;
		cameraManager.Distance = levelManager.cameraDistance;
		cameraManager.Target = levelManager.Target;
	}

	private void InitalizeEnemySpawner()
	{
		enemySpawner.NodeFactory = nodeFactory;
		enemySpawner.Target = levelManager.Target;
		enemySpawner.MaximumActiveEnemies = levelManager.maximumActiveEnemies;
		enemySpawner.LandEnemyPrefabKeyList = levelManager.landEnemyPrefabKeyList;
		enemySpawner.AirEnemyPrefabKeyList = levelManager.airEnemyPrefabKeyList;
	}

	private void InitalizeCamera()
	{
		camera.AddUserSignal(this.GetSignalAddDeathPit());
		camera.AddUserSignal(this.GetSignalRemoveDeathPit());
		camera.AddUserSignal(this.GetSignalSetFightArea());
		camera.AddUserSignal(this.GetSignalAddEnemySpawnSpot());
		camera.AddUserSignal(this.GetSignalRemoveEnemySpawnSpot());
		camera.AddUserSignal(this.GetSignalSaveLevelProgress());
		camera.AddUserSignal(this.GetSignalOnFightAreaEntered());
		camera.AddUserSignal(this.GetSignalOnFightAreaExited());

		camera.Connect(this.GetSignalAddDeathPit(), cameraManager,
				this.GetMethodAddDeathPit());
		camera.Connect(this.GetSignalRemoveDeathPit(), cameraManager,
				this.GetMethodRemoveDeathPit());
		camera.Connect(this.GetSignalSetFightArea(), cameraManager,
				this.GetMethodSetFightArea());
		camera.Connect(this.GetSignalAddEnemySpawnSpot(), enemySpawner,
				this.GetMethodAddEnemySpawnSpot());
		camera.Connect(this.GetSignalRemoveEnemySpawnSpot(), enemySpawner,
				this.GetMethodRemoveEnemySpawnSpot());
		camera.Connect(this.GetSignalSaveLevelProgress(), levelProgress,
				this.GetMethodSaveLevelProgress());
		camera.Connect(this.GetSignalOnFightAreaEntered(), animationStateMachine,
				this.GetGDMethodTravel(), this.CreateSingleBind("fight_bgm"));
		camera.Connect(this.GetSignalOnFightAreaExited(), animationStateMachine,
				this.GetGDMethodTravel(), this.CreateSingleBind(levelManager.mainBGMAnimationName));
	}

	private void InitalizeLevelProgress()
	{
		levelProgress.GlobalData = GetNode(globalDataNodePath);
		levelProgress.NodeFactory = nodeFactory;
		levelProgress.CurrentLevelScenePath = levelManager.currentLevelScenePath;
		levelProgress.NextLevelScenePath = levelManager.nextLevelScenePath;
		levelProgress.Target = levelManager.Target;
		levelProgress.PlayerStartingDirection = levelManager.playerStartingDirection;
		levelProgress.BossPrefabKeyList = levelManager.bossPrefabKeyList;
		levelProgress.BossPositionList = levelManager.bossPositionList;
		levelProgress.FightAreaPositionList = levelManager.fightAreaPositionList;
		levelProgress.FightAreaTriggerPositionList =
				levelManager.fightAreaTriggerPositionList;
		levelProgress.CheckpointPositionMap =
				levelManager.checkpointPositionMap;

		levelProgress.AddUserSignal(this.GetSignalFinishLevel());
		levelProgress.AddUserSignal(this.GetSignalOnIntroFinished());

		levelProgress.Connect(this.GetSignalFinishLevel(), animationStateMachine,
				this.GetGDMethodTravel(), this.CreateSingleBind("finish"));
		levelProgress.Connect(this.GetSignalOnIntroFinished(), animationStateMachine,
				this.GetGDMethodTravel(), this.CreateSingleBind(levelManager.mainBGMAnimationName));
	}

	private void InitializePlayerCharacter()
	{
		levelManager.Target.AddUserSignal(this.GetSignalDead());
		levelManager.Target.Connect(this.GetSignalDead(), animationStateMachine,
				this.GetGDMethodTravel(), this.CreateSingleBind("reload"));
	}

	public override void _EnterTree()
	{
		Initialize();
		InitalizeCameraManager();
		InitalizeEnemySpawner();
		InitalizeLevelProgress();
		InitalizeCamera();
		InitializePlayerCharacter();
	}


	[Export]
	public NodePath levelManagerNP;

	[Export]
	public NodePath cameraManagerNP;

	[Export]
	public NodePath enemySpawnerNP;

	[Export]
	public NodePath levelProgressNP;

	[Export]
	public NodePath animationTreeNP;

	[Export]
	public string globalDataNodePath = "/root/GlobalData";

	[Export]
	public string nodeFactoryNodePath = "/root/NodeFactory";


	private Node nodeFactory;
	private Camera camera;

	private LevelManager levelManager;
	private CameraManager cameraManager;
	private EnemySpawer enemySpawner;
	private LevelProgress levelProgress;
	private AnimationNodeStateMachinePlayback animationStateMachine;
}
