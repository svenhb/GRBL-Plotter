; Move down 10, stop when switch triggers, set new z-coordinate
G91 G1 F600 	(relative distance mode, feed mode)
G38.3 Z-30		(probe toward workpiece, stop on contact)
G90 
G53 G1 Z#PRBZ	(move back to probing result)
G92 Z-6 		(Set new coordinate system, Z at switch was triggered)
G90 G1 F600 Z0	(absolute distance mode, move to Z=0)