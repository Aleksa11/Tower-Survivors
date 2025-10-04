using Godot;
using System;
using System.Collections.Generic;

namespace TowerSurvivors
{
    public partial class UpgradeUI : CanvasLayer
    {
        private Panel _upgradePanel;
        private VBoxContainer _cardsContainer;
        private Upgrade _upgrade1, _upgrade2, _upgrade3;

        [Signal]
        public delegate void UpgradeSelectedEventHandler(Upgrade upgrade);

        // Track ranks for UI display
        public Dictionary<string, int> TowerRanks = new Dictionary<string, int>
        {
            { "Lightning", 0 },
            { "Cannon", 0 },
            { "Ice", 0 },
            { "Laser", 0 }
        };

        public override void _Ready()
        {
            CreateUpgradeUI();
            ProcessMode = ProcessModeEnum.Always; // UI works even when paused
        }

        private void CreateUpgradeUI()
        {
            // Main panel (hidden by default)
            _upgradePanel = new Panel();
            _upgradePanel.SetAnchorsPreset(Control.LayoutPreset.FullRect);
            _upgradePanel.Visible = false;
            AddChild(_upgradePanel);

            // Semi-transparent dark background
            var background = new ColorRect();
            background.SetAnchorsPreset(Control.LayoutPreset.FullRect);
            background.Color = new Color(0, 0, 0, 0.8f);
            _upgradePanel.AddChild(background);

            // Center container
            var centerContainer = new VBoxContainer();
            centerContainer.SetAnchorsPreset(Control.LayoutPreset.Center);
            centerContainer.Position = new Vector2(-300, -250);
            centerContainer.CustomMinimumSize = new Vector2(600, 500);
            _upgradePanel.AddChild(centerContainer);

            // Title
            var title = new Label();
            title.Text = "CHOOSE AN UPGRADE";
            title.HorizontalAlignment = HorizontalAlignment.Center;
            title.AddThemeColorOverride("font_color", Colors.Gold);
            title.AddThemeFontSizeOverride("font_size", 36);
            centerContainer.AddChild(title);

            var spacer1 = new Control();
            spacer1.CustomMinimumSize = new Vector2(0, 30);
            centerContainer.AddChild(spacer1);

            // Cards container
            _cardsContainer = new VBoxContainer();
            _cardsContainer.AddThemeConstantOverride("separation", 20);
            centerContainer.AddChild(_cardsContainer);
        }

        public void ShowUpgrades(Upgrade upgrade1, Upgrade upgrade2, Upgrade upgrade3)
        {
            _upgrade1 = upgrade1;
            _upgrade2 = upgrade2;
            _upgrade3 = upgrade3;

            // Clear previous cards
            foreach (var child in _cardsContainer.GetChildren())
            {
                child.QueueFree();
            }

            // Create 3 upgrade cards
            CreateUpgradeCard(upgrade1, 0);
            CreateUpgradeCard(upgrade2, 1);
            CreateUpgradeCard(upgrade3, 2);

            _upgradePanel.Visible = true;
        }

        private void CreateUpgradeCard(Upgrade upgrade, int index)
        {
            var button = new Button();
            button.CustomMinimumSize = new Vector2(580, 120); // Taller for rank bars
            button.AddThemeFontSizeOverride("font_size", 24);

            // Card text with icon and rank
            string rankText = "";
            if (upgrade.RequiredTowerType != null)
            {
                int currentRank = TowerRanks.GetValueOrDefault(upgrade.RequiredTowerType, 0);
                rankText = $"\nRank: {currentRank}/3 {'▮'}{(currentRank >= 1 ? "▮" : "▯")}{(currentRank >= 2 ? "▮" : "▯")}";
            }

            button.Text = $"{upgrade.Icon}  {upgrade.Name}\n{upgrade.Description}{rankText}";

            // Style button - PURPLE for tower-specific upgrades
            var styleBox = new StyleBoxFlat();

            bool isTowerSpecific = upgrade.RequiredTowerType != null;
            if (isTowerSpecific)
            {
                // Purple/violet theme for tower-specific upgrades
                styleBox.BgColor = new Color(0.4f, 0.2f, 0.5f); // Dark purple
                styleBox.BorderColor = new Color(0.8f, 0.4f, 1f); // Bright purple

                // Add hover effect
                var styleBoxHover = new StyleBoxFlat();
                styleBoxHover.BgColor = new Color(0.5f, 0.3f, 0.6f);
                styleBoxHover.BorderColor = new Color(1f, 0.6f, 1f);
                styleBoxHover.SetBorderWidthAll(3);
                styleBoxHover.SetCornerRadiusAll(10);
                button.AddThemeStyleboxOverride("hover", styleBoxHover);
            }
            else
            {
                // Normal blue theme
                styleBox.BgColor = new Color(0.2f, 0.3f, 0.4f);
                styleBox.BorderColor = new Color(0.6f, 0.7f, 0.8f);
            }

            styleBox.SetBorderWidthAll(3);
            styleBox.SetCornerRadiusAll(10);
            button.AddThemeStyleboxOverride("normal", styleBox);

            button.Pressed += () => OnUpgradeSelected(index);

            _cardsContainer.AddChild(button);
        }

        private void OnUpgradeSelected(int index)
        {
            Upgrade selectedUpgrade = null;

            switch (index)
            {
                case 0: selectedUpgrade = _upgrade1; break;
                case 1: selectedUpgrade = _upgrade2; break;
                case 2: selectedUpgrade = _upgrade3; break;
            }

            if (selectedUpgrade != null)
            {
                EmitSignal(SignalName.UpgradeSelected, selectedUpgrade);
                Hide();
            }
        }

        public new void Hide()
        {
            _upgradePanel.Visible = false;
        }
    }
}
