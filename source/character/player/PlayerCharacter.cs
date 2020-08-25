using Godot;


public class PlayerCharacter : KinematicBody
{
	public void Interact(Godot.Object signalData)
	{
		signalData.Call(this.GetMethodSet(),
				playerCharacterAction.Interact());
	}

	public void CancelInteraction(Godot.Object signalData)
	{
		signalData.Call(this.GetMethodSet(),
				playerCharacterAction.CancelInteraction());
	}

	// public void Block(Vector3 direction, Godot.Object signalData)
	// {
	// 	signalData.Call(this.GetMethodSet(),
	// 			playerCharacterAction.Block(direction));
	// }

	public void Attack(Vector3 direction, Godot.Object signalData)
	{
		signalData.Call(this.GetMethodSet(),
				playerCharacterAction.Attack(direction));
	}

	public void Jump(Vector3 direction, Godot.Object signalData)
	{
		signalData.Call(this.GetMethodSet(),
				playerCharacterAction.Jump(direction));
	}

	public void Crouch(Vector3 direction, Godot.Object signalData)
	{
		signalData.Call(this.GetMethodSet(),
				playerCharacterAction.Crouch(direction));
	}

	public void Move(Vector3 direction, Godot.Object signalData)
	{
		signalData.Call(this.GetMethodSet(),
				playerCharacterAction.Move(direction));
	}

	public void Cheer()
	{
		ActivateInput(false);
		playerCharacterAction.Cheer();
	}

	public void IsOnGround(Godot.Object signalData)
	{
		signalData.Call(this.GetMethodSet(),
				characterGround.IsOnGround());
	}

	public void IsOnGround(bool left, bool mid, bool right, Godot.Object signalData)
	{
		signalData.Call(this.GetMethodSet(),
				characterGround.IsOnGround(left, mid, right));
	}

	public void IsDead(Godot.Object signalData)
	{
		signalData.Call(this.GetMethodSet(), playerCharacterVitality.Dead);
	}

	public void CanAttack(Godot.Object signalData)
	{
		signalData.Call(this.GetMethodSet(),
				playerCharacterVitality.CanAttack());
	}

	public void CanBlock(Godot.Object signalData)
	{
		signalData.Call(this.GetMethodSet(),
				playerCharacterVitality.CanBlock());
	}

	public void SetVitality(Spatial energySword, Spatial energyShield,
			float maxHealth, float currentHealth, float maxEnergy, float currentEnergy)
	{
		playerCharacterVitality.SetVitality(energySword, energyShield,
				maxHealth, currentHealth, maxEnergy, currentEnergy);
	}

	public void SetItems(Spatial energySword, Spatial energyShield, float maxEnergy)
	{
		playerCharacterVitality.SetItems(energySword, energyShield, maxEnergy);
	}

	public void GetVitalityMap(Godot.Object signalData)
	{
		signalData.Call(this.GetMethodSet(),
				playerCharacterVitality.GetVitalityMap());
	}

	public void FixBodyDirection(Vector3 direction)
	{
		playerCharacterAction.FixBodyDirection(direction);
	}

	public void SetInteractionObject(Spatial interactionObject)
	{
		playerCharacterAction.InteractionObject = interactionObject;
	}

	public void SetInteractionObject()
	{
		playerCharacterAction.InteractionObject = null;
	}

	public void ActivateInput(bool active)
	{
		playerInputInterpreter.SetProcess(active);
		playerInputInterpreter.SetPhysicsProcess(active);
	}

	public void SetToDialogueInteraction(bool active)
	{
		playerInputInterpreter.SetProcess(!active);
		playerInputInterpreter.SetPhysicsProcess(!active);
		playerCharacterAction.SetToDialogueInteraction(active);
	}

	private void Initialize()
	{
		characterGround = GetNode<CharacterGround>(characterGroundNP);
		playerCharacterAction = GetNode<PlayerCharacterAction>(
				playerCharacterActionNP);
		playerCharacterVitality = GetNode<PlayerCharacterVitality>(
				playerCharacterVitalityNP);
		playerInputInterpreter = GetNode<PlayerInputInterpreter>(
				playerInputInterpreterNP);
	}

	public override void _EnterTree()
	{
		Initialize();
	}


	[Export]
	public NodePath characterGroundNP;

	[Export]
	public NodePath playerCharacterActionNP;

	[Export]
	public NodePath playerCharacterVitalityNP;

	[Export]
	public NodePath playerInputInterpreterNP;

	private CharacterGround characterGround;
	private PlayerCharacterAction playerCharacterAction;
	private PlayerCharacterVitality playerCharacterVitality;
	private PlayerInputInterpreter playerInputInterpreter;
}
