# TDMonoGameEngine
A 2D MonoGame engine with flexible core fuctionality and useful wrappers and utilities.

This simple engine aims to eliminate a lot of boilerplate code by providing the following:
* Several debug features (frame-by-frame stepping, logs, FPS counter, rendering statistics) that can be toggled
* Keyboard, mouse, and joystick input wrappers
* 2D camera
* Finite state machine for game states
* Injectable debug update and draw routines that can be unique to game states
* Screenshot fuunctionality
* A RenderingManager that handles setting up a RenderTarget and drawing everything to it
* A simple SoundManager that pools SoundEffectInstances and handles separate music and sound volume
* Resolution scaling
* A RenderLayer system using multiple RenderTargets - can be replaced by your own system if not needed
* Basic post-processing system allowing shaders to be applied after all rendering is done
* Interpolation methods
* Plenty of other classes and utilities, such as Line, Circle, and RectangleF structs, and an EnumUtility for handling bitmask Enums
* Crash handler that works on Windows, macOS, and Linux
* And more

This engine is primarily made for 2D games on desktop platforms, but it can easily be extended to work with other platforms. If you'd like to add something useful to this engine, feel free to submit a PR.