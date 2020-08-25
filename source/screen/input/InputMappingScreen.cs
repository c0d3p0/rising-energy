using Godot;


public class InputMappingScreen : Control
{
	public void Interact(Control hideControl)
	{
		inputMappingGUI.Interact(hideControl);
	}

	public void Load(string filePath, bool notNull, Godot.Object signalData)
	{
		signalData.Call(this.GetMethodSet(),
				jsonSerializer.Load(filePath, notNull));
	}

	private void Initialize()
	{
		inputMappingGUI = GetNode<InputMappingGUI>(inputMappingGUINP);
		jsonSerializer = GetNode<JsonSerializer>(jsonSerializerNP);
	}

	public override void _EnterTree()
	{
		Initialize();
	}


	
	[Export]
	public NodePath inputMappingGUINP;

	[Export]
	public NodePath jsonSerializerNP;


	private InputMappingGUI inputMappingGUI;
	private JsonSerializer jsonSerializer;

}
