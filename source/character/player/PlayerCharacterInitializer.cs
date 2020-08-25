using Godot;


public class PlayerCharacterInitializer : Node
{
	private void Initialize()
	{
		hurtArea = GetNode<Area>(hurtAreaNP);
		playerCharacter = GetNode<PlayerCharacter>(playerCharacterNP);
		playerCharacterAction = GetNode(playerCharacterActionNP);
		characterPhysics = GetNode<CharacterPhysics>(characterPhysicsNP);
		characterGround = GetNode<CharacterGround>(characterGroundNP);
		characterMove = GetNode<CharacterMove>(characterMoveNP);
		characterJump = GetNode<CharacterJump>(characterJumpNP);
		characterHurtHop = GetNode<CharacterJump>(characterHurtHopNP);
		playerInputInterpreter = GetNode(playerInputInterpreterNP);
		healthStatusBarManager = GetNode(healthStatusBarManagerNP);
		energyStatusBarManager = GetNode(energyStatusBarManagerNP);
		playerCharacterVitality = GetNode<PlayerCharacterVitality>(
				playerCharacterVitalityNP);
	}

	private void InitializeCharacterGround()
	{
		characterGround.AddUserSignal(this.GetSignalIncreaseTranslation());
		characterGround.Connect(this.GetSignalIncreaseTranslation(),
				characterPhysics, this.GetMethodIncreaseTranslation());
	}

	private void InitializePlayerCharacterAction()
	{
		playerCharacterAction.AddUserSignal(this.GetSignalApplyMove());
		playerCharacterAction.AddUserSignal(this.GetSignalPrepareJump());
		playerCharacterAction.AddUserSignal(this.GetSignalPrepareHurtHop());
		playerCharacterAction.AddUserSignal(this.GetSignalIsOnGround());
		playerCharacterAction.AddUserSignal(this.GetSignalCanAttack());
		playerCharacterAction.AddUserSignal(this.GetSignalCanBlock());
		playerCharacterAction.AddUserSignal(this.GetSignalDead());
		playerCharacterAction.AddUserSignal(this.GetSignalHealthChanged());
		playerCharacterAction.AddUserSignal(this.GetSignalEnergyChanged());

		playerCharacterAction.Connect(this.GetSignalApplyMove(),
				characterMove, this.GetMethodApplyMove());
		playerCharacterAction.Connect(this.GetSignalPrepareJump(),
				characterJump, this.GetMethodPrepareJump());
		playerCharacterAction.Connect(this.GetSignalPrepareHurtHop(),
				characterHurtHop, this.GetMethodPrepareJump());
		playerCharacterAction.Connect(this.GetSignalIsOnGround(),
				playerCharacter, this.GetMethodIsOnGround());
		playerCharacterAction.Connect(this.GetSignalCanAttack(),
				playerCharacter, this.GetMethodCanAttack());
		playerCharacterAction.Connect(this.GetSignalCanBlock(),
				playerCharacter, this.GetMethodCanBlock());
		playerCharacterAction.Connect(this.GetSignalDead(),
				playerCharacter, this.GetMethodIsDead());
		playerCharacterAction.Connect(this.GetSignalHealthChanged(),
				playerCharacterVitality, this.GetMethodDecreaseHealth());
		playerCharacterAction.Connect(this.GetSignalEnergyChanged(),
				playerCharacterVitality, this.GetMethodDecreaseEnergy());
	}

	private void InitializeCharacterMove()
	{
		characterMove.AddUserSignal(this.GetSignalIncreaseVelocity());
		characterMove.Connect(this.GetSignalIncreaseVelocity(),
				characterPhysics, this.GetMethodIncreaseVelocity());
	}

	private void InitializeCharacterJump()
	{
		characterJump.AddUserSignal(this.GetSignalIsOnGround());
		characterJump.AddUserSignal(this.GetSignalIncreaseVelocity());

		characterJump.Connect(this.GetSignalIsOnGround(),
				playerCharacter, this.GetMethodIsOnGround());
		characterJump.Connect(this.GetSignalIncreaseVelocity(),
				characterPhysics, this.GetMethodIncreaseVelocity());
	}

