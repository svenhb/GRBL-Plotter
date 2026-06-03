/*  GRBL-Plotter. Another GCode sender for GRBL.
    This file is part of the GRBL-Plotter application.
   
    Copyright (C) 2015-2024 Sven Hasemann contact: svenhb@web.de

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
 * 2024-02-12 split file MainFormOtherForms.cs
 * 2024-12-02 l:114 f:OnRaiseProcessEvent add "G-Code Data"
 * 2026-06-02 add "CreateText Size", "CreateText Align" and "CreateText LineDistance" automation commands
 * 2026-06-02 add "2D-View Regenerate" to re-create the current graphic with current pen/import settings
*/

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Windows.Forms;

namespace GrblPlotter
{
    public partial class MainForm : Form
    {
        /********************************************************************
         * ProcessAutomation - a form to automate process steps
         * _process_form
         ********************************************************************/
        private void ProcessAutomationFormOpen(object sender, EventArgs e)
        {
            if (_process_form == null)
            {
                _process_form = new ProcessAutomation();
                _process_form.FormClosed += FormClosed_Process;
                _process_form.RaiseProcessEvent += OnRaiseProcessEvent;
                EventCollector.SetOpenForm("Fprc");
            }
            else
            {
                _process_form.Visible = false;
            }

            if (showFormInFront) _process_form.Show(this);
            else _process_form.Show();  // this);
                                        //   _process_form.Show(this);
            showFormsToolStripMenuItem.Visible = true;
            _process_form.WindowState = FormWindowState.Normal;
        }
        private void FormClosed_Process(object sender, FormClosedEventArgs e)
        { _process_form = null; EventCollector.SetOpenForm("FCprc"); }



