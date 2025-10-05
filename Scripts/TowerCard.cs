using Godot;
using System;

namespace TowerSurvivors
{
    public partial class TowerCard : Panel
    {
        public string TowerName { get; set; }
        public string TowerType { get; set; }
        public int Cost { get; set; }
        public string Icon { get; set; }

        private Label _nameLabel;
        private Label _costLabel;
        private Label _iconLabel;
        private ColorRect _background;
        private ColorRect _selectionBorder;
        private bool _canAfford = false;
        private bool _isSelected = false;

        [Signal]
        public delegate void CardClickedEventHandler(TowerCard card);

        public override void _Ready()
        {
            CustomMinimumSize = new Vector2(135, 145);
            MouseFilter = MouseFilterEnum.Stop; // Enable mouse input
            CreateCardUI();

            GuiInput += OnGuiInput;
            MouseEntered += OnMouseEntered;
            MouseExited += OnMouseExited;
        }

        private void OnMouseEntered()
        {
            if (_canAfford)
            {
                // Smooth scale up animation
                var tween = CreateTween();
                tween.TweenProperty(this, "scale", new Vector2(1.05f, 1.05f), 0.15f).SetTrans(Tween.TransitionType.Back).SetEase(Tween.EaseType.Out);
                tween.Parallel().TweenProperty(_background, "color", new Color(0.25f, 0.35f, 0.5f), 0.15f);
            }
        }

        private void OnMouseExited()
        {
            if (_canAfford)
            {
                // Smooth scale down animation
                var tween = CreateTween();
                tween.TweenProperty(this, "scale", new Vector2(1.0f, 1.0f), 0.15f).SetTrans(Tween.TransitionType.Quad).SetEase(Tween.EaseType.Out);
                tween.Parallel().TweenProperty(_background, "color", new Color(0.15f, 0.2f, 0.28f, 0.95f), 0.15f);
            }
        }

        private void CreateCardUI()
        {
            // Outer glow/border for selection
            _selectionBorder = new ColorRect();
            _selectionBorder.Size = new Vector2(143, 153);
            _selectionBorder.Position = new Vector2(-4, -4);
            _selectionBorder.Color = new Color(0.2f, 0.8f, 1.0f, 1.0f);
            _selectionBorder.Visible = false;
            _selectionBorder.MouseFilter = MouseFilterEnum.Ignore;
            AddChild(_selectionBorder);

            // Gradient background panel
            var bgPanel = new Panel();
            bgPanel.Size = CustomMinimumSize;
            bgPanel.Position = Vector2.Zero;
            bgPanel.MouseFilter = MouseFilterEnum.Ignore;
            AddChild(bgPanel);

            _background = new ColorRect();
            _background.Size = CustomMinimumSize;
            _background.Color = new Color(0.15f, 0.2f, 0.28f, 0.95f);
            _background.MouseFilter = MouseFilterEnum.Ignore;
            bgPanel.AddChild(_background);

            // Top accent bar
            var accentBar = new ColorRect();
            accentBar.Size = new Vector2(135, 4);
            accentBar.Position = Vector2.Zero;
            accentBar.Color = new Color(0.3f, 0.5f, 0.8f);
            accentBar.MouseFilter = MouseFilterEnum.Ignore;
            _background.AddChild(accentBar);

            var container = new VBoxContainer();
            container.Position = new Vector2(10, 8);
            container.CustomMinimumSize = new Vector2(115, 129);
            container.AddThemeConstantOverride("separation", 4);
            container.MouseFilter = MouseFilterEnum.Ignore;
            AddChild(container);

            // Icon container with circular background
            var iconContainer = new CenterContainer();
            iconContainer.CustomMinimumSize = new Vector2(115, 60);
            iconContainer.MouseFilter = MouseFilterEnum.Ignore;
            container.AddChild(iconContainer);

            var iconBg = new Panel();
            iconBg.CustomMinimumSize = new Vector2(56, 56);
            iconBg.MouseFilter = MouseFilterEnum.Ignore;
            iconContainer.AddChild(iconBg);

            var iconBgColor = new ColorRect();
            iconBgColor.Size = new Vector2(56, 56);
            iconBgColor.Color = new Color(0.2f, 0.3f, 0.4f, 0.6f);
            iconBgColor.MouseFilter = MouseFilterEnum.Ignore;
            iconBg.AddChild(iconBgColor);

            _iconLabel = new Label();
            _iconLabel.Text = Icon ?? "🗼";
            _iconLabel.HorizontalAlignment = HorizontalAlignment.Center;
            _iconLabel.VerticalAlignment = VerticalAlignment.Center;
            _iconLabel.Position = new Vector2(0, 0);
            _iconLabel.Size = new Vector2(56, 56);
            _iconLabel.AddThemeFontSizeOverride("font_size", 40);
            _iconLabel.MouseFilter = MouseFilterEnum.Ignore;
            iconBg.AddChild(_iconLabel);

            // Name
            _nameLabel = new Label();
            _nameLabel.Text = TowerName ?? "Tower";
            _nameLabel.HorizontalAlignment = HorizontalAlignment.Center;
            _nameLabel.AddThemeColorOverride("font_color", new Color(0.9f, 0.9f, 1.0f));
            _nameLabel.AddThemeFontSizeOverride("font_size", 13);
            _nameLabel.AutowrapMode = TextServer.AutowrapMode.WordSmart;
            _nameLabel.CustomMinimumSize = new Vector2(115, 0);
            _nameLabel.MouseFilter = MouseFilterEnum.Ignore;
            container.AddChild(_nameLabel);

            // Cost container with styled background
            var costBg = new Panel();
            costBg.CustomMinimumSize = new Vector2(80, 32);
            costBg.MouseFilter = MouseFilterEnum.Ignore;
            container.AddChild(costBg);

            var costBgColor = new ColorRect();
            costBgColor.Size = new Vector2(80, 32);
            costBgColor.Color = new Color(0.1f, 0.15f, 0.2f, 0.8f);
            costBgColor.MouseFilter = MouseFilterEnum.Ignore;
            costBg.AddChild(costBgColor);

            var costContainer = new HBoxContainer();
            costContainer.Position = new Vector2(8, 6);
            costContainer.AddThemeConstantOverride("separation", 4);
            costContainer.MouseFilter = MouseFilterEnum.Ignore;
            costBg.AddChild(costContainer);

            var goldIcon = new Label();
            goldIcon.Text = "💰";
            goldIcon.AddThemeFontSizeOverride("font_size", 16);
            goldIcon.MouseFilter = MouseFilterEnum.Ignore;
            costContainer.AddChild(goldIcon);

            _costLabel = new Label();
            _costLabel.Text = Cost.ToString();
            _costLabel.AddThemeColorOverride("font_color", new Color(1.0f, 0.84f, 0.0f));
            _costLabel.AddThemeFontSizeOverride("font_size", 16);
            _costLabel.MouseFilter = MouseFilterEnum.Ignore;
            costContainer.AddChild(_costLabel);
        }

