# Test 3D CSharp

## FPS Template Project
Basically the same project as Test3DGDScript, but all scripts written in C#

__________________________________________________

### Keyboard controls:


  ___Movement Controls:___
* WASD - Walking (done)
* Left Ctrl - Crouching (done)
* Left Shift - Running (done)
* Space - Jumping (done)
* Q - Lean Left (done)
* E - Lean Right (done)

  ___Interaction Controls:___
* F - Interact (done)
* G - Drop Current Weapon (done)
* Tab - Inventory (incoming)

  ___Combat Controls:___
* LMB - Shoot (incoming)
* RMB - Aim Down Sights (armed) / Focus (unarmed) (incoming)
* R - Reload (incoming)
* ` - Empty Hands (done)
* 1 - Primary Weapon (done)
* 2 - Secondary Weapon (done)
* 3 - Pistol Weapon (done)
* 4 - Melee Weapon (done)

  ___Menu & Debug Controls:___
* Esc - In-Game Menu (done)
* / (Slash) - Debug Panel (done)

__________________________________________________

### Currently done:
> * Fullbody character with separated head and body meshes
> * WASD movement and sprinting functionality
> * Test_Level for testing project's mechanics/features
> * Debug Panel that shows player's current_state and movement_velocity and fps
> * 8-Way directional movement animations for crouch walking, stand walking
> * Jump and falling states and connected animations
> * Prototype stairs for slope climbing mechanics in test_level scene
> * AKM_Custom with magazine and romanian dong module
> * In-Game Menu with Resume, Options and Exit buttons -> functionable
> * Functionable interaction system (equip and drop works with all weapons)
> * Functionable "Weapon System" (including primary, secondary, pistol and melee slots)

    IMPORTANT:
> * Implemented "State Machine Pattern" for managing player movement states and for better animation control
> * Implemented "Command Pattern" for WeaponManager using async methods
> * Implemented "Service Locator Pattern" for better singletons organization and making architecture more loosely-coupled

__________________________________________________

### Incoming updates:
> * Functionable Shooting(procedural-animation), Reloading(non-procedural), Aiming(procedural), Switch Fire Mode(non-procedural) mechanics
> * Functionable UI for Interaction System for picking-up items
> * Functionable bullet projectile ballistics, based on its caliber
> * Functionable player equipment UI panel
> * Functionable player spatial-inventory and implemented UI
> * Functionable Item System
> * Functionable Multiplayer Mode "Team-Deathmatch", "Zone-Capture" and "Search & Destroy(with looting-mechanics)"
> * Functionable foot-work IK for stairs
