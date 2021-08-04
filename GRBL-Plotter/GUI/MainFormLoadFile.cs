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
/* MainFormLoadFile
 * Methods to load data (nc, svg, dxf, pictures)
 * HotKeys
 * Load setups
 * 2019-03-17 Add custom buttons 13-16, save dialog add *.cnc, *.gcode
 * 2019-04-23 Add virtualJoystickA_lastIndex line 990
 * 2019-05-12 tBURL - disable 2nd event in Line 272
 * 2019-09-28 insert usecase dialog
 * 2019-12-07 Line 221, message on unknown file extension
 * 2019-12-20 in Line 439 replace   "File.WriteAllText(sfd.FileName, txt)" by "fCTBCode.SaveToFile(sfd.FileName, Encoding.Unicode);"
 * 2020-01-01 add trace level loggerTraceImport to hide log of any gcode command during import line 678
 * 2020-01-01 replace #if debuginfo by Logger.Info
 * 2020-02-05 add HPGL format
 * 2020-03-05 export some settings to registry
 * 2020-05-06 add *.tap as gcode file extension
 * 2020-05-29 add CSV support
 * 2020-11-18 line 794, change search range from 100 to 200
 * 2020-12-01 newCodeEnd line 174 add Application.DoEvents()
 * 2021-02-06 lock saveStreamingStatus() line 1677
 * 2021-03-28 btnSaveFile_Click save last path
 * 2021-05-18 line 250 check parser result
 * 2021-07-14 code clean up / code quality
 * 2021-08-03 remove root from MRU save path line 105
*/

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Text;
using System.Windows.Forms;
using System.Xml;

//#pragma warning disable CA1303
//#pragma warning disable CA1304
//#pragma warning disable CA1305
//#pragma warning disable CA1307

namespace GrblPlotter
{
    public partial class MainForm : Form
    {
        private const string extensionGCode = ".nc,.cnc,.ngc,.gcode,.tap";
        private const string extensionDrill = ".drd,.drl,.dri";
        private const string extensionGerber = ".gbr,.ger,.gtl,.gbl,.gts,.gbs,.gto,.gbo,.gko,.g2l,.g3l";
        private const string extensionPicture = ".bmp,.gif,.png,.jpg";
        private const string extensionHPGL = ".plt,.hpgl";
        private const string extensionCSV = ".csv,.dat";

        private const string loadFilter = "G-Code (*.nc, *.cnc, *.ngc, *.gcode, *.tap)|*.nc;*.cnc;*.ngc;*.gcode;*.tap|" +
                                            "SVG - Scalable Vector Graphics|*.svg|" +
                                            "DXF - Drawing Exchange Format |*.dxf|" +
                                            "HPGL - HP Graphics Language (*.plt, *.hpgl)|*.plt;*.hpgl|" +
                                            "CSV  - Comma-separated values (*.csv, *.dat)|*.csv;*.dat|" +
                                            "Drill files (*.drd, *.drl, *.dri)|*.drd;*.drl;*.dri|" +
                                            "Gerber files (*.gbr, *.ger, kicad)|*.gbr;*.ger;*.gtl;*.gbl;*.gts;*.gbs;*.gto;*.gbo;*.gko;*.g2l;*.g3l|" +
                                            "Images (*.bmp,*.gif,*.png,*.jpg)|*.bmp;*.gif;*.png;*.jpg|" +
                                            "All files (*.*)|*.*";


        #region MAIN-MENU FILE
        // open a file via dialog
        private void BtnOpenFile_Click(object sender, EventArgs e)
        {
            openFileDialog1.FileName = "";
            openFileDialog1.Filter = loadFilter;// "gcode files (*.nc, *.cnc, *.ngc, *.gcode)|*.nc;*.cnc;*.ngc;*.gcode|SVG files (*.svg)|*.svg|DXF files (*.dxf)|*.dxf|Drill files (*.drd, *.drl, *.dri)|*.drd;*.drl;*.dri|All files (*.*)|*.*";
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                LoadFile(openFileDialog1.FileName);
                isHeightMapApplied = false;
            }
        }
        // handle MRU List
        private readonly int MRUnumber = 20;
        private string saveName = "";
        private string savePath = Datapath.AppDataFolder;
        private readonly List<string> MRUlist = new List<string>();
        private void SaveRecentFile(string path, bool addPath = true)
        {
            Logger.Trace("SaveRecentFile {0}", path);
            saveName = Path.GetFileNameWithoutExtension(path);
            toolStripMenuItem2.DropDownItems.Clear();
            LoadRecentList(); //load list from file
			
			if (path.StartsWith(Datapath.AppDataFolder))
			{	path = path.Substring(Datapath.AppDataFolder.Length+1);}
			
            if (MRUlist.Contains(path)) //prevent duplication on recent list
                MRUlist.Remove(path);

            if (addPath)
            {
                MRUlist.Insert(0, path);    //insert given path into list on top
                cmsPicBoxReloadFile.ToolTipText = string.Format("Load '{0}'", path);
            }

            //keep list number not exceeded the given value
            while (MRUlist.Count > MRUnumber)
            { MRUlist.RemoveAt(MRUlist.Count - 1); }

            if (MRUlist.Count > 0)
            {	// add list to gui menu
                foreach (string item in MRUlist)
                {
                    ToolStripMenuItem fileRecent = new ToolStripMenuItem(item, null, RecentFile_click);
                    toolStripMenuItem2.DropDownItems.Add(fileRecent); //add the menu to "recent" menu
                }
            }
            try
            {	// write list to file
                StreamWriter stringToWrite = new StreamWriter(Datapath.RecentFile); //System.Environment.CurrentDirectory
                if (MRUlist.Count > 0)
                {
                    foreach (string item in MRUlist)
                    { stringToWrite.WriteLine(item); }
                }
                else
                { stringToWrite.WriteLine("data\\examples\\graphic_grbl.svg"); }
                stringToWrite.Flush(); //write stream to file
                stringToWrite.Close(); //close the stream and reclaim memory
            }
            catch (Exception er) { Logger.Error(er, "SaveRecentFile - StreamWriter '{0}' ", Datapath.RecentFile); }
        }
        private void LoadRecentList()
        {
            Logger.Info("LoadRecentList : {0}", Datapath.RecentFile);
            MRUlist.Clear();
            try
            {
                if (File.Exists(Datapath.RecentFile))
                {
                    StreamReader listToRead = new StreamReader(Datapath.RecentFile);
                    string line;
                    MRUlist.Clear();
                    while ((line = listToRead.ReadLine()) != null) //read each line until end of file
                        MRUlist.Add(line);      //insert to list
                    listToRead.Close();         //close the stream					
                }
                if (MRUlist.Count == 0)
                    MRUlist.Add("data\\examples\\graphic_grbl.svg");
            }
            catch (Exception er) { Logger.Error(er, "LoadRecentList "); }
        }
        private void RecentFile_click(object sender, EventArgs e)
        {
            string mypath = sender.ToString();
            if (!Path.IsPathRooted(mypath))
            {
                mypath = Path.Combine(Datapath.AppDataFolder, mypath); 
            }
            if (!LoadFile(mypath))
            {
                SaveRecentFile(sender.ToString(), false);   // remove nonexisting file-path
                StatusStripSet(1, string.Format("[{0}: {1}]", Localization.GetString("statusStripeFileNotFound"), sender.ToString()), Color.Yellow);
            }
        }

