using Godot;
using System;

namespace TowerSurvivors
{
    public partial class ArrowProjectile : Projectile
    {
        public ArrowProjectile()
        {
            Speed = 400f;
        }

        protected override void SetupVisuals()
        {
            // Arrow shape
            var arrow = new Polygon2D();
            arrow.Polygon = new Vector2[]
            {
                new Vector2(10, 0),
                new Vector2(-5, -5),
                new Vector2(-5, 5)
            };
            arrow.Color = new Color(0.9f, 0.7f, 0.2f); // Gold color
            AddChild(arrow);
        }
    }
}
