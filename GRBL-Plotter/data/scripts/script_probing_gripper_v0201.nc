(Move to stationary length sensor at machine coordinates (HOMING NEEDED!))
(Move down 50, stop when switch triggers, set new z-coordinate)
G90 G0			(absolute distance mode, rapid move)
G53 Z-50			(move in machine coordinates to save height)
G53 X-146.6 Y-74	(move in machine coordinates to stationary switch/tool length sensor)
G91 G1 F500 	(relative distance mode, feed mode)
G38.3 Z-10		(probe toward tool length sensor, stop on contact)
G43.1 Z#PRBZ	(Offset Tool)
G53 G0 Z-33.5		(move in machine coordinates to save height)
G92 Z0
G53 Y-85
G90 G1 F2000