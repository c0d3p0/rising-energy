public class BaseAreaProjectileInitializer : BaseEnemyInitializer
{
	protected override void InitializeEnemyBehavior()
	{
		base.InitializeEnemyBehavior();
		(enemyBehavior as BaseAreaProjectileBehavior).RepositionPivot =
				(enemyCharacter as BaseAreaProjectile).RepositionPivot;
	}
}
