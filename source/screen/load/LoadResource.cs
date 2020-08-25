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
		}

		loaded = true;
	}

	private void HandleFinished()
	{
		if(loaded && splashScreenTimer.IsStopped())
		{
			nextNode.Call(this.GetMethodSetActive());
			SetProcess(false);
		}
	}

	private void Initialize()
	{
		globalResource = GetNode(globalResourceNodePath);
		nextNode = GetNode(nextNodeNP);
		splashScreenTimer = GetNode<Timer>(splashScreenTimerNP);

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
		StartRequest();
	}

	public override void _Process(float delta)
	{
		HandleFinished();
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
	public NodePath splashScreenTimerNP;

	private Node globalResource;
	private Node nextNode;
	private Timer splashScreenTimer;
	private bool loaded;
}
