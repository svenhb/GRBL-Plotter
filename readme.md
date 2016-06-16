# GRBL-Plotter
A GCode sender for GRBL under windows using DotNET 4.5 (should also work with XP)
Written in C# VisualStudio 2015 

### Requirements for compiling
* VisualStudio 2015 
* DotNET 4.5

### Features:
* User defined Buttons - GCode from text-field or file
* Joystick like control
* Automatic reconnect on program start
* Recent File List (Files and URLs)
* 2-dimensional preview
* Import/creation and conversion into GCode 
  - from SVG Graphics
  - from Text (into Hershey Font)
  - conversion of the Z-Dimension into Z-axis (Fräse) or Spindle on/off (Laser) or Spindle-Speed (RC-Servo PWM) 
* GCode can be edited and saved.
* Drag & Drop of GCode (*.nc) and SVG (*.svg) files
* Drag & Drop (or Copy & Paste) of Browserlinks to SVG-files (works only with Chrome) for example from https://openclipart.org/
* Transformation of GCodes (scale, rotation, mirror, zero-Offset)
* Optional usage of a WebCam with graphics overlay of the current GCode, set zero point, measure angle, zoom

### Screenshots
![GRBL-Plotter GUI](GRBLPlotter_GUI.png?raw=true "Main GUI")

![GRBL-Plotter COM interface](GRBLPlotter_COM.png?raw=true "Serial connection")

![GRBL-Plotter Setup1](GRBLPlotter_Setup1.png?raw=true "Setup1")

![GRBL-Plotter Setup2](GRBLPlotter_Setup2.png?raw=true "Setup2")

![GRBL-Plotter Setup3](GRBLPlotter_Setup3.png?raw=true "Setup3")

![GRBL-Plotter Text](GRBLPlotter_Text.png?raw=true "Text conversion")

![GRBL-Plotter Scaling](GRBLPlotter_scaling.png?raw=true "GCode scaling")
