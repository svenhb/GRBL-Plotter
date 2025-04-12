/*  GRBL-Plotter. Another GCode sender for GRBL.
    This file is part of the GRBL-Plotter application.
   
    Copyright (C) 2015-2025 Sven Hasemann contact: svenhb@web.de

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
* Thanks to http://stackoverflow.com/questions/217902/reading-writing-an-ini-file
*/
/* 2018-12-26 Commits from RasyidUFA via Github
 * 2019-08-15 add logger
 * 2019-09-17 update settings
 * 2019-11-16 add gamepad
 * 2019-12-07 showIniSettings -> selection between actual (Properties.Settings.Default.x) and ini-file values
 * 2020-03-10 add tangential axis
 * 2020-07-08 add hatch fill
 * 2020-09-21 add 'Button' at end of last used button 1-32 line 960
 * 2020-10-05 add 2D-view widths and colors
 * 2021-01-22 add missing settings
 * 2021-01-27 add missing settings
 * 2021-02-06 add gamePad PointOfViewController0
 * 2021-04-19 add importGCSubPenUpDown
 * 2021-08-08 add GCode conversion - shape development
 * 2021-09-10 add new properties 2DView colors
 * 2021-11-02 add new properties fiducials
 * 2022-02-23 add Command extension settings
 * 2023-03-05 add importGCZIncNoZUp
 * 2023-04-26 add importGraphicFilterEnable
 * 2023-06-05 add further simple shape variables
 * 2024-02-10 add create text and barcode
 * 2024-07-22 l:281/841 f: add Hatch fill distance offset
 * 2024-12-19 l:950 f:ReadImport bug fix #429
 * 2025-02-23 add M6PassThrough #435
 * 2025-03-06 l:112 f:Write Value.Replace ',' by '.'
 * 2025-03-14 outsourcing of ini-key variable storage - e.g. ControlSetupFormIni.cs
*/

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Reflection;
using System.Text;
using System.Windows.Forms;

namespace GrblPlotter
{
    public partial class IniFile
    {
        readonly string iniPath;
        readonly string ExeName = Assembly.GetExecutingAssembly().GetName().Name;
        IDictionary<string, string[,]> iniSection = new Dictionary<string, string[,]>();

        // Trace, Debug, Info, Warn, Error, Fatal
        private static readonly NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();

        public IniFile(string IniPath = null, bool isurl = false)
        {
            Logger.Info("++++++ IniFile START {0} ++++++", IniPath);
            if (isurl)
            {
                string tmpfile = Datapath.AppDataFolder + "\\" + "tmp.ini";
                string content;
                using (var wc = new System.Net.WebClient())
                {
                    try
                    {
                        content = wc.DownloadString(IniPath);
                        System.IO.File.WriteAllText(tmpfile, content, Encoding.Unicode);
                    }
                    catch { MessageBox.Show("IniFile - Could not load content from " + IniPath); }
                }
                IniPath = tmpfile;
                Logger.Trace("finish download to {0}", tmpfile);
            }
            iniPath = new FileInfo(IniPath ?? ExeName + ".ini").FullName.ToString();

            if (!iniSection.ContainsKey(sectionSetupGcodeGeneration)) { iniSection.Add(sectionSetupGcodeGeneration, keyValueSetupGcodeGeneration); }
            if (!iniSection.ContainsKey(sectionSetupImportParameter)) { iniSection.Add(sectionSetupImportParameter, keyValueSetupImportParameter); }
            if (!iniSection.ContainsKey(sectionSetupMachineLimits)) { iniSection.Add(sectionSetupMachineLimits, keyValueSetupMachineLimits); }
            if (!iniSection.ContainsKey(sectionSetupRotaryAxis)) { iniSection.Add(sectionSetupRotaryAxis, keyValueSetupRotaryAxis); }
            if (!iniSection.ContainsKey(sectionSetupFileLoading)) { iniSection.Add(sectionSetupFileLoading, keyValueSetupFileLoading); }
            if (!iniSection.ContainsKey(sectionSetupFlowControl)) { iniSection.Add(sectionSetupFlowControl, keyValueSetupFlowControl); }
            if (!iniSection.ContainsKey(sectionSetupSvgDxfCsv)) { iniSection.Add(sectionSetupSvgDxfCsv, keyValueSetupSvgDxfCsv); }
            if (!iniSection.ContainsKey(sectionSetupPathInterpretation)) { iniSection.Add(sectionSetupPathInterpretation, keyValueSetupPathInterpretation); }
            if (!iniSection.ContainsKey(sectionSetupPathAddon)) { iniSection.Add(sectionSetupPathAddon, keyValueSetupPathAddon); }
            if (!iniSection.ContainsKey(sectionSetupPathModifications)) { iniSection.Add(sectionSetupPathModifications, keyValueSetupPathModifications); }
            if (!iniSection.ContainsKey(sectionSetupCodeConversion)) { iniSection.Add(sectionSetupCodeConversion, keyValueSetupCodeConversion); }
            if (!iniSection.ContainsKey(sectionSetupCommandExtension)) { iniSection.Add(sectionSetupCommandExtension, keyValueSetupCommandExtension); }
            if (!iniSection.ContainsKey(sectionSetupGCodeModification)) { iniSection.Add(sectionSetupGCodeModification, keyValueSetupGCodeModification); }
            if (!iniSection.ContainsKey(sectionSetupToolChange)) { iniSection.Add(sectionSetupToolChange, keyValueSetupToolChange); }
            if (!iniSection.ContainsKey(sectionSetup2DView)) { iniSection.Add(sectionSetup2DView, keyValueSetup2DView); }
            if (!iniSection.ContainsKey(sectionSetupButtons)) { iniSection.Add(sectionSetupButtons, keyValueSetupButtons); }
            if (!iniSection.ContainsKey(sectionSetupJoystick)) { iniSection.Add(sectionSetupJoystick, keyValueSetupJoystick); }
            if (!iniSection.ContainsKey(sectionSetupGamePad)) { iniSection.Add(sectionSetupGamePad, keyValueSetupGamePad); }
            /* other forms */
            if (!iniSection.ContainsKey(sectionCamera)) { iniSection.Add(sectionCamera, keyValueCamera); }
            if (!iniSection.ContainsKey(sectionConnections)) { iniSection.Add(sectionConnections, keyValueConnections); }
            if (!iniSection.ContainsKey(sectionText)) { iniSection.Add(sectionText, keyValueText); }
            if (!iniSection.ContainsKey(sectionBarcode)) { iniSection.Add(sectionBarcode, keyValueBarcode); }
            if (!iniSection.ContainsKey(sectionShapeTool)) { iniSection.Add(sectionShapeTool, keyValueShapeTool); }
            if (!iniSection.ContainsKey(sectionProcAuto)) { iniSection.Add(sectionProcAuto, keyValueProcAuto); }
        }

