using Godot;


public class BaseEnemyCharacter : KinematicBody
{
	public void IsDead(Godot.Object signalData)
	{
		signalData.Call(this.GetMethodSet(), enemyCharacterVitality.Dead);
	}

	public void Move(Vector3 direction, Godot.Object signalData)
	{
		signalData.Call(this.GetMethodSet(),
				enemyCharacterAction.Move(direction, direction));
	}

	public void Move(Vector3 direction, Vector3 bodyDirection, Godot.Object signalData)
	{
		signalData.Call(this.GetMethodSet(),
				enemyCharacterAction.Move(direction, bodyDirection));
	}

	public void Attack(string attackAnimation, Vector3 direction,
			Godot.Object signalData)
	{
		signalData.Call(this.GetMethodSet(),
				enemyCharacterAction.Attack(direction));
	}

	public void Attack(Vector3 direction, Godot.Object signalData)
	{
		signalData.Call(this.GetMethodSet(),
				enemyCharacterAction.Attack(direction));
	}

	public void TransitTo(string actionName)
	{
		enemyCharacterAction.TransitTo(actionName);
	}

	public void ActivateInput(bool active)
	{
		enemyBehavior.SetProcessBehavior(active);
	}

	protected virtual void Initialize()
	{
		enemyBehavior = GetNode<BaseEnemyBehavior>(enemyBehaviorNP);
		enemyCharacterAction = GetNode<BaseEnemyCharacterAction>(
				enemyCharacterActionNP);
		enemyCharacterVitality = GetNode<BaseCharacterVitality>(
				enemyCharacterVitalityNP);

		if(nodeFactory != null && nodeFactoryNP != null)
			nodeFactory = GetNodeOrNull<Node>(nodeFactoryNP);

		if(target == null && targetNP != null)
			target = GetNodeOrNull<Spatial>(targetNP);
	}

	public override void _EnterTree()
	{
		Initialize();
	}

	public override void _ExitTree()
	{
		QueueFree();
	}
	
	public void SetTarget(Spatial target)
	{
		this.target = target;

		if(enemyBehavior != null)
			enemyBehavior.Target = target;
	}

	public void SetDirection(Vector3 direction)
	{
		this.direction = direction;

		if(enemyBehavior != null)
			enemyBehavior.Direction = direction;
	}

	public void SetNodeFactory(Node nodeFactory)
	{
		this.nodeFactory = nodeFactory;

		if(enemyBehavior != null)
			enemyBehavior.NodeFactory = nodeFactory;
	}

	public void SetDespawnTimeTolerance(float despawnTimeTolerance)
	{
		this.despawnTimeTolerance = despawnTimeTolerance;

		if(enemyCharacterAction != null)
			enemyCharacterAction.DespawnTimeTolerance = despawnTimeTolerance;
	}

	public float DespawnTimeTolerance
	{
		get
		{
			return despawnTimeTolerance;
		}
	}

	public Spatial Target
	{
		get
		{
			return target;
		}
	}

	public Vector3 Direction
	{
		get
		{
			return direction;
		}
	}

	public Node NodeFactory
	{
		get
		{
			return nodeFactory;
		}
	}


	[Export]
	public NodePath nodeFactoryNP;

	[Export]
	public NodePath targetNP;

	[Export]
	public NodePath enemyBehaviorNP;

	[Export]
	public NodePath enemyCharacterActionNP;

	[Export]
	public NodePath enemyCharacterVitalityNP;

	[Export]
	public bool immortal;


	protected Node nodeFactory;
	protected Spatial target;
	protected Vector3 direction;
	protected BaseEnemyBehavior enemyBehavior;
	protected BaseEnemyCharacterAction enemyCharacterAction;
	protected BaseCharacterVitality enemyCharacterVitality;
	protected float despawnTimeTolerance;
}
