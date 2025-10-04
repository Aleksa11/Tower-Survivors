using Godot;
using System;

namespace TowerSurvivors
{
    public partial class PlacementIndicator : CanvasLayer
    {
        private Label _instructionLabel;

        public override void _Ready()
        {
            CreateIndicator();
        }

        private void CreateIndicator()
        {
            // Instruction label at top center
            _instructionLabel = new Label();
            _instructionLabel.Text = "";
            _instructionLabel.Position = new Vector2(200, 120);
            _instructionLabel.CustomMinimumSize = new Vector2(320, 50);
            _instructionLabel.HorizontalAlignment = HorizontalAlignment.Center;
            _instructionLabel.AddThemeColorOverride("font_color", Colors.Yellow);
            _instructionLabel.AddThemeFontSizeOverride("font_size", 24);
            _instructionLabel.Visible = false;
            _instructionLabel.MouseFilter = Control.MouseFilterEnum.Ignore; // CRITICAL: Let clicks pass through

            // Add background for visibility
            var bg = new ColorRect();
            bg.Size = new Vector2(320, 50);
            bg.Position = new Vector2(200, 120);
            bg.Color = new Color(0, 0, 0, 0.7f);
            bg.Visible = false;
            bg.MouseFilter = Control.MouseFilterEnum.Ignore; // CRITICAL: Let clicks pass through
            AddChild(bg);

            AddChild(_instructionLabel);
        }

        public void ShowPlacementMode(string towerName)
        {
            _instructionLabel.Text = $"Click a green slot to place {towerName}";
            _instructionLabel.Visible = true;
            GetChild(0).Set("visible", true); // Show background
        }

        public void HidePlacementMode()
        {
            _instructionLabel.Visible = false;
            GetChild(0).Set("visible", false); // Hide background
        }
    }
}