        private static readonly Stopwatch stopwatch = new Stopwatch();
        private void NewCodeStart(bool cleanupGraphic = true)
        {
            Logger.Trace("----newCodeStart++++");
            stopwatch.Start();
            Cursor.Current = Cursors.WaitCursor;
            pBoxTransform.Reset(); zoomFactor = 1;
            showPicBoxBgImage = false;                  // don't show background image anymore
            pictureBox1.BackgroundImage = null;
            pictureBox1.Image = null;
            VisuGCode.ClearDrawingPath();	
            Graphic.pathBackground.Reset();// = new GraphicsPath();

            pictureBox1.Invalidate();                   // resfresh view

            fCTBCode.UnbookmarkLine(fCTBCodeClickedLineLast);
            fCTBCodeClickedLineNow = 0;
            fCTBCodeClickedLineLast = 0;
            fCTBCode.Clear();

            SetEditMode(false);

            VisuGCode.MarkSelectedFigure(-1);           // hide highlight
            VisuGCode.pathBackground.Reset();
            Grbl.PosMarker = new XyzPoint(0, 0, 0);
            if (cleanupGraphic) Graphic.CleanUp();  // clear old data
            StatusStripSet(0, "Start import", Color.LightYellow);
            Application.DoEvents();
        }

        private void NewCodeEnd(bool imported = false)
        {
            int objectCount = Graphic.GetObjectCount();
            Logger.Trace("---- newCodeEnd ++++");

            if (!imported) objectCount = 0;     // no extra handling for GCode

            int maxObjects = 20000;

            if (objectCount > maxObjects)
                StatusStripSet(0, "Display GCode, huge amount of objects (" + objectCount.ToString() + ") - takes more time", Color.Fuchsia);
            else
                StatusStripSet(0, "Display GCode", Color.YellowGreen);

            //           setEditMode(false);

            if (!imported && Properties.Settings.Default.ctrlCommentOut)
            { FctbCheckUnknownCode(); }                              // check code

            Logger.Info("Object count:{0} KB  maxObjects:{1} KB  Process Gcode lines-showProgress:{2}", objectCount, maxObjects, (objectCount > maxObjects));

            if (objectCount <= maxObjects)
            {
                if (imported) SetFctbCodeText(Graphic.GCode.ToString());    // newCodeEnd
                VisuGCode.GetGCodeLines(fCTBCode.Lines, null, null);     	// get code path
            }
            else
            {
                using (VisuWorker f = new VisuWorker())					// GCodeVisuWorker.cs
                {
                    if (!imported)
                    { f.SetTmpGCode(fCTBCode.Lines); }
                    else
                    { f.SetTmpGCode(); }								// take code from Graphic.GCode.ToString()
                    f.ShowDialog(this);									// perform VisuGCode.GetGCodeLines via worker
                    fCTBCode.Text = "PLEASE WAIT !!!\r\nDisplaying a large number of lines\r\ntakes some seconds.";
                }
            }
            VisuGCode.CalcDrawingArea();                                // calc ruler dimension
            VisuGCode.DrawMachineLimit();

            if (loadTimerStep > 0)				// will be set in StartConvert if CodeSize > 250kb (showProgress = true)
            {
                loadTimerStep++;				// will perform SetFctbCodeText(Graphic.GCode.ToString()) in LoadTimer_tick
                loadTimer.Stop();
                loadTimer.Start();
            }

            StatusStripClear(0);
            Update_GCode_Depending_Controls();                          // update GUI controls
            timerUpdateControlSource = "newCodeEnd";
            UpdateControlEnables();                                   	// update control enable 
            lbInfo.BackColor = SystemColors.Control;
            this.Cursor = Cursors.Default;

            cmsPicBoxReverseSelectedPath.Enabled = false;

            pictureBox1.Invalidate();                                   // resfresh view
            Application.DoEvents();                                     // after creating drawing paths

            if (VisuGCode.errorString.Length > 10)						// list GCode import errors if available, and mark editor-lines
            {
                timerShowGCodeError = true;
            }
        }

        private static bool timerShowGCodeError = false;
        private readonly object balanceLock = new object();
        private void ShowGCodeErrors()
        {
            string err = VisuGCode.errorString;
            if (err.Contains("\n"))
            {
                StatusStripSet(0, err.Substring(0, err.IndexOf("\n") - 1), Color.OrangeRed);
                string[] errlines = err.Split('\n');
                Logger.Info("errorstring contains n {0}", errlines.Length);

                lock (balanceLock)
                {
                    for (int i = 0; i < errlines.Length; i++)
                    {
                        string errline = errlines[i];
                        if (errline.Contains("["))                        // find line number e.g. [61] to mark line in editor
                        {
                            int strt = errline.IndexOf("[");
                            int end = errline.IndexOf("]", strt);
                            Logger.Trace("Mark error start:{0} end:{1} from string:'{2}'", strt, end, errline);
                            if ((strt >= 0) && (end > strt))
                            {
                                string numstr = errline.Substring(strt + 1, end - 2);
                                if (int.TryParse(numstr, out int errorLine))
                                {
                                    Logger.Trace("Mark error line:{0} start:{1} end:{2} from string:'{3}'", errorLine, strt, end, numstr);
                                    if ((errorLine > 0) && (errorLine < fCTBCode.LinesCount))
                                    {
                                        ErrorLines.Add(errorLine - 1);
                                        MarkErrorLine(errorLine - 1);
                                    }
                                }
                            }
                        }
                    }
                }
                MessageBox.Show("Errors in GCode, please check:\r\n\r\n" + VisuGCode.errorString, "Attention");
            }
            else
            {
                StatusStripSet(0, VisuGCode.errorString, Color.OrangeRed);
                Logger.Info("errorstring contains not n");
            }
        }

        string importOptions = "";
        //	public enum ConversionType { SVG, DXF, HPGL, CSV, Drill }; 