	private void InitializeCharacterHurtHop()
	{
		characterHurtHop.AddUserSignal(this.GetSignalIsOnGround());
		characterHurtHop.AddUserSignal(this.GetSignalIncreaseVelocity());

		characterHurtHop.Connect(this.GetSignalIsOnGround(),
				playerCharacter, this.GetMethodIsOnGround());
		characterHurtHop.Connect(this.GetSignalIncreaseVelocity(),
				characterPhysics, this.GetMethodIncreaseVelocity());
	}

	private void InitializePlayerInputInterpreter()
	{
		playerInputInterpreter.AddUserSignal(this.GetSignalInteract());
		playerInputInterpreter.AddUserSignal(this.GetSignalCancelInteraction());
		playerInputInterpreter.AddUserSignal(this.GetSignalBlock());
		playerInputInterpreter.AddUserSignal(this.GetSignalAttack());
		playerInputInterpreter.AddUserSignal(this.GetSignalJump());
		playerInputInterpreter.AddUserSignal(this.GetSignalCrouch());
		playerInputInterpreter.AddUserSignal(this.GetSignalMove());

		playerInputInterpreter.Connect(this.GetSignalInteract(),
				playerCharacter, this.GetMethodInteract());
		playerInputInterpreter.Connect(this.GetSignalCancelInteraction(),
				playerCharacter, this.GetMethodCancelInteraction());
		playerInputInterpreter.Connect(this.GetSignalBlock(),
				playerCharacter, this.GetMethodBlock());
		playerInputInterpreter.Connect(this.GetSignalAttack(),
				playerCharacter, this.GetMethodAttack());
		playerInputInterpreter.Connect(this.GetSignalJump(),
				playerCharacter, this.GetMethodJump());
		playerInputInterpreter.Connect(this.GetSignalCrouch(),
				playerCharacter, this.GetMethodCrouch());
		playerInputInterpreter.Connect(this.GetSignalMove(),
				playerCharacter, this.GetMethodMove());
	}

	private void InitializePlayerCharacterVitality()
	{
		playerCharacterVitality.AddUserSignal(this.GetSignalHealthChanged());
		playerCharacterVitality.AddUserSignal(this.GetSignalEnergyChanged());

		playerCharacterVitality.Connect(this.GetSignalHealthChanged(),
				healthStatusBarManager, this.GetMethodSetProgress());
		playerCharacterVitality.Connect(this.GetSignalEnergyChanged(),
				energyStatusBarManager, this.GetMethodSetProgress());
	}
	
	private void InitializeHurtArea()
	{
		hurtArea.AddUserSignal(this.GetSignalHit());
		hurtArea.AddUserSignal(this.GetSignalSetInteractionObject());

		hurtArea.Connect(this.GetSignalHit(), playerCharacterAction,
				this.GetMethodHit());
		hurtArea.Connect(this.GetSignalSetInteractionObject(),
				playerCharacter, this.GetMethodSetInteractionObject());
	}
	
	public override void _EnterTree()
	{
		Initialize();
		InitializeCharacterGround();
		InitializeCharacterMove();
		InitializeCharacterJump();
		InitializeCharacterHurtHop();
		InitializePlayerInputInterpreter();
		InitializePlayerCharacterAction();
		InitializePlayerCharacterVitality();
		InitializeHurtArea();
	}


	[Export]
	public NodePath playerCharacterNP;

	[Export]
	public NodePath characterPhysicsNP;

	[Export]
	public NodePath characterGroundNP;

	[Export]
	public NodePath characterMoveNP;

	[Export]
	public NodePath characterJumpNP;

	[Export]
	public NodePath characterHurtHopNP;

	[Export]
	public NodePath playerCharacterVitalityNP;	

	[Export]
	public NodePath playerCharacterActionNP;

	[Export]
	public NodePath playerInputInterpreterNP;

	[Export]
	public NodePath healthStatusBarManagerNP;

	[Export]
	public NodePath energyStatusBarManagerNP;

	[Export]
	public NodePath hurtAreaNP;


	private PlayerCharacter playerCharacter;
	private CharacterPhysics characterPhysics;
	private CharacterGround characterGround;
	private Node playerCharacterAction;
	private CharacterMove characterMove;
	private CharacterJump characterJump;
	private CharacterJump characterHurtHop;
	private PlayerCharacterVitality playerCharacterVitality;
	private Node playerInputInterpreter;
	private Node healthStatusBarManager;
	private Node energyStatusBarManager;
	private Area hurtArea;
}
