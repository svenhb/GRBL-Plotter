; Draw circle with radius 1
G91 G1 F500		(relative distance mode, feed mode)
X-1				(move to radius)
Z-2				(move down)
G2 X0 Y0 I1		(end-position = start position, radius 1)
G1 Z2			(move up)
X1				(move back to center of circle)
G90