        /********************************************************************
         * ProcessAutomation - a form to automate process steps
         * Process commands
         ********************************************************************/
      //  private static List<Process> processURL = new List<Process>();
        private void OnRaiseProcessEvent(object sender, ProcessEventArgs e)
        {
            string act = e.Command.ToLower();
            //    string val = e.Value.ToLower();

            Logger.Trace("➤➤➤➤ OnRaiseProcessEvent  {0}  {1} ", e.Command, e.Value);

            if (act.Contains("load"))
            {
                if (act.Contains("url"))
                {
                    bool ok = true;
                    try
                    {
                      //  processURL.Add(
                            Process.Start(e.Value);
                      /*  if (processURL.Count > 2)
                        {
                            Logger.Trace("kill start");
                        //    processURL[0].Kill();
                            Logger.Trace("kill end");
                        }
                        if (processURL.Count > 3)
                        {
                            Logger.Trace("remove start");
                            processURL.RemoveAt(0);
                            Logger.Trace("remove end");
                        }*/
                    }
                    catch (Exception err)
                    { ok = false; Logger.Error(err," err ");
                        MessageBox.Show("Could not open URL : " + err.Message, "Error"); }
                    _process_form?.Feedback(e.Command, e.Value, ok);
                    _process_form?.BringToFront();
                }
                else
                {
                    string mypath = Datapath.MakeAbsolutePath(e.Value);
                    _process_form?.Feedback(e.Command, e.Value, LoadFile(mypath));
                    _process_form?.BringToFront();
                }
            }
            else if (act.Contains("paste clipboard"))
            {
                _process_form?.Feedback(e.Command, e.Value, LoadFromClipboard());
                _process_form?.BringToFront();
            }
            else if (act.Contains("g-code"))
            {
                if (act.Contains("send") || act.Contains("data"))
                    ProcessCommands(e.Value);		//SendCommands(e.Value);	includes macro-file
                if (act.Contains("stream"))
                    StartStreaming(0, fCTBCode.LinesCount - 1);
            }
            else if (act == "probe automatic")
            {
                if (_probing_form == null)
                { probingToolLengthToolStripMenuItem.PerformClick(); }
                if (_probing_form != null)
                {
                    _probing_form.StartProbing(e.Value.ToUpper());
                    // when finished, probing form sends event back _probing_form.RaiseProcessEvent += OnRaiseProbingProcessEvent;
                }
                else
                {
                    _process_form?.Feedback(e.Command, "Probing form is not open", false);
                }
            }
            else if (act == "camera automatic")
            {
                if (_camera_form == null)
                { toolStripMenuItem1.PerformClick(); }
                if (_camera_form != null)
                {
                    _camera_form.StartFiducialDetection();
                    // when finished, camera form sends event back _camera_form.RaiseProcessEvent += OnRaiseCameraProcessEvent;
                }
                else
                {
                    _process_form?.Feedback(e.Command, "Camera form is not open", false);
                }
            }

            else if (act.Contains("createtext"))
            {
                if (_text_form == null)
                { textWizzardToolStripMenuItem.PerformClick(); }

                if (_text_form != null)
                {
                    if (act.Contains("size"))           // CreateText Size  -> Value = height in mm
                    {
                        if (double.TryParse(e.Value.Replace(',', '.'), System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture, out double sz))
                        {
                            _text_form.SetFontSize(sz);
                            _process_form?.Feedback(e.Command, "size " + e.Value, true);
                        }
                        else
                        { _process_form?.Feedback(e.Command, "bad size: " + e.Value, false); }
                    }
                    else if (act.Contains("align"))     // CreateText Align -> Value = left|center|right (or 1|2|3)
                    {
                        _text_form.SetAlignment(e.Value);
                        _process_form?.Feedback(e.Command, "align " + e.Value, true);
                    }
                    else if (act.Contains("line") || act.Contains("distance") || act.Contains("spacing"))   // CreateText LineDistance -> Value = mm
                    {
                        if (double.TryParse(e.Value.Replace(',', '.'), System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture, out double ld))
                        {
                            _text_form.SetLineDistance(ld);
                            _process_form?.Feedback(e.Command, "line distance " + e.Value, true);
                        }
                        else
                        { _process_form?.Feedback(e.Command, "bad line distance: " + e.Value, false); }
                    }
                    else
                    {
                        _text_form.SetText(e.Value);
                        _process_form?.Feedback(e.Command, e.Value, true);
                    }
                }
                else
                {
                    _process_form?.Feedback(e.Command, "Text form is not open", false);
                }
            }

            else if (act.Contains("createbarcode"))	// == "Barcode 1D Text")
            {
                if (_barcode_form == null)
                { createBarcodeToolStripMenuItem.PerformClick(); }

                if (_barcode_form != null)
                {
                    if (act.Contains("1d text")) { _barcode_form.SetText1D(e.Value); }
                    else if (act.Contains("1d data")) { _barcode_form.SetText1D(e.Value); }
                    else if (act.Contains("2d text")) { _barcode_form.SetText2D(e.Value); }
                    else if (act.Contains("2d data")) { _barcode_form.SetText2D(e.Value); }
                    else if (act.Contains("2d url")) { _barcode_form.SetUrl2D(e.Value); }
                    else if (act.Contains("2d durl")) { _barcode_form.SetUrl2D(e.Value); }
                    _process_form?.Feedback(e.Command, "Barcode form: " + e.Value, true);
                }
                else
                {
                    _process_form?.Feedback(e.Command, "Barcode form is not open", false);
                }
            }

            else if (act.Contains("2d-view"))
            {
                if (act.Contains("clear"))
                {
                    ClearWorkspace(); _process_form?.Feedback(e.Command, "Workspace cleared", true);
                    Properties.Settings.Default.fromFormInsertEnable = true;
                    Graphic2GCode.multiImport = true;
                }

                else if (act.Contains("offset"))
                {
                    //Select globalCollectionCounter
                    SetTextSelection(XmlMarker.lastCollection.LineStart, XmlMarker.lastCollection.LineEnd); // select Gcode

                    VisuGCode.MarkSelectedCollection(XmlMarker.lastCollection.LineStart);                   // highlight 2D-view
                    SelectionHandle.SelectedMarkerLine = XmlMarker.lastCollection.LineStart;
                    SelectionHandle.SelectedCollection = XmlMarker.lastCollection.Id;

                    string[] tmp = e.Value.Split(';');
                    if (tmp.Length > 2)
                    {
                        // int o;
                        //   double x, y;
                        if (int.TryParse(tmp[0], out int o))
                        {
                            if (double.TryParse(tmp[1], out double x))
                            {
                                if (double.TryParse(tmp[2], out double y))
                                {
                                    int lineStart = XmlMarker.GetStartLineOfGroup(codeInsert.Y);
                                    if (LineIsInRange(lineStart))
                                        SetSelection(lineStart, XmlMarkerType.Group);
                                    Logger.Trace("Last codeInsert:{0}  {1}", codeInsert.X, lineStart);

                                    double newx = 0, newy = 0;
                                    if (SelectionHandle.IsActive)
                                    {
                                        System.Drawing.RectangleF dim = SelectionHandle.Bounds;
                                        Logger.Trace("SelectionHandle.IsActiv x:{0:0.00}  y:{1:0.00}  posx:{2:0.00}  posy:{3:0.00}  width:{4:0.00}", x, y, dim.Left, dim.Y, dim.Width);
                                        VisuGCode.GetTransaltionOffset(ref newx, ref newy, -x, y, dim.Left, dim.Y, dim.Width, dim.Height, VisuGCode.GetTranslate(o));
                                        Logger.Trace("SelectionHandle.IsActiv newx:{0:0.00} newy:{1:0.00}  ", newx, newy);
                                        SetFctbCodeText(VisuGCode.TransformGCodeOffset(newx, newy, VisuGCode.Translate.None));
                                    }
                                    else
                                        SetFctbCodeText(VisuGCode.TransformGCodeOffset(-x, -y, VisuGCode.GetTranslate(o)));
                                    SelectionHandle.ClearSelected();
                                    TransformEnd();
                                    _process_form?.Feedback(e.Command, "Offset applied", true);
                                }
                            }
                        }
                    }
                }
                else if (act.Contains("rotate"))
                {
                    //  double angle;
                    if (double.TryParse(e.Value, out double angle))
                    {
                        int lineStart = XmlMarker.GetStartLineOfGroup(codeInsert.Y);
                        if (LineIsInRange(lineStart))
                            SetSelection(lineStart, XmlMarkerType.Group);
                        Logger.Trace("Last codeInsert:{0}  {1}", codeInsert.X, lineStart);

                        TransformStart("Rotate");
                        SetFctbCodeText(VisuGCode.TransformGCodeRotate(angle, 1, new XyPoint(0, 0)));
                        SelectionHandle.ClearSelected();
                        TransformEnd();
                        _process_form?.Feedback(e.Command, "Rotate applied", true);
                    }
                }
                else if (act.Contains("scale"))
                {
                    //    double sizenew;
                    if (double.TryParse(e.Value, out double sizenew))
                    {
                        double size = 100;
                        if (act.Contains("xyx"))
                        {
                            size = 100 * sizenew / VisuGCode.xyzSize.dimx;
                        }
                        else if (act.Contains("xyy"))
                        {
                            size = 100 * sizenew / VisuGCode.xyzSize.dimy;
                        }
                        int lineStart = XmlMarker.GetStartLineOfGroup(codeInsert.Y);
                        if (LineIsInRange(lineStart))
                            SetSelection(lineStart, XmlMarkerType.Group);
                        Logger.Trace("Last codeInsert:{0}  {1}", codeInsert.X, lineStart);

                        TransformStart("Scale");
                        SetFctbCodeText(VisuGCode.TransformGCodeScale(size, size, false));
                        SelectionHandle.ClearSelected();
                        TransformEnd();
                        _process_form?.Feedback(e.Command, "Scale applied", true);
                    }
                }

                else if (act.Contains("regenerate") || act.Contains("refresh"))
                {
                    // Re-generate the last form graphic (text / barcode / image / shape) with the
                    // CURRENT pen / import settings - same path the Setup form uses after a setting change.
                    // Temporarily switch to 'replace' so the regenerated graphic overwrites the current
                    // one instead of stacking a duplicate. Place this BEFORE 2D-View Offset/Rotate/Scale,
                    // because re-generating resets the graphic to its origin.
                    bool prevInsert = LoadProperties.MultipleImportFromForm;
                    bool prevMulti = Graphic2GCode.multiImport;
                    LoadProperties.MultipleImportFromForm = false;
                    Graphic2GCode.multiImport = false;
                    ReStartConvertFile(this, e, false, -1);
                    LoadProperties.MultipleImportFromForm = prevInsert;
                    Graphic2GCode.multiImport = prevMulti;
                    _process_form?.Feedback(e.Command, "Graphic reloaded", true);
                }
            }

            else if (act == "checkform")
            {
                if (e.Value == "Probe") { _process_form?.Feedback(e.Command, e.Value, (_probing_form != null)); }
                if (e.Value == "Cam") { _process_form?.Feedback(e.Command, e.Value, (_camera_form != null)); }
            }
        }

    }
}
