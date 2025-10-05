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
		private ColorRect _border;
		private ColorRect _shadow;
		private ColorRect _innerGlow;
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
			_clickArea.MouseEntered += OnMouseEntered;
			_clickArea.MouseExited += OnMouseExited;
		}

		private void OnMouseEntered()
		{
			if (!IsUnlocked || HasTower) return;
			if (!HasMeta("glow_poly")) return;

			var glowPoly = GetMeta("glow_poly").As<Polygon2D>();
			var mainPlatform = GetMeta("main_platform").As<Polygon2D>();

			// Highlight on hover
			var tween = CreateTween();
			tween.TweenProperty(glowPoly, "color", new Color(0.4f, 0.9f, 1.0f, 0.5f), 0.2f);
			tween.Parallel().TweenProperty(mainPlatform, "modulate", new Color(1.2f, 1.2f, 1.2f), 0.2f);
		}

		private void OnMouseExited()
		{
			if (!IsUnlocked || HasTower) return;
			if (!HasMeta("glow_poly")) return;

			var glowPoly = GetMeta("glow_poly").As<Polygon2D>();
			var mainPlatform = GetMeta("main_platform").As<Polygon2D>();

			// Reset on exit
			var tween = CreateTween();
			tween.TweenProperty(glowPoly, "color", new Color(0.2f, 0.7f, 1.0f, 0.25f), 0.2f);
			tween.Parallel().TweenProperty(mainPlatform, "modulate", Colors.White, 0.2f);
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
			// Create hexagonal turret mounting point
			CreateHexagonalPlatform();
		}

		private void CreateHexagonalPlatform()
		{
			// Shadow layer (bottom-most)
			var shadowPoly = new Polygon2D();
			shadowPoly.Color = new Color(0f, 0f, 0f, 0.5f);
			shadowPoly.ZIndex = -3;
			shadowPoly.Position = new Vector2(2, 2); // Offset for shadow effect
			shadowPoly.Polygon = CreateHexagonPoints(38f);
			AddChild(shadowPoly);

			// Outer platform (metallic rim)
			var outerPlatform = new Polygon2D();
			outerPlatform.ZIndex = -2;

			if (IsUnlocked)
			{
				outerPlatform.Color = new Color(0.12f, 0.15f, 0.18f, 0.7f); // Much darker metallic rim
			}
			else
			{
				outerPlatform.Color = new Color(0.08f, 0.08f, 0.1f, 0.6f); // Very dark locked
			}

			outerPlatform.Polygon = CreateHexagonPoints(36f);
			AddChild(outerPlatform);
			_border = new ColorRect(); // Dummy for compatibility
			_border.Visible = false;
			AddChild(_border);

			// Main platform surface
			var mainPlatform = new Polygon2D();
			mainPlatform.ZIndex = -1;

			if (IsUnlocked)
			{
				mainPlatform.Color = new Color(0.08f, 0.1f, 0.13f, 0.8f); // Very dark tech surface
			}
			else
			{
				mainPlatform.Color = new Color(0.06f, 0.06f, 0.08f, 0.7f); // Almost black when locked
			}

			mainPlatform.Polygon = CreateHexagonPoints(30f);
			AddChild(mainPlatform);

			// Holographic grid lines
			CreateHolographicGrid();

			// Energy glow effect (stored as _innerGlow for compatibility)
			var glowPoly = new Polygon2D();
			glowPoly.ZIndex = 0;

			if (IsUnlocked)
			{
				glowPoly.Color = new Color(0.2f, 0.7f, 1.0f, 0.25f); // Subtle cyan holographic glow
			}
			else
			{
				glowPoly.Color = new Color(0.8f, 0.4f, 0.1f, 0.15f); // Subtle orange locked indicator
			}

			glowPoly.Polygon = CreateHexagonPoints(28f);
			AddChild(glowPoly);

			// Store references (use dummy ColorRect for compatibility with existing code)
			_visualIndicator = new ColorRect();
			_visualIndicator.Visible = false;
			AddChild(_visualIndicator);

			_innerGlow = new ColorRect();
			_innerGlow.Visible = false;
			AddChild(_innerGlow);

			_shadow = new ColorRect();
			_shadow.Visible = false;
			AddChild(_shadow);

			// Store polygon references in metadata for later updates
			SetMeta("shadow_poly", shadowPoly);
			SetMeta("outer_platform", outerPlatform);
			SetMeta("main_platform", mainPlatform);
			SetMeta("glow_poly", glowPoly);
		}

		private Vector2[] CreateHexagonPoints(float radius)
		{
			var points = new Vector2[6];
			for (int i = 0; i < 6; i++)
			{
				float angle = Mathf.DegToRad(60 * i - 30); // -30 to make flat-top hexagon
				points[i] = new Vector2(
					Mathf.Cos(angle) * radius,
					Mathf.Sin(angle) * radius
				);
			}
			return points;
		}

		private void CreateHolographicGrid()
		{
			if (!IsUnlocked) return;

			// Create crosshair/targeting lines
			var line1 = new Line2D();
			line1.DefaultColor = new Color(0.3f, 0.8f, 1.0f, 0.4f);
			line1.Width = 1f;
			line1.ZIndex = 1;
			line1.AddPoint(new Vector2(-20, 0));
			line1.AddPoint(new Vector2(20, 0));
			AddChild(line1);

			var line2 = new Line2D();
			line2.DefaultColor = new Color(0.3f, 0.8f, 1.0f, 0.4f);
			line2.Width = 1f;
			line2.ZIndex = 1;
			line2.AddPoint(new Vector2(0, -20));
			line2.AddPoint(new Vector2(0, 20));
			AddChild(line2);

			// Corner brackets for tech look
			CreateCornerBrackets();
		}

		private void CreateCornerBrackets()
		{
			var bracketColor = new Color(0.3f, 0.8f, 1.0f, 0.5f);
			float size = 8f;
			float offset = 18f;

			// Top-left
			CreateBracket(new Vector2(-offset, -offset), bracketColor, size, true, true);
			// Top-right
			CreateBracket(new Vector2(offset, -offset), bracketColor, size, false, true);
			// Bottom-left
			CreateBracket(new Vector2(-offset, offset), bracketColor, size, true, false);
			// Bottom-right
			CreateBracket(new Vector2(offset, offset), bracketColor, size, false, false);
		}

		private void CreateBracket(Vector2 pos, Color color, float size, bool left, bool top)
		{
			var bracket = new Line2D();
			bracket.DefaultColor = color;
			bracket.Width = 1.5f;
			bracket.ZIndex = 1;

			if (left && top)
			{
				bracket.AddPoint(pos + new Vector2(size, 0));
				bracket.AddPoint(pos);
				bracket.AddPoint(pos + new Vector2(0, size));
			}
			else if (!left && top)
			{
				bracket.AddPoint(pos + new Vector2(-size, 0));
				bracket.AddPoint(pos);
				bracket.AddPoint(pos + new Vector2(0, size));
			}
			else if (left && !top)
			{
				bracket.AddPoint(pos + new Vector2(size, 0));
				bracket.AddPoint(pos);
				bracket.AddPoint(pos + new Vector2(0, -size));
			}
			else
			{
				bracket.AddPoint(pos + new Vector2(-size, 0));
				bracket.AddPoint(pos);
				bracket.AddPoint(pos + new Vector2(0, -size));
			}

			AddChild(bracket);
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
			UpdatePlatformColors();
		}

		public void Lock()
		{
			IsUnlocked = false;
			UpdatePlatformColors();
		}

		private void UpdatePlatformColors()
		{
			if (!HasMeta("outer_platform")) return;

			var outerPlatform = GetMeta("outer_platform").As<Polygon2D>();
			var mainPlatform = GetMeta("main_platform").As<Polygon2D>();
			var glowPoly = GetMeta("glow_poly").As<Polygon2D>();

			if (IsUnlocked)
			{
				outerPlatform.Color = new Color(0.12f, 0.15f, 0.18f, 0.7f);
				mainPlatform.Color = new Color(0.08f, 0.1f, 0.13f, 0.8f);
				glowPoly.Color = new Color(0.2f, 0.7f, 1.0f, 0.25f);
			}
			else
			{
				outerPlatform.Color = new Color(0.08f, 0.08f, 0.1f, 0.6f);
				mainPlatform.Color = new Color(0.06f, 0.06f, 0.08f, 0.7f);
				glowPoly.Color = new Color(0.8f, 0.4f, 0.1f, 0.15f);
			}
		}

		public void SetPlacementHighlight(bool active)
		{
			if (!IsUnlocked || HasTower) return;
			if (!HasMeta("glow_poly")) return;

			var glowPoly = GetMeta("glow_poly").As<Polygon2D>();
			var outerPlatform = GetMeta("outer_platform").As<Polygon2D>();

			if (active)
			{
				// Bright cyan glow when ready for placement
				glowPoly.Color = new Color(0.3f, 1.0f, 0.5f, 0.6f); // Bright green-cyan
				outerPlatform.Color = new Color(0.3f, 0.6f, 0.5f, 1.0f); // Highlight rim

				// Add pulsing animation
				var tween = CreateTween();
				tween.SetLoops();
				tween.TweenProperty(glowPoly, "modulate:a", 0.7f, 0.8f);
				tween.TweenProperty(glowPoly, "modulate:a", 1.0f, 0.8f);
				SetMeta("placement_tween", tween);
			}
			else
			{
				// Stop pulsing
				if (HasMeta("placement_tween"))
				{
					var tween = GetMeta("placement_tween").As<Tween>();
					if (tween != null && IsInstanceValid(tween))
					{
						tween.Kill();
					}
					RemoveMeta("placement_tween");
				}

				// Normal state
				glowPoly.Color = new Color(0.2f, 0.7f, 1.0f, 0.3f);
				glowPoly.Modulate = new Color(1, 1, 1, 1);
				outerPlatform.Color = new Color(0.25f, 0.35f, 0.45f, 0.9f);
			}
		}
	}
}
