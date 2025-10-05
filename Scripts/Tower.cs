using Godot;
using System;

namespace TowerSurvivors
{
	public partial class Tower : Node2D
	{
		[Export] public float AttackRange { get; set; } = 200f;
		[Export] public float AttackSpeed { get; set; } = 1.0f; // Attacks per second
		[Export] public int Damage { get; set; } = 10;
		[Export] public string TowerName { get; set; } = "Tower";

		protected Enemy _currentTarget;
		protected float _attackCooldown = 0f;
		protected Area2D _detectionArea;
		protected Sprite2D _visual;
		protected AudioStreamPlayer2D _shootSound;
		protected CpuParticles2D _muzzleFlash;

		public override void _Ready()
		{
			SetupDetectionArea();
			SetupVisuals();
			SetupAudio();
			SetupMuzzleFlash();
		}

		protected virtual void SetupDetectionArea()
		{
			_detectionArea = new Area2D();
			_detectionArea.Name = "DetectionArea";
			AddChild(_detectionArea);

			var collisionShape = new CollisionShape2D();
			var shape = new CircleShape2D();
			shape.Radius = AttackRange;
			collisionShape.Shape = shape;
			_detectionArea.AddChild(collisionShape);

			_detectionArea.AreaEntered += OnEnemyEntered;
			_detectionArea.AreaExited += OnEnemyExited;
		}

		protected virtual void SetupVisuals()
		{
			// Scale factor to make towers stand out (20% larger)
			float towerScale = 1.2f;

			// Create shadow (bottom-most) - more prominent
			var shadow = new Sprite2D();
			shadow.Name = "Shadow";
			shadow.Position = new Vector2(4, 10); // Offset down-right
			shadow.Modulate = new Color(0f, 0f, 0f, 0.5f); // Darker shadow
			shadow.Scale = new Vector2(1.1f * towerScale, 0.5f * towerScale); // Squashed ellipse
			shadow.ZIndex = -3;

			// Create glow effect layer
			var glow = new Sprite2D();
			glow.Name = "Glow";
			glow.Position = Vector2.Zero;
			glow.Modulate = new Color(0.5f, 0.8f, 1.0f, 0.4f); // Cyan glow
			glow.Scale = new Vector2(1.3f * towerScale, 1.3f * towerScale);
			glow.ZIndex = -2;

			// Create outline sprite (behind main sprite)
			var outline = new Sprite2D();
			outline.Name = "Outline";
			outline.Position = Vector2.Zero;
			outline.Modulate = new Color(0.1f, 0.3f, 0.5f, 0.8f); // Blue-tinted outline
			outline.Scale = new Vector2(1.15f * towerScale, 1.15f * towerScale);
			outline.ZIndex = -1;

			// Create main sprite - larger scale
			_visual = new Sprite2D();
			_visual.Position = Vector2.Zero;
			_visual.Scale = new Vector2(towerScale, towerScale);

			// Create a placeholder texture if no sprite is loaded
			var placeholderTexture = CreatePlaceholderTexture(50, 50, new Color(0.8f, 0.4f, 0.2f));
			_visual.Texture = placeholderTexture;
			outline.Texture = placeholderTexture;
			shadow.Texture = placeholderTexture;
			glow.Texture = placeholderTexture;

			AddChild(shadow);
			AddChild(glow);
			AddChild(outline);
			AddChild(_visual);

			// Add pulsing glow animation
			CreateGlowAnimation(glow);
		}

		private void CreateGlowAnimation(Sprite2D glow)
		{
			var tween = CreateTween();
			tween.SetLoops();
			tween.TweenProperty(glow, "modulate:a", 0.2f, 1.5f).SetTrans(Tween.TransitionType.Sine);
			tween.TweenProperty(glow, "modulate:a", 0.5f, 1.5f).SetTrans(Tween.TransitionType.Sine);
		}

		protected virtual void SetupAudio()
		{
			_shootSound = new AudioStreamPlayer2D();
			_shootSound.Name = "ShootSound";
			_shootSound.MaxDistance = 1000f;
			// Audio stream can be set in derived classes or loaded from Assets/Audio/SFX
			AddChild(_shootSound);
		}

