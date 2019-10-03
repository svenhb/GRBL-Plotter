( Select and grip pen nr.)
( G53 indicates machine coordinates - no transform required)
( move Z in relative coordinates to compensate different tool length)
M8							( open gripper)
G91 G0 Z20					( lift gripper in save position)
G53 G90 G0 X#TOAX Y#TOAY	( move gripper in front of actual pen in absolute machine coordinates)
G53 G90 G0 Z#TOAZ			( move gripper to correct height)

G91 Y3						( move gripper in position)
X-15
M9							( close gripper)
Y-3
Z20							( lift pen)
X15							( take pen)
($TOOL-IN)
G90
