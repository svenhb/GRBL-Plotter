G91 G38.3 Z-5 F100	(move down relative, max 5 mm)
($PROBE)
G10 L2 P0 Z#PRBZ	(set actual coord-system)
G90 G0 Z1.0			(move to save height)
