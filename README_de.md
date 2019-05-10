# GRBL-Plotter
[README english](README.md)  
Ein  GRBL GCode-Sender unter windows mit DotNET 4.0 (funktioniert auch mit Windows XP)  
Geschrieben in C# VisualStudio 2015.  
Wenn dir GRBL-Plotter gefällt, zeige es mir durch eine kleine Spende :-) [![Donate](https://www.paypalobjects.com/de_DE/DE/i/btn/btn_donateCC_LG.gif)](https://www.paypal.com/cgi-bin/webscr?cmd=_s-xclick&hosted_button_id=WK8ATWHC872JU)   
  
### Neu in version 1.2.4.x:  
* Erweiterung der Override Funktionen für grbl version 1.1
* Fehlerbehbung für drag tool compensation: Korrektur der Startposition des ersten Punktes eines Objekts
* Abbildung einer Drehachse auf X oder Y Achse in der 2D Ansicht
* Neuer Menupunkt 'Ansicht'. Wechsle zwischen den Ansichten des gesamten Arbeitsbereiches oder des Codebereiches 
* Hervorhebung des selektierten Pfaed (z.B. zum Löschen)
* Kopiere den aktuellen Code in den Hintergrund um Orientierungspunkte zum Anfahren zu erhalten, an welchen neuer GCode ausgeführt werden soll (Kontextmenu 2D Ansicht)
* Neues Fenster um mit Koordinatensystemen G54 zu arbeiten (Menu Machine - Koordinatensysteme)
* Codeerzeugung mit Z-Achse und Spindle - Hinzufügen einer Zeitverzögerung nach Einschalten der Spindle (um sicher zu sein dass diese läuft)    
* Zusätzliche Dateiendungen 'cnc', 'gcode' als mögliche GCode-Quellen 
* Neuer Menupunkt 'Entferne jede Z Bewegung' in GCode transformieren  
* Ersetzung vom G92 Befehl durch G10 L20 P0
* Entfernung der HotKey Zuordnung "Strg" im Formdesigner (Strg / Ctrl Problem)
* Some code clean up
  
### Neu in Version 1.2.3.x:
* 1.2.3.9 Add 4 more custom buttons, status of feed rate and spindle speed in main GUI
* 1.2.3.8 Bug fixes: wrong offset, caused by incomplete dimension calculation, svg import scaling (96,72 dpi); Add svg dpi selection in setup (96 or 72)
* 1.2.3.3 Z-adaption also for G0 (only to pos. values); add 3rd decimal to Surface scan, Max. depth
* 1.2.3.2 Bugfix in Z-Wert Korrektur, wenn eine Heightmap verwendet wird
* 1.2.3.1 Removed feedback-loop. Save last spindle speed setting from GUI
* Benutzerdefinierbare [hotkeys](https://github.com/svenhb/GRBL-Plotter/wiki/keyboard-control)   
* Surface Scan mit [Z-Probing](https://github.com/svenhb/GRBL-Plotter/wiki/Surface-scan#Z-Value-from-DIY-Control-interface) Wert vom DIY-Control Interface
* Wiederaufnahme eines pausierten Jobs nach Programneustart
* Einige Fehlerbehebungen und Verbesserungen
  
### Neu in Version 1.2.2.x:
* Steuerung über  [Keyboard](https://github.com/svenhb/GRBL-Plotter/wiki/keyboard-control)   
* Verbesserter Bildimport
  - Glättung der Aussenkontour
  - Schrumpfen der Aussenkontour um Stiftbreite zu kompensieren  
  
### Neu in Version 1.2.1.x:
* Verbesserter Bildimport - check [wiki](https://github.com/svenhb/GRBL-Plotter/wiki/Bild-import)
  - Neue Filter für einfache Farbersetzung
  - Erzeugung einer Aussenkontur um wellige Ränder zu vermeiden
  
### Neu in Version 1.2.0.x:  
* Werkzeugliste statt Farbpalette (inkl. Werkzeugwechselpositionen (bzw. Wasserfarben Position))  
* Schleppwerkzeugkompensation (um den Offset der Pinselspitze zu kompensieren)  
* Automatisches Einfügen eines Unterprogramms (um die Farbe eines Pinsels 'nachzutanken'  - [Video](https://youtu.be/3LHnGV8jKIs))  
* Anzeige des max. Arbeitsbereiches, Alarm bei Überschreitung  
* Reduzierung der CPU-Last durch Anzeige eines Hintergrundbildes statt realtime Anzeige während des Streamens  
* Empfang von Steuerbefehlen über eine serielle Schnittstelle für DIY-Steuerungen  
  
### Neu in Version 1.1.6.x: 
* 1.1.6.4 Höhenprofil Manipulation und Export als STL und X3D  [X3D Beispiel](http://svenhb.bplaced.net/?CNC___GRBL-Plotter___Hoehenprofil)  
* 1.1.6.3 Bug fix in surface scan   
* 1.1.6.2 Fixed: nach SVG Import wurde die erste 'pen-up' Bewegung als 'pen-down' angezeigt   
* 1.1.6.1 Import von [maker.js](https://maker.js.org/demos/) generiertem Code   
  - Wähle .DXF oder .SVG aus, 'Generate', dann 'copy the text above'  
  - den Code in GRBL-Plotter via Ctrl-V einfügen   
* DXF-Text import [Font Beispiele](https://www.circuitousroot.com/artifice/drafting/librecad-miscellany/index.html) 
  - Fonts einfach erweitern durch kopieren von LFF Dateien in den Unterordner fonts  
  - Information über die benutzen Fonts [Fonts](https://github.com/svenhb/GRBL-Plotter/blob/master/GRBL-Plotter/fonts/README.md)  
    
### Neu in Version 1.1.5.0: 
Die Kamera nutzt nun ihr eigenes Koordinatensystem G59 (gegenüber dem Default-Koordinatensystem G54). Achtung! Vor Ausführung des GCodes muss wieder auf das Default-Koordinatensystem G54 umgeschaltet werden!  
GCode Transformierung mit Hilfe der Kamera, um die Bohrdatei auf die Platine zu projezieren [Platinen bohren](https://github.com/svenhb/GRBL-Plotter/wiki/Platinen-bohren)   
Formenerkennung um das Zentrieren von Passmarken zu erleichtern  
Neue Importoption 'Code wiederholen' um Pfade mehrfach abzufahren (Laserschneiden mit schwachem Laser)  
Überprüfung der GRBL Limits - max. STEP Frequenz und min. FEED rate im COM CNC Fenster  
    
### Neu in Version 1.1.4.0: 
Import von Eagle Drill Dateien
Unterstützung von NoName USB GamePad  
Importauswahl (und Linealeinstellung) 'mm' / 'inch' im Setup  
Auskommentierung von unbekannten GCode  
  
  
[Im Wiki gibt es weitere Informationen](https://github.com/svenhb/GRBL-Plotter/wiki)  

### Das Programm ist umsonst und kann auf eigene Gefahr genutzt werden, verständlicherweise gibt es keine Garantie.
Die Zip-Datei enthält die ClickOnce Setupdatei. Falls keine Installation gewünscht ist: alle nötigen Dateien liegen im Ordner GRBL-Plotter/bin/release.  
#### [GRBL-Plotter Vers. 1.2.4.8](https://github.com/svenhb/GRBL-Plotter/releases/latest)  2019-05-10    
  
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

Setup Import / GCode Konvertierung  
![GRBL-Plotter Setup1.1](doc/GRBLPlotter_Setup1_1_de.png?raw=true "Setup1.1") 
Setup Import / GCode Konvertierung  
![GRBL-Plotter Setup1.2](doc/GRBLPlotter_Setup1_2_de.png?raw=true "Setup1.2") 
Setup Import / GCode Konvertierung  
![GRBL-Plotter Setup1.3](doc/GRBLPlotter_Setup1_3_de.png?raw=true "Setup1.3") 
  
Setup Werkzeugwechsel  
![GRBL-Plotter Setup2](doc/GRBLPlotter_Setup2_de.png?raw=true "Setup2") 
  
Setup Konfiguration 
![GRBL-Plotter Setup3](doc/GRBLPlotter_Setup3_de.png?raw=true "Setup3") 
  
Setup benutzerdefinierte Befehle  
![GRBL-Plotter Setup4](doc/GRBLPlotter_Setup4_de.png?raw=true "Setup4") 
  
Setup GamePad  
![GRBL-Plotter Setup5](doc/GRBLPlotter_Setup5_de.png?raw=true "Setup5") 
  
Setup virtual Joystick and colors  
![GRBL-Plotter Setup6](doc/GRBLPlotter_Setup6_de.png?raw=true "Setup6") 
  
Formenerkennung für Passmarken       
![GRBL-Plotter Setup7](doc/GRBLPlotter_Setup7_de.png?raw=true "Setup7") 
    
Text Import  
![GRBL-Plotter Text](doc/GRBLPlotter_Text.png?raw=true "Text conversion") 

Picture Import  
![GRBL-Plotter Image](doc/GRBLPlotter_Image.png?raw=true "Image import") 

Different scaling options  
![GRBL-Plotter Scaling](doc/GRBLPlotter_scaling.png?raw=true "GCode scaling") 

Feed rate overrid for Version 0.9 and 1.1  
![GRBL-Plotter Override](doc/GRBLPlotter_override.png?raw=true "GCode override") ![GRBL-Plotter Override](doc/GRBLPlotter_override2.png?raw=true "GCode override")
