using Godot;


public class LoadScreen : Node
{
	private void HandleChangeScene()
	{
		if(loadedScene != null)
		{
			taskRunner.Call(this.GetMethodSetActive(), false);
			taskRunner.Call(this.GetMethodClear());
			GetTree().ChangeSceneTo(loadedScene);
		}
	}

	private void LoadScene()
	{
		loadedScene = ResourceLoader.Load(scenePath) as PackedScene;
	}

	private void Initialize()
	{
		Node globalData = GetNode(globalDataNodePath);
		scenePath = this.Call<string>(globalData, this.GetMethodGet(), "scene_to_load");

		taskRunner = GetNode(taskRunnerNodePath);
		taskRunner.Call(this.GetMethodSetActive(), false);
		taskRunner.Call(this.GetMethodClear());
		taskRunner.Call(this.GetMethodPut(), this, "LoadScene");
		taskRunner.Call(this.GetMethodSetActive(), true);
	}

	public override void _EnterTree()
	{
		Initialize();
	}

	public override void _Ready()
	{
		SetProcess(changeSceneAfterLoad);
	}

	public override void _Process(float delta)
	{
		HandleChangeScene();
	}


	[Export]
	public bool changeSceneAfterLoad = true;

	[Export]
	public string globalDataNodePath = "/root/GlobalData";

	[Export]
	public string taskRunnerNodePath = "/root/TaskRunner";


	private Node taskRunner;
	private string scenePath;
	private PackedScene loadedScene;
}
