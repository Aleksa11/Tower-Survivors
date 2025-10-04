# Tower Survivors: Endless Siege

A fast-paced mobile roguelite tower defense game where you defend a central core from waves of enemies by placing towers in a circular formation.

## Phase 1 - MVP Complete! ✓

### What's Implemented:
- ✅ Player core with HP system in the center
- ✅ 12-slot circular tower placement grid (3 slots unlocked initially)
- ✅ Arrow Tower with auto-targeting and projectile shooting
- ✅ Enemy system that moves toward the center
- ✅ Wave spawning system (20 waves total)
- ✅ Win/lose conditions
- ✅ Game UI (HP, Wave counter, Gold display)
- ✅ Game Over and Victory screens

### How to Play:
1. Tap on green slots to place Arrow Towers (costs 50 gold)
2. Towers automatically shoot at nearby enemies
3. Enemies spawn in waves and move toward your core
4. Survive 20 waves to win!
5. Game over if your core reaches 0 HP

### Controls:
- **Tap green slots**: Place tower
- **Restart button**: Restart game after win/loss

## Setup Instructions:

### Prerequisites:
- Godot 4.3 or later with .NET support
- .NET 6.0 SDK or later

### Steps:
1. Open Godot 4.3
2. Click "Import" and select this project folder
3. Godot will automatically detect the C# project
4. Build the project (Build → Build Project)
5. Run the game (F5)

### For Mobile Testing:
1. Go to Project → Export
2. Add Android/iOS export template
3. Configure export settings
4. Export and test on device

## Game Balance (Phase 1):

### Player Core:
- Starting HP: 100
- Takes damage when enemies reach center

### Arrow Tower:
- Cost: 50 gold
- Damage: 15
- Attack Speed: 2 attacks/second
- Range: 250 pixels

### Enemies:
- Wave 1: 5 enemies, 50 HP, 80 speed
- Scales with each wave (+3 enemies, +10 HP, +5 speed, +2 damage)
- Gold reward: 5 per kill
- Wave clear bonus: 25 gold

### Starting Resources:
- Gold: 100 (enough for 2 towers)
- Unlocked slots: 3

## Next Steps (Phase 2):
- [ ] Upgrade system (choose 1 of 3 upgrades every 30 seconds)
- [ ] 4 more tower types (Cannon, Laser, Ice, Lightning)
- [ ] Meta-progression with gems
- [ ] Permanent unlocks between runs
- [ ] Save/load system

## Next Steps (Phase 3):
- [ ] Particle effects and screen shake
- [ ] Sound effects and music
- [ ] Boss enemies
- [ ] 20+ total upgrades
- [ ] Difficulty levels

## Known Issues:
- UI is functional but basic (will be improved in Phase 3)
- No sound effects yet
- Visual effects are placeholder colors

## File Structure:
```
/Scripts
  ├── PlayerCore.cs          - Player health and core logic
  ├── TowerPlacementGrid.cs  - Manages circular tower slots
  ├── TowerSlot.cs           - Individual slot logic
  ├── Tower.cs               - Base tower class
  ├── ArrowTower.cs          - Arrow tower implementation
  ├── Projectile.cs          - Base projectile class
  ├── ArrowProjectile.cs     - Arrow projectile
  ├── Enemy.cs               - Enemy AI and movement
  ├── WaveManager.cs         - Wave spawning system
  ├── GameManager.cs         - Main game logic coordinator
  └── GameUI.cs              - UI display and updates

/Scenes
  └── Main.tscn              - Main game scene
```

## Credits:
Built with Godot Engine 4.3 and C#
