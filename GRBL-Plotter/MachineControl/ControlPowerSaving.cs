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

/* Thanks to:
 * https://decatec.de/programmierung/c-sharp-windows-standby-unterdruecken/
 * 
*/
/*  2018-12-26	Commits from RasyidUFA via Github
 */

namespace GrblPlotter
{
    public static class ControlPowerSaving
    {
        public static void SuppressStandby()
        {   if (NativeMethods.PowerAvailabilityRequestsSupported())
                NativeMethods.SuppressStandbyWin7();
            else
                NativeMethods.SuppressStandbyXP();
        }
        public static void EnableStandby()
        {   if (NativeMethods.PowerAvailabilityRequestsSupported())
                NativeMethods.EnableStandbyWin7();
            else
                NativeMethods.EnableStandbyXP();
        }
	}
}
