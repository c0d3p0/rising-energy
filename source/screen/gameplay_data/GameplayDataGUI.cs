using Godot;
using Godot.Collections;


public class GameplayDataGUI : Node
{
	public void Interact(Control hideControl)
	{
		this.hideControl = hideControl;
		EmitSignal(this.GetSignalLoad());
		normalGameRankedDataMap = this.EmitSignal<Dictionary>(
				this, this.GetSignalGet(), "normalGame");
		survivalGameRankedDataMap = this.EmitSignal<Dictionary>(
				this, this.GetSignalGet(), "survivalGame");
		UpdateNormalGameGUI();
		UpdateSurvivalGameGUI();
		ShowScreen(true);
	}

	public void OnReturnButtonPressed()
	{
		ShowScreen(false);
	}

	private void UpdateNormalGameGUI()
	{
		Control placeControl;
		Array<Label> labelList;
		Dictionary dataMap;
		string[] keys = new string[]{"rank", "totalTime", "deaths"};
		string[] prefixes = new string[]{"Rank ", "Total Time: ", "Deaths: "};
		string indexText;

		for(int id = 0; id < normalGameLabelListMap.Count; id++)
		{
			placeControl = normalGamePlaceControlMap[id];
			indexText = id.ToString();

			if(id < normalGameRankedDataMap.Count &&
					normalGameRankedDataMap.Contains(indexText))
			{
				placeControl.Visible = true;
				labelList = normalGameLabelListMap[id];
				dataMap = normalGameRankedDataMap[indexText] as Dictionary;

				for(int index = 0; index < labelList.Count; index++)
					labelList[index].Text = prefixes[index] + dataMap[keys[index]];
			}
			else
				placeControl.Visible = false;
		}

		normalGameNoDataLabel.Visible = normalGameRankedDataMap.Count < 1;
	}

	private void UpdateSurvivalGameGUI()
	{
		Control placeControl;
		Dictionary rankMap;
		string key = "playerScore";
		string indexText;

		for(int id = 0; id < survivalGameLabelMap.Count; id++)
		{
			placeControl = survivalGamePlaceControlMap[id];
			indexText = id.ToString();

			if(id < survivalGameRankedDataMap.Count &&
					survivalGameRankedDataMap.Contains(indexText))
			{
				placeControl.Visible = true;
				rankMap = survivalGameRankedDataMap[indexText] as Dictionary;
				survivalGameLabelMap[id].Text = "Score: " + rankMap[key] as string;
			}
			else
				placeControl.Visible = false;
		}

		survivalGameNoDataLabel.Visible = survivalGameRankedDataMap.Count < 1;
	}

	private void ShowScreen(bool show)
	{
		hideControl.Visible = !show;
		gameplayDataScreen.ShowBehindParent = !show;
		gameplayDataScreen.Visible = show;
	}
	
	private void InitializeNormalGamePlaceControlMap()
	{
		Control placeControl;
		int id;

		for(int i = 1; i < normalGameControl.GetChildCount(); i++)
		{
			id = i - 1;
			placeControl = normalGameControl.GetChild<Control>(i);
			InitializeNormalGameLabelListMap(id, placeControl);
			normalGamePlaceControlMap.Add(id, placeControl);
		}
	}

	private void InitializeSurvivalGamePlaceControlMap()
	{
		Control placeControl;
		int id;

		for(int i = 1; i < survivalGameControl.GetChildCount() - 1; i++)
		{
			id = (i - 1);
			placeControl = survivalGameControl.GetChild<Control>(i);
			survivalGameLabelMap.Add(id, placeControl.GetChild<Label>(
					placeControl.GetChildCount() - 1));
			survivalGamePlaceControlMap.Add(id, placeControl);
		}
	}

	private void InitializeNormalGameLabelListMap(int id, Control placeControl)
	{
		Array<Label> labelList = new Array<Label>();
		
		for(int i = 1; i < placeControl.GetChildCount(); i++)
			labelList.Add(placeControl.GetChild<Label>(i));
		
		normalGameLabelListMap.Add(id, labelList);
	}

	private void Initialize()
	{
		normalGameLabelListMap = new Dictionary<int, Array<Label>>();
		survivalGameLabelMap = new Dictionary<int, Label>();
		normalGamePlaceControlMap = new Dictionary<int, Control>();
		survivalGamePlaceControlMap = new Dictionary<int, Control>();
		gameplayDataScreen = GetNode<Control>(gameplayDataScreenNP); 
		normalGameControl = GetNode<Control>(normalGameControlNP);
		survivalGameControl = GetNode<Control>(survivalGameControlNP);
		normalGameNoDataLabel = normalGameControl.GetChild<Label>(
					normalGameControl.GetChildCount() - 1);
		survivalGameNoDataLabel = survivalGameControl.GetChild<Label>(
					survivalGameControl.GetChildCount() - 1);
	}

	public override void _EnterTree()
	{
		Initialize();
		InitializeNormalGamePlaceControlMap();
		InitializeSurvivalGamePlaceControlMap();
	}
	

	[Export]
	public NodePath gameplayDataScreenNP;

	[Export]
	public NodePath normalGameControlNP;

	[Export]
	public NodePath survivalGameControlNP;


	private Dictionary<int, Control> normalGamePlaceControlMap;
	private Dictionary<int, Array<Label>> normalGameLabelListMap;
	private Dictionary<int, Control> survivalGamePlaceControlMap;
	private Dictionary<int, Label> survivalGameLabelMap;
	private Label normalGameNoDataLabel;
	private Label survivalGameNoDataLabel;

	private Control gameplayDataScreen;
	private Control survivalGameControl;
	private Control normalGameControl;
	private Control hideControl;
	private Dictionary normalGameRankedDataMap;
	private Dictionary survivalGameRankedDataMap;
}
