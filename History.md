## History
Needs to be read from the bottom up ;-)  
 
### 2022-04 Version 1.6.6.x 
* New form "Projector" shows the 2D-view in fix scale, to be shown on the workpiece with an projector [Machine control - Projector].
* Additional forms (e.g. setup-form or text-creation) are not pinned to the front anymore, but can be brought to front via menu-button.
* Changed layout in main GUI: Select use of Router/Plotter/Laser to control spindle speed / PWM adjustment
  
### 2022-02 Version 1.6.5.x 
* Improoved Object handling in 2D-view: added handles to selected object (figure or group) for moving, scaling, rotation.
* Adding Text to existing (formerly imported) gcode
* New setup-option "Command extension", to add new axis with travel-distance related value
 
### 2022-01 Version 1.6.4.x 
* Czech language
* Add background pattern for image-import halftone 
 
### 2021-11 Version 1.6.3.x 
* option to show path-nodes (vertices) (=gcode coordinates) of graphics in 2D view (will be switched off on each program start to save recources) - [File - Setup - Program appearance - 2D-View]
* show status of D0-D3 of grbl-Mega-5X enable and set description: [File - Setup - Program behavior - Program start - Pin description]
* new extension "knock-in nut recess" [G-Code Creation - Extensions]
* support VoidMicro controller with just two grbl-axis - not tested, feedback is welcome
* Bugfix: remove short moves
 
### 2021-10 Version 1.6.2.x 
Reworking of the camera form https://grbl-plotter.de/index.php?id=form-camera&setlang=en:
Different camera mountings:
* Fix mount above working area -> fix scaling, fix XY offset
* Mount on XY platform -> fix scaling but variable XY offset
* Mount on Z axis of XY platform -> scaling depends on Z position - sorry, could not be testet
Add quadrilateral transformation to remove distortion caused by camera angle

* Automatic fiducial correction https://grbl-plotter.de/index.php?id=fiducial-correction&setlang=en
* GCode import: take care of G20 inch, large files 100000 lines -> disable some functionality
* Graphics import: huge amount 1000 paths -> disable figureXMLs
* Text creation form - add center alignment


### 2021-09 Version 1.6.1.x 
* #214 Stream selected figure, group or define individual start, stop lines -> right click on stream-button
* #217 2D-View figure color: If Tool table is in use, replace figure colors (original from graphic) by assigned tool colors.
* 2D-View display of tiles: new option: show correct tile position in 2D-view, even if offset of tiles is 0
* #220 Add option to avoid notches in curves
 
 
### 2021-07 Version 1.6.0.x 
- Switch to Visual Studio 2019
- Tried to get rid of all of the visual studio's warnings - with moderate success
- New installer via Inno Setup
- Old: same location as application folder.
New: depending on user privileges during installation:
Administrator: "C:\ProgramData\GrblPlotter\GRBL-Plotter"
User : "C:\Users\John.Doe\AppData\Roaming\GRBL-Plotter\GRBL-Plotter"
 
### 2021-03 Version 1.5.8.x
- Improoved halftone processing 
- Probing dialog: avoid crash into touch plate by keeping "Save distance" higher than "Touch plate dimension" #196 
- Text import: insert line break after reaching certain line length (by estimating an word lengths) #197 

