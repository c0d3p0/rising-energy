using SCG = System.Collections.Generic;

using Godot;
using Godot.Collections;


public class LoadResource : Node
{
	private void LoadResources()
	{
		if(resourcePathList != null)
		{
			int index;
			string[] split;
			SCG.IEnumerator<string> it = resourcePathList.GetEnumerator();

			while(it.MoveNext())
			{
				split = it.Current.Split("/");
				index = split.Length - 1;
				globalResource.Call(this.GetMethodPut(),
						split[index].Split(".")[0], ResourceLoader.Load(it.Current));			
			}

			WriteDebugMessage("Resources stored inside GlobalResources!");
		}

		preload.Visible = true;
		preload.Connect(this.GetGDSignalTreeExited(),
				this, nameof(HandleFinished));
	}

	private void HandleFinished()
	{
		SetProcess(false);
		nextNode.SetProcess(true);
		WriteDebugMessage("Resources Loaded!");
	}

	private void Initialize()
	{
		globalResource = GetNode(globalResourceNodePath);
		nextNode = GetNode(nextNodeNP);
		preload = GetNode<Spatial>(preloadNP);

		Node globalData = GetNode(globalDataNodePath);
		globalData.Call(this.GetMethodPut(), "scene_to_load",
				this.GetScenePath(nextScreenScenePath));
	}

	private void StartRequest()
	{
		Node taskRunner = GetNode(taskRunnerNodePath);
		taskRunner.Call(this.GetMethodPut(), this, "LoadResources");
	}

	public override void _EnterTree()
	{
		Initialize();
	}

	public override void _Ready()
	{
		SetProcess(active);
		StartRequest();
	}

	private void WriteDebugMessage(string message)
	{
		if(OS.IsDebugBuild())
			GD.PushWarning(message);				
	}


	[Export]
	public Array<string> resourcePathList;

	[Export]
	public string nextScreenScenePath = "screen/title_screen";

	[Export]
	public string globalDataNodePath = "/root/GlobalData";

	[Export]
	public string globalResourceNodePath = "/root/GlobalResource";

	[Export]
	public string taskRunnerNodePath = "/root/TaskRunner";

	[Export]
	public NodePath nextNodeNP;

	[Export]
	public NodePath preloadNP;

	[Export]
	public bool active = true;


	private Node globalResource;
	private Node nextNode;
	private Spatial preload;
}
