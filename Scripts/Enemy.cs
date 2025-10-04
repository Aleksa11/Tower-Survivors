using Godot;
using System;

namespace TowerSurvivors
{
    public partial class Enemy : Node2D
    {
        [Export] public int MaxHealth { get; set; } = 50;
        [Export] public float MoveSpeed { get; set; } = 100f;
        [Export] public int Damage { get; set; } = 10;
        [Export] public int GoldReward { get; set; } = 5;
        [Export] public int XPReward { get; set; } = 10;

        public int CurrentHealth { get; private set; }
        public Vector2 TargetPosition { get; set; }

        // Slow effect
        private float _slowMultiplier = 1f;
        private float _slowTimer = 0f;
        public float BaseSpeed { get; private set; }

        [Signal]
        public delegate void DiedEventHandler(Enemy enemy);

        [Signal]
        public delegate void ReachedCoreEventHandler(Enemy enemy, int damage);

        private Area2D _hitbox;
        private ColorRect _visual;
        private ColorRect _healthBar;
        private ColorRect _healthBarBackground;

        public override void _Ready()
        {
            CurrentHealth = MaxHealth;
            BaseSpeed = MoveSpeed;
            SetupHitbox();
            SetupVisuals();
        }

        private void SetupHitbox()
        {
            _hitbox = new Area2D();
            _hitbox.Name = "Hitbox";
            AddChild(_hitbox);

            var collisionShape = new CollisionShape2D();
            var shape = new CircleShape2D();
            shape.Radius = 20;
            collisionShape.Shape = shape;
            _hitbox.AddChild(collisionShape);
        }

        private void SetupVisuals()
        {
            _visual = new ColorRect();
            _visual.Size = new Vector2(30, 30);
            _visual.Position = new Vector2(-15, -15);
            _visual.Color = new Color(1, 0.2f, 0.2f); // Red
            AddChild(_visual);

            // Health bar background
            _healthBarBackground = new ColorRect();
            _healthBarBackground.Size = new Vector2(30, 4);
            _healthBarBackground.Position = new Vector2(-15, -25);
            _healthBarBackground.Color = new Color(0.2f, 0.2f, 0.2f);
            AddChild(_healthBarBackground);

            // Health bar
            _healthBar = new ColorRect();
            _healthBar.Size = new Vector2(30, 4);
            _healthBar.Position = new Vector2(-15, -25);
            _healthBar.Color = new Color(0, 1, 0);
            AddChild(_healthBar);
        }

        public override void _Process(double delta)
        {
            // Update slow effect
            if (_slowTimer > 0)
            {
                _slowTimer -= (float)delta;
                if (_slowTimer <= 0)
                {
                    _slowMultiplier = 1f;
                    _visual.Color = new Color(1, 0.2f, 0.2f); // Back to red
                }
            }

            MoveTowardTarget(delta);
            CheckIfReachedCore();
        }

        private void MoveTowardTarget(double delta)
        {
            if (TargetPosition == Vector2.Zero) return;

            Vector2 direction = (TargetPosition - GlobalPosition).Normalized();
            float currentSpeed = BaseSpeed * _slowMultiplier;
            GlobalPosition += direction * currentSpeed * (float)delta;
        }

        public void ApplySlow(float slowAmount, float duration)
        {
            _slowMultiplier = slowAmount; // 0.5 = 50% speed
            _slowTimer = duration;
            _visual.Color = new Color(0.5f, 0.8f, 1f); // Light blue when slowed
        }

        private void CheckIfReachedCore()
        {
            if (GlobalPosition.DistanceTo(TargetPosition) < 20f)
            {
                ReachCore();
            }
        }

        public void TakeDamage(int damage)
        {
            CurrentHealth -= damage;
            CurrentHealth = Mathf.Max(CurrentHealth, 0);

            UpdateHealthBar();

            if (CurrentHealth <= 0)
            {
                Die();
            }
        }

        private void UpdateHealthBar()
        {
            float healthPercent = (float)CurrentHealth / MaxHealth;
            _healthBar.Size = new Vector2(30 * healthPercent, 4);

            // Color gradient from green to red
            _healthBar.Color = new Color(1 - healthPercent, healthPercent, 0);
        }

        private void ReachCore()
        {
            EmitSignal(SignalName.ReachedCore, this, Damage);
            QueueFree();
        }

        private void Die()
        {
            EmitSignal(SignalName.Died, this);
            QueueFree();
        }

        public Area2D GetHitbox()
        {
            return _hitbox;
        }
    }
}
