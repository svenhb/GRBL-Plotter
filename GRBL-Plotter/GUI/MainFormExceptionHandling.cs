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
/*
 * 2021-12-10 new
*/

using GrblPlotter.GUI;
using System;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using System.Threading;
using System.Diagnostics;
using System.Text;

namespace GrblPlotter
{
    public partial class MainForm : Form
    {
        private void Application_ThreadException(object sender, ThreadExceptionEventArgs e)
        {
			QuitSplashScreen();
            if (e != null)
            {			
				Exception ex = e.Exception;
				ShowException(ex, "ThreadException");
			}
			else 
			{	Logger.Error("Application_ThreadException - Exception is NULL");
				Properties.Settings.Default.guiLastEndReason += "ThreadException is null ---";		
			}
        }
		
        private void Application_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
			QuitSplashScreen();
            if (e.ExceptionObject != null)
            {
                Exception ex = (Exception)e.ExceptionObject;
				ShowException(ex, "UnhandledException");
            }
			else 
			{	Logger.Error("Application_UnhandledException - Exception is NULL");
				Properties.Settings.Default.guiLastEndReason += "UnhandledException is null ---";		
			}
        }
		
		private void QuitSplashScreen()
		{
            this.Opacity = 100;
            if (_splashscreen != null)
            {
                _splashscreen.Close();
                _splashscreen.Dispose();
            }			
		}
		
		private void ShowException(Exception ex, string source)
		{
			Logger.Error(ex, "ShowException {0} - Quit GRBL Plotter? ",source);
			if (ex != null)
			{	MessageBox.Show(ex.Message + "\r\n\r\n" + GetAllFootprints(ex) + "\r\n\r\nCheck " + Datapath.AppDataFolder + "\\logfile.txt", "Main Form " + source);
				Properties.Settings.Default.guiLastEndReason += source + ": " + GetAllFootprints(ex, false) +"---";
			}
			else 
			{	Logger.Error("ShowException - Exception is NULL");
				Properties.Settings.Default.guiLastEndReason += "Exception is null ---";		
			}
            if (MessageBox.Show(Localization.GetString("mainQuit"), Localization.GetString("mainProblem"), MessageBoxButtons.YesNo) == DialogResult.Yes)
			{ 	Properties.Settings.Default.guiLastEnd = DateTime.Now.Ticks;
				Application.Exit(); 
			}			
		}
		
		private void ShowIoException(string source, string fileName)
		{
			Logger.Error("{0}: LoadFile IOException: {1}",source, fileName);
			int strt = fileName.Length - 50;
			if (strt < 0) {strt = 0;}
			string txt = string.Format("Could not load file ...{0}!!!",fileName.Substring(strt));
			StatusStripSet(2, txt, Color.Fuchsia);			
            this.Text = appName + " | " + txt;
            lbInfo.Text = "Error loading file";
			lbInfo.BackColor = Color.Fuchsia;
		}
		
        public static string GetAllFootprints(Exception except, bool full=true)
        {
            var st = new StackTrace(except, true);
            var frames = st.GetFrames();
            var traceString = new StringBuilder();
			traceString.Append("Except: " + except.Message + " Source: " + except.Source + " Target: " + except.TargetSite);

			try
			{
				if (frames.Length > 0)
				{	foreach (var frame in frames)
					{
						if (frame.GetFileLineNumber() < 1)
							continue;

						traceString.Append(", File: " + frame.GetFileName());
						traceString.Append(", Method:" + frame.GetMethod().Name);
						traceString.Append(", LineNumber: " + frame.GetFileLineNumber());
						if (!full) 
							break;
						traceString.Append("  -->  ");
					}
				}
				else
				{	traceString.Append(" No frames to add");}
			}
			catch {	return traceString.ToString();}
            return traceString.ToString();
        }
    }
}