		protected virtual void SetupMuzzleFlash()
		{
			_muzzleFlash = new CpuParticles2D();
			_muzzleFlash.Name = "MuzzleFlash";
			_muzzleFlash.Emitting = false;
			_muzzleFlash.OneShot = true;
			_muzzleFlash.Amount = 10;
			_muzzleFlash.Lifetime = 0.2f;
			_muzzleFlash.SpeedScale = 2.0f;

			// Particle properties
			_muzzleFlash.EmissionShape = CpuParticles2D.EmissionShapeEnum.Sphere;
			_muzzleFlash.EmissionSphereRadius = 5f;
			_muzzleFlash.Direction = new Vector2(0, -1);
			_muzzleFlash.Spread = 45f;
			_muzzleFlash.InitialVelocityMin = 50f;
			_muzzleFlash.InitialVelocityMax = 100f;
			_muzzleFlash.Scale = new Vector2(1, 1);

			// Visual properties
			_muzzleFlash.Color = new Color(1f, 0.8f, 0.2f); // Yellow-orange flash
			_muzzleFlash.ColorRamp = CreateMuzzleFlashGradient();

			AddChild(_muzzleFlash);
		}

		protected Gradient CreateMuzzleFlashGradient()
		{
			var gradient = new Gradient();
			gradient.SetColor(0, new Color(1f, 1f, 0.5f, 1f)); // Bright yellow start
			gradient.SetColor(1, new Color(1f, 0.3f, 0f, 0f)); // Fade to transparent orange
			return gradient;
		}

		protected ImageTexture CreatePlaceholderTexture(int width, int height, Color color)
		{
			var image = Image.CreateEmpty(width, height, false, Image.Format.Rgba8);
			image.Fill(color);
			return ImageTexture.CreateFromImage(image);
		}

		public override void _Process(double delta)
		{
			if (_attackCooldown > 0)
			{
				_attackCooldown -= (float)delta;
			}

			if (_currentTarget != null && IsInstanceValid(_currentTarget))
			{
				// Rotate tower to face target
				RotateTowardsTarget(_currentTarget);

				if (_attackCooldown <= 0)
				{
					Attack(_currentTarget);
					_attackCooldown = 1f / AttackSpeed;
				}
			}
			else
			{
				// Reset rotation when no target
				ResetRotation();
				FindNewTarget();
			}
		}

		protected virtual void RotateTowardsTarget(Enemy target)
		{
			if (_visual == null || target == null) return;

			Vector2 direction = (target.GlobalPosition - GlobalPosition).Normalized();
			float targetRotation = direction.Angle();

			// Smooth rotation
			_visual.Rotation = Mathf.LerpAngle(_visual.Rotation, targetRotation, 0.2f);
		}

		protected virtual void ResetRotation()
		{
			if (_visual == null) return;

			// Smoothly return to default rotation (0)
			_visual.Rotation = Mathf.LerpAngle(_visual.Rotation, 0f, 0.1f);
		}

		protected virtual void Attack(Enemy target)
		{
			if (target != null && IsInstanceValid(target))
			{
				// Play shoot sound
				if (_shootSound != null && _shootSound.Stream != null)
				{
					_shootSound.Play();
				}

				// Trigger muzzle flash
				if (_muzzleFlash != null)
				{
					_muzzleFlash.Restart();
				}

				// Create projectile
				var projectile = CreateProjectile();
				if (projectile != null)
				{
					GetParent().GetParent().AddChild(projectile); // Add to main scene
					projectile.GlobalPosition = GlobalPosition;
					projectile.SetTarget(target);
				}
			}
		}

		protected virtual Projectile CreateProjectile()
		{
			var projectile = new ArrowProjectile();
			projectile.Damage = Damage;
			projectile.Speed = 400f;
			return projectile;
		}

		protected void FindNewTarget()
		{
			_currentTarget = null;

			var overlappingAreas = _detectionArea.GetOverlappingAreas();
			float closestDistance = float.MaxValue;
			Enemy closestEnemy = null;

			foreach (var area in overlappingAreas)
			{
				if (area.GetParent() is Enemy enemy && IsInstanceValid(enemy))
				{
					float distance = GlobalPosition.DistanceTo(enemy.GlobalPosition);
					if (distance < closestDistance)
					{
						closestDistance = distance;
						closestEnemy = enemy;
					}
				}
			}

			_currentTarget = closestEnemy;
		}

		private void OnEnemyEntered(Area2D area)
		{
			if (_currentTarget == null && area.GetParent() is Enemy enemy)
			{
				_currentTarget = enemy;
			}
		}

		private void OnEnemyExited(Area2D area)
		{
			if (area.GetParent() == _currentTarget)
			{
				FindNewTarget();
			}
		}
	}
}
