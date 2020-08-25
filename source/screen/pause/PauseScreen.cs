using Godot;


public class PauseScreen : Node
{
	public void OnResumeButtonPressed()
	{
		if(paused)
			Pause();
	}

	public void OnInputMappingButtonPressed()
	{
		inputMappingScreen.Call(this.GetMethodInteract(), contentControl);
	}

	public void OnQuitButtonPressed()
	{
		ReturnToTitleScreen();
	}

	private void Pause()
	{
		paused = !paused;
		Input.SetMouseMode(paused ? Input.MouseMode.Visible : Input.MouseMode.Captured);
		GetTree().Paused = paused;
		pauseScreenControl.Visible = paused;
		pauseScreenControl.ShowBehindParent = !paused;
		pauseScreenControl.ShowOnTop = paused;
	}

	private void ReturnToTitleScreen()
	{
		globalData.Call(this.GetMethodPut(), "scene_to_load",
				this.GetScenePath(titleScreenScenePath));
		GetTree().ChangeScene(this.GetScenePath(loadScreenScenePath));
		GetTree().Paused = false;
	}

	private void HandlePauseKeyPressed()
	{
		if(Input.IsActionJustPressed("pause_game"))
			Pause();
	}

	private void Initialize()
	{
		globalData = GetNode(globalDataNodePath);
		pauseScreenControl = GetNode<Control>(pauseScreenControlNP);
		inputMappingScreen = GetNode<Control>(inputMappingScreenNP);
		contentControl = GetNode<Control>(contentControlNP);
		pauseScreenControl.Visible = paused;
		manualPause = this.Call<bool>(globalData, this.GetMethodGet(), "manual_pause");
	}

	public override void _EnterTree()
	{
		Initialize();
	}

	public override void _Process(float delta)
	{
		HandlePauseKeyPressed();
	}

	public override void _Notification(int what)
	{
		if(!paused && !manualPause && what == MainLoop.NotificationWmFocusOut)
			Pause();
	}


	[Export]
	public NodePath pauseScreenControlNP;
	
	[Export]
	public NodePath contentControlNP;

	[Export]
	public NodePath inputMappingScreenNP;

	[Export]
	public string loadScreenScenePath;

	[Export]
	public string titleScreenScenePath;

	[Export]
	public string globalDataNodePath = "/root/GlobalData";


	private Node globalData;

	private Control pauseScreenControl;
	private Control contentControl;
	private Control inputMappingScreen;
	private bool paused;
	private bool manualPause;
}
