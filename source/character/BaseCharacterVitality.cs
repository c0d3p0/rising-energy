using Godot;
using Godot.Collections;


public class BaseCharacterVitality : Node
{
	public void DecreaseHealth(float amount)
	{
		this.CurrentHealth = currentHealth - amount;
	}

	public void DecreaseEnergy(float amount)
	{
		this.CurrentEnergy = currentEnergy - amount;
	}

	protected virtual void RecoverHealth(float delta)
	{
		if(currentHealth > 0)
			this.CurrentHealth = currentHealth + (healthRecoverPerSecond * delta);
	}

	protected virtual void RecoverEnergy(float delta)
	{
		this.CurrentEnergy = currentEnergy + (energyRecoverPerSecond * delta);
	}

	public override void _Process(float delta)
	{
		RecoverHealth(delta);
		RecoverEnergy(delta);
	}

	protected float GetDamageGiven(int index)
	{
		return GetVitalityMapValue(damageGivenMap, "default", index);
	}

	protected virtual float GetDamageGiven(string key, int index)
	{
		return GetVitalityMapValue(damageGivenMap, key, index);
	}

	protected virtual float GetEnergyTaken(int index)
	{
		return GetVitalityMapValue(energyTakenMap, "default", index);
	}

	protected float GetEnergyTaken(string key, int index)
	{
		return GetVitalityMapValue(energyTakenMap, key, index);
	}

	protected float GetEnergySpent(int index)
	{
		return GetVitalityMapValue(energySpentMap, "default", index);
	}

	protected float GetEnergySpent(string key, int index)
	{
		return GetVitalityMapValue(energySpentMap, key, index);
	}

	protected float GetVitalityMapValue(
			Dictionary<string, Vector3> map, string key, int index)
	{
		if(map != null)
		{
			Vector3 values;

			if(map.TryGetValue(key, out values))
			{
				if(index == 0)
					return values.x;
				else if(index == 1)
					return values.y;
				else if(index == 2)
					return values.z;
				else
					return values.x;
			}
		}

		return 0f;
	}

	protected virtual void Initialize()
	{
		strikeArea = GetNode<Area>(strikeAreaNP);

		if(hurtAreaNP != null)
			hurtArea = GetNode<Area>(hurtAreaNP);
	}

	public override void _Ready()
	{
		CurrentHealth = currentHealth;
		CurrentEnergy = currentEnergy;
	}

	public override void _EnterTree()
	{
		Initialize();
	}

	public void OnHurtAreaEntered(Area area)
	{
		area.EmitSignal(this.GetGDSignalAreaEntered(), hurtArea);
	}

	public virtual void OnStrikeAreaEntered(Area area)
	{
		area.EmitSignal(this.GetSignalHit(), strikeArea, area,
				strikeId, GetDamageGiven(0), GetEnergyTaken(0));
	}

	public virtual float MaxHealth
	{
		get
		{
			return maxHealth;
		}
		set
		{
			maxHealth = value;

			if(maxHealth < 0)
				maxHealth = 0;
		}
	}

	public virtual float MaxEnergy
	{
		get
		{
			return maxEnergy;
		}
		set
		{
			maxEnergy = value;

			if(maxEnergy < 0)
				maxEnergy = 0;
		}
	}

	public virtual float CurrentHealth
	{
		get
		{
			return currentHealth;
		}
		set
		{
			currentHealth = value;

			if(currentHealth > maxHealth)
				currentHealth = maxHealth;
			else if(currentHealth <= 0)
				currentHealth = 0;
		}
	}

	public virtual float CurrentEnergy
	{
		get
		{
			return currentEnergy;
		}
		set
		{
			currentEnergy = value;
			
			if(currentEnergy > maxEnergy)
				currentEnergy = maxEnergy;
			else if(currentEnergy < 0)
				currentEnergy = 0;
		}
	}

	public bool Dead
	{
		get
		{
			return currentHealth <= 0f;
		}
	}


	[Export]
	public NodePath strikeAreaNP;

	[Export]
	public NodePath hurtAreaNP;

	[Export]
	public float maxHealth;

	[Export]
	public float maxEnergy;

	[Export]
	public float currentHealth;

	[Export]
	public float currentEnergy;

	[Export]
	public float healthRecoverPerSecond = 2f;

	[Export]
	public float energyRecoverPerSecond = 4f;

	[Export]
	public Dictionary<string, Vector3> damageGivenMap;

	[Export]
	public Dictionary<string, Vector3> energyTakenMap;

	[Export]
	public Dictionary<string, Vector3> energySpentMap;


	public Area strikeArea;
	public Area hurtArea;

	protected ulong strikeId;
}
