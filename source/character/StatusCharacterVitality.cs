public class StatusCharacterVitality : BaseCharacterVitality
{
	public override void _Ready()
	{
		EmitSignal(this.GetSignalHealthChanged(), 0f, maxHealth, currentHealth);
		EmitSignal(this.GetSignalEnergyChanged(), 0f, maxEnergy, currentEnergy);
	}

	public override float MaxHealth
	{
		set
		{
			base.MaxHealth = value;
			EmitSignal(this.GetSignalHealthChanged(), 0f, maxHealth, currentHealth);
		}
	}

	public override float MaxEnergy
	{
		set
		{
			base.MaxEnergy = value;
			EmitSignal(this.GetSignalEnergyChanged(), 0f, maxEnergy, currentEnergy);
		}
	}

	public override float CurrentHealth
	{
		set
		{
			base.CurrentHealth = value;
			EmitSignal(this.GetSignalHealthChanged(), currentHealth);
		}
	}

	public override float CurrentEnergy
	{
		set
		{
			base.CurrentEnergy = value;
			EmitSignal(this.GetSignalEnergyChanged(), currentEnergy);
		}
	}
}
