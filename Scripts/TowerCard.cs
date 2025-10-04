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
        private bool _canAfford = false;

        [Signal]
        public delegate void CardClickedEventHandler(TowerCard card);

        public override void _Ready()
        {
            CustomMinimumSize = new Vector2(140, 180);
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
                _background.Color = new Color(0.4f, 0.5f, 0.6f);
            }
        }

        private void OnMouseExited()
        {
            if (_canAfford)
            {
                _background.Color = new Color(0.3f, 0.4f, 0.5f);
            }
        }

        private void CreateCardUI()
        {
            // Background
            _background = new ColorRect();
            _background.Size = CustomMinimumSize;
            _background.Color = new Color(0.2f, 0.25f, 0.3f);
            AddChild(_background);

            var container = new VBoxContainer();
            container.Position = new Vector2(5, 5);
            container.CustomMinimumSize = new Vector2(130, 170);
            AddChild(container);

            // Icon
            _iconLabel = new Label();
            _iconLabel.Text = Icon ?? "🗼";
            _iconLabel.HorizontalAlignment = HorizontalAlignment.Center;
            _iconLabel.AddThemeFontSizeOverride("font_size", 48);
            container.AddChild(_iconLabel);

            // Name
            _nameLabel = new Label();
            _nameLabel.Text = TowerName ?? "Tower";
            _nameLabel.HorizontalAlignment = HorizontalAlignment.Center;
            _nameLabel.AddThemeColorOverride("font_color", Colors.White);
            _nameLabel.AddThemeFontSizeOverride("font_size", 18);
            container.AddChild(_nameLabel);

            var spacer = new Control();
            spacer.CustomMinimumSize = new Vector2(0, 10);
            container.AddChild(spacer);

            // Cost
            var centerControl = new CenterContainer();
            container.AddChild(centerControl);

            var costContainer = new HBoxContainer();
            costContainer.AddThemeConstantOverride("separation", 5);
            centerControl.AddChild(costContainer);

            var goldIcon = new Label();
            goldIcon.Text = "💰";
            goldIcon.AddThemeFontSizeOverride("font_size", 20);
            costContainer.AddChild(goldIcon);

            _costLabel = new Label();
            _costLabel.Text = Cost.ToString();
            _costLabel.AddThemeColorOverride("font_color", Colors.Gold);
            _costLabel.AddThemeFontSizeOverride("font_size", 20);
            costContainer.AddChild(_costLabel);
        }

        public void UpdateAffordability(bool canAfford)
        {
            _canAfford = canAfford;

            if (canAfford)
            {
                // Full color - can afford
                Modulate = Colors.White;
                _background.Color = new Color(0.3f, 0.4f, 0.5f);
            }
            else
            {
                // Greyed out - cannot afford
                Modulate = new Color(0.5f, 0.5f, 0.5f);
                _background.Color = new Color(0.15f, 0.15f, 0.2f);
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
    }
}
