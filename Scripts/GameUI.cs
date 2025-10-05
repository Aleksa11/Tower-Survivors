using Godot;
using System;

namespace TowerSurvivors
{
	public partial class GameUI : CanvasLayer
	{
		private Label _healthLabel;
		private Label _waveLabel;
		private Label _goldLabel;
		private ProgressBar _healthBar;
		private ProgressBar _xpBar;
		private Label _xpLabel;
		private Panel _gameOverPanel;
		private Panel _victoryPanel;
		private Label _gameOverLabel;
		private Label _victoryLabel;
		private Button _restartButton;
		private Button _victoryRestartButton;

		public override void _Ready()
		{
			CreateUI();
		}

		private void CreateUI()
		{
			// XP Bar at the very top
			CreateXPBar();

			// Main game HUD container with background panel
			var hudPanel = new Panel();
			hudPanel.Position = new Vector2(15, 75);
			hudPanel.CustomMinimumSize = new Vector2(230, 160);
			hudPanel.MouseFilter = Control.MouseFilterEnum.Ignore;
			AddChild(hudPanel);

			// Panel background with premium styling
			var panelBg = new StyleBoxFlat();
			panelBg.BgColor = new Color(0.05f, 0.06f, 0.08f, 0.85f);
			panelBg.BorderColor = new Color(0.3f, 0.5f, 0.7f, 0.6f);
			panelBg.SetBorderWidthAll(2);
			panelBg.CornerRadiusTopLeft = 8;
			panelBg.CornerRadiusTopRight = 8;
			panelBg.CornerRadiusBottomLeft = 8;
			panelBg.CornerRadiusBottomRight = 8;
			hudPanel.AddThemeStyleboxOverride("panel", panelBg);

			var hudContainer = new VBoxContainer();
			hudContainer.Position = new Vector2(10, 10);
			hudContainer.AddThemeConstantOverride("separation", 8);
			hudContainer.MouseFilter = Control.MouseFilterEnum.Ignore;
			hudPanel.AddChild(hudContainer);

			// Health display with outline
			var healthContainer = new HBoxContainer();
			hudContainer.AddChild(healthContainer);

			var healthLabelTitle = new Label();
			healthLabelTitle.Text = "HP: ";
			healthLabelTitle.AddThemeColorOverride("font_color", Colors.White);
			healthLabelTitle.AddThemeFontSizeOverride("font_size", 24);
			healthLabelTitle.AddThemeColorOverride("font_outline_color", new Color(0, 0, 0, 0.8f));
			healthLabelTitle.AddThemeConstantOverride("outline_size", 3);
			healthContainer.AddChild(healthLabelTitle);

			_healthLabel = new Label();
			_healthLabel.Text = "100/100";
			_healthLabel.AddThemeColorOverride("font_color", Colors.LightGreen);
			_healthLabel.AddThemeFontSizeOverride("font_size", 24);
			_healthLabel.AddThemeColorOverride("font_outline_color", new Color(0, 0, 0, 0.8f));
			_healthLabel.AddThemeConstantOverride("outline_size", 3);
			healthContainer.AddChild(_healthLabel);

			// Health bar with better styling
			_healthBar = new ProgressBar();
			_healthBar.CustomMinimumSize = new Vector2(200, 32);
			_healthBar.MaxValue = 100;
			_healthBar.Value = 100;
			_healthBar.ShowPercentage = false;
			hudContainer.AddChild(_healthBar);

			// Wave display with better styling
			_waveLabel = new Label();
			_waveLabel.Text = "Wave: 0";
			_waveLabel.AddThemeColorOverride("font_color", new Color(0.8f, 0.9f, 1.0f));
			_waveLabel.AddThemeFontSizeOverride("font_size", 24);
			_waveLabel.AddThemeColorOverride("font_outline_color", new Color(0, 0, 0, 0.8f));
			_waveLabel.AddThemeConstantOverride("outline_size", 3);
			hudContainer.AddChild(_waveLabel);

			// Gold display with better contrast
			var goldContainer = new HBoxContainer();
			hudContainer.AddChild(goldContainer);

			var goldIcon = new Label();
			goldIcon.Text = "💰 ";
			goldIcon.AddThemeFontSizeOverride("font_size", 26);
			goldContainer.AddChild(goldIcon);

			_goldLabel = new Label();
			_goldLabel.Text = "100";
			_goldLabel.AddThemeColorOverride("font_color", new Color(1.0f, 0.85f, 0.1f));
			_goldLabel.AddThemeFontSizeOverride("font_size", 26);
			_goldLabel.AddThemeColorOverride("font_outline_color", new Color(0, 0, 0, 0.9f));
			_goldLabel.AddThemeConstantOverride("outline_size", 3);
			goldContainer.AddChild(_goldLabel);

			// Game Over screen
			CreateGameOverScreen();

			// Victory screen
			CreateVictoryScreen();
		}

