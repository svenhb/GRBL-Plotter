/*  GRBL-Plotter. Another GCode sender for GRBL.
    This file is part of the GRBL-Plotter application.
   
    Copyright (C) 2015-2019 Sven Hasemann contact: svenhb@web.de

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
/* MainFormLoadFile
 * Methods to load data (nc, svg, dxf, pictures)
 * Load setups
 * */

//#define debuginfo

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.IO;
using System.Windows.Forms;
using System.Threading;
using System.Xml;
using System.Globalization;

namespace GRBL_Plotter
{
    public partial class MainForm : Form
    {
        private const string extensionDrill = ".drd,.drl,.dri";             //   else if ((ext == ".drd") || (ext == ".drl") || (ext == ".dri"))
        private const string extensionPicture = ".bmp,.gif,.png,.jpg";      //   else if ((ext == ".bmp") || (ext == ".gif") || (ext == ".png") || (ext == ".jpg"))
        private const string extensionGCode = ".nc,.cnc,.gcode";            //   else if (ext == ".nc")

        #region MAIN-MENU FILE
        // open a file via dialog
        private void btnOpenFile_Click(object sender, EventArgs e)
        {
            openFileDialog1.FileName = "";
            openFileDialog1.Filter = "gcode files (*.nc, *.cnc, *.gcode)|*.nc;*.cnc;*.gcode|SVG files (*.svg)|*.svg|DXF files (*.dxf)|*.dxf|Drill files (*.drd, *.drl, *.dri)|*.drd;*.drl;*.dri|All files (*.*)|*.*";
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                loadFile(openFileDialog1.FileName);
                isHeightMapApplied = false;
            }
        }
        // handle MRU List
        private int MRUnumber = 20;
        private List<string> MRUlist = new List<string>();
        private void SaveRecentFile(string path)
        {
            //   recentToolStripMenuItem.DropDownItems.Clear();
            toolStripMenuItem2.DropDownItems.Clear();
            LoadRecentList(); //load list from file
            if (MRUlist.Contains(path)) //prevent duplication on recent list
                MRUlist.Remove(path);
            MRUlist.Insert(0, path);    //insert given path into list on top
                                        //keep list number not exceeded the given value
            while (MRUlist.Count > MRUnumber)
            { MRUlist.RemoveAt(MRUlist.Count - 1); }
            foreach (string item in MRUlist)
            {
                ToolStripMenuItem fileRecent = new ToolStripMenuItem(item, null, RecentFile_click);
                //           recentToolStripMenuItem.DropDownItems.Add(fileRecent);
                toolStripMenuItem2.DropDownItems.Add(fileRecent); //add the menu to "recent" menu
            }
            StreamWriter stringToWrite =
            new StreamWriter(System.Environment.CurrentDirectory + "\\Recent.txt");
            foreach (string item in MRUlist)
            { stringToWrite.WriteLine(item); }
            stringToWrite.Flush(); //write stream to file
            stringToWrite.Close(); //close the stream and reclaim memory
        }
        private void LoadRecentList()
        {
            MRUlist.Clear();
            try
            {
                StreamReader listToRead =
                new StreamReader(System.Environment.CurrentDirectory + "\\Recent.txt");
                string line;
                MRUlist.Clear();
                while ((line = listToRead.ReadLine()) != null) //read each line until end of file
                    MRUlist.Add(line); //insert to list
                listToRead.Close(); //close the stream
            }
            catch (Exception) { }
        }
        private void RecentFile_click(object sender, EventArgs e)
        {
            loadFile(sender.ToString());
        }

        private void newCodeStart()
        {
#if (debuginfo)
            log.Add("MainFormLoadFile newCodeStart");
#endif
            fCTBCode.UnbookmarkLine(fCTBCodeClickedLineLast);
            fCTBCodeClickedLineNow = 0;
            fCTBCodeClickedLineLast = 0;
            Cursor.Current = Cursors.WaitCursor;
            showPicBoxBgImage = false;                  // don't show background image anymore
            pictureBox1.BackgroundImage = null;
            visuGCode.markSelectedFigure(-1);           // hide highlight
            grbl.posMarker = new xyPoint(0, 0);
        }

        private void newCodeEnd()
        {
#if (debuginfo)
            log.Add("MainFormLoadFile newCodeEnd");
#endif
            if (commentOut)
            { fCTB_CheckUnknownCode(); }                                // check code
            visuGCode.getGCodeLines(fCTBCode.Lines);                    // get code path
            visuGCode.calcDrawingArea();                                 // calc ruler dimension
            visuGCode.drawMachineLimit(toolTable.getToolCordinates());
            pictureBox1.Invalidate();                                   // resfresh view
            update_GCode_Depending_Controls();                          // update GUI controls
            updateControls();                                           // update control enable 
            lbInfo.BackColor = SystemColors.Control;
            this.Cursor = Cursors.Default;
            showPaths = true;
        }

