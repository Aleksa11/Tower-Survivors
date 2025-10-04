using Godot;
using System;

namespace TowerSurvivors
{
	public partial class TowerSlot : Node2D
	{
		[Export] public bool IsUnlocked { get; set; } = false;
		[Export] public int SlotIndex { get; set; }

		public Tower CurrentTower { get; private set; }
		public bool HasTower => CurrentTower != null;

		[Signal]
		public delegate void SlotClickedEventHandler(TowerSlot slot);

		private Area2D _clickArea;
		private ColorRect _visualIndicator;
		private CollisionShape2D _collisionShape;
		private const float ClickRadius = 40f;

		public override void _Ready()
		{
			SetupClickDetection();
			SetupVisuals();
		}

		private void SetupClickDetection()
		{
			_clickArea = new Area2D();
			_clickArea.Name = "ClickArea";
			_clickArea.InputPickable = true;
			AddChild(_clickArea);

			_collisionShape = new CollisionShape2D();
			var shape = new CircleShape2D();
			shape.Radius = ClickRadius;
			_collisionShape.Shape = shape;
			_clickArea.AddChild(_collisionShape);

			_clickArea.InputEvent += OnInputEvent;
		}

		public override void _Input(InputEvent @event)
		{
			// Direct input handling for desktop testing
			bool clicked = false;
			Vector2 clickPos = Vector2.Zero;

			if (@event is InputEventMouseButton mouseButton)
			{
				clicked = mouseButton.Pressed && mouseButton.ButtonIndex == MouseButton.Left;
				clickPos = mouseButton.Position;
			}
			else if (@event is InputEventScreenTouch touch)
			{
				clicked = touch.Pressed;
				clickPos = touch.Position;
			}

			if (clicked)
			{
				// Convert screen position to local position
				Vector2 localClick = GetGlobalTransformWithCanvas().AffineInverse() * clickPos;
				float distance = localClick.Length();

				if (distance <= ClickRadius)
				{
					GD.Print($"Slot {SlotIndex} clicked via _Input! Unlocked: {IsUnlocked}, HasTower: {HasTower}");
					EmitSignal(SignalName.SlotClicked, this);
					GetViewport().SetInputAsHandled();
				}
			}
		}

		private void SetupVisuals()
		{
			_visualIndicator = new ColorRect();
			_visualIndicator.Size = new Vector2(60, 60);
			_visualIndicator.Position = new Vector2(-30, -30);

			if (IsUnlocked)
			{
				_visualIndicator.Color = new Color(0.3f, 0.8f, 0.3f, 0.5f); // Green semi-transparent
			}
			else
			{
				_visualIndicator.Color = new Color(0.3f, 0.3f, 0.3f, 0.3f); // Gray semi-transparent
			}

			AddChild(_visualIndicator);

			// Add border
			var border = new ColorRect();
			border.Size = new Vector2(64, 64);
			border.Position = new Vector2(-32, -32);
			border.Color = new Color(1, 1, 1, 0.3f);
			AddChild(border);
			MoveChild(border, 0); // Put border behind indicator
		}

		private void OnInputEvent(Node viewport, InputEvent @event, long shapeIdx)
		{
			// Support both mouse (desktop) and touch (mobile)
			bool clicked = false;

			if (@event is InputEventMouseButton mouseButton)
			{
				clicked = mouseButton.Pressed && mouseButton.ButtonIndex == MouseButton.Left;
			}
			else if (@event is InputEventScreenTouch touchEvent)
			{
				clicked = touchEvent.Pressed;
			}

			if (clicked)
			{
				GD.Print($"Slot {SlotIndex} clicked! Unlocked: {IsUnlocked}, HasTower: {HasTower}");
				EmitSignal(SignalName.SlotClicked, this);
			}
		}

		public void PlaceTower(Tower tower)
		{
			if (!IsUnlocked || HasTower) return;

			CurrentTower = tower;
			AddChild(tower);
			tower.Position = Vector2.Zero;

			// Hide the indicator when tower is placed
			if (_visualIndicator != null)
			{
				_visualIndicator.Visible = false;
			}
		}

		public Tower RemoveTower()
		{
			if (!HasTower) return null;

			var tower = CurrentTower;
			RemoveChild(tower);
			CurrentTower = null;

			// Show indicator again
			if (_visualIndicator != null)
			{
				_visualIndicator.Visible = true;
			}

			return tower;
		}

		public void Unlock()
		{
			IsUnlocked = true;
			if (_visualIndicator != null)
			{
				_visualIndicator.Color = new Color(0.3f, 0.8f, 0.3f, 0.5f);
			}
		}

		public void Lock()
		{
			IsUnlocked = false;
			if (_visualIndicator != null)
			{
				_visualIndicator.Color = new Color(0.3f, 0.3f, 0.3f, 0.3f);
			}
		}

		public void SetPlacementHighlight(bool active)
		{
			if (!IsUnlocked || HasTower || _visualIndicator == null) return;

			if (active)
			{
				// Bright green when in placement mode
				_visualIndicator.Color = new Color(0.5f, 1.0f, 0.5f, 0.8f);
			}
			else
			{
				// Normal green
				_visualIndicator.Color = new Color(0.3f, 0.8f, 0.3f, 0.5f);
			}
		}
	}
}
