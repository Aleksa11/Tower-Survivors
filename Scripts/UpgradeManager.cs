using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

namespace TowerSurvivors
{
    public partial class UpgradeManager : Node
    {
        [Export] public int XPPerLevel { get; set; } = 100;
        [Export] public int XPIncreasePerLevel { get; set; } = 20;

        private int _currentXP = 0;
        private int _xpToNextLevel = 100;
        private int _currentLevel = 0;
        private List<Upgrade> _availableUpgrades = new List<Upgrade>();
        private bool _isPaused = false;

        [Signal]
        public delegate void UpgradeReadyEventHandler(Upgrade upgrade1, Upgrade upgrade2, Upgrade upgrade3);

        public override void _Ready()
        {
            InitializeUpgrades();
            _xpToNextLevel = XPPerLevel;
        }

        private void InitializeUpgrades()
        {
            // Tower damage upgrades
            _availableUpgrades.Add(new Upgrade(
                "Sharp Arrows",
                "+25% Tower Damage",
                UpgradeType.TowerDamage,
                0.25f,
                "🗡️"
            ));

            _availableUpgrades.Add(new Upgrade(
                "Deadly Shots",
                "+50% Tower Damage",
                UpgradeType.TowerDamage,
                0.5f,
                "⚔️"
            ));

            // Attack speed upgrades
            _availableUpgrades.Add(new Upgrade(
                "Quick Draw",
                "+30% Attack Speed",
                UpgradeType.TowerAttackSpeed,
                0.3f,
                "⚡"
            ));

            _availableUpgrades.Add(new Upgrade(
                "Rapid Fire",
                "+60% Attack Speed",
                UpgradeType.TowerAttackSpeed,
                0.6f,
                "💨"
            ));

            // Range upgrades
            _availableUpgrades.Add(new Upgrade(
                "Eagle Eye",
                "+40% Tower Range",
                UpgradeType.TowerRange,
                0.4f,
                "👁️"
            ));

            _availableUpgrades.Add(new Upgrade(
                "Sniper Scope",
                "+80% Tower Range",
                UpgradeType.TowerRange,
                0.8f,
                "🔭"
            ));

            // Slot unlocks
            _availableUpgrades.Add(new Upgrade(
                "Expand Grid",
                "Unlock 2 Random Slots",
                UpgradeType.UnlockSlot,
                2f,
                "🔓"
            ));

            // Core upgrades
            _availableUpgrades.Add(new Upgrade(
                "Fortify Core",
                "+50 Max HP",
                UpgradeType.CoreHealth,
                50f,
                "❤️"
            ));

            _availableUpgrades.Add(new Upgrade(
                "Regeneration",
                "Heal 5 HP per second",
                UpgradeType.CoreRegen,
                5f,
                "💚"
            ));

            // Special abilities
            _availableUpgrades.Add(new Upgrade(
                "Piercing Shots",
                "Projectiles pierce 1 enemy",
                UpgradeType.ProjectilePierce,
                1f,
                "🎯"
            ));

            _availableUpgrades.Add(new Upgrade(
                "Explosive Rounds",
                "Projectiles explode on impact",
                UpgradeType.ProjectileExplosion,
                1f,
                "💥"
            ));

            _availableUpgrades.Add(new Upgrade(
                "Gold Rush",
                "+50% Gold from enemies",
                UpgradeType.GoldMultiplier,
                0.5f,
                "💰"
            ));

            // Tower-specific upgrades
            _availableUpgrades.Add(new Upgrade(
                "Chain Lightning+",
                "+1 Chain Target (Lightning Tower)",
                UpgradeType.LightningChainCount,
                1f,
                "⚡",
                "Lightning"
            ));

            _availableUpgrades.Add(new Upgrade(
                "Bigger Boom",
                "+30% Explosion Radius (Cannon Tower)",
                UpgradeType.CannonExplosionRadius,
                0.3f,
                "💥",
                "Cannon"
            ));

            _availableUpgrades.Add(new Upgrade(
                "Frost Nova",
                "+2 Projectiles (Ice Tower)",
                UpgradeType.IceMultiShot,
                2f,
                "❄️",
                "Ice"
            ));

            _availableUpgrades.Add(new Upgrade(
                "Laser Barrage",
                "Multi-shot: Fire 2 extra beams (Laser Tower)",
                UpgradeType.LaserMultiShot,
                2f,
                "🔴",
                "Laser"
            ));
        }

        public void AddXP(int xp)
        {
            _currentXP += xp;

            if (_currentXP >= _xpToNextLevel)
            {
                LevelUp();
            }
        }

