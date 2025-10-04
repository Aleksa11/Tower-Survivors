using Godot;
using System;
using System.Collections.Generic;

namespace TowerSurvivors
{
    public partial class TowerPlacementGrid : Node2D
    {
        [Export] public int TotalSlots { get; set; } = 26;
        [Export] public float CellSize { get; set; } = 80f;
        [Export] public int InitialUnlockedSlots { get; set; } = 26;

        private List<TowerSlot> _slots = new List<TowerSlot>();

        [Signal]
        public delegate void SlotSelectedEventHandler(TowerSlot slot);

        public override void _Ready()
        {
            CreateDiamondGrid();
        }

        private void CreateDiamondGrid()
        {
            // Diamond layout matching the image - 26 slots total
            // Layout (x, y offsets from center):
            List<Vector2> positions = new List<Vector2>
            {
                // Top row (1 slot)
                new Vector2(0, -3),

                // Second row (3 slots)
                new Vector2(-1, -2), new Vector2(0, -2), new Vector2(1, -2),

                // Third row (5 slots)
                new Vector2(-2, -1), new Vector2(-1, -1), new Vector2(0, -1), new Vector2(1, -1), new Vector2(2, -1),

                // Middle row (7 slots)
                new Vector2(-3, 0), new Vector2(-2, 0), new Vector2(-1, 0), new Vector2(0, 0), new Vector2(1, 0), new Vector2(2, 0), new Vector2(3, 0),

                // Fifth row (5 slots)
                new Vector2(-2, 1), new Vector2(-1, 1), new Vector2(0, 1), new Vector2(1, 1), new Vector2(2, 1),

                // Sixth row (3 slots)
                new Vector2(-1, 2), new Vector2(0, 2), new Vector2(1, 2),

                // Bottom row (1 slot)
                new Vector2(0, 3)
            };

            for (int i = 0; i < positions.Count; i++)
            {
                Vector2 gridPos = positions[i];
                Vector2 slotPosition = new Vector2(gridPos.X * CellSize, gridPos.Y * CellSize);

                var slot = new TowerSlot();
                slot.SlotIndex = i;
                slot.Position = slotPosition;
                slot.IsUnlocked = i < InitialUnlockedSlots;

                AddChild(slot);
                _slots.Add(slot);

                slot.SlotClicked += OnSlotClicked;
            }
        }

        private void OnSlotClicked(TowerSlot slot)
        {
            GD.Print($"Grid received slot click: {slot.SlotIndex}, Unlocked: {slot.IsUnlocked}, HasTower: {slot.HasTower}");

            // Always emit signal, let GameManager decide if it's valid
            EmitSignal(SignalName.SlotSelected, slot);
        }

        public void UnlockSlot(int index)
        {
            if (index >= 0 && index < _slots.Count)
            {
                _slots[index].Unlock();
            }
        }

        public void UnlockNextSlot()
        {
            foreach (var slot in _slots)
            {
                if (!slot.IsUnlocked)
                {
                    slot.Unlock();
                    return;
                }
            }
        }

        public TowerSlot GetSlot(int index)
        {
            if (index >= 0 && index < _slots.Count)
            {
                return _slots[index];
            }
            return null;
        }

        public List<TowerSlot> GetAllSlots()
        {
            return new List<TowerSlot>(_slots);
        }

        public int GetUnlockedSlotCount()
        {
            int count = 0;
            foreach (var slot in _slots)
            {
                if (slot.IsUnlocked) count++;
            }
            return count;
        }
    }
}
