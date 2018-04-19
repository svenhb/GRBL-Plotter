## History
Needs to be read from the bottom up ;-)  
  
### Soon:  
DXF-Text import [Font examples](https://www.circuitousroot.com/artifice/drafting/librecad-miscellany/index.html) (not supported up to now)  
  
### 2018-04 version 1.1.5.0 to 1.1.5.4: 
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

### 2015-09 Start of GRBL-Plotter
After struggeling with an other GCode sender (just working when logged-in as admin) and missing features,
I started my own GCode sender. 
My main use is 2.5D usage for a CNC router and a plotter.  
The most disturbing points were:  
* no automatic reconnect, when starting the program
* no automatic set of the feed rate, when moving the stage the first time
* poor manual control (joystick) via user interface
