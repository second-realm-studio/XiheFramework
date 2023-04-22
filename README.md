# XiheFramework  
A Unity-based game framework for agile development designed to be used by small teams (1-10). Uses names instead of IDs to prevent you from filling a bunch of excel tables :)
## Document
[XiheFramework API Document](https://sky-haihai.github.io/xiheframework-document/) 
## Main Features  

### Core Modules
#### 1. Audio  
Encapsulation of Unity Audio Source and Audio Mixer. Output audio into different channels (Planning to change to FMod instead)
#### 2. Blackboard
Runtime Variable Pool.  
#### 3. Event
Runtime Event Pool.
#### 4. FSM(Finite State Machine)
Runtime FSM Pool. Indexing by name
#### 5. Input
Encapsulation of Unity Legacy Iput System. Provide useful features like Keyboard remapping and useful functions like GetMouseDelta(get mouse direction and speed)
#### 6. Localization
Get text and assets(textures, models) that match the current Language setting. 
#### 7. Serialization
Serialize runtime data into binary data to save the game progress.
#### 8. UI
Manage all UI Canvas states. (Active/Deactive)

### Name-based indexing
Core Modules usually uses names instead of IDs as keys to indexing.

### FlowCanvas Intergration
Functions can be used as nodes inside FlowCanvas via Service layer.

## Tips
* Create your own Game class as a partial class in your own project to get custom modules faster.
* Create your own GameString class as a constant string manager(e.g. event names, blackboard variable names).
## Add to your Unity project
Add Git Submodule  
Path: Assets/XiheFramework

#Special Thanks
Lawrence P
