using Godot;


public class EnergySource : Spatial
{
	public void Interact(Spatial playerCharacter, int interaction, Godot.Object signalData)
	{
		signalData.Call(this.GetMethodSet(),
				energySourceBoost.Interact(playerCharacter, interaction));
	}	

	private void Initialize()
	{
		energySourceBoost = GetNode<EnergySourceBoost>(energySourceBoostNP);
	}

	public override void _EnterTree()
	{
		Initialize();
	}

	public override void _ExitTree()
	{
		QueueFree();
	}

	
	[Export]
	public NodePath energySourceBoostNP;

	[Export]
	public float maxEnergy = 1000;

	[Export]
	public float energyBoostRate = 50;

	[Export]
	public float energyRecoveryPerSecond = 150;
	

	private EnergySourceBoost energySourceBoost;
}