		private void CreateXPBar()
		{
			// Container for XP bar at top
			var xpContainer = new VBoxContainer();
			xpContainer.Position = new Vector2(10, 10);
			xpContainer.CustomMinimumSize = new Vector2(700, 60);
			xpContainer.MouseFilter = Control.MouseFilterEnum.Ignore;
			AddChild(xpContainer);

			// Label with better styling
			_xpLabel = new Label();
			_xpLabel.Text = "⚡ LEVEL UP IN: 30s";
			_xpLabel.HorizontalAlignment = HorizontalAlignment.Center;
			_xpLabel.AddThemeColorOverride("font_color", new Color(1.0f, 0.9f, 0.3f)); // Brighter gold
			_xpLabel.AddThemeFontSizeOverride("font_size", 22);

			// Add outline/shadow effect to label
			_xpLabel.AddThemeColorOverride("font_outline_color", new Color(0, 0, 0, 0.8f));
			_xpLabel.AddThemeConstantOverride("outline_size", 4);
			xpContainer.AddChild(_xpLabel);

			// Progress bar with better styling
			_xpBar = new ProgressBar();
			_xpBar.CustomMinimumSize = new Vector2(700, 30);
			_xpBar.MaxValue = 1.0f;
			_xpBar.Value = 0.0f;
			_xpBar.ShowPercentage = false;

			// Style the bar with premium look
			var styleBox = new StyleBoxFlat();
			styleBox.BgColor = new Color(0.1f, 0.1f, 0.15f, 0.9f);
			styleBox.BorderColor = new Color(0.8f, 0.7f, 0.2f, 0.8f);
			styleBox.SetBorderWidthAll(2);
			styleBox.CornerRadiusTopLeft = 4;
			styleBox.CornerRadiusTopRight = 4;
			styleBox.CornerRadiusBottomLeft = 4;
			styleBox.CornerRadiusBottomRight = 4;

			var fillBox = new StyleBoxFlat();
			fillBox.BgColor = new Color(1, 0.84f, 0, 0.9f);
			fillBox.CornerRadiusTopLeft = 3;
			fillBox.CornerRadiusTopRight = 3;
			fillBox.CornerRadiusBottomLeft = 3;
			fillBox.CornerRadiusBottomRight = 3;

			xpContainer.AddChild(_xpBar);
		}

		private void CreateGameOverScreen()
		{
			_gameOverPanel = new Panel();
			_gameOverPanel.SetAnchorsPreset(Control.LayoutPreset.FullRect);
			_gameOverPanel.Visible = false;
			AddChild(_gameOverPanel);

			var gameOverContainer = new VBoxContainer();
			gameOverContainer.SetAnchorsPreset(Control.LayoutPreset.Center);
			gameOverContainer.Position = new Vector2(-150, -100);
			gameOverContainer.CustomMinimumSize = new Vector2(300, 200);
			_gameOverPanel.AddChild(gameOverContainer);

			_gameOverLabel = new Label();
			_gameOverLabel.Text = "GAME OVER";
			_gameOverLabel.HorizontalAlignment = HorizontalAlignment.Center;
			_gameOverLabel.AddThemeColorOverride("font_color", Colors.Red);
			_gameOverLabel.AddThemeFontSizeOverride("font_size", 48);
			gameOverContainer.AddChild(_gameOverLabel);

			var spacer = new Control();
			spacer.CustomMinimumSize = new Vector2(0, 20);
			gameOverContainer.AddChild(spacer);

			_restartButton = new Button();
			_restartButton.Text = "Restart";
			_restartButton.CustomMinimumSize = new Vector2(200, 60);
			_restartButton.AddThemeFontSizeOverride("font_size", 32);
			_restartButton.Pressed += OnRestartPressed;
			gameOverContainer.AddChild(_restartButton);
		}

