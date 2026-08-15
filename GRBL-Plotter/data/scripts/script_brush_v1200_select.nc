( Select water color nr.)
( G53 indicates machine coordinates - no transform required)
( move Z in relative coordinates to compensate different tool length)
( G91 G0 Z30 					( move brush upwards in relative coordinates)
G53 G90 G0 X#TOAX Y#TOAY	( move brush towards color pad in absolute machine coordinates)
G53 G90 G0 Z#TOAZ			( move brush downwards)

G91 G01 X-8.000 Y0.000 F4000    ( stir brush inside cup )
G02 X0.000 Y0.000 I8.000 J0.000 
G02 X0.000 Y0.000 I8.000 J0.000 
G02 X0.000 Y0.000 I8.000 J0.000 

G91 G0 Z20                ( move brush upwards to save height)
G53 G90 G0 X-120
G90
