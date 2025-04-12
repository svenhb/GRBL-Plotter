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
/* MainFormFCTB
 * FCTB (fast colored text box) related methods
 * fCTBCode_Click
 * fCTBCode_KeyDown
 * fCTBCode_TextChanged
 * fCTBCode_TextChangedDelayed
 * */
/*
 * 2018-01-04 no selection of text to highlight current line 136...
 * 2019-12-07 add move up/down of selected code block
 * 2020-01-01 improve group handling, new class codeSelection
 * 2020-04-06 improove selection of figures and groups
 * 2020-06-15 add 'Header' tag
 * 2020-08-12 check index line 524
 * 2020-12-16 add Tile handling
 * 2021-03-06 disabled line 372
 * 2021-03-26 highlight 'tool-table'
 * 2021-07-14 code clean up / code quality
 * 2021-09-30 abort FindFigureMarkSelection if XmlMarker.GetFigureCount()==0
 * 2021-11-26 fix ThreadException line 310, 649
 * 2021-12-14 line 397 if (lineIsInRange(fCTBCodeClickedLineNow))
 * 2021-12-16 ErrorLines = new ConcurrentBag<int>(); (old List<int>())
 * 2022-04-07 add style for warning and error
 * 2022-09-14 line 898 ClearTextSelection(int line) add LineIsInRange
			  line 660 change paste-function to LoadFromClipboard();
 * 2022-11-03 check InvokeRequired in MarkErrorLine, ClearErrorLines, FctbSetBookmark
			  new ToList in foreach (int myline in ErrorLines.ToList())
 * 2022-11-04 FctbSetBookmark unbook 5 lines before
 * 2023-01-22 line 90 ClearStyle();
 * 2023-01-24 FctbCode_Click add selectionPathOrig = (GraphicsPath)VisuGCode.pathMarkSelection.Clone();
 * 2023-01-29 line 719 GotoNextBookmark(fCTBCodeClickedLineNow-1), FctbSetBookmark add=10, if (fCTBCodeClickedLineNow < 20)
 * 2023-02-18 line 375 check  if (selStartGrp.iLine < 0)
 * 2023-03-11 l:777/790 Requested Clipboard operation did not succeed. 
 * 2023-03-15 l:740 f:CmsFctb_ItemClicked - if (e.ClickedItem.Name == "cmsCodeCopy") -> if no selection copy all to clipboard
 * 2023-04-12 f:FctbCode_Click / FctbCode_KeyDown replace FctbSetBookmark to avoid collapse blocks
 * 2023-04-13 l:592; 675 f:FctbCode_KeyDown add Tile
 * 2023-06-20 l:380 f:InsertCodeToFctb check if xmlLine is in range
 * 2023-07-31 l:375 f:InsertCodeToFctb insert duplicated figure/group right after selected figure/group
 * 2023-09-05 l:109 f:FctbCode_TextChanged allow 3-digit M-word 
 * 2024-06-01 l:712 check range
 * 2024-09-04 l:893 f:ShowMessageForm try/catch Except: Cannot access a disposed object
 * 2024-11-27 l:735 f:SelectNextFigureGroupTile  check if LineIsInRange
*/

