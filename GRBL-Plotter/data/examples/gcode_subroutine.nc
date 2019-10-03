(Test subroutine)
G0 X10 Y10 Z2 F2000
M98 P1234       (call sub once)
G0 X30 Y10 Z2
M98 P1234 L2    (call sub twice)
G0 X50 Y10 Z2
M98 P1234 L3    (call sub 3 times)
M30

( Test relative moves)
O1234   (start of sub)
G91
G1 Z-4
Y10
X10
Y-10
X-10
G0 Z4
X1 Y1
G90
M99     (end of sub)