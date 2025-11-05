using Godot;

public partial class Player : CharacterBody2D
{
	// player movement speed
	[Export] public float Speed = 300f;

	public override void _PhysicsProcess(double delta)
	{
		// start with no movement
		Vector2 velocity = Vector2.Zero;

		// move right
		if (Input.IsActionPressed("ui_right"))
			velocity.X += 1;
		
		// move left
		if (Input.IsActionPressed("ui_left"))
			velocity.X -= 1;
		
		// move down
		if (Input.IsActionPressed("ui_down"))
			velocity.Y += 1;
		
		// move up
		if (Input.IsActionPressed("ui_up"))
			velocity.Y -= 1;

		// normalize direction and apply speed
		if (velocity.Length() > 0)
			velocity = velocity.Normalized() * Speed;

		// move using built-in physics so collisions work
		Velocity = velocity;
		MoveAndSlide();
	}
}
