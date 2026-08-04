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
	
	private SpriteTexture _testSprite;
	
	public override void _Ready()
	{
		_renderOutput = GetNode<TextureRect>("../Display/RenderOutput");

		_device = RenderingServer.GetRenderingDevice();
		_resources = new RenderResources(_device);
		
		_testSprite = new SpriteTexture(
			_device,
			"res://Assets/Sprites/test_object.png"
		);
		
		_resources.CreateSpriteUniformSet(_testSprite.Texture);
		
		_camera = new RenderCamera
		{
			Position = Vector3.Zero,
			Yaw = 0.0f,
			Pitch = 0.0f,
			Roll = 0.0f,
			FieldOfView = 70.0f
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
		_testSprite?.Dispose();
		_resources?.Dispose();
	}
	
	public override void _Process(double delta)
	{
		_time += delta;

		_testObject.Position = new Vector3(
			Mathf.Sin((float)_time) * 3.0f,
			Mathf.Sin((float)_time * 1.5f) * 5.0f,
			-60.0f
		);
		

		ProjectedPoint? projected = ProjectToScreen(_testObject.Position);

		if (projected.HasValue)
		{
			_resources?.DrawSprite(
				projected.Value.ScreenPosition,
				projected.Value.Depth
			);
		}
	}
	
	private ProjectedPoint? ProjectToScreen(Vector3 worldPosition)
	{
		Vector3 relative = worldPosition - _camera.Position;
		
		float cosYaw = Mathf.Cos(-_camera.Yaw);
		float sinYaw = Mathf.Sin(-_camera.Yaw);

		relative = new Vector3(
			relative.X * cosYaw - relative.Z * sinYaw,
			relative.Y,
			relative.X * sinYaw + relative.Z * cosYaw
		);
		
		float cosPitch = Mathf.Cos(-_camera.Pitch);
		float sinPitch = Mathf.Sin(-_camera.Pitch);

		relative = new Vector3(
			relative.X,
			relative.Y * cosPitch - relative.Z * sinPitch,
			relative.Y * sinPitch + relative.Z * cosPitch
		);
		
		float cosRoll = Mathf.Cos(-_camera.Roll);
		float sinRoll = Mathf.Sin(-_camera.Roll);

		relative = new Vector3(
			relative.X * cosRoll - relative.Y * sinRoll,
			relative.X * sinRoll + relative.Y * cosRoll,
			relative.Z
		);

		if (relative.Z >= 0.0f)
			return null;

		float depth = -relative.Z;
		float fieldOfViewRadians = Mathf.DegToRad(_camera.FieldOfView);

		float focalLength =
			(RenderResources.Width / 2.0f) /
			Mathf.Tan(fieldOfViewRadians / 2.0f);

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
