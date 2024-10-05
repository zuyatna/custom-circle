using Godot;

public partial class Arrow : Sprite2D
{
	// Speed of the arrow rotation
	private float _rotationSpeed = 2f;

	public override void _Process(double delta)
	{
		// Rotate the arrow continuously
		Rotation += (float)delta * _rotationSpeed;
	}
}
