using SCG = System.Collections.Generic;

using Godot;
using Godot.Collections;


public class TeleportBossBehavior : BaseBossBehavior
{
	public void Teleport()
	{
		int index = this.RandiRange(rng, 0, teleportPoints.Length - 1);
		enemyCharacter.Translation = teleportPoints[index];
	}

	protected bool TryToTeleport()
	{
		if(ShouldTeleport() &&
				this.EmitSignal<bool>(this, this.GetSignalMove(), direction))
		{
			SetProcessBehavior(false);
			return true;
		}

		return false;
	}

	protected bool TryToIdle()
	{
		return this.EmitSignal<bool>(this, this.GetSignalMove(), Vector3.Zero, direction);
	}

	protected override void DecideWhatToDo()
	{
		AddKinToTheScene();
		TryToRequestKin();
		SetDirectionToTarget();

		if(TryToTeleport() || TryToSpell() || TryToIdle())
			return;
	}

	protected bool ShouldTeleport()
	{
		return this.RandiRange(rng, rngMinValue, rngMaxValue)
				<= teleportTriggerRng;
	}

	protected void InitializeTeleportPoints()
	{
		teleportPoints = new Vector3[teleportPointList.Count];
		SCG.IEnumerator<Vector3> it = teleportPointList.GetEnumerator();
		int index = 0;

		while(it.MoveNext())
			teleportPoints[index++] = kinSpawnPivot + it.Current;
	}

	protected override void Initialize()
	{
		base.Initialize();
	}

	public override void _Ready()
	{
		base._Ready();
		InitializeTeleportPoints();
	}


	[Export]
	public float teleportTriggerRng = 100f;

	[Export]
	public Array<Vector3> teleportPointList;


	private Vector3[] teleportPoints;
}
