using Godot;

public partial class GameRenderer : Node3D
{
	private TextureRect _renderOutput;
	
	public override void _Ready()
	{
		_renderOutput = GetNode<TextureRect>("../Display/RenderOutput");
		GD.Print("Render output connected");
	}
}
