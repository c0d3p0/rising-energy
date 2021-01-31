using Godot;


public class PlayerCharacterDebug : Node
{
	public override void _Input(InputEvent inputEvent)
	{
		InputEventKey iek = inputEvent as InputEventKey;

		if(iek != null && iek.Pressed)
		{
			uint kc = iek.Scancode;

			if(kc == (uint) KeyList.Kp7)
				IncreaseCurrentHealth(-100f);
			else if(kc == (uint) KeyList.Kp8)
				IncreaseCurrentHealth(100f);

			else if(kc == (uint) KeyList.Kp9)
				IncreaseMaxHealth(-100f);
			else if(kc == (uint) KeyList.KpAdd)
				IncreaseMaxHealth(100f);

			else if(kc == (uint) KeyList.Kp4)
				IncreaseCurrentEnergy(-100f);
			else if(kc == (uint) KeyList.Kp5)
				IncreaseCurrentEnergy(100f);

			else if(kc == (uint) KeyList.Kp6)
				IncreaseMaxEnergy(-100f);
			else if(kc == (uint) KeyList.KpPeriod)
				IncreaseMaxEnergy(100f);

			else if(kc == (uint) KeyList.KpDivide)
				SetEnergyName("blaze");
			else if(kc == (uint) KeyList.KpMultiply)
				SetEnergyName("avenger");
			else if(kc == (uint) KeyList.KpSubtract)
				SetEnergyName("monarch");

			else if(kc == (uint) KeyList.Kp1)
				ToggleGodMode();
			else if(kc == (uint) KeyList.Kp0)
				Suicide();
		}
	}

	private void ToggleGodMode()
	{
		if(playerCharacterVitality.EnergyName == null ||
				!playerCharacterVitality.EnergyName.Equals("god_mode"))
		{
			SetEnergyName("god_mode");
			characterMove.moveSpeed = new Vector3(15f, 0f, 0f);
			characterJump.jumpSpeed = new Vector3(6f, 14f, 0f);
			playerCharacterVitality.MaxHealth = 10000000f;
			playerCharacterVitality.CurrentHealth = 10000000f;
		}
		else
		{
			SetEnergyName(null);
			characterMove.moveSpeed = new Vector3(4.2f, 0f, 0f);
			characterJump.jumpSpeed = new Vector3(5f, 9f, 0f);
			playerCharacterVitality.MaxHealth = 1000f;
			playerCharacterVitality.CurrentHealth = 1000f;
		}
	}

	private void IncreaseMaxHealth(float amount)
	{
		playerCharacterVitality.MaxHealth += amount;
	}

	private void IncreaseCurrentHealth(float amount)
	{
		playerCharacterVitality.CurrentHealth += amount;
	}

	private void IncreaseMaxEnergy(float amount)
	{
		playerCharacterVitality.MaxEnergy += amount;
	}

	private void IncreaseCurrentEnergy(float amount)
	{
		playerCharacterVitality.CurrentEnergy += amount;
	}

	private void SetEnergyName(string energyName)
	{
		playerCharacterVitality.EnergyName = energyName;
	}

	private void Suicide()
	{
		playerCharacterAction.Die();
	}

	private void Initialize()
	{
		globalData = GetNode(globalDataNodePath);
		characterMove = GetNode<CharacterMove>(characterMoveNP);
		characterJump = GetNode<CharacterJump>(characterJumpNP);
		playerCharacterAction = GetNode<PlayerCharacterAction>(
				playerCharacterActionNP);
		playerCharacterVitality = GetNode<PlayerCharacterVitality>(
				playerCharacterVitalityNP);
	}

	public override void _EnterTree()
	{
		Initialize();
	}

	public override void _Ready()
	{
		SetProcessInput(OS.IsDebugBuild());
	}


	[Export]
	public NodePath playerCharacterActionNP;

	[Export]
	public NodePath playerCharacterVitalityNP;

	[Export]
	public NodePath characterMoveNP;

	[Export]
	public NodePath characterJumpNP;

	[Export]
	public string globalDataNodePath;


	private Node globalData;
	private PlayerCharacterAction playerCharacterAction;
	private PlayerCharacterVitality playerCharacterVitality;
	private CharacterMove characterMove;
	private CharacterJump characterJump;
}
