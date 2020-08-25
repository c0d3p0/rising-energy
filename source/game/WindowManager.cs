using Godot;


public class WindowManager : Node
{
	private void HandleKeyboardInput(InputEventKey inputEventKey)
	{
		if(inputEventKey != null && inputEventKey.Pressed)
		{
			if(inputEventKey.Scancode == (uint) KeyList.F4)
				HandleToggleMaximize();
		}
	}

	private void HandleToggleMaximize()
	{
		OS.WindowMaximized = !OS.WindowMaximized;
	}

	public override void _Input(InputEvent inputEvent)
	{
		HandleKeyboardInput(inputEvent as InputEventKey);
	}
}
