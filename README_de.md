# GRBL-Plotter
[README english](README.md)  
Ein  GRBL GCode-Sender unter windows mit DotNET 4.0 (funktioniert auch mit Windows XP)  
Geschrieben in C# VisualStudio 2015.

### Neu in Version 1.1.0.0:  
Oberflächenabtastung zur Erzeugung eines Höhenprofils  
[Autoleveling](https://github.com/svenhb/GRBL-Plotter/wiki/Autoleveling) mit Hilfe eines Höhenprofils  
Substitution für [Drehachse](https://github.com/svenhb/GRBL-Plotter/wiki/Drehachse) (statt X oder Y eine Drehachse ansteuern)  
[Unterprogramme](https://github.com/svenhb/GRBL-Plotter/wiki/Unterprogramme) M98, M99 Sub-Program Call (P, L)  
GCode Erzeugung absolut oder relative (für weitere Verwendung als Unterprogramm)  
DXF Import (Fehlend: Text und Ellipse, Spline mit mehr als 4 punkten nok)  
  
### Neu in Version 1.0.3.0:  
Zweisprachig - deutsch, englisch  
Überarbeitung der Kameraunterstützung
    
### Neu in Version 1.0.2.0:
Unterstützung der neuen GRBL Version 1.1 (und auch der Version 0.9)  
Export / Import maschinen-spezifischer Einstellungen (Joystick, Buttons)  
Erzeugung einfacher Formen (Kreis, Rechteck auch als Tasche)  
Einlesen von GCode mit optionaler Ersetzung von M3 / M4 Befehlen (hilfreich für den neuen 'Laser Mode' $32=1)  

[Im Wiki gibt es weitere Informationen](https://github.com/svenhb/GRBL-Plotter/wiki)  

### Das Programm ist umsonst und kann auf eigene Gefahr genutzt werden, verständlicherweise gibt es keine Garantie.
Die Zip-Datei enthält die ClickOnce Setupdatei. Falls keine Installation gewünscht ist: alle nötigen Dateien liegen im Ordner GRBL-Plotter/bin/release.  
#### [GRBL-Plotter Vers. 1.1.0.0](GRBL-Plotter_1100_Publish.zip)  2017-08-11  
  
### Voraussetzung für das Kompilieren
* VisualStudio 2015 
* DotNET 4.0
 
### Funktionen:
* Unterstützung der GRBL Versionen 1.1 (und auch 0.9)  
* Export / import machine specific settings (Joystick, Buttons)  
* Ansteuerung einer zweiten GRBL-Hardware 
* automatischer Werkzeugwechsel (mit zweiter GRBL-Hardware)
* Benutzerdefinierte Buttons - GCode aus Textfeld oder Datei ausführen
* Joystick-ähnliche Steuerung
* Automatische Verbindung zur GRBL-Hardware beim Programmstart
* Recent File List (Files and URLs)
* 2D Vorschau
* Import/Erzeugung und Umwandlung in GCode 
  - aus SVG Grafik
  - aus Text (in Hershey Font)
  - Umwandlung der Höheninformation in Bewegung der Z-Achse (Fräse) oder Fräsmotor ein/aus (Laser) oder Fräser-Drehzahl (RC-Servo PWM) 
* GCode kann geändert und gespeichert werden
* Drag & Drop von GCode (*.nc) und SVG (*.svg) Dateien
* Drag & Drop (oder Copy & Paste) von Browserlinks auf SVG-Dateien (funktioniert nur unter Chrome) z.B. von https://openclipart.org/
* Transformation von GCodes (Skalierung, Drehung, Spiegeln, Nullpunkt)
* Optionale Nutzung einer WebCam mit Grafikeinblendung des GCodes, Nullpunktsetzung, Winkelmessung, Zoom
* Interne Variablen um Probing zu unterstützen:
  - G38.3 Z-50		(probe toward tool length sensor, stop on contact - because of decelaration stop-pos. is not trigger-pos.)
  - G43.1 Z@PRBZ	(Offset Tool with value stored on trigger of sensor switch)
  - examine SerialForm.cs for implementation

### Meine Testumgebung
[Meine XYZ platform](http://svenhb.bplaced.net/?CNC___Plotter)

### Video: GRBL-Plotter beim 'Werkzeugwechsel'
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
