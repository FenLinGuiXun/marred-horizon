using Godot;

public sealed class RenderResources
{
	public const uint Width = 320;
	public const uint Height = 180;

	public RenderingDevice Device { get; }
	public Rid ColorTexture { get; private set; }
	
	public Rid ClearShader { get; private set; }
	public Rid ClearPipeline { get; private set; }

	public RenderResources(RenderingDevice device)
	{
		Device = device;
		
		CreateColorTexture();
		CreateClearPipeline();
	}

	private void CreateColorTexture()
	{
		var format = new RDTextureFormat
		{
			Width = Width,
			Height = Height,
			Format = RenderingDevice.DataFormat.R8G8B8A8Unorm,
			UsageBits =
				RenderingDevice.TextureUsageBits.StorageBit |
				RenderingDevice.TextureUsageBits.SamplingBit
		};

		ColorTexture = Device.TextureCreate(
			format,
			new RDTextureView()
		);
	}
	
	private void CreateClearPipeline()
	{
		var shaderFile = GD.Load<RDShaderFile>(
			"res://Rendering/Shaders/clear_color.glsl"
		);

		var shaderSpirV = shaderFile.GetSpirV();

		ClearShader = Device.ShaderCreateFromSpirV(shaderSpirV);
		ClearPipeline = Device.ComputePipelineCreate(ClearShader);
	}
}
