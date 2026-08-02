using Godot;

public sealed class RenderCamera
{
	public Vector3 Position { get; set; }
	public float Yaw { get; set; }
	public float Pitch { get; set; }
	public float Roll { get; set; }
	public float FieldOfView { get; set; } = 70.0f;
}
