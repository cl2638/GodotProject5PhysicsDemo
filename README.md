# Godot Physics Demo

## Overview
This project demonstrates several interactive physics-based mechanics in Godot 4.4 using C# (Mono). The project includes:

- **Chain/Rope Physics:** A rope made of multiple `ChainSegment` nodes connected with `PinJoint2D`. The rope swings realistically, especially at the bottom segments, and reacts to the player hitting it.  
  - *Note* The rope does not swing excessively when the player interacts; the top is pinned to prevent unrealistic motion.

- **Exploding Particles:** A `GpuParticles2D` node shows idle particles until the player collides with it. On collision, it explodes into a burst of particles, simulating an impact effect.

- **Laser Detector:** A `RayCast2D` detects the player. When the player passes through, the laser changes color and prints a message to the console indicating detection.

## Implementation Details
- The rope was challenging to implement initially. To achieve proper swinging, `ChainSegment` nodes are connected using `PinJoint2D` with damping and mass adjustments to achieve realistic motion.  
- Particle behavior is handled fully within the `ParticleController.cs` script, including runtime `Area2D` collision detection.  
- The laser is implemented as a `Node2D` with a `RayCast2D` and `Line2D` for visual feedback.  
- All player movement is handled via a simple `CharacterBody2D` script using `MoveAndSlide`.

## Notes
- Rope motion is constrained to prevent excessive swinging, providing stable and predictable gameplay.  
- Particle explosion and laser detection are triggered only when the player interacts with them.  
- The project uses Godot 4.4 and C# (Mono), ensure Mono is installed for proper compilation.

## How to Run
1. Open the project in Godot 4.4 with Mono support.  
2. Run the `Main` scene.  
3. Use arrow keys to move the player around.  
4. Interact with rope, particles, and laser to see the physics and effects in action.

## Pitfalls
- Initial attempts at rope physics caused overlapping and excessive swinging. This was corrected by spacing segments based on their collision/sprite size and tuning `PinJoint2D` parameters.  
- Particles require runtime `Area2D` for collision; otherwise, explosion effects would not trigger.

