# Tower Survivors: Endless Siege - Development Prompt

## Project Overview
**Game Title:** Tower Survivors: Endless Siege

**Core Concept:** A fast-paced roguelite tower defense where you stand in the center of a circular arena and place towers around you. Enemies swarm from all directions. Every 30 seconds, choose 1 of 3 upgrades (Vampire Survivors style).

**Genre:** Roguelite Tower Defense Survivor

**Inspiration Games:** Vampire Survivors, Kingdom Rush, Brotato

**Platform:** Mobile (iOS/Android) - Portrait orientation preferred

---

## Gameplay Mechanics

### Core Loop
- **Primary Gameplay:**
  - Player is stationary in the CENTER of a circular arena
  - Tap empty slots around the circle to place towers
  - Towers auto-shoot at enemies
  - Enemies spawn from edges and move toward center
  - Survive waves of increasing difficulty

- **Run Duration:** 15-20 minutes per run

- **Victory Condition:** Survive 20 waves + final boss wave

- **Failure State:** Player's center core reaches 0 HP

### Tower Placement System
- **Circular Grid:** 12 slots arranged in a circle around player
- **Tower Types:**
  - Arrow Tower (fast, low damage, long range)
  - Cannon Tower (slow, high damage, medium range)
  - Laser Tower (constant beam, medium damage)
  - Ice Tower (slows enemies, low damage)
  - Lightning Tower (chains between enemies)

- **Placement Rules:**
  - Start with 3 slots unlocked
  - More slots unlock as you level up
  - Can sell/replace towers mid-run

### Progression Systems
- **In-Run Progression:**
  - Every 30 seconds: choose 1 of 3 random upgrades
  - Upgrades include:
    - New tower type unlock
    - Tower stat boosts (+damage, +range, +attack speed)
    - Special abilities (projectiles pierce, explosions, etc.)
    - Core upgrades (more HP, damage reflection)

- **Meta-Progression:**
  - Gems collected during runs
  - Permanent unlocks: new tower types, starting bonuses, handicaps for enemies
  - Unlock new "difficulties" that give more gems

### Roguelite Elements
- **Randomization:**
  - Upgrade choices are random each run
  - Enemy wave composition varies
  - Boss selection changes

- **Permadeath:** Complete - run ends, keep gems only

- **Unlockables:**
  - 15+ tower types
  - 10+ meta upgrades
  - 5 difficulty levels

---

## Technical Requirements

### Game Structure
- **Perspective:** 2D Top-down view
- **Controls:** Touch only - tap to place/interact
- **Platform Target:** Mobile (Android/iOS)
- **Orientation:** Portrait or Square (better for circular design)

### Key Systems to Implement

#### Phase 1 - Core MVP
1. **Player Core System**
   - Stationary center object with HP
   - Take damage when enemies reach center
   - Game over screen

2. **Circular Tower Placement Grid**
   - 12 positions in a circle
   - Visual slots showing available positions
   - Tap to place tower (start with 1 tower type)

3. **Basic Enemy System**
   - Enemies spawn from edge
   - Move toward center
   - Simple HP and damage

4. **Wave System**
   - Wave counter
   - Increasing difficulty
   - Short break between waves

5. **Basic Tower AI**
   - Auto-target nearest enemy
   - Shoot projectiles
   - Simple damage calculation

#### Phase 2 - Roguelite Core
6. **Upgrade System**
   - Timer that triggers every 30 seconds
   - Show 3 random upgrade cards
   - Apply selected upgrade

7. **Multiple Tower Types**
   - At least 5 different towers
   - Different stats and behaviors

8. **Meta-Progression**
   - Gem collection
   - Persistent save system
   - Unlock shop between runs

#### Phase 3 - Polish & Content
9. **Visual Polish**
   - Particle effects
   - Screen shake
   - Smooth animations

10. **Audio**
    - Background music
    - SFX for shooting, impacts, upgrades

11. **More Content**
    - Boss enemies
    - 15+ tower types
    - 20+ upgrades
    - Multiple difficulties

---

## Art & Audio Direction
- **Visual Style:** Clean, minimal 2D with bright colors (good visibility on mobile)
- **Color Palette:** Dark background with bright projectiles/enemies for contrast
- **UI:** Large touch-friendly buttons, clear icons
- **Audio Needs:**
  - Upbeat electronic music
  - Satisfying shooting SFX
  - UI click sounds

---

## Mobile-Specific Considerations
- **Touch Controls:** All interactions must be tap-based, large hit areas
- **Performance:** Must run at 60fps on mid-range phones
- **Screen Size:** Design for various aspect ratios (16:9, 18:9, 19.5:9)
- **Battery:** Optimize to avoid excessive battery drain
- **File Size:** Keep under 100MB for easy downloads

---

## Scope & Priorities

### MVP (Phase 1) - ~2-3 weeks
- [ ] Player core with HP
- [ ] 12-slot circular grid
- [ ] 1 basic tower type
- [ ] Basic enemy that moves to center
- [ ] Wave spawning system
- [ ] Win/lose conditions

### Enhanced (Phase 2) - ~2-3 weeks
- [ ] 5 tower types
- [ ] 30-second upgrade system
- [ ] 10+ upgrade options
- [ ] Gem collection
- [ ] Meta-progression/unlock system
- [ ] Save/load system

### Polish (Phase 3) - ~1-2 weeks
- [ ] Particle effects & juice
- [ ] Sound effects & music
- [ ] Boss enemies
- [ ] Expanded tower/upgrade pool
- [ ] Difficulty levels
- [ ] Main menu & settings

---

## Development Approach

**Recommended:** Build part-by-part in phases

**Reasoning:**
- Test core gameplay loop early
- Iterate on tower balance before adding complexity
- Ensure performance on mobile before adding content
- Get playable prototype quickly to validate fun factor

---

## Success Criteria
- Runs feel different each time (replayability)
- Core gameplay loop is satisfying in first 5 minutes
- Clear upgrade choices with meaningful impact
- Smooth 60fps on mobile devices
- "One more run" addictiveness

---

## Notes
- Keep UI simple and large (thumbs are big)
- Test on actual mobile device early
- Balance tower costs vs upgrade frequency
- Consider adding "auto-place" option for casual players
- Circular design should feel unique compared to lane-based TD