        private bool LoadFile(string fileName)
        {
            Logger.Info("LoadFile ▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄");
            String ext = Path.GetExtension(fileName).ToLower();
            MainTimer.Stop();
            MainTimer.Start();

            if (ext == ".ini")
            {
                var MyIni = new IniFile(fileName);
                Logger.Info("Load INI: '{0}'", fileName);
                MyIni.ReadAll();    // ReadImport();
                timerUpdateControlSource = "loadFile";
                UpdateControlEnables();
                UpdateWholeApplication();
                StatusStripSet(2, "INI File '" + fileName + "' loaded", Color.Lime);
                return true;
            }
            else
                Logger.Info("Load file {0}", fileName);


            StatusStripSet(1, string.Format("[{0}: {1}]", Localization.GetString("statusStripeFileLoad"), fileName), Color.Lime);
            StatusStripSet(2, "Press 'Space bar' to toggle PenUp path", Color.Lime);

            importOptions = "";
            //            statusStripClear(0, 1);
            //            statusStripClear(2);
            this.Invalidate();      // force gui update

            showPathPenUp = true;
            bool fileLoaded = false;
            SimuStop();
            StatusStripClear(0);

            if (unDoToolStripMenuItem.Enabled)
                VisuGCode.SetPathAsLandMark(true);
            SetUndoText("");
            if (isStreaming)
            {
                Logger.Error(" loadFile not allowed during streaming {0} ", fileName);
                return false;
            }
            if (fileName.IndexOf("http") >= 0)
            {
                tBURL.Text = fileName;
                return true;
            }
            else
            {
                if (!File.Exists(fileName))
                {
                    Logger.Error("▄▄▄▄▄ File not found {0}", fileName);
                    MessageBox.Show(Localization.GetString("mainLoadError1") + fileName + "'", Localization.GetString("mainAttention"));
                    return false;
                }
            }
            if (ext == ".svg")
            {
                if (Properties.Settings.Default.importSVGRezise) importOptions = "<SVG Resize> " + importOptions;
                StartConvert(Graphic.SourceType.SVG, fileName); fileLoaded = true;
            }

            else if ((ext == ".dxf") || (ext == ".dxf~"))
            { StartConvert(Graphic.SourceType.DXF, fileName); fileLoaded = true; }

            else if (extensionDrill.Contains(ext))
            { StartConvert(Graphic.SourceType.Drill, fileName); fileLoaded = true; }

            else if (extensionGerber.Contains(ext))
            { StartConvert(Graphic.SourceType.Gerber, fileName); fileLoaded = true; }

            else if (extensionHPGL.Contains(ext))
            { StartConvert(Graphic.SourceType.HPGL, fileName); fileLoaded = true; }

            else if (extensionCSV.Contains(ext))
            {
                if (Properties.Settings.Default.importCSVAutomatic) importOptions = "<CSV Automatic> " + importOptions;
                StartConvert(Graphic.SourceType.CSV, fileName); fileLoaded = true;
            }

            else if (extensionGCode.Contains(ext))              // extensionGCode = ".nc,.cnc,.ngc,.gcode,.tap";
            {
                tbFile.Text = fileName;                         // hidden textBox
                LoadGcode();
                Properties.Settings.Default.counterImportGCode += 1;
                fileLoaded = true;
            }
            else if (extensionPicture.Contains(ext))  //((ext == ".bmp") || (ext == ".gif") || (ext == ".png") || (ext == ".jpg"))
            {
                if (_image_form == null)
                {
                    _image_form = new GCodeFromImage(true);
                    _image_form.FormClosed += FormClosed_ImageToGCode;
                    _image_form.btnGenerate.Click += GetGCodeFromImage;      // assign btn-click event
                }
                else
                {
                    _image_form.Visible = false;
                }
                _image_form.Show(this);
                _image_form.WindowState = FormWindowState.Normal;
                _image_form.LoadExtern(fileName);
                fileLoaded = true;
            }

            else if (ext == ".txt")
            {
                if (File.Exists(fileName))
                {
                    LoadFromClipboard(File.ReadAllText(fileName));
                    fileLoaded = true;
                    this.Text = appName + " | Source: " + fileName;
                }
            }

            if (ext == ".url")
            { GetURL(fileName); fileLoaded = true; }

            Logger.Info("Load file fileLoaded:{0}", fileLoaded);
            if (fileLoaded)
            {
                SaveRecentFile(fileName);
                SetLastLoadedFile("Data from file", fileName);
                Cursor.Current = Cursors.Default;
                pBoxTransform.Reset();
                EnableCmsCodeBlocks(VisuGCode.CodeBlocksAvailable());
                pictureBox1.Invalidate();
                return true;
            }
            else
            {
                Logger.Error("!!! File could not be converted - unsupported format '{0}'", ext);
                MessageBox.Show(Localization.GetString("mainLoadError3") + ext + "'", Localization.GetString("mainAttention"));
                return false;
            }
        }

