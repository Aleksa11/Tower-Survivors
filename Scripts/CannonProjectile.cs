using Godot;
using System;
using System.Collections.Generic;

namespace TowerSurvivors
{
    public partial class CannonProjectile : Projectile
    {
        [Export] public float SplashRadius { get; set; } = 80f;

        public CannonProjectile()
        {
            ProjectileColor = new Color(0.4f, 0.2f, 0.1f);
        }

        protected override void HitTarget()
        {
            // Create splash damage effect
            DealSplashDamage();

            // Create visual explosion effect
            CreateExplosionEffect();

            QueueFree();
        }

        private void DealSplashDamage()
        {
            if (Target == null || !IsInstanceValid(Target)) return;

            Vector2 explosionCenter = Target.GlobalPosition;

            // Find all enemies in splash radius
            var enemyContainer = GetParent().GetNode<Node2D>("Enemies");
            if (enemyContainer != null)
            {
                foreach (var child in enemyContainer.GetChildren())
                {
                    if (child is Enemy enemy && IsInstanceValid(enemy))
                    {
                        float distance = enemy.GlobalPosition.DistanceTo(explosionCenter);
                        if (distance <= SplashRadius)
                        {
                            // Damage falls off with distance
                            float damageMultiplier = 1f - (distance / SplashRadius) * 0.5f;
                            int splashDamage = (int)(Damage * damageMultiplier);
                            enemy.TakeDamage(splashDamage);
                        }
                    }
                }
            }
        }

        private void CreateExplosionEffect()
        {
            if (Target == null || !IsInstanceValid(Target)) return;

            // Screen shake
            var camera = GetTree().Root.GetNode<CameraShake>("Main/Camera2D");
            if (camera != null)
            {
                camera.Shake(8f, 0.2f);
            }

            // Orange explosion circle
            var explosion = new ColorRect();
            explosion.Size = new Vector2(SplashRadius * 2, SplashRadius * 2);
            explosion.Position = Target.GlobalPosition - explosion.Size / 2;
            explosion.Color = new Color(1f, 0.5f, 0f, 0.6f);

            GetParent().AddChild(explosion);

            // Create particle effect
            CreateExplosionParticles(Target.GlobalPosition);

            // Fade out and remove
            var tween = GetTree().CreateTween();
            tween.TweenProperty(explosion, "modulate:a", 0f, 0.3f);
            tween.TweenCallback(Callable.From(() => explosion.QueueFree()));
        }

        private void CreateExplosionParticles(Vector2 position)
        {
            for (int i = 0; i < 12; i++)
            {
                var particle = new ColorRect();
                particle.Size = new Vector2(6, 6);
                particle.Position = position - particle.Size / 2;
                particle.Color = new Color(1f, (float)GD.RandRange(0.3f, 0.7f), 0f);

                GetParent().AddChild(particle);

                // Random direction
                float angle = (float)GD.RandRange(0, Mathf.Tau);
                Vector2 direction = new Vector2(Mathf.Cos(angle), Mathf.Sin(angle));
                float speed = (float)GD.RandRange(100f, 200f);

                var tween = GetTree().CreateTween();
                tween.SetParallel(true);
                tween.TweenProperty(particle, "position", particle.Position + direction * speed * 0.3f, 0.3f);
                tween.TweenProperty(particle, "modulate:a", 0f, 0.3f);
                tween.Chain().TweenCallback(Callable.From(() => particle.QueueFree()));
            }
        }
    }
}
