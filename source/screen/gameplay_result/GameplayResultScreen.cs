using Godot;
using Godot.Collections;


public class GameplayResultScreen : Control
{
	public void Interact(Control hideControl, bool newResult)
	{
		gameplayResultGUI.Interact(hideControl, newResult);
	}

	public void Save(Dictionary dataMap, string filePath)
	{
		jsonSerializer.Save(dataMap, filePath);
	}

	public void Load(string filePath, bool notNull, Godot.Object signalData)
	{
		signalData.Call(this.GetMethodSet(),
				jsonSerializer.Load(filePath, notNull));
	}

	public void IsValidDataMap(Dictionary dataMap, Godot.Object signalData)
	{
		signalData.Call(this.GetMethodSet(),
				gameplayDataRank.IsValidDataMap(dataMap));
	}

	private void Initialize()
	{
		gameplayResultGUI = GetNode<GameplayResultGUI>(gameplayResultGUINP);
		gameplayDataRank = GetNode<GameplayDataRank>(gameplayDataRankNP);
		jsonSerializer = GetNode<JsonSerializer>(jsonSerializerNP);
	}

	public override void _EnterTree()
	{
		Initialize();
	}


	
	[Export]
	public NodePath gameplayResultGUINP;

	[Export]
	public NodePath gameplayDataRankNP;

	[Export]
	public NodePath jsonSerializerNP;

	[Export]
	public string globalDataNodePath;
	

	private GameplayResultGUI gameplayResultGUI;
	private GameplayDataRank gameplayDataRank;
	private JsonSerializer jsonSerializer;
}
