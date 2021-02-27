( Clean brush in water cup)
( G53 indicates machine coordinates - no transform required)
M98 P97 			( Pen up subroutine from main program)
G53 G90 G0 X40 Y20	( move brush towards water glass in absolute machine coordinates)
M98 P98				( Pen down subroutine from main program)

G91 G01 X-10.000 Y0.000 F4000       ( stir brush inside cup )
G02 X0.000 Y0.000 I10.000 J0.000
G02 X0.000 Y0.000 I10.000 J0.000

M98 P97         	( Pen up subroutine from main program)
G90					( finally switch to absolute coordinates)
