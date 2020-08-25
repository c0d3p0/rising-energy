using Godot;
using Godot.Collections;


public class CreditsScreen : Node
{
	public void PrepareNextCharacterAnimationData()
	{
		if(currentCharacterId < characterMap.Count)
		{
			if(currentCharacterId > 0)
				characterMap[currentCharacterId - 1].Visible = false;

			characterNameLabel.Text = characterNameMap[currentCharacterId];
			characterMap[currentCharacterId].Visible = true;
			animationStateMachine.Travel(characterPositionLeftMap[currentCharacterId] ? 
					"character_from_left" : "character_from_right");
			currentCharacterId++;
		}
		else
			animationStateMachine.Travel("start_3d_model");
	}

	public void End()
	{
		if(!shortCredits)
			globalData.Call(this.GetMethodPut(), "gameplay_result", true);

		globalData.Call(this.GetMethodPut(), "scene_to_load",
				this.GetScenePath(nextScreenScenePath));
		GetTree().ChangeScene(this.GetScenePath(loadScreenScenePath));
	}

	public void Start()
	{
		if(shortCredits)
			animationStateMachine.Travel("start_3d_model");
		else
			animationStateMachine.Travel("game_title");
	}

	private void Initialize()
	{
		globalData = GetNode(globalDataNodePath);
		characterContainer = GetNode<Spatial>(characterContainerNP);
		characterNameLabel = GetNode<Label>(characterNameLabelNP);
		characterMap = this.GetNodeMap<int, Spatial>(this, characterNPMap);
		animationStateMachine = GetNode<AnimationTree>(animationTreeNP).Get(
				"parameters/playback") as AnimationNodeStateMachinePlayback;
		shortCredits = this.Call<bool>(globalData,
				this.GetMethodGet(), "short_credits");
	}

	public override void _EnterTree()
	{
		Initialize();
	}


	[Export]
	public NodePath characterContainerNP;

	[Export]
	public NodePath characterNameLabelNP;

	[Export]
	public NodePath animationTreeNP;
	
	[Export]
	public string globalDataNodePath;

	[Export]
	public string loadScreenScenePath;

	[Export]
	public string nextScreenScenePath;
	
	[Export]
	public Dictionary<int, NodePath> characterNPMap;

	[Export]
	public Dictionary<int, string> characterNameMap;

	[Export]
	public Dictionary<int, bool> characterPositionLeftMap;


	private Node globalData;
	private Spatial characterContainer;
	private Label characterNameLabel;
	private Dictionary<int, Spatial> characterMap;
	private AnimationNodeStateMachinePlayback animationStateMachine;

	private int currentCharacterId;
	private bool shortCredits;
}