        private void loadFile(string fileName)
        {
            if (isStreaming)
            {
                MessageBox.Show("Streaming must be stopped before loading new file","Attention");
                return;
            }
            if (fileName.IndexOf("http") >= 0)
            {
                tBURL.Text = fileName;
                return;
            }
            else
            {
                if (!File.Exists(fileName))
                {
                    MessageBox.Show("File not found: '" + fileName + "'");
                    return;
                }
            }
//            preset2DView();

            String ext = Path.GetExtension(fileName).ToLower();
            if (ext == ".svg")
            { startConvertSVG(fileName); }
            else if (ext == ".dxf")
            { startConvertDXF(fileName); }
            else if (extensionDrill.Contains(ext))  //((ext == ".drd") || (ext == ".drl") || (ext == ".dri"))
            { startConvertDrill(fileName); }
            else if (extensionGCode.Contains(ext))  //(ext == ".nc")
            {
                tbFile.Text = fileName;
                loadGcode();
            }
            else if (extensionPicture.Contains(ext))  //((ext == ".bmp") || (ext == ".gif") || (ext == ".png") || (ext == ".jpg"))
            {
                if (_image_form == null)
                {
                    _image_form = new GCodeFromImage(true);
                    _image_form.FormClosed += formClosed_ImageToGCode;
                    _image_form.btnGenerate.Click += getGCodeFromImage;      // assign btn-click event
                }
                else
                {
                    _image_form.Visible = false;
                }
                _image_form.Show(this);
                _image_form.loadExtern(fileName);
            }
            SaveRecentFile(fileName);
            setLastLoadedFile("Data from file",fileName);

            if (ext == ".url")
            { getURL(fileName); }
            Cursor.Current = Cursors.Default;
            pBoxTransform.Reset();
        }

        private void setLastLoadedFile(string text, string file)
        {
            lastLoadSource = text; showPaths = true;
            lastLoadFile = file;
            if (_setup_form != null)
            { _setup_form.setLastLoadedFile(lastLoadSource); }
        }
        private void getURL(string filename)
        {
            var MyIni = new IniFile(filename);
            tBURL.Text = MyIni.Read("URL", "InternetShortcut");
        }

        // drag and drop file or URL
        private void MainForm_DragEnter(object sender, DragEventArgs e)
        {
            e.Effect = DragDropEffects.All;
        }
        private void MainForm_DragDrop(object sender, DragEventArgs e)
        {
            string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);
            string s = (string)e.Data.GetData(DataFormats.Text);
            if (files != null)
            { loadFile(files[0]); }
            else if (s.Length > 0)
            { tBURL.Text = s; }
            this.WindowState = FormWindowState.Normal;
        }
        private void tBURL_TextChanged(object sender, EventArgs e)
        {
            var parts = tBURL.Text.Split('.');
            string ext = parts[parts.Length - 1].ToLower();   // get extension
            if (ext.IndexOf("svg") >= 0)
            {
                startConvertSVG(tBURL.Text);
                setLastLoadedFile("Data from URL",tBURL.Text);
                tBURL.Text = "";
            }
            else if (ext.IndexOf("dxf") >= 0)
            {
                startConvertDXF(tBURL.Text);
                setLastLoadedFile("Data from URL", tBURL.Text);
                tBURL.Text = "";
            }
            else if (extensionPicture.Contains(ext)) //((ext.ToLower().IndexOf("bmp") >= 0) || (ext.ToLower().IndexOf("gif") >= 0) || (ext.ToLower().IndexOf("png") >= 0) || (ext.ToLower().IndexOf("jpg") >= 0))
            {
                if (_image_form == null)
                {
                    _image_form = new GCodeFromImage(true);
                    _image_form.FormClosed += formClosed_ImageToGCode;
                    _image_form.btnGenerate.Click += getGCodeFromImage;      // assign btn-click event
                }
                else
                {
                    _image_form.Visible = false;
                }
                _image_form.Show(this);
                _image_form.loadURL(tBURL.Text);
                setLastLoadedFile("Data from URL",tBURL.Text);
                tBURL.Text = "";
            }
            else
            {
                if (tBURL.Text.Length > 5)
                {
                    MessageBox.Show("URL extension is not 'svg' or 'dxf'\r\nTry SVG import anyway, but without setting 'Recent File' list.");
                    startConvertSVG(tBURL.Text);
                }
            }
        }
        private void reloadFileToolStripMenuItem_Click(object sender, EventArgs e)
        {
            reStartConvertFile(sender, e);
        }
        public void reStartConvertFile(object sender, EventArgs e)   // event from setup form
        {
            if (!isStreaming)
            {
                this.Cursor = Cursors.WaitCursor;
                if (lastLoadSource.IndexOf("Clipboard") >= 0)
                { loadFromClipboard(); }
                else
                { loadFile(lastLoadFile); }
                this.Cursor = Cursors.Default;
            }
        }
        public void moveToPickup(object sender, EventArgs e)   // event from setup form
        {
            sendCommand(_setup_form.commandToSend);
            _setup_form.commandToSend = "";
        }

        private void startConvertSVG(string source)
        {
#if (debuginfo)
            log.Add("MainFormLoadFile startConvertSVG");
#endif
            newCodeStart();
            fCTBCode.Text = GCodeFromSVG.convertFromFile(source);        // get code
            newCodeEnd();
            SaveRecentFile(source);
            this.Text = appName + " | Source: " + source;
            lbInfo.Text = "SVG-Code loaded";
        }

        private void startConvertDXF(string source)
        {
#if (debuginfo)
            log.Add("MainFormLoadFile startConvertDXF");
#endif
            newCodeStart();
            fCTBCode.Text = GCodeFromDXF.ConvertFromFile(source);
            newCodeEnd();
            SaveRecentFile(source);
            this.Text = appName + " | Source: " + source;
            lbInfo.Text = "DXF-Code loaded";
        }

        private void startConvertDrill(string source)
        {
#if (debuginfo)
            log.Add("MainFormLoadFile startConvertDrill");
#endif
            newCodeStart();
            fCTBCode.Text = GCodeFromDrill.ConvertFile(source);
            newCodeEnd();
            SaveRecentFile(source);
            this.Text = appName + " | Source: " + source;
            lbInfo.Text = "Drill-Code loaded";
        }


        //        bool blockFCTB_Events = true;
        private void loadGcode()
        {
            if (File.Exists(tbFile.Text))
            {
                newCodeStart();
                fCTBCode.OpenFile(tbFile.Text);
                if (_serial_form.isLasermode && Properties.Settings.Default.ctrlReplaceEnable)
                {
                    if (Properties.Settings.Default.ctrlReplaceM3)
                        fCTBCode.Text = "(!!! Replaced M3 by M4 !!!)\r\n" + fCTBCode.Text.Replace("M03", "M04");
                    else
                        fCTBCode.Text = "(!!! Replaced M4 by M3 !!!)\r\n" + fCTBCode.Text.Replace("M04", "M03");
                }
                newCodeEnd();

                SaveRecentFile(tbFile.Text);
                this.Text = appName + " | File: " + tbFile.Text;
                lbInfo.Text = "G-Code loaded";

                if (tbFile.Text.EndsWith(fileLastProcessed + ".nc"))
                {   string fileInfo = Path.ChangeExtension(tbFile.Text, ".xml");
                    if (File.Exists(fileInfo))
                    {
                        int lineNr = loadStreamingStatus();
                        DialogResult dialogResult = MessageBox.Show("The last job was paused at line "+lineNr+" of "+ fCTBCode.LinesCount + ",\r\ndo you want to continue the job?","Attention", MessageBoxButtons.YesNo);
                        if (dialogResult == DialogResult.Yes)
                        {
                            loadStreamingStatus(true);                            //do something
                            updateControls(true);
                            btnStreamStart.Image = Properties.Resources.btn_play;
                            isStreamingPause = true;
                            lbInfo.Text = "Pause streaming - press play ";
                            signalPlay = 1;
                            lbInfo.BackColor = Color.Yellow;
                        }
                    }
                }
                else
                    SaveRecentFile(tbFile.Text);
            }
        }

        // save content from TextEditor (GCode) to file
        private void btnSaveFile_Click(object sender, EventArgs e)
        {
            SaveFileDialog sfd = new SaveFileDialog();
            sfd.Filter = "GCode|*.nc";
            if (sfd.ShowDialog() == DialogResult.OK)
            {
                string txt = fCTBCode.Text;
                File.WriteAllText(sfd.FileName, txt);
            }
        }
        // save Properties.Settings.Default... to text-file
        private void saveMachineParametersToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SaveFileDialog sfd = new SaveFileDialog();
            sfd.Filter = "Machine Ini files (*.ini)|*.ini";
            sfd.FileName = "GRBL-Plotter.ini";
            if (sfd.ShowDialog() == DialogResult.OK)
            {
                var MyIni = new IniFile(sfd.FileName);
                MyIni.WriteAll(_serial_form.GRBLSettings);
            }
        }
        // load Properties.Settings.Default... from text-file
        private void loadMachineParametersToolStripMenuItem_Click(object sender, EventArgs e)
        {
            openFileDialog1.FileName = "GRBL-Plotter.ini";
            openFileDialog1.Filter = "Machine Ini files (*.ini)|*.ini";
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                var MyIni = new IniFile(openFileDialog1.FileName);
                MyIni.ReadAll();
                loadSettings(sender, e);
            }
        }

        // switch language
        private void englishToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Properties.Settings.Default.language = "en";
            MessageBox.Show("Restart of GRBL-Plotter is needed");
        }
        private void deutschToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Properties.Settings.Default.language = "de-DE";
            MessageBox.Show("Ein Neustart von GRBL-Plotter ist erforderlich");
        }