        private void SetLastLoadedFile(string text, string file)
        {
            lastLoadSource = text; showPaths = true;
            lastLoadFile = file;
            if (_setup_form != null)
            { _setup_form.SetLastLoadedFile(lastLoadSource); }
        }
        private void GetURL(string filename)
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
            { LoadFile(files[0]); }
            else if (s.Length > 0)
            { tBURL.Text = s; }
            this.WindowState = FormWindowState.Normal;
        }
        private void TbURL_TextChanged(object sender, EventArgs e)
        {
            var parts = tBURL.Text.Split('.');
            string ext = parts[parts.Length - 1].ToLower();   // get extension
            importOptions = "";                                                  //   String ext = Path.GetExtension(fileName).ToLower();
                                                                                 //   MessageBox.Show("-" + ext + "-");			
            Logger.Trace("tBURL_TextChanged: '{0}'", tBURL.Text);
            Logger.Info("LoadFile URL ▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄");
            MainTimer.Stop();
            MainTimer.Start();

            StatusStripSet(1, string.Format("[{0}: {1}]", Localization.GetString("statusStripeFileLoad"), tBURL.Text), Color.Lime);
            StatusStripSet(2, "Press 'Space bar' to toggle PenUp path", Color.Lime);

            if (ext.IndexOf("ini") >= 0)
            {
                Logger.Info("Load INI (URL): '{0}'", tBURL.Text);
                var MyIni = new IniFile(tBURL.Text, true);
                MyIni.ReadAll();    // ReadImport();
                timerUpdateControlSource = "tBURL_TextChanged";
                UpdateControlEnables();
                UpdateWholeApplication();
                StatusStripSet(2, "INI File '" + tBURL.Text + "' loaded", Color.Lime);
                return;
            }

            else if (ext.IndexOf("svg") >= 0)
            {
                if (Properties.Settings.Default.importSVGRezise) importOptions = "<SVG Resize> " + importOptions;
                StartConvert(Graphic.SourceType.SVG, tBURL.Text);
                SaveRecentFile(tBURL.Text);
                SetLastLoadedFile("Data from URL", tBURL.Text);
            }
            else if (ext.IndexOf("dxf") >= 0)
            {
                StartConvert(Graphic.SourceType.DXF, tBURL.Text);
                SaveRecentFile(tBURL.Text);
                SetLastLoadedFile("Data from URL", tBURL.Text);
            }
            else if (extensionHPGL.Contains(ext))
            {
                StartConvert(Graphic.SourceType.HPGL, tBURL.Text);
                SaveRecentFile(tBURL.Text);
                SetLastLoadedFile("Data from URL", tBURL.Text);
            }
            else if (extensionGerber.Contains(ext))
            {
                StartConvert(Graphic.SourceType.Gerber, tBURL.Text);
                SetLastLoadedFile("Data from URL", tBURL.Text);
            }
            else if (extensionCSV.Contains(ext))
            {
                if (Properties.Settings.Default.importCSVAutomatic) importOptions = "<CSV Automatic> " + importOptions;
                StartConvert(Graphic.SourceType.CSV, tBURL.Text);
                SetLastLoadedFile("Data from URL", tBURL.Text);
            }

            else if (extensionPicture.Contains(ext)) //((ext.ToLower().IndexOf("bmp") >= 0) || (ext.ToLower().IndexOf("gif") >= 0) || (ext.ToLower().IndexOf("png") >= 0) || (ext.ToLower().IndexOf("jpg") >= 0))
            {
                if (_image_form == null)
                {
                    _image_form = new GCodeFromImage(true);
                    _image_form.FormClosed += FormClosed_ImageToGCode;
                    _image_form.btnGenerate.Click += GetGCodeFromImage;      // assign btn-click event
                }
                else
                {
                    _image_form.Visible = false;
                }
                _image_form.Show(this);
                _image_form.WindowState = FormWindowState.Normal;
                _image_form.LoadUrl(tBURL.Text);
                SetLastLoadedFile("Data from URL", tBURL.Text);
            }
            else
            {
                if (tBURL.Text.Length > 5)
                {
                    MessageBox.Show(Localization.GetString("mainLoadError2"));
                    StartConvert(Graphic.SourceType.SVG, tBURL.Text);
                }
            }
            tBURL.TextChanged -= TbURL_TextChanged;     // avoid further event
            tBURL.Text = "";
            tBURL.TextChanged += TbURL_TextChanged;
        }

        public void ReStartConvertFile(object sender, EventArgs e)   // event from setup form
        {
            Logger.Info("ReStartConvertFile SourceType:{0}", Graphic.graphicInformation.SourceType);
            if (!isStreaming)
            {
                if (Graphic.graphicInformation.SourceType == Graphic.SourceType.Text)
                {
                    if (_text_form != null)
                    {
                        _text_form.CreateText();        // restart text creation
                        GetGCodeFromText(sender, e);
                    }
                }
                else
                {
                    this.Cursor = Cursors.WaitCursor;
                    if (lastLoadSource.IndexOf("Clipboard") >= 0)
                    { LoadFromClipboard(); }
                    else
                    {
                        if (lastLoadFile.Contains("Nothing"))
                        { LoadFile(MRUlist[0]); }
                        else
                        { LoadFile(lastLoadFile); }
                    }
                    this.Cursor = Cursors.Default;
                }
            }
        }
        public void MoveToPickup(object sender, EventArgs e)   // event from setup form
        {
            SendCommands(_setup_form.commandToSend);
            _setup_form.commandToSend = "";
        }

        private void StartConvert(Graphic.SourceType type, string source)
        {
            Logger.Info("StartConvert type:{0}  source:{1}", type.ToString(), source);
            UseCaseDialog();
            NewCodeStart();
            StatusStripSet(0, "Start import of vector graphic, read graphic elements, process options", Color.Yellow);
            Application.DoEvents();
            string conversionInfo = "";

            /* Show modal progress Dialog if file size is too big */
            bool showProgress = false;
            if (!source.StartsWith("http"))
            {
                FileInfo fs = new FileInfo(source);
                int sizeLimit = 250;
                long filesize = fs.Length / 1024;
                showProgress = filesize > sizeLimit;
                Logger.Info("File size:{0} KB  sizeLimit:{1} KB  Import-showProgress:{2}", filesize, sizeLimit, showProgress);
            }
            loadTimerStep = 0;

            if (showProgress)
            {
                using (ImportWorker f = new ImportWorker())   //MainFormImportWorker
                {
                    f.SetImport(type, source);  // set e.Result = GCodeFromDXF.ConvertFromFile(source, worker, e)
                    f.ShowDialog(this);
                }
            }
            switch (type)
            {
                case Graphic.SourceType.SVG:   // uses Graphic-Class, get result from Graphic.GCode
                    {
                        if (!showProgress) GCodeFromSvg.ConvertFromFile(source, null, null);
                        conversionInfo = GCodeFromSvg.ConversionInfo;
                        Properties.Settings.Default.counterImportSVG += 1;
                        break;
                    }
                case Graphic.SourceType.DXF:   // uses Graphic-Class, get result from Graphic.GCode
                    {
                        if (!showProgress) GCodeFromDxf.ConvertFromFile(source, null, null);
                        conversionInfo = GCodeFromDxf.ConversionInfo;
                        Properties.Settings.Default.counterImportDXF += 1;
                        break;
                    }
                case Graphic.SourceType.HPGL:  // uses Graphic-Class, get result from Graphic.GCode
                    {
                        if (!showProgress) GCodeFromHpgl.ConvertFromFile(source, null, null);
                        conversionInfo = GCodeFromHpgl.ConversionInfo;
                        Properties.Settings.Default.counterImportHPGL += 1;
                        break;
                    }
                case Graphic.SourceType.CSV:   // uses Graphic-Class, get result from Graphic.GCode
                    {
                        if (!showProgress) GCodeFromCsv.ConvertFromFile(source, null, null);
                        conversionInfo = GCodeFromCsv.ConversionInfo;
                        Properties.Settings.Default.counterImportCSV += 1;
                        break;
                    }
                case Graphic.SourceType.Drill:
                    {
                        if (!showProgress) GCodeFromDrill.ConvertFromFile(source, null, null);
                        conversionInfo = GCodeFromDrill.ConversionInfo;
                        Properties.Settings.Default.counterImportDrill += 1;
                        break;
                    }
                case Graphic.SourceType.Gerber:    // uses Graphic-Class, get result from Graphic.GCode
                    {
                        if (!showProgress) GCodeFromGerber.ConvertFromFile(source, null, null);
                        conversionInfo = GCodeFromGerber.conversionInfo;
                        Properties.Settings.Default.counterImportGerber += 1;
                        break;
                    }
                default: break;
            }

            if (!showProgress)
            {
                VisuGCode.xyzSize.AddDimensionXY(Graphic.actualDimension);
                VisuGCode.CalcDrawingArea();                                // calc ruler dimension
            }

            pictureBox1.Invalidate();                                   // resfresh view
            Application.DoEvents();
            this.Invalidate();

            this.Text = appName + " | Source: " + source;

            lbInfo.Text = type.ToString() + "-Code loaded";
            ShowImportOptions();

            if (conversionInfo.Length > 1)
            {
                stopwatch.Stop();
                TimeSpan ts = stopwatch.Elapsed;
                string elapsedTime = String.Format("{0:00}:{1:00}.{2:00} minutes",
                    ts.Minutes, ts.Seconds,
                    ts.Milliseconds / 10);
                conversionInfo += ", duration: " + elapsedTime;

                if (conversionInfo.Contains("Error"))
                    StatusStripSet(2, conversionInfo, Color.Fuchsia);
                else
                    StatusStripSet(2, conversionInfo, Color.YellowGreen);
            }
            Logger.Info(" Conversion info: {0}", conversionInfo);

            if (showProgress)   //  start timer delayed process
            {
                loadTimerStep++;
                loadTimer.Start();
                return;
            }
            NewCodeEnd(true);               // code was imported, no need to check for bad GCode
            FoldCode();
            UpdateControlEnables();
            Logger.Trace("foldCode");
        }

        int loadTimerStep = -1;
        private void LoadTimer_Tick(object sender, EventArgs e)
        {
            switch (loadTimerStep)
            {
                case 1:
                    loadTimer.Stop();
                    NewCodeEnd(true);   // read code line 187
                    break;
                case 2:
                    SetFctbCodeText(Graphic.GCode.ToString());  // loadTimer_Tick
                    FoldCode();
                    loadTimerStep++;
                    break;
                default:
                    loadTimer.Stop();
                    break;
            }
        }

        private void ShowImportOptions()
        {
            Logger.Trace(" showImportOptions {0}", importOptions);
            importOptions = Graphic.graphicInformation.ListOptions() + importOptions;
            if (Properties.Settings.Default.importGCCompress) importOptions += "<Compress> ";
            if (Properties.Settings.Default.importGCRelative) importOptions += "< G91 > ";
            if (Properties.Settings.Default.importGCLineSegmentation) importOptions += "<Line segmentation> ";

            if (importOptions.Length > 1)
            {
                importOptions = "Import options: " + importOptions;
                StatusStripSet(1, importOptions, Color.Yellow);
                Logger.Info(" {0}", importOptions);
            }
            //            Logger.Info(" ImportOptions: {0}", importOptions);
        }
        private void FoldCode()
        {
            if (Properties.Settings.Default.importCodeFold)
            {
                if (Properties.Settings.Default.importGCZIncEnable)
                { foldLevel = 0; }
                else
                { fCTBCode.CollapseAllFoldingBlocks(); foldLevel = 1; }
            }
        }

        private void LoadGcode()
        {
            Logger.Trace(" LoadGCode");
            if (File.Exists(tbFile.Text))
            {
                NewCodeStart();
                fCTBCode.OpenFile(tbFile.Text);

                if (_serial_form.IsLasermode && Properties.Settings.Default.ctrlReplaceEnable)
                {
                    if (Properties.Settings.Default.ctrlReplaceM3)
                        fCTBCode.Text = "(!!! Replaced M3 by M4 !!!)\r\n" + fCTBCode.Text.Replace("M03", "M04");
                    else
                        fCTBCode.Text = "(!!! Replaced M4 by M3 !!!)\r\n" + fCTBCode.Text.Replace("M04", "M03");
                }
                NewCodeEnd();       // -> fCTB_CheckUnknownCode

                SaveRecentFile(tbFile.Text);
                this.Text = appName + " | File: " + tbFile.Text;
                lbInfo.Text = "G-Code loaded";
                UpdateControlEnables();

                if (tbFile.Text.EndsWith(fileLastProcessed + ".nc"))
                {
                    string fileInfo = Path.ChangeExtension(tbFile.Text, ".xml");    // see also saveStreamingStatus
                    if (File.Exists(fileInfo))
                    {
                        int lineNr = LoadStreamingStatus();
                        if (lineNr > 0)
                        {
                            DialogResult dialogResult = MessageBox.Show(Localization.GetString("mainPauseStream1") + lineNr + " / " + fCTBCode.LinesCount + Localization.GetString("mainPauseStream2"), Localization.GetString("mainAttention"), MessageBoxButtons.YesNo);
                            if (dialogResult == DialogResult.Yes)
                            {
                                LoadStreamingStatus(true);                            //do something
                                timerUpdateControlSource = "loadGcode";
                                UpdateControlEnables(true);
                                btnStreamStart.Image = Properties.Resources.btn_play;
                                //                                btnStreamStart.Enabled = _serial_form.serialPortOpen;
                                isStreamingPause = true;
                                lbInfo.Text = Localization.GetString("mainPauseStream");    // "Pause streaming - press play ";
                                signalPlay = 1;
                                lbInfo.BackColor = Color.Yellow;
                            }
                        }
                        else
                            Logger.Trace("loadGcode() check XML lineNr=0");
                    }
                }
                //               else
                //                    SaveRecentFile(tbFile.Text);
                Logger.Trace(" loadGCode end");
            }
        }

        // save content from TextEditor (GCode) to file
        private void BtnSaveFile_Click(object sender, EventArgs e)
        {
            savePath = Properties.Settings.Default.guiPathSaveCode;
            if ((string.IsNullOrEmpty(savePath)) || (!Directory.Exists(Path.GetDirectoryName(savePath))))
            { savePath = Datapath.AppDataFolder; }
            SaveFileDialog sfd = new SaveFileDialog
            {
                InitialDirectory = Path.GetDirectoryName(savePath),
                FileName = saveName + "_",
                Filter = "GCode (*.nc)|*.nc|GCode (*.cnc)|*.cnc|GCode (*.ngc)|*.ngc|GCode (*.gcode)|*.gcode|All files (*.*)|*.*"   // "GCode|*.nc";
            };
            if (sfd.ShowDialog() == DialogResult.OK)
            {
                //   string txt = fCTBCode.Text;
                //File.WriteAllText(sfd.FileName, txt);
                fCTBCode.SaveToFile(sfd.FileName, Encoding.Unicode);
                Logger.Info("Save GCode as {0}", sfd.FileName);
                Properties.Settings.Default.guiPathSaveCode = sfd.FileName;
                Properties.Settings.Default.Save();
            }
            sfd.Dispose();
        }
        // save Properties.Settings.Default... to text-file
        private void SaveMachineParametersToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SaveFileDialog sfd = new SaveFileDialog
            {
                Filter = "Machine Ini files (*.ini)|*.ini",
                FileName = "GRBL-Plotter_" + DateTime.Now.ToString("yyyyMMdd_HHmmss") + ".ini"
            };
            if (sfd.ShowDialog() == DialogResult.OK)
            {
                var MyIni = new IniFile(sfd.FileName);
                MyIni.WriteAll(_serial_form.GRBLSettings, true);	// write all properties, even if default
                Logger.Info("Save machine parameters as {0}", sfd.FileName);
            }
            sfd.Dispose();
        }
        // load Properties.Settings.Default... from text-file
        private void LoadMachineParametersToolStripMenuItem_Click(object sender, EventArgs e)
        {
            openFileDialog1.FileName = "GRBL-Plotter.ini";
            openFileDialog1.Filter = "Machine Ini files (*.ini)|*.ini";
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                var MyIni = new IniFile(openFileDialog1.FileName);
                MyIni.ReadAll();
                LoadSettings(sender, e);
                Logger.Info("Load machine parameters as {0}", openFileDialog1.FileName);
            }
        }

        // switch language
        private static void TryRestart()
        {
            Logger.Info(" Change Language to {0}", Properties.Settings.Default.guiLanguage);
            if (MessageBox.Show("Restart now?", "Attention", MessageBoxButtons.OKCancel) == DialogResult.OK)
            {
                Logger.Info("  tryRestart()");
                Application.Restart();
                Application.ExitThread();  // 20200716
            }
        }

        private void EnglishToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Properties.Settings.Default.guiLanguage = "en";
            MessageBox.Show("Restart of GRBL-Plotter is needed", "Attention");
            TryRestart();
        }
        private void DeutschToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Properties.Settings.Default.guiLanguage = "de-DE";
            MessageBox.Show("Ein Neustart von GRBL-Plotter ist erforderlich", "Achtung");
            TryRestart();
        }
        private void RussianToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Properties.Settings.Default.guiLanguage = "ru";
            MessageBox.Show("Необходим перезапуск GRBL-Plotter.\r\n" +
                "Чтобы улучшить перевод, пожалуйста, откройте вопрос с предлагаемым исправлением на https://github.com/svenhb/GRBL-Plotter/issues", "Внимание");
            TryRestart();
        }
        private void SpanToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Properties.Settings.Default.guiLanguage = "es";
            MessageBox.Show("Es necesario reiniciar GRBL-Plotter.\r\n" +
                "Para mejorar la traducción, abra un problema con la corrección sugerida en https://github.com/svenhb/GRBL-Plotter/issues", "Atención");
            TryRestart();
        }
        private void FranzToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Properties.Settings.Default.guiLanguage = "fr";
            MessageBox.Show("Un redémarrage de GRBL-Plotter est nécessaire.\r\n" +
                "Pour améliorer la traduction, veuillez ouvrir un problème avec la correction suggérée sur https://github.com/svenhb/GRBL-Plotter/issues", "Attention");
            TryRestart();
        }
        private void ChinesischToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Properties.Settings.Default.guiLanguage = "zh-CN";
            MessageBox.Show("需要重启GRBL-Plotter\r\n" +
                "为了改善翻译，请在 https://github.com/svenhb/GRBL-Plotter/issues 上打开建议更正的问题", "注意");
            TryRestart();
        }
        private void PortugisischToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Properties.Settings.Default.guiLanguage = "pt";
            MessageBox.Show("É necessário reiniciar o GRBL-Plotter.\r\n" +
                "Para melhorar a tradução, abra um problema com a correção sugerida em https://github.com/svenhb/GRBL-Plotter/issues", "Atenção");
            TryRestart();
        }
        private void ArabischToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Properties.Settings.Default.guiLanguage = "ar";
            MessageBox.Show("مطلوب إعادة تشغيل GRBL- الراسمة.\r\n" +
                "لتحسين الترجمة ، يرجى فتح مشكلة مع التصحيح المقترح على  https://github.com/svenhb/GRBL-Plotter/issues", "انتباه");
            TryRestart();
        }
        private void JapanischToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Properties.Settings.Default.guiLanguage = "ja";
            MessageBox.Show("GRBL-Plotterの再起動が必要です。\r\n" +
                "翻訳を改善するには、https：//github.com/svenhb/GRBL-Plotter/issuesで修正を提案する問題を開いてください。", "注意");
            TryRestart();
        }

        #endregion

        // Ctrl-V to paste graphics
        private void MainForm_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.V && e.Modifiers == Keys.Control)         // ctrl V = paste
            {
                LoadFromClipboard();
                EnableCmsCodeBlocks(VisuGCode.CodeBlocksAvailable());
                e.SuppressKeyPress = true;
                e.Handled = true;
                return;
            }
            else if ((e.KeyCode == Keys.Space) && (pictureBox1.Focused))    // space = hide pen-up path
            {
                showPathPenUp = !showPathPenUp; // false;
                StatusStripSet(2, "Toggle PenUp path", Color.Lime);
                pictureBox1.Invalidate();
                e.SuppressKeyPress = true;
            }

            else if ((e.KeyCode == Keys.E) && (pictureBox1.Focused))
            { expandGCode = !expandGCode; }

            else if (e.KeyCode == Keys.NumLock)
            {
                virtualJoystickXY.Focus();
                virtualJoystickXY.JoystickRasterMark = virtualJoystickXY_lastIndex;
                virtualJoystickZ.JoystickRasterMark = virtualJoystickZ_lastIndex;
                virtualJoystickA.JoystickRasterMark = virtualJoystickA_lastIndex;
                virtualJoystickB.JoystickRasterMark = virtualJoystickA_lastIndex;
                virtualJoystickC.JoystickRasterMark = virtualJoystickA_lastIndex;
                e.SuppressKeyPress = true;
            }
            else if (fCTBCode.Focused)
                return;

            e.SuppressKeyPress = ProcessHotkeys(e.KeyData.ToString(), true);
            //   e.SuppressKeyPress = true;
        }

        private void MainForm_KeyUp(object sender, KeyEventArgs e)  // KeyDown in MainFormLoadFile 344
        {
            if ((e.KeyCode == Keys.Space))
            {
                StatusStripClear(2);
                //        showPathPenUp = true;
                pictureBox1.Invalidate();
            }
            else if (fCTBCode.Focused)
                return;
            ProcessHotkeys(e.KeyData.ToString(), false);
        }

        // paste from clipboard SVG or image
        private void LoadFromClipboard(string text = "")
        {
            //         Logger.Info(" loadFromClipboard");
            NewCodeStart();
            importOptions = "";
            bool fromClipboard = true;
            if (text.Length > 1)
                fromClipboard = false;
            string svg_format1 = "image/x-inkscape-svg";
            string svg_format2 = "image/svg+xml";
            IDataObject iData = Clipboard.GetDataObject();
            MemoryStream stream = new MemoryStream();
            if ((iData.GetDataPresent(DataFormats.Text)) || (!fromClipboard))            // not working anymore?
            {
                Logger.Info(" loadFromClipboard 1");
                string source = "Textfile";
                string checkContent = "";
                if (fromClipboard)
                { checkContent = (String)iData.GetData(DataFormats.Text); source = "Clipboard"; }
                else
                    checkContent = text;
                string[] checkLines = checkContent.Split(new[] { "\r\n", "\r", "\n" }, StringSplitOptions.None);
                int posSVG = checkContent.IndexOf("<svg ");
                if ((posSVG >= 0) && (posSVG < 200))
                {
                    string txt = "";
                    if (!(checkContent.IndexOf("<?xml version") >= 0))
                        txt += "<?xml version=\"1.0\"?>\r\n";
                    if (fromClipboard)
                    {
                        //                       MemoryStream stream = new MemoryStream();
                        stream = (MemoryStream)iData.GetData("text");
                        byte[] bytes = stream.ToArray();
                        stream.Dispose();
                        txt += System.Text.Encoding.Default.GetString(bytes);
                    }
                    else
                        txt += checkContent;
                    if (!(txt.IndexOf("xmlns") >= 0))
                        txt = txt.Replace("<svg", "<svg xmlns=\"http://www.w3.org/2000/svg\" version=\"1.1\" ");

                    UseCaseDialog();
                    NewCodeStart();
                    GCodeFromSvg.ConvertFromText(txt.Trim((char)0x00), true);    // replaceUnitByPixel = true,  import as mm
                    SetFctbCodeText(Graphic.GCode.ToString());      // loadFromClipboard
                    Properties.Settings.Default.counterImportSVG += 1;
                    if (fCTBCode.LinesCount <= 1)
                    { fCTBCode.Text = "( Code conversion failed )"; return; }
                    NewCodeEnd(true);               // code was imported, no need to check for bad GCode

                    this.Text = appName + " | Source: from " + source;
                    SetLastLoadedFile("Data from " + source + ": SVG", "");
                    lbInfo.Text = "SVG from " + source;
                    if (Properties.Settings.Default.importSVGRezise) importOptions = "<SVG Resize> " + importOptions;
                    ShowImportOptions();
                }
                else if ((checkLines[0].Trim() == "0") && (checkLines[1].Trim() == "SECTION"))
                {
                    string txt = "";
                    if (fromClipboard)
                    {
                        //          MemoryStream stream = new MemoryStream();
                        stream = (MemoryStream)iData.GetData("text");
                        byte[] bytes = stream.ToArray();
                        stream.Dispose();
                        txt = System.Text.Encoding.Default.GetString(bytes);
                    }
                    else
                        txt += checkContent;

                    UseCaseDialog();
                    NewCodeStart();
                    GCodeFromDxf.ConvertFromText(txt);
                    SetFctbCodeText(Graphic.GCode.ToString());      // loadFromClipboard

                    Properties.Settings.Default.counterImportDXF += 1;
                    if (fCTBCode.LinesCount <= 1)
                    { fCTBCode.Text = "( Code conversion failed )"; return; }
                    NewCodeEnd(true);               // code was imported, no need to check for bad GCode

                    this.Text = appName + " | Source: from " + source;
                    SetLastLoadedFile("Data from " + source + ": DXF", "");
                    lbInfo.Text = "DXF from " + source;
                    ShowImportOptions();
                }
                else
                {
                    NewCodeStart();
                    if (fromClipboard)
                        fCTBCode.Text = (String)iData.GetData(DataFormats.Text);
                    else
                        fCTBCode.Text = text;
                    Properties.Settings.Default.counterImportGCode += 1;
                    NewCodeEnd();
                    SetLastLoadedFile("Data from " + source + ": Text", "");
                    lbInfo.Text = "GCode from " + source;
                }
            }
            else if (iData.GetDataPresent(svg_format1) || iData.GetDataPresent(svg_format2))
            {
                Logger.Info(" loadFromClipboard 2");
                //    MemoryStream stream = new MemoryStream();
                if (iData.GetDataPresent(svg_format1))
                    stream = (MemoryStream)iData.GetData(svg_format1);
                else
                    stream = (MemoryStream)iData.GetData(svg_format2);

                byte[] bytes = stream.ToArray();
       //         stream.Dispose();
                string txt = System.Text.Encoding.Default.GetString(bytes);

                UseCaseDialog();
                NewCodeStart();
                GCodeFromSvg.ConvertFromText(txt, false);       // replaceUnitByPixel = false
                SetFctbCodeText(Graphic.GCode.ToString());      // loadFromClipboard

                Properties.Settings.Default.counterImportSVG += 1;
                if (fCTBCode.LinesCount <= 1)
                { fCTBCode.Text = "( Code conversion failed )"; return; }
                NewCodeEnd(true);               // code was imported, no need to check for bad GCode

                this.Text = appName + " | Source: from Clipboard";
                SetLastLoadedFile("Data from Clipboard: SVG", "");
                lbInfo.Text = "SVG from clipboard";
                if (Properties.Settings.Default.importSVGRezise) importOptions = "<SVG Resize> " + importOptions;
                ShowImportOptions();
            }
            else if (iData.GetDataPresent(DataFormats.Bitmap))
            {
                Logger.Info(" loadFromClipboard 3");
                if (_image_form == null)
                {
                    _image_form = new GCodeFromImage(true);
                    _image_form.FormClosed += FormClosed_ImageToGCode;
                    _image_form.btnGenerate.Click += GetGCodeFromImage;      // assign btn-click event
                }
                else
                {
                    _image_form.Visible = false;
                }
                _image_form.Show(this);
                _image_form.WindowState = FormWindowState.Normal;
                _image_form.LoadClipboard();
                Properties.Settings.Default.counterImportImage += 1;
                SetLastLoadedFile("Data from Clipboard: Image", "");
            }
            else
            {
                Logger.Info(" loadFromClipboard 4");
                string tmp = "";
                foreach (string format in iData.GetFormats())
                { tmp += format + "\r\n"; }
                MessageBox.Show(tmp);
            }
            stream.Dispose();
        }

        // Save settings
        public void SaveSettings()
        {
            try
            {
                Properties.Settings.Default.guiLastFileLoaded = tbFile.Text;
                Properties.Settings.Default.Save();
            }
            catch (Exception e)
            {
                MessageBox.Show("Save Settings: " + e);
                Logger.Error(e, "saveSettings() ");
            }
        }

        #region HotKeys
        // load hotkeys
        private readonly Dictionary<string, string> hotkey = new Dictionary<string, string>();
        private readonly Dictionary<string, string> hotkeyCode = new Dictionary<string, string>();
        private readonly XmlReaderSettings settings = new XmlReaderSettings()
        { DtdProcessing = DtdProcessing.Prohibit };
        private void LoadHotkeys()
        {
            Logger.Trace("loadHotkeys");
            hotkey.Clear();
            hotkeyCode.Clear();
            string fileName = Datapath.Hotkeys;
            if (!File.Exists(fileName))
            {
                Logger.Error("File 'hotkeys.xml' not found in {0}", fileName);
                return;
            }

            XmlReader content = XmlReader.Create(fileName, settings);   // "hotkeys.xml");
            while (content.Read())
            {
                if (!content.IsStartElement())
                    continue;

                switch (content.Name)
                {
                    case "hotkeys":
                        break;
                    case "bind":
                        if ((content["keydata"].Length > 0) && (content["action"] != null))
                        {
                            if (!hotkey.ContainsKey(content["keydata"]))
                                hotkey.Add(content["keydata"], content["action"]);
                        }
                        else if ((content["keydata"].Length > 0) && (content["code"] != null))
                        {
                            if (!hotkeyCode.ContainsKey(content["keydata"]))
                                hotkeyCode.Add(content["keydata"], content["code"]);
                        }
                        break;
                }
            }
            //	content.Dispose();
        }
        private bool ProcessHotkeys(string keyData, bool keyDown)
        {
            if (hotkeyCode.TryGetValue(keyData, out string code)) // Returns true.
            { if (!keyDown) ProcessCommands(code); }

            else if (hotkey.TryGetValue(keyData, out string action)) // Returns true.
            {
                if (action.StartsWith("CustomButton") && keyDown)
                {
                    string num = action.Substring("CustomButton".Length);
                    //   int num1;
                    if (!int.TryParse(num, out int num1))
                        MessageBox.Show(Localization.GetString("mainHotkeyError1") + action, Localization.GetString("mainHotkeyError2"));
                    else
                    {
                        if (_serial_form.SerialPortOpen && (!isStreaming || isStreamingPause) || Grbl.grblSimulate)
                            ProcessCommands(btnCustomCommand[num1]);
                    }
                    return true;
                }
                if (action.StartsWith("JogAxis") && (virtualJoystickXY.Focused || virtualJoystickZ.Focused || virtualJoystickA.Focused || virtualJoystickB.Focused || virtualJoystickC.Focused))
                {
                    if (keyDown)
                    {
                        bool cmdFound = false;
                        if (action.Contains("X") || action.Contains("Y"))
                        {
                            int moveX = 0, moveY = 0;
                            if (action.Contains("XDec")) { moveX = -virtualJoystickXY_lastIndex; }
                            if (action.Contains("XInc")) { moveX = virtualJoystickXY_lastIndex; }
                            if (action.Contains("YDec")) { moveY = -virtualJoystickXY_lastIndex; }
                            if (action.Contains("YInc")) { moveY = virtualJoystickXY_lastIndex; }
                            VirtualJoystickXY_move(moveX, moveY);
                            cmdFound = true;
                        }
                        if (action.Contains("ZDec")) { VirtualJoystickZ_move(-virtualJoystickZ_lastIndex); cmdFound = true; }
                        if (action.Contains("ZInc")) { VirtualJoystickZ_move(virtualJoystickZ_lastIndex); cmdFound = true; }
                        if (action.Contains("ADec")) { VirtualJoystickA_move(-virtualJoystickA_lastIndex, ctrl4thName); cmdFound = true; }
                        if (action.Contains("AInc")) { VirtualJoystickA_move(virtualJoystickA_lastIndex, ctrl4thName); cmdFound = true; }
                        if (cmdFound)
                        {
                            virtualJoystickXY.JoystickRasterMark = virtualJoystickXY_lastIndex;
                            virtualJoystickZ.JoystickRasterMark = virtualJoystickZ_lastIndex;
                            virtualJoystickA.JoystickRasterMark = virtualJoystickA_lastIndex;
                            virtualJoystickB.JoystickRasterMark = virtualJoystickA_lastIndex;
                            virtualJoystickC.JoystickRasterMark = virtualJoystickA_lastIndex;
                            return true;
                        }
                    }
                    else
                    { if (!Grbl.isVersion_0 && cBSendJogStop.Checked) SendRealtimeCommand(133); return true; }

                    if (action.Contains("Stop") && keyDown && !Grbl.isVersion_0) { SendRealtimeCommand(133); return true; }

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
                        return true;
                    }
                    if (action == "JogSpeedZDec")
                    {
                        virtualJoystickZ_lastIndex--;
                        if (virtualJoystickZ_lastIndex > virtualJoystickZ.JoystickRaster) virtualJoystickZ_lastIndex = virtualJoystickZ.JoystickRaster;
                        if (virtualJoystickZ_lastIndex < 1) virtualJoystickZ_lastIndex = 1;
                        virtualJoystickZ.JoystickRasterMark = virtualJoystickZ_lastIndex;
                        return true;
                    }
                    if (action == "JogSpeedAInc")
                    {
                        virtualJoystickA_lastIndex++;
                        if (virtualJoystickA_lastIndex > virtualJoystickA.JoystickRaster) virtualJoystickA_lastIndex = virtualJoystickA.JoystickRaster;
                        if (virtualJoystickA_lastIndex < 1) virtualJoystickA_lastIndex = 1;
                        virtualJoystickA.JoystickRasterMark = virtualJoystickA_lastIndex;
                        return true;
                    }
                    if (action == "JogSpeedADec")
                    {
                        virtualJoystickA_lastIndex--;
                        if (virtualJoystickA_lastIndex > virtualJoystickA.JoystickRaster) virtualJoystickA_lastIndex = virtualJoystickA.JoystickRaster;
                        if (virtualJoystickA_lastIndex < 1) virtualJoystickA_lastIndex = 1;
                        virtualJoystickA.JoystickRasterMark = virtualJoystickA_lastIndex;
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
                    if (action.StartsWith("Offset") && _serial_form.SerialPortOpen && (!isStreaming || isStreamingPause))
                    {
                        if (action.Contains("XYZ")) { btnZeroXYZ.PerformClick(); }
                        else if (action.Contains("XY")) { btnZeroXY.PerformClick(); }
                        else if (action.Contains("X")) { btnZeroX.PerformClick(); }
                        else if (action.Contains("Y")) { btnZeroY.PerformClick(); }
                        else if (action.Contains("Z")) { btnZeroZ.PerformClick(); }
                        else if (action.Contains("A")) { btnZeroA.PerformClick(); }
                        return true;
                    }
                    if (action.StartsWith("MoveZero") && _serial_form.SerialPortOpen && (!isStreaming || isStreamingPause))
                    {
                        if (action.Contains("XY")) { btnJogZeroXY.PerformClick(); }
                        else if (action.Contains("X")) { btnJogZeroX.PerformClick(); }
                        else if (action.Contains("Y")) { btnJogZeroY.PerformClick(); }
                        else if (action.Contains("Z")) { btnJogZeroZ.PerformClick(); }
                        else if (action.Contains("A")) { btnJogZeroA.PerformClick(); }
                        return true;
                    }
                    if (action.StartsWith("grbl") && _serial_form.SerialPortOpen)
                    {
                        if (action.Contains("Home")) { btnHome.PerformClick(); }
                        else if (action.Contains("FeedHold")) { btnFeedHold.PerformClick(); }
                        else if (action.Contains("Reset")) { btnReset.PerformClick(); }
                        else if (action.Contains("Resume")) { btnResume.PerformClick(); }
                        else if (action.Contains("KillAlarm")) { btnKillAlarm.PerformClick(); }
                        return true;
                    }
                    if (action.StartsWith("Toggle") && _serial_form.SerialPortOpen)
                    {
                        if (action.Contains("ToolInSpindle")) { cBTool.Checked = !cBTool.Checked; }     // order is important...
                        else if (action.Contains("Spindle")) { cBSpindle.Checked = !cBSpindle.Checked; }
                        else if (action.Contains("Coolant")) { cBCoolant.Checked = !cBCoolant.Checked; }
                        return true;
                    }
                }
            }
            return false;
        }
        #endregion

        static readonly object lockSaveAction = new object();
        private void SaveStreamingStatus(int lineNr)
        {
            lock (lockSaveAction)
            {
                string fileName = Datapath.AppDataFolder + "\\" + fileLastProcessed + ".xml";  //System.Environment.CurrentDirectory
                XmlWriterSettings set = new XmlWriterSettings
                {
                    Indent = true
                };
                XmlWriter content = XmlWriter.Create(fileName, set);
                content.WriteStartDocument();
                content.WriteStartElement("GCode");
                content.WriteAttributeString("lineNr", lineNr.ToString());
                if (lineNr > 0)
                    content.WriteAttributeString("lineContent", fCTBCode.Lines[lineNr - 1]);
                else
                    content.WriteAttributeString("lineContent", fCTBCode.Lines[0]);

                content.WriteStartElement("WPos");
                content.WriteAttributeString("X", Grbl.posWork.X.ToString().Replace(',', '.'));
                content.WriteAttributeString("Y", Grbl.posWork.Y.ToString().Replace(',', '.'));
                content.WriteAttributeString("Z", Grbl.posWork.Z.ToString().Replace(',', '.'));
                content.WriteEndElement();

                content.WriteStartElement("Parser");
                content.WriteAttributeString("State", _serial_form.parserStateGC);
                content.WriteEndElement();

                content.WriteEndElement();
                content.Close();
            }
        }

        private int LoadStreamingStatus(bool setPause = false)
        {
            string fileName = Datapath.AppDataFolder + "\\" + fileLastProcessed + ".xml";
            if (!File.Exists(fileName))
                return 0;
            FileInfo fi = new FileInfo(fileName);
            if (fi.Length > 1)
            {
                XmlReader content = XmlReader.Create(fileName, settings);

                XyzPoint tmp = new XyzPoint(0, 0, 0);
                int codeLine = 0;
                string parserState = "";
                while (content.Read())
                {
                    if (!content.IsStartElement())
                        continue;

                    switch (content.Name)
                    {
                        case "GCode":
                            codeLine = int.Parse(content["lineNr"].Replace(',', '.'), NumberFormatInfo.InvariantInfo);
                            break;
                        case "WPos":
                            tmp.X = double.Parse(content["X"].Replace(',', '.'), NumberFormatInfo.InvariantInfo);
                            tmp.Y = double.Parse(content["Y"].Replace(',', '.'), NumberFormatInfo.InvariantInfo);
                            tmp.Z = double.Parse(content["Z"].Replace(',', '.'), NumberFormatInfo.InvariantInfo);
                            break;
                        case "Parser":
                            parserState = content["State"];
                            break;
                    }
                }
                content.Close();

                if (setPause)
                {
                    fCTBCodeClickedLineNow = codeLine;
                    FctbCodeMarkLine();
                    _serial_form.parserStateGC = parserState;
                    _serial_form.posPause = tmp;
                    StartStreaming(codeLine);
                }
                return codeLine;
            }
            Logger.Trace("loadStreamingStatus fileSize=0 {0}", fileName);
            return 0;
        }

        private void UseCaseDialog()
        {
            if (Properties.Settings.Default.importShowUseCaseDialog)
            {
                using (ControlSetupUseCase f = new ControlSetupUseCase())
                {
                    var result = f.ShowDialog(this);
                    if (result == DialogResult.OK)
                    {
                        _serial_form.RequestSend(f.ReturnValue1, true); // set or clear lasermode $32=x
                        _serial_form.ReadSettings();
                    }
                }
            }
        }


        // handle Extension List
        private void LoadExtensionList()
        {
            Logger.Trace("LoadExtensionList");
            string extensionPath = Datapath.Extension;
            string[] fileEntries;

            try
            {
                if (Directory.Exists(extensionPath))
                {
                    fileEntries = Directory.GetFiles(extensionPath);
                    foreach (string item in fileEntries)
                    {
                        string file = Path.GetFileName(item);
                        if (!file.StartsWith("_"))
                        {
                            ToolStripMenuItem fileExtension = new ToolStripMenuItem(file, null, ExtensionFile_click);
                            startExtensionToolStripMenuItem.DropDownItems.Add(fileExtension);
                            Logger.Trace("  - Add Extension {0}", file);
                        }
                    }
                }
                else Logger.Warn("Extension path not found {0}", extensionPath);
            }
            catch (Exception err) { Logger.Error(err, "LoadExtensionList "); }
        }
        private void ExtensionFile_click(object sender, EventArgs e)
        {
            string tmp = Datapath.Extension + "\\" + sender.ToString();
            //            MessageBox.Show(tmp);
            Logger.Debug("Start Extension {0}", tmp);
            try { System.Diagnostics.Process.Start(tmp); }
            catch (Exception er) { Logger.Error(er, "ExtensionFile_click Start Process {0} ", tmp); }
        }
    }
}
