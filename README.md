# Roshar

Roshar is a turn-based strategy game on a hexagonal board, built in Unity and inspired by Brandon Sanderson's *Stormlight Archive*: the **Fluctuomancers** (Radiants, allied with Honor) face off against the **Fused** (allied with Odium). Each side summons troops, moves them across a hexagonal grid, and fights until the opponent is wiped out.

---

## 1. The game

- **Board**: a hexagonal `Tilemap` (pointy-top). Each tile has a state (`TileState`): `basic`, `reachable`, `occupied`, `inAttackRange`, `summoningTile`.
- **Two factions**: `isFluct = true` (Fluctuomancers / Honor) or `false` (Fused / Odium). Summoning tiles are assigned to a faction depending on who starts (`GameData.highPlayer`).
- **Ability resources**: `stormlight` (fluct) and `voidlight` (fus), 6 points each, shared across the whole faction, fully restored every 8 turns.
- **Turn structure**: each turn is split into 3 phases handled by `PhaseManager`:
  1. **Movement** — move pieces already on the board (up to `nbMov` tiles, shortest path avoiding occupied tiles).
  2. **Summon** — summon a new piece on a summoning tile of your color (skipped on non-summoning turns, or chained straight into combat if an "enhanced summon" was performed).
  3. **Combat** — attack enemy pieces in range, or use a special ability.
- **Victory condition** (`WinCondition`): outside of summoning turns, if a faction has no pieces left on the board, the other faction wins the battle (`GameData.fluctScore` / `fusScore` +1) and the matching victory scene loads. The first faction to reach `GameData.winScore` (2 by default) wins the war.

---

## 2. Project structure

```
Roshar/
├── Assets/
│   ├── Scripts/
│   │   ├── GameData.cs                  # Global static data (scores, starting faction, unlocked troops)
│   │   ├── baseTileCreation.cs           # Custom Tile (BaseTile): color per state, shrink factor
│   │   ├── BoardSelectionScript.cs       # Hover/selection on the "BoardSelection" screen
│   │   ├── GameManagers/                 # The singletons driving the match
│   │   │   ├── PhaseManager.cs           # movement -> summon -> combat cycle
│   │   │   ├── TurnManager.cs            # Turn number, active player, summons, enhanced summon
│   │   │   ├── PieceInteractionManager.cs# Per-faction piece dictionaries, targeting, attack resolution
│   │   │   ├── PieceStateManager.cs      # State of each piece (moving/attacking/casting/locked/distracted)
│   │   │   ├── TileStateManager.cs       # State of each tile + summoning tile assignment
│   │   │   ├── InterruptionManager.cs    # Modal windows for "reactive" abilities
│   │   │   ├── AnimationManager.cs       # Triggers VFX (damage, death, StormShape)
│   │   │   ├── AppearanceManager.cs      # Visual icons (protected/locked/distracted)
│   │   │   ├── InterfaceManager.cs       # HUD (phase, turn, stormlight/voidlight, right-click inspector)
│   │   │   └── WinCondition.cs           # Win/loss condition
│   │   ├── PieceScripts/
│   │   │   ├── PieceMovement.cs          # Hex movement, summoning, locking
│   │   │   ├── PieceAttack.cs            # HP, range, damage, protection, distraction
│   │   │   ├── PieceAttributes.cs        # Piece name, "summoned minion" flag and summoner
│   │   │   ├── PieceGenerator.cs         # Regenerates a copy of the piece on its starting tile after a summon
│   │   │   ├── PieceAppearance.cs        # Bridges piece state -> AppearanceManager
│   │   │   └── Abilities/
│   │   │       ├── Ability.cs            # Abstract base class: cost, activation, resource restoration
│   │   │       ├── FluctAbilities/       # Edgedancer, Lightweaver, Skybreaker, Stoneward, Windrunner
│   │   │       └── FusAbilities/         # Deepest, Heavenly, Magnified, Masked, Pursuer, StormShape
│   │   ├── MapParameters/MapParameters.cs# ScriptableObject: summoning turns, special summons, troops unlocked per map
│   │   ├── WindowManagers/               # Menu, map selection, Honor/Odium victory screens
│   │   ├── InterruptionScripts/          # Interruption window + buttons (accept/cancel a reactive ability)
│   │   ├── AnimationScripts/             # Generic "slide"-based animation system (AnimationCreator + Slide)
│   │   ├── Listeners/PhaseAnnounceListener.cs # Displays "Move !/Summon !/Fight !" and special announcements
│   │   ├── UIScripts/HoverSelection.cs   # Highlights menu buttons on hover
│   │   ├── GameUtils/Utils.cs            # Utility functions (D4 roll, mouse -> tile, lookup by value)
│   │   ├── EditorUtils/                  # Editor tools (button label, phase button text/color)
│   │   └── UML/                          # Design diagrams
│   ├── Editor/AbilityScriptGenerator.cs  # Unity menu item to generate a new ability from a template
│   ├── Docs/                             # Generated Doxygen documentation (html/ and latex/)
│   └── Scenes/                           # Menu, map selection, playable maps, victory screens
├── Packages/, ProjectSettings/, UserSettings/  # Standard Unity configuration (6000.0.47f1)
```

---

## 3. Game loop in detail

