( Clean brush in water cup)
( G53 indicates machine coordinates - no transform required)
( move Z in relative coordinates to compensate different tool length)
G91 G0 Z20 				( move brush upwards in relative coordinates)
G53 G90 G0 X-167 Y-160	( move brush towards water glass in absolute machine coordinates)
G53 G90 G0 Z#TOAZ		( move brush downwards into cup)

G91 G01 X-15.000 Y0.000 F4000       ( stir brush inside cup )
G02 X0.000 Y0.000 I15.000 J0.000
G02 X0.000 Y0.000 I15.000 J0.000

G91 G0 Z20              ( move brush upwards to save height)
G90
