using Godot;
using System;
using System.Collections.Generic;

namespace TowerSurvivors
{
    public partial class TowerShopUI : CanvasLayer
    {
        private HBoxContainer _cardsContainer;
        private List<TowerCard> _cards = new List<TowerCard>();

        [Signal]
        public delegate void TowerPurchasedEventHandler(string towerType, int cost);

        public override void _Ready()
        {
            CreateShopUI();
            CreateTowerCards();
        }

        private void CreateShopUI()
        {
            // Panel at bottom of screen
            var panel = new Panel();
            panel.SetAnchorsPreset(Control.LayoutPreset.BottomWide);
            panel.OffsetTop = -200;
            AddChild(panel);

            // Background
            var background = new ColorRect();
            background.SetAnchorsPreset(Control.LayoutPreset.FullRect);
            background.Color = new Color(0.1f, 0.1f, 0.15f, 0.9f);
            panel.AddChild(background);

            // Cards container
            _cardsContainer = new HBoxContainer();
            _cardsContainer.Position = new Vector2(20, 10);
            _cardsContainer.AddThemeConstantOverride("separation", 15);
            panel.AddChild(_cardsContainer);
        }

        private void CreateTowerCards()
        {
            // Arrow Tower card
            CreateCard("Arrow", "Arrow Tower", 50, "🏹");

            // Cannon Tower card (coming soon)
            CreateCard("Cannon", "Cannon Tower", 75, "💣");

            // Laser Tower card (coming soon)
            CreateCard("Laser", "Laser Tower", 100, "⚡");

            // Ice Tower card (coming soon)
            CreateCard("Ice", "Ice Tower", 80, "❄️");

            // Lightning Tower card (coming soon)
            CreateCard("Lightning", "Lightning Tower", 120, "⚡");
        }

        private void CreateCard(string type, string name, int cost, string icon)
        {
            var card = new TowerCard();
            card.TowerType = type;
            card.TowerName = name;
            card.Cost = cost;
            card.Icon = icon;

            _cardsContainer.AddChild(card);
            _cards.Add(card);

            card.CardClicked += OnCardClicked;
        }

        private void OnCardClicked(TowerCard card)
        {
            EmitSignal(SignalName.TowerPurchased, card.TowerType, card.Cost);
        }

        public void UpdateAffordability(int currentGold)
        {
            foreach (var card in _cards)
            {
                card.UpdateAffordability(currentGold >= card.Cost);
            }
        }
    }
}
