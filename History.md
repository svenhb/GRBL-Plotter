## History
Needs to be read from the bottom up ;-)  

### 2019-07-05 version 1.2.5.x:
* Transform of all or selected code
* ___simple___ cutter radius compensation
* option for final Z depth in several passes
* update Hershey fonts from evil mad scientist (before their new release)
* New option for text creation: connect characters within a word
* Hot keys: assign direct commands / scripts to keys    

### 2019-02 to 04 version 1.2.4.x:  
* 1.2.4.8 Extend override buttons in GUI for grbl version 1.1
* 1.2.4.8 Bug fix drag tool compensation: correct start point of first line in object
* 1.2.4.7 Optional sending of code when pausing or stopping gcode-streaming
* 1.2.4.7 Selectable polling frequency for digital read out
* 1.2.4.7 Use 255 byte buffer for Mega2560
* 1.2.4.6 Bug fix Control coordinate system (dot / comma)
* 1.2.4.5 Add Arduino Uno/Nano hex vor servo support 
* 1.2.4.5 Add Mega2560 hex vor 4th / 5th axis support 
* 1.2.4.4 Add axis name to virtual joystick
* 1.2.4.4 Automatic display of additional axis controls and read-outs for A,B,C (not tested with real hardware up to now)
* 1.2.4.4 Save last position extended to A,B,C
* 1.2.4.4 Bugfix creating height map (export x3d, hidden buttons)
* 1.2.4.4 Bugfix support of grbl version 0.9
* 1.2.4.3 2D view of rotary axis (mapped to X or Y axis)
* 1.2.4.3 save and restore of form size
* 1.2.4.2 Resizable editor
* 1.2.4.1 Ruler scale also for negative dimension
* 1.2.4.1 Resizeable Joystick control (Setup - Joystick)
* 1.2.4.1 Mirror of Rotary axis
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
  