        public void UpdateAffordability(bool canAfford)
        {
            _canAfford = canAfford;

            if (canAfford)
            {
                // Full color - can afford
                Modulate = Colors.White;
                _background.Color = new Color(0.15f, 0.2f, 0.28f, 0.95f);
            }
            else
            {
                // Greyed out - cannot afford
                Modulate = new Color(0.5f, 0.5f, 0.5f);
                _background.Color = new Color(0.1f, 0.12f, 0.15f, 0.95f);
            }
        }

        private void OnGuiInput(InputEvent @event)
        {
            // Support both mouse (desktop) and touch (mobile)
            bool clicked = false;

            if (@event is InputEventMouseButton mouseButton)
            {
                clicked = mouseButton.Pressed && mouseButton.ButtonIndex == MouseButton.Left;
            }
            else if (@event is InputEventScreenTouch touch)
            {
                clicked = touch.Pressed;
            }

            if (clicked && _canAfford)
            {
                GD.Print($"Tower card clicked: {TowerName}");
                EmitSignal(SignalName.CardClicked, this);
            }
        }

        public void SetSelected(bool selected)
        {
            _isSelected = selected;

            if (selected)
            {
                // Show bright border around selected card
                if (_selectionBorder != null)
                {
                    _selectionBorder.Visible = true;
                    _selectionBorder.Color = new Color(0.2f, 0.8f, 1.0f, 1.0f);
                }

                // Brighten the card with blue tint
                Modulate = new Color(1.15f, 1.15f, 1.3f);
                _background.Color = new Color(0.25f, 0.35f, 0.5f, 0.95f);
            }
            else
            {
                // Hide border
                if (_selectionBorder != null)
                {
                    _selectionBorder.Visible = false;
                }

                // Dim non-selected cards when there's a selection
                if (_canAfford)
                {
                    Modulate = new Color(0.7f, 0.7f, 0.7f);
                    _background.Color = new Color(0.15f, 0.2f, 0.28f, 0.95f);
                }
            }
        }

        public void ResetVisuals()
        {
            _isSelected = false;
            if (_selectionBorder != null)
            {
                _selectionBorder.Visible = false;
            }

            // Reset to normal affordability state
            UpdateAffordability(_canAfford);
        }
    }
}