#endregion

        // Ctrl-V to paste graphics
        private void MainForm_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.V && e.Modifiers == Keys.Control)         // ctrl V = paste
            {
                loadFromClipboard();
                e.SuppressKeyPress = true;
            }
            else if ((e.KeyCode == Keys.Space) && (pictureBox1.Focused))    // space = hide pen-up path
            {
                showPathPenUp = false;
                pictureBox1.Invalidate();
                e.SuppressKeyPress = true;
            }

            else if (e.KeyCode == Keys.NumLock)
            {
                virtualJoystickXY.Focus();
                virtualJoystickXY.JoystickRasterMark = virtualJoystickXY_lastIndex;
                virtualJoystickZ.JoystickRasterMark = virtualJoystickZ_lastIndex;
                virtualJoystickA.JoystickRasterMark = virtualJoystickZ_lastIndex;
                e.SuppressKeyPress = true;
            }
            else if (fCTBCode.Focused)
                return;

            e.SuppressKeyPress = processHotkeys(e.KeyData.ToString(), true);
            //   e.SuppressKeyPress = true;
        }

        private void MainForm_KeyUp(object sender, KeyEventArgs e)  // KeyDown in MainFormLoadFile 344
        {
            if ((e.KeyCode == Keys.Space))
            {
                showPathPenUp = true;
                pictureBox1.Invalidate();
            }
            else if (fCTBCode.Focused)
                return;
            processHotkeys(e.KeyData.ToString(), false);
        }

        // paste from clipboard SVG or image
        private void loadFromClipboard()
        {
            newCodeStart();
            string svg_format1 = "image/x-inkscape-svg";
            string svg_format2 = "image/svg+xml";
            IDataObject iData = Clipboard.GetDataObject();
            if (iData.GetDataPresent(DataFormats.Text))             // not working anymore?
            {
                string checkContent = (String)iData.GetData(DataFormats.Text);
                string[] checkLines = checkContent.Split('\n');
                int posSVG = checkContent.IndexOf("<svg ");
                if ((posSVG >= 0) && (posSVG < 100))
                {
                    MemoryStream stream = new MemoryStream();
                    stream = (MemoryStream)iData.GetData("text");
                    byte[] bytes = stream.ToArray();
                    string txt = "";
                    if (!(checkContent.IndexOf("<?xml version") >= 0))
                        txt += "<?xml version=\"1.0\"?>\r\n";
                    txt += System.Text.Encoding.Default.GetString(bytes);
                    if (!(txt.IndexOf("xmlns") >= 0))
                        txt = txt.Replace("<svg", "<svg xmlns=\"http://www.w3.org/2000/svg\" version=\"1.1\" ");

                    newCodeStart();
                    fCTBCode.Text = GCodeFromSVG.convertFromText(txt.Trim((char)0x00), true);    // import as mm
                    newCodeEnd();
                    this.Text = appName + " | Source: from Clipboard";
                    setLastLoadedFile("Data from Clipboard: SVG","");
                    lbInfo.Text = "SVG from clipboard";
                }
                else if ((checkLines[0].Trim() == "0") && (checkLines[1].Trim() == "SECTION"))
                {
                    MemoryStream stream = new MemoryStream();
                    stream = (MemoryStream)iData.GetData("text");
                    byte[] bytes = stream.ToArray();
                    string txt = System.Text.Encoding.Default.GetString(bytes);

                    newCodeStart();
                    fCTBCode.Text = GCodeFromDXF.convertFromText(txt);
                    newCodeEnd();
                    this.Text = appName + " | Source: from Clipboard";
                    setLastLoadedFile("Data from Clipboard: DXF","");
                    lbInfo.Text = "DXF from clipboard";
                }
                else
                {
                    newCodeStart();
                    fCTBCode.Text = (String)iData.GetData(DataFormats.Text);
                    newCodeEnd();
                    setLastLoadedFile("Data from Clipboard: Text","");
                    lbInfo.Text = "GCode from clipboard";
                }
            }
            else if (iData.GetDataPresent(svg_format1) || iData.GetDataPresent(svg_format2))
            {
                MemoryStream stream = new MemoryStream();
                if (iData.GetDataPresent(svg_format1))
                    stream = (MemoryStream)iData.GetData(svg_format1);
                else
                    stream = (MemoryStream)iData.GetData(svg_format2);

                byte[] bytes = stream.ToArray();
                string txt = System.Text.Encoding.Default.GetString(bytes);

                newCodeStart();
                fCTBCode.Text = GCodeFromSVG.convertFromText(txt);
                newCodeEnd();
                this.Text = appName + " | Source: from Clipboard";
                setLastLoadedFile("Data from Clipboard: SVG","");
                lbInfo.Text = "SVG from clipboard";
            }
            else if (iData.GetDataPresent(DataFormats.Bitmap))
            {
                if (_image_form == null)
                {
                    _image_form = new GCodeFromImage(true);
                    _image_form.FormClosed += formClosed_ImageToGCode;
                    _image_form.btnGenerate.Click += getGCodeFromImage;      // assign btn-click event
                }
                else
                {
                    _image_form.Visible = false;
                }
                _image_form.Show(this);
                _image_form.loadClipboard();
                setLastLoadedFile("Data from Clipboard: Image","");
            }
            else
            {
                string tmp = "";
                foreach (string format in iData.GetFormats())
                { tmp += format + "\r\n"; }
                MessageBox.Show(tmp);
            }
        }

        private void pasteFromClipboardToolStripMenuItem_Click(object sender, EventArgs e)
        { loadFromClipboard(); }

        // load settings
        public void loadSettings(object sender, EventArgs e)
        {
#if (debuginfo)
            log.Add("MainFormLoadFile loadSettings");
#endif
            try
            {
                if (Properties.Settings.Default.UpgradeRequired)
                {
                    Properties.Settings.Default.Upgrade();
                    Properties.Settings.Default.UpgradeRequired = false;
                    Properties.Settings.Default.Save();
                }
                tbFile.Text = Properties.Settings.Default.file;
                setCustomButton(btnCustom1, Properties.Settings.Default.custom1, 1);
                setCustomButton(btnCustom2, Properties.Settings.Default.custom2, 2);
                setCustomButton(btnCustom3, Properties.Settings.Default.custom3, 3);
                setCustomButton(btnCustom4, Properties.Settings.Default.custom4, 4);
                setCustomButton(btnCustom5, Properties.Settings.Default.custom5, 5);
                setCustomButton(btnCustom6, Properties.Settings.Default.custom6, 6);
                setCustomButton(btnCustom7, Properties.Settings.Default.custom7, 7);
                setCustomButton(btnCustom8, Properties.Settings.Default.custom8, 8);
                setCustomButton(btnCustom9, Properties.Settings.Default.custom9, 9);
                setCustomButton(btnCustom10, Properties.Settings.Default.custom10, 10);
                setCustomButton(btnCustom11, Properties.Settings.Default.custom11, 11);
                setCustomButton(btnCustom12, Properties.Settings.Default.custom12, 12);
                fCTBCode.BookmarkColor = Properties.Settings.Default.colorMarker; ;
                pictureBox1.BackColor = Properties.Settings.Default.colorBackground;
                //                visuGCode.setColors();
                penUp.Color = Properties.Settings.Default.colorPenUp;
                penDown.Color = Properties.Settings.Default.colorPenDown;
                penRotary.Color = Properties.Settings.Default.colorRotaryInfo;
                penHeightMap.Color = Properties.Settings.Default.colorHeightMap;
                penRuler.Color = Properties.Settings.Default.colorRuler;
                penTool.Color = Properties.Settings.Default.colorTool;
                penMarker.Color = Properties.Settings.Default.colorMarker;
                penHeightMap.Width = (float)Properties.Settings.Default.widthHeightMap;
                penRuler.Width = (float)Properties.Settings.Default.widthRuler;
                penUp.Width = (float)Properties.Settings.Default.widthPenUp;
                penDown.Width = (float)Properties.Settings.Default.widthPenDown;
                penRotary.Width = (float)Properties.Settings.Default.widthRotaryInfo;
                penTool.Width = (float)Properties.Settings.Default.widthTool;
                penMarker.Width = (float)Properties.Settings.Default.widthMarker;
                brushMachineLimit = new HatchBrush(HatchStyle.DiagonalCross, Properties.Settings.Default.colorMachineLimit, Color.Transparent);
                picBoxBackround = new Bitmap(pictureBox1.Width, pictureBox1.Height);
                commentOut = Properties.Settings.Default.ctrlCommentOut;

                joystickXYStep[0] = 0;
                joystickXYStep[1] = (double)Properties.Settings.Default.joyXYStep1;
                joystickXYStep[2] = (double)Properties.Settings.Default.joyXYStep2;
                joystickXYStep[3] = (double)Properties.Settings.Default.joyXYStep3;
                joystickXYStep[4] = (double)Properties.Settings.Default.joyXYStep4;
                joystickXYStep[5] = (double)Properties.Settings.Default.joyXYStep5;
                joystickZStep[0] = 0;
                joystickZStep[1] = (double)Properties.Settings.Default.joyZStep1;
                joystickZStep[2] = (double)Properties.Settings.Default.joyZStep2;
                joystickZStep[3] = (double)Properties.Settings.Default.joyZStep3;
                joystickZStep[4] = (double)Properties.Settings.Default.joyZStep4;
                joystickZStep[5] = (double)Properties.Settings.Default.joyZStep5;
                joystickXYSpeed[0] = 0.1;
                joystickXYSpeed[1] = (double)Properties.Settings.Default.joyXYSpeed1;
                joystickXYSpeed[2] = (double)Properties.Settings.Default.joyXYSpeed2;
                joystickXYSpeed[3] = (double)Properties.Settings.Default.joyXYSpeed3;
                joystickXYSpeed[4] = (double)Properties.Settings.Default.joyXYSpeed4;
                joystickXYSpeed[5] = (double)Properties.Settings.Default.joyXYSpeed5;
                joystickZSpeed[0] = 0.1;
                joystickZSpeed[1] = (double)Properties.Settings.Default.joyZSpeed1;
                joystickZSpeed[2] = (double)Properties.Settings.Default.joyZSpeed2;
                joystickZSpeed[3] = (double)Properties.Settings.Default.joyZSpeed3;
                joystickZSpeed[4] = (double)Properties.Settings.Default.joyZSpeed4;
                joystickZSpeed[5] = (double)Properties.Settings.Default.joyZSpeed5;
                virtualJoystickXY.JoystickLabel = joystickXYStep;
                virtualJoystickZ.JoystickLabel = joystickZStep;
                virtualJoystickA.JoystickLabel = joystickZStep;
                skaliereXAufDrehachseToolStripMenuItem.Enabled = false;
                skaliereXAufDrehachseToolStripMenuItem.BackColor = SystemColors.Control;
                skaliereXAufDrehachseToolStripMenuItem.ToolTipText = "Enable rotary axis in Setup - Control";
                skaliereAufXUnitsToolStripMenuItem.BackColor = SystemColors.Control;
                skaliereAufXUnitsToolStripMenuItem.ToolTipText = "Enable in Setup - Control";
                skaliereYAufDrehachseToolStripMenuItem.Enabled = false;
                skaliereYAufDrehachseToolStripMenuItem.BackColor = SystemColors.Control;
                skaliereYAufDrehachseToolStripMenuItem.ToolTipText = "Enable rotary axis in Setup - Control";
                skaliereAufYUnitsToolStripMenuItem.BackColor = SystemColors.Control;
                skaliereAufYUnitsToolStripMenuItem.ToolTipText = "Enable in Setup - Control";
                toolStrip_tb_rotary_diameter.Text = string.Format("{0:0.00}", Properties.Settings.Default.rotarySubstitutionDiameter);

                if (Properties.Settings.Default.rotarySubstitutionEnable)
                {
                    string tmp = string.Format("Calculating rotary angle depending on part diameter ({0:0.00} units) and desired size.\r\nSet part diameter in Setup - Control.", Properties.Settings.Default.rotarySubstitutionDiameter);
                    if (Properties.Settings.Default.rotarySubstitutionX)
                    {
                        skaliereXAufDrehachseToolStripMenuItem.Enabled = true;
                        skaliereXAufDrehachseToolStripMenuItem.BackColor = Color.Yellow;
                        skaliereAufXUnitsToolStripMenuItem.BackColor = Color.Yellow;
                        skaliereAufXUnitsToolStripMenuItem.ToolTipText = tmp;
                        skaliereXAufDrehachseToolStripMenuItem.ToolTipText = "";
                    }
                    else
                    {
                        skaliereYAufDrehachseToolStripMenuItem.Enabled = true;
                        skaliereYAufDrehachseToolStripMenuItem.BackColor = Color.Yellow;
                        skaliereAufYUnitsToolStripMenuItem.BackColor = Color.Yellow;
                        skaliereAufYUnitsToolStripMenuItem.ToolTipText = tmp;
                        skaliereYAufDrehachseToolStripMenuItem.ToolTipText = "";
                    }
                }
                if (Properties.Settings.Default.rotarySubstitutionSetupEnable)
                {
                    string[] commands;
                    if (Properties.Settings.Default.rotarySubstitutionEnable)
                    { commands = Properties.Settings.Default.rotarySubstitutionSetupOn.Split(';'); }
                    else
                    { commands = Properties.Settings.Default.rotarySubstitutionSetupOff.Split(';'); }
                    if (_serial_form.serialPortOpen)
                        foreach (string cmd in commands)
                        {
                            sendCommand(cmd.Trim());
                            Thread.Sleep(100);
                        }
                }

                ctrl4thAxis = Properties.Settings.Default.ctrl4thUse;
                ctrl4thName = Properties.Settings.Default.ctrl4thName;
                label_a.Visible = ctrl4thAxis;
                label_a.Text = ctrl4thName;
                label_wa.Visible = ctrl4thAxis;
                label_ma.Visible = ctrl4thAxis;
                btnZeroA.Visible = ctrl4thAxis;
                mirrorRotaryToolStripMenuItem.Visible = ctrl4thAxis;
                btnZeroA.Text = "Zero " + ctrl4thName;
                if (Properties.Settings.Default.language == "de-DE")
                    btnZeroA.Text = ctrl4thName + " nullen";

                virtualJoystickA.Visible = ctrl4thAxis;
                btnJogZeroA.Visible = ctrl4thAxis;
                btnJogZeroA.Text = ctrl4thName + "=0";

                resizeJoystick();
                if (ctrl4thAxis)
                {
                    label_status0.Location = new Point(1, 128);
                    label_status.Location = new Point(1, 148);
                    btnHome.Location = new Point(122, 138);
                    btnHome.Size = new Size(117, 30);
                }
                else
                {
                    label_status0.Location = new Point(1, 118);
                    label_status.Location = new Point(1, 138);
                    btnHome.Location = new Point(122, 111);
                    btnHome.Size = new Size(117, 57);
                }
                toolStripViewMachine.Checked = Properties.Settings.Default.machineLimitsShow;
                toolStripViewTool.Checked = Properties.Settings.Default.toolTableShow;
                toolStripViewMachineFix.Checked = Properties.Settings.Default.machineLimitsFix;

                gamePadTimer.Enabled = Properties.Settings.Default.gPEnable;
                checkMachineLimit();
                loadHotkeys();
                newCodeEnd();
            }
            catch (Exception a)
            {
                MessageBox.Show("Load Settings: " + a);
                //               logError("Loading settings", e);
            }
        }
        // Save settings
        public void saveSettings()
        {
            try
            {
                Properties.Settings.Default.file = tbFile.Text;
                Properties.Settings.Default.Save();
            }
            catch (Exception e)
            {
                MessageBox.Show("Save Settings: " + e);
                //               logError("Saving settings", e);
            }
        }
        // update controls on Main form
        public void updateControls(bool allowControl = false)
        {
            bool isConnected = _serial_form.serialPortOpen;
            virtualJoystickXY.Enabled = isConnected && (!isStreaming || allowControl);
            virtualJoystickZ.Enabled = isConnected && (!isStreaming || allowControl);
            virtualJoystickA.Enabled = isConnected && (!isStreaming || allowControl);
            btnCustom1.Enabled = isConnected && (!isStreaming || allowControl);
            btnCustom2.Enabled = isConnected & !isStreaming | allowControl;
            btnCustom3.Enabled = isConnected & !isStreaming | allowControl;
            btnCustom4.Enabled = isConnected & !isStreaming | allowControl;
            btnCustom5.Enabled = isConnected & !isStreaming | allowControl;
            btnCustom6.Enabled = isConnected & !isStreaming | allowControl;
            btnCustom7.Enabled = isConnected & !isStreaming | allowControl;
            btnCustom8.Enabled = isConnected & !isStreaming | allowControl;
            btnCustom9.Enabled = isConnected & !isStreaming | allowControl;
            btnCustom10.Enabled = isConnected & !isStreaming | allowControl;
            btnCustom11.Enabled = isConnected & !isStreaming | allowControl;
            btnCustom12.Enabled = isConnected & !isStreaming | allowControl;
            btnHome.Enabled = isConnected & !isStreaming | allowControl;
            btnZeroX.Enabled = isConnected & !isStreaming | allowControl;
            btnZeroY.Enabled = isConnected & !isStreaming | allowControl;
            btnZeroZ.Enabled = isConnected & !isStreaming | allowControl;
            btnZeroA.Enabled = isConnected & !isStreaming | allowControl;
            btnZeroXY.Enabled = isConnected & !isStreaming | allowControl;
            btnZeroXYZ.Enabled = isConnected & !isStreaming | allowControl;
            btnJogZeroX.Enabled = isConnected & !isStreaming | allowControl;
            btnJogZeroY.Enabled = isConnected & !isStreaming | allowControl;
            btnJogZeroZ.Enabled = isConnected & !isStreaming | allowControl;
            btnJogZeroA.Enabled = isConnected & !isStreaming | allowControl;
            btnJogZeroXY.Enabled = isConnected & !isStreaming | allowControl;
            btnOffsetApply.Enabled = !isStreaming;
            gCodeToolStripMenuItem.Enabled = !isStreaming;

            cBSpindle.Enabled = isConnected & !isStreaming | allowControl;
            tBSpeed.Enabled = isConnected & !isStreaming | allowControl;
            cBCoolant.Enabled = isConnected & !isStreaming | allowControl;
            cBTool.Enabled = isConnected & !isStreaming | allowControl;
            btnReset.Enabled = isConnected;
            btnFeedHold.Enabled = isConnected;
            btnResume.Enabled = isConnected;
            btnKillAlarm.Enabled = isConnected;
            btnStreamStart.Enabled = isConnected;// & isFileLoaded;
            btnStreamStop.Enabled = isConnected; // & isFileLoaded;
            btnStreamCheck.Enabled = isConnected;// & isFileLoaded;

            btnJogStop.Visible = !_serial_form.isGrblVers0;
            btnJogStop.Enabled = isConnected & !isStreaming | allowControl;
            btnOverrideFRGB.Enabled = !_serial_form.isGrblVers0 & isConnected;// & isStreaming | allowControl;
            btnOverrideSSGB.Enabled = !_serial_form.isGrblVers0 & isConnected;// & isStreaming | allowControl;
        }
        // load hotkeys
        private Dictionary<string, string> hotkey = new Dictionary<string, string>();
        private void loadHotkeys()
        {
            hotkey.Clear();
            string fileName = System.Environment.CurrentDirectory + "\\hotkeys.xml";
            if (!File.Exists(fileName))
            {
                MessageBox.Show("File 'hotkeys.xml' not found in program-directory, no hotkeys set!","Attention");
                return;
            }

            XmlReader r = XmlReader.Create("hotkeys.xml");
            while (r.Read())
            {
                if (!r.IsStartElement())
                    continue;

                switch (r.Name)
                {
                    case "hotkeys":
                        break;
                    case "bind":
                        if (r["keydata"].Length > 0)
                        {
                            if (!hotkey.ContainsKey(r["keydata"]))
                                hotkey.Add(r["keydata"], r["action"]);
                        }
                        break;
                }
            }
        }
        private bool processHotkeys(string keyData, bool keyDown)
        {
            if (!hotkey.ContainsKey(keyData))
                return false;
            string action = hotkey[keyData];

            if (action.StartsWith("CustomButton") && keyDown)
            {
                string num = action.Substring("CustomButton".Length);
                int num1;
                if (!int.TryParse(num, out num1))
                    MessageBox.Show("Unknown action: " + action, "Error with Hotkey.xml");
                else
                {
                    if (_serial_form.serialPortOpen && (!isStreaming || isStreamingPause))
                        processCommands(btnCustomCommand[num1]);
                }
                return true;
            }
            if (action.StartsWith("JogAxis") && (virtualJoystickXY.Focused || virtualJoystickZ.Focused || virtualJoystickA.Focused))
            {
                if (keyDown)
                {
                    if (action.Contains("X") || action.Contains("Y"))
                    {
                        int moveX = 0, moveY = 0;
                        if (action.Contains("XDec")) { moveX = -virtualJoystickXY_lastIndex; }
                        if (action.Contains("XInc")) { moveX = virtualJoystickXY_lastIndex; }
                        if (action.Contains("YDec")) { moveY = -virtualJoystickXY_lastIndex; }
                        if (action.Contains("YInc")) { moveY = virtualJoystickXY_lastIndex; }
                        virtualJoystickXY_move(moveX, moveY);
                        return true;
                    }
                    if (action.Contains("ZDec")) { virtualJoystickZ_move(-virtualJoystickZ_lastIndex); return true; }
                    if (action.Contains("ZInc")) { virtualJoystickZ_move(virtualJoystickZ_lastIndex); return true; }
                    if (action.Contains("ADec")) { virtualJoystickA_move(-virtualJoystickZ_lastIndex); return true; }
                    if (action.Contains("AInc")) { virtualJoystickA_move(virtualJoystickZ_lastIndex); return true; }
                }
                else
                { if (!_serial_form.isGrblVers0 && cBSendJogStop.Checked) sendRealtimeCommand(133); return true; }

                if (action.Contains("Stop") && keyDown && !_serial_form.isGrblVers0) { sendRealtimeCommand(133); return true; }

                return false;
            }

            if (keyDown)
            {
                if (action == "JogSpeedXYInc")
                {
                    virtualJoystickXY_lastIndex++;
                    if (virtualJoystickXY_lastIndex > virtualJoystickXY.JoystickRaster) virtualJoystickXY_lastIndex = virtualJoystickXY.JoystickRaster;
                    if (virtualJoystickXY_lastIndex < 1) virtualJoystickXY_lastIndex = 1;
                    virtualJoystickXY.JoystickRasterMark = virtualJoystickXY_lastIndex;
                    return true;
                }
                if (action == "JogSpeedXYDec")
                {
                    virtualJoystickXY_lastIndex--;
                    if (virtualJoystickXY_lastIndex > virtualJoystickXY.JoystickRaster) virtualJoystickXY_lastIndex = virtualJoystickXY.JoystickRaster;
                    if (virtualJoystickXY_lastIndex < 1) virtualJoystickXY_lastIndex = 1;
                    virtualJoystickXY.JoystickRasterMark = virtualJoystickXY_lastIndex;
                    return true;
                }
                if (action == "JogSpeedZInc")
                {
                    virtualJoystickZ_lastIndex++;
                    if (virtualJoystickZ_lastIndex > virtualJoystickZ.JoystickRaster) virtualJoystickZ_lastIndex = virtualJoystickZ.JoystickRaster;
                    if (virtualJoystickZ_lastIndex < 1) virtualJoystickZ_lastIndex = 1;
                    virtualJoystickZ.JoystickRasterMark = virtualJoystickZ_lastIndex;
                    virtualJoystickA.JoystickRasterMark = virtualJoystickZ_lastIndex;
                    return true;
                }
                if (action == "JogSpeedZDec")
                {
                    virtualJoystickZ_lastIndex--;
                    if (virtualJoystickZ_lastIndex > virtualJoystickZ.JoystickRaster) virtualJoystickZ_lastIndex = virtualJoystickZ.JoystickRaster;
                    if (virtualJoystickZ_lastIndex < 1) virtualJoystickZ_lastIndex = 1;
                    virtualJoystickZ.JoystickRasterMark = virtualJoystickZ_lastIndex;
                    virtualJoystickA.JoystickRasterMark = virtualJoystickZ_lastIndex;
                    return true;
                }

                if (action.StartsWith("Stream"))
                {
                    if (action.Contains("Start")) { btnStreamStart.PerformClick(); }
                    if (action.Contains("Stop")) { btnStreamStop.PerformClick(); }
                    if (action.Contains("Check")) { btnStreamCheck.PerformClick(); }
                    return true;
                }
                if (action.StartsWith("Override"))
                {
                    if (action.Contains("FeedInc10")) { btnOverrideFR1.PerformClick(); }
                    else if (action.Contains("FeedInc1")) { btnOverrideFR2.PerformClick(); }
                    else if (action.Contains("FeedDec10")) { btnOverrideFR4.PerformClick(); }
                    else if (action.Contains("FeedDec1")) { btnOverrideFR3.PerformClick(); }
                    else if (action.Contains("FeedSet100")) { btnOverrideFR0.PerformClick(); }
                    else if (action.Contains("SpindleInc10")) { btnOverrideSS1.PerformClick(); }
                    else if (action.Contains("SpindleInc1")) { btnOverrideSS2.PerformClick(); }
                    else if (action.Contains("SpindleDec10")) { btnOverrideSS4.PerformClick(); }
                    else if (action.Contains("SpindleDec1")) { btnOverrideSS3.PerformClick(); }
                    else if (action.Contains("SpindleSet100")) { btnOverrideSS0.PerformClick(); }
                    return true;
                }
                if (action.StartsWith("Offset") && _serial_form.serialPortOpen && (!isStreaming || isStreamingPause))
                {
                    if (action.Contains("XYZ")) { btnZeroXYZ.PerformClick(); }
                    else if (action.Contains("XY")) { btnZeroXY.PerformClick(); }
                    else if (action.Contains("X")) { btnZeroX.PerformClick(); }
                    else if (action.Contains("Y")) { btnZeroY.PerformClick(); }
                    else if (action.Contains("Z")) { btnZeroZ.PerformClick(); }
                    else if (action.Contains("A")) { btnZeroA.PerformClick(); }
                    return true;
                }
                if (action.StartsWith("MoveZero") && _serial_form.serialPortOpen && (!isStreaming || isStreamingPause))
                {
                    if (action.Contains("XY")) { btnJogZeroXY.PerformClick(); }
                    else if (action.Contains("X")) { btnJogZeroX.PerformClick(); }
                    else if (action.Contains("Y")) { btnJogZeroY.PerformClick(); }
                    else if (action.Contains("Z")) { btnJogZeroZ.PerformClick(); }
                    else if (action.Contains("A")) { btnJogZeroA.PerformClick(); }
                    return true;
                }
                if (action.StartsWith("grbl") && _serial_form.serialPortOpen)
                {
                    if (action.Contains("Home")) { btnHome.PerformClick(); }
                    else if (action.Contains("FeedHold")) { btnFeedHold.PerformClick(); }
                    else if (action.Contains("Reset")) { btnReset.PerformClick(); }
                    else if (action.Contains("Resume")) { btnResume.PerformClick(); }
                    else if (action.Contains("KillAlarm")) { btnKillAlarm.PerformClick(); }
                    return true;
                }
                if (action.StartsWith("Toggle") && _serial_form.serialPortOpen)
                {
                    if (action.Contains("ToolInSpindle")) { cBTool.Checked = !cBTool.Checked; }     // order is important...
                    else if(action.Contains("Spindle")) { cBSpindle.Checked = !cBSpindle.Checked; }
                    else if (action.Contains("Coolant")) { cBCoolant.Checked = !cBCoolant.Checked; }
                    return true;
                }
            }
            return false;
        }

        private void saveStreamingStatus(int lineNr)
        {
            string fileName = System.Environment.CurrentDirectory + "\\" + fileLastProcessed + ".xml";
            XmlWriterSettings set = new XmlWriterSettings();
            set.Indent = true;
            XmlWriter w = XmlWriter.Create(fileName, set);
            w.WriteStartDocument();
            w.WriteStartElement("GCode");
            w.WriteAttributeString("lineNr", lineNr.ToString());
            w.WriteAttributeString("lineContent", fCTBCode.Lines[lineNr - 1]);

            w.WriteStartElement("WPos");
            w.WriteAttributeString("X", grbl.posWork.X.ToString().Replace(',', '.'));
            w.WriteAttributeString("Y", grbl.posWork.Y.ToString().Replace(',', '.'));
            w.WriteAttributeString("Z", grbl.posWork.Z.ToString().Replace(',', '.'));
            w.WriteEndElement();

            w.WriteStartElement("Parser");
            w.WriteAttributeString("State", _serial_form.parserStateGC);
            w.WriteEndElement();

            w.WriteEndElement();
            w.Close();
        }

        private int loadStreamingStatus(bool setPause=false)
        {
            string fileName = System.Environment.CurrentDirectory + "\\" + fileLastProcessed + ".xml";
            XmlReader r = XmlReader.Create(fileName);

            xyzPoint tmp = new xyzPoint(0, 0, 0);
            int codeLine = 0;
            string parserState = "";
            while (r.Read())
            {
                if (!r.IsStartElement())
                    continue;

                switch (r.Name)
                {
                    case "GCode":
                        codeLine = int.Parse(r["lineNr"].Replace(',', '.'), NumberFormatInfo.InvariantInfo);
                        break;
                    case "WPos":
                        tmp.X = double.Parse(r["X"].Replace(',', '.'), NumberFormatInfo.InvariantInfo);
                        tmp.Y = double.Parse(r["Y"].Replace(',', '.'), NumberFormatInfo.InvariantInfo);
                        tmp.Z = double.Parse(r["Z"].Replace(',', '.'), NumberFormatInfo.InvariantInfo);
                        break;
                    case "Parser":
                        parserState = r["State"];
                        break;
                }
            }
            r.Close();

            if (setPause)
            {   fCTBCodeClickedLineNow = codeLine;
                fCTBCodeMarkLine();
                _serial_form.parserStateGC = parserState;
                _serial_form.posPause = tmp;
                startStreaming(codeLine);
            }
            return codeLine;
        }


    }

}
