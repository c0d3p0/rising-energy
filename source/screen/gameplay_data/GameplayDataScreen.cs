using Godot;


public class GameplayDataScreen : Control
{
	public void Interact(Control hideControl)
	{
		gameplayDataGUI.Interact(hideControl);
	}

	public void Load(string filePath, bool notNull, Godot.Object signalData)
	{
		signalData.Call(this.GetMethodSet(),
				jsonSerializer.Load(filePath, notNull));
	}

	public void Get(string gameTypeKey, Godot.Object signalData)
	{
		signalData.Call(this.GetMethodSet(), gameplayDataRank.GetDataMap(gameTypeKey));
	}

	private void Initialize()
	{
		gameplayDataRank = GetNode<GameplayDataRank>(gameplayDataRankNP);
		gameplayDataGUI = GetNode<GameplayDataGUI>(gameplayDataGUINP);
		jsonSerializer = GetNode<JsonSerializer>(jsonSerializerNP);
	}

	public override void _EnterTree()
	{
		Initialize();
	}


	[Export]
	public NodePath gameplayDataGUINP;

	[Export]
	public NodePath gameplayDataRankNP;

	[Export]
	public NodePath jsonSerializerNP;


	private GameplayDataGUI gameplayDataGUI;
	private GameplayDataRank gameplayDataRank;
	private JsonSerializer jsonSerializer;
}
