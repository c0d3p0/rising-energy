using Godot;


public class BaseAreaProjectile : BaseEnemyCharacter
{
	protected override void Initialize()
	{
		base.Initialize();
		projectileBehavior = enemyBehavior as BaseAreaProjectileBehavior;
	}

	public void SetRepositionPivot(Vector3 position)
	{
		repositionPivot = position;
		projectileBehavior.RepositionPivot = repositionPivot;
	}

	public Vector3 RepositionPivot
	{
		get
		{
			return repositionPivot;
		}
	}


	public BaseAreaProjectileBehavior projectileBehavior;
	protected Vector3 repositionPivot;
}