using FastColoredTextBoxNS;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace GrblPlotter
{
    public partial class MainForm : Form
    {
        private bool resetView = false;
        private bool manualEdit = false;
        private readonly bool logMain = false;

        private int globalCollectionCounter = 1;

        #region fCTB FastColoredTextBox related
        // highlight code in editor
        // 0   : Black, 105 : DimGray , 128 : Gray, 169 : DarkGray!, 192 : Silver, 211 : LightGray , 220 : Gainsboro, 245 : Ghostwhite, 255 : White
        private readonly Style StyleComment = new TextStyle(Brushes.Gray, null, FontStyle.Italic);
        private readonly Style StyleCommentxml = new TextStyle(Brushes.DimGray, null, FontStyle.Bold);
        private readonly Style StyleGWord = new TextStyle(Brushes.Blue, null, FontStyle.Bold);
        private readonly Style StyleMWord = new TextStyle(Brushes.SaddleBrown, null, FontStyle.Regular);
        private readonly Style StyleFWord = new TextStyle(Brushes.Red, null, FontStyle.Regular);
        private readonly Style StyleSWord = new TextStyle(Brushes.OrangeRed, null, FontStyle.Regular);
        private readonly Style StyleTool = new TextStyle(Brushes.Black, null, FontStyle.Regular);
        private readonly Style StyleLineN = new TextStyle(Brushes.DimGray, null, FontStyle.Regular);
        private readonly Style StyleXAxis = new TextStyle(Brushes.Green, null, FontStyle.Bold);
        private readonly Style StyleYAxis = new TextStyle(Brushes.BlueViolet, null, FontStyle.Bold);
        private readonly Style StyleZAxis = new TextStyle(Brushes.Red, null, FontStyle.Bold);
        private readonly Style StyleAAxis = new TextStyle(Brushes.DarkCyan, null, FontStyle.Bold);
        private readonly Style StyleFail = new TextStyle(Brushes.Black, Brushes.LightCyan, FontStyle.Bold);
        private readonly Style StyleTT = new TextStyle(Brushes.Black, Brushes.LightYellow, FontStyle.Regular);
        private readonly Style Style2nd = new TextStyle(Brushes.Black, null, FontStyle.Bold);

        private readonly Style ErrorStyle = new TextStyle(Brushes.Red, Brushes.Yellow, FontStyle.Underline);
        private ConcurrentBag<int> ErrorLines = new ConcurrentBag<int>();
        private void FctbCode_TextChanged(object sender, FastColoredTextBoxNS.TextChangedEventArgs e)
        {
            try
            {
                e.ChangedRange.ClearStyle();
                //e.ChangedRange.ClearStyle(StyleComment, ErrorStyle);
                e.ChangedRange.SetStyle(ErrorStyle, "(?i)attention|warning|error", System.Text.RegularExpressions.RegexOptions.Compiled);
                e.ChangedRange.SetStyle(StyleTT, "(tool-table)|(PU)|(PD)", System.Text.RegularExpressions.RegexOptions.Compiled);
                e.ChangedRange.SetStyle(Style2nd, "\\(\\^[23].*", System.Text.RegularExpressions.RegexOptions.Compiled);
                e.ChangedRange.SetStyle(StyleCommentxml, "(\\<.*\\>)", System.Text.RegularExpressions.RegexOptions.Compiled);
                e.ChangedRange.SetStyle(StyleComment, "(\\(.*\\))", System.Text.RegularExpressions.RegexOptions.Compiled);
                e.ChangedRange.SetStyle(StyleGWord, "(G\\d{1,2})", System.Text.RegularExpressions.RegexOptions.Compiled);
                e.ChangedRange.SetStyle(StyleMWord, "(M\\d{1,3})", System.Text.RegularExpressions.RegexOptions.Compiled);           // switch to ",3"
                e.ChangedRange.SetStyle(StyleFWord, "(F\\d+)", System.Text.RegularExpressions.RegexOptions.Compiled);
                e.ChangedRange.SetStyle(StyleSWord, "(S\\d+)", System.Text.RegularExpressions.RegexOptions.Compiled);
                e.ChangedRange.SetStyle(StyleLineN, "(N\\d+)", System.Text.RegularExpressions.RegexOptions.Compiled);
                e.ChangedRange.SetStyle(StyleTool, "(T\\d{1,2})", System.Text.RegularExpressions.RegexOptions.Compiled);
                e.ChangedRange.SetStyle(StyleXAxis, "[XIxi]{1}-?\\d+(.\\d+)?", System.Text.RegularExpressions.RegexOptions.Compiled);
                e.ChangedRange.SetStyle(StyleYAxis, "[YJyj]{1}-?\\d+(.\\d+)?", System.Text.RegularExpressions.RegexOptions.Compiled);
                e.ChangedRange.SetStyle(StyleZAxis, "[Zz]{1}-?\\d+(.\\d+)?", System.Text.RegularExpressions.RegexOptions.Compiled);
                e.ChangedRange.SetStyle(StyleAAxis, "[AaBbCcUuVvWw]{1}-?\\d+(.\\d+)?", System.Text.RegularExpressions.RegexOptions.Compiled);

                e.ChangedRange.ClearFoldingMarkers();
                e.ChangedRange.SetFoldingMarkers(XmlMarker.HeaderStart, XmlMarker.HeaderEnd);
                e.ChangedRange.SetFoldingMarkers(XmlMarker.FillStart, XmlMarker.FillEnd);
                e.ChangedRange.SetFoldingMarkers(XmlMarker.ContourStart, XmlMarker.ContourEnd);
                e.ChangedRange.SetFoldingMarkers(XmlMarker.ClearanceStart, XmlMarker.ClearanceEnd);
                e.ChangedRange.SetFoldingMarkers(XmlMarker.RevolutionStart, XmlMarker.RevolutionEnd);
                e.ChangedRange.SetFoldingMarkers(XmlMarker.PassStart, XmlMarker.PassEnd);
                e.ChangedRange.SetFoldingMarkers(XmlMarker.FigureStart, XmlMarker.FigureEnd);
                e.ChangedRange.SetFoldingMarkers(XmlMarker.GroupStart, XmlMarker.GroupEnd);
                e.ChangedRange.SetFoldingMarkers(XmlMarker.TileStart, XmlMarker.TileEnd);
                e.ChangedRange.SetFoldingMarkers(XmlMarker.CollectionStart, XmlMarker.CollectionEnd);
            }
            catch { }

            if (!manualEdit)
            {
                EnableCmsCodeBlocks(VisuGCode.CodeBlocksAvailable());
            }

            if (ErrorLines.Count > 0)
            {
                foreach (int myline in ErrorLines.ToList())
                {
                    MarkErrorLine(myline);
                }
            }
        }
        private void MarkErrorLine(int line)
        {
            if (LineIsInRange(line))
            {
                if (this.fCTBCode.InvokeRequired)
                {
                    this.fCTBCode.BeginInvoke((MethodInvoker)delegate ()
                    {
                        SetTextSelection(line, line);
                        fCTBCode.Selection.ClearStyle(StyleGWord, StyleXAxis, StyleYAxis);
                        fCTBCode.Selection.SetStyle(ErrorStyle);
                    });
                }
                else
                {
                    SetTextSelection(line, line);
                    fCTBCode.Selection.ClearStyle(StyleGWord, StyleXAxis, StyleYAxis);
                    fCTBCode.Selection.SetStyle(ErrorStyle);
                }
            }
            else
            { Logger.Error("MarkErrorLine LineIsNOTInRange: {0}", line); }
        }
        private void ClearErrorLines()
        {
            if (ErrorLines.Count > 0)
            {
                foreach (int myline in ErrorLines.ToList())
                {
                    if (LineIsInRange(myline))
                    {
                        if (this.fCTBCode.InvokeRequired)
                        {
                            this.fCTBCode.BeginInvoke((MethodInvoker)delegate ()
                            {
                                SetTextSelection(myline, myline);
                                fCTBCode.Selection.ClearStyle(ErrorStyle);
                                fCTBCode.Selection.SetStyle(StyleGWord);
                                fCTBCode.Selection.SetStyle(StyleXAxis);
                                fCTBCode.Selection.SetStyle(StyleYAxis);
                            });
                        }
                        else
                        {
                            SetTextSelection(myline, myline);
                            fCTBCode.Selection.ClearStyle(ErrorStyle);
                            fCTBCode.Selection.SetStyle(StyleGWord);
                            fCTBCode.Selection.SetStyle(StyleXAxis);
                            fCTBCode.Selection.SetStyle(StyleYAxis);
                        }
                    }
                }
            }
            ErrorLines = new ConcurrentBag<int>();
        }
        private void FctbCode_TextChangedDelayed(object sender, FastColoredTextBoxNS.TextChangedEventArgs e)
        {
            if (Gcode.LoggerTrace && logMain)
                Logger.Trace("Event  fCTBCode_TextChanged  manualEdit:{0}  resetView:{1}", manualEdit, resetView);
            fCTBCode.Cursor = Cursors.IBeam;
            pictureBox1.Cursor = Cursors.Cross;

            if (resetView && !manualEdit)
            {
                try
                {
                    TransformEnd();
                    if (foldLevel == 1)
                    { fCTBCode.CollapseAllFoldingBlocks(); foldLevel = 1; }
                    else if (foldLevel == 2)
                    { FoldBlocks2(); }

                    FctbSetBookmark();         // set Bookmark and marker in 2D-View
                                               //	FindFigureMarkSelection(markedBlockType, fCTBCodeClickedLineNow, new DistanceByLine(0));//);
                                               //               fCTBCode.DoCaretVisible();
                }
                catch (Exception err)
                { Logger.Error(err, "FctbCode_TextChangedDelayed "); }
            }
            resetView = false;
        }

        private void FctbCode_ToolTipNeeded(object sender, FastColoredTextBoxNS.ToolTipNeededEventArgs e)
        {
            if (!string.IsNullOrEmpty(e.HoveredWord))
            {
                if (CodeMessage.IsMessage(e.HoveredWord))
                {
                    try
                    {
                        var range = new Range(sender as FastColoredTextBox, e.Place, e.Place);
                        string hoveredWord = range.GetFragment("[^\n]").Text;
                        int codeStrt = hoveredWord.IndexOf("-") + 1;
                        int codeEnd = hoveredWord.IndexOf(":");
                        string code = "";
                        if ((codeStrt > 0) && (codeEnd > codeStrt))
                        { code = hoveredWord.Substring(codeStrt, codeEnd - codeStrt).Trim(); }

                        //    string code = hoveredWord.Substring(e.HoveredWord.Length + 3, 4);
                        string text = CodeMessage.GetMessage(code);
                        if (text.Length > 0)
                        {
                            e.ToolTipTitle = CodeMessage.GetMessage(e.HoveredWord);
                            e.ToolTipText = "Code: " + code + "\r\n" + text;
                        }
                        else
                        {
                            e.ToolTipTitle = CodeMessage.GetMessage(e.HoveredWord);
                            e.ToolTipText = hoveredWord.Substring(e.HoveredWord.Length + 1);
                        }
                    }
                    catch (Exception err)
                    { Logger.Error(err, "FctbCode_ToolTipNeeded "); }
                }
            }
        }

        private void FctbCheckUnknownCode()
        {
            string curLine;
            string allowed = "$ABCNGMFIJKLNPRSTUVWXYZOPLabcngmfijklnprstuvwxyzopl ";
            string number = " +-.0123456789";
            string cmt = "(;";
            string message = "";
            int messageCnt = 30;
            if (Gcode.LoggerTrace && logMain) Logger.Trace(" fCTB_CheckUnknownCode");

            fCTBCode.TextChanged -= FctbCode_TextChanged;       // disable textChanged events
            for (int i = 0; i < fCTBCode.LinesCount; i++)
            {
                curLine = fCTBCode.Lines[i].Trim();
                if ((curLine.Length > 0) && (cmt.IndexOf(curLine[0]) >= 0))             // if comment, nothing to do
                {
                    if (curLine[0] == '(')
                    {
                        if (curLine.IndexOf(')') < 0)                               // if last ')' is missing
                        {
                            fCTBCode.Selection = fCTBCode.GetLine(i);
                            fCTBCode.SelectedText = curLine + " <- unknown command)";
                            if (--messageCnt > 0) message += "Line " + i.ToString() + " : " + curLine + "\r\n";
                        }
                    }
                }
                else if ((curLine.Length > 0) && (allowed.IndexOf(curLine[0]) < 0))     // if 1st char is unknown - no gcode
                {
                    fCTBCode.Selection = fCTBCode.GetLine(i);
                    fCTBCode.SelectedText = "(" + curLine + " <- unknown command)";
                    if (--messageCnt > 0) message += "Line " + i.ToString() + " : " + curLine + "\r\n";
                }
                else if ((curLine.Length > 1) && (number.IndexOf(curLine[1]) < 0))  // if 1st known but 2nd not part of number
                {
                    fCTBCode.Selection = fCTBCode.GetLine(i);
                    fCTBCode.SelectedText = "(" + curLine + " <- unknown command)";
                    if (--messageCnt > 0) message += "Line " + i.ToString() + " : " + curLine + "\r\n";
                }
                if (messageCnt <= 0)
                    break;
            }
            fCTBCode.TextChanged += FctbCode_TextChanged;       // enable textChanged events

            if (message.Length > 0)
            {
                if (messageCnt <= 0)
                {
                    message = "Too many errors - stop fixing the code!!!\r\n\r\n" + message;
                    Logger.Debug("  fCTB_CheckUnknownCode -> Too many errors - stop fixing the code!!!");
                }
                MessageBox.Show(Localization.GetString("mainUnknownCode") + message);
            }
        }

        private int SetFctbCodeText(string code, bool insertCode = false)
        {
            fCTBCode.Cursor = Cursors.WaitCursor;
            pictureBox1.Cursor = Cursors.WaitCursor;
            Application.DoEvents();

            CmsPicBoxEnable();
            ClearErrorLines();
            Logger.Trace("---- SetFctbCodeText insertCode:{0}  enabled:{1}", insertCode, Properties.Settings.Default.fromFormInsertEnable);
            if (insertCode && (Properties.Settings.Default.fromFormInsertEnable || Properties.Settings.Default.multipleLoadAllwaysLoad))
            { return InsertCodeToFctb(code, true, 0, 0, 0); }
            else
            { fCTBCode.Text = code; }
            //           disable e.ChangedRange.SetStyle
            return -1;
        }

        private void InsertTextAtLine(int line, string text)
        {
            //    Logger.Trace("InsertTextAtLine {0}  text:{1}", line, text);
            if (LineIsInRange(line))
            {
                Place selStart;
                selStart.iLine = line;
                selStart.iChar = 0;
                Range mySelection = new Range(fCTBCode);
                mySelection.Start = mySelection.End = selStart;
                fCTBCode.Selection = mySelection;
                fCTBCode.InsertText(text, true);    // insert new code		
                fCTBCode.DoCaretVisible();
            }
            else
            { Logger.Error("InsertTextAtLine LineIsNOTInRange: {0}", line); }
        }
        System.Drawing.Point codeInsert = new System.Drawing.Point();
        private int InsertCodeToFctb(string sourceGCode, bool fromFile, int lineSelected, double offsetX, double offsetY)
        {
            /* if graphic import, add new group, 
             * if duplicate group, add new group
             * if duplicate figure -> put into same group (if available)
            */
            importOptions = "";
            SimuStop();
            bool createGroup = false;
            int insertLineCollection = XmlMarker.FindInsertPositionCollectionMostBottom();
            int insertLineGroup = XmlMarker.FindInsertPositionGroupMostTop();
            int insertLineFigure = XmlMarker.FindInsertPositionFigureMostTop(-1);
            int insertLineNr = insertLineGroup;

            bool containsCollection = sourceGCode.Contains(XmlMarker.CollectionStart);
            bool containsGroup = sourceGCode.Contains(XmlMarker.GroupStart);
            bool containsFigure = sourceGCode.Contains(XmlMarker.FigureStart);

            int newID = 0;

            if (fromFile)
            {
                if (insertLineCollection > 0)
                    insertLineNr = insertLineCollection;
                else if (insertLineGroup > 0)
                    insertLineNr = insertLineGroup;
                else if (insertLineNr < 0)
                {
                    insertLineNr = insertLineFigure;  // no group? use figure
                    createGroup = true;
                }
                Logger.Info("InsertCodeToFctb Collection:{0}  Group:{1}  Figure:{2}  insert:{3}", insertLineCollection, insertLineGroup, insertLineFigure, insertLineNr);
            }
            else
            {
                insertLineGroup = XmlMarker.FindInsertPositionGroupNext(lineSelected);
                insertLineFigure = XmlMarker.FindInsertPositionFigureNext(lineSelected);
                if (containsGroup)
                { insertLineNr = insertLineGroup; }  // add as next group
                else if (containsFigure)
                { insertLineNr = insertLineFigure; }  // add as next figure
            }

            if (!fromFile && ((offsetX != 0) || (offsetY != 0)))
            { sourceGCode = ModifyCode.ApplyXYOffsetSimple(sourceGCode, offsetX, offsetY); }

            Logger.Info("....InsertCodeToFctb fromFile:{0}  sourceGroupTag:{1}  sourceFigureTag:{2}  lineSelected:{3}   lineGroup:{4}  lineFigure:{5}  lineInsert:{6}  offX:{7:0.00}  offY:{8:0.00}", fromFile, containsGroup, containsFigure, lineSelected, insertLineGroup, insertLineFigure, insertLineNr, offsetX, offsetY);

            if (LineIsInRange(insertLineNr))
            {
                if (createGroup)
                {    // add startGroup for existing figures
                    Place selStartGrp;
                    int xmlLine = XmlMarker.FindInsertPositionFigureMostBottom(insertLineNr);
                    if (LineIsInRange(xmlLine))
                        selStartGrp.iLine = xmlLine;
                    else
                        selStartGrp.iLine = insertLineNr;

                    selStartGrp.iChar = 0;
                    Range mySelectionGrp = new Range(fCTBCode);
                    mySelectionGrp.Start = mySelectionGrp.End = selStartGrp;
                    fCTBCode.Selection = mySelectionGrp;
                    //    fCTBCode.InsertText("(" + XmlMarker.GroupEnd + ">)\r\n", false);    // insert new code
                    fCTBCode.InsertText("(" + XmlMarker.CollectionEnd + ">)\r\n", false);    // insert new code
                }

                // extract group code from generated gcode
                string tmpCodeString = sourceGCode; // Graphic.GCode.ToString();
                StringBuilder tmpCodeFinish = new StringBuilder();
                string[] tmpCodeLines = tmpCodeString.Split('\n');// new string[] { Environment.NewLine }, StringSplitOptions.None);
                bool useCode = false, useGroup = false, useCollection = false;
                string line;
                int figureCount = 1;

                if (!containsCollection) { tmpCodeFinish.AppendLine(string.Format("({0} Id=\"{1}\">)", XmlMarker.CollectionStart, globalCollectionCounter++)); }

                for (int k = 0; k < tmpCodeLines.Length; k++)       // go through code-lines to insert
                {
                    line = tmpCodeLines[k].Trim();
                    if (line.Contains(XmlMarker.CollectionStart))
                    { useCode = true; useCollection = true; }

                    if (line.Contains(XmlMarker.GroupStart))        // find group-tag and increment id
                    {
                        useCode = true; useGroup = true;
                        int idStart = line.IndexOf("Id=");
                        int idCount = XmlMarker.GetGroupCount();
                        if (idStart > 1)
                        {
                            string tmp = line.Substring(0, idStart);
                            tmp += "Id=\"" + (idCount + 1).ToString() + "\"";
                            int strtIndex = line.IndexOf("\"", idStart + 4) + 1;
                            if (strtIndex < (idStart + 6)) { strtIndex = idStart + 5 + idCount.ToString().Length; }
                            tmp += line.Substring(strtIndex);
                            //        Logger.Info("getGCodeFromText figure  idStart:{0}  digits:{1}  final:{2}  string:'{3}'-'{4}'", idStart, idCount.ToString().Length, strtIndex, line.Substring(0, idStart), line.Substring(strtIndex));
                            line = tmp;
                        }
                        newID = idCount;
                    }
                    if (line.Contains(XmlMarker.FigureStart))       // find figure-tag and increment id
                    {
                        useCode = true;
                        if (!useGroup)
                        {
                            int idStart = line.IndexOf("Id=");
                            int idCount = XmlMarker.GetFigureCount();
                            if (idStart > 1)
                            {
                                string tmp = line.Substring(0, idStart);
                                tmp += "Id=\"" + (idCount + figureCount++).ToString() + "\"";
                                int strtIndex = line.IndexOf("\"", idStart + 4) + 1;
                                if (strtIndex < (idStart + 6)) { strtIndex = idStart + 5 + idCount.ToString().Length; }
                                tmp += line.Substring(strtIndex);
                                //            Logger.Info("getGCodeFromText figure  idStart:{0}  digits:{1}  final:{2}  string:'{3}'-'{4}'", idStart, idCount.ToString().Length, strtIndex, line.Substring(0, idStart), line.Substring(strtIndex));
                                line = tmp;
                            }
                            newID = idCount;
                        }
                    }

                    if (useCode)
                    { tmpCodeFinish.AppendLine(line.Trim()); }
                    if (!useCollection)
                    {
                        if (useGroup)
                        { if (line.Contains(XmlMarker.GroupEnd)) useCode = false; }
                        else
                        { if (line.Contains(XmlMarker.FigureEnd)) useCode = false; }
                    }
                    if (line.Contains(XmlMarker.CollectionEnd)) useCode = false;
                }
                if (!containsCollection) { tmpCodeFinish.AppendLine(string.Format("({0} >)", XmlMarker.CollectionEnd)); }

                if (createGroup)
                { tmpCodeFinish.AppendLine("(" + XmlMarker.CollectionStart + " Id=\"0\" Type=\"Existing code\" >)"); }    // add startGroup for existing figures
                                                                                                                          //    { tmpCodeFinish.AppendLine("(" + XmlMarker.GroupStart + " Id=\"0\" Type=\"Existing code\" >)"); }    // add startGroup for existing figures

                InsertTextAtLine(insertLineNr, tmpCodeFinish.ToString());
                if (fromFile)
                {
                    char[] charsToTrim = { '(', ')', '\r', '\n' };
                    if (tmpCodeLines.Length > 2)
                        InsertTextAtLine(1, "( ADD code from " + tmpCodeLines[2].Trim(charsToTrim) + " )\r\n");
                    else
                        InsertTextAtLine(1, "( ADD code from file )\r\n");

                }
                Logger.Info("◆◆◆◆ Insert code to existing code at line {0}", insertLineNr);
                codeInsert = new System.Drawing.Point(insertLineNr, newID);
                return insertLineNr;
            }
            else
            {
                fCTBCode.Text = sourceGCode;
                Logger.Warn("⚠⚠⚠ Insert code was not possible at line: {0}", insertLineNr);
                codeInsert = new System.Drawing.Point(-1, -1);
                return -1;
            }
        }

        // mark clicked line in editor
        private int fCTBCodeClickedLineNow = 0;
        private int fCTBCodeClickedLineLast = 0;
        private void FctbCode_Click(object sender, EventArgs e)         // click into FCTB with mouse
        {
            //            if (gcode.loggerTrace && logMain)    Logger.Trace("Event  fCTBCode_Click  fCTBCodeClickedLineNow {0}",fCTBCodeClickedLineNow);
            fCTBCodeClickedLineLast = fCTBCodeClickedLineNow;
            fCTBCodeClickedLineNow = fCTBCode.Selection.ToLine;
            if (fCTBCodeClickedLineNow < 0) fCTBCodeClickedLineNow = 0;

            markerType = markedBlockType = XmlMarkerType.None;
            if (manualEdit)
                return;

            EnableBlockCommands(false);       // disable CMS-Menu block-move items 
            fCTBCode.DoCaretVisible();

            if (expandGCode)    //Properties.Settings.Default.FCTBBlockExpandOnSelect)
            { foldLevel = foldLevelSelected; }

            if (Panel.ModifierKeys == Keys.Alt)
            {
                FindFigureMarkSelection(lastMarkerType = markerType = XmlMarkerType.Figure, fCTBCodeClickedLineNow, new DistanceByLine(0));     //, fCTBCodeClickedLineNow); }  // Alt = Figure
                selectionPathOrig = (GraphicsPath)VisuGCode.pathMarkSelection.Clone();
            }
            else if (Panel.ModifierKeys == Keys.Control)
            {
                FindFigureMarkSelection(lastMarkerType = markerType = XmlMarkerType.Group, fCTBCodeClickedLineNow, new DistanceByLine(0));      // Control = Group
                selectionPathOrig = (GraphicsPath)VisuGCode.pathMarkSelection.Clone();
            }
            else if (Panel.ModifierKeys == Keys.Shift)
            {
                FindFigureMarkSelection(lastMarkerType = markerType = XmlMarkerType.Tile, fCTBCodeClickedLineNow, new DistanceByLine(0));       // Shift = Tile
                selectionPathOrig = (GraphicsPath)VisuGCode.pathMarkSelection.Clone();
            }

            else if (XmlMarker.IsFoldingMarkerFigure(fCTBCodeClickedLineNow))
            {
                FindFigureMarkSelection(lastMarkerType = markerType = XmlMarkerType.Figure, fCTBCodeClickedLineNow, new DistanceByLine(0));     //, (foldLevel > 0)); } // 2020-08-21 add , (foldLevel>0)
                selectionPathOrig = (GraphicsPath)VisuGCode.pathMarkSelection.Clone();
            }

            else if (XmlMarker.IsFoldingMarkerGroup(fCTBCodeClickedLineNow))
            {
                FindFigureMarkSelection(lastMarkerType = markerType = XmlMarkerType.Group, fCTBCodeClickedLineNow, new DistanceByLine(0));      //, (foldLevel > 0)); }  // 2020-08-21 add , (foldLevel>0)
                selectionPathOrig = (GraphicsPath)VisuGCode.pathMarkSelection.Clone();
            }

            else if (XmlMarker.IsFoldingMarkerTile(fCTBCodeClickedLineNow))
            {
                FindFigureMarkSelection(lastMarkerType = markerType = XmlMarkerType.Tile, fCTBCodeClickedLineNow, new DistanceByLine(0));       //, (foldLevel > 0)); }	    // 2020-12-16 add tile
                selectionPathOrig = (GraphicsPath)VisuGCode.pathMarkSelection.Clone();
            }
            else if (XmlMarker.IsFoldingMarkerCollection(fCTBCodeClickedLineNow))
            {
                FindFigureMarkSelection(lastMarkerType = markerType = XmlMarkerType.Collection, fCTBCodeClickedLineNow, new DistanceByLine(0));       //, (foldLevel > 0)); }	    // 2020-12-16 add tile
                selectionPathOrig = (GraphicsPath)VisuGCode.pathMarkSelection.Clone();
            }

            //    SendCommand(string.Format("(Click line:{0}  marked:{1})", fCTBCodeClickedLineNow, lastMarkerType));


            if ((markerType != XmlMarkerType.None) && VisuGCode.CodeBlocksAvailable() && !isStreaming)
            {
                SelectionHandle.SelectedMarkerType = markerType;
                SelectionHandle.SelectedMarkerLine = fCTBCodeClickedLineNow;
            }

            if (VisuGCode.CodeBlocksAvailable() && !isStreaming)
            { StatusStripSet(1, Localization.GetString("statusStripeClickKeys2"), Color.LightGreen); }  // Click: mark Figure; Control-Click: mark Group; Shift-Click: mark Tile;Alt-Click: show GCode line

            fCTBCode.UnbookmarkLine(fCTBCodeClickedLineLast);
            fCTBCode.BookmarkLine(fCTBCodeClickedLineNow);

            VisuGCode.SetPosMarkerLine(fCTBCodeClickedLineNow, false);  // !isStreaming);
            pictureBox1.Invalidate(); // avoid too much events												 
        }
        private void FctbCode_KeyDown(object sender, KeyEventArgs e)    // key up down 
        {
            //            if (gcode.loggerTrace && logMain) Logger.Trace("Event  fCTBCode_KeyDown {0} {1} {2}  clickedLine {3}", e.KeyValue, ModifierKeys, markedBlockType.ToString(), fCTBCodeClickedLineNow);
            if (e.Control && e.KeyCode == Keys.D)
            {
                pictureBox1.Focus();
                DuplicateSelectedPath();
                e.SuppressKeyPress = true;
                return;
            }
            markerType = XmlMarkerType.None;

            int key = e.KeyValue;
            #region key up
            if (key == 38)                                  // up
            {
                if (fCTBCode.LinesCount < 1) return;

                if (Panel.ModifierKeys == Keys.Control)
                { MoveSelectedCodeBlockUp(); }
                else if (Panel.ModifierKeys == (Keys.Control | Keys.Shift))
                { MoveSelectedCodeBlockUp(true); }
                else
                { SelectNextFigureGroupTile(-1); }

                StatusStripClear(1); // FctbCode_KeyDown
                if (VisuGCode.CodeBlocksAvailable() && figureIsMarked && !isStreaming)
                {
                    StatusStripSet(1, Localization.GetString("statusStripeUpKeys"), Color.LightGreen);      // Control-Up to move selected Block one up; Control-Shift-Up to move to top
                }
                e.SuppressKeyPress = true;
            }
            #endregion
            #region key down
            else if (key == 40)       // down
            {
                if (fCTBCode.LinesCount < 1) return;

                if (Panel.ModifierKeys == Keys.Control)
                { MoveSelectedCodeBlockDown(); }
                else if (Panel.ModifierKeys == (Keys.Control | Keys.Shift))
                { MoveSelectedCodeBlockDown(true); }
                else
                { SelectNextFigureGroupTile(+1); }

                StatusStripClear(1); // FctbCode_KeyDown
                if (VisuGCode.CodeBlocksAvailable() && figureIsMarked && !isStreaming)
                {
                    StatusStripSet(1, Localization.GetString("statusStripeDownKeys"), Color.LightGreen);        // Control-Down to move selected Block one down; Control-Shift-Down to move to end
                }
                e.SuppressKeyPress = true;
            }
            #endregion

            if (!manualEdit && ((key == 8) || (key == 46)))    // delete
            {
                UnDo.SetCode(fCTBCode.Text, "Delete", this);
                resetView = true;
            }
            else if (((key >= 41) && (key <= 127)) || (key == (char)13))
            { SetEditMode(true); }
        }

        private void SelectNextFigureGroupTile(int direction)
        {
            if (LineIsInRange(fCTBCodeClickedLineNow))
            {
                //    SendCommand(string.Format("(Key up/down {0} line:{1}  marked:{2})", direction, fCTBCodeClickedLineNow, markedBlockType));
                fCTBCodeClickedLineLast = fCTBCodeClickedLineNow;
                //    XmlMarker.BlockData tmp = new XmlMarker.BlockData();

                if (!VisuGCode.CodeBlocksAvailable())
                {
                    fCTBCodeClickedLineNow += direction;
                    if (fCTBCodeClickedLineNow < 1) { fCTBCodeClickedLineNow = 1; }
                    if (fCTBCodeClickedLineNow >= fCTBCode.LinesCount) { fCTBCodeClickedLineNow = fCTBCode.LinesCount - 1; }
                    VisuGCode.SetPosMarkerLineSimple(fCTBCodeClickedLineNow);
                }
                else if (markedBlockType == XmlMarkerType.Figure)
                {
                    //	if ((direction > 0)? XmlMarker.GetFigureNext(ref tmp): XmlMarker.GetFigurePrev(ref tmp))
                    if (XmlMarker.GetFigure(XmlMarker.lastFigure.LineStart, direction))     // find and set new figure
                    {
                        fCTBCodeClickedLineNow = XmlMarker.lastFigure.LineStart;    //tmp.LineStart;
                        FindFigureMarkSelection(lastMarkerType = markerType = XmlMarkerType.Figure, fCTBCodeClickedLineNow, new DistanceByLine(0));
                        selectionPathOrig = (GraphicsPath)VisuGCode.pathMarkSelection.Clone();
                    }
                    //    XmlMarker.GetFigure(XmlMarker.lastFigure.LineStart, direction);    // find figure before
                    //    fCTBCodeClickedLineNow = XmlMarker.lastFigure.LineEnd;
                    //    if (Gcode.LoggerTrace && logMain) Logger.Trace("Figure up found {0}  {1}", XmlMarker.lastFigure.LineStart, XmlMarker.lastFigure.LineEnd);
                    //    FindFigureMarkSelection(lastMarkerType = markerType = XmlMarkerType.Figure, fCTBCodeClickedLineNow, new DistanceByLine(0));
                    //    selectionPathOrig = (GraphicsPath)VisuGCode.pathMarkSelection.Clone();
                }
                else if (markedBlockType == XmlMarkerType.Group)
                {
                    //	if ((direction > 0)? XmlMarker.GetGroupNext(ref tmp): XmlMarker.GetGroupPrev(ref tmp))
                    if (XmlMarker.GetGroup(XmlMarker.lastGroup.LineStart, direction))       // find and set new group
                    {
                        fCTBCodeClickedLineNow = XmlMarker.lastGroup.LineStart; //tmp.LineStart;
                        FindFigureMarkSelection(lastMarkerType = markerType = XmlMarkerType.Group, fCTBCodeClickedLineNow, new DistanceByLine(0));
                        selectionPathOrig = (GraphicsPath)VisuGCode.pathMarkSelection.Clone();
                    }
                    //    XmlMarker.GetGroup(XmlMarker.lastGroup.LineStart, direction);    // find figure before
                    //    fCTBCodeClickedLineNow = XmlMarker.lastGroup.LineEnd;
                    //    if (Gcode.LoggerTrace && logMain) Logger.Trace("Group up found {0}  {1}", XmlMarker.lastGroup.LineStart, XmlMarker.lastGroup.LineEnd);
                    //    FindFigureMarkSelection(lastMarkerType = markerType = XmlMarkerType.Group, fCTBCodeClickedLineNow, new DistanceByLine(0));
                    //    selectionPathOrig = (GraphicsPath)VisuGCode.pathMarkSelection.Clone();
                }
                else if (markedBlockType == XmlMarkerType.Tile)
                {
                    if (XmlMarker.GetTile(XmlMarker.lastTile.LineStart, direction))     // find and set new tile
                    {
                        fCTBCodeClickedLineNow = XmlMarker.lastTile.LineStart;
                        if (Gcode.LoggerTrace && logMain) Logger.Trace("Group up found {0}  {1}", XmlMarker.lastTile.LineStart, XmlMarker.lastTile.LineEnd);
                        FindFigureMarkSelection(lastMarkerType = markerType = XmlMarkerType.Tile, fCTBCodeClickedLineNow, new DistanceByLine(0));
                        selectionPathOrig = (GraphicsPath)VisuGCode.pathMarkSelection.Clone();
                    }
                }
                else
                {
                    fCTBCodeClickedLineNow += direction;
                    if (fCTBCodeClickedLineNow < 1) { fCTBCodeClickedLineNow = 1; }
                    if (fCTBCodeClickedLineNow >= fCTBCode.LinesCount) { fCTBCodeClickedLineNow = fCTBCode.LinesCount - 1; }

                    while ((fCTBCode.GetVisibleState(fCTBCodeClickedLineNow) == VisibleState.Hidden) && (fCTBCodeClickedLineNow > 1) && (fCTBCodeClickedLineNow < fCTBCode.LinesCount))
                        fCTBCodeClickedLineNow += direction;
                    if (Gcode.LoggerTrace && logMain) Logger.Trace("Else up {0} ", markedBlockType.ToString());
                }

                fCTBCode.UnbookmarkLine(fCTBCodeClickedLineLast);
                fCTBCode.BookmarkLine(fCTBCodeClickedLineNow);
                //    fCTBCode.DoCaretVisible();
                if ((markerType != XmlMarkerType.None) && VisuGCode.CodeBlocksAvailable() && !isStreaming)
                {
                    SelectionHandle.SelectedMarkerType = markerType;
                    SelectionHandle.SelectedMarkerLine = fCTBCodeClickedLineNow;
                }
                VisuGCode.SetPosMarkerLine(fCTBCodeClickedLineNow, false);  // !isStreaming);
                if (fCTBCodeClickedLineNow > 0)
                    fCTBCode.GotoNextBookmark(fCTBCodeClickedLineNow - 1);// .DoCaretVisible();
                pictureBox1.Invalidate(); // avoid too much events												  //             toolStrip_tb_StreamLine.Text = fCTBCodeClickedLineNow.ToString();
            }
        }

        private void FctbSetBookmark(bool markAnyway = false)     // after click on gcode line, mark text and graphics
        {
            if (LineIsInRange(fCTBCodeClickedLineNow))  //(fCTBCodeClickedLineNow < fCTBCode.LinesCount) && (fCTBCodeClickedLineNow >= 0))
            {
                if ((fCTBCodeClickedLineNow != fCTBCodeClickedLineLast) || markAnyway)
                {
                    try
                    {
                        if (this.fCTBCode.InvokeRequired)
                        {
                            this.fCTBCode.BeginInvoke((MethodInvoker)delegate ()
                             { fCTBCode.Bookmarks.Clear(); });
                        }
                        else
                        {
                            fCTBCode.Bookmarks.Clear();
                        }

                        if (this.fCTBCode.InvokeRequired)
                        {
                            this.fCTBCode.BeginInvoke((MethodInvoker)delegate ()
                            { this.fCTBCode.BookmarkLine(fCTBCodeClickedLineNow); fCTBCode.GotoNextBookmark(fCTBCodeClickedLineNow - 1); });
                        }
                        else
                        { this.fCTBCode.BookmarkLine(fCTBCodeClickedLineNow); fCTBCode.GotoNextBookmark(fCTBCodeClickedLineNow - 1); }
                    }
                    catch { }   // nothing to unbook - no problem
                }
                fCTBCodeClickedLineLast = fCTBCodeClickedLineNow;
            }
        }
        #endregion

        #region CMS Menu
        // context Menu on fastColoredTextBox
        private void CmsFctb_ItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {
            Logger.Trace("###### CmsFctb_ItemClicked: {0} fCTBCodeClickedLineNow: {1}", e.ClickedItem.Name, fCTBCodeClickedLineNow);
            if (e.ClickedItem.Name == "cmsCodeSelect")
                fCTBCode.SelectAll();
            else if (e.ClickedItem.Name == "cmsCodeCopy")
            {
                if (fCTBCode.SelectedText.Length > 0)
                { fCTBCode.Copy(); }
                else
                {
                    fCTBCode.SelectAll();
                    fCTBCode.Copy();
                }
            }
            else if (e.ClickedItem.Name == "cmsCodePaste")
            {       //fCTBCode.Paste();
                /*    IDataObject iData = Clipboard.GetDataObject();
                    UnDo.SetCode(fCTBCode.Text, "Paste Code", this);
                    fCTBCode.Text = (String)iData.GetData(DataFormats.Text);
                    NewCodeEnd();
                    Logger.Trace("CmsFctb_ItemClicked: fCTBCodeClickedLineNow: {0}", fCTBCodeClickedLineNow);
                    SetLastLoadedFile("Data from Clipboard: Text", "");
                    lbInfo.Text = "GCode from clipboard";*/

                UnDo.SetCode(fCTBCode.Text, cmsPicBoxPasteFromClipboard.Text, this);
                LoadFromClipboard();
                EnableCmsCodeBlocks(VisuGCode.CodeBlocksAvailable());
            }
            else if (e.ClickedItem.Name == "cmsEditMode")
            {
                if (!manualEdit)
                { SetEditMode(true); }
                else
                { NewCodeEnd(); SetEditMode(false); }
                ClearErrorLines();
            }
            else if (e.ClickedItem.Name == "cmsCodePasteSpecial1")  // Pen up
            {
                SetEditMode(true);
                Gcode.Setup(true);  // convertGraphics=true (repeat, inser sub)
                StringBuilder gcodeString = new StringBuilder();
                Gcode.PenUp(gcodeString, "pasted");
                SetTextSelection(fCTBCodeClickedLineNow, fCTBCodeClickedLineNow);
                gcodeString.Append(fCTBCode.SelectedText.Replace("G01", "G00") + " (modified)");
                if (!string.IsNullOrEmpty(gcodeString.ToString()))
                {
                    Clipboard.SetText(gcodeString.ToString());
                    fCTBCode.Paste();
                    FctbSetBookmark();
                }   // 2023-03-11
            }
            else if (e.ClickedItem.Name == "cmsCodePasteSpecial2")  // Pen down
            {
                SetEditMode(true);
                Gcode.Setup(true);  // convertGraphics=true (repeat, inser sub)
                StringBuilder gcodeString = new StringBuilder();
                Gcode.PenDown(gcodeString, "pasted");
                SetTextSelection(fCTBCodeClickedLineNow, fCTBCodeClickedLineNow);
                gcodeString.Append(fCTBCode.SelectedText.Replace("G00", "G01") + " F" + Gcode.GcodeXYFeed + " (modified)");
                if (!string.IsNullOrEmpty(gcodeString.ToString()))
                {
                    Clipboard.SetText(gcodeString.ToString());
                    fCTBCode.Paste();
                    FctbSetBookmark();
                }   // 2023-03-11
            }
            else if (e.ClickedItem.Name == "cmsCodeSendLine")
            {
                int clickedLine = fCTBCode.Selection.ToLine;
                SendCommand(fCTBCode.Lines[clickedLine], false);
            }
            else if (e.ClickedItem.Name == "cmsCommentOut")
                FctbCheckUnknownCode();
            else if (e.ClickedItem.Name == "cmsUpdate2DView")
            { ClearErrorLines(); NewCodeEnd(); SetEditMode(false); }
            else if (e.ClickedItem.Name == "cmsReplaceDialog")
                fCTBCode.ShowReplaceDialog();
            else if (e.ClickedItem.Name == "cmsFindDialog")
                fCTBCode.ShowFindDialog();
            else if (e.ClickedItem.Name == "cmsEditorHotkeys")
            {
                string[] lines = Properties.Resources.fctb_hotkeys.Split('\n');
                string result = "";
                if (lines.Length > 1)
                {
                    result += "<h2>" + lines[0] + "</h2><ul>";
                    for (int i = 1; i < lines.Length; i++)
                    {
                        result += "<li>" + lines[i] + "</li>";
                    }
                    result += "</ul>";
                    ShowMessageForm(result);
                }
            }
        }

        public void ShowMessageForm(string text)
        {
            if (_message_form == null)
            {
                _message_form = new MessageForm();
                _message_form.FormClosed += FormClosed_MessageForm;
            }
            else
            {
                _message_form.Visible = false;
            }
            try
            {
                _message_form.ShowMessage(600, 650, "Information", text, 0);  // show FCTB Info
                _message_form.Show(this);
                _message_form.WindowState = FormWindowState.Normal;
            }
            catch (Exception err) { Logger.Error(err, " ShowMessageForm "); }
        }
        private void FormClosed_MessageForm(object sender, FormClosedEventArgs e)
        { _message_form = null; }

        #endregion

        private void SetEditMode(bool set)
        {
            if (isStreaming)
                return;

            manualEdit = set;
            //      fCTBCode.BackColor = set? Color.FromArgb(255, 255, 255, 100): Color.White;
            if (set)
            {
                fCTBCode.BackColor = Color.FromArgb(255, 255, 255, 100);
                StatusStripSet(1, Localization.GetString("statusStripeEditModeOn"), Color.FromArgb(255, 255, 255, 100));        // Edit mode on - click into 2D view to finish
            }
            else
            {
                fCTBCode.BackColor = Color.White;
                StatusStripClear(1);//, "setEditMode");
            }
        }

        private void EnableCmsCodeBlocks(bool enable)
        {
            cmsCodeBlocksFold.Enabled = enable;
            cmsCodeBlocksMove.Enabled = enable;
            cmsCodeBlocksSort.Enabled = enable;
            cmsCodeBlocksRemoveAll.Enabled = enable;
            cmsCodeBlocksRemoveGroup.Enabled = enable;
            if (!enable)
            {
                try { fCTBCode.ExpandAllFoldingBlocks(); foldLevel = 0; fCTBCode.DoCaretVisible(); }
                catch (Exception err) { Logger.Error(err, "EnableCmsCodeBlocks "); }
            }
        }

        #region find blocks
        private static bool figureIsMarked = false;
        private static XmlMarkerType markedBlockType = XmlMarkerType.None;

        private bool FindFigureMarkSelection(XmlMarkerType marker, int clickedLine, DistanceByLine markerProperties)    //bool collapse = true)   // called by click on figure in 2D view
        {
            if ((manualEdit || !VisuGCode.CodeBlocksAvailable() || isStreaming))
                return false;
            if (logDetailed || logSelection)
                Logger.Trace("FindFigureMarkSelection marker:{0}  line:{1}  ", marker, clickedLine);//, collapse);

            fCTBCode.Selection.ColumnSelectionMode = false;

            bool gcodeIsSeleced = false;
            EnableBlockCommands(gcodeIsSeleced);

            markedBlockType = marker;
            Color highlight = Color.GreenYellow;

            if (marker == XmlMarkerType.Collection)
            {
                if (XmlMarker.GetCollectionCount() > 0)                                                     // is Tile-Tag present
                {
                    if (XmlMarker.GetCollection(clickedLine) && LineIsInRange(XmlMarker.lastCollection.LineStart))  // is Tile-Tag valid
                    {
                        SetTextSelection(XmlMarker.lastCollection.LineStart, XmlMarker.lastCollection.LineEnd); // select Gcode

                        if (SelectionHandle.SelectedCollection != XmlMarker.lastCollection.Id)
                        {
                            VisuGCode.MarkSelectedCollection(XmlMarker.lastCollection.LineStart);                   // highlight 2D-view
                            SelectionHandle.SelectedMarkerLine = clickedLine;
                            SelectionHandle.SelectedCollection = XmlMarker.lastCollection.Id;
                            gcodeIsSeleced = true;
                        }
                        else
                        {
                            VisuGCode.MarkSelectedFigure(-3);
                            SelectionHandle.ClearSelected();
                            ClearTextSelection();
                        }

                        StatusStripSet(2, string.Format("Marked: {0}", fCTBCode.Lines[XmlMarker.lastCollection.LineStart]), highlight);
                        pictureBox1.Invalidate();
                    }
                }
                fCTBCodeClickedLineNow = XmlMarker.lastCollection.LineStart;
            }
            else if (marker == XmlMarkerType.Tile)
            {
                if (XmlMarker.GetTileCount() > 0)                                                       // is Tile-Tag present
                {
                    if (XmlMarker.GetTile(clickedLine) && LineIsInRange(XmlMarker.lastTile.LineStart))  // is Tile-Tag valid
                    {
                        SetTextSelection(XmlMarker.lastTile.LineStart, XmlMarker.lastTile.LineEnd); // select Gcode

                        if (SelectionHandle.SelectedTile != XmlMarker.lastTile.Id)
                        {
                            VisuGCode.MarkSelectedTile(XmlMarker.lastTile.LineStart);                   // highlight 2D-view
                            SelectionHandle.SelectedMarkerLine = clickedLine;
                            SelectionHandle.SelectedTile = XmlMarker.lastTile.Id;
                            gcodeIsSeleced = true;
                        }
                        else
                        {
                            VisuGCode.MarkSelectedFigure(-3);
                            SelectionHandle.ClearSelected();
                            ClearTextSelection();
                        }

                        StatusStripSet(2, string.Format("Marked: {0}", fCTBCode.Lines[XmlMarker.lastTile.LineStart]), highlight);
                        pictureBox1.Invalidate();
                    }
                }
                fCTBCodeClickedLineNow = XmlMarker.lastTile.LineStart;
            }
            else if (marker == XmlMarkerType.Group)
            {
                if (XmlMarker.GetGroupCount() > 0)                                                      // is Group-Tag present
                {
                    if (XmlMarker.GetGroup(clickedLine) && LineIsInRange(XmlMarker.lastGroup.LineStart))// is Group-Tag valid
                    {
                        SetTextSelection(XmlMarker.lastGroup.LineStart, XmlMarker.lastGroup.LineEnd);   // select Gcode

                        if (SelectionHandle.SelectedGroup != XmlMarker.lastGroup.Id)
                        {
                            VisuGCode.MarkSelectedGroup(XmlMarker.lastGroup.LineStart);                 // highlight 2D-view
                            SelectionHandle.SelectedMarkerLine = clickedLine;
                            SelectionHandle.SelectedGroup = XmlMarker.lastGroup.Id;
                            if (_setup_form != null)
                            {
                                _setup_form.LastMarkedColor = XmlMarker.lastGroup.PenColor;
                                _setup_form.LastMarkedWidth = Math.Round(XmlMarker.lastGroup.PenWidth, 3).ToString().Replace(',', '.');
                                Logger.Trace("FCTB GetGroup color:'{0}' width:'{1}'", XmlMarker.lastGroup.PenColor, XmlMarker.lastGroup.PenWidth);
                            }
                            gcodeIsSeleced = true;
                        }
                        else
                        {
                            VisuGCode.MarkSelectedFigure(-3);
                            SelectionHandle.ClearSelected();
                            ClearTextSelection();
                        }

                        StatusStripSet(2, string.Format("Marked: {0}", fCTBCode.Lines[XmlMarker.lastGroup.LineStart]), highlight);
                        pictureBox1.Invalidate();
                    }
                    fCTBCodeClickedLineNow = XmlMarker.lastGroup.LineStart;
                }
                else        // group not present, try figure / switch to figure
                {
                    markedBlockType = XmlMarkerType.Figure;
                }
            }
            else if (marker == XmlMarkerType.Figure)
            {
                if (XmlMarker.GetFigure(clickedLine) && LineIsInRange(XmlMarker.lastFigure.LineStart))  // is Figure-Tag valid
                {
                    SetTextSelection(XmlMarker.lastFigure.LineStart, XmlMarker.lastFigure.LineEnd); // select Gcode

                    if (SelectionHandle.SelectedFigure != XmlMarker.lastFigure.Id)
                    {
                        VisuGCode.MarkSelectedFigure(XmlMarker.lastFigure.FigureNr);                // highlight 2D-view
                        SelectionHandle.SelectedMarkerLine = clickedLine;
                        SelectionHandle.SelectedFigure = XmlMarker.lastFigure.Id;
                        if (_setup_form != null)
                        {
                            _setup_form.LastMarkedColor = XmlMarker.lastFigure.PenColor;
                            _setup_form.LastMarkedWidth = Math.Round(XmlMarker.lastFigure.PenWidth, 3).ToString().Replace(',', '.');
                            Logger.Trace("FCTB GetFigure color:'{0}' width:'{1}'", XmlMarker.lastFigure.PenColor, XmlMarker.lastFigure.PenWidth);
                        }
                        gcodeIsSeleced = true;
                    }
                    else
                    {
                        VisuGCode.MarkSelectedFigure(-3);
                        SelectionHandle.ClearSelected();
                        ClearTextSelection();
                    }
                    StatusStripSet(2, string.Format("Marked: {0}", fCTBCode.Lines[XmlMarker.lastFigure.LineStart]), highlight);
                }
                fCTBCodeClickedLineNow = XmlMarker.lastFigure.LineStart;
            }
            else if (marker == XmlMarkerType.Node)
            {
                if (LineIsInRange(clickedLine))
                {
                    if (SelectionHandle.SelectedMarkerLine != clickedLine)
                    {
                        SetTextSelection(clickedLine, clickedLine); //, (fCTBCodeClickedLineNow == clickedLine));
                        if (markerProperties.lineNumber != 0)
                        { VisuGCode.MarkSelectedNode(markerProperties); }                // highlight 2D-view
                        SelectionHandle.SelectedMarkerLine = clickedLine;
                    }
                    else
                    {
                        VisuGCode.MarkSelectedFigure(-3);
                        SelectionHandle.ClearSelected();
                        ClearTextSelection();
                    }
                    fCTBCodeClickedLineNow = clickedLine;
                }
            }
            //    Logger.Trace("FindFigureMarkSelection marker:{0}  active:{1}", marker, SelectionHandle.IsActive);
            EnableBlockCommands(gcodeIsSeleced, true);                                            // enable CMS menu

            if (!gcodeIsSeleced)
                StatusStripClear(2);

            try { fCTBCode.DoCaretVisible(); }
            catch (Exception err) { Logger.Error(err, "FindFigureMarkSelection "); }
            this.Invalidate();
            return gcodeIsSeleced;
        }

        private bool LineIsInRange(int line)
        { return ((line >= 0) && (line < fCTBCode.LinesCount)); }


        private void ClearTextSelection()
        { fCTBCode.Selection.End = fCTBCode.Selection.Start; }

        private void ClearTextSelection(int line)
        {
            if (LineIsInRange(line))
            {
                Range mySelection = new Range(fCTBCode, 0, line, 0, line);
                fCTBCode.Selection = mySelection;
                fCTBCode.SelectionColor = Color.Green;
            }
        }
        private void SetTextSelection(int start, int end)   //, bool toggle = true)
        {
            if (start < 0) start = 0; if (start >= fCTBCode.LinesCount) start = 0;
            if (end < 0) end = 0;
            if (end >= fCTBCode.LinesCount) end = fCTBCode.LinesCount - 1;

            Place selStart, selEnd;
            selStart.iLine = start;
            selStart.iChar = 0;
            Range mySelection = new Range(fCTBCode)
            { Start = selStart };
            selEnd.iLine = end;
            selEnd.iChar = fCTBCode.Lines[end].Length;
            mySelection.End = selEnd;
            //        Logger.Trace("SetTextSelection  start:{0} start:{1}  end:{2}  end:{3}", selStart, fCTBCode.Selection.Start, selEnd, fCTBCode.Selection.End);
            fCTBCode.Selection = mySelection;
            fCTBCode.SelectionColor = Color.Red;
        }

        private bool deleteMarkedCode = false;
        private void DeletenotMarkToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (deleteMarkedCode)
            {
                deleteMarkedCode = false;
                deletenotMarkToolStripMenuItem.Text = "Mark (not delete)";
            }
            else
            {
                deleteMarkedCode = true;
                deletenotMarkToolStripMenuItem.Text = "Delete (not mark)";
            }
        }
        #endregion

        #region fold blocks
        private static byte foldLevel = 0;
        private static byte foldLevelSelected = 0;


        private void FoldBlocksByLevel(XmlMarkerType _markerType, int _clickedLineNr)
        {
            Range range = fCTBCode.Selection.Clone();
            FctbSetBookmark(true);

            bool changeFoldStatus = toggleBlockExpansionToolStripMenuItem.Checked;//Properties.Settings.Default.FCTBBlockExpandOnSelect;
            int lineCollection = 0;
            int lineGroup = 0;
            int lineFigure = 0;

            if (XmlMarker.GetCollection(_clickedLineNr))
            { lineCollection = XmlMarker.lastCollection.LineStart; }

            if (XmlMarker.GetGroup(_clickedLineNr))
            { lineGroup = XmlMarker.lastGroup.LineStart; }

            if (XmlMarker.GetFigure(_clickedLineNr))
            { lineFigure = XmlMarker.lastFigure.LineStart; }

            Logger.Trace("FoldBlocksByLevel  lc:{0} lg:{1}  lf:{2}  type:{3}", lineCollection, lineGroup, lineFigure, _markerType);
            //	fCTBCode.CollapseAllFoldingBlocks();
            if (_markerType == XmlMarkerType.Node)
            {
               if (changeFoldStatus)
                {
                    FoldBlocks2(lineFigure);    // expand all, then collapse, except lineFigure
                    foldLevelSelected = 2;
                }
                else 
                { fCTBCode.ExpandFoldedBlock(lineFigure); }
            }
            else if (_markerType == XmlMarkerType.Figure)
            {
                if (changeFoldStatus)
                {
                    FoldBlocks2(lineGroup);          // expand all, then collapse
                    foldLevelSelected = 2;
                }
            }
            else if (_markerType == XmlMarkerType.Group)
            {
                if (changeFoldStatus)
                {
                    FoldBlocks2(lineCollection);          // expand all, then collapse
                    foldLevelSelected = 1;
                }
            }
            else
            { if (changeFoldStatus) fCTBCode.CollapseAllFoldingBlocks(); }

            fCTBCode.Selection = range; // SetTextSelection
            fCTBCode.SelectionColor = Color.Red;
            fCTBCode.Invalidate();
            if (logDetailed)
                Logger.Trace("◯◯◯ FoldBlocksByLevel type:{0}  line:{1}  figure:{2}  group:{3} status:{4}  range:{5}", _markerType, _clickedLineNr, lineFigure, lineGroup, changeFoldStatus, range);
        }

        private void FoldBlocksByLevel(int level)
        {
            if (level == 1)
            { fCTBCode.CollapseAllFoldingBlocks(); foldLevel = 1; }
            else if (level == 2)
            { FoldBlocks2(); }
            else if (level == 3)
            { FoldBlocks3(); }
            if (logDetailed)
                Logger.Trace("◯◯◯ FoldBlocksByLevel level: {0}   {1}", level, foldLevel);
        }


        /* Fold 1 */
        private void FoldBlocks1stToolStripMenuItem1_Click(object sender, EventArgs e)  // fold all blocks
        { FoldBlocks1(); foldLevelSelected = 1; }
        private void FoldBlocks1(int exceptLine = 0)
        {
            fCTBCode.CollapseAllFoldingBlocks();
            if (exceptLine > 0)
            {
                try { fCTBCode.ExpandFoldedBlock(exceptLine); }
                catch (Exception err) { Logger.Error(err, "FoldBlocks1 "); }    // no need to track				
            }
            foldLevel = 1;
        }


        /* Fold 2 */
        private void FoldBlocks2ndToolStripMenuItem1_Click(object sender, EventArgs e)  // fold 2nd level blocks
        { FoldBlocks2(); foldLevelSelected = 2; }
        private void FoldBlocks2(int exceptLine = 0)
        {
            string tmp;
            bool exceptFigureActive = false;
            fCTBCode.ExpandAllFoldingBlocks();
            for (int i = 0; i < fCTBCode.LinesCount; i++)
            {
                tmp = fCTBCode.GetLine(i).Text;
                if (tmp.Contains(XmlMarker.FigureStart))
                { exceptFigureActive = (i == exceptLine); }

                if (tmp.Contains(XmlMarker.FigureStart) || tmp.Contains(XmlMarker.ContourStart) || tmp.Contains(XmlMarker.FillStart) || tmp.Contains(XmlMarker.PassStart))
                {
                    if (!exceptFigureActive)    //(i != exceptLine)
                    {
                        try { fCTBCode.CollapseFoldingBlock(i); }
                        catch (Exception err) { Logger.Error(err, "FoldBlocks2 "); }    // no need to track
                    }
                }
            }
            foldLevel = 2;
        }


        /* Fold 3 */
        private void FoldBlocks3rdToolStripMenuItem1_Click(object sender, EventArgs e)
        { FoldBlocks3(); foldLevelSelected = 3; }
        private void FoldBlocks3()
        {
            string tmp;
            fCTBCode.ExpandAllFoldingBlocks();
            for (int i = 0; i < fCTBCode.LinesCount; i++)
            {
                tmp = fCTBCode.GetLine(i).Text;
                if (tmp.Contains(XmlMarker.ContourStart) || tmp.Contains(XmlMarker.FillStart) || tmp.Contains(XmlMarker.PassStart))
                { fCTBCode.CollapseFoldingBlock(i); }
            }
            foldLevel = 3;
        }

        private void ExpandCodeBlocksToolStripMenuItem_Click(object sender, EventArgs e)
        { fCTBCode.ExpandAllFoldingBlocks(); foldLevel = 0; fCTBCode.DoCaretVisible(); }

        private void EnableBlockCommands(bool tmp, bool keepSelection = false)
        {
            if (!tmp && !keepSelection && LineIsInRange(fCTBCodeClickedLineNow))
                ClearTextSelection(fCTBCodeClickedLineNow);
            figureIsMarked = tmp;
            cmsFCTBMoveSelectedCodeBlockMostUp.Enabled = tmp;
            cmsFCTBMoveSelectedCodeBlockUp.Enabled = tmp;
            cmsFCTBMoveSelectedCodeBlockDown.Enabled = tmp;
            cmsFCTBMoveSelectedCodeBlockMostDown.Enabled = tmp;

            cmsPicBoxDuplicatePath.Enabled = tmp;
            cmsPicBoxDeletePath.Enabled = tmp;
            cmsPicBoxCropSelectedPath.Enabled = tmp;
            cmsPicBoxShowProperties.Enabled = tmp;
            cmsPicBoxReverseSelectedPath.Enabled = tmp && Graphic.SizeOk(); // Data in Graphic-class is needed
            cmsPicBoxRotateSelectedPath.Enabled = tmp && Graphic.SizeOk(); // Data in Graphic-class is needed
        }
        #endregion

        #region move blocks
        // Move code block upwards
        private void MoveSelectedCodeBlockMostUpToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (LineIsInRange(fCTBCodeClickedLineNow))
                fCTBCode.UnbookmarkLine(fCTBCodeClickedLineNow);
            if (logDetailed) Logger.Trace("moveSelectedCodeBlockMostUpToolStripMenuItem_Click");
            MoveSelectedCodeBlockUp(true);
            XmlMarker.ListIds();
        }
        private void MoveSelectedCodeBlockUpToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (LineIsInRange(fCTBCodeClickedLineNow))
                fCTBCode.UnbookmarkLine(fCTBCodeClickedLineNow);
            if (logDetailed) Logger.Trace("moveSelectedCodeBlockUpToolStripMenuItem_Click");
            MoveSelectedCodeBlockUp();
            XmlMarker.ListIds();
        }
        private void MoveSelectedCodeBlockUp(bool mostTop = false)
        {
            if (!figureIsMarked)
                return;
            XmlMarker.BlockData block;
            int insert;
            if (markedBlockType == XmlMarkerType.Figure)
            {
                block = XmlMarker.lastFigure;       // save current selection range
                if (mostTop)
                    insert = XmlMarker.FindInsertPositionFigureMostTop(block.LineStart);
                else
                    insert = XmlMarker.FindInsertPositionFigureTop(block.LineStart);
                if (MoveBlockUp(block.LineStart, block.LineEnd, insert))
                {
                    NewCodeEnd();
                    resetView = true;
                    fCTBCodeClickedLineNow = insert;
                }
            }
            else if (markedBlockType == XmlMarkerType.Group)
            {
                block = XmlMarker.lastGroup;       // save current selection range
                if (mostTop)
                    insert = XmlMarker.FindInsertPositionGroupMostTop();// block.LineStart);
                else
                    insert = XmlMarker.FindInsertPositionGroupTop(block.LineStart);
                if (MoveBlockUp(block.LineStart, block.LineEnd, insert))
                {
                    NewCodeEnd();
                    resetView = true;
                    fCTBCodeClickedLineNow = insert;
                }
            }
        }
        private bool MoveBlockUp(int blockStart, int blockEnd, int insert)
        {
            if (insert >= 0)
            {
                if (Gcode.LoggerTrace && logMain) Logger.Trace(" moveBlockUp blockStart:{0} Text:{1},  blockEnd:{2} insert:{3}  Text:{4}", blockStart, fCTBCode.Lines[blockStart].Substring(0, 10), blockEnd, insert, fCTBCode.Lines[insert].Substring(0, 10));
                Bookmarks tmp = new Bookmarks(fCTBCode);
                tmp.Clear();
                tmp.Dispose();
                fCTBCode.Cut();
                List<int> remove = new List<int>
                {
                    blockStart
                };
                fCTBCode.RemoveLines(remove);       // remove empty line after cut

                Place selStart;
                selStart.iLine = insert;
                selStart.iChar = 0;
                Range mySelection = new Range(fCTBCode);// fCTBCode.Range;
                mySelection.Start = mySelection.End = selStart;
                fCTBCode.Selection = mySelection;
                fCTBCode.InsertText("\r\n");
                fCTBCode.Selection = mySelection;
                fCTBCode.Paste();
                return true;
            }
            return false;
        }


        private void MoveSelectedCodeBlockMostDownToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (LineIsInRange(fCTBCodeClickedLineNow))
                fCTBCode.UnbookmarkLine(fCTBCodeClickedLineNow);
            if (logDetailed) Logger.Trace("moveSelectedCodeBlockMostDownToolStripMenuItem_Click");
            MoveSelectedCodeBlockDown(true);
            XmlMarker.ListIds();
        }
        private void MoveSelectedCodeBlockDownToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (LineIsInRange(fCTBCodeClickedLineNow))
                fCTBCode.UnbookmarkLine(fCTBCodeClickedLineNow);
            if (logDetailed) Logger.Trace("moveSelectedCodeBlockDownToolStripMenuItem_Click");
            MoveSelectedCodeBlockDown();
            XmlMarker.ListIds();
        }

        private void MoveSelectedCodeBlockDown(bool mostBottom = false)
        {
            if (!figureIsMarked)
                return;
            XmlMarker.BlockData block;
            int insert;
            if (markedBlockType == XmlMarkerType.Figure)
            {
                block = XmlMarker.lastFigure;       // save current selection range
                if (mostBottom)
                    insert = XmlMarker.FindInsertPositionFigureMostBottom(block.LineStart);
                else
                    insert = XmlMarker.FindInsertPositionFigureBottom(block.LineStart);
                if (MoveBlockDown(block.LineStart, block.LineEnd, insert))
                {
                    NewCodeEnd();
                    resetView = true;
                    fCTBCodeClickedLineNow = insert - 1;
                }
            }
            else if (markedBlockType == XmlMarkerType.Group)
            {
                block = XmlMarker.lastGroup;       // save current selection range
                if (mostBottom)
                    insert = XmlMarker.FindInsertPositionGroupMostBottom();// block.LineStart);
                else
                    insert = XmlMarker.FindInsertPositionGroupBottom(block.LineStart);
                if (MoveBlockDown(block.LineStart, block.LineEnd, insert))
                {
                    NewCodeEnd();
                    resetView = true;
                    fCTBCodeClickedLineNow = insert - 1;
                }
            }
        }
        private bool MoveBlockDown(int blockStart, int blockEnd, int insert)
        {
            if (insert >= blockEnd)
            {
                if (Gcode.LoggerTrace && logMain) Logger.Trace(" moveBlockDown blockStart:{0} Text:{1},  blockEnd:{2} insert:{3}  Text:{4}", blockStart, fCTBCode.Lines[blockStart].Substring(0, 10), blockEnd, insert, fCTBCode.Lines[insert].Substring(0, 10));
                Bookmarks tmp = new Bookmarks(fCTBCode);
                tmp.Clear();
                tmp.Dispose();
                fCTBCode.Copy();                        // copy selection
                Range oldSelection = fCTBCode.Selection.Clone();

                Place selStart;
                selStart.iLine = insert;
                selStart.iChar = 0;
                Range mySelection = new Range(fCTBCode);
                mySelection.Start = mySelection.End = selStart;
                fCTBCode.Selection = mySelection;
                fCTBCode.InsertText("\r\n");
                fCTBCode.Selection = mySelection;
                fCTBCode.Paste();                       // insert code at new position
                fCTBCode.Selection = oldSelection;
                fCTBCode.Cut();                         // remove code from old position
                List<int> remove = new List<int>
                {
                    blockStart
                };
                fCTBCode.RemoveLines(remove);           // remove empty line after cut
                return true;
            }
            return false;
        }
        #endregion

        #region remove tags
        private void CmsCodeBlocksRemoveGroup_Click(object sender, EventArgs e)
        { RemoveXMLTags(true); }
        private void CmsCodeBlocksRemoveAll_Click(object sender, EventArgs e)
        { RemoveXMLTags(); }
        private void RemoveXMLTags(bool groupsOnly = false)
        {
            UnDo.SetCode(fCTBCode.Text, cmsCodeBlocksRemoveAll.Text, this);
            ClearTextSelection(0);
            manualEdit = true;
            if (Gcode.LoggerTrace && logMain) Logger.Trace(" removeXMLTags groupsOnly:{0} ", groupsOnly);
            string find1 = groupsOnly ? "(<Group" : "(<";
            string find2 = groupsOnly ? "(</Group" : "(</";
            StringBuilder tmp = new StringBuilder();
            for (int i = 0; i < fCTBCode.LinesCount; i++)
            {
                if (!(fCTBCode.Lines[i].Contains(find1) || fCTBCode.Lines[i].Contains(find2)))
                    tmp.AppendLine(fCTBCode.Lines[i]);
                else if (fCTBCode.Lines[i].Contains("(<Tang"))
                    tmp.AppendLine(fCTBCode.Lines[i]);
            }
            fCTBCode.Text = tmp.ToString();
            TransformEnd();
        }
        #endregion

        #region sort blocks
        // GroupOptions { none = 0, ByColor = 1, ByWidth = 2, ByLayer = 3, ByType = 4, ByTile = 5};
        // 
        private void CmsCodeBlocksSortById_Click(object sender, EventArgs e)
        { CodeBlocksSort(XmlMarker.SortOption.Id); }
        private void CmsCodeBlocksSortByColor_Click(object sender, EventArgs e)
        { CodeBlocksSort(XmlMarker.SortOption.Color); }
        private void CmsCodeBlocksSortByWidth_Click(object sender, EventArgs e)
        { CodeBlocksSort(XmlMarker.SortOption.Width); }
        private void CmsCodeBlocksSortByLayer_Click(object sender, EventArgs e)
        { CodeBlocksSort(XmlMarker.SortOption.Layer); }
        private void CmsCodeBlocksSortByType_Click(object sender, EventArgs e)
        { CodeBlocksSort(XmlMarker.SortOption.Type); }
        private void CmsCodeBlocksSortByGeometry_Click(object sender, EventArgs e)
        { CodeBlocksSort(XmlMarker.SortOption.Geometry); }
        private void CmsCodeBlocksSortByToolNr_Click(object sender, EventArgs e)
        { CodeBlocksSort(XmlMarker.SortOption.ToolNR); }
        private void CmsCodeBlocksSortByToolName_Click(object sender, EventArgs e)
        { CodeBlocksSort(XmlMarker.SortOption.ToolName); }
        private void CmsCodeBlocksSortByCodeSize_Click(object sender, EventArgs e)
        { CodeBlocksSort(XmlMarker.SortOption.CodeSize); }
        private void CmsCodeBlocksSortByCodeArea_Click(object sender, EventArgs e)
        { CodeBlocksSort(XmlMarker.SortOption.CodeArea); }

        private void CmsCodeBlocksSortByDistance_Click(object sender, EventArgs e)
        { CodeBlocksSort(XmlMarker.SortOption.Distance); }

        private void CodeBlocksSort(XmlMarker.SortOption tmp)
        {
            Cursor.Current = Cursors.WaitCursor;
            XmlMarker.SortBy(tmp, cmsCodeBlocksSortReverse.Checked);
            GetSortedCode(tmp);
            Cursor.Current = Cursors.Default;
            XmlMarker.ListIds();
        }

        private void GetSortedCode(XmlMarker.SortOption tmp)
        {
            UnDo.SetCode(fCTBCode.Text, cmsCodeBlocksRemoveAll.Text, this);
            ClearTextSelection(0);
            manualEdit = true;
            if (Gcode.LoggerTrace && logMain) Logger.Trace(" sortXMLTags by {0}", tmp.ToString());

            fCTBCode.Text = XmlMarker.GetSortedCode(fCTBCode.Lines.ToArray<string>());
            TransformEnd();
            if (foldLevel == 1) { fCTBCode.CollapseAllFoldingBlocks(); foldLevel = 1; }
            else if (foldLevel == 2) FoldBlocks2();
        }

        #endregion
    }
    #region code messages
    internal static class CodeMessage
    {
        internal const string Attention = "Attention";
        internal const string Warning = "Warning";
        internal const string Error = "Error";

        internal static Dictionary<string, string> MessageCode = new Dictionary<string, string>();

        internal static bool IsMessage(string tmp)
        { return ((tmp == Attention) || (tmp == Warning) || (tmp == Error)); }

        internal static string GetMessage(string key)
        {
            if (MessageCode.ContainsKey(key))
            { return MessageCode[key]; }

            return "";
        }
        internal static void Init()
        {   // 1st digit 1-Import from file / form; 2nd digit source: 1-SVG, 2-DXF, 3-HPGL, 4-Drill, 5-CSV...
            // 1st digit 2-collect path data
            // 1st digit 3-path2gcode
            // 1st digit 4-create gcode
            MessageCode.Add(Attention, Localization.GetString("codeMessage_attention"));
            MessageCode.Add(Warning, Localization.GetString("codeMessage_warning"));
            MessageCode.Add(Error, Localization.GetString("codeMessage_error"));

            string tmpMessage = Localization.GetString("codeMessage_1_unknowncommand");
            MessageCode.Add("1101", tmpMessage);
            MessageCode.Add("1201", tmpMessage);
            MessageCode.Add("1301", tmpMessage);
            MessageCode.Add("1401", tmpMessage);
            MessageCode.Add("1501", tmpMessage);

            MessageCode.Add("1102", Localization.GetString("codeMessage_1102_dontplot"));
            MessageCode.Add("1202", Localization.GetString("codeMessage_1202_dontplot"));

            MessageCode.Add("1103", Localization.GetString("codeMessage_1103_elementsupport"));     // not supported
            MessageCode.Add("1104", Localization.GetString("codeMessage_1104_elementuk"));          // unknown
            MessageCode.Add("1105", Localization.GetString("codeMessage_1105_textpath"));           // 
            MessageCode.Add("1106", Localization.GetString("codeMessage_1106_attributesupport"));   // not supported


            MessageCode.Add("2001", Localization.GetString("codeMessage_2_zdoubletang"));       // GraphicCollectData.cs 746
            MessageCode.Add("2002", Localization.GetString("codeMessage_2_highdataamount"));    // GraphicCollectData.cs 787

            MessageCode.Add("3001", Localization.GetString("codeMessage_3_zdoubletang"));       // Graphic2GCodeRelated.cs 510
            MessageCode.Add("3002", Localization.GetString("codeMessage_3_zdoubleaux"));        // Graphic2GCodeRelated.cs 513
            MessageCode.Add("3003", Localization.GetString("codeMessage_3_spindlestaysoff"));   // Graphic2GCodeRelated.cs 548
        }
    }
    #endregion
}
