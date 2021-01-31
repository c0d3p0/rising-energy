using SCG = System.Collections.Generic;

using Godot;
using Godot.Collections;


public class PreLoader : Node
{
	public void RequestResources()
	{
		if(materialRenderScenePathList != null)
		{
			SCG.IEnumerator<string> it = materialRenderScenePathList.GetEnumerator();
			PackedScene ps;
			Spatial s;
			ulong instanceId;
			int index = 0;

			while(it.MoveNext())
			{
				ps = ResourceLoader.Load<PackedScene>(it.Current);
				s = ps.Instance() as Spatial;
				instanceId = s.GetInstanceId();
				materialRenderSceneMap.Add(instanceId, s);
				materialRenderIdsList.Add(index++, instanceId);
					
				if(OS.IsDebugBuild())
					GD.PushWarning("RequestFinished: " + s.Name);
			}

			if(OS.IsDebugBuild())
				GD.PushWarning("All Resources requests finished!");
		}
	}

	private void AddRequestedNodesToTheTree()
	{
		if(renderTimer.IsStopped())
		{
			Spatial s;
			ulong checkingId;

			for(int i = 0; i < materialRenderScenePathList.Count; i++)
			{
				if(materialRenderIdsList.ContainsKey(i) &&
						materialRenderIdsList.TryGetValue(i, out checkingId) &&
						!addedMaterialRenderIdsList.ContainsKey(checkingId))
				{
					s = materialRenderSceneMap[checkingId];
					materialRenderContainer.CallDeferred(this.GetGDMethodAddChild(), s);
					addedMaterialRenderIdsList.Add(checkingId, null);
					renderTimer.Start();
					addedNodeIdList.Add(s.GetInstanceId());

					if(OS.IsDebugBuild())
						GD.PushWarning("Adding: " + s.Name);
				}
			}
		}
	}

	public void RemoveRequestedNodesFromTheTree()
	{
		if(renderTimer.IsStopped() && addedNodeIdList.Count > 0)
		{
			ulong id = addedNodeIdList[0];
			Spatial s = materialRenderSceneMap[id];
			s.QueueFree();
			addedNodeIdList.RemoveAt(0);

			if(OS.IsDebugBuild())
				GD.PushWarning("Removing: " + s.Name);
		}
	}

	private void TryToFinish()
	{
		if(renderTimer.IsStopped() && addedMaterialRenderIdsList.Count >=
				materialRenderScenePathList.Count && 
				materialRenderContainer.GetChildCount() < 2)
		{
			if(OS.IsDebugBuild())
				GD.PushWarning("Resources rendering finished!");
			
			nextNode.SetProcess(true);
			SetProcess(false);
		}
	}

	private void Initialize()
	{
		materialRenderSceneMap = new Dictionary<ulong, Spatial>();
		materialRenderIdsList = new Dictionary<int, ulong>();
		addedMaterialRenderIdsList = new Dictionary<ulong, object>();
		addedNodeIdList = new Array<ulong>();
		
		taskRunner.Call(this.GetMethodSetActive(), true);
		taskRunner.Call(this.GetMethodPut(), this, nameof(RequestResources));
		PutGlobal("sceneToLoad", this.GetScenePath(nextScreenScenePath));
		AudioServer.SetBusMute(AudioServer.GetBusIndex("Master"), true);
	}

	private void ObtainNodes()
	{
		globalData = GetNode(globalDataNodePath);
		taskRunner = GetNode(taskRunnerNodePath);
		nextNode = GetNode(nextNodeNP);
		materialRenderContainer = GetNode<Spatial>(materialRenderContainerNP);
		renderTimer = GetNode<Timer>(renderTimerNP);
	}

	private void PutGlobal(string key, object value)
	{
		globalData.Call(this.GetMethodPut(), key, value);
	}

	public override void _EnterTree()
	{
		ObtainNodes();
		Initialize();
	}

	public override void _Process(float delta)
	{
		RemoveRequestedNodesFromTheTree();
		AddRequestedNodesToTheTree();
		TryToFinish();
	}

	public override void _ExitTree()
	{
		AudioServer.SetBusMute(AudioServer.GetBusIndex("Master"), false);
	}


	[Export]
	public string nextScreenScenePath = "screen/title_screen";

	[Export]
	public string taskRunnerNodePath = "/root/TaskRunner";

	[Export]
	public string globalDataNodePath = "/root/GlobalData";

	[Export]
	public NodePath nextNodeNP;

	[Export]
	public NodePath materialRenderContainerNP;

	[Export]
	public NodePath renderTimerNP;

	[Export]
	public Array<string> materialRenderScenePathList;


	private Node taskRunner;
	private Node globalData;
	private Node nextNode;
	private Spatial materialRenderContainer;
	private Timer renderTimer;

	private Dictionary<ulong, Spatial> materialRenderSceneMap;
	private Dictionary<int, ulong> materialRenderIdsList;
	private Dictionary<ulong, object> addedMaterialRenderIdsList;
	private Array<ulong> addedNodeIdList;
}
