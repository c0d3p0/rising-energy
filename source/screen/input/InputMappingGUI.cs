using SC = System.Collections;
using SCG = System.Collections.Generic;

using Godot;
using Godot.Collections;


public class InputMappingGUI : Node
{
	public void Interact(Control hideControl)
	{
		this.CreateFolder(inputMappingFolderName);
		keyboardInputMapping = this.EmitSignal<Dictionary>(this,
				this.GetSignalLoad(), GetFilePath(keyboardMappingFileName), true);
		controllerInputMapping = this.EmitSignal<Dictionary>(this,
				this.GetSignalLoad(), GetFilePath(controllerMappingFileName), true);
		this.hideControl = hideControl;
		currentInputMapping = keyboardInputMapping;
		currentInputName = null;
		UpdateButtonsTextToCurrentMapping();
		ShowScreen(true);
	}

	public void OnDeviceTypeButtonPressed(bool keyboard)
	{
		if(keyboard)
		{
			currentInputMapping = keyboardInputMapping;
			deviceTypeLabel.Text = "Keyboard";
		}
		else
		{
			currentInputMapping = controllerInputMapping;
			deviceTypeLabel.Text = "Controller";
		}

		UpdateButtonsTextToCurrentMapping();
	}

	public void OnInputButtonPressed(string inputName)
	{
		if(currentInputName != null)
			SetButtonText(currentInputName, "Not mapped");

		currentInputName = inputName;
		buttonMap[currentInputName].Text = "Press an input";
	}

	public void OnSaveButtonPressed()
	{
		EmitSignal(this.GetSignalSave(), keyboardInputMapping,
				GetFilePath(keyboardMappingFileName));
		EmitSignal(this.GetSignalSave(), controllerInputMapping,
				GetFilePath(controllerMappingFileName));
		ShowScreen(false);
		AddKeyboardInputsToGameMapping();
		AddControllerInputsToGameMapping();
		keyboardInputMapping.Dispose();
		controllerInputMapping.Dispose();
	}

	public void OnIgnoreButtonPressed()
	{
		ShowScreen(false);
		keyboardInputMapping.Dispose();
		controllerInputMapping.Dispose();
	}

	private void AddControllerInputsToGameMapping()
	{
		string action;
		InputEventJoypadMotion iejm;
		InputEventJoypadButton iejb;
		SC.IDictionaryEnumerator it = controllerInputMapping.GetEnumerator();

		if(controllerInputMapping.Contains("deviceId"))
		{
			int deviceId = int.Parse(controllerInputMapping["deviceId"] as string);
			it = controllerInputMapping.GetEnumerator();

			while(it.MoveNext())
			{
				if(!it.Entry.Key.Equals("deviceId"))
				{
					action = it.Entry.Key as string;
					string inputValue = it.Entry.Value as string;

					if(IsDirectionInput(inputValue))
					{
						iejm = new InputEventJoypadMotion();
						float[] axisData = ConvertInputcodeToJoystickAxis(inputValue);
						iejm.Axis = (int) axisData[0];
						iejm.AxisValue = axisData[1];
						iejm.Device = deviceId;
						InputMap.ActionAddEvent(action, iejm);
					}
					else
					{
						iejb = new InputEventJoypadButton();
						iejb.ButtonIndex = int.Parse(it.Entry.Value as string);
						iejb.Device = deviceId;
						InputMap.ActionAddEvent(action, iejb);
					}
				}
			}
		}
	}

	private void AddKeyboardInputsToGameMapping()
	{
		string action;
		InputEventKey iek;
		SC.IDictionaryEnumerator it = keyboardInputMapping.GetEnumerator();

		while(it.MoveNext())
		{
			action = it.Entry.Key as string;
			InputMap.ActionEraseEvents(action);
			iek = new InputEventKey();
			iek.Scancode = uint.Parse(it.Entry.Value as string);
			InputMap.ActionAddEvent(action, iek);
		}
	}

	private void ConfigureInputKey(InputEventKey inputEventKey)
	{
		if(inputEventKey != null && inputEventKey.Pressed && 
				currentInputName != null && currentInputMapping == keyboardInputMapping)
		{
			uint keycode = inputEventKey.Scancode;

			if(!ignoreKeyMap.ContainsKey((int) keycode))
				AddInput(currentInputName, keycode.ToString());

			SetButtonText(currentInputName, "Not mapped");
			currentInputName = null;
		}	
	}

