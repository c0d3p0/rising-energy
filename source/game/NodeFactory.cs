using System.Text;
using SCG = System.Collections.Generic;
using ST = System.Threading;

using Godot;
using Godot.Collections;


// Can't lock the access to the variables here or the game will slowdown.
// So a LinkedList was used to handle the requests.
// The finished requests will be removed from the list when the LinkedList.Count
// is bigger that 3, that will make sure both threads will not change the same data
// at the same time and cause a crash.
public class NodeFactory : Node
{
	public void Get(string key, Godot.Object signalData)
	{
		PackedScene ps = this.Call<PackedScene>(globalResource,
						this.GetMethodGet(), key);

		if(ps != null)
		{
			Node node = ps.Instance();
			node.Name = this.CreateUniqueNodeName(node);
			signalData.Call(this.GetMethodSet(), node);
		}
	}

	public void Put(Node requester, string nodeKey,
			string callbackMethod, Dictionary paramMap)
	{
		object[] data = new object[5];
		data[REQUEST_FINISHED] = false;
		data[REQUESTER] = requester;
		data[NODE_KEY] = nodeKey;
		data[CALLBACK_METHOD] = callbackMethod;
		data[PARAM_MAP] = paramMap;
		requestDataList.AddLast(data);
	}

	public void Clear()
	{
		lock(requestDataList) lock(nodeInstanceMap)
		{
			DisposeAllNodeInstances();
			requestDataList.Clear();
			nodeInstanceMap.Clear();
		}
	}

	public void DisposeAllNodeInstances()
	{
		Node n;
		SCG.IEnumerator<SCG.KeyValuePair<ulong, Node>> it =
				nodeInstanceMap.GetEnumerator();

		while(it.MoveNext())
		{
			try
			{
				n = it.Current.Value;

				if(n != null && !n.IsQueuedForDeletion())
					n.QueueFree();
			}
			catch(System.Exception e)
			{
				GD.PushWarning(this.CreateString("Problems trying to dispose a Node: ",
						e.Message, e.StackTrace));
			}
		}
	}

	public void ProcessCreateInstance()
	{
		while(!IsQueuedForDeletion() && IsInsideTree())
		{
			manualResetEvent.WaitOne(ST.Timeout.Infinite);
			object[] data = null;
			Node r = null;
			string nodeKey = null;
			string callbackMethod = null;
			PackedScene ps = null;
			Node n = null;
			
			try
			{
				data = GetFirstValidRequest();

				if(data != null)
				{
					r = data[REQUESTER] as Node;
					nodeKey = data[NODE_KEY] as string;
					ps = this.Call<PackedScene>(globalResource, this.GetMethodGet(), nodeKey);

					if(ps != null)
					{
						callbackMethod = data[CALLBACK_METHOD] as string;
						n = ps.Instance();

						if(nodeInstanceMap.ContainsKey(n.GetInstanceId()))
							nodeInstanceMap[n.GetInstanceId()] = n;
						else
							nodeInstanceMap.Add(n.GetInstanceId(), n);

						n.Name = this.CreateUniqueNodeName(n);
						r.Call(callbackMethod, n, data[PARAM_MAP] as Dictionary);
					}
					else
					{
						GD.PushWarning(this.CreateString(
								"GlobalResources couldn't return a PackedScene with the key nodeKey '",
								nodeKey, "'"));
					}
				}
			}
			catch(System.Exception e)
			{
				GD.PushWarning(GetErrorMessage(r, callbackMethod, nodeKey, e));

				if(n != null && !nodeInstanceMap.ContainsKey(n.GetInstanceId()))
					n.QueueFree();
			}
		}	
	}

	private object[] GetFirstValidRequest()
	{
		if(requestDataList.Count > 0)
		{
			SCG.LinkedListNode<object[]> lln = requestDataList.First;
			object[] data;

			while(lln != null)
			{			
				data = lln.Value;

				if(data != null && !((bool)data[REQUEST_FINISHED]) &&
						data[REQUESTER] != null && data[NODE_KEY] != null &&
						data[CALLBACK_METHOD] != null)
				{
					data[REQUEST_FINISHED] = true;
					return data;
				}
				else
					lln = lln.Next;
			}
		}
		
		return null;
	}

	private void RemoveFirstProcessedRequest()
	{
		if(requestDataList.Count > 3)
		{
			object[] data = requestDataList.First.Value;

			if((bool)data[REQUEST_FINISHED])
				requestDataList.RemoveFirst();
		}
	}

	private string GetErrorMessage(Node requester, string callbackMethod,
			string nodeKey, System.Exception exception)
	{
		StringBuilder sb = new StringBuilder();
		sb.Append("An error happened when trying to create a node! ");

		try
		{
			if(callbackMethod != null)
				sb.Append("NodeKey: ").Append(nodeKey).Append("\n");

			if(callbackMethod != null)
				sb.Append("Method: ").Append(callbackMethod).Append("\n");

			if(requester != null)
				sb.Append("Object: ").Append(requester.Name).Append("\n");
		}
		catch(System.Exception e)
		{
			sb.Append("Error trying to get where it failed!\n");
		}

		sb.Append(exception.Message).Append('\n').Append(exception.StackTrace);
		return sb.ToString();
	}

	private void Initialize()
	{
		globalResource = GetNode(globalResourceNodePath);
		requestDataList = new SCG.LinkedList<object[]>();
		nodeInstanceMap = new SCG.Dictionary<ulong, Node>();
		manualResetEvent = new ST.ManualResetEvent(false);
		nodeFactoryThread = new ST.Thread(new ST.ThreadStart(ProcessCreateInstance));
		nodeFactoryThread.Start();
	}

	public override void _EnterTree()
	{
		Initialize();
	}

	public override void _Process(float delta)
	{
		RemoveFirstProcessedRequest();
	}

	public override void _ExitTree()
	{
		Clear();
	}

	public void SetActive(bool active)
	{
		if(active)
			manualResetEvent.Set();
		else
			manualResetEvent.Reset();
	}


	[Export]
	public string globalResourceNodePath = "/root/GlobalResource";


	private Node globalResource;

	private ST.Thread nodeFactoryThread;
	private ST.ManualResetEvent manualResetEvent;
	private SCG.LinkedList<object[]> requestDataList;
	private SCG.Dictionary<ulong, Node> nodeInstanceMap;
	

	private const byte REQUEST_FINISHED = 0;
	private const byte REQUESTER = 1;
	private const byte NODE_KEY = 2;
	private const byte CALLBACK_METHOD = 3;
	private const byte PARAM_MAP = 4;
}
