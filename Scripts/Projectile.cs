using Godot;
using System;

namespace TowerSurvivors
{
    public partial class Projectile : Node2D
    {
        [Export] public float Speed { get; set; } = 300f;
        [Export] public int Damage { get; set; } = 10;

        protected Enemy _target;
        protected ColorRect _visual;
        protected Color ProjectileColor { get; set; } = new Color(1, 1, 0);

        public Enemy Target
        {
            get => _target;
            set => _target = value;
        }

        public override void _Ready()
        {
            SetupVisuals();
        }

        protected virtual void SetupVisuals()
        {
            _visual = new ColorRect();
            _visual.Size = new Vector2(10, 10);
            _visual.Position = new Vector2(-5, -5);
            _visual.Color = ProjectileColor;
            AddChild(_visual);
        }

        public void SetTarget(Enemy target)
        {
            _target = target;
        }

        public override void _Process(double delta)
        {
            if (_target == null || !IsInstanceValid(_target))
            {
                QueueFree();
                return;
            }

            // Move toward target
            Vector2 direction = ((_target.GlobalPosition - GlobalPosition).Normalized());
            GlobalPosition += direction * Speed * (float)delta;

            // Rotate to face target
            Rotation = direction.Angle();

            // Check if reached target
            if (GlobalPosition.DistanceTo(_target.GlobalPosition) < 10f)
            {
                HitTarget();
            }
        }

        protected virtual void HitTarget()
        {
            if (_target != null && IsInstanceValid(_target))
            {
                _target.TakeDamage(Damage);
            }
            QueueFree();
        }
    }
}
