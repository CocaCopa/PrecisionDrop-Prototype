# PrecisionDrop (Work In Progress)  

![hippo](https://github.com/user-attachments/assets/daaa8a49-73ba-47bc-b849-ad3a4e3ab567)

## Quick Overview

A Helix Jump-inspired gameplay prototype built in Unity. (**Work in progress**)  

This prototype focuses on improving collision fairness in a Helix Jump-style game. While playing the original, I frequently encountered moments where the ball looked safe but still triggered a bounce or failure due to oversized collision boundaries.  
Here, the contact area is intentionally reduced to better match what the player sees, favoring clarity and perceived control over strict physical correctness.

### Current Focus

- Expanding generation rules
- Refining system orchestration
- Gameplay feel tuning
- Difficulty balancing

---

## Goals

The goal is to build gameplay systems that can be extended comfortably as complexity increases.

- Data-driven level generation
- Tunable difficulty without rewriting systems
- Editor tooling for better game-balance experience
- Clear separation between runtime logic and Unity-specific code

---

## Current State

The project is structured into three layers:
- `Contracts` : Interfaces and data definitions shared between systems.
- `Runtime`   : Pure C# gameplay logic independent of Unity APIs.
- `Unity`     : MonoBehaviour components responsible for scene integration and presentation.

Assembly Definitions are used to enforce boundaries and prevent unintended coupling.

Main domains:
- **App bootstrap & system wiring**  
Makes sure initialization timing is correct between systems
- **Game flow coordination**  
Applies game rules
- **Input abstraction layer**  
Captures player's input and provides consumable data
- **Player system**  
Manages player-side activity
- **Platform system**  
Platform behaviour and spawning
- **Level generation system**  
Procedural generation logic

---

### Level Generation

- Configurable platform segments
- Progressive danger density scaling based on player survival time*
- Gap configurations with weighted chances
- Custom editors for better clarity

---

### Procedural Danger Generation

Platforms are generated using a multi-phase procedural pipeline:

1. <b>Pair Count Phase:</b>  
Determines how many danger pairs will appear on the platform.

2. <b>Gap Distribution Phase:</b>  
Evenly distributes safe segments between danger sections.

3. <b>Gap Variation Phase:</b>  
Introduces randomness by shrinking safe sections between danger sections.

4. <b>Offset Phase:</b>  
Offsets the entire pattern to avoid predictable layouts.

5. <b>Edge Snap Phase:</b>  
Occasionally snaps the first or last danger section to platform edges.

The phases are intentionally isolated to allow debugging and balancing each step independently.

---

### Platform & Player Systems

- Interface-driven design
- Event-based communication between components
- Separation of behavior, structure, and presentation
- ScriptableObject-driven configuration for themes and player settings

### Custom Gravity
- Custom gravity controller with separate ascent/descent scaling and tunable fall behavior
- Curve-based modulation for precise jump/fall feel
- Optional max fall speed to prevent uncontrolled acceleration
