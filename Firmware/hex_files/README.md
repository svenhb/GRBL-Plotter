## grbl for Arduino Uno / Nano (Atmega328)
-------------------------------------------
All made from latest https://github.com/gnea/grbl (2019 = original)
with changes for servo-support from https://github.com/cprezzi/grbl-servo (config.h, cpu_map.h, spindle_control.c)
and servo-off position.

Pin-swap needed for Arduino-Nano CNC-shield V4 with bug

Compiled, using following options:
Config.	Options
- A	original
- B	original and servo
- C	original and servo and servo-off inverted
  
- E	pin-swap (Pin-swap needed for Arduino-Nano CNC-shield V4 with bug)
- F	pin-swap and servo
- G	pin-swap and servo and servo-off inverted
  
- I	COREXY
- J	COREXY and servo
- K	COREXY and servo and servo-off inverted

Send $I to read out uploaded version, response like:
```
[VER:1.1h.G_240107:]
          ^
       Config G = pin-swap and servo and servo-off inverted
```

## grbl for Arduino Mega2560
---------------------------------------------
grbl-Mega-5X-v1.2d.20211118.hex
from https://github.com/fra589/grbl-Mega-5X

grbl-Mega-5X-v1.1p.20201228_end_stops.hex
compiled with hard-limits by 78Matteo78 
