# GameArknightUnity

A Unity 3D tower-defense project inspired by the tactical deployment and lane-defense gameplay of *Arknights*. The project is being developed as a hands-on exploration of gameplay systems, scene flow, and data-driven character setup in Unity.

## Current Features

- **Grid-based battlefield** with low-ground and high-ground tiles, placement restrictions, and occupied-tile tracking.
- **Operator squad selection** with a configurable squad limit and ScriptableObject-based operator data.
- **Operator deployment** with DP costs, deployment cooldown handling, and facing-direction selection.
- **Combat systems** for melee attacks, projectile attacks, target selection, enemy blocking, and operator healing.
- **Enemy waves** configured through `WaveData`, with enemies following a waypoint path toward the base.
- **DP economy** with starting DP, timed recovery, spending, and refunds.
- **Game flow and UI** covering the main menu, preparation/battle phases, pause/state handling, and win/defeat screens.

## Tech Stack

- Unity
- C#
- Universal Render Pipeline (URP) assets
- TextMesh Pro
- ScriptableObjects for operator and wave configuration

## Project Structure

```text
Assets/_Game/
├── Art/          # Operator models, animations, materials, and class icons
├── Data/         # Operator database and wave configurations
├── Prefabs/      # Characters, tiles, effects, and UI cards
├── Scenes/       # MainMenu and Level_01
└── Scripts/
    ├── Core/
    ├── Gameplay/
    │   ├── Combat/
    │   ├── Deployment/
    │   ├── DP/
    │   ├── Enemies/
    │   ├── Map/
    │   ├── Operators/
    │   └── Waves/
    ├── Managers/
    └── UI/
```

## Getting Started

1. Clone or download this repository.
2. Open the project through **Unity Hub** using the Unity Editor version configured for the repository.
3. Open `Assets/_Game/Scenes/MainMenu/MainMenu.unity` to start from the main menu, or `Assets/_Game/Scenes/Gameplay/Level_01.unity` to inspect the current level.
4. In the Editor, press **Play**.

> Open the complete Unity project, not just the `Assets` folder. If the project does not open as expected, check that its `Packages` and `ProjectSettings` folders are present in the repository.

## Development Status

**Work in progress.** The core gameplay architecture and a playable test level are under active development. Systems, balancing, visuals, and content may change as the project evolves.

## Notes

This is an independent learning and development project inspired by *Arknights*. It is not affiliated with or endorsed by Hypergryph or Yostar. Any third-party assets remain the property of their respective owners.
