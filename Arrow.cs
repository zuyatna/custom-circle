using Godot;
using System;

public partial class Arrow : Sprite2D
{
	[Export] private Area2D _pointer;
	
	// Speed of the arrow rotation
	private float _rotationSpeed = 2f;

	public override void _Ready()
	{
		AddToGroup("Arrow");
		
		// Connect the "area_entered" signal to detect collision with circle segments
		_pointer.Connect("area_entered", new Callable(this, nameof(OnArea2DBodyEntered)));
	}

	public override void _Process(double delta)
	{
		// Rotate the arrow continuously
		Rotation += (float)delta * _rotationSpeed;
	}

	private void OnArea2DBodyEntered(Node body)
	{
		if (body.IsInGroup("CircleSegment"))
		{
			GD.Print("Circle segment entered");
		}
	}
}
