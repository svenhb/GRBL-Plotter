# GRBL-Plotter
[README deutsch](README_de.md)  
A GCode sender for GRBL under windows, using DotNET 4.0 (should also work with Windows XP)  
Written in C# VisualStudio 2015.  
If you like GRBL-Plotter, show it to me with a small donation :-) [![Donate](https://www.paypalobjects.com/en_US/DE/i/btn/btn_donateCC_LG.gif)](https://www.paypal.com/cgi-bin/webscr?cmd=_s-xclick&hosted_button_id=PVBK8U866QNQ6)   
   
### New in Version 1.3.2.x   
* Bug fix DXF Import - Ellipse:
Fix DXFLib.dll, Add Ellipse support
* Bug fix DXF Import - Spline
Adapt import-code from Inkscape
* Probing window: add storage of checkbox states
* Add probing window with edge / corner finder, center finder and tool length correction
* Increased number of custom buttons to 32
* Add a rough localization for spanish, french, russian and chinese language
The translation was done with google translator, so maybe not quite right. In addition, the texts are sometimes too long and do not fit into the controls.
* Bug fix saving G-Code
* Bug fix Game pad support
   
### New in Version 1.3.0.x
#### New feature 'use cases':
* select a 'use-case' when importing DXF or SVG
* preset import options, pen up/down definitions, Z deepth, etc.
* use it for machine-selection, material-selection or tool-selection
* example [Using a pen and laser together](https://youtu.be/Ebe2kFlE058)
#### Improoved 'grouping'
* Paths from imports by DXF or SVG can be grouped (by color or layer)
* Groups can be selected, transformed, deleted
#### Misc
* process dashed lines from DXF or SVG
* Add a logger
* extend error handling on SVG import
* Laser tools to optimize parameters e.g.  [Scan Z to find laser focus](https://youtu.be/blhaZ8rb2ec)

#### Attentions: some incompatibilities:  
* new file format for csv tool table now ',' comma separated instead of ';' semicolon  
* new file locations for examples, fonts, scripts, tools
   
### New in Version 1.2.5.x:
* Transform of all or selected code
* ___simple___ cutter radius compensation
* option for final Z depth in several passes
* update Hershey fonts from evil mad scientist (before their new release)
* New option for text creation: connect characters within a word
* Hot keys: assign direct commands / scripts to keys    
   
### New in version 1.2.4.x:  
* extend override buttons in GUI for grbl version 1.1
* bug fix drag tool compensation: correct start point of first line in object
* For 2D view: mapping rotary axis to X or Y axis
* Resizeable Joystick control (Setup - Joystick)
* Resizable editor
* Ruler scale also for negative dimension
* Mirroring of Rotary axis
* New menu 'view'. E.g. switch between traditional object view or view to the complete machine area  
* Highlight selected path (e.g. for deletion)
* Copy gcode path to background as a landmark for positions applying new gcode (check context menu on 2D view)
* New window to handle coordinate systems G54, etc (Menu Machine control - Coordinate system)
* Code creation with usage of Z and spindle - add variable delay after spindle start (to be sure spindle really started)    
* Add extensions 'cnc', 'gcode' as possible gcode source
* New menu item 'Remove any Z movement' in GCode Transform  
* Replace G92 command by G10 L20 P0
* Remove hotkey assignment with "Strg" via form-designer (Strg / Ctrl Problem)
* Some code clean up
  
### Check [History](https://github.com/svenhb/GRBL-Plotter/blob/master/History.md)  
      
Check the [Wiki](https://github.com/svenhb/GRBL-Plotter/wiki) for further information   

### Program is free and you can use it at your own risk,<br>as you understand there is no warranty of any kind
Zip folder contains ClickOnce application setup. Exe can be found in sub-folder GRBL-Plotter/bin/release.
#### [GRBL-Plotter Vers. 1.3.2.3](https://github.com/svenhb/GRBL-Plotter/releases/latest)  2019-11-24    

### Requirements for compiling
* VisualStudio 2015 
* DotNET 4.0
 
### Feature list:
#### Import/Export:  
* Several options to translate Pen Up/Down
  - controlling a Z axis
  - controlling a servo
  - controlling a laser
  - user defined commands
  - Create GCode absolute or relative (for further use as subroutine)  
* Ruler and import units can be switched between mm or inch
* GCode import via file load, drag & drop or copy & paste
  - Loading GCode with optional replacement of M3 / M4 commands (helpful for 'Laser Mode' $32=1) 
* SVG graphics import via file load, drag & drop (also URL) or copy & paste - tested with [Inkscape](https://inkscape.org/de/) generated SVGs 
  - optional resize to fixed size
  - optional output of nodes only (generating drill holes for string art [Video 'String Art'](https://youtu.be/ymWi15rvTvM)  )
  - optional sorting of paths by used color
  - optional tool change
  - if text needs to be imported, convert text into path first
* DXF graphics import via file load, drag & drop (also URL) - tested with [LibreCAD](http://librecad.org/cms/home.html) generated DXFs 
  - few entities are missing
* Drill file import via file load, drag & drop
* Image import via file load or drag & drop
* GCode can be edited and saved
* Recent File List (Files and URLs)
* Export / import machine specific settings (Joystick, Buttons)
  
#### GCode creation:
* Create Text
  - own created 'Dot Matrix' font [Video 'Dot Matrix'](https://youtu.be/ip_qCQwoufw) 
* Create simple shapes
    
#### GCode manipulation:  
* Transformation of GCodes (scale, rotation, mirror, zero-Offset)  
any A,B,C,U,V,W commands will stay in place.
* Transformation via camera teaching
* Axis Substitution for Rotary Axis
  
#### Machine control:  
* Individual commands via user defined Buttons  
* Joystick like control in user interface  
* support of no-name USB GamePad  
* Optional usage of a WebCam with separate coordinate system: graphics overlay of the current GCode, set zero point, measure angle, zoom, teaching  
  - Shape recognition for easier teach-point (fiducial) assignment  
  - Transforming GCode with camera aid, to match drill file with PCB view [Wiki 'PCB drilling'](https://github.com/svenhb/GRBL-Plotter/wiki/PCB-drilling)   
  
#### Flow control:
* Supporting subroutines M98, M99 Sub-Program Call (P, L)
* Internal variable to support probing, e.g.:
  - G38.3 Z-50		(probe toward tool length sensor, stop on contact - because of deceleration stop-pos. is not trigger-pos.)
  - G43.1 Z@PRBZ	(Offset Tool with value stored on trigger of sensor switch)
  - examine SerialForm.cs for implementation
  
#### GRBL:  
* Automatic reconnect on program start  
* Supporting GRBL 1.1 (and 0.9 also)
* Supporting new GRBL 1.1 features
  - Jogging
  - Feed rate override
  - Spindle speed override
  - real time display GRBL states (in COM CNC window)
* Check limits of GRBL setup - max. STEP frequency and min. FEED rate in COM CNC window  
* Controlling a 2nd GRBL-Hardware
* Supports 4th axis (A, B, C, U, V or W). Status and control (special GRBL version needed)

 
### My test bed
On my german homepage:
[my XYZ platform](http://svenhb.bplaced.net/?CNC___Plotter) 

### GRBL-Plotter in tool change action
[![Import an image](https://i9.ytimg.com/vi/GGtdwYdZWi8/mq2.jpg?sqp=COypi98F&rs=AOn4CLAbkofKlCN1cepOQkGvpG6YlnRwrQ)](https://youtu.be/GGtdwYdZWi8) 

### Screenshots
Main GUI
![GRBL-Plotter GUI](doc/GRBLPlotter_GUI.png?raw=true "Main GUI") 

Separate serial COM window(s) - one for the CNC, one for the tool changer (or 4th axis)  
![GRBL-Plotter COM interface](doc/GRBLPlotter_COM2.png?raw=true "Serial connection") ![2nd GRBL control](doc/GRBLPlotter_Control_COM2.png?raw=true "Serial connection")

Setup import / GCode conversion  
![GRBL-Plotter Setup1.1](doc/GRBLPlotter_Setup1_1_en.png?raw=true "Setup1.1") 
Setup import / GCode conversion  
![GRBL-Plotter Setup1.2](doc/GRBLPlotter_Setup1_2_en.png?raw=true "Setup1.2") 
Setup import / GCode conversion  
![GRBL-Plotter Setup1.3](doc/GRBLPlotter_Setup1_3_en.png?raw=true "Setup1.3") 
   
Setup main tool table  
![GRBL-Plotter Setup2](doc/GRBLPlotter_Setup2_en.png?raw=true "Setup2")  
   
Setup main configuration  
![GRBL-Plotter Setup3](doc/GRBLPlotter_Setup3_en.png?raw=true "Setup3")  
  
Setup user defined buttons  
![GRBL-Plotter Setup4](doc/GRBLPlotter_Setup4_en.png?raw=true "Setup4")  
  
Setup GamePad  
![GRBL-Plotter Setup5](doc/GRBLPlotter_Setup5_en.png?raw=true "Setup5")  
  
Setup virtual Joystick  
![GRBL-Plotter Setup6](doc/GRBLPlotter_Setup6_en.png?raw=true "Setup6")  
  
Setup shape recogniton for fiducial camera     
![GRBL-Plotter Setup7](doc/GRBLPlotter_Setup7_en.png?raw=true "Setup7")  
  
Text import  
![GRBL-Plotter Text](doc/GRBLPlotter_Text.png?raw=true "Text conversion")  
  
Picture import  
![GRBL-Plotter Image](doc/ImageImport/ImageImport1.png?raw=true "Image import")  
  
Different scaling options  
![GRBL-Plotter Scaling](doc/GRBLPlotter_scaling.png?raw=true "GCode scaling")  

Feed rate overrid for Version 0.9 and 1.1  
![GRBL-Plotter Override](doc/GRBLPlotter_override.png?raw=true "GCode override") ![GRBL-Plotter Override](doc/GRBLPlotter_override2.png?raw=true "GCode override")
