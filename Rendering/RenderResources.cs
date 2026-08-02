using Godot;

public sealed class RenderResources
{
	public const uint Width = 320;
	public const uint Height = 180;

	public RenderingDevice Device { get; }
	public Rid ColorTexture { get; private set; }
	
	public Rid ClearShader { get; private set; }
	public Rid ClearPipeline { get; private set; }
	
	public Rid ClearUniformSet { get; private set; }

	public RenderResources(RenderingDevice device)
	{
		Device = device;
		
		CreateColorTexture();
		CreateClearPipeline();
		CreateClearUniformSet();
	}
	
	public void ClearColorTexture()
	{
		var computeList = Device.ComputeListBegin();

		Device.ComputeListBindComputePipeline(
			computeList,
			ClearPipeline
		);

		Device.ComputeListBindUniformSet(
			computeList,
			ClearUniformSet,
			0
		);

		Device.ComputeListDispatch(
			computeList,
			40,
			23,
			1
		);

		Device.ComputeListEnd();
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
	
	private void CreateClearUniformSet()
	{
		var outputUniform = new RDUniform
		{
			UniformType = RenderingDevice.UniformType.Image,
			Binding = 0
		};

		outputUniform.AddId(ColorTexture);

		ClearUniformSet = Device.UniformSetCreate(
			new Godot.Collections.Array<RDUniform> { outputUniform },
			ClearShader,
			0
		);
	}
	public void Dispose()
	{
		if (ClearUniformSet.IsValid)
			Device.FreeRid(ClearUniformSet);

		if (ClearPipeline.IsValid)
			Device.FreeRid(ClearPipeline);

		if (ClearShader.IsValid)
			Device.FreeRid(ClearShader);

		if (ColorTexture.IsValid)
			Device.FreeRid(ColorTexture);
	}
}
