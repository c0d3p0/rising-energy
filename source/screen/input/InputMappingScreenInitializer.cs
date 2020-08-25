using Godot;


public class InputMappingScreenInitializer : Node
{
	private void Initialize()
	{
		inputMappingScreen = GetNode(inputMappingScreenNP);
		inputMappingGUI = GetNode(inputMappingGUINP);
		jsonSerializer = GetNode(jsonSerializerNP);
	}

	private void InitializeInputMappingGUI()
	{
		inputMappingGUI.AddUserSignal(this.GetSignalSave());
		inputMappingGUI.AddUserSignal(this.GetSignalLoad());

		inputMappingGUI.Connect(this.GetSignalSave(), jsonSerializer,
				this.GetMethodSave());
		inputMappingGUI.Connect(this.GetSignalLoad(), inputMappingScreen,
				this.GetMethodLoad());
	}

	public override void _EnterTree()
	{
		Initialize();
		InitializeInputMappingGUI();
	}


	[Export]
	public NodePath inputMappingScreenNP;

	[Export]
	public NodePath inputMappingGUINP;

	[Export]
	public NodePath jsonSerializerNP;


	private Node inputMappingScreen;
	private Node inputMappingGUI;
	private Node jsonSerializer;
}
