# GRBL-Plotter
[README deutsch](README_de.md)  
A GCode sender for GRBL under windows, using DotNET 4.0 (should also work with Windows XP)  
Written in C# VisualStudio 2015.
 
### New in version 1.1.4.0: 
Add import of Eagle Drill file  
Add support of noname USB GamePad  
Add import (and ruler) selection 'mm' / 'inch' in setup  
Comment-out unknown GCode  
 
### New in version 1.1.3.0: 
Add feed rate and spindle speed override buttons (GRBL 1.1) to main GUI  
Bug fix in main GUI: no zooming during streaming - disabled background image (perhaps performance in Win-XP is affected)  
Bug fix SVG Import: rectangle transform (G3 in roundrect) was bad  
Bug fix SVG Import: missing end of GCode path before next SVG subpath starts  
Bug fix routing error-message during streaming (or code check) from serialform to main GUI  
  
### New in version 1.1.2.0: 
Import of SVG graphics or Image via Ctrl-V 'Paste' into GRBL-Plotter   
Copy SVG data from "URL imported SVG" to clipboard for pasting into inkscape   
Check for GRBL-Plotter update   
SVG-Import bug fix  
  
### New in version 1.1.1.0: 
Zooming into graphic  
New font ['Dot Matrix'](https://youtu.be/ip_qCQwoufw)   
Option for SVG-Import 'Process path nodes only' usefull for ['String Art'](https://youtu.be/ymWi15rvTvM)  
Support of 4th axis  
SVG-Import bug fix  
      
[Check the Wiki for further information](https://github.com/svenhb/GRBL-Plotter/wiki)  

### Program is free and you can use it at your own risk, as you understand there is no warranty of any kind
Zip folder contains ClickOnce application setup. Exe can be found in sub-folder GRBL-Plotter/bin/release.  
#### [GRBL-Plotter Vers. 1.1.4.0](GRBL-Plotter_1140_Publish.zip)  2018-03-25  

### Requirements for compiling
* VisualStudio 2015 
* DotNET 4.0
 
### Features:
* Supporting GRBL 1.1 (and 0.9 also)
* Controlling a 2nd GRBL-Hardware
* Supports 4th axis (A, B, C, U, V or W). Status and control (special GRBL version needed)
* Axis Substitution for Rotary Axis
* Surface scanning for height map creation and Autoleveling
  
  
* GCode import via file load, drag & drop or copy & paste
  - Loading GCode with optional replacement of M3 / M4 commands (helpful for 'Laser Mode' $32=1)
* SVG graphics import via file load, drag & drop (also URL) or copy & paste
* DXF graphics import via file load, drag & drop (also URL)
* Image import via file load, drag & drop (also URL) or copy & paste
* GCode generation from Text (Hershey Font)
* GCode generation from simple shape
* Several options to translate Pen Up/Down
  - controlling a Z axis
  - controlling a servo
  - controlling a laser
  - user defined commands
  - Create GCode absolute or relative (for further use as subroutine)  
    
    
* Transformation of GCodes (scale, rotation, mirror, zero-Offset)
* Supporting subroutines M98, M99 Sub-Program Call (P, L)
* GCode can be edited and saved
* User defined Buttons - GCode from text-field or file
* Joystick like control
* Export / import machine specific settings (Joystick, Buttons)
* Automatic reconnect on program start
* Recent File List (Files and URLs)
* 2-dimensional preview
* Optional usage of a WebCam with graphics overlay of the current GCode, set zero point, measure angle, zoom
* Internal variable to support probing, e.g.:
  - G38.3 Z-50		(probe toward tool length sensor, stop on contact - because of decelaration stop-pos. is not trigger-pos.)
  - G43.1 Z@PRBZ	(Offset Tool with value stored on trigger of sensor switch)
  - examine SerialForm.cs for implementation
 
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
