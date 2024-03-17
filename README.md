# XiheFramework  
A Unity-based game framework for agile development designed to be used by small teams (1-10). Uses names instead of IDs to prevent you from filling a bunch of excel tables :)
## Document
[XiheFramework API Document](https://sky-haihai.github.io/xiheframework-document/) 
## Main Features  

### Core Modules
#### Blackboard
Runtime Variable Pool.  
#### Event
Runtime Event Pool.
#### FSM(Finite State Machine)
Runtime FSM Pool. Indexing by name
#### Input
Encapsulation of Unity Legacy Iput System. Provide useful features like Keyboard remapping and useful functions like GetMouseDelta(get mouse direction and speed)
#### Localization
Get text and assets(textures, models) that match the current Language setting. 
#### Serialization
Serialize runtime data into binary data to save the game progress.
#### UI
Manage all UI Canvas states. (Active/Deactive)

### Combat Modules
#### Action
#### Animation
#### Buff
#### CameraSwitcher
#### Damage
#### Dashable
#### Interact
#### Particle
#### Projectile

## Tips
* Create your own ```Game(<Description>)``` class as a partial class within ```XiheFramework.Entry``` namespace as entry points of all game modules.
## Add to your Unity project
Add Git Submodule  
Path: Assets/XiheFramework

## Special Thanks
[Lawrence P](https://github.com/ShenKSPZ)
