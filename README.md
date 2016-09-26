# GRBL-Plotter
A GCode sender for GRBL under windows, using DotNET 4.0 (should also work with Windows XP)
Written in C# VisualStudio 2015.

### New in version 0205:
Feed rate and spindle speed override during streaming gcode.

[Check the Wiki for further information](https://github.com/svenhb/GRBL-Plotter/wiki)  

### Program is free and you can use it at your own risk, as you understand there is no warranty of any kind
Zip folder contains ClickOnce application setup. Exe can be found in sub-folder GRBL-Plotter/bin/release.  
####[GRBL-Plotter Vers. 0205](GRBL-Plotter_0205.zip)  2016-09-26  
Unfortunatly the enclosed '_misc' folder will not be installed with 'ClickOnce'. Therefore the example files for color palettes and GCode-scripts will not be found by GRBL-Plotter (with default setup). You need to set the according paths new in the setup dialog - check screenshots below.

### Requirements for compiling
* VisualStudio 2015 
* DotNET 4.0
 
### Features:
* User defined Buttons - GCode from text-field or file
* Joystick like control
* Automatic reconnect on program start
* Recent File List (Files and URLs)
* 2-dimensional preview
* Import/creation and conversion into GCode 
  - from SVG Graphics
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
![GRBL-Plotter GUI](GRBLPlotter_GUI.png?raw=true "Main GUI")

Seperate serial COM window(s) - one for the CNC, one for the tool changer (or 4th axis)  
![GRBL-Plotter COM interface](GRBLPlotter_COM2.png?raw=true "Serial connection")

Setup import / GCode conversion  
![GRBL-Plotter Setup1](GRBLPlotter_Setup1.png?raw=true "Setup1")

Setup user defined buttons  
![GRBL-Plotter Setup2](GRBLPlotter_Setup2.png?raw=true "Setup2")

Setup tool change and colors  
![GRBL-Plotter Setup3](GRBLPlotter_Setup3.png?raw=true "Setup3")

Text import  
![GRBL-Plotter Text](GRBLPlotter_Text.png?raw=true "Text conversion")

Picture import  
![GRBL-Plotter Image](GRBLPlotter_Image2.png?raw=true "Image import")

Different scaling options  
![GRBL-Plotter Scaling](GRBLPlotter_scaling.png?raw=true "GCode scaling")

Feed rate override  
![GRBL-Plotter Override](GRBLPlotter_override.png?raw=true "GCode override")
