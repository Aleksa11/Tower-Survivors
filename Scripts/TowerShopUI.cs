using Godot;
using System;
using System.Collections.Generic;

namespace TowerSurvivors
{
    public partial class TowerShopUI : CanvasLayer
    {
        private HBoxContainer _cardsContainer;
        private List<TowerCard> _cards = new List<TowerCard>();
        private TowerCard _selectedCard = null;

        [Signal]
        public delegate void TowerPurchasedEventHandler(string towerType, int cost);

        [Signal]
        public delegate void SelectionCancelledEventHandler();

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
            panel.OffsetTop = -160;
            AddChild(panel);

            // Background with gradient effect - darker and more premium
            var background = new ColorRect();
            background.SetAnchorsPreset(Control.LayoutPreset.FullRect);
            background.Color = new Color(0.05f, 0.06f, 0.08f, 0.98f);
            panel.AddChild(background);

            // Top border accent with glow effect
            var topBorder = new ColorRect();
            topBorder.SetAnchorsPreset(Control.LayoutPreset.TopWide);
            topBorder.CustomMinimumSize = new Vector2(0, 4);
            topBorder.Color = new Color(0.4f, 0.7f, 1.0f, 0.9f);
            panel.AddChild(topBorder);

            // Add subtle top glow
            var topGlow = new ColorRect();
            topGlow.SetAnchorsPreset(Control.LayoutPreset.TopWide);
            topGlow.Position = new Vector2(0, 4);
            topGlow.CustomMinimumSize = new Vector2(0, 20);
            topGlow.Color = new Color(0.2f, 0.4f, 0.6f, 0.15f);
            panel.AddChild(topGlow);

            // Container to center and distribute cards
            var centerContainer = new CenterContainer();
            centerContainer.SetAnchorsPreset(Control.LayoutPreset.FullRect);
            centerContainer.OffsetLeft = 5;
            centerContainer.OffsetTop = 5;
            centerContainer.OffsetRight = -5;
            centerContainer.OffsetBottom = -5;
            panel.AddChild(centerContainer);

            // Cards container with alignment
            _cardsContainer = new HBoxContainer();
            _cardsContainer.Alignment = BoxContainer.AlignmentMode.Center;
            _cardsContainer.AddThemeConstantOverride("separation", 4);
            centerContainer.AddChild(_cardsContainer);
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
            // If clicking the same card, cancel selection
            if (_selectedCard == card)
            {
                CancelSelection();
                return;
            }

            // Select new card
            _selectedCard = card;
            UpdateCardVisuals();

            EmitSignal(SignalName.TowerPurchased, card.TowerType, card.Cost);
        }

        public void CancelSelection()
        {
            _selectedCard = null;
            UpdateCardVisuals();
            EmitSignal(SignalName.SelectionCancelled);
        }

        private void UpdateCardVisuals()
        {
            if (_selectedCard == null)
            {
                // No selection - reset all cards to normal
                foreach (var card in _cards)
                {
                    card.ResetVisuals();
                }
            }
            else
            {
                // Something selected - highlight selected, dim others
                foreach (var card in _cards)
                {
                    if (card == _selectedCard)
                    {
                        card.SetSelected(true);
                    }
                    else
                    {
                        card.SetSelected(false);
                    }
                }
            }
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
