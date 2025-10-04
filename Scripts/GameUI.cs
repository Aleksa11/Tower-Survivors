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

			// Main game HUD container
			var hudContainer = new VBoxContainer();
			hudContainer.Position = new Vector2(20, 70);
			hudContainer.MouseFilter = Control.MouseFilterEnum.Ignore; // CRITICAL: Let clicks pass through
			AddChild(hudContainer);

			// Health display
			var healthContainer = new HBoxContainer();
			hudContainer.AddChild(healthContainer);

			var healthLabelTitle = new Label();
			healthLabelTitle.Text = "HP: ";
			healthLabelTitle.AddThemeColorOverride("font_color", Colors.White);
			healthLabelTitle.AddThemeFontSizeOverride("font_size", 24);
			healthContainer.AddChild(healthLabelTitle);

			_healthLabel = new Label();
			_healthLabel.Text = "100/100";
			_healthLabel.AddThemeColorOverride("font_color", Colors.LightGreen);
			_healthLabel.AddThemeFontSizeOverride("font_size", 24);
			healthContainer.AddChild(_healthLabel);

			// Health bar
			_healthBar = new ProgressBar();
			_healthBar.CustomMinimumSize = new Vector2(200, 30);
			_healthBar.MaxValue = 100;
			_healthBar.Value = 100;
			_healthBar.ShowPercentage = false;
			hudContainer.AddChild(_healthBar);

			// Wave display
			_waveLabel = new Label();
			_waveLabel.Text = "Wave: 0";
			_waveLabel.AddThemeColorOverride("font_color", Colors.White);
			_waveLabel.AddThemeFontSizeOverride("font_size", 24);
			hudContainer.AddChild(_waveLabel);

			// Gold display
			var goldContainer = new HBoxContainer();
			hudContainer.AddChild(goldContainer);

			var goldIcon = new Label();
			goldIcon.Text = "💰 ";
			goldIcon.AddThemeFontSizeOverride("font_size", 24);
			goldContainer.AddChild(goldIcon);

			_goldLabel = new Label();
			_goldLabel.Text = "100";
			_goldLabel.AddThemeColorOverride("font_color", Colors.Gold);
			_goldLabel.AddThemeFontSizeOverride("font_size", 24);
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
			xpContainer.CustomMinimumSize = new Vector2(700, 50);
			xpContainer.MouseFilter = Control.MouseFilterEnum.Ignore; // CRITICAL: Let clicks pass through
			AddChild(xpContainer);

			// Label
			_xpLabel = new Label();
			_xpLabel.Text = "⚡ LEVEL UP IN: 30s";
			_xpLabel.HorizontalAlignment = HorizontalAlignment.Center;
			_xpLabel.AddThemeColorOverride("font_color", Colors.Gold);
			_xpLabel.AddThemeFontSizeOverride("font_size", 20);
			xpContainer.AddChild(_xpLabel);

			// Progress bar
			_xpBar = new ProgressBar();
			_xpBar.CustomMinimumSize = new Vector2(700, 25);
			_xpBar.MaxValue = 1.0f;
			_xpBar.Value = 0.0f;
			_xpBar.ShowPercentage = false;

			// Style the bar
			var styleBox = new StyleBoxFlat();
			styleBox.BgColor = new Color(0.2f, 0.2f, 0.3f);
			styleBox.BorderColor = new Color(1, 0.84f, 0);
			styleBox.SetBorderWidthAll(2);

			var fillBox = new StyleBoxFlat();
			fillBox.BgColor = new Color(1, 0.84f, 0, 0.8f);

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

		public void UpdateHealth(int current, int max)
		{
			if (_healthLabel == null || _healthBar == null) return;

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
		}

		public void UpdateWave(int wave)
		{
			if (_waveLabel == null) return;
			_waveLabel.Text = $"Wave: {wave}";
		}

		public void UpdateGold(int gold)
		{
			if (_goldLabel == null) return;
			_goldLabel.Text = gold.ToString();
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
