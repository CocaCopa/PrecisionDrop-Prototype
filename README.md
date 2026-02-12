# PrecisionDrop (Work In Progress)  

![hippo](https://github.com/user-attachments/assets/386e1c69-4580-4212-b62c-a55bf1d54d95)

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

Features are split into:
- `Contracts`: interfaces & data definitions
- `Runtime`: engine-agnostic gameplay logic
- `Unity`: MonoBehaviours and scene integration

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

*In development

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