        public string Read(string Key, string Section = null)
        {
            var RetVal = new StringBuilder(255);
            try
            {
                NativeMethods.GetPrivateProfileString(Section ?? ExeName, Key, "", RetVal, 255, iniPath);
                return RetVal.ToString();
            }
            catch (Exception err) { MessageBox.Show("Error in IniFile-Read: " + err.ToString()); return ""; }
        }

        public void Write(string Key, string Value, string Section = null)
        {
            if (Section != "Info")
                Value = Value.Replace(',', '.');
            try
            {
                NativeMethods.WritePrivateProfileString(Section ?? ExeName, Key, Value, iniPath);
            }
            catch (Exception err) { MessageBox.Show("Error in IniFile-Read: " + err.ToString()); }
        }

        public void WriteSection(string section, bool allSettings = true)
        {
            if (iniSection.ContainsKey(section))
            {
                string[,] keyValue = iniSection[section];
                var setup = Properties.Settings.Default;
                string lastKey = "", newKey;
                int splitIndex = 0;
                string[] keySplit;
                for (int i = 0; i < keyValue.GetLength(0); i++)
                {
                    /* only write settings of same sort if option is enabled */
					if ((keyValue[i, 0] == "") || (keyValue[i, 1] == ""))
					{
						Logger.Error("WriteSection: missing key or setting in {0} {1}", i, section);
						continue;
					}
                    if (!allSettings)
                    {
                        if ((keyValue.GetLength(1) > 2) && (keyValue[i, 2].Length > 0) && (setup[keyValue[i, 1]].ToString() == keyValue[i, 2]))
                            continue;
                        keySplit = keyValue[i, 0].Split(' ');
                        if (keyValue[i, 0].ToLower().Contains("enable"))
                        {
                            lastKey = "";
                            if (!(bool)setup[keyValue[i, 1]])
                            {
                                lastKey = keySplit[0];
                                splitIndex = 0;
                                if (keySplit.Length > 2)
                                {
                                    lastKey += " " + keySplit[1];
                                    splitIndex++;
                                }
                                continue;
                            }
                        }
                        newKey = keySplit[0];
                        if ((keySplit.Length > 1) && (splitIndex > 0))
                            newKey += " " + keySplit[1];
                        if ((lastKey.Length > 0) && (lastKey == newKey))
                            continue;
                        else
                            lastKey = "";
                    }

                    try
                    {
                        var vtyp = setup[keyValue[i, 1]];   // convertString(keyValue[i, 2]);
                        if (vtyp is Font)
                        {
                            var cvt = new FontConverter();
                            string s = cvt.ConvertToString(setup[keyValue[i, 1]]);
                            Write(keyValue[i, 0], s, section);
                        }
                        else if (vtyp is Color)
                        {
                            Write(keyValue[i, 0], ColorTranslator.ToHtml((Color)setup[keyValue[i, 1]]), section);
                            Logger.Trace("Write color {0}", setup[keyValue[i, 1]], ColorTranslator.ToHtml((Color)setup[keyValue[i, 1]]));
                        }
                        else
                            Write(keyValue[i, 0], setup[keyValue[i, 1]].ToString(), section);
                    }
                    catch (Exception err) { Logger.Error(err, " WriteSection {0} {1} {2} ", section, keyValue[i, 0], keyValue[i, 1]); }
                }
            }
            else
            { Logger.Warn("WriteSection section not found:'{0}'", section); }
        }

