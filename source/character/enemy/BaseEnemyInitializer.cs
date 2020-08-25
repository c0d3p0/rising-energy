using Godot;


public class BaseEnemyInitializer : Node
{
	protected virtual void Initialize()
	{
		enemyCharacter = GetNode<BaseEnemyCharacter>(enemyCharacterNP);
		characterPhysics = GetNode(characterPhysicsNP);
		characterMove = GetNode(characterMoveNP);
		enemyBehavior = GetNode<BaseEnemyBehavior>(enemyBehaviorNP);
		enemyCharacterAction = GetNode<BaseEnemyCharacterAction>(
					enemyCharacterActionNP);
		enemyCharacterVitality = GetNode<BaseCharacterVitality>(
					enemyCharacterVitalityNP);

		if(!enemyCharacter.immortal)
			hurtArea = GetNode<Area>(hurtAreaNP);
	}

	protected virtual void InitializeCharacterMove()
	{
		characterMove.AddUserSignal(this.GetSignalIncreaseVelocity());
		characterMove.Connect(this.GetSignalIncreaseVelocity(),
				characterPhysics, this.GetMethodIncreaseVelocity());
	}

	protected virtual void InitializeEnemyBehavior()
	{
		enemyBehavior.Target = enemyCharacter.Target;
		enemyBehavior.Direction = enemyCharacter.Direction;
		enemyBehavior.NodeFactory = enemyCharacter.NodeFactory;

		enemyBehavior.AddUserSignal(this.GetSignalMove());
		enemyBehavior.AddUserSignal(this.GetSignalAttack());

		enemyBehavior.Connect(this.GetSignalMove(), enemyCharacter,
				this.GetMethodMove());
		enemyBehavior.Connect(this.GetSignalAttack(), enemyCharacter,
				this.GetMethodAttack());
	}

	protected virtual void InitializeEnemyCharacterAction()
	{
		if(enemyCharacter.Target != null)
		{
			float x = enemyCharacter.Target.GlobalTransform.origin.x <
					enemyCharacter.GlobalTransform.origin.x ? -1 : 1;
			enemyCharacterAction.FixBodyDirection(new Vector3(x, 0f, 0f));
		}

		enemyCharacterAction.DespawnTimeTolerance = enemyCharacter.DespawnTimeTolerance;

		enemyCharacterAction.AddUserSignal(this.GetSignalDead());
		enemyCharacterAction.AddUserSignal(this.GetSignalApplyMove());
		enemyCharacterAction.AddUserSignal(this.GetSignalHealthChanged());
		enemyCharacterAction.AddUserSignal(this.GetSignalEnergyChanged());

		enemyCharacterAction.Connect(this.GetSignalApplyMove(),
				characterMove, this.GetMethodApplyMove());
		enemyCharacterAction.Connect(this.GetSignalDead(),
				enemyCharacter, this.GetMethodIsDead());
		enemyCharacterAction.Connect(this.GetSignalHealthChanged(),
				enemyCharacterVitality, this.GetMethodDecreaseHealth());
		enemyCharacterAction.Connect(this.GetSignalEnergyChanged(),
				enemyCharacterVitality, this.GetMethodDecreaseEnergy());
	}

	protected virtual void InitializeHurtArea()
	{
		if(!enemyCharacter.immortal)
		{
			hurtArea.AddUserSignal(this.GetSignalHit());
			hurtArea.Connect(this.GetSignalHit(), enemyCharacterAction,
					this.GetMethodHit());
		}
	}

	public override void _EnterTree()
	{
		Initialize();
		InitializeCharacterMove();
		InitializeEnemyBehavior();
		InitializeEnemyCharacterAction();
		InitializeHurtArea();
	}



	[Export]
	public NodePath enemyCharacterNP;

	[Export]
	public NodePath characterPhysicsNP;

	[Export]
	public NodePath characterMoveNP;

	[Export]
	public NodePath enemyBehaviorNP;

	[Export]
	public NodePath enemyCharacterActionNP;

	[Export]
	public NodePath enemyCharacterVitalityNP;

	[Export]
	public NodePath hurtAreaNP;


	protected BaseEnemyCharacter enemyCharacter;
	protected Node characterPhysics;
	protected Node characterMove;
	protected BaseEnemyBehavior enemyBehavior;
	protected BaseEnemyCharacterAction enemyCharacterAction;
	protected Node enemyCharacterVitality;
	protected Area hurtArea;
}
