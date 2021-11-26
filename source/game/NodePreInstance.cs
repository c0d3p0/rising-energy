using SCG = System.Collections.Generic;

using Godot;
using Godot.Collections;


// TODO: Unused, REMOVE
public class NodePreInstance : Node
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
		if(enemyContainer.GetChildCount() == 0)
		{
			nodeFactory.Call(this.GetMethodSetActive(), false);
			nodeFactory.Call(this.GetMethodClear());
			loadScreen.SetProcess(true);
		}
	}

	public void IncreaseInstanceCount()
	{
		instanceCount++;
	}

	private void RequestNodeInstances()
	{
		for(int i = 0; i < preInstanceKeyList.Count; i++)
		{
			nodeFactory.Call(this.GetMethodPut(), this,
					preInstanceKeyList[i], this.GetMethodSetKinSpawnData(), null);
		}
	}

	private void AddRequestedNodesToTheScene()
	{
		if(requestedNodeList.Count == preInstanceKeyList.Count && !nodesAdded)
		{
			SCG.IEnumerator<Node> it = requestedNodeList.GetEnumerator();
			Node n;

			while(it.MoveNext())
			{
				n = it.Current;

				if(n != null && !n.IsInsideTree())
					this.AddChildToEnemyContainer(this, n);
			}

			nodesAdded = true;
		}
	}

	private void RemoveRequestedNodesFromTheScene()
	{
		if(instanceCount == preInstanceKeyList.Count)
		{
			for(int i = 0; i < enemyContainer.GetChildCount(); i++)
				enemyContainer.CallDeferred(this.GetGDMethodRemoveChild(), enemyContainer.GetChild(i));
		}
	}

	private void Initialize()
	{
		loadScreen = GetNode(loadScreenNP);
		enemyContainer = GetNode<Spatial>(enemyContainerNP);
		nodeFactory = GetNode(nodeFactoryNodePath);
		nodeFactory.Call(this.GetMethodClear());
		nodeFactory.Call(this.GetMethodSetActive(), true);
		requestedNodeList = new Array<Node>();
	}

	public override void _EnterTree()
	{
		Initialize();
	}

	public override void _Process(float delta)
	{
		AddRequestedNodesToTheScene();
		RemoveRequestedNodesFromTheScene();
	}

	public void SetActive()
	{
		RequestNodeInstances();
	}


	[Export]
	public NodePath loadScreenNP;

	[Export]
	public NodePath enemyContainerNP;

	[Export]
	public string nodeFactoryNodePath = "/root/NodeFactory";

	[Export]
	public Array<string> preInstanceKeyList;


	private Node nodeFactory;
	private Node loadScreen;
	private Spatial enemyContainer;
	private int instanceCount;

	private Array<Node> requestedNodeList;
	private bool nodesAdded;
}
