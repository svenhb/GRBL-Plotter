( Select and grip pen nr.)
( G53 indicates machine coordinates - no transform required)
( move Z in relative coordinates to compensate different tool length)
M8							( open gripper)
G53 G90 G0 X#TOAX Y#TOAY	( move gripper in front of actual pen in absolute machine coordinates)
G53 G90 G0 Z#TOAZ			( move gripper to correct height)

G91 Y3						( move gripper in position)
X-15
M9							( close gripper)
Y-3
Z20							( lift pen to get it out of holder)
X15							( take pen)

G90
G53 G90 G0 Z#TOAZ			( move gripper to correct height)

