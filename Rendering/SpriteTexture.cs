using Godot;

public sealed class SpriteTexture
{
	public Rid Texture { get; private set; }
	public uint Width { get; }
	public uint Height { get; }

	private readonly RenderingDevice _device;

	public SpriteTexture(RenderingDevice device, string path)
	{
		_device = device;

		Texture2D sourceTexture = GD.Load<Texture2D>(path);
		Image image = sourceTexture.GetImage();

		image.Convert(Image.Format.Rgba8);

		Width = (uint)image.GetWidth();
		Height = (uint)image.GetHeight();

		CreateTexture(image);
	}

	private void CreateTexture(Image image)
	{
		var format = new RDTextureFormat
		{
			Width = Width,
			Height = Height,
			Format = RenderingDevice.DataFormat.R8G8B8A8Unorm,
			UsageBits = RenderingDevice.TextureUsageBits.SamplingBit
		};

		var imageData = new Godot.Collections.Array<byte[]>
		{
			image.GetData()
		};

		Texture = _device.TextureCreate(
			format,
			new RDTextureView(),
			imageData
		);
	}

	public void Dispose()
	{
		if (Texture.IsValid)
			_device.FreeRid(Texture);
	}
}
