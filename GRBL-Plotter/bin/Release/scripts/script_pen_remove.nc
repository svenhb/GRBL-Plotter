(Move pen back to pen storage)
(Assumption: active pen storage position is empty)

(CNC: Move in Machine Coordinates)
G90 G53 G0 Z-8
G90 G53 G0 X-146.6

(Turn pen storage a little bit to get pen)
(^2 G91 G0 X0.15)

(Move to pen holder)
G90 G53 G1 X-168 F1000

(Open gripper - RC-Servo via Spinde PWM)
(^2 M3 S800)
(^2 G4 P0.5)
(Turn pen storage back)
(^2 G91 G0 X-0.15)
($TOOL-OUT)

(Remove gripper)
G4 P0.5
G90 G53 G1 X-146.6 F2000
