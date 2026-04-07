# PrecisionDrop

![hippo](https://github.com/user-attachments/assets/daaa8a49-73ba-47bc-b849-ad3a4e3ab567)

## Quick Overview

A Helix Jump–style prototype used as a playground for architecture.

The goal is not to recreate the game, but to build it in a way that scales.
Simple mechanics, strict scope, and full focus on system design.

The project is structured with clear boundaries between gameplay logic and Unity-specific code.
Systems communicate through events and interfaces, with dependency injection handling composition.
All tunable gameplay elements are data-driven and supported by custom editor tooling.

🔗 **Play (WebGL):**  
https://cocacopa.itch.io/precision-drop
  
🎥 **Gameplay Showcase:**  
https://youtu.be/jM7w2Ax6zHA

---

## Design Principles & Systems

The project is built around a set of design principles that prioritize scalability, control, and clarity.  

### Core Principles
- **Data-driven generation**  
  Level generation is fully configurable, allowing difficulty tuning without rewriting logic
- **Separation of concerns**  
  Runtime logic is isolated from Unity-specific code to keep systems maintainable and testable
- **Extensibility**  
  Systems are designed to evolve without requiring structural rewrites
- **Tooling support**  
  Editor tools are used to improve iteration speed and game-balance visibility

---

## Project Architecture

The project is structured into three layers:
- `Contracts` : Interfaces and data definitions shared between systems.
- `Runtime`   : Pure C# gameplay logic independent of Unity APIs.
- `Unity`     : MonoBehaviour components responsible for scene integration and presentation.

Assembly Definitions are used to enforce boundaries and prevent unintended coupling.

### Main domains
- **App bootstrap & system wiring**  
  Handles system initialization and dependency setup
- **Game flow coordination**  
  Applies gameplay rules and coordinates progression
- **Input abstraction layer**  
  Captures player's input and provides consumable data
- **Player system**  
  Manages player-side activity
- **Platform system**  
  Platform behaviour and spawning
- **Level generation system**  
  Procedural generation logic

### Implementation Patterns
- **Interface-driven design**  
  Systems communicate through contracts instead of direct dependencies
- **Event-based communication**  
  Decoupled systems interact through events rather than tight coupling
- **Behaviour / structure / presentation split**  
  Gameplay logic, data, and visuals are handled independently
- **ScriptableObject configuration**  
  Themes, settings, and tunables are driven through assets instead of hardcoded values

---

## Player Physics (Custom Gravity)
<img width="525" height="383" alt="DynamicGravity" src="https://github.com/user-attachments/assets/d27da461-711d-4465-8e2f-cd51b8b4c050" />

- Custom gravity controller with separate ascent/descent scaling
- Curve-based modulation for precise jump and fall behavior
- Optional max fall speed to control acceleration

---

## Platform Generation Pipeline 
<img width="540" height="421" alt="Configuration" src="https://github.com/user-attachments/assets/35192ea6-7328-4122-983e-41762e62d06d" />  

Platforms are generated through a staged pipeline that separates gap creation from danger placement. Each phase is isolated to keep the system debuggable, tunable, and predictable under iteration.  

- **Gap Generation**   
Defines how many gaps exist and their allowed sizes, by picking a random gap configuration based on a weighted chance.
- **Solid Section Extraction**    
The remaining segments between gaps are computed as solid sections.
- **Danger Section Decision**  
  - Small sections may become fully dangerous (based on capped probability).
  - Otherwise, danger is generated inside the section using a dedicated pipeline.

### Danger Generation Pipeline
Danger placement within a solid section is handled in multiple controlled steps:
- **Pair Count**  
  Decides how many danger pairs to spawn
- **Distribution**  
  Spaces them across the section
- **Variation**  
  Introduces randomness by shrinking safe areas
- **Offset**  
  Shifts the pattern to avoid repetition
- **Edge Snap**  
  Occasionally aligns danger pieces to section edges
