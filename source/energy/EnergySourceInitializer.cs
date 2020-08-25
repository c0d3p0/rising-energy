using Godot;


public class EnergySourceInitializer : Node
{
	private void Initialize()
	{
		energySource = GetNode<EnergySource>(energySourceNP);
		energySourceBoost = GetNode<EnergySourceBoost>(energySourceBoostNP);
		energyStatusBarManager = GetNode(energyStatusBarManagerNP);
	}

	private void InitializeSourceBoost()
	{
		energySourceBoost.MaxEnergy = energySource.maxEnergy;
		energySourceBoost.EnergyBoostRate = energySource.energyBoostRate;
		energySourceBoost.EnergyRecoveryPerSecond = energySource.energyRecoveryPerSecond;

		energySourceBoost.AddUserSignal(this.GetSignalEnergyChanged());
		energySourceBoost.Connect(this.GetSignalEnergyChanged(),
				energyStatusBarManager, this.GetMethodSetProgress());
	}

	public override void _EnterTree()
	{
		Initialize();
		InitializeSourceBoost();
	}


	[Export]
	public NodePath energySourceNP;

	[Export]
	public NodePath energySourceBoostNP;

	[Export]
	public NodePath energyStatusBarManagerNP;


	private EnergySource energySource;
	private EnergySourceBoost energySourceBoost;
	private Node energyStatusBarManager;
}
