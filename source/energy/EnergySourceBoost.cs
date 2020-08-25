using Godot;


public class EnergySourceBoost : Node
{
	public bool Interact(Spatial playerCharacter, sbyte interaction)
	{
		if(!finished)
		{
			if(interaction == 1)
			{
				UpdateProgress(currentEnergy - energyBoostRate);

				if(finished)
				{
					GiveEnergyItems(playerCharacter);
					animationStateMachine.Travel("vanish");
					return false;
				}
				else
				{
					animationStateMachine.Travel("boost_energy");
					return true;
				}
			}
			else if(interaction == -1)
			{
				animationStateMachine.Travel("idle");
				return false;
			}
			else if(interaction == 0)
			{
				UpdateProgress(maxEnergy);
				animationStateMachine.Travel("boost_energy");
				return true;
			}
		}
		
		return false;
	}

	private void OnPlayerEnterEnergySource(Area playerArea)
	{
		UpdateProgress(maxEnergy);
		hintAnimationPlayer.Play("hint_idle");
		playerArea.EmitSignal(this.GetSignalSetInteractionObject(), energySource);
	}

	private void OnPlayerLeaveEnergySource(Area playerArea)
	{
		if(!finished)
			animationStateMachine.Travel("idle");

		// Force animation to go to the 0.0 avoiding the text to stuck on screen.
		hintAnimationPlayer.Seek(0, true);
		hintAnimationPlayer.Stop();

		playerArea.EmitSignal(this.GetSignalSetInteractionObject());
	}

	public void SelfDestroy()
	{
		energySource.GetParent().CallDeferred(
				this.GetGDMethodRemoveChild(), energySource);
	}
		
	private void GiveEnergyItems(Spatial playerCharacter)
	{
		energySource.CallDeferred(this.GetGDMethodRemoveChild(), energySword);
		energySource.CallDeferred(this.GetGDMethodRemoveChild(), energyShield);
		playerCharacter.Call(this.GetMethodSetItems(),
				energySword, energyShield, maxEnergy);
	}

	private void IncreaseEnergy(float delta)
	{
		UpdateProgress(currentEnergy + (energyRecoveryPerSecond * delta));
	}

	private void UpdateProgress(float value)
	{
		if(!finished)
		{
			if(value < maxEnergy)
			{
				currentEnergy = value;

				if(value <= 0f)
					finished = true;
			}
			else
				currentEnergy = maxEnergy;

			EmitSignal(this.GetSignalEnergyChanged(), currentEnergy);
		}
	}

	public void OnAreaEntered(Area area)
	{
		OnPlayerEnterEnergySource(area);
	}

	public void OnAreaExited(Area area)
	{
		OnPlayerLeaveEnergySource(area);
	}

	private void Initialize()
	{
		currentEnergy = maxEnergy;
		energySource = GetNode<Spatial>(energySourceNP);
		energySword = GetNode<Spatial>(energySwordNP);
		energyShield = GetNode<Spatial>(energyShieldNP);
		hintAnimationPlayer = GetNode<AnimationPlayer>(hintAnimationPlayerNP);
		animationStateMachine = GetNode<AnimationTree>(animationTreeNP).Get(
				"parameters/playback") as AnimationNodeStateMachinePlayback;
	}

	public override void _Process(float delta)
	{
		IncreaseEnergy(delta);
	}

	public override void _EnterTree()
	{
		Initialize();
	}

	public override void _Ready()
	{
		EmitSignal(this.GetSignalEnergyChanged(), 0f, maxEnergy, currentEnergy);
	}

	public float MaxEnergy
	{
		set
		{
			maxEnergy = value;
		}
	}

	public float EnergyBoostRate
	{
		set
		{
			energyBoostRate = value;
		}
	}

	public float EnergyRecoveryPerSecond
	{
		set
		{
			energyRecoveryPerSecond = value;
		}
	}



	[Export]
	public NodePath energySourceNP;

	[Export]
	public NodePath energySwordNP;

	[Export]
	public NodePath energyShieldNP;

	[Export]
	public NodePath energyStatusBarNP;

	[Export]
	public NodePath animationTreeNP;

	[Export]
	public NodePath hintAnimationPlayerNP;
	
	private float maxEnergy;
	private float energyBoostRate;
	private float energyRecoveryPerSecond;

	private Spatial energySource;
	private Spatial energySword;
	private Spatial energyShield;
	private AnimationNodeStateMachinePlayback animationStateMachine;
	public AnimationPlayer hintAnimationPlayer;

	private bool finished;
	private float currentEnergy;
}