	private void ConfigureInputJoypadButton(InputEventJoypadButton inputEventJoyButton)
	{
		if(inputEventJoyButton != null && inputEventJoyButton.Pressed && 
				currentInputName != null && currentInputMapping == controllerInputMapping)
		{
			int buttonIndex = (int) inputEventJoyButton.ButtonIndex;
			AddInput(currentInputName, buttonIndex.ToString());
			AddInput("deviceId", inputEventJoyButton.Device);
			SetButtonText(currentInputName, "Not mapped");
			currentInputName = null;
			GetTree().SetInputAsHandled();
		}	
	}

	private void ConfigureInputJoypadMotion(InputEventJoypadMotion inputEventJoyMotion)
	{
		if(inputEventJoyMotion != null && inputEventJoyMotion.AxisValue != 0 && 
				currentInputName != null && currentInputMapping == controllerInputMapping)
		{
			string inputcode = ConvertJoystickAxisInputcode(inputEventJoyMotion.Axis,
					inputEventJoyMotion.AxisValue);
			AddInput(currentInputName, inputcode);
			AddInput("deviceId", inputEventJoyMotion.Device);
			SetButtonText(currentInputName, "Not mapped");
			currentInputName = null;
		}	
	}

	private void UpdateButtonsTextToCurrentMapping()
	{
		string defaultText = "Not mapped";
		SCG.IEnumerator<SCG.KeyValuePair<object, Button>> it =
				buttonMap.GetEnumerator();

		while(it.MoveNext())
			SetButtonText(it.Current.Key, defaultText);
	}
	
	private void SetButtonText(object buttonKey, string defaultText)
	{
		if(currentInputMapping.Contains(buttonKey))
		{
			object inputValue = currentInputMapping[buttonKey];

			if(currentInputMapping == controllerInputMapping)
			{
				if(IsDirectionInput(inputValue))
				{
					float[] axisData = ConvertInputcodeToJoystickAxis(inputValue);
					buttonMap[buttonKey].Text = "Axis " + axisData[0] + ", " + axisData[1];
				}
				else
					buttonMap[buttonKey].Text = "Button " + inputValue;
			}
			else
			{
				buttonMap[buttonKey].Text =
						((KeyList) uint.Parse(inputValue as string)).ToString();
			}
		}
		else
			buttonMap[buttonKey].Text = defaultText;
	}

	private void ShowScreen(bool show)
	{
		hideControl.Visible = !show;
		inputMappingScreen.ShowBehindParent = !show;
		inputMappingScreen.Visible = show;
		this.SetProcessInput(show);
	}

	private void AddInput(object inputName, object value)
	{
		if(currentInputMapping.Contains(inputName))
			currentInputMapping[inputName] = value.ToString();
		else
			currentInputMapping.Add(inputName, value.ToString());
	}

	private string ConvertJoystickAxisInputcode(int axis, float axisValue)
	{
		return axis + "_" + axisValue;
	}

	private float[] ConvertInputcodeToJoystickAxis(object value)
	{
		string[] data = (value as string).Split("_");
		int axis = int.Parse(data[0]);
		float axisValue = float.Parse(data[1]);
		return new float[]{axis, axisValue};
	}

	private bool IsDirectionInput(object value)
	{
		return (value as string).Contains("_");
	}

	private string GetFilePath(string fileName)
	{
		return this.GetFilePath(fileName, "json", inputMappingFolderName);
	}
	
	private void Initialize()
	{
		inputMappingScreen = GetNode<Control>(inputMappingScreenNP);
		buttonMap = this.GetNodeMap<object, Button>(this, buttonNPMap);
		deviceTypeLabel = GetNode<Label>(deviceTypeLabelNP);
	}

	public override void _EnterTree()
	{
		Initialize();
	}

	public override void _Ready()
	{
		this.SetProcessInput(false);
	}

	public override void _Input(InputEvent inputEvent)
	{
		ConfigureInputKey(inputEvent as InputEventKey);
		ConfigureInputJoypadMotion(inputEvent as InputEventJoypadMotion);
		ConfigureInputJoypadButton(inputEvent as InputEventJoypadButton);
	}


	[Export]
	public NodePath inputMappingScreenNP;

	[Export]
	public NodePath deviceTypeLabelNP;

	[Export]
	public Dictionary<object, NodePath> buttonNPMap;

	[Export]
	public Dictionary<int, object> ignoreKeyMap;

	[Export]
	public string inputMappingFolderName;

	[Export]
	public string keyboardMappingFileName;

	[Export]
	public string controllerMappingFileName;


	private Control inputMappingScreen;
	private Control hideControl;
	private Label deviceTypeLabel;
	private Dictionary<object, Button> buttonMap;

	private Dictionary keyboardInputMapping;
	private Dictionary controllerInputMapping;
	private Dictionary currentInputMapping;
	private string currentInputName;
}
