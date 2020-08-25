using Godot;


public class BaseBossInitializer : BaseEnemyInitializer
{
	protected override void Initialize()
	{
		base.Initialize();
		healthStatusBarManager = GetNode(healthStatusBarManagerNP);
		energyStatusBarManager = GetNode(energyStatusBarManagerNP);
	}

	protected void InitializeBossBehavior()
	{
		BaseBossCharacter bc = enemyCharacter as BaseBossCharacter;
		BaseBossBehavior bb = enemyBehavior as BaseBossBehavior;
		bb.KinSpawnPivot = bc.KinSpawnPivot;
		enemyBehavior.AddUserSignal(this.GetSignalCanAttack());
		enemyBehavior.Connect(this.GetSignalCanAttack(),
				enemyCharacter, this.GetMethodCanAttack());
	}

	protected void InitializeBossVitality()
	{
		enemyCharacterVitality.AddUserSignal(this.GetSignalHealthChanged());
		enemyCharacterVitality.AddUserSignal(this.GetSignalEnergyChanged());

		enemyCharacterVitality.Connect(this.GetSignalHealthChanged(),
				healthStatusBarManager, this.GetMethodSetProgress());
		enemyCharacterVitality.Connect(this.GetSignalEnergyChanged(),
				energyStatusBarManager, this.GetMethodSetProgress());
	}

	public override void _EnterTree()
	{
		base._EnterTree();
		InitializeBossVitality();
		InitializeBossBehavior();
	}


	[Export]
	public NodePath healthStatusBarManagerNP;

	[Export]
	public NodePath energyStatusBarManagerNP;


	protected Node healthStatusBarManager;
	protected Node energyStatusBarManager;
}
