; Move to stationary length sensor at machine coordinates (HOMING NEEDED!)
; Move down 50, stop when switch triggers, set new z-coordinate
G90 G0			(absolute distance mode, rapid move)
G53 Z-1			(move in machine coordinates to save height)
G53 X-158 Y-15	(move in machine coordinates to stationary switch/tool length sensor)
G91 G1 F 500 	(relative distance mode, feed mode)
G38.3 Z-50		(probe toward tool length sensor, stop on contact)
; G53 G1 Z#PRBZ	(move back to probing result)
G43.1 Z#PRBZ	(Offset Tool)
G53 G0 Z-1		(move in machine coordinates to save height)
G53 Y-35