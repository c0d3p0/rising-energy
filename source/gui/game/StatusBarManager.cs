using System.Text;

using Godot;


public class StatusBarManager : Node
{
	public void SetProgress(float min, float max, float current)
	{
		statusBar.MinValue = min;
		statusBar.MaxValue = max;
		SetProgress(current);
	}

	public void SetProgress(float current)
	{
		statusBar.Value = current;
		statusBarLabel.Text = new StringBuilder().Append(statusText).
				Append(": ").Append(Mathf.Round(current)).Append(" / ").
				Append(statusBar.MaxValue).ToString().Trim();
	}

	private void Initialize()
	{
		statusBar = GetNode<TextureProgress>(statusBarNP);
		statusBarLabel = GetNode<Label>(statusBarLabelNP);
	}

	public override void _EnterTree()
	{
		Initialize();
	}


	[Export]
	public NodePath statusBarNP;

	[Export]
	public NodePath statusBarLabelNP;

	[Export]
	public string statusText;

	public TextureProgress statusBar;
	public Label statusBarLabel;
}