        private void LevelUp()
        {
            _currentLevel++;
            _currentXP -= _xpToNextLevel;
            _xpToNextLevel += XPIncreasePerLevel;

            GD.Print($"LEVEL UP! Now level {_currentLevel}. Next level at {_xpToNextLevel} XP");

            ShowUpgradeChoices();
        }

        private void ShowUpgradeChoices()
        {
            // Get placed tower types and counts
            var placedTowerTypes = GetPlacedTowerTypes();
            var towerCounts = GetTowerCounts();

            // Filter available upgrades - tower-specific upgrades need 3+ towers
            var validUpgrades = _availableUpgrades.Where(upgrade =>
            {
                if (upgrade.RequiredTowerType == null)
                    return true; // General upgrades always available

                // Tower-specific upgrades need 3+ of that tower type
                return placedTowerTypes.Contains(upgrade.RequiredTowerType) &&
                       towerCounts.GetValueOrDefault(upgrade.RequiredTowerType, 0) >= 3;
            }).ToList();

            // Pick 3 random upgrades - weight tower-specific upgrades higher
            var random = new Random();

            // Add tower-specific upgrades multiple times to increase chance
            var weightedUpgrades = new List<Upgrade>();
            foreach (var upgrade in validUpgrades)
            {
                if (upgrade.RequiredTowerType != null)
                {
                    // Add tower-specific 3 times (3x more likely)
                    weightedUpgrades.Add(upgrade);
                    weightedUpgrades.Add(upgrade);
                    weightedUpgrades.Add(upgrade);
                }
                else
                {
                    weightedUpgrades.Add(upgrade);
                }
            }

            var shuffled = weightedUpgrades.OrderBy(x => random.Next()).ToList();

            var upgrade1 = shuffled[0];
            var upgrade2 = shuffled[1];
            var upgrade3 = shuffled[2];

            EmitSignal(SignalName.UpgradeReady, upgrade1, upgrade2, upgrade3);
            PauseGame();
        }

        private HashSet<string> GetPlacedTowerTypes()
        {
            var towerTypes = new HashSet<string>();

            // Get TowerGrid from main scene
            var mainScene = GetParent();
            var towerGrid = mainScene.GetNode<TowerPlacementGrid>("TowerGrid");

            if (towerGrid != null)
            {
                foreach (var slot in towerGrid.GetAllSlots())
                {
                    if (slot.HasTower && slot.CurrentTower != null)
                    {
                        var tower = slot.CurrentTower;
                        if (tower is LightningTower)
                            towerTypes.Add("Lightning");
                        else if (tower is CannonTower)
                            towerTypes.Add("Cannon");
                        else if (tower is IceTower)
                            towerTypes.Add("Ice");
                        else if (tower is LaserTower)
                            towerTypes.Add("Laser");
                    }
                }
            }

            return towerTypes;
        }

        private Dictionary<string, int> GetTowerCounts()
        {
            var counts = new Dictionary<string, int>();

            // Get TowerGrid from main scene
            var mainScene = GetParent();
            var towerGrid = mainScene.GetNode<TowerPlacementGrid>("TowerGrid");

            if (towerGrid != null)
            {
                foreach (var slot in towerGrid.GetAllSlots())
                {
                    if (slot.HasTower && slot.CurrentTower != null)
                    {
                        var tower = slot.CurrentTower;
                        string towerType = null;

                        if (tower is LightningTower)
                            towerType = "Lightning";
                        else if (tower is CannonTower)
                            towerType = "Cannon";
                        else if (tower is IceTower)
                            towerType = "Ice";
                        else if (tower is LaserTower)
                            towerType = "Laser";

                        if (towerType != null)
                        {
                            if (!counts.ContainsKey(towerType))
                                counts[towerType] = 0;
                            counts[towerType]++;
                        }
                    }
                }
            }

            return counts;
        }

        public void PauseGame()
        {
            _isPaused = true;
            GetTree().Paused = true;
        }

        public void ResumeGame()
        {
            _isPaused = false;
            GetTree().Paused = false;
        }

        public int GetCurrentXP()
        {
            return _currentXP;
        }

        public int GetXPToNextLevel()
        {
            return _xpToNextLevel;
        }

        public int GetCurrentLevel()
        {
            return _currentLevel;
        }

        public float GetUpgradeProgress()
        {
            // Returns 0.0 to 1.0 (percentage complete)
            return (float)_currentXP / _xpToNextLevel;
        }
    }
}
