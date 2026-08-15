( Select and grip pen nr.)
( G53 indicates machine coordinates - no transform required)
( move Z in relative coordinates to compensate different tool length)
#TOGO                       ( open gripper)
G53 G90 G0 X#TOAX Y#TOAY    ( move gripper in front of actual pen in absolute machine coordinates)
G53 G90 G0 Z#TOAZ           ( move gripper to correct height)
G91 G1 F500                 ( set relative mode and feedrate )
X-1                         ( move gripper in position)
Y-12
X1

#TOGC                       ( close gripper)
G91 G1 F500                 ( set relative mode and feedrate )
Z20                         ( lift pen to get it out of holder)
G53 G90 Y#TOAY              ( move back to orig. Y pos )
G53 G90 Z#TOAZ              ( move gripper to correct height)

