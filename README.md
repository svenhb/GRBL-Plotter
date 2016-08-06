# GRBL-Plotter
A GCode sender for GRBL under windows, using DotNET 4.0 (should also work with Windows XP)
Written in C# VisualStudio 2015.

### New:
Controlling a 2nd Arduino with GRBL for tool change or 4th (5th, 6th) axis control (GRBL-Plotter is waiting for 'IDLE' on one GRBL before controlling the other). Commands for 2nd GRBL will be introduced via special formatted remarks in GCode e.g.: (^2 G90 X2) to move 2nd GRBL to position X2.

### Program is free and you can use it at your own risk, as you understand there is no warranty of any kind
Zip folder contains ClickOnce application setup. Exe can be found in sub-folder GRBL-Plotter/bin/release.  
####[GRBL-Plotter](GRBL-Plotter_0201.zip)  
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
* Setup of the Joystick parameters (speed depending on step-size)

### My test bed
On my german homepage:
[my XYZ platform](http://svenhb.bplaced.net/?CNC___Plotter)

### GRBL-Plotter in tool change action
[![Import an image](https://img.youtube.com/vi/fvYWyE2GBsg/0.jpg)](https://www.youtube.com/watch?v=fvYWyE2GBsg)

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
![GRBL-Plotter Image](GRBLPlotter_Image.png?raw=true "Image import")

Different scaling options  
![GRBL-Plotter Scaling](GRBLPlotter_scaling.png?raw=true "GCode scaling")
