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
		protected ColorRect _visual;

		public override void _Ready()
		{
			SetupDetectionArea();
			SetupVisuals();
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
			_visual = new ColorRect();
			_visual.Size = new Vector2(50, 50);
			_visual.Position = new Vector2(-25, -25);
			_visual.Color = new Color(0.8f, 0.4f, 0.2f); // Orange color
			AddChild(_visual);
		}

		public override void _Process(double delta)
		{
			if (_attackCooldown > 0)
			{
				_attackCooldown -= (float)delta;
			}

			if (_currentTarget != null && IsInstanceValid(_currentTarget))
			{
				if (_attackCooldown <= 0)
				{
					Attack(_currentTarget);
					_attackCooldown = 1f / AttackSpeed;
				}
			}
			else
			{
				FindNewTarget();
			}
		}

		protected virtual void Attack(Enemy target)
		{
			if (target != null && IsInstanceValid(target))
			{
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
