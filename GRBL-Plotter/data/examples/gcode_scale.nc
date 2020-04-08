(Draw a scale on the )
(circumference of a cylinder)
(using a rotary axis)
G00 Z2
G00 X0 Y0
M98 P0090 L4
M98 P0030 L12
M98 P0005 L72
M30

(90° graduation)
O0090
G91
G01 Z-4
X10
G00 Z4
X-10 Y90
G90
M99

(30° graduation)
O0030
G91
G01 Z-4
X7.5
G00 Z4
X-7.5 Y-30
G90
M99

(5° graduation)
O0005
G91
G01 Z-4
X5
G00 Z4
X-5 Y5
G90
M99
