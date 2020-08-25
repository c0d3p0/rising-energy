using Godot;


public class BaseBossCharacter : BaseEnemyCharacter
{
	public void CanAttack(string kinName, Godot.Object signalData)
	{
		signalData.Call(this.GetMethodSet(), bossVitality.CanAttack(kinName));
	}

	public void SetKinSpawnPivot(Vector3 position)
	{
		kinSpawnPivot = position;
	}

	protected override void Initialize()
	{
		base.Initialize();
		bossVitality = enemyCharacterVitality as BaseBossVitality;
	}
	
	public Vector3 KinSpawnPivot
	{
		get
		{
			return kinSpawnPivot;
		}
	}


	protected BaseBossVitality bossVitality;

	private Vector3 kinSpawnPivot;
}

