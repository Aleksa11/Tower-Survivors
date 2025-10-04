using Godot;
using System;

namespace TowerSurvivors
{
    public partial class ArrowTower : Tower
    {
        public ArrowTower()
        {
            TowerName = "Arrow Tower";
            AttackRange = 250f;
            AttackSpeed = 2.0f;
            Damage = 15;
        }

        protected override void SetupVisuals()
        {
            _visual = new ColorRect();
            _visual.Size = new Vector2(50, 50);
            _visual.Position = new Vector2(-25, -25);
            _visual.Color = new Color(0.6f, 0.3f, 0.1f); // Brown color for arrow tower
            AddChild(_visual);

            // Add an arrow indicator on top
            var arrow = new Polygon2D();
            arrow.Polygon = new Vector2[]
            {
                new Vector2(0, -30),
                new Vector2(-10, -15),
                new Vector2(10, -15)
            };
            arrow.Color = new Color(0.9f, 0.9f, 0.2f); // Yellow arrow
            AddChild(arrow);
        }
    }
}
