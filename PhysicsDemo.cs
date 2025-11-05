using Godot;
using System.Collections.Generic;

public partial class PhysicsDemo : Node2D
{
	// chain segment scene
	[Export] public PackedScene SegmentScene;
	
	// number of rope links
	[Export] public int ChainSegments = 8;
	
	// spacing factor
	[Export] public float SegmentSpacingMultiplier = 1.1f;

	// store all segments
	private List<RigidBody2D> _segments = new();

	public override void _Ready()
	{
		// warn if no scene
		if (SegmentScene == null)
		{
			GD.PrintErr("SegmentScene not assigned!");
			return;
		}

		// create rope on ready
		CreateRope();
	}

	private void CreateRope()
	{
		// top anchor node
		var anchor = new StaticBody2D();
		
		// attach anchor
		AddChild(anchor);
		
		// place at origin
		anchor.Position = Vector2.Zero;
		
		// get path for joints
		var anchorPath = anchor.GetPath();

		// store previous segment for joints
		RigidBody2D prevSegment = null;

		for (int i = 0; i < ChainSegments; i++)
		{
			// create segment
			var seg = SegmentScene.Instantiate<RigidBody2D>();
			
			// add to scene
			AddChild(seg);
			
			// name segment
			seg.Name = $"Segment_{i}";

			// default spacing
			float spacing = 32f;

			// check sprite for spacing
			var sprite = seg.GetNodeOrNull<Sprite2D>("Sprite2D");
			if (sprite != null && sprite.Texture != null)
				spacing = sprite.Texture.GetSize().Y * sprite.Scale.Y * SegmentSpacingMultiplier;
			else
			{
				// check collision shape for spacing
				var shapeNode = seg.GetNodeOrNull<CollisionShape2D>("CollisionShape2D");
				if (shapeNode?.Shape is RectangleShape2D rect)
					spacing = rect.Size.Y * SegmentSpacingMultiplier;
			}

			// place segment
			seg.Position = new Vector2(0, i * spacing);

			// set minimal physics
			seg.LinearDamp = 0.2f;
			seg.AngularDamp = 0.2f;
			seg.Mass = 1f;
			
			// awake for physics
			seg.Sleeping = false;

			// add to list
			_segments.Add(seg);

			// create joint
			var joint = new PinJoint2D();
			
			// add joint to scene
			AddChild(joint);

			if (i == 0)
			{
				// connect first segment to anchor
				joint.NodeA = anchorPath;
				joint.NodeB = seg.GetPath();
				joint.GlobalPosition = (anchor.GlobalPosition + seg.GlobalPosition) / 2f;
			}
			else
			{
				// connect segment to previous segment
				joint.NodeA = prevSegment.GetPath();
				joint.NodeB = seg.GetPath();
				joint.GlobalPosition = (prevSegment.GlobalPosition + seg.GlobalPosition) / 2f;
			}

			// joint bias
			joint.Bias = 0.1f;
			
			// joint softness
			joint.Softness = 0.2f;
			
			// disable collision between segments
			joint.DisableCollision = true;

			// update previous segment
			prevSegment = seg;
		}

		// small push to bottom segment to start swing
		if (_segments.Count > 0)
			_segments[^1].ApplyImpulse(new Vector2(20f, -5f));
	}
}
