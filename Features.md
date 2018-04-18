

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
  - if text needs to be imported, convert text into path first
* DXF graphics import via file load, drag & drop (also URL) - tested with [LibreCAD](http://librecad.org/cms/home.html) generated DXFs 
  - few entities are missing
* Drill file import via file load, drag & drop
* GCode can be edited and saved
* Recent File List (Files and URLs)
* Export / import machine specific settings (Joystick, Buttons)
  
#### GCode creation:
* Create Text
* Create simple shapes
    
#### GCode manipulation:  
* Transformation of GCodes (scale, rotation, mirror, zero-Offset)  
* Transformation via camera teaching
* Axis Substitution for Rotary Axis
  
#### Machine control:  
* Individual commands via user defined Buttons  
* Joystick like control in user interface  
* support of no-name USB GamePad  
* Optional usage of a WebCam with separate coordinate system: graphics overlay of the current GCode, set zero point, measure angle, zoom, teaching  
  - Shape recognition for easier teach-point (fiducial) assignment  
  - Transforming GCode with camera aid, to match drill file with PCB view [PCB drilling](https://github.com/svenhb/GRBL-Plotter/wiki/PCB-drilling)   
  
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
