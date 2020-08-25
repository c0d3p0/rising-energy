public class BaseBossVitality : StatusCharacterVitality
{
	public bool CanAttack(string kinName)
	{
		energySpent = GetEnergySpent(kinName, 0);
		return energySpent <= currentEnergy;
	}

	public void UpdateAttackData()	// Called by an animation
	{
		DecreaseEnergy(energySpent);
		strikeId++;
	}


	private float energySpent;
}
