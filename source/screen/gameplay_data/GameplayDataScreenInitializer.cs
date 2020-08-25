using Godot;


public class GameplayDataScreenInitializer : Node
{
	private void Initialize()
	{
		gameplayDataScreen = GetNode(gameplayDataScreenNP);
		gameplayDataGUI = GetNode(gameplayDataGUINP);
		gameplayDataRank = GetNode(gameplayDataRankNP);
	}

	private void InitializeGameplayDataGUI()
	{
		gameplayDataGUI.AddUserSignal(this.GetSignalLoad());
		gameplayDataGUI.AddUserSignal(this.GetSignalGet());
		
		gameplayDataGUI.Connect(this.GetSignalLoad(), gameplayDataRank,
				this.GetMethodLoad());
		gameplayDataGUI.Connect(this.GetSignalGet(), gameplayDataScreen,
				this.GetMethodGet());
	}

	private void InitializeGameplayDataRank()
	{
		gameplayDataRank.AddUserSignal(this.GetSignalLoad());
		gameplayDataRank.Connect(this.GetSignalLoad(), gameplayDataScreen,
				this.GetMethodLoad());
	}
	
	public override void _EnterTree()
	{
		Initialize();
		InitializeGameplayDataGUI();
		InitializeGameplayDataRank();
	}


	[Export]
	public NodePath gameplayDataScreenNP;

	[Export]
	public NodePath gameplayDataGUINP;

	[Export]
	public NodePath gameplayDataRankNP;
	

	private Node gameplayDataScreen;
	private Node gameplayDataGUI;
	private Node gameplayDataRank;
}
