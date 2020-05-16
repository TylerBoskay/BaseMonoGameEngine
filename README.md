# BaseMonoGameEngine
A simple 2D MonoGame boilerplate engine with useful wrappers and utilities.

This simple engine aims to eliminate a lot of boilerplate code by providing the following:
* Several debug features (frame-by-frame stepping, logs, FPS counter, rendering statistics)
* Keyboard, mouse, gamepad, and joystick input wrappers
* 2D camera
* Finite state machine for game states
* Injectable debug update and draw routines, which can also be unique to certain game states
* Screenshot functionality
* A RenderingManager that handles setting up a RenderTarget and drawing everything to it
* A simple SoundManager that pools SoundEffectInstances and supports separate music and sound volumes
* Resolution scaling
* A RenderLayer system using multiple RenderTargets - can be replaced by your own system if not needed
* Basic post-processing system allowing shaders to be applied in a desired order after all rendering is done
* Interpolation methods
* Plenty of other classes and utilities, such as Line, Circle, and RectangleF structs, and an EnumUtility for handling bitmask Enums
* Crash handler that works on Windows, macOS, and Linux
* And more!

This engine is primarily made for 2D games on desktop platforms. If you'd like to add onto this engine, feel free to submit a PR.
