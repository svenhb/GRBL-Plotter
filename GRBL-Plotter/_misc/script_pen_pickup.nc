(Get pen from pen storage)
(Assumption: active pen storage position is needed pen)

(Open gripper - RC-Servo via Spinde PWM)
(^2 M3 S800)

(CNC: Move in Machine Coordinates close to pen)
G90 G53 G0 Z-8
G90 G53 G0 X-146.6

(Move to pen holder)
G90 G53 G1 X-168 F2000

(Turn pen storage a little bit to touch gripper)
(^2 G91 G0 X0.15)
(Close gripper - RC-Servo via Spinde PWM)
(^2 M5)
(^2 G4 P0.5)
($TOOL-IN)

(pull pen )
G90 G53 G1 X-146.6 F1000

(Turn pen storage back)
(^2 G91 G0 X-0.15)
