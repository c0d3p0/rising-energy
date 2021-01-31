using Godot;


public class SurvivalLevelManagerInitializer : Node
{
	private void Initialize()
	{
		levelManager = GetNode<SurvivalLevelManager>(levelManagerNP);
		cameraManager = GetNode<CameraManager>(cameraManagerNP);
		levelProgress = GetNode<SurvivalLevelProgress>(levelProgressNP);
		scoreLabel = GetNode<Label>(scoreLabelNP);
		nextBossScoreLabel = GetNode<Label>(nextBossScoreLabelNP);
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

	private void InitalizeCamera()
	{
		camera.AddUserSignal(this.GetSignalAddDeathPit());
		camera.AddUserSignal(this.GetSignalRemoveDeathPit());
		camera.AddUserSignal(this.GetSignalSetFightArea());
		camera.AddUserSignal(this.GetSignalAddEnemySpawnSpot());
		camera.AddUserSignal(this.GetSignalRemoveEnemySpawnSpot());

		camera.Connect(this.GetSignalAddDeathPit(), cameraManager,
				this.GetMethodAddDeathPit());
		camera.Connect(this.GetSignalRemoveDeathPit(), cameraManager,
				this.GetMethodRemoveDeathPit());
		camera.Connect(this.GetSignalSetFightArea(), cameraManager,
				this.GetMethodSetFightArea());
		camera.Connect(this.GetSignalAddEnemySpawnSpot(), levelProgress,
				this.GetMethodAddEnemySpawnSpot());
		camera.Connect(this.GetSignalRemoveEnemySpawnSpot(), levelProgress,
				this.GetMethodRemoveEnemySpawnSpot());
	}

	private void InitalizeLevelProgress()
	{
		levelProgress.GlobalData = GetNode(globalDataNodePath);
		levelProgress.NodeFactory = GetNode(nodeFactoryNodePath);
		levelProgress.NextLevelScenePath = levelManager.nextLevelScenePath;
		levelProgress.Target = levelManager.Target;
		levelProgress.PlayerStartingDirection = levelManager.playerStartingDirection;
		levelProgress.EnergySourceSpawnPosition = levelManager.energySourceSpawnPosition;
		levelProgress.MaximumActiveEnemies = levelManager.maximumActiveEnemies;

		levelProgress.LandEnemyPrefabKeyList = levelManager.landEnemyPrefabKeyList;
		levelProgress.AirEnemyPrefabKeyList = levelManager.airEnemyPrefabKeyList;
		levelProgress.BossPrefabKeyList = levelManager.bossPrefabKeyList;
		
		levelProgress.BossPositionList = levelManager.bossPositionList;
		levelProgress.FightAreaPositionList = levelManager.fightAreaPositionList;
		levelProgress.EnemyKillScoreMap = levelManager.enemyKillScoreMap;
		levelProgress.BossKillScoreMap = levelManager.bossKillScoreMap;
		levelProgress.BossSpawnScoreFromEnemyInterval = levelManager.bossSpawnScoreFromEnemyInterval;
		levelProgress.FightAreaTriggerPositionList =
				levelManager.fightAreaTriggerPositionList;

		levelProgress.AddUserSignal(this.GetSignalPlayerScoreChanged());
		levelProgress.AddUserSignal(this.GetSignalNextBossScoreChanged());

		levelProgress.Connect(this.GetSignalPlayerScoreChanged(), scoreLabel,
				this.GetGDMethodSetText());
		levelProgress.Connect(this.GetSignalNextBossScoreChanged(),
				nextBossScoreLabel, this.GetGDMethodSetText());
		levelProgress.Connect(this.GetGDSignalReady(), animationStateMachine,
				this.GetGDMethodTravel(), this.CreateSingleBind("intro"));
	}

	private void InitializePlayerCharacter()
	{
		levelManager.Target.AddUserSignal(this.GetSignalDead());
		levelManager.Target.Connect(this.GetSignalDead(), animationStateMachine,
				this.GetGDMethodTravel(), this.CreateSingleBind("finish"));
	}

	public override void _EnterTree()
	{
		Initialize();
		InitalizeCameraManager();
		InitalizeLevelProgress();
		InitalizeCamera();
		InitializePlayerCharacter();
	}


	[Export]
	public NodePath levelManagerNP;

	[Export]
	public NodePath cameraManagerNP;

	[Export]
	public NodePath levelProgressNP;

	[Export]
	public NodePath animationTreeNP;

	[Export]
	public NodePath scoreLabelNP;

	[Export]
	public NodePath nextBossScoreLabelNP;

	[Export]
	public string globalDataNodePath = "/root/GlobalData";

	[Export]
	public string nodeFactoryNodePath = "/root/NodeFactory";


	private Camera camera;

	private SurvivalLevelManager levelManager;
	private CameraManager cameraManager;
	private SurvivalLevelProgress levelProgress;
	private AnimationNodeStateMachinePlayback animationStateMachine;	
	private Label scoreLabel;
	private Label nextBossScoreLabel;
}