- **`TurnManager`** alternates `turn` between the two factions, increments `turnNumber`, caps the number of summons per turn (`MapParameters.numberOfSummon`), and handles the "enhanced summon": a piece flagged `enhancedTroop` (in `PieceMovement`) that was just summoned lets its faction skip straight past the movement phase next turn, at the cost of a turn without summoning afterwards.
- **`PhaseManager.nextPhase()`** advances through the 3 phases; at the end of the combat phase, if the faction about to play has no pieces left on the board, the movement phase is skipped and the faction goes straight to summoning.
- **`PieceMovement`** computes reachable tiles with a breadth-first search over the 6 hex neighbors (the neighbor offsets change depending on the parity of `y`, see `neighbourOffset` / `neighbourOffsetOdd`). Pieces are moved either by dragging with the mouse or by clicking the piece then the destination tile.
- **`PieceAttack`** manages HP (`lp`), attack range, number of attacks per turn, damage (`damage`/`trueDamage`, the latter ignoring damage reduction), healing, attack boosts, damage reduction, distraction (prevents attacking) and locking (prevents moving).

---

## 4. Abilities (`Assets/Scripts/PieceScripts/Abilities`)

Every ability inherits from `Ability` (cost in stormlight/voidlight, activated by right-clicking its own piece, auto-reset on turn change) and overrides `ActivateAbility()` / `resetAbility()`.

| Faction | Ability | File | Effect |
|---|---|---|---|
| Fluct (Radiants) | Edgedancer | `EdgedancerAbility.cs` | Heals an ally within range 1 |
| Fluct | Lightweaver | `LightweaverAbility.cs` | Deals fixed damage to an enemy within range 1 |
| Fluct | Skybreaker | `SkybreakerAbility.cs` | Deals variable damage (1d4+1) to an enemy within range 1 |
| Fluct | Stoneward | `StonewardAbility.cs` | Summons up to 2 minions around the piece |
| Fluct | Windrunner | `WindrunnerAbility.cs` | Locks an enemy piece (can no longer move) for N turns |
| Fus (Fused) | Deepest | `DeepestAbility.cs` | Interrupts an incoming attack: the piece becomes immune for that hit |
| Fus | Magnified | `MagnifiedAbility.cs` | Interrupts an attack, protects nearby allies and reduces damage |
| Fus | Masked | `MaskedAbility.cs` | Distracts an enemy piece (prevents it from attacking) for N turns |
| Fus | Heavenly | `HeavenlyAbility.cs` | Teleports up to 2 allies to free tiles |
| Fus | Pursuer | `PursuerAbility.cs` | Moves adjacent to an enemy and boosts its own attack |
| Fus | StormShape | `StormShapeAbility.cs` | True damage (ignores damage reduction) within range 2, on a 1-turn cooldown |

**Deepest** and **Magnified** are interrupt abilities: when the piece is attacked, `PieceAttack.OnMouseDown` detects the ability (on the targeted piece, or on a neighboring ally for Magnified) and opens a confirmation window through `InterruptionManager`/`WindowInterruption`, which blocks the game (`isInterruptionActive`) until the player answers.

A new ability script can be generated from Unity via `Assets > Create > Ability Script` (`AbilityScriptGenerator.cs`), based on the `Assets/Editor/AbilityScriptTemplate.txt` template.

---

## 5. Interface, animations and maps

- **`InterfaceManager`** shows the current phase, turn, stormlight/voidlight gauges, and a tile/piece inspector on right-click (HP, attacks left, movement left, damage reduction, ability cost).
- **`AnimationManager`** triggers one-shot effects (damage text colored by amount, death, StormShape lightning); **`AppearanceManager`** adds/removes persistent icons (protected, locked, distracted) as long as the matching piece state is active.
- **`AnimationCreator` + `Slide`** form a small step-based animation system: a list of `Slide`s (direction in degrees, speed, duration) played sequentially, used notably by `PhaseAnnounceListener` for the phase announcement ("Move !"/"Summon !"/"Fight !") and special summoning turns.
- **Playable maps** (`Assets/Scenes`): Akinah, Azimir, Kholinar, Kurth, Narak, Panatham, Rall Elorim, Shinovar, Thaylenah City, Vedenar — each with its own `MapParameters` (summoning turns, unlocked troops). The menu (`Menu`, `Menu - MapSelection`) and the `HonorVictory`/`OdiumVictory` screens complete the navigation flow.

---

## 6. Running the project

- Unity **6000.0.47f1** (Unity 6) — open the `Roshar/` folder with Unity Hub.
- Launch the `Menu.unity` scene for the normal flow (menu → map selection → match), or open a map scene directly for quick testing.
- Technical documentation generated with Doxygen (from the `/** ... */` comments in the code) lives in `Assets/Docs/html/index.html`.

---

## 7. Known issues / current limitations

- The game is designed for 2 local players on the same machine (no networking).
- `PieceInteractionManager.resetTargets()` resets the whole map instead of only the affected tiles (per the code comment: an optimization was planned but postponed because some tiles stayed stuck in the `reachable` state otherwise).
- Victory score (`GameData.winScore`) and progress data (`isUnlocked`, scores) are static variables: they persist as long as the application stays open, including between battles of the same war.
