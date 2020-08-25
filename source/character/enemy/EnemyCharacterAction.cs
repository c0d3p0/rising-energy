using Godot;
using Godot.Collections;


public class EnemyCharacterAction : BaseEnemyCharacterAction
{
	public void Hit(Area attackerArea, Area victimArea, uint strikeId,
			float damageTaken, float energyGiven)
	{
		if(!IsStrikeTaken(attackerArea.GetInstanceId(), strikeId))
		{
			float dt = attackerArea.CollisionLayer == reducedDamageStrikeMask ?
					damageTaken * damageReductionScale : damageTaken;
			EmitSignal(this.GetSignalHealthChanged(), dt);
			EmitSignal(this.GetSignalEnergyChanged(), energyGiven);
			AddStrikeTaken(attackerArea.GetInstanceId(), strikeId);
			enemyCharacter.Call(this.GetMethodActivateInput(), false);

			if(this.EmitSignal<bool>(this, this.GetSignalDead()))
				animationStateMachine.Travel("die");
			else
				animationStateMachine.Travel("hit");
		}
	}

	protected void AddStrikeTaken(ulong attackerInstanceId, uint strikeId)
	{
		strikeIdMap.Add(CreateStrikeData(attackerInstanceId, strikeId), null);
	}

	protected bool IsStrikeTaken(ulong attackerInstanceId, ulong strikeId)
	{
		return strikeIdMap.ContainsKey(CreateStrikeData(attackerInstanceId, strikeId));
	}

	protected Array<ulong> CreateStrikeData(ulong attackerInstanceId, ulong strikeId)
	{
		Array<ulong> strikeDataList = new Array<ulong>();
		strikeDataList.Add(attackerInstanceId);
		strikeDataList.Add(strikeId);
		return strikeDataList;
	}

	protected override void Initialize()
	{
		base.Initialize();
		strikeIdMap = new Dictionary<Array<ulong>, object>();
	}


	[Export]
	public uint reducedDamageStrikeMask = 512;

	[Export]
	public float damageReductionScale = 0.1f;


	// Used to prevent the same hit to be taken more than once.
	protected Dictionary<Array<ulong>, object> strikeIdMap;
}
