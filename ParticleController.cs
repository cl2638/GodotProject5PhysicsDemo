using Godot;

public partial class ParticleController : GpuParticles2D
{
	// shader for particle distortion
	private ShaderMaterial _shaderMaterial;
	
	// track if particle already exploded
	private bool _hasExploded = false;
	
	// area for collision detection
	private Area2D _area;

	public override void _Ready()
	{
		// stop in editor preview
		if (Engine.IsEditorHint())
		{
			Emitting = false;
			return;
		}

		// --- idle particle setup ---
		// set texture
		Texture = GD.Load<Texture2D>("res://whitecircle2.png");
		
		// number of idle particles
		Amount = 50;
		
		// particle lifetime
		Lifetime = 1.5f;
		
		// repeat continuously
		OneShot = false;
		Emitting = true;

		// particle motion settings
		var processMat = new ParticleProcessMaterial
		{
			Direction = Vector3.Up,
			Gravity = Vector3.Zero,
			InitialVelocityMin = 10f,
			InitialVelocityMax = 25f,
			Spread = 45f,
			ScaleMin = 0.8f,
			ScaleMax = 1.2f
		};
		ProcessMaterial = processMat;

		// shader material for visual effect
		_shaderMaterial = new ShaderMaterial
		{
			Shader = GD.Load<Shader>("res://custom_particle.gdshader")
		};
		Material = _shaderMaterial;

		// --- create Area2D for runtime collision ---
		_area = new Area2D();
		var shape = new CollisionShape2D();
		
		// circle collision shape
		var circle = new CircleShape2D { Radius = 16f };
		shape.Shape = circle;

		_area.AddChild(shape);
		AddChild(_area);

		// connect collision signal
		_area.BodyEntered += OnBodyEntered;
	}

	private void OnBodyEntered(Node2D body)
	{
		// only explode once and only for player
		if (_hasExploded || body.Name != "Player")
			return;

		_hasExploded = true;
		Explode();
	}

	private void Explode()
	{
		// --- create explosion particle burst ---
		var explosion = new GpuParticles2D
		{
			Texture = Texture,
			Amount = 300,
			Lifetime = 0.6f,
			OneShot = true,
			Emitting = true
		};

		// explosion motion
		var processMat = new ParticleProcessMaterial
		{
			InitialVelocityMin = 200f,
			InitialVelocityMax = 400f,
			Spread = 180f,
			Gravity = Vector3.Zero,
			ScaleMin = 0.8f,
			ScaleMax = 1.2f
		};
		explosion.ProcessMaterial = processMat;

		// add explosion to scene
		GetParent().AddChild(explosion);
		explosion.GlobalPosition = GlobalPosition;

		// remove original particle
		QueueFree();
	}

	public override void _Process(double delta)
	{
		// skip if no shader or in editor
		if (_shaderMaterial == null || Engine.IsEditorHint())
			return;

		// update wave effect
		float t = (float)Time.GetTicksMsec() / 1000f;
		float wave = 0.05f + 0.03f * Mathf.Sin(t * 3f);
		_shaderMaterial.SetShaderParameter("wave_intensity", wave);
	}
}
