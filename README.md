# GRBL-Plotter
[README deutsch](README_de.md)  
A GCode sender for GRBL under windows, using DotNET 4.0 (should also work with Windows XP)  
Written in C# VisualStudio 2015.
 
### New in version 1.1.1.0: 
Zooming into graphic 
New font 'Dot Matrix' 
Option for SVG-Import 'Process path nodes only' 
Support of 4th axis 
SVG-Import bug fix 
 
### New in version 1.1.0.0:  
Surface scanning for height map creation  
[Autoleveling](https://github.com/svenhb/GRBL-Plotter/wiki/auto-leveling) using height map  
Substitution for [Rotary Axis](https://github.com/svenhb/GRBL-Plotter/wiki/rotary-axis) (instead of X or Y control a rotary axis)  
[Subroutines](https://github.com/svenhb/GRBL-Plotter/wiki/subroutines) M98, M99 Sub-Program Call (P, L)    
Create GCode absolute or relative (for further use as subroutine)  
DXF Import (Missing: Text and Ellipse, Spline with more than 4 points nok)  
  
### New in version 1.0.3.0:  
Two languages - german, english  
Rework the camera form 
  
### New in version 1.0.2.0:
Supporting GRBL 1.1 (and 0.9 also)  
Export / import machine specific settings (Joystick, Buttons)  
Simple shapes  
Loading GCode with optional replacement of M3 / M4 commands (helpful for 'Laser Mode' $32=1)  

[Check the Wiki for further information](https://github.com/svenhb/GRBL-Plotter/wiki)  

### Program is free and you can use it at your own risk, as you understand there is no warranty of any kind
Zip folder contains ClickOnce application setup. Exe can be found in sub-folder GRBL-Plotter/bin/release.  
#### [GRBL-Plotter Vers. 1.1.1.0](GRBL-Plotter_1110_Publish.zip)  2017-10-22  

### Requirements for compiling
* VisualStudio 2015 
* DotNET 4.0
 
### Features:
* Supporting GRBL 1.1 (and 0.9 also)  
* Export / import machine specific settings (Joystick, Buttons)
* Controlling a 2nd GRBL-Hardware
* Tool exchange
* User defined Buttons - GCode from text-field or file
* Joystick like control
* Automatic reconnect on program start
* Recent File List (Files and URLs)
* 2-dimensional preview
* Import/creation and conversion into GCode 
  - from SVG and DXF Graphics
  - from Text (into Hershey Font)
  - conversion of the Z-Dimension into Z-axis (router) or Spindle on/off (laser) or Spindle-Speed (RC-Servo PWM) 
* GCode can be edited and saved
* Drag & Drop of GCode (*.nc) and SVG (*.svg) files
* Drag & Drop (or Copy & Paste) of Browserlinks to SVG-files (works only with Chrome) for example from https://openclipart.org/
* Transformation of GCodes (scale, rotation, mirror, zero-Offset)
* Optional usage of a WebCam with graphics overlay of the current GCode, set zero point, measure angle, zoom
* Internal variable to support probing, e.g.:
  - G38.3 Z-50		(probe toward tool length sensor, stop on contact - because of decelaration stop-pos. is not trigger-pos.)
  - G43.1 Z@PRBZ	(Offset Tool with value stored on trigger of sensor switch)
  - examine SerialForm.cs for implementation

### ToDo
* Wizzard to generate simple forms in GCode

### My test bed
On my german homepage:
[my XYZ platform](http://svenhb.bplaced.net/?CNC___Plotter) 

### GRBL-Plotter in tool change action
[![Import an image](https://img.youtube.com/vi/x5UTHpgsfII/0.jpg)](https://www.youtube.com/watch?v=x5UTHpgsfII) 

### Screenshots
Main GUI
![GRBL-Plotter GUI](doc/GRBLPlotter_GUI.png?raw=true "Main GUI") 

Separate serial COM window(s) - one for the CNC, one for the tool changer (or 4th axis)  
![GRBL-Plotter COM interface](doc/GRBLPlotter_COM2.png?raw=true "Serial connection") ![2nd GRBL control](doc/GRBLPlotter_Control_COM2.png?raw=true "Serial connection")

Setup import / GCode conversion  
![GRBL-Plotter Setup1](doc/GRBLPlotter_Setup1.png?raw=true "Setup1") 

Setup user defined buttons  
![GRBL-Plotter Setup2](doc/GRBLPlotter_Setup2.png?raw=true "Setup2") 

Setup tool change and colors  
![GRBL-Plotter Setup3](doc/GRBLPlotter_Setup3.png?raw=true "Setup3") 

Text import  
![GRBL-Plotter Text](doc/GRBLPlotter_Text.png?raw=true "Text conversion") 

Picture import  
![GRBL-Plotter Image](doc/GRBLPlotter_Image.png?raw=true "Image import") 

Different scaling options  
![GRBL-Plotter Scaling](doc/GRBLPlotter_scaling.png?raw=true "GCode scaling") 

Feed rate overrid for Version 0.9 and 1.1  
![GRBL-Plotter Override](doc/GRBLPlotter_override.png?raw=true "GCode override") ![GRBL-Plotter Override](doc/GRBLPlotter_override2.png?raw=true "GCode override")