        public void ReadSection(string section)
        {
            string tmpstring = "";
            bool tmpbool = false;
            int tmpint = 0;
            decimal tmpdeci = 0;
            double tmpdouble = 0;
            byte tmpbyte = 0;
            Int64 tmpint64 = 0;
            float tmpfloat = 0;
            Color tmpcolor = Color.Black;
            Font tmpfont = null;

            if (iniSection.ContainsKey(section))
            {
                string[,] keyValue = iniSection[section];
                var setup = Properties.Settings.Default;
                for (int i = 0; i < keyValue.GetLength(0); i++)
                {
                    try
                    {
                        var vtyp = setup[keyValue[i, 1]];   // convertString(keyValue[i, 2]);
                        if (vtyp is string) { if (SetVariable(ref tmpstring, section, keyValue[i, 0])) { setup[keyValue[i, 1]] = tmpstring; } }
                        else if (vtyp is bool) { if (SetVariable(ref tmpbool, section, keyValue[i, 0])) { setup[keyValue[i, 1]] = tmpbool; } }
                        else if (vtyp is byte) { if (SetVariable(ref tmpbyte, section, keyValue[i, 0])) { setup[keyValue[i, 1]] = tmpbyte; } }
                        else if (vtyp is int) { if (SetVariable(ref tmpint, section, keyValue[i, 0])) { setup[keyValue[i, 1]] = tmpint; } }
                        else if (vtyp is decimal) { if (SetVariable(ref tmpdeci, section, keyValue[i, 0])) { setup[keyValue[i, 1]] = tmpdeci; } }
                        else if (vtyp is double) { if (SetVariable(ref tmpdouble, section, keyValue[i, 0])) { setup[keyValue[i, 1]] = tmpdouble; } }
                        else if (vtyp is Int64) { if (SetVariable(ref tmpint64, section, keyValue[i, 0])) { setup[keyValue[i, 1]] = tmpint64; } }
                        else if (vtyp is float) { if (SetVariable(ref tmpfloat, section, keyValue[i, 0])) { setup[keyValue[i, 1]] = tmpfloat; } }
                        else if (vtyp is Color) { if (SetVariable(ref tmpcolor, section, keyValue[i, 0])) { setup[keyValue[i, 1]] = tmpcolor; } }
                        else if (vtyp is Font) { if (SetVariable(ref tmpfont, section, keyValue[i, 0])) { setup[keyValue[i, 1]] = tmpfont; } }
                        else Logger.Error("Type not defined: type:{0}  key:{1}", vtyp.GetType().ToString(), keyValue[i, 1]);
                    }
                    catch (Exception err) { Logger.Error(err, " ReadSection {0} {1} {2} ", section, keyValue[i, 0], keyValue[i, 1]); }
                }
            }
        }

        public void ResetSection(string section)
        {
			bool extendedInfo = false;
            if (iniSection.ContainsKey(section))
            {
                string[,] keyValue = iniSection[section];
                var setup = Properties.Settings.Default;
				if (keyValue.GetLength(1) < 3)
				{
					if (extendedInfo) Logger.Trace("ResetSection: no default values set in {0}", section);
					return;
				}
                for (int i = 0; i < keyValue.GetLength(0); i++)
                {
					if (keyValue[i, 2] == "")	// no default set
						continue;
					if ((keyValue[i, 0] == "") || (keyValue[i, 1] == ""))
					{
						Logger.Error("ResetSection: missing key or setting in {0} {1}", i, section);
						continue;
					}
					
                    try
                    {
                        var vtyp = setup[keyValue[i, 1]];
                        if (vtyp is bool) 
						{ 
							if (bool.TryParse(keyValue[i, 2], out bool tmp))
							{ setup[keyValue[i, 1]] = tmp;}		
							if (extendedInfo) Logger.Trace("Reset {0}  '{1}'  {2}  to  {3}", section, keyValue[i, 0], keyValue[i, 1], keyValue[i, 2]);
						}
                        else Logger.Error("ResetSection Type not defined: type:{0}  key:{1}", vtyp.GetType().ToString(), keyValue[i, 1]);
                    }
                    catch (Exception err) { Logger.Error(err, " ResetSection {0} {1} {2} ", section, keyValue[i, 0], keyValue[i, 1]); }
                }
            }
        }

