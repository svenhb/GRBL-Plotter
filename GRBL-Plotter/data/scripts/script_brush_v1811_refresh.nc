( Refresh brush 2026-09-06)
( %START_HIDECODE )
( G53 indicates machine coordinates - no transform required)
(get color)
#TOV4	( Pen far up )
G53 G90 X#TOAX Y#TOAY   ( move brush towards color bowl in absolute machine coordinates)
#TOV5	( Pen down )
G91 G1 F1000 Y-20Z2
Z5

#TOV4	( Pen far up )
G90	( finally switch to absolute coordinates)
( %STOP_HIDECODE )
