using Godot;

public readonly struct ProjectedPoint
{
	public Vector2 ScreenPosition { get; }
	public float Depth { get; }

	public ProjectedPoint(Vector2 screenPosition, float depth)
	{
		ScreenPosition = screenPosition;
		Depth = depth;
	}
}
