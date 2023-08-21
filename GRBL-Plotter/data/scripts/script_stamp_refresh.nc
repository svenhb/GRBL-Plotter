( Refresh stamp)
( %START_HIDECODE )
( G53 indicates machine coordinates - no transform required)
M3 S5 
G04 P0.500
G53 G90 G0 X0 Y0	( move brush towards water glass in absolute machine coordinates)
M3 S30 
G04 P1
M3 S5 
G04 P0.500
G90
( %STOP_HIDECODE )
