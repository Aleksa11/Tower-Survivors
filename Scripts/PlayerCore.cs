using Godot;
using System;

namespace TowerSurvivors
{
    public partial class PlayerCore : Node2D
    {
        [Export] public int MaxHealth { get; set; } = 100;
        [Export] public int CurrentHealth { get; set; }

        [Signal]
        public delegate void HealthChangedEventHandler(int currentHealth, int maxHealth);

        [Signal]
        public delegate void CoreDestroyedEventHandler();

        private Sprite2D _sprite;
        private CollisionShape2D _collisionShape;

        public override void _Ready()
        {
            CurrentHealth = MaxHealth;
            SetupVisuals();
        }

        private void SetupVisuals()
        {
            // Create sprite for visual representation
            _sprite = new Sprite2D();
            AddChild(_sprite);

            // We'll add the actual texture later - for now use a placeholder color rect
            var colorRect = new ColorRect();
            colorRect.Size = new Vector2(100, 100);
            colorRect.Color = new Color(0.2f, 0.6f, 1.0f); // Blue color
            colorRect.Position = new Vector2(-50, -50);
            AddChild(colorRect);
        }

        public void TakeDamage(int damage)
        {
            CurrentHealth -= damage;
            CurrentHealth = Mathf.Max(CurrentHealth, 0);

            EmitSignal(SignalName.HealthChanged, CurrentHealth, MaxHealth);

            if (CurrentHealth <= 0)
            {
                Die();
            }
        }

        public void Heal(int amount)
        {
            CurrentHealth += amount;
            CurrentHealth = Mathf.Min(CurrentHealth, MaxHealth);

            EmitSignal(SignalName.HealthChanged, CurrentHealth, MaxHealth);
        }

        private void Die()
        {
            EmitSignal(SignalName.CoreDestroyed);
            GD.Print("Player Core Destroyed - Game Over!");
        }

        public float GetHealthPercentage()
        {
            return (float)CurrentHealth / MaxHealth;
        }
    }
}
