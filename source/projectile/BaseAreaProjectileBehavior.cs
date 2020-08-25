using Godot;


public abstract class BaseAreaProjectileBehavior : BaseEnemyBehavior
{
	protected virtual void Reposition()
	{
		if(repositionEnabled)
		{
			float rX = this.RandfRange(rng, -repositionRange.x, repositionRange.x);
			float rY = this.RandfRange(rng, 0f, repositionRange.y);
			enemyCharacter.Translation = new Vector3(repositionPivot.x + rX,
					repositionPivot.y + rY, repositionPivot.z);
		}
	}

	protected void ForceDefaultDirection()
	{
		if(direction.Length() == 0)
			direction.x = -1;
	}

	public Vector3 RepositionPivot
	{
		set
		{
			repositionPivot = value;
			Reposition();
		}	
	}


	[Export]
	public Vector2 repositionRange = new Vector2(12f, 0f);

	[Export]
	public bool repositionEnabled = false;


	protected Vector3 repositionPivot;
}
