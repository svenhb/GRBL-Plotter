/*  GRBL-Plotter. Another GCode sender for GRBL.
    This file is part of the GRBL-Plotter application.
   
    Copyright (C) 2015-2022 Sven Hasemann contact: svenhb@web.de

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
 * 2023-06-20 New file
*/

namespace GrblPlotter
{
    public static class MessageText
    {
        public static string HtmlHeader = "" +
            "<!DOCTYPE html>\r\n<html>\r\n" +
            "<head>\r\n" +
            //        "< meta http-equiv='X-UA-Compatible' content='IE=10' />\r\n" +
            " <style>\r\n   body {  font-family: Verdana;}\r\n" +
            "   h1,h2,h3 {margin:5px 0px 0px 0px; text-align:center;}\r\n" +
            //        "   h3 {margin-left:-5px;}\r\n" +
            "   ul {margin-top:2px;}\r\n" +

            "   .optlist {width:100%; border-collapse: collapse; background-color: #f2f2f2;}\r\n" +
            "   table.optlist { border: 1px solid black; }\r\n" +
            //		"   .optlist tr:nth-child(even) { background-color: #e4ebf2; color: #000; }\r\n" +
            "   .optlist th { font-weight: bold; text-align: center;  font-size: 1.2em; background-color: #ccc;}\r\n" +
            "   .optlist td { padding:5px 20px; }\r\n" +
            "   .optlist td:nth-of-type(2) { text-align: right;}\r\n" +

            "   .highlightWarn {background-color:yellow}\r\n" +
            "   .highlightError {background-color:pink}\r\n" +
            "   .highlightGood {background-color:lightgreen}\r\n" +
            "   .highlightInfo {background-color:lightyellow}\r\n" +

            //        "   .line1 {background-color:lightyellow}\r\n" +
            "   .line2 { background-color: #dbe9ff; }\r\n" +

            " </style>\r\n" +
            "</head>\r\n";

        public static string HtmlColgroup3 = "<colgroup>\r\n<col width = '50%'>\r\n<col width = '25%'>\r\n<col width = '25%'>\r\n</colgroup>\r\n";

        public static string GetGcodeLoadOptions()
        {
            string importModification = "";
            if (Properties.Settings.Default.ctrlReplaceEnable)
            {
                if (Properties.Settings.Default.ctrlReplaceM3)
                {
                    importModification += "<br><h3 class='highlightWarn'>" + Localization.GetString("loadMessageReplaceM34") + "</h3>\r\n";
                }
                else
                {
                    importModification += "<br><h3 class='highlightWarn'>" + Localization.GetString("loadMessageReplaceM43") + "</h3>\r\n";
                }
            }
            return importModification;
        }

        public static string GetStreamingOptions()
        {
            string toolChangeOptions = "";
            if (Properties.Settings.Default.ctrlToolChange)
            {
                toolChangeOptions += "<br><table class='optlist'><tr><th colspan='2' class='highlightWarn'>" + Localization.GetString("loadMessageToolChange") + "</td></tr>\r\n";
                toolChangeOptions += "<tr class='line2'><td>REMOVE</td><td>" + Properties.Settings.Default.ctrlToolScriptPut + "</td></tr>";
                toolChangeOptions += "<tr><td>SELECT</td><td>" + Properties.Settings.Default.ctrlToolScriptSelect + "</td></tr>";
                toolChangeOptions += "<tr class='line2'><td>PICK UP</td><td>" + Properties.Settings.Default.ctrlToolScriptGet + "</td></tr>";
                toolChangeOptions += "<tr><td>PROBE</td><td>" + Properties.Settings.Default.ctrlToolScriptProbe + "</td></tr></table>";
            }
            return toolChangeOptions;
        }

        public static string GetGrblSettings()
        {
            string grblSettings = "";
            if (Grbl.isConnected)
            {
                int i, axisCount = Grbl.axisCount;
                string[] axis = { "X", "Y", "Z", "A", "B", "C" };
                grblSettings += "<br><table class='optlist'><tr><th colspan='7'><a href='https://github.com/gnea/grbl/wiki/Grbl-v1.1-Configuration' title='Link to grbl on GitHub' target='_blank'>grbl configuration</a></td></tr>\r\n";

                int homing = (int)Grbl.GetSetting(22);
                int limitSoft = (int)Grbl.GetSetting(20);
                int limitHard = (int)Grbl.GetSetting(21);

                if (homing != 0)
                {
                    grblSettings += string.Format("<tr><td colspan='{0}'><a href='https://github.com/gnea/grbl/wiki/Grbl-v1.1-Configuration#22---homing-cycle-boolean' title='Link to grbl on GitHub' target='_blank'>$22 Homing</a> is enabled</td></tr>", axisCount + 1);
                    if (limitSoft != 0)
                        grblSettings += string.Format("<tr><td colspan='{0}'><a href='https://github.com/gnea/grbl/wiki/Grbl-v1.1-Configuration#20---soft-limits-boolean' title='Link to grbl on GitHub' target='_blank'>$20 Soft limits</a> are enabled</td></tr>", axisCount + 1);
                }
                else
                {
                    if (limitSoft != 0) { grblSettings += string.Format("<tr><td colspan='{0}' class='highlightError'><a href='https://github.com/gnea/grbl/wiki/Grbl-v1.1-Configuration#20---soft-limits-boolean' title='Link to grbl on GitHub' target='_blank'>$20 Soft limits</a> are enabled, but $22 Homing not</td></tr>", axisCount + 1); }
                }
                if (limitHard != 0)
                    grblSettings += string.Format("<tr><td colspan='{0}'><a href='https://github.com/gnea/grbl/wiki/Grbl-v1.1-Configuration#21---hard-limits-boolean' title='Link to grbl on GitHub' target='_blank'>$21 Hard limits</a> are enabled</td></tr>", axisCount + 1);

                grblSettings += "<tr><td>Configuration</td>";
                for (i = 0; i < axisCount; i++)
                    grblSettings += string.Format("<td>{0}</td>", axis[i]);
                grblSettings += "</tr>";

                grblSettings += "<tr class='line2'><td>$100/$101/$102</td>";
                for (i = 0; i < axisCount; i++)
                    grblSettings += string.Format("<td>{0}</td>", Grbl.GetSetting(100 + i));
                grblSettings += "</tr>";

                grblSettings += "<tr ><td>$110/$111/$112</td>";
                for (i = 0; i < axisCount; i++)
                    grblSettings += string.Format("<td>{0}</td>", Grbl.GetSetting(110 + i));
                grblSettings += "</tr>";

                grblSettings += "<tr class='line2'><td>$120/$121/$122</td>";
                for (i = 0; i < axisCount; i++)
                    grblSettings += string.Format("<td>{0}</td>", Grbl.GetSetting(120 + i));
                grblSettings += "</tr>";

                grblSettings += "<tr ><td>$130/$131/$132</td>";
                for (i = 0; i < axisCount; i++)
                    grblSettings += string.Format("<td>{0}</td>", Grbl.GetSetting(130 + i));
                grblSettings += "</tr>";

                grblSettings += "</table>\r\n";
            }
            return grblSettings;
        }

    }
}
