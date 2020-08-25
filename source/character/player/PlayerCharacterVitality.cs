using Godot;
using Godot.Collections;


public class PlayerCharacterVitality : StatusCharacterVitality
{
	public void UpdateAttackData() // Called by an animation
	{
		if(maxEnergy > 10f)
		{
			float currentEnergyPercentage = currentEnergy / maxEnergy;
			int index = currentEnergyPercentage > 0.5f ? 0 :
					currentEnergyPercentage > 0.25f ? 1 : 2;
			damageGiven = GetDamageGiven(energyName, index);
			DecreaseEnergy(GetEnergySpent(energyName, index));
			strikeId++;
		}
	}

	public Dictionary GetVitalityMap()
	{
		Dictionary map = new Dictionary();
		int itemId = maxEnergy > 1999f ? 2 : maxEnergy > 1499f ? 1 :
				maxEnergy > 999f ? 0 : -1;
		map.Add("sword_id", itemId);
		map.Add("shield_id", itemId);
		map.Add("max_health", maxHealth);
		map.Add("current_health", currentHealth);
		map.Add("max_energy", maxEnergy);
		map.Add("current_energy", currentEnergy);
		return map;
	}

	public void SetVitality(Spatial energySword, Spatial energyShield,
			float maxHealth, float currentHealth, float maxEnergy, float currentEnergy)
	{
		this.MaxHealth = maxHealth;
		this.CurrentHealth = currentHealth;
		this.MaxEnergy = maxEnergy;
		this.CurrentEnergy = currentEnergy;
		SetItem(swordSpot, energySword);
		SetItem(shieldSpot, energyShield);
		EnergyName = maxEnergy > 1999f ? "monarch" :
				maxEnergy > 1499f ? "avenger" : maxEnergy > 999f ? "blaze" : null;
	}

	public void SetItems(Spatial energySword, Spatial energyShield,
			float maxEnergy)
	{
		this.MaxEnergy = maxEnergy;
		this.CurrentEnergy = maxEnergy;
		SetItem(swordSpot, energySword);
		SetItem(shieldSpot, energyShield);
		EnergyName = maxEnergy > 1500f ? "monarch" :
				maxEnergy > 1000f ? "avenger" : "blaze";
	}

	public bool CanAttack()
	{
		return maxEnergy > 10f;
	}

	public bool CanBlock()
	{
		return maxEnergy > 10f && currentEnergy > 500f;
	}

	protected void SetItem(Spatial spot, Spatial item)
	{
		if(item != null)
		{
			if(spot.GetChildCount() > 0)
			{
				Node n = spot.GetChild(0);
				spot.CallDeferred(this.GetGDMethodRemoveChild(), n);
				n.QueueFree();
			}

			spot.CallDeferred(this.GetGDMethodAddChild(), item);
			item.Visible = true;
		}
	}

	protected override float GetDamageGiven(string key, int index)
	{
		float damageGiven = GetVitalityMapValue(damageGivenMap, key, index);

		if(damageGiven > currentEnergy)
			damageGiven = GetVitalityMapValue(damageGivenMap, "default", 0);
			
		return damageGiven;
	} 

	protected override void RecoverEnergy(float delta)
	{
		this.CurrentEnergy = currentEnergy +
				((maxEnergy / 1000f) * energyRecoverPerSecond * delta);
	}

	protected override void Initialize()
	{
		base.Initialize();
		swordSpot = GetNode<Spatial>(swordSpotNP);
		shieldSpot = GetNode<Spatial>(shieldSpotNP);
	}

	public override void OnStrikeAreaEntered(Area area)
	{
		area.EmitSignal(this.GetSignalHit(), strikeArea, area,
				strikeId, damageGiven, 0f);
	}

	public override void _EnterTree()
	{
		Initialize();
	}

	public string EnergyName
	{
		get
		{
			return energyName;
		}	
		set
		{
			if(strikeArea.IsInGroup(energyName))
				strikeArea.RemoveFromGroup(energyName);

			energyName = value;

			if(energyName != null)
				strikeArea.AddToGroup(energyName);
		}
	}
	

	[Export]
	public NodePath swordSpotNP;

	[Export]
	public NodePath shieldSpotNP;


	protected Spatial swordSpot;
	protected Spatial shieldSpot;

	protected float damageGiven;
	protected string energyName;
}