### 2019-01 version 1.2.3.x:
* 1.2.3.9 Add 4 more custom buttons, status of feed rate and spindle speed in main GUI
* 1.2.3.8 Bug fixes: wrong offset, caused by incomplete dimension calculation, svg import scaling (96,72 dpi); Add svg dpi selection in setup (96 or 72)
* 1.2.3.3 Z-adaption also for G0 (only to pos. values); add 3rd decimal to Surface scan, Max. depth
* 1.2.3.2 Bugfix in Z-Value correction when applying height map
* 1.2.3.1 Removed feedback-loop. Save last spindle speed setting from GUI
* User definable [hotkeys](https://github.com/svenhb/GRBL-Plotter/wiki/keyboard-control) 
* Surface Scan with [Z-Probing](https://github.com/svenhb/GRBL-Plotter/wiki/Surface-scan#Z-Value-from-DIY-Control-interface) value from DIY-Control interface 
* Restart paused job after program restart 
* some bug fixes and improvements
 
### 2018-12 version 1.2.2.x: 
* Control via [Keyboard](https://github.com/svenhb/GRBL-Plotter/wiki/keyboard-control) 
* Improved import of images 
  - check wiki 
  - smoothing of outline contour
  - shrink contour to compensate pen width


### 2018-11 version 1.2.1.x:
* Improved import of images - check [wiki](https://github.com/svenhb/GRBL-Plotter/wiki/Image-import)
  - New filters for easy color quantization
  - Create outline contour to avoid wavy edges
  
### 2018-09 version 1.2.0.x:
* Tool table instead of color palette (incl. tool exchange positions (water color pos.))  
* drag tool compensation (to compensate offset when drawing with a brush)   
* automatic subroutine insertion (to refresh brush after drawing a certain distance)  
* Show machine working area, alert on exceed  
* Reduce CPU load by showing background picture instead of real object path during streaming   
* Receive commands via serial port for DIY control pad / pendant  
  
### 2018-04 version 1.1.6.0:  
* Add height map manipulation and export to STL, X3D [X3D example](http://svenhb.bplaced.net/?CNC___GRBL-Plotter___Hoehenprofil)  
* DXF-Text import, check [Font examples](https://www.circuitousroot.com/artifice/drafting/librecad-miscellany/index.html)  using libreCAD fonts  
  - Just extend fonts by copying LFF files to sub folder fonts  
  - Information about used fonts [Fonts](https://github.com/svenhb/GRBL-Plotter/blob/master/GRBL-Plotter/fonts/README.md)  
* Revision of Text creation dialog, supporting libreCAD fonts  
* Hide 'PenUp' path in 2D view by pressing the space bar.  
* Bug fixes:
  - Win7 zooming in 2D view not possible
  - Paste code via Ctrl-V was disabled  
* Import copied code from [maker.js](https://maker.js.org/demos/)  
  - Just select .DXF or .SVG, press 'Generate', then 'copy the text above'  
  - then paste the code into GRBL-Plotter  
   
### 2018-04 version 1.1.5.0 to 1.1.5.4: 
Unfortunatly I get confused with some key and mouse events, which makes some bug fixes needed. 
* For the camera view now a seperate coordinate system G59 will be used. Attention! Switch back to default coordinate system G54 before starting any GCode!  
* Transforming GCode with camera aid, to match drill file with PCB view [PCB drilling](https://github.com/svenhb/GRBL-Plotter/wiki/PCB-drilling)   
* Shape recogniton for easier teachpoint (fiducial) assignment  
* New import option 'Repeat code' to repeat paths (laser cutting with weak laser)  
* Check limits of GRBL setup - max. STEP frequency and min. FEED rate in COM CNC window  
    
### 2018-03 version 1.1.4.0: 
* Add import of Eagle Drill file  
* Add support of noname USB GamePad for control of XY stage   
* Add import (and ruler) selection 'mm' / 'inch' in setup  
* Comment-out unknown GCode  
 
### 2018-02 version 1.1.3.0: 
* Add feed rate and spindle speed override buttons (GRBL 1.1) to main GUI  
* Bug fix in main GUI: no zooming during streaming - disabled background image (perhaps performance in Win-XP is affected)  
* Bug fix SVG Import: rectangle transform (G3 in roundrect) was bad  
* Bug fix SVG Import: missing end of GCode path before next SVG subpath starts  
* Bug fix routing error-message during streaming (or code check) from serialform to main GUI  
  
### 2017-11 version 1.1.2.0: 
* Import of SVG graphics or Image via Ctrl-V 'Paste' into GRBL-Plotter   
* Copy SVG data from "URL imported SVG" to clipboard for pasting into inkscape   
* Check for GRBL-Plotter update   
* SVG-Import bug fix  
  
### 2017-10 version 1.1.1.0: 
* Zooming into graphic  
* New font ['Dot Matrix'](https://youtu.be/ip_qCQwoufw)   
* Option for SVG-Import 'Process path nodes only' usefull for ['String Art'](https://youtu.be/ymWi15rvTvM)  
* Support of 'real' 4th axis (other hardware and grbl version needed) 
* SVG-Import bug fix  
 
### 2017-08 version 1.1.0.0:
* Surface scanning to generate height profile
* Autoleveling using height profile
* Axis substitution: using X or Y axis as rotary axis
* Supporting Sub routine M98, M99
* Generating GCode in absolute or relative coordinates (good for sub routines)
* DXF-Import (missing: text, ellipse)
 
### 2017-01 version 1.0.1.1:
* Support GRBL Vers. 1.1 incl. Override und Jogging
* Import/Export GUI settings (Joystick, Buttons)
* Control window for 2nd GRBL hardware
  
### 2016-07:
* Controlling a 2nd GRBL hardware for tool exchange or 4th axis
* SVG-Import: assigning SVG colors to tool numbers
* Generating tool exchange command (M6 Tx) during SVG-Import
* Gcode streaming: replace tool change commands by user defined scripts
  
### 2015-09 Start of GRBL-Plotter
After struggeling with an other GCode sender (just working when logged-in as admin) and missing features,
I started my own GCode sender. 
My main use is 2.5D usage for a CNC router and a plotter.  
The most disturbing points were:  
* no automatic reconnect, when starting the program
* no automatic set of the feed rate, when moving the stage the first time
* poor manual control (joystick) via user interface
