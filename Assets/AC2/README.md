 # AnimatorController as Code (AC2)
AC2 is a Unity plugin to convert a YAML template to an AnimatorController asset.
## Features
Besides generating a AnimatorController from code, AC2 also offers:
1. Generating transition animations
2. Generating AnimationEvents
3. Link Assets by Path
4. Very light weight because AC2 uses native Unity classes
5. Works well with animations from Adobe mixamo.
## Hollo World
```
name: GeneratedAC2AnimatorController
saveTo: Assets/Resources/Animators
controller:
  layers:
  - name: Base Layer
    stateMachine:
      name: Root
      stateMachines:
      - stateMachine:
        name: Idle
        states:
        - state:
            name: Idle
            speed: 1
            motion:
			  !AssetRef
			  path: Animations/Idle
		    transitions:
		    - hasExitTime: true
```
Output:


## Usage
Add **Creator** to an empty game object.
