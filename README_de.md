# GRBL-Plotter
[README english](README.md)  
Ein  GRBL GCode-Sender unter windows mit DotNET 4.0 (funktioniert auch mit Windows XP)  
Geschrieben in C# VisualStudio 2015.
  
### Neu in Version 1.1.4.0: 
Import von Eagle Drill Dateien
Unterstützung von NoName USB GamePad  
Importauswahl (und Linealeinstellung) 'mm' / 'inch' im Setup  
Auskommentierung von unbekannten GCode  
  
### Neu in Version 1.1.3.0: 
Knöpfe für "Feed rate-" und "Spindle speed override" (GRBL 1.1) hinzugefügt  
Fehlerbehebung - keine Vergrößerung während Code-Streamings  
Fehlerbehebung SVG Import: Rechteck-Transformation (G3 Befehl)  
Fehlerbehebung SVG Import: fehlendes Ende im GCode bevor im SVG ein neuer Suppath beginnt  
Fehlerbehebung Weiterleitung von Fehlermeldung im serial Form zum Hauptfenster  o main GUI
 
### Neu in Version 1.1.2.0: 
Import von SVG Grafik oder Bild via Strg-V 'Paste' in GRBL-Plotter   
Kopieren von SVG Daten ins Clipboard wenn diese über eine URL geladen wurden - um diese in Inkscape zu laden   
Überprüfung auf GRBL-Plotter updates   
SVG-Import bug fix  
  
### Neu in Version 1.1.1.0: 
Zoomen in der Grafikdarstellung  
Neue Schriftart ['Dot Matrix'](https://youtu.be/ip_qCQwoufw)   
Option für SVG-Import 'Nur Pfadknoten verarbeiten' nützlich für ['String Art - Fadenbilder'](https://youtu.be/ymWi15rvTvM)  
Unsterstützung einer vierten Achse  
SVG-Import bug fix  
  
### Neu in Version 1.1.0.0:  
Oberflächenabtastung zur Erzeugung eines Höhenprofils  
Autoleveling mit Hilfe eines Höhenprofils  
Substitution für Drehachse (statt X oder Y eine Drehachse ansteuern)  
Unterprogramme M98, M99 Sub-Program Call (P, L)  
GCode Erzeugung absolut oder relative (für weitere Verwendung als Unterprogramm)  
DXF Import (Fehlend: Text und Ellipse, Spline mit mehr als 4 punkten nok)  
  
  
[Im Wiki gibt es weitere Informationen](https://github.com/svenhb/GRBL-Plotter/wiki)  

### Das Programm ist umsonst und kann auf eigene Gefahr genutzt werden, verständlicherweise gibt es keine Garantie.
Die Zip-Datei enthält die ClickOnce Setupdatei. Falls keine Installation gewünscht ist: alle nötigen Dateien liegen im Ordner GRBL-Plotter/bin/release.  
#### [GRBL-Plotter Vers. 1.1.4.0](GRBL-Plotter_1140_Publish.zip)  2018-03-25  
  
### Voraussetzung für das Kompilieren
* VisualStudio 2015 
* DotNET 4.0
 
### Funktionen:
* Unterstützung der GRBL Versionen 1.1 (und auch 0.9)  
* Ansteuerung einer zweiten GRBL-Hardware 
* Unterstützung einer vierten Achse (A, B, C, U, V or W). Status und Steuerung (spezielle GRBL Version wird benötigt) 
* Substitution für Drehachse (statt X oder Y eine Drehachse ansteuern)
* Oberflächenabtastung zur Erzeugung eines Höhenprofils für Autoleveling

* GCode Import via file load, drag & drop oder copy & paste
  - Laden des GCodes mit optionaler Ersetzung von M3 / M4 Befehlen (nützlich für 'Laser Mode' $32=1)
* SVG Grafikimport via file load, drag & drop (auch URL) oder copy & paste
* DXF Grafikimport via file load, drag & drop (auch URL)
* Bildimport via file load, drag & drop (auch URL) oder copy & paste
* GCode-Erzeugung von Text (Hershey Font)
* GCode-Erzeugung von einfachen Formen
* Verschiedene Optionen um "Stift Auf/Ab" umzusetzen
  - Steuerung der Z axis
  - Steuerung  eines Servos
  - Steuerung  eines Lasers
  - Benutzerdefinierte Kommandos
  - Erzeugt GCode in absoluten oder relativen Koordinaten (nützlich für Unterprogramme)  
  
* Transformation von GCodes (Skalierung, Drehung, Spiegeln, Nullpunkt)
* Unterprogramme M98, M99 Sub-Program Call (P, L)  
* GCode kann geändert und gespeichert werden
* Benutzerdefinierte Buttons - GCode aus Textfeld oder Datei ausführen
* Joystick-ähnliche Steuerung
* Export / import machine specific settings (Joystick, Buttons)  
* Automatische Verbindung zur GRBL-Hardware beim Programmstart
* Recent File List (Files and URLs)
* 2D Vorschau
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
![GRBL-Plotter Setup1](doc/GRBLPlotter_Setup1_de.png?raw=true "Setup1") 
  
Setup configuration 
![GRBL-Plotter Setup3](doc/GRBLPlotter_Setup3_de.png?raw=true "Setup2") 
  
Setup Custom Buttons  
![GRBL-Plotter Setup2](doc/GRBLPlotter_Setup2_de.png?raw=true "Setup3") 
  
Setup GamePad  
![GRBL-Plotter Setup3](doc/GRBLPlotter_Setup3_de.png?raw=true "Setup4") 
  
Setup virtual Joystick and colors  
![GRBL-Plotter Setup3](doc/GRBLPlotter_Setup3_de.png?raw=true "Setup5") 
  
Text import  
![GRBL-Plotter Text](doc/GRBLPlotter_Text.png?raw=true "Text conversion") 

Picture import  
![GRBL-Plotter Image](doc/GRBLPlotter_Image.png?raw=true "Image import") 

Different scaling options  
![GRBL-Plotter Scaling](doc/GRBLPlotter_scaling.png?raw=true "GCode scaling") 

Feed rate overrid for Version 0.9 and 1.1  
![GRBL-Plotter Override](doc/GRBLPlotter_override.png?raw=true "GCode override") ![GRBL-Plotter Override](doc/GRBLPlotter_override2.png?raw=true "GCode override")
