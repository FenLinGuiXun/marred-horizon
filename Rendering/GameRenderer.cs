using Godot;

public partial class GameRenderer : Node3D
{
	private TextureRect _renderOutput;
	
	private RenderingDevice _device;
	private RenderResources _resources;
	private Texture2Drd _displayTexture;
	
	private RenderCamera _camera;
	private RenderObject _testObject;
	
	private double _time;
	
	public override void _Ready()
	{
		_renderOutput = GetNode<TextureRect>("../Display/RenderOutput");

		_device = RenderingServer.GetRenderingDevice();
		_resources = new RenderResources(_device);
		
		_camera = new RenderCamera
		{
			Position = Vector3.Zero,
			Yaw = 0.0f,
			Pitch = 0.0f
		};

		_testObject = new RenderObject
		{
			Position = new Vector3(0.0f, 0.0f, -10.0f)
		};

		_displayTexture = new Texture2Drd
		{
			TextureRdRid = _resources.ColorTexture
		};

		_renderOutput.Texture = _displayTexture;

		GD.Print("GPU texture connected");
	}
	
	public override void _ExitTree()
	{
		_resources?.Dispose();
	}
	
	public override void _Process(double delta)
	{
		_time += delta;

		_testObject.Position = new Vector3(
			Mathf.Sin((float)_time) * 3.0f,
			Mathf.Sin((float)_time * 1.5f) * 2.0f,
			-(Mathf.Sin((float)_time * 1.2f) * 3.0f) - 8.0f
		);

		ProjectedPoint? projected = ProjectToScreen(_testObject.Position);

		if (projected.HasValue)
		{
			_resources?.ClearColorTexture(
				projected.Value.ScreenPosition,
				projected.Value.Depth
			);
		}
	}
	
	private ProjectedPoint? ProjectToScreen(Vector3 worldPosition)
	{
		Vector3 relative = worldPosition - _camera.Position;

		if (relative.Z >= 0.0f)
			return null;

		float depth = -relative.Z;
		float focalLength = RenderResources.Width / 2.0f;

		float screenX =
			RenderResources.Width / 2.0f +
			relative.X / depth * focalLength;

		float screenY =
			RenderResources.Height / 2.0f -
			relative.Y / depth * focalLength;

		return new ProjectedPoint(
			new Vector2(screenX, screenY),
			depth
		);
	}
}
