using Godot;
using Godot.Collections;


public class TitleScreen : Node
{
	public void OnNewGameButtonPressed()
	{
		PrepareNewGameData();
		ChangeSceneToLoadScreen();
	}

	public void OnSurvivalGameButtonPressed()
	{
		PrepareSurvivalGameData();
		ChangeSceneToLoadScreen();
	}

	public void OnGameplayDataButtonPressed()
	{
		gameplayDataScreen.Call(this.GetMethodInteract(), contentControl);
	}

	public void OnLastGameplayResultButtonPressed()
	{
		lastGameplayResultScreen.Call(this.GetMethodInteract(),
				contentControl, gameplayResult);
	}

	public void OnInputMappingButtonPressed()
	{
		inputMappingScreen.Call(this.GetMethodInteract(), contentControl);
	}

	public void OnCreditsButtonPressed()
	{
		globalData.Call(this.GetMethodClear());
		globalData.Call(this.GetMethodPut(), "short_credits", true);
		globalData.Call(this.GetMethodPut(), "load_screen",
				this.GetScenePath(loadScreenScenePath));
		globalData.Call(this.GetMethodPut(), "scene_to_load",
				this.GetScenePath(creditsScreenScenePath));
		ChangeSceneToLoadScreen();
	}

	public void OnQuitButtonPressed()
	{
		GetTree().Quit();
	}

	public void OnContentControlVisibilityChanged()
	{
		if(contentControl.Visible && gameplayResult)
		{
			gameplayResult = false;
			animationPlayer.Play("enable_menu");
		}
	}
	
	public void OnToggleCheatCode(string cheatCode, bool active)
	{
		if(active && cheatCodeLabelMap.ContainsKey(cheatCode))
			cheatLabel.Text = cheatCodeLabelMap[cheatCode];
		else
			cheatLabel.Text = "";
	}

	private void PrepareNewGameData()
	{
		globalData.Call(this.GetMethodClear());
		globalData.Call(this.GetMethodPut(), "total_time", 0);
		globalData.Call(this.GetMethodPut(), "game_type", "normalGame");
		globalData.Call(this.GetMethodPut(), "level_progress", 0);
		globalData.Call(this.GetMethodPut(), "sword_id", -1);
		// globalData.Call(this.GetMethodPut(), "shield_id", -1);
		globalData.Call(this.GetMethodPut(), "max_health", 1000f);
		globalData.Call(this.GetMethodPut(), "current_health", 1000f);
		globalData.Call(this.GetMethodPut(), "max_energy", 1f);
		globalData.Call(this.GetMethodPut(), "current_energy", 1f);
		globalData.Call(this.GetMethodPut(), "deaths", 0);
		globalData.Call(this.GetMethodPut(), "load_screen",
				this.GetScenePath(loadScreenScenePath));
		globalData.Call(this.GetMethodPut(), "scene_to_load",
				this.GetScenePath(firstLevelScenePath));

		globalData.Call(this.GetMethodPut(),
				this.Call<Dictionary>(cheatCode, this.GetMethodGet()));
	}

	private void PrepareSurvivalGameData()
	{
		globalData.Call(this.GetMethodClear());
		globalData.Call(this.GetMethodPut(), "max_health", 1000f);
		globalData.Call(this.GetMethodPut(), "current_health", 1000f);
		globalData.Call(this.GetMethodPut(), "load_screen",
				this.GetScenePath(loadScreenScenePath));
		globalData.Call(this.GetMethodPut(), "scene_to_load",
				this.GetScenePath(survivalScenePath));
		
		globalData.Call(this.GetMethodPut(),
				this.Call<Dictionary>(cheatCode, this.GetMethodGet()));
	}

	private void InitializeTitleScreen()
	{
		if(gameplayResult)
		{
			lastGameplayResultScreen.Call(this.GetMethodInteract(),
					contentControl, gameplayResult);
			globalData.Call(this.GetMethodPut(), "gameplay_result", false);
		}
		else
			animationPlayer.Play("intro");
	}

	private void ChangeSceneToLoadScreen()
	{
		GetTree().ChangeScene(this.Call<string>(globalData,
				this.GetMethodGet(), "load_screen"));
	}

	private void Initialize()
	{
		Input.SetMouseMode(Input.MouseMode.Visible);
		globalData = GetNode(globalDataNodePath);
		gameplayDataScreen = GetNode(gameplayDataScreenNP);
		inputMappingScreen = GetNode(inputMappingScreenNP);
		cheatCode = GetNode(cheatCodeNP);
		contentControl = GetNode<Control>(contentControlNP);
		lastGameplayResultScreen = GetNode(lastGameplayResultScreenNP);
		cheatLabel = GetNode<Label>(cheatLabelNP);
		animationPlayer = GetNode<AnimationPlayer>(animationPlayerNP);
		gameplayResult = this.Call<bool>(globalData, this.GetMethodGet(),
				"gameplay_result");
	}

	public override void _EnterTree()
	{
		Initialize();
	}

	public override void _Ready()
	{
		InitializeTitleScreen();
	}

	public override void _Notification(int what)
	{
		if(what == MainLoop.NotificationWmFocusOut)
			GetTree().Paused = true;
		else if(what == MainLoop.NotificationWmFocusIn)
			GetTree().Paused = false;
	}


	[Export]
	public NodePath gameplayDataScreenNP;

	[Export]
	public NodePath lastGameplayResultScreenNP;

	[Export]
	public NodePath inputMappingScreenNP;

	[Export]
	public NodePath cheatCodeNP;

	[Export]
	public NodePath contentControlNP;

	[Export]
	public NodePath cheatLabelNP;
	
	[Export]
	public NodePath animationPlayerNP;

	[Export]
	public string firstLevelScenePath;

	[Export]
	public string survivalScenePath;

	[Export]
	public string creditsScreenScenePath;

	[Export]
	public string loadScreenScenePath;

	[Export]
	public Dictionary<string, string> cheatCodeLabelMap;

	[Export]
	public string globalDataNodePath = "/root/GlobalData";


	private Node globalData;
	private Node gameplayDataScreen;
	private Node lastGameplayResultScreen;
	private Node inputMappingScreen;
	private Node cheatCode;
	private Control contentControl;
	private Label cheatLabel;
	private AnimationPlayer animationPlayer;

	private bool gameplayResult;
}
