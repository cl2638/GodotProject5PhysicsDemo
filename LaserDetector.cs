using Godot;

public partial class LaserDetector : Node2D
{
	// length of the laser ray
	[Export] public float LaserLength = 500f;

	// laser colors for normal/alert
	[Export] public Color LaserColorNormal = new Color(0f, 1f, 0f); // green
	[Export] public Color LaserColorAlert = new Color(1f, 0f, 0f);   // red

	// path to player node
	[Export] public NodePath PlayerPath;

	// raycast for detection
	private RayCast2D _rayCast;
	
	// visual line for laser
	private Line2D _laserBeam;
	
	// player reference
	private Node2D _player;
	
	// tracks if player is already detected
	private bool _playerDetected = false;
	
	// timer to reset laser color
	private Timer _alarmTimer;

	public override void _Ready()
	{
		// setup raycast node
		SetupRaycast();
		
		// setup laser visual
		SetupVisuals();

		// get player reference
		_player = GetNodeOrNull<Node2D>(PlayerPath);

		// timer to reset laser color after 3 seconds
		_alarmTimer = new Timer();
		_alarmTimer.WaitTime = 3.0f;
		_alarmTimer.OneShot = true;
		_alarmTimer.Timeout += ResetLaserColor;
		AddChild(_alarmTimer);
	}

	private void SetupRaycast()
	{
		// create raycast node
		_rayCast = new RayCast2D();
		_rayCast.TargetPosition = new Vector2(LaserLength, 0);
		_rayCast.Enabled = true;

		// detect objects on layer 1
		_rayCast.CollisionMask = 1;
		AddChild(_rayCast);
	}

	private void SetupVisuals()
	{
		// create line2D for laser
		_laserBeam = new Line2D();
		_laserBeam.Width = 8f;
		_laserBeam.DefaultColor = LaserColorNormal;
		_laserBeam.Antialiased = true;
		_laserBeam.ZIndex = 100;

		// set laser start/end points
		_laserBeam.AddPoint(Vector2.Zero);
		_laserBeam.AddPoint(new Vector2(LaserLength, 0));
		AddChild(_laserBeam);
	}

	public override void _PhysicsProcess(double delta)
	{
		// update raycast every frame
		_rayCast.ForceRaycastUpdate();

		// check if raycast collides
		if (_rayCast.IsColliding())
		{
			Node collider = _rayCast.GetCollider() as Node;

			// check if collider is player
			if (collider != null && collider == _player)
			{
				if (!_playerDetected)
				{
					GD.Print("Laser hit: Player");
					TriggerLaserAlert(); // trigger alarm
					_playerDetected = true;
				}
			}
		}
		else
		{
			// player left laser
			_playerDetected = false;
		}
	}

	private void TriggerLaserAlert()
	{
		// turn laser red
		_laserBeam.DefaultColor = LaserColorAlert;
		GD.Print("ALARM! Player detected!");
		
		// start timer to reset color
		_alarmTimer.Start();
	}

	private void ResetLaserColor()
	{
		// only reset to green if player not in beam
		if (!_playerDetected)
		{
			_laserBeam.DefaultColor = LaserColorNormal;
		}
		else
		{
			// player still in beam; restart timer
			_alarmTimer.Start();
		}
	}
}
