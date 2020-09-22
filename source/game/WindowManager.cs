using Godot;


// OS.WindowMaximized property is always returning false and I have no idea why.
// Maybe a GodotEngine 3.2.3 bug?
public class WindowManager : Node
{
	private void HandleToggleMaximize()
	{
		OS.WindowMaximized = !OS.WindowMaximized;
	}

	private void HandleToggleFullScreen()
	{
		OS.WindowFullscreen = !OS.WindowFullscreen;
	}

	private void HandleToggleWindowBorderless()
	{
		OS.WindowBorderless = !OS.WindowBorderless;
	}

	private void HandleKeyboardInput(InputEventKey inputEventKey)
	{
		if(inputEventKey != null && inputEventKey.Pressed)
		{
			if(inputEventKey.Scancode == (uint) KeyList.F4)
				HandleToggleMaximize();
			else if(inputEventKey.Scancode == (uint) KeyList.F5)
				HandleToggleWindowBorderless();
			else if(inputEventKey.Scancode == (uint) KeyList.F11)
				HandleToggleFullScreen();
		}
	}

	public override void _EnterTree()
	{
		OS.SetWindowTitle(gameTitle);
	}

	public override void _Input(InputEvent inputEvent)
	{
		HandleKeyboardInput(inputEvent as InputEventKey);
	}


	[Export]
	public string gameTitle = "Rising Energy (github.com/c0d3p0 - Godot Engine)";
}
