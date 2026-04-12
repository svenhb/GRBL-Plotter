/*  GRBL-Plotter. Another GCode sender for GRBL.
    This FileName is part of the GRBL-Plotter application.
   
    Copyright (C) 2015-2026 Sven Hasemann contact: svenhb@web.de

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
/*
 * 2026-04-09 GUI rework for vers. 1.8.0.0
*/

using System.Globalization;
using System.Xml;

namespace GrblPlotter.Helper
{
    internal static class XML
    {
        // Trace, Debug, Info, Warn, Error, Fatal
        private static readonly NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();

        internal static float GetFloat(XmlReader reader, string s, float def)
        {
            if (AttributeOk(reader, s))
            {
                if (float.TryParse(reader[s].Replace(',', '.'), NumberStyles.Float, NumberFormatInfo.InvariantInfo, out float f))
                    return f;
                else
                    Logger.Warn("ReadXML GetFloat nok {0}=\"{1}\"", s, reader[s]);
            }
            return def;
        }
        internal static int GetInt(XmlReader reader, string s, int def)
        {
            if (AttributeOk(reader, s))
            {
                if (int.TryParse(reader[s].Replace(',', '.'), NumberStyles.Float, NumberFormatInfo.InvariantInfo, out int i))
                    return i;
                else
                    Logger.Warn("ReadXML GetInt nok {0}=\"{1}\"", s, reader[s]);
            }
            return def;
        }
        internal static bool GetBool(XmlReader reader, string s, bool def)
        {
            if (AttributeOk(reader, s))
            {
                if (bool.TryParse(reader[s], out bool b))
                    return b;
                else
                    Logger.Warn("ReadXML GetBool nok {0}=\"{1}\"", s, reader[s]);
            }
            return def;
        }
        internal static string GetString(XmlReader reader, string s, string def)
        {
            if (AttributeOk(reader, s))
            {
                return reader[s];
            }
            return def;
        }

        private static bool AttributeOk(XmlReader reader, string s)
        {
            if ((reader[s] != null) && (reader[s].Length > 0))
                return true;
            return false;
        }
    }
}