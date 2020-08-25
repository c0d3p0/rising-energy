using Godot;


public abstract class BaseEnemyBehavior : Node
{
	public void SetProcessBehavior(bool active)
	{
		SetProcess(active);
		SetPhysicsProcess(active);
	}

	public virtual void SetDirectionToTarget()
	{
		if(target != null)
		{
			float enemyX = enemyCharacter.GlobalTransform.origin.x;
			float targetX = target.GlobalTransform.origin.x;
			direction.x = enemyX - targetX > 0f ? -1f : 1f;
		}
		else
			direction.x = (this.RandiRange(rng, 0, 1) * 2) - 1;
	}

	protected virtual void Initialize()
	{
		enemyCharacter = GetNode<Spatial>(enemyCharacterNP);
		rng = new RandomNumberGenerator();
	}

	public override void _EnterTree()
	{
		Initialize();
	}

	public override void _Ready()
	{
		SetDirectionToTarget();
	}

	public override void _PhysicsProcess(float delta)
	{
		DecideWhatToDo();
	}

	protected abstract void DecideWhatToDo();

	public Node NodeFactory
	{
		set
		{
			nodeFactory = value;
		}	
	}

	public Spatial Target
	{
		set
		{
			target = value;
		}
	}

	public virtual Vector3 Direction
	{
		set
		{
			direction = value;
		}
	}
	

	[Export]
	public NodePath enemyCharacterNP;


	protected Node nodeFactory;
	protected Spatial target;

	protected Spatial enemyCharacter;
	protected RandomNumberGenerator rng;
	protected Vector3 direction;
}