		/* partial writing of ini keys */
        public void WriteImport(bool all = false)
        {
            var setup = Properties.Settings.Default;

            string section = "Info";
            string localDate = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");

            if (!File.Exists(iniPath))  // Write entry manually to force UTF-16 encoding
            {
                string myunicode = string.Format("[{0}]\r\nDate={1}\r\n", section, localDate);
                File.WriteAllText(iniPath, myunicode, Encoding.Unicode);
            }

            Write("Use case info", setup.importUseCaseInfo.ToString().Replace("\r", "\\r").Replace("\n", "\\n"), section);
            Write("Show Use Case", setup.importShowUseCaseDialog.ToString(), section);
            Write("Set Defaults", "True", section);

            WriteSection(sectionSetupGcodeGeneration, all);
            WriteSection(sectionSetupImportParameter, all);
            WriteSection(sectionSetupMachineLimits, all);
            WriteSection(sectionSetupRotaryAxis, all);
            WriteSection(sectionSetupFileLoading, all);
            WriteSection(sectionSetupFlowControl, all);
            WriteSection(sectionSetupSvgDxfCsv, all);
            WriteSection(sectionSetupPathInterpretation, all);
            WriteSection(sectionSetupPathAddon, all);
            WriteSection(sectionSetupPathModifications, all);
            WriteSection(sectionSetupCodeConversion, all);
            WriteSection(sectionSetupCommandExtension, all);
            WriteSection(sectionSetupGCodeModification, all);
            WriteSection(sectionSetupToolChange, all);
        }

		/* complete writing of all ini keys */
        public void WriteAll(bool all = false)
        {
            WriteImport(all);
            var setup = Properties.Settings.Default;
            string section;

            WriteSection(sectionCamera);
            WriteSection(sectionConnections);
            WriteSection(sectionText);
            WriteSection(sectionBarcode);
            WriteSection(sectionShapeTool);
            WriteSection(sectionProcAuto);

            if (setup.guiExtendedLoggingEnabled)
            {
                section = "Logging";
                Write("Log Enable", setup.guiExtendedLoggingEnabled.ToString(), section);
                Write("Log Flags", setup.importLoggerSettings.ToString(), section);
            }

            WriteGrblSetting();
        }
		
        public void WriteGrblSetting()
        {
            var section = "GRBL Settings";
            if (Grbl.Settings.Count > 0)
            {
                foreach (string setting in Grbl.Settings)
                {
                    string[] splt = setting.Split('=');
                    if (splt.Length > 1)
                        Write(splt[0], splt[1], section);
                }
            }
        }
        public string ReadUseCaseInfo()
        { return Read("Use case info", "Info").Replace("\\r", "\r").Replace("\\n", "\n"); }

        private void SetDefaults()
        {
            var setup = Properties.Settings.Default;
            Logger.Info("+++ Set Defaults ");

            foreach (string key in iniSection.Keys)
            {
                ResetSection(key);	// up to now only bool wiil be handled
            }
        }
		
        public void ReadImport()
        {
            var setup = Properties.Settings.Default;
            string section;
            bool tmpbool = false;
            string tmpstr = "";
            section = "Info";
            if (SetVariable(ref tmpbool, section, "Set Defaults") && tmpbool)
                SetDefaults();

            setup.importUseCaseInfo = Read("Use case info", section).Replace("\\r", "\r").Replace("\\n", "\n");
            if (SetVariable(ref tmpbool, section, "Show Use Case")) { setup.importShowUseCaseDialog = tmpbool; }

            if (SetVariable(ref tmpstr, "Tool change", "Tool table loaded"))
            {
                setup.toolTableLastLoaded = tmpstr;
                Logger.Info("Copy Tool table {0} to {1}", tmpstr, ToolTable.DefaultFileName);
                string fpath = tmpstr;
                if (!Path.IsPathRooted(fpath))
                    fpath = Path.Combine(Datapath.Tools, tmpstr);

                if (File.Exists(fpath))
                {
                    try
                    {
                        File.Copy(Datapath.Tools + "\\" + ToolTable.DefaultFileName, Datapath.Tools + "\\_beforeUseCase.csv", true);	// backup old _current_.csv
                        File.Copy(fpath, Datapath.Tools + "\\" + ToolTable.DefaultFileName, true);              // apply new _current_.csv

                        setup.toolTableOriginal = true;
                        ToolTable.Init(" (IniFile)");
                    }
                    catch (Exception err)
                    {
                        Logger.Error(err, "Could not copy data: tmpstr:{0}  fpath:{1} ", tmpstr, fpath);
                        MessageBox.Show("Could not copy data: " + err.Message, "Error");
                    }
                }
                else
                {
                    Logger.Error("File does not exist {0}", tmpstr);
                    MessageBox.Show("Tool table not found: " + fpath, "Attention");
                }
            }
            setup.Save();
        }

