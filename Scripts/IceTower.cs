using Godot;
using System;

namespace TowerSurvivors
{
    public partial class IceTower : Tower
    {
        [Export] public float SlowAmount { get; set; } = 0.5f; // 50% speed
        [Export] public float SlowDuration { get; set; } = 2f;
        public int ProjectileCount { get; set; } = 1; // Can be upgraded

        public IceTower()
        {
            Damage = 8;
            AttackSpeed = 1.2f;
            AttackRange = 220f;
        }

        protected override void SetupVisuals()
        {
            _visual = new ColorRect();
            _visual.Size = new Vector2(50, 50);
            _visual.Position = new Vector2(-25, -25);
            _visual.Color = new Color(0.5f, 0.8f, 1f); // Light blue
            AddChild(_visual);
        }

        protected override void Attack(Enemy target)
        {
            if (target == null || !IsInstanceValid(target)) return;

            // Fire multiple projectiles if upgraded
            for (int i = 0; i < ProjectileCount; i++)
            {
                var projectile = new IceProjectile();
                projectile.Damage = Damage;
                projectile.Speed = 400f;
                projectile.Target = target;
                projectile.SlowAmount = SlowAmount;
                projectile.SlowDuration = SlowDuration;

                // Add to Main scene, then set position
                var mainScene = GetTree().Root.GetChild(0);
                mainScene.AddChild(projectile);
                projectile.GlobalPosition = GlobalPosition;

                // Spread projectiles in an arc
                if (ProjectileCount > 1)
                {
                    float angleOffset = (i - (ProjectileCount - 1) / 2f) * 0.3f;
                    Vector2 direction = (target.GlobalPosition - GlobalPosition).Normalized();
                    direction = direction.Rotated(angleOffset);
                    projectile.Rotation = direction.Angle();
                }
            }
        }
    }
}
