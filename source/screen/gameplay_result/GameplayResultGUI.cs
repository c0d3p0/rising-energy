using Godot;
using Godot.Collections;


public class GameplayResultGUI : Node
{
	public void Interact(Control hideControl, bool newResult)
	{
		this.hideControl = hideControl;
		this.CreateFolder(gameplayResultFolderPath);
		CreateGameResultDataMap(newResult);
		UpdateNormalGameGUI();
		UpdateSurvivalGameGUI();
		UpdateNoDataGUI();
		ShowScreen(true);
	}

	public void OnReturnButtonPressed()
	{
		ShowScreen(false);
	}

	private void CreateGameResultDataMap(bool newResult)
	{
		if(newResult)
		{
			gameplayResultDataMap = new Dictionary();
			string gameType = this.Call<string>(globalData,
					this.GetMethodGet(), "game_type");

			if(gameType.Equals("normalGame"))
			{
				int totalTime = this.Call<int>(globalData,
						this.GetMethodGet(), "total_time");
				int deaths = this.Call<int>(globalData,
						this.GetMethodGet(), "deaths");
				int rankScore = CalculateRankScore(totalTime, deaths);
				gameplayResultDataMap.Add("rank", GetRank(rankScore));
				gameplayResultDataMap.Add("rankScore", rankScore.ToString());
				gameplayResultDataMap.Add("totalTime", GetTimeString(totalTime));
				gameplayResultDataMap.Add("deaths", deaths.ToString());
				EmitSignal(this.GetSignalSave(), gameplayResultDataMap, true);
			}
			else if(gameType.Equals("survivalGame"))
			{
				gameplayResultDataMap.Add("playerScore", this.Call<string>(globalData,
						this.GetMethodGet(), "player_score").ToString());
				EmitSignal(this.GetSignalSave(), gameplayResultDataMap, false);
			}

			gameplayResultScreen.Call(this.GetMethodSave(),
					gameplayResultDataMap, GetFilePath());	// Save the new rank.
			return;
		}

		gameplayResultDataMap = this.Call<Dictionary>(gameplayResultScreen,
				this.GetMethodLoad(), GetFilePath(), true);	// Save the last gameplay.
	}

	private void UpdateNormalGameGUI()
	{		
		if(this.EmitSignal<int>(this, this.GetSignalIsValidDataMap(),
				gameplayResultDataMap) == 0)
		{
			normalGameControl.Visible = true;
			string[] prefixes = new string[]{"Rank ", "Total Time: ", "Deaths: "};
			string[] keys = new string[]{"rank", "totalTime", "deaths",
					"rankScore", "ac"};

			for(int i = 0; i < prefixes.Length; i++)
			{
				normalGameLabelList[i].Text = prefixes[i] +
						(gameplayResultDataMap[keys[i]] as string);
			}
		}
		else
			normalGameControl.Visible = false;
	}

	private void UpdateSurvivalGameGUI()
	{
		if(this.EmitSignal<int>(this, this.GetSignalIsValidDataMap(),
				gameplayResultDataMap) == 1)
		{
			string[] keys = new string[]{"playerScore", "ac"};
			string[] prefixes = new string[]{"Score: "};
			survivalGameControl.Visible = true;
			survivalGameLabel.Text = "Score: " +
					(gameplayResultDataMap[keys[0]] as string);
		}
		else
			survivalGameControl.Visible = false;
	}

	private void UpdateNoDataGUI()
	{
		noDataLabel.Visible = !(normalGameControl.Visible || survivalGameControl.Visible);
	}

	private void ShowScreen(bool show)
	{
		hideControl.Visible = !show;
		gameplayResultScreen.ShowBehindParent = !show;
		gameplayResultScreen.Visible = show;
	}

	private string GetTimeString(int time)
	{
		int allSeconds = (int) (time / 1000L);
		int allMinutes = (int) (allSeconds / 60);
		int allHours = (int) (allMinutes / 60);

		if(allHours < 96 && time > 0)
		{
			return string.Format("{0:D2}:{1:D2}:{2:D2}.{3:D3}",
					allHours, allMinutes % 60, allSeconds % 60, time % 1000);
		}
		else
			return "4+ day";
	}

	private int CalculateRankScore(int totalTime, int deaths)
	{
		// RankScore is the maximum int value discount the total time and the deaths.
		// Each millisecond costs 1 point.
		// All deaths will cost 5 minutes.
		// 1st death will cost 30 extra minutes.
		// 4th death will cost 20 extra minutes.
		// 11th death will cost 40 extra minutes.
		// Limit of 70000 deaths to prevent the variable to overflow.
		// This ranking will not work properly if the player is using cheats.
		
		if(deaths < 70000 && totalTime > 0)
		{
			int deathPoints = deaths > 10 ? 2400000 : 0;
			deathPoints += deaths > 3 ? 1800000 : 0;
			deathPoints += deaths > 0 ? 1200000 : 0;

			deathPoints += deaths > 0 ? deaths * 300000 : 0;

			if((long)(deathPoints + totalTime) < int.MaxValue)
				return int.MaxValue - (deathPoints + totalTime);
		}

		return 0;
	}

	private string GetRank(int rankScore)
	{
		if(rankScore >= int.MaxValue - 2700000)	// Score bigger than the maximum minus 45 minutes.
			return "S";
		else if(rankScore >= int.MaxValue - 5400000)	// Score bigger than the maximum minus 90 minutes.
			return "A";
		else if(rankScore >= int.MaxValue - 9600000)  // Score bigger than the maximum minus 160 minutes.
			return "B";
		else
			return "C";
	}

	private string GetFilePath()
	{
		return this.GetFilePath(gameplayResultFilePath,
				"json", gameplayResultFolderPath);
	}

	private void InitializeNormalGameLabelList()
	{
		normalGameLabelList = new Array<Label>();
		
		for(int i = 1; i < normalGameControl.GetChildCount(); i++)
			normalGameLabelList.Add(normalGameControl.GetChild<Label>(i));
	}

	private void Initialize()
	{
		gameplayResultScreen = GetNode<Control>(gameplayResultScreenNP); 
		normalGameControl = GetNode<Control>(normalGameControlNP);
		survivalGameControl = GetNode<Control>(survivalGameControlNP);
		noDataLabel = GetNode<Label>(noDataLabelNP);
		survivalGameLabel = survivalGameControl.GetChild<Label>(
				survivalGameControl.GetChildCount() - 1);
	}

	public override void _EnterTree()
	{
		Initialize();
		InitializeNormalGameLabelList();
	}

	public Node GlobalData
	{
		set
		{
			globalData = value;
		}
	}


	[Export]
	public NodePath gameplayResultScreenNP;

	[Export]
	public NodePath normalGameControlNP;

	[Export]
	public NodePath survivalGameControlNP;

	[Export]
	public NodePath noDataLabelNP;

	[Export]
	public string gameplayResultFolderPath;

	[Export]
	public string gameplayResultFilePath;


	private Node globalData;

	private Array<Label> normalGameLabelList;
	private Label survivalGameLabel;
	private Label noDataLabel;

	private Control gameplayResultScreen;
	private Control survivalGameControl;
	private Control normalGameControl;
	private Control hideControl;
	private Dictionary gameplayResultDataMap;
}
