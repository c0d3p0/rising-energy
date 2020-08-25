using Godot;
using Godot.Collections;


public class MemoryDrainer : Node
{
	public void SetKinSpawnData(Node node, Dictionary paramMap)
	{
		node.Call(this.GetMethodSetTarget(), enemyContainer);
		node.Connect(this.GetGDSignalTreeEntered(), this,
				this.GetMethodIncreaseInstanceCount());
		node.Connect(this.GetGDSignalTreeExited(), this,
				this.GetMethodOnKinDestroyed(), this.CreateSingleBind(node));
		requestedNodeList.Add(node);
	}

	public void OnKinDestroyed(Node kin)
	{
		nodesRemoved++;

		if(nodesRemoved % 100 == 0)
			GD.PushWarning("NodesRemoved: " + nodesRemoved);	
	}

	public void IncreaseInstanceCount()
	{
		instanceCount++;
	}

	private void RequestNodeInstances()
	{
		if(nodeRequestedAmount < nodeInstanceAmount)
		{
			for(int i = 0; i < preInstanceKeyList.Count; i++)
			{
				if(requestedNodeList.Count < nodeInstanceAmount)
				{
					nodeRequestedAmount++;
					nodeFactory.Call(this.GetMethodPut(), this,
							preInstanceKeyList[i], this.GetMethodSetKinSpawnData(), null);
				}
				else
					break;
			}

			GD.PushWarning("Loop finished!");
		}
	}

	private void AddRequestedNodesToTheScene()
	{
		if(requestedNodeList.Count >= nodeInstanceAmount &&
				nodeToAddIndex < requestedNodeList.Count)
		{
			Node n = requestedNodeList[nodeToAddIndex];
			nodeToAddIndex++;

			if(n != null && !n.IsInsideTree())
				this.AddChildToEnemyContainer(this, n);

			if(nodeToAddIndex % 100 == 0)
				GD.PushWarning("NodeToAddIndex: " + nodeToAddIndex);
		}
	}

	private void RemoveRequestedNodesFromTheScene()
	{
		if(enemyContainer.GetChildCount() > 0)
		{
			enemyContainer.CallDeferred(
					this.GetGDMethodRemoveChild(), enemyContainer.GetChild(0));
		}
	}

	private void HandleFinished()
	{
		if(nodesRemoved >= nodeInstanceAmount)
		{
			GD.PushWarning("Finishing!");
			nodeFactory.Call(this.GetMethodSetActive(), false);
			nodeFactory.Call(this.GetMethodClear());
			loadScreen.SetProcess(true);
			SetProcess(false);
		}
	}

	private void Initialize()
	{
		requestedNodeList = new Array<Node>();
		loadScreen = GetNode(loadScreenNP);
		enemyContainer = GetNode<Spatial>(enemyContainerNP);
		nodeFactory = GetNode(nodeFactoryNodePath);
		nodeFactory.Call(this.GetMethodClear());
		nodeFactory.Call(this.GetMethodSetActive(), true);
	}

	public override void _EnterTree()
	{
		Initialize();
	}

	public override void _Process(float delta)
	{
		RequestNodeInstances();
		AddRequestedNodesToTheScene();
		RemoveRequestedNodesFromTheScene();
		HandleFinished();
	}


	[Export]
	public NodePath loadScreenNP;

	[Export]
	public NodePath enemyContainerNP;

	[Export]
	public string nodeFactoryNodePath = "/root/NodeFactory";

	[Export]
	public Array<string> preInstanceKeyList;

	[Export]
	public int nodeInstanceAmount = 10000;


	private Node nodeFactory;
	private Node loadScreen;
	private Spatial enemyContainer;
	private int instanceCount;

	private Array<Node> requestedNodeList;
	private int nodeRequestedAmount;
	private int nodesRemoved;
	private int nodeToAddIndex;
	private bool nodesAdded;
}
