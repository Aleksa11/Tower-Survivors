using Godot;
using System;

namespace TowerSurvivors
{
    public enum UpgradeType
    {
        TowerDamage,
        TowerAttackSpeed,
        TowerRange,
        UnlockSlot,
        NewTowerType,
        CoreHealth,
        CoreRegen,
        ProjectilePierce,
        ProjectileExplosion,
        GoldMultiplier,

        // Tower-specific upgrades
        LightningChainCount,
        CannonExplosionRadius,
        IceMultiShot,
        LaserMultiShot
    }

    public partial class Upgrade : RefCounted
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public UpgradeType Type { get; set; }
        public float Value { get; set; }
        public string Icon { get; set; }
        public string RequiredTowerType { get; set; } = null; // null = always available

        public Upgrade(string name, string description, UpgradeType type, float value, string icon = "⚡", string requiredTowerType = null)
        {
            Name = name;
            Description = description;
            Type = type;
            Value = value;
            Icon = icon;
            RequiredTowerType = requiredTowerType;
        }
    }
}