        public void ReadAll()
        {
            ReadImport();
            var setup = Properties.Settings.Default;
            string section;
            bool tmpbool = false;
            Color tmpcolor = Color.Black;
            int tmpint = 0;
            string tmpstr = "";

            foreach (string key in iniSection.Keys)
            {
                ReadSection(key);
            }

			/* Add a button without number at the end */
            if (SetVariable(ref tmpstr, sectionSetupButtons, "Button")) // add button at end. Find last filled btn
            {
                bool btn_set = false;
                Logger.Trace("Buttontext {0} ", tmpstr);
                for (int i = 32; i >= 1; i--)
                {
                    try
                    {
                        string[] tmp = Properties.Settings.Default["guiCustomBtn" + i.ToString()].ToString().Split('|');
                        if ((tmp[0].Length > 1) && (i < 32))
                        {
                            setup["guiCustomBtn" + (i + 1).ToString()] = tmpstr;
                            //Properties.Settings.Default["guiCustomBtn" + (i+1).ToString()] =  tmpstr;
                            Logger.Trace("Button {0}  set {1}", (i + 1), tmpstr);
                            btn_set = true;
                            break;
                        }
                    }
                    catch { Logger.Error("Set Button at end nok found:{0} text:{1}", i, tmpstr); }
                }
                if (!btn_set)
                {
                    Properties.Settings.Default["guiCustomBtn1"] = tmpstr;
                    Logger.Trace("Button {0}  set {1}", 1, tmpstr);
                }
            }

            section = "Logging";
            if (SetVariable(ref tmpbool, section, "Log Enable")) { setup.guiExtendedLoggingEnabled = tmpbool; }
            if (SetVariable(ref tmpint, section, "Log Flags")) { setup.importLoggerSettings = Convert.ToByte(tmpint); }

            section = "Homepage";
            if (SetVariable(ref tmpstr, section, "UpdateURL")) { setup.guiCheckUpdateURL = tmpstr; }

            setup.Save();
        }

        public string ShowIniMachineSettingsHTML(string header)
        {
            StringBuilder tmp = new StringBuilder();
            tmp.Append("<!DOCTYPE html><html lang = 'de'><head><style>\r\n" +
                "body {font-family: Verdana;}\r\n" +
                "h1,h2 {margin:5px 0px 0px 0px;}" +// text-align:center;}\r\n" +
                "h3,h4 {margin:5px 0px 0px 0px;}\r\n" +
                "</style><title>INI Machine Settings</title></head><body>");

			if (header != "")
				tmp.AppendFormat("<h1>{0}</h1>\r\n", header);
            tmp.AppendFormat("<h2>File: '{0}'</h2>\r\n", Path.GetFileName(iniPath));
            tmp.AppendFormat("<h2>{0}</h2>\r\n", Read("Use case info", "Info").Replace("\\r", "\r").Replace("\\n", "\n"));
            bool SetDefaults = false; SetVariable(ref SetDefaults, "Info", "Set Defaults");

            tmp.AppendFormat("<table border=1>\r\n");
            tmp.AppendFormat("<tr><td colspan=3><h4>Last saved: {0}</h4></td></tr>\r\n", Read("Date", "Info"));
            if (SetDefaults) tmp.AppendFormat("<tr><td colspan=3><h4>Reset almost all settings to default</h4></td></tr>\r\n");

            tmp.AppendFormat("<tr><th>Key</th><th>INI-value</th><th>actual value</th></tr>\r\n");

            tmp.Append(KeyValueRegHTML("GCode generation", "Dec Places", "importGCDecPlaces"));
            tmp.Append(KeyValueRegHTML("GCode generation", "Header Code", "importGCHeader"));
            tmp.Append(KeyValueRegHTML("GCode generation", "Footer Code", "importGCFooter"));

            tmp.Append(KeyValueRegHTML("GCode generation", "XY Feedrate", "importGCXYFeed", 1));
            tmp.Append(KeyValueRegHTML("GCode generation", "XY Feedrate from TT", "importGCTTXYFeed"));

            tmp.Append(KeyValueRegHTML("GCode generation", "Spindle Speed", "importGCSSpeed", 1));
            tmp.Append(KeyValueRegHTML("GCode generation", "Spindle Speed from TT", "importGCTTSSpeed"));
            tmp.Append(KeyValueRegHTML("GCode generation", "Spindle Use Laser", "importGCSpindleToggleLaser"));

            tmp.Append(KeyValueRegHTML("GCode generation", "Spindle Direction M3", "importGCSDirM3"));
            tmp.Append(KeyValueRegHTML("GCode generation", "Spindle Delay", "importGCSpindleDelay"));

            tmp.Append(KeyValueRegHTML("GCode generation", "Add Tool Cmd", "importGCTool"));
            tmp.Append(KeyValueRegHTML("GCode generation", "Add Tool M0", "importGCToolM0"));
            tmp.Append(KeyValueRegHTML("GCode generation", "Add Comments", "importGCAddComments"));


            tmp.AppendFormat("<tr><td colspan=3><b>Pen up/down translation</b></td></tr>\r\n");
            bool Z_enable = false; SetVariable(ref Z_enable, "GCode generation", "Z Enable");
            tmp.Append(KeyValueRegHTML("GCode generation", "Z Enable", "importGCZEnable"));
            if (Z_enable)
            {
                tmp.Append(KeyValueRegHTML("GCode generation", "Z Values from TT", "importGCTTZAxis"));
                tmp.Append(KeyValueRegHTML("GCode generation", "Z Feedrate", "importGCZFeed", 1));
                tmp.Append(KeyValueRegHTML("GCode generation", "Z Up Pos", "importGCZUp"));
                tmp.Append(KeyValueRegHTML("GCode generation", "Z Down Pos", "importGCZDown"));
                tmp.Append(KeyValueRegHTML("GCode generation", "Z Inc Enable", "importGCZIncEnable"));
                tmp.Append(KeyValueRegHTML("GCode generation", "Z Increment at zero", "importGCZIncStartZero"));
                tmp.Append(KeyValueRegHTML("GCode generation", "Z Increment", "importGCZIncrement"));
                tmp.Append(KeyValueRegHTML("GCode generation", "Z Increment no up", "importGCZIncNoZUp"));
                tmp.Append(KeyValueRegHTML("GCode generation", "Z Prevent Spindle", "importGCZPreventSpindle"));
            }

            //	tmp.AppendFormat("<tr><td colspan=3>-</td></tr>\r\n");
            bool PWM_enable = false; SetVariable(ref Z_enable, "GCode generation", "PWM Enable");
            tmp.Append(KeyValueRegHTML("GCode generation", "PWM Enable", "importGCPWMEnable"));
            if (PWM_enable)
            {
                tmp.Append(KeyValueRegHTML("GCode generation", "PWM Up Val", "importGCPWMUp"));
                tmp.Append(KeyValueRegHTML("GCode generation", "PWM Up Dly", "importGCPWMDlyUp"));
                tmp.Append(KeyValueRegHTML("GCode generation", "PWM Down Val", "importGCPWMDown"));
                tmp.Append(KeyValueRegHTML("GCode generation", "PWM Down Dly", "importGCPWMDlyDown"));
                tmp.Append(KeyValueRegHTML("GCode generation", "PWM Zero Val", "importGCPWMZero"));
                tmp.Append(KeyValueRegHTML("GCode generation", "PWM P93 Val", "importGCPWMP93"));
                tmp.Append(KeyValueRegHTML("GCode generation", "PWM P93 Dly", "importGCPWMDlyP93"));
                tmp.Append(KeyValueRegHTML("GCode generation", "PWM P94 Val", "importGCPWMP94"));
                tmp.Append(KeyValueRegHTML("GCode generation", "PWM P94 Dly", "importGCPWMDlyP94"));
                tmp.Append(KeyValueRegHTML("GCode generation", "PWM Skip M30", "importGCPWMSkipM30"));
            }

            //	tmp.AppendFormat("<tr><td colspan=3>-</td></tr>\r\n");
            tmp.Append(KeyValueRegHTML("GCode generation", "Spindle Toggle", "importGCSpindleToggle"));

            //	tmp.AppendFormat("<tr><td colspan=3>-</td></tr>\r\n");
            bool ind_enable = false; SetVariable(ref Z_enable, "GCode generation", "Individual Enable");
            tmp.Append(KeyValueRegHTML("GCode generation", "Individual Enable", "importGCIndEnable"));
            if (ind_enable)
            {
                tmp.Append(KeyValueRegHTML("GCode generation", "Individual PenUp", "importGCIndPenUp"));
                tmp.Append(KeyValueRegHTML("GCode generation", "Individual PenDown", "importGCIndPenDown"));
            }

            tmp.AppendFormat("</table>\r\n");

            tmp.AppendFormat("</body></html>");
            return tmp.ToString();
        }

