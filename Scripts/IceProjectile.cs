using Godot;
using System;

namespace TowerSurvivors
{
    public partial class IceProjectile : Projectile
    {
        public float SlowAmount { get; set; } = 0.5f;
        public float SlowDuration { get; set; } = 2f;

        public IceProjectile()
        {
            ProjectileColor = new Color(0.7f, 0.9f, 1f);
        }

        protected override void HitTarget()
        {
            if (Target != null && IsInstanceValid(Target))
            {
                Target.TakeDamage(Damage);
                Target.ApplySlow(SlowAmount, SlowDuration);

                // Create ice visual effect
                var iceEffect = new ColorRect();
                iceEffect.Size = new Vector2(40, 40);
                iceEffect.Position = Target.GlobalPosition - iceEffect.Size / 2;
                iceEffect.Color = new Color(0.5f, 0.8f, 1f, 0.6f);

                GetParent().AddChild(iceEffect);

                var tween = GetTree().CreateTween();
                tween.TweenProperty(iceEffect, "modulate:a", 0f, 0.5f);
                tween.TweenCallback(Callable.From(() => iceEffect.QueueFree()));
            }

            QueueFree();
        }
    }
}
