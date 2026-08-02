using Godot;

public partial class GameRenderer : Node3D
{
	private TextureRect _renderOutput;
	
	private RenderingDevice _device;
	private RenderResources _resources;
	private Texture2Drd _displayTexture;
	
	public override void _Ready()
	{
		_renderOutput = GetNode<TextureRect>("../Display/RenderOutput");

		_device = RenderingServer.GetRenderingDevice();
		_resources = new RenderResources(_device);
		
		_resources = new RenderResources(_device);
		_resources.ClearColorTexture();

		_displayTexture = new Texture2Drd
		{
			TextureRdRid = _resources.ColorTexture
		};

		_renderOutput.Texture = _displayTexture;

		GD.Print("GPU texture connected");
	}
}
