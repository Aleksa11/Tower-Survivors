using Godot;
using System;
using System.Collections.Generic;

namespace TowerSurvivors
{
    public partial class LightningTower : Tower
    {
        [Export] public int ChainCount { get; set; } = 3; // Hits 3 enemies
        [Export] public float ChainRange { get; set; } = 150f;
        [Export] public float ChainDamageReduction { get; set; } = 0.7f; // 70% damage to next target
        public bool Evolved { get; set; } = false;

        public LightningTower()
        {
            Damage = 20;
            AttackSpeed = 1.5f;
            AttackRange = 200f;
        }

        protected override void SetupVisuals()
        {
            _visual = new ColorRect();
            _visual.Size = new Vector2(50, 50);
            _visual.Position = new Vector2(-25, -25);
            _visual.Color = new Color(1f, 1f, 0.3f); // Yellow
            AddChild(_visual);
        }

        protected override void Attack(Enemy target)
        {
            if (target == null || !IsInstanceValid(target)) return;

            // Chain lightning effect
            List<Enemy> hitEnemies = new List<Enemy>();
            Enemy currentTarget = target;
            int currentDamage = Damage;

            for (int i = 0; i < ChainCount && currentTarget != null; i++)
            {
                if (IsInstanceValid(currentTarget))
                {
                    currentTarget.TakeDamage(currentDamage);
                    hitEnemies.Add(currentTarget);

                    // Create lightning visual
                    Vector2 startPos = (i == 0) ? GlobalPosition : hitEnemies[i - 1].GlobalPosition;
                    CreateLightningBolt(startPos, currentTarget.GlobalPosition);

                    // Find next target in chain
                    currentTarget = FindNextChainTarget(currentTarget, hitEnemies);
                    currentDamage = (int)(currentDamage * ChainDamageReduction);
                }
                else
                {
                    break;
                }
            }
        }

        private Enemy FindNextChainTarget(Enemy currentTarget, List<Enemy> excludeEnemies)
        {
            Enemy closestEnemy = null;
            float closestDistance = ChainRange;

            var enemyContainer = GetTree().Root.FindChild("Enemies", true, false);
            if (enemyContainer != null)
            {
                foreach (var child in enemyContainer.GetChildren())
                {
                    if (child is Enemy enemy && !excludeEnemies.Contains(enemy) && IsInstanceValid(enemy))
                    {
                        float distance = enemy.GlobalPosition.DistanceTo(currentTarget.GlobalPosition);
                        if (distance < closestDistance)
                        {
                            closestDistance = distance;
                            closestEnemy = enemy;
                        }
                    }
                }
            }

            return closestEnemy;
        }

        private void CreateLightningBolt(Vector2 start, Vector2 end)
        {
            var line = new Line2D();
            line.DefaultColor = new Color(1f, 1f, 0.3f, 1f); // Bright yellow
            line.Width = 3f;

            // Add to Main scene
            var mainScene = GetTree().Root.GetChild(0);
            mainScene.AddChild(line);

            // Create jagged lightning effect AFTER adding to tree
            int segments = 5;
            Vector2 direction = (end - start);

            for (int i = 0; i <= segments; i++)
            {
                float t = i / (float)segments;
                Vector2 point = start + direction * t;

                // Add random offset (except for start and end)
                if (i > 0 && i < segments)
                {
                    Vector2 perpendicular = new Vector2(-direction.Y, direction.X).Normalized();
                    float offset = (float)GD.RandRange(-20, 20);
                    point += perpendicular * offset;
                }

                line.AddPoint(point);
            }

            // Fade out
            var tween = GetTree().CreateTween();
            tween.TweenProperty(line, "modulate:a", 0f, 0.3f);
            tween.TweenCallback(Callable.From(() => line.QueueFree()));
        }
    }
}