		private void CreateVictoryScreen()
		{
			_victoryPanel = new Panel();
			_victoryPanel.SetAnchorsPreset(Control.LayoutPreset.FullRect);
			_victoryPanel.Visible = false;
			AddChild(_victoryPanel);

			var victoryContainer = new VBoxContainer();
			victoryContainer.SetAnchorsPreset(Control.LayoutPreset.Center);
			victoryContainer.Position = new Vector2(-150, -100);
			victoryContainer.CustomMinimumSize = new Vector2(300, 200);
			_victoryPanel.AddChild(victoryContainer);

			_victoryLabel = new Label();
			_victoryLabel.Text = "VICTORY!";
			_victoryLabel.HorizontalAlignment = HorizontalAlignment.Center;
			_victoryLabel.AddThemeColorOverride("font_color", Colors.Gold);
			_victoryLabel.AddThemeFontSizeOverride("font_size", 48);
			victoryContainer.AddChild(_victoryLabel);

			var spacer = new Control();
			spacer.CustomMinimumSize = new Vector2(0, 20);
			victoryContainer.AddChild(spacer);

			_victoryRestartButton = new Button();
			_victoryRestartButton.Text = "Play Again";
			_victoryRestartButton.CustomMinimumSize = new Vector2(200, 60);
			_victoryRestartButton.AddThemeFontSizeOverride("font_size", 32);
			_victoryRestartButton.Pressed += OnRestartPressed;
			victoryContainer.AddChild(_victoryRestartButton);
		}

		private int _previousHealth = 100;

		public void UpdateHealth(int current, int max)
		{
			if (_healthLabel == null || _healthBar == null) return;

			// Check if health decreased
			bool healthDecreased = current < _previousHealth;
			_previousHealth = current;

			_healthLabel.Text = $"{current}/{max}";
			_healthBar.MaxValue = max;
			_healthBar.Value = current;

			// Update health bar color based on percentage
			float percentage = (float)current / max;
			if (percentage > 0.6f)
			{
				_healthBar.Modulate = Colors.LightGreen;
			}
			else if (percentage > 0.3f)
			{
				_healthBar.Modulate = Colors.Yellow;
			}
			else
			{
				_healthBar.Modulate = Colors.Red;
			}

			// Add damage flash effect
			if (healthDecreased)
			{
				var tween = CreateTween();
				tween.TweenProperty(_healthLabel, "modulate", new Color(2.0f, 0.5f, 0.5f), 0.1f);
				tween.TweenProperty(_healthLabel, "modulate", Colors.White, 0.3f);
			}
		}

		public void UpdateWave(int wave)
		{
			if (_waveLabel == null) return;
			_waveLabel.Text = $"Wave: {wave}";
		}

		public void UpdateGold(int gold)
		{
			if (_goldLabel == null) return;

			// Get previous gold value
			int previousGold = 0;
			if (int.TryParse(_goldLabel.Text, out int parsed))
			{
				previousGold = parsed;
			}

			// Update text immediately
			_goldLabel.Text = gold.ToString();

			// Add visual feedback for changes
			if (gold > previousGold)
			{
				// Gold increased - positive flash
				var tween = CreateTween();
				tween.TweenProperty(_goldLabel, "modulate", new Color(1.5f, 1.5f, 0.5f), 0.15f);
				tween.TweenProperty(_goldLabel, "modulate", Colors.White, 0.3f);

				// Scale pulse
				tween.Parallel().TweenProperty(_goldLabel, "scale", new Vector2(1.2f, 1.2f), 0.15f);
				tween.TweenProperty(_goldLabel, "scale", new Vector2(1.0f, 1.0f), 0.3f);
			}
			else if (gold < previousGold)
			{
				// Gold decreased - negative flash
				var tween = CreateTween();
				tween.TweenProperty(_goldLabel, "modulate", new Color(1.0f, 0.5f, 0.5f), 0.15f);
				tween.TweenProperty(_goldLabel, "modulate", Colors.White, 0.3f);
			}
		}

		public void UpdateXPBar(float progress, int currentXP, int xpToNext, int level)
		{
			if (_xpBar == null || _xpLabel == null) return;

			_xpBar.Value = progress;
			_xpLabel.Text = $"⚡ LEVEL {level} | XP: {currentXP}/{xpToNext}";
		}

		public void ShowGameOverScreen()
		{
			_gameOverPanel.Visible = true;
		}

		public void ShowVictoryScreen()
		{
			_victoryPanel.Visible = true;
		}

		private void OnRestartPressed()
		{
			GetTree().ReloadCurrentScene();
		}
	}
}
