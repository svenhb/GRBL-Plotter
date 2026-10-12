( Select water color nr. 2026-09-06 )
#TOV4	( Pen far up )
#TOV3	( move brush towards water bowl 3)
#TOV5   ( Pen down )
#TOV6   ( Pen up )

G91 G1 F1000 X-20Z12
X-10

#TOV4	( Pen far up )
G53 G90 G0 X#TOAX Y#TOAY	( move brush towards color pad in absolute machine coordinates )
#TOV5	( Pen down  )

#TOV7	( stir brush inside cup )
#TOV8 
#TOV8 
#TOV8 
G1X8
G1 Y-20Z2 F1000
Z5
#TOV4	( Pen far up subroutine from main program)
G90	( finally switch to absolute coordinates)
