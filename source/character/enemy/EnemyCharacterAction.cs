using Godot;
using Godot.Collections;


public class EnemyCharacterAction : BaseEnemyCharacterAction
{
	public void Hit(Area attackerArea, Area victimArea, uint strikeId,
			float damageTaken, float energyGiven)
	{
		string aaiid = attackerArea.GetInstanceId().ToString();
		string sid = strikeId.ToString();

		if(!IsStrikeTaken(aaiid, sid))
		{
			float dt = attackerArea.CollisionLayer == reducedDamageStrikeMask ?
					damageTaken * damageReductionScale : damageTaken;
			EmitSignal(this.GetSignalHealthChanged(), dt);
			EmitSignal(this.GetSignalEnergyChanged(), energyGiven);
			AddStrikeTaken(aaiid, sid);
			enemyCharacter.Call(this.GetMethodActivateInput(), false);

			if(this.EmitSignal<bool>(this, this.GetSignalDead()))
				animationStateMachine.Travel("die");
			else
				animationStateMachine.Travel("hit");
		}
	}

	protected void AddStrikeTaken(string attackerInstanceId, string strikeId)
	{
		strikeIdMap.Add(CreateStrikeData(attackerInstanceId, strikeId), null);
	}

	protected bool IsStrikeTaken(string attackerInstanceId, string strikeId)
	{
		return strikeIdMap.ContainsKey(CreateStrikeData(attackerInstanceId, strikeId));
	}

	protected Array<string> CreateStrikeData(string attackerInstanceId, string strikeId)
	{
		Array<string> strikeDataList = new Array<string>();
		strikeDataList.Add(attackerInstanceId);
		strikeDataList.Add(strikeId);
		return strikeDataList;
	}

	protected override void Initialize()
	{
		base.Initialize();
		strikeIdMap = new Dictionary<Array<string>, object>();
	}


	[Export]
	public uint reducedDamageStrikeMask = 512;

	[Export]
	public float damageReductionScale = 0.1f;


	// Used to prevent the same hit to be taken more than once.
	protected Dictionary<Array<string>, object> strikeIdMap;
}
