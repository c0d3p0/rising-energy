using Godot;


public class ClassicProjectileBehavior : BaseEnemyBehavior
{
	private bool Move()
	{
		return this.EmitSignal<bool>(this, this.GetSignalMove(), direction);
	}

	private void UpdateDirection()
	{
		if(direction.x != 0)
			direction.x = moveAxis.x != 0f ? direction.x * moveAxis.x : direction.x;
		else
			direction.x = moveAxis.x != 0f ? moveAxis.x : -1f;

		if(direction.y != 0)
			direction.y = direction.y * moveAxis.y;
		else
			direction.y = moveAxis.y;
	}

	protected override void DecideWhatToDo()
	{
		Move();
	}

	public override void _Ready() {}

	public override Vector3 Direction
	{
		set
		{
			direction = value;
			UpdateDirection();
		}
	}
	

	[Export]
	public Vector3 moveAxis = new Vector3(1.0f, 0.0f, 0.0f);	
}