Bug fixes: 
- #188 don't send further grbl-comments to 3rd serial 
- #189 wrong angle of first arrow in 2D-view pen-up path 
- #191 New option for GCode-modification - Split original moves: sent pen-up/down before/after script 
- #192 Overlap paths at tile borders / Display tile borders and numbers 
- #199 "Kill Alarm" didn't worked 
 
 
### 2021-02 Version 1.5.7.x
- New option: ramp down to "Pen-down" position (when using a brush) 
- Text to GCode: Support of SVG Font files (https://gitlab.com/oskay/svg-fonts) #185 
- Code repetition: option to repeate code including header and footer #186 
- To make the scripts more variable, the pen-up/down code will be available as subroutines O97 and O98 - callable from the tool-change scripts. Then it's easier to switch from real Z movement to servo-control. 
- Option to control the servo during setup of the Servo PWM values 

Bug fixes: 
- Bug fix: Text to Gcode - allow leading space 
- Bug fix: Using Z-Axis with 'several passes': inital 'Pen up' was nok 
- Bug fix: described in #186 MainFormStreaming - Line 186 
- Improovement in Tool change script handling #184 
- Subroutines O97 and O98 will only be generated if subroutine calls are found in tool-change scripts. 
- Fixed Problem: during streaming, sometimes commands are missing. 
- Bug fix in Tool change script handling #184 
 
 
### 2021-02 Version 1.5.6.x 
- Problem: If using a servo (controlled via spindle PWM) the PWM will be set to '0' on program end M30 (because grbl will reset the parser state).
Depending on hardware setup, this will lower the pen and draw a dot.
To avoid this, the only way is to skip the M30 command.
- GampePad: add PointOfViewControllers0
- Simple Shapes: add "Round off Z" to generate 1/4 circle round off on edge of workpiece 
  

 
### 2021-01  Version 1.5.5.x    
New features: 
- Tool change scripts: adjustable delay after script 
- Improoved streaming: synchronize streaming feedback with status polling frequency 
- Option to add a frame 
- Option to multiply graphics in x in y direction 
- Added a 3rd serial com - without grbl handling [Menu - Machine control - Control 3rd serial COM] needed for #159 
- Check GCode for bad G2/3 code which causes grbl error 33  
- Added Arduino-Nano binary 'grbl_v1.1f_Servo_switch_dir_step.hex' for use with cheap nano-cnc-shield where dir and step pins are switched,
 
Features to simplify manual work: 
- Added a jog path creator - [Menu - G-Code Creation - Create jog path] 
- Added bevel / round off in [Menu - G-Code Creation - Create simple shapes] 
- Added M2 nut to [Menu - G-Code Creation - Extensions - GCode-Nut-Recess]  

### 2020-12-30 Version 1.5.4.0    
* Add Marlin support 

### 2020-12-19 Version 1.5.3.0    
* Add XML-Tag for tiles 
* Notification via email or pushbullet about streaming process [Setup - Program behavior - Notifier] 
* Improoved handling when importing large graphic files 
 
### 2020-10-22 Version 1.5.2.1    
Custom-Buttons in color, right-click for edit, predefinitions
  
### 2020-08-15 Version 1.5.0.5    
Some bug fixes  
Gerber import 
  
### 2020-07-23 Version 1.5.0.0    
Revision of the graphical import algorithms:   
* grouping by pen-color, pen-width, layer-name
* clipping, tiling [Setup - Graphics import - Path import - Clipping](issue #109)
* sort objects by shortest connections [Setup - Graphics import - Path import - General Options](issue #119)
* merge / append almost connected paths (issue #121)
* path repetition optionally path by path [Setup - Graphics import - Path import - General Options]
* hatch fill of closed paths [Setup - Graphics import - Path import - Path modifications](issue #124)
* path extension of closed path for laser cutting
* path interpretation: translate pen-width to Z-value [Setup - Graphics import - Path import - General options](issue #127)
  
New features:    
* import of CSV data (like airfoil data from: https://m-selig.ae.illinois.edu/ads/coord_database.html)
* creation of Barcode and QR-Code [G-Code Creation - Create Barcode](issue #123)
  
Improovements:    
* continuously switching of visibility of PenUp path with space-bar (issue #122)
* editor: adjustable line distance [Setup - Program appearance - 2D-View/Editor](issue #125)
* tool table: add axis A for tool change (issue #126)
* marking of the processed path 
  
    
### 2020-03-08 Version 1.3.4.0  
* Import of basic HPGL code  
* Tangential knife support for axis A,B,C or Z 
  - If Z is used, knife can be lifted with servo, using e.g. 'grbl_v1.1f_Servo.hex'  
  - 360 units/turn slows down everything - can be reduced to custom value (only usefull if gcode will be generated by graphics import)  
* Program extensions (in Menu - G-Code creation) 
  - write your own tools/scripts for gcode generation  
* Showing dimension box arround GCode in 2D view  
* New sort option for groups - sort by object dimension (area used by objects)
  

### 2020-01-07 Version 1.3.3.0
* Path 'simulation' #96
  - automatically go through G-Code lines and show position in 2D view
* Optimized joystick control
  - Calculates speeds depending on max speed from $11x settings every 50 ms
  - Speed / feedrate for smallest joystick deflection can be set, applied with 200 ms interval
* Serial connection
  - automatic reset if expected status reports are missing
  - automatic disconnect if receiving-error appears
* 2D-View 
  - with middle mouse button: move complete view or selection #93
  - new option "Don't change pen width on zooming"
* Some bug fixes
  - Group selection for code folding
  - using new FCTB release #95

### 2019-12-20 Version 1.3.2.6
* Add arabic and japanese language
* Bug fix save gcode - using new FastColoredTextBox.dll

### 2019-12-07 Version 1.3.2.4
* Update
  - #84 add setting to DXF-Import
  - #85 offset marked figure by moving in 2d view, while middle mouse button is pressed (after moving, confirm translation via menu)
  - #88 marked figure can be moved up/down in GCode editor to change order
  - enable/disable 2D-view of ruler, info, Pen-up-path
  - Run GCode or script after Reset (Setup - Flow Control - Behavior after Reset)
  - Add key-words for custom-buttons, hot-keys, Game-pad: #FEEDHOLD, #RESUME, #KILLALARM, #RST, #HRST
* Bug fixes 
  - #85 try to load from locked file 
  - #89 multiple passes and laser 
  - misc 

### 2019-11-24 version 1.3.2.3
* Bug fix DXF Import - Ellipse:
Fix DXFLib.dll, Add Ellipse support
* Bug fix DXF Import - Spline
Adapt import-code from Inkscape
* Probing window: add storage of checkbox states
 
### 2019-11-10 version 1.3.2.0
* Add probing window with edge / corner finder, center finder and tool length correction
* Increased number of custom buttons to 32

### 2019-10-29 version 1.3.1.0
* Add a rough localization for spanish, french, russian and chinese language
The translation was done with google translator, so maybe not quite right. In addition, the texts are sometimes too long and do not fit into the controls.
* Bug fix saving G-Code
* Bug fix Game pad support

### 2019-10-03 version 1.3.0.0
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

#### Attention: some incompatibilities:  
* new file format for csv tool table now ',' comma separated instead of ';' semicolon  
* new file locations for examples, fonts, scripts, tools

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
