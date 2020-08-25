using Godot;


public class GameplayResultScreenInitializer : Node
{
	private void Initialize()
	{
		gameplayResultScreen = GetNode<GameplayResultScreen>(gameplayResultScreenNP);
		gameplayResultGUI = GetNode<GameplayResultGUI>(gameplayResultGUINP);
		gameplayDataRank = GetNode(gameplayDataRankNP);
		jsonSerializer = GetNode(jsonSerializerNP);
	}

	private void InitializeGameplayResultScreen()
	{
		gameplayResultScreen.AddUserSignal(this.GetSignalSave());
		gameplayResultGUI.Connect(this.GetSignalSave(), jsonSerializer,
				this.GetMethodSave());
	}

	private void InitializeGameplayResultGUI()
	{
		gameplayResultGUI.GlobalData = GetNode(gameplayResultScreen.globalDataNodePath);
		gameplayResultGUI.AddUserSignal(this.GetSignalLoad());
		gameplayResultGUI.AddUserSignal(this.GetSignalSave());
		gameplayResultGUI.AddUserSignal(this.GetSignalIsValidDataMap());

		gameplayResultGUI.Connect(this.GetSignalLoad(), gameplayDataRank,
				this.GetMethodLoad());
		gameplayResultGUI.Connect(this.GetSignalSave(), gameplayDataRank,
				this.GetMethodSave());
		gameplayResultGUI.Connect(this.GetSignalIsValidDataMap(), gameplayResultScreen,
				this.GetMethodIsValidDataMap());
	}

	private void InitializeGameplayDataRank()
	{
		gameplayDataRank.AddUserSignal(this.GetSignalLoad());
		gameplayDataRank.AddUserSignal(this.GetSignalSave());

		gameplayDataRank.Connect(this.GetSignalLoad(), gameplayResultScreen,
				this.GetMethodLoad());
		gameplayDataRank.Connect(this.GetSignalSave(), jsonSerializer,
				this.GetMethodSave());
	}
	
	public override void _EnterTree()
	{
		Initialize();
		InitializeGameplayResultGUI();
		InitializeGameplayDataRank();
	}


	[Export]
	public NodePath gameplayResultScreenNP;

	[Export]
	public NodePath gameplayResultGUINP;

	[Export]
	public NodePath gameplayDataRankNP;

	[Export]
	public NodePath jsonSerializerNP;


	private GameplayResultScreen gameplayResultScreen;
	private GameplayResultGUI gameplayResultGUI;
	private Node gameplayDataRank;
	private Node jsonSerializer;
}
