using System.Text;

using Godot;
using Godot.Collections;


public class CheatCode : Node
{
	public void Get(Godot.Object signalData)
	{
		signalData.Call(this.GetMethodSet(), activeCheatCodeMap);
	}

	private void HandleKeyboardInput(InputEventKey inputEventKey)
	{
		if(inputEventKey != null && inputEventKey.Pressed)
		{
			uint scancode = inputEventKey.Scancode;

			if(scancode == (uint) KeyList.Space)
			{
				string cheatCodeString = cheatCode.ToString().ToLower();

				if(cheatCodeDataMap.ContainsKey(cheatCodeString))
				{
          GD.PushWarning("Activating/deactivating cheat code");
					Array data = this.cheatCodeDataMap[cheatCodeString];
					object dataKey = data[0];
					object dataValue = data[1];

					if(activeCheatCodeMap.Contains(dataKey))
					{
            activeCheatCodeMap.Remove(dataKey);
            target.Call(this.GetMethodOnToggleCheatCode(), cheatCodeString, false);
					}
					else
					{
            activeCheatCodeMap.Add(dataKey, dataValue);
            target.Call(this.GetMethodOnToggleCheatCode(), cheatCodeString, true);
					}

          cheatCode.Clear();
				}
			}
			else if(scancode == (uint) KeyList.Backspace)
				cheatCode.Clear();
			else
				cheatCode.Append((char) scancode);
		}
	}

	private void Initialize()
	{
		target = GetNode(targetNP);
		cheatCode = new StringBuilder();
		activeCheatCodeMap = new Dictionary();
	}

	public override void _EnterTree()
	{
		Initialize();
	}

	public override void _Ready()
	{
		SetProcessInput(active);
	}

	public override void _Input(InputEvent inputEvent)
	{
		HandleKeyboardInput(inputEvent as InputEventKey);
	}


	[Export]
	public NodePath targetNP;

	[Export]
	public Dictionary<string, Array> cheatCodeDataMap;

	[Export]
	public bool active = false;

	
	private Dictionary activeCheatCodeMap;
	private Node target;
	private StringBuilder cheatCode;
}
