using Godot;
using System;
using System.Collections.Generic;

namespace TowerSurvivors
{
    public partial class RankIndicatorUI : CanvasLayer
    {
        private VBoxContainer _container;
        private Dictionary<string, Label> _rankLabels = new Dictionary<string, Label>();

        public override void _Ready()
        {
            CreateIndicatorUI();
        }

        private void CreateIndicatorUI()
        {
            // Container on right side
            _container = new VBoxContainer();
            _container.Position = new Vector2(600, 200); // Right side
            _container.AddThemeConstantOverride("separation", 10);
            _container.MouseFilter = Control.MouseFilterEnum.Ignore;
            AddChild(_container);

            // Create rank displays for each tower type
            CreateRankDisplay("⚡", "Lightning", Colors.Yellow);
            CreateRankDisplay("💥", "Cannon", Colors.Orange);
            CreateRankDisplay("❄️", "Ice", Colors.LightBlue);
            CreateRankDisplay("🔴", "Laser", Colors.Red);
        }

        private void CreateRankDisplay(string icon, string towerType, Color color)
        {
            var label = new Label();
            label.Text = $"{icon} {towerType}: ▯▯▯";
            label.AddThemeColorOverride("font_color", color);
            label.AddThemeFontSizeOverride("font_size", 20);
            label.MouseFilter = Control.MouseFilterEnum.Ignore;
            _container.AddChild(label);

            _rankLabels[towerType] = label;
        }

        public void UpdateRank(string towerType, int rank)
        {
            if (_rankLabels.ContainsKey(towerType))
            {
                string bars = "";
                for (int i = 0; i < 3; i++)
                {
                    bars += (i < rank) ? "▮" : "▯";
                }

                string icon = towerType switch
                {
                    "Lightning" => "⚡",
                    "Cannon" => "💥",
                    "Ice" => "❄️",
                    "Laser" => "🔴",
                    _ => ""
                };

                string evolutionText = rank >= 3 ? " [EVOLVED]" : "";
                _rankLabels[towerType].Text = $"{icon} {towerType}: {bars}{evolutionText}";
            }
        }
    }
}
