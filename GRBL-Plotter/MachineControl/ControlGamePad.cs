/*  GRBL-Plotter. Another GCode sender for GRBL.
    This file is part of the GRBL-Plotter application.
   
    Copyright (C) 2015-2021 Sven Hasemann contact: svenhb@web.de

    This program is free software: you can redistribute it and/or modify
    it under the terms of the GNU General Public License as published by
    the Free Software Foundation, either version 3 of the License, or
    (at your option) any later version.

    This program is distributed in the hope that it will be useful,
    but WITHOUT ANY WARRANTY; without even the implied warranty of
    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
    GNU General Public License for more details.

    You should have received a copy of the GNU General Public License
    along with this program.  If not, see <http://www.gnu.org/licenses/>.
*/
/* https://stackoverflow.com/questions/3929764/taking-input-from-a-joystick-with-c-sharp-net
 * 
 * 2020-09-02 check joystickGuid before Instantiate the joystick
 * 2021-07-26 code clean up / code quality
*/

using SharpDX.DirectInput;
using System;

//#pragma warning disable CA1305

namespace GrblPlotter
{
    public static class ControlGamePad
    {
        internal static Joystick gamePad;
        public static bool Initialize()
        {
            // Initialize DirectInput
            var directInput = new DirectInput();
            //     var logstring="";
            // Find a Joystick Guid
            var joystickGuid = Guid.Empty;

            foreach (var deviceInstance in directInput.GetDevices(DeviceType.Gamepad, DeviceEnumerationFlags.AllDevices))
                joystickGuid = deviceInstance.InstanceGuid;

            // If Gamepad not found, look for a Joystick
            if (joystickGuid == Guid.Empty)
                foreach (var deviceInstance in directInput.GetDevices(DeviceType.Joystick, DeviceEnumerationFlags.AllDevices))
                    joystickGuid = deviceInstance.InstanceGuid;

            // If Joystick not found, throws an error
            if (joystickGuid == Guid.Empty)
            {
                //        logstring += ("No joystick/Gamepad found.");
            }
            else
            {
                // Instantiate the joystick
                gamePad = new Joystick(directInput, joystickGuid);

                // Query all suported ForceFeedback effects
                //	var allEffects = gamePad.GetEffects();
                //	foreach (var effectInfo in allEffects)
                //		logstring += string.Format("Effect available {0}", effectInfo.Name);

                // Set BufferSize in order to use buffered data.
                gamePad.Properties.BufferSize = 128;

                // Acquire the joystick
                gamePad.Acquire();

                directInput.Dispose();
                return true;
            }
            directInput.Dispose();
            return false;
        }
    }
}

