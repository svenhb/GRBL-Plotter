/* https://stackoverflow.com/questions/3929764/taking-input-from-a-joystick-with-c-sharp-net
 * 
 * */

using System;
using SharpDX.DirectInput;

namespace GRBL_Plotter
{
    class ControlGamePad
    {
        public static Joystick gamePad;
        public static void Initialize()
        {
            // Initialize DirectInput
            var directInput = new DirectInput();
            var logstring = "";
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
                logstring+=("No joystick/Gamepad found.");
            }

            // Instantiate the joystick
            gamePad = new Joystick(directInput, joystickGuid);

            // Query all suported ForceFeedback effects
            var allEffects = gamePad.GetEffects();
            foreach (var effectInfo in allEffects)
                logstring += string.Format("Effect available {0}", effectInfo.Name);

            // Set BufferSize in order to use buffered data.
            gamePad.Properties.BufferSize = 128;

            // Acquire the joystick
            gamePad.Acquire();
        }
    }
}