        private string KeyValueRegHTML(string section, string key, string property, int style = 0)
        {
            string propVal = Properties.Settings.Default[property].ToString();
            string iniVal = Read(key, section);
            string color = "", styleOn = "", styleOff = "";
            if (propVal == "True" || iniVal == "True")
                color = "bgcolor= \"lime\"";
            if (style == 1)
            { styleOn = "<b>"; styleOff = "</b>"; }
            key = string.Format("{0}{1}{2}", styleOn, key, styleOff);
            iniVal = string.Format("{0}{1}{2}", styleOn, iniVal, styleOff);
            propVal = string.Format("{0}{1}{2}", styleOn, propVal, styleOff);

            return string.Format("<tr {0}><td>{1}</td><td>{2}</td><td>{3}</td></tr>\r\n", color, key, iniVal, propVal);
        }

        public string ShowIniSettings(bool fromSettings = false)
        {
            StringBuilder tmp = new StringBuilder();
            bool fromTTZ = false; SetVariable(ref fromTTZ, "GCode generation", "Z Values from TT");
            bool fromTTXY = false; SetVariable(ref fromTTXY, "GCode generation", "XY Feedrate from TT");
            bool fromTTSS = false; SetVariable(ref fromTTSS, "GCode generation", "Spindle Speed from TT");
            bool ZEnable = false; SetVariable(ref ZEnable, "GCode generation", "Z Enable");
            bool TTImport = false; SetVariable(ref TTImport, "Graphics Import", "Tool table enable");
            bool TangEnable = false; SetVariable(ref TangEnable, "GCode modification", "Tangential axis enable");

            if (fromSettings)
            {
                fromTTZ = Properties.Settings.Default.importGCTTZAxis;
                fromTTXY = Properties.Settings.Default.importGCTTXYFeed;
                fromTTSS = Properties.Settings.Default.importGCTTSSpeed;
                ZEnable = Properties.Settings.Default.importGCZEnable;
                TTImport = Properties.Settings.Default.importGCToolTableUse;
                TangEnable = Properties.Settings.Default.importGCTangentialEnable;
            }
            //           string state;
            tmp.Append("Graphic import:\r\n");
            AddInfo(tmp, "SVG resize    : {0}\r\n", fromSettings ? Properties.Settings.Default.importSVGRezise.ToString() : Read("SVG resize enable", "Graphics Import"));
            AddInfo(tmp, "SVG hatch fill: {0}\r\n", fromSettings ? Properties.Settings.Default.importSVGApplyFill.ToString() : Read("SVG apply fill", "Graphics Import"));
            AddInfo(tmp, "Set origin 0;0: {0}\r\n", fromSettings ? Properties.Settings.Default.importGraphicOffsetOrigin.ToString() : Read("Objects offset origin", "Graphics Import"));
            AddInfo(tmp, "Remove largest: {0}\r\n", fromSettings ? Properties.Settings.Default.importGraphicOffsetLargestRemove.ToString() : Read("Objects offset  remove largest", "Graphics Import"));
            AddInfo(tmp, "Sort paths    : {0}\r\n", fromSettings ? Properties.Settings.Default.importGraphicSortDistance.ToString() : Read("Objects sort by distance", "Graphics Import"));
            AddInfo(tmp, "Process dashed: {0}\r\n", fromSettings ? Properties.Settings.Default.importLineDashPattern.ToString() : Read("Process Dashed Lines", "Graphics Import"));
            AddInfo(tmp, "Path nodes only: {0}\r\n", fromSettings ? Properties.Settings.Default.importSVGNodesOnly.ToString() : Read("SVG Process nodes only", "Graphics Import"));
            AddInfo(tmp, "Pen width to Z: {0}\r\n", fromSettings ? Properties.Settings.Default.importDepthFromWidth.ToString() : Read("Pen width to z", "Graphics Import"));
            AddInfo(tmp, "Circle to dot : {0}\r\n", fromSettings ? Properties.Settings.Default.importSVGCircleToDot.ToString() : Read("SVG circle to dot", "Graphics Import"));

            AddInfo(tmp, "Laser mode    : {0}\r\n", fromSettings ? Properties.Settings.Default.importGCSpindleToggleLaser.ToString() : Read("Spindle Use Laser", "GCode generation"));
            if (!(TTImport && fromTTZ))
                tmp.AppendFormat("XY Feedrate   : {0}\r\n", fromSettings ? Properties.Settings.Default.importGCXYFeed.ToString() : Read("XY Feedrate", "GCode generation"));
            else
                tmp.AppendFormat("XY Feedrate   : from tool table!\r\n");

            if (!(TTImport && fromTTSS))
                tmp.AppendFormat("Spindle Speed : {0}\r\n", fromSettings ? Properties.Settings.Default.importGCSSpeed.ToString() : Read("Spindle Speed", "GCode generation"));
            else
                tmp.AppendFormat("Spindle Speed : from tool table!\r\n");

            tmp.AppendFormat("Z Enable      : {0}\r\n", ZEnable.ToString());
            if (ZEnable)
            {
                if (!(TTImport && fromTTZ))
                {
                    tmp.AppendFormat("  Z Feedrate  : {0}\r\n", fromSettings ? Properties.Settings.Default.importGCZFeed.ToString() : Read("Z Feedrate", "GCode generation"));
                    tmp.AppendFormat("  Z Save      : {0}\r\n", fromSettings ? Properties.Settings.Default.importGCZUp.ToString() : Read("Z Up Pos", "GCode generation"));
                    tmp.AppendFormat("  Z Down Pos  : {0}\r\n", fromSettings ? Properties.Settings.Default.importGCZDown.ToString() : Read("Z Down Pos", "GCode generation"));
                    tmp.AppendFormat("  Z in passes : {0}\r\n", fromSettings ? Properties.Settings.Default.importGCZIncEnable.ToString() : Read("Z Inc Enable", "GCode generation"));
                    tmp.AppendFormat("  Z step/pass : {0}\r\n", fromSettings ? Properties.Settings.Default.importGCZIncrement.ToString() : Read("Z Increment", "GCode generation"));
                }
                else
                { tmp.AppendFormat("  Z values    : from tool table!\r\n"); }
            }
            AddInfo(tmp, "Spindle Toggle: {0}\r\n", fromSettings ? Properties.Settings.Default.importGCSpindleToggle.ToString() : Read("Spindle Toggle", "GCode generation"));
            AddInfo(tmp, "PWM RC-Servo  : {0}\r\n", fromSettings ? Properties.Settings.Default.importGCPWMEnable.ToString() : Read("PWM Enable", "GCode generation"));
            tmp.AppendFormat("Tangent.Enable: {0}\r\n", TangEnable.ToString());
            if (TangEnable)
            {
                tmp.AppendFormat("  Tang. Axis  : {0}\r\n", fromSettings ? Properties.Settings.Default.importGCTangentialAxis.ToString() : Read("Tangential axis name", "GCode modification"));
                tmp.AppendFormat("  Tang. angle : {0}\r\n", fromSettings ? Properties.Settings.Default.importGCTangentialAngle.ToString() : Read("Tangential axis angle", "GCode modification"));
                tmp.AppendFormat("  Tang. turn  : {0}\r\n", fromSettings ? Properties.Settings.Default.importGCTangentialTurn.ToString() : Read("Tangential axis turn", "GCode modification"));
                tmp.AppendFormat("  Tang. range  : {0}\r\n", fromSettings ? Properties.Settings.Default.importGCTangentialRange.ToString() : Read("Tangential axis range", "GCode modification"));
            }
            tmp.AppendLine();
            AddInfo(tmp, "Tool table enable : {0}\r\n", fromSettings ? Properties.Settings.Default.importGCToolTableUse.ToString() : Read("Tool table enable", "Graphics Import"));
            AddInfo(tmp, "Tool table apply  : {0}\r\n", fromSettings ? Properties.Settings.Default.toolTableLastLoaded.ToString() : Read("Tool table loaded", "Tool change"));
            AddInfo(tmp, "Tool change enable: {0}\r\n", fromSettings ? Properties.Settings.Default.ctrlToolChange.ToString() : Read("Tool change enable", "Tool change"));
            return tmp.ToString();
        }
        private static void AddInfo(StringBuilder tmp, string key, string value)
        {
            if (value.Length > 0)
                tmp.AppendFormat(key, value);
        }

