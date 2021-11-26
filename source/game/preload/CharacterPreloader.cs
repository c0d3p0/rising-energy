using SCG = System.Collections.Generic;

using Godot;
using Godot.Collections;


public class CharacterPreloader : Node
{
	public void RequestResources()
	{
		if(characterKeyList != null && !requested)
		{
			SCG.IEnumerator<string> it = characterKeyList.GetEnumerator();
			Dictionary paramMap = new Dictionary();

			while(it.MoveNext())
			{
				nodeFactory.Call(this.GetMethodPut(), this,
						it.Current, nameof(OnRequestReceived), paramMap);
				WriteDebugMessage("RequestFinished: " + it.Current);	
			}

			requested = true;
			WriteDebugMessage("All requests are finished!");
		}
	}

	public void OnRequestReceived(Spatial resource, Dictionary paramMap)
	{
		string name = resource.Name;
		preloadMap.Add(name, resource);
		preloadIdList.Add(name);
		WriteDebugMessage("Resource received: " + name);
	}

	private void AddResourceInTheTree()
	{
		if(preloadIdList.Count > 0)
		{
			string sid = preloadIdList[0];
			Spatial s = preloadMap[sid];
			s.Call(this.GetMethodSetTarget(), playerCharacter);
			s.Call(this.GetMethodSetNodeFactory(), this);
			s.Translation = new Vector3(0f, 0f, 0f);
			s.Connect(this.GetGDSignalTreeEntered(), this,
					nameof(OnResourceTreeEntered), this.CreateSingleBind(s));
			s.Connect(this.GetGDSignalTreeExited(), this,
					nameof(OnResourceTreeExited));
			s.Connect(this.GetGDSignalReady(), s,
					this.GetMethodTransitTo(), this.CreateSingleBind("spawn"));
			preloadIdList.RemoveAt(0);
			preloadMap.Remove(sid);
			rootNode.CallDeferred(this.GetGDMethodAddChild(), s);
			WriteDebugMessage("Adding Resource: " + s.Name);
		}
	}
	
	private void RemoveResourceFromTheTree()
	{
		if(removeNameList.Count > 0)
		{
			string name = removeNameList[0];
			ulong removeTime = System.UInt64.Parse(removeTimeMap[name]);

			if(OS.GetTicksMsec() > removeTime)
			{
				Node s = rootNode.GetNode(name);
				rootNode.CallDeferred(this.GetGDMethodRemoveChild(), s);
				removeNameList.RemoveAt(0);
				removeTimeMap.Remove(name);
			}
		}
	}

	public void InitializePlayerCharacter()
	{
		playerCharacter.Call(this.GetMethodSetVitality(), null, null,
				100000f, 100000f, 10000f, 10000f);
		playerCharacter.Call(this.GetMethodTransitTo(), "trigger_hit");
		Control hud = playerCharacter.GetNode<Control>("HUDControl");
		hud.Visible = false;
	}

	private void TryToFinish()
	{
		if(preloadCount == characterKeyList.Count)
		{
			WriteDebugMessage("Preload finished!");
			rootNode.GetParent().CallDeferred(
					this.GetGDMethodRemoveChild(), rootNode);
		}
	}

	public void OnResourceTreeEntered(Spatial resource)
	{
		string removeTime = (OS.GetTicksMsec() + 5000L).ToString();
		removeTimeMap.Add(resource.Name, removeTime);
		removeNameList.Add(resource.Name);
	}

	public void OnResourceTreeExited()
	{
		preloadCount++;
	}

	private void Initialize()
	{
		preloadMap = new Dictionary<string, Spatial>();
		removeTimeMap = new Dictionary<string, string>();
		preloadIdList = new Array<string>();
		removeNameList = new Array<string>();
		
		nodeFactory.Call(this.GetMethodSetActive(), true);
		nodeFactory.Call(this.GetMethodPut(), this, nameof(RequestResources));
		PutGlobal("sceneToLoad", this.GetScenePath(nextScreenScenePath));
		AudioServer.SetBusMute(AudioServer.GetBusIndex("Master"), true);
	}

	private void ObtainNodes()
	{
		rootNode = GetNode<Spatial>(rootNodeNP);
		globalData = GetNode(globalDataNodePath);
		nodeFactory = GetNode(nodeFactoryNodePath);
		playerCharacter = GetNode<Spatial>(playerCharacterNP);
	}

	private void PutGlobal(string key, object value)
	{
		globalData.Call(this.GetMethodPut(), key, value);
	}

	public override void _EnterTree()
	{
		ObtainNodes();
		Initialize();
	}

	public override void _Ready()
	{
		InitializePlayerCharacter();
	}

	public override void _Process(float delta)
	{
		if(rootNode.Visible)
		{
			RequestResources();
			AddResourceInTheTree();
			RemoveResourceFromTheTree();
			TryToFinish();
		}
	}

	public override void _ExitTree()
	{
		AudioServer.SetBusMute(AudioServer.GetBusIndex("Master"), false);
	}

	private void WriteDebugMessage(string message)
	{
		if(OS.IsDebugBuild())
			GD.PushWarning(message);				
	}


	[Export]
	public string nextScreenScenePath = "screen/title_screen";

	[Export]
	public string nodeFactoryNodePath = "/root/NodeFactory";

	[Export]
	public string globalDataNodePath = "/root/GlobalData";

	[Export]
	public NodePath rootNodeNP;

	[Export]
	public NodePath playerCharacterNP;

	[Export]
	public Array<string> characterKeyList;


	private Spatial rootNode;
	private Node globalData;
	private Node nodeFactory;
	private Node playerCharacter;


	private Dictionary<string, Spatial> preloadMap;
	private Array<string> preloadIdList;
	private Dictionary<string, string> removeTimeMap;
	private Array<string> removeNameList;
	private int preloadCount;
	private bool requested;
}
