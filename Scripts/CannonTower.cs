using Godot;
using System;

namespace TowerSurvivors
{
    public partial class CannonTower : Tower
    {
        [Export] public float SplashRadius { get; set; } = 80f;

        public CannonTower()
        {
            Damage = 25;
            AttackSpeed = 0.8f;
            AttackRange = 200f;
        }

        protected override void SetupVisuals()
        {
            _visual = new ColorRect();
            _visual.Size = new Vector2(50, 50);
            _visual.Position = new Vector2(-25, -25);
            _visual.Color = new Color(0.6f, 0.3f, 0.1f); // Brown
            AddChild(_visual);
        }

        protected override void Attack(Enemy target)
        {
            if (target == null || !IsInstanceValid(target)) return;

            // Create cannon projectile
            var projectile = new CannonProjectile();
            projectile.Damage = Damage;
            projectile.Speed = 300f;
            projectile.Target = target;
            projectile.SplashRadius = SplashRadius;

            // Add to Main scene, then set position
            var mainScene = GetTree().Root.GetChild(0);
            mainScene.AddChild(projectile);
            projectile.GlobalPosition = GlobalPosition;
        }
    }
}