        private bool SetVariable(ref string variable, string section, string key)
        {
            string valString = Read(key, section);
            if (string.IsNullOrEmpty(valString)) return false;
            variable = valString;
            return true;
        }
        private bool SetVariable(ref bool variable, string section, string key)
        {
            string valString = Read(key, section);
            if (string.IsNullOrEmpty(valString)) return false;
            //     bool tmp;
            if (bool.TryParse(valString, out bool tmp))
            { variable = tmp; return true; }
            return false;
        }
        private bool SetVariable(ref byte variable, string section, string key)
        {
            string valString = Read(key, section);
            if (string.IsNullOrEmpty(valString)) return false;
            if (byte.TryParse(valString, NumberStyles.Float, NumberFormatInfo.InvariantInfo, out byte tmp))
            { variable = tmp; return true; }
            return false;
        }
        private bool SetVariable(ref int variable, string section, string key)
        {
            string valString = Read(key, section);
            if (string.IsNullOrEmpty(valString)) return false;
            if (int.TryParse(valString, NumberStyles.Float, NumberFormatInfo.InvariantInfo, out int tmp))
            { variable = tmp; return true; }
            return false;
        }
        private bool SetVariable(ref decimal variable, string section, string key)
        {
            string valString = Read(key, section);
            if (string.IsNullOrEmpty(valString)) return false;
            if (decimal.TryParse(valString, NumberStyles.Float, NumberFormatInfo.InvariantInfo, out decimal tmp))
            { variable = tmp; return true; }
            return false;
        }
        private bool SetVariable(ref double variable, string section, string key)
        {
            string valString = Read(key, section);
            if (string.IsNullOrEmpty(valString)) return false;
            if (double.TryParse(valString, NumberStyles.Float, NumberFormatInfo.InvariantInfo, out double tmp))
            { variable = tmp; return true; }
            return false;
        }
        private bool SetVariable(ref Int64 variable, string section, string key)
        {
            string valString = Read(key, section);
            if (string.IsNullOrEmpty(valString)) return false;
            if (Int64.TryParse(valString, NumberStyles.Float, NumberFormatInfo.InvariantInfo, out Int64 tmp))
            { variable = tmp; return true; }
            return false;
        }
        private bool SetVariable(ref float variable, string section, string key)
        {
            string valString = Read(key, section);
            if (string.IsNullOrEmpty(valString)) return false;
            if (float.TryParse(valString, NumberStyles.Float, NumberFormatInfo.InvariantInfo, out float tmp))
            {
                Logger.Trace("Float: {0}", tmp);
                variable = tmp; return true;
            }
            return false;
        }
        private bool SetVariable(ref Color variable, string section, string key)
        {
            string valString = Read(key, section);
            if (string.IsNullOrEmpty(valString)) return false;
            try
            {
                variable = ColorTranslator.FromHtml(valString);
                return true;
            }
            catch (Exception er)
            { Logger.Error(er, "setVariable key:{0} section:{1} variable:{2} ", key, section, valString); }
            return false;
        }
        private bool SetVariable(ref Font variable, string section, string key)
        {
            string valString = Read(key, section);
            if (string.IsNullOrEmpty(valString)) return false;
            var cvt = new FontConverter();
            variable = cvt.ConvertFromString(valString) as Font;
            Logger.Trace("Read font {0}", variable);
            return true;
        }
    }
}
