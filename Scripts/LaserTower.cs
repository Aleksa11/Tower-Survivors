using Godot;
using System;
using System.Collections.Generic;

namespace TowerSurvivors
{
    public partial class LaserTower : Tower
    {
        public int ExtraBeams { get; set; } = 0; // Can be upgraded

        public LaserTower()
        {
            Damage = 15;
            AttackSpeed = 2.5f; // Very fast
            AttackRange = 250f;
        }

        protected override void SetupVisuals()
        {
            _visual = new ColorRect();
            _visual.Size = new Vector2(50, 50);
            _visual.Position = new Vector2(-25, -25);
            _visual.Color = new Color(0f, 1f, 1f); // Cyan
            AddChild(_visual);
        }

        protected override void Attack(Enemy target)
        {
            if (target == null || !IsInstanceValid(target)) return;

            // Instant laser beam - no projectile
            target.TakeDamage(Damage);

            // Create visual laser effect
            CreateLaserBeam(target);

            // Fire extra beams if upgraded
            if (ExtraBeams > 0)
            {
                var enemies = FindNearbyEnemies(target, ExtraBeams);
                foreach (var enemy in enemies)
                {
                    enemy.TakeDamage(Damage);
                    CreateLaserBeam(enemy);
                }
            }
        }

        private List<Enemy> FindNearbyEnemies(Enemy excludeEnemy, int count)
        {
            var enemies = new List<Enemy>();
            var enemyContainer = GetTree().Root.FindChild("Enemies", true, false);

            if (enemyContainer != null)
            {
                foreach (var child in enemyContainer.GetChildren())
                {
                    if (child is Enemy enemy && enemy != excludeEnemy && IsInstanceValid(enemy))
                    {
                        float distance = enemy.GlobalPosition.DistanceTo(GlobalPosition);
                        if (distance <= AttackRange)
                        {
                            enemies.Add(enemy);
                            if (enemies.Count >= count) break;
                        }
                    }
                }
            }

            return enemies;
        }

        private void CreateLaserBeam(Enemy target)
        {
            if (target == null || !IsInstanceValid(target)) return;

            var line = new Line2D();
            line.DefaultColor = new Color(1f, 0f, 0f, 0.9f); // Red laser
            line.Width = 4f;

            // Add to Main scene
            var mainScene = GetTree().Root.GetChild(0);
            mainScene.AddChild(line);

            // Set points AFTER adding to tree
            line.AddPoint(GlobalPosition);
            line.AddPoint(target.GlobalPosition);

            // Fade out quickly
            var tween = GetTree().CreateTween();
            tween.TweenProperty(line, "modulate:a", 0f, 0.2f);
            tween.TweenCallback(Callable.From(() => line.QueueFree()));
        }
    }
}
