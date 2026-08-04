using Godot;
using System;

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
	}
	
	public void ClearColorTexture(Vector2 objectPosition, float depth)
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

		float[] pushValues =
		{
			objectPosition.X,
			objectPosition.Y,
			depth,
			0.0f
		};

		byte[] pushData = new byte[16];

		Buffer.BlockCopy(
			pushValues,
			0,
			pushData,
			0,
			pushData.Length
		);

		Device.ComputeListSetPushConstant(
			computeList,
			pushData,
			(uint)pushData.Length
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
	
	public void CreateClearUniformSet(Rid spriteTexture)
	{
		var outputUniform = new RDUniform
		{
			UniformType = RenderingDevice.UniformType.Image,
			Binding = 0
		};

		outputUniform.AddId(ColorTexture);

		var spriteUniform = new RDUniform
		{
			UniformType = RenderingDevice.UniformType.SamplerWithTexture,
			Binding = 1
		};

		var samplerState = new RDSamplerState
		{
			MinFilter = RenderingDevice.SamplerFilter.Nearest,
			MagFilter = RenderingDevice.SamplerFilter.Nearest
		};

		Rid sampler = Device.SamplerCreate(samplerState);

		spriteUniform.AddId(sampler);
		spriteUniform.AddId(spriteTexture);

		ClearUniformSet = Device.UniformSetCreate(
			new Godot.Collections.Array<RDUniform>
			{
				outputUniform,
				spriteUniform
			},
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
