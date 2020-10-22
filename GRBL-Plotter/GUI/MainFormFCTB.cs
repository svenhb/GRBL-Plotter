/*  GRBL-Plotter. Another GCode sender for GRBL.
    This file is part of the GRBL-Plotter application.
   
    Copyright (C) 2015-2020 Sven Hasemann contact: svenhb@web.de

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
*/

using System;
using System.Drawing;
using System.Windows.Forms;
using FastColoredTextBoxNS;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GRBL_Plotter
{
    public partial class MainForm : Form
    {
        private bool resetView = false;
        private bool manualEdit= false;
		private bool logMain = false;

        #region fCTB FastColoredTextBox related
        // highlight code in editor
        private Style StyleComment = new TextStyle(Brushes.Gray, null, FontStyle.Italic);
        private Style StyleCommentxml = new TextStyle(Brushes.DarkGray, null, FontStyle.Regular);
        private Style StyleGWord = new TextStyle(Brushes.Blue, null, FontStyle.Bold);
        private Style StyleMWord = new TextStyle(Brushes.SaddleBrown, null, FontStyle.Regular);
        private Style StyleFWord = new TextStyle(Brushes.Red, null, FontStyle.Regular);
        private Style StyleSWord = new TextStyle(Brushes.OrangeRed, null, FontStyle.Regular);
        private Style StyleTool = new TextStyle(Brushes.Black, null, FontStyle.Regular);
        private Style StyleLineN = new TextStyle(Brushes.DarkGray, null, FontStyle.Regular);
        private Style StyleXAxis = new TextStyle(Brushes.Green, null, FontStyle.Bold);
        private Style StyleYAxis = new TextStyle(Brushes.BlueViolet, null, FontStyle.Bold);
        private Style StyleZAxis = new TextStyle(Brushes.Red, null, FontStyle.Bold);
        private Style StyleAAxis = new TextStyle(Brushes.DarkCyan, null, FontStyle.Bold);
        private Style StyleFail  = new TextStyle(Brushes.Black, Brushes.LightCyan, FontStyle.Bold);

        private void fCTBCode_TextChanged(object sender, FastColoredTextBoxNS.TextChangedEventArgs e)
        {
            if (gcode.loggerTrace) Logger.Trace("Event  fCTBCode_TextChanged  manualEdit:{0}",manualEdit);
            e.ChangedRange.ClearStyle(StyleComment);
            e.ChangedRange.SetStyle(StyleComment, "(\\(.*\\))", System.Text.RegularExpressions.RegexOptions.Compiled);
            e.ChangedRange.SetStyle(StyleCommentxml, "(\\(<.*\\))", System.Text.RegularExpressions.RegexOptions.Compiled);
            e.ChangedRange.SetStyle(StyleGWord, "(G\\d{1,2})", System.Text.RegularExpressions.RegexOptions.Compiled);
            e.ChangedRange.SetStyle(StyleMWord, "(M\\d{1,2})", System.Text.RegularExpressions.RegexOptions.Compiled);
            e.ChangedRange.SetStyle(StyleFWord, "(F\\d+)", System.Text.RegularExpressions.RegexOptions.Compiled);
            e.ChangedRange.SetStyle(StyleSWord, "(S\\d+)", System.Text.RegularExpressions.RegexOptions.Compiled);
            e.ChangedRange.SetStyle(StyleLineN, "(N\\d+)", System.Text.RegularExpressions.RegexOptions.Compiled);
            e.ChangedRange.SetStyle(StyleTool, "(T\\d{1,2})", System.Text.RegularExpressions.RegexOptions.Compiled);
            e.ChangedRange.SetStyle(StyleXAxis, "[XIxi]{1}-?\\d+(.\\d+)?", System.Text.RegularExpressions.RegexOptions.Compiled);
            e.ChangedRange.SetStyle(StyleYAxis, "[YJyj]{1}-?\\d+(.\\d+)?", System.Text.RegularExpressions.RegexOptions.Compiled);
            e.ChangedRange.SetStyle(StyleZAxis, "[Zz]{1}-?\\d+(.\\d+)?", System.Text.RegularExpressions.RegexOptions.Compiled);
            e.ChangedRange.SetStyle(StyleAAxis, "[AaBbCcUuVvWw]{1}-?\\d+(.\\d+)?", System.Text.RegularExpressions.RegexOptions.Compiled);

            e.ChangedRange.ClearFoldingMarkers();
            e.ChangedRange.SetFoldingMarkers(xmlMarker.headerStart, xmlMarker.headerEnd);
            e.ChangedRange.SetFoldingMarkers(xmlMarker.fillStart, xmlMarker.fillEnd);
            e.ChangedRange.SetFoldingMarkers(xmlMarker.contourStart, xmlMarker.contourEnd);
            e.ChangedRange.SetFoldingMarkers(xmlMarker.clearanceStart, xmlMarker.clearanceEnd);
            e.ChangedRange.SetFoldingMarkers(xmlMarker.revolutionStart, xmlMarker.revolutionEnd);
            e.ChangedRange.SetFoldingMarkers(xmlMarker.passStart, xmlMarker.passEnd);
            e.ChangedRange.SetFoldingMarkers(xmlMarker.figureStart, xmlMarker.figureEnd);
            e.ChangedRange.SetFoldingMarkers(xmlMarker.groupStart, xmlMarker.groupEnd);

            if (!manualEdit)
            {   if (Properties.Settings.Default.importCodeFold)
                { foldBlocks2(); fCTBCode.CollapseAllFoldingBlocks(); foldLevel = 1; }

                enableCmsCodeBlocks(VisuGCode.codeBlocksAvailable());
            }
        }
        private void fCTBCode_TextChangedDelayed(object sender, FastColoredTextBoxNS.TextChangedEventArgs e)
        {
            if (gcode.loggerTrace && logMain) Logger.Trace("Event  fCTBCode_TextChanged  manualEdit:{0}  resetView:{1}", manualEdit, resetView);
            if (resetView && !manualEdit)
            {   transformEnd();
                if (foldLevel == 1)
                { fCTBCode.CollapseAllFoldingBlocks(); foldLevel = 1; }
                else if (foldLevel == 2)
                { foldBlocks2(); }

                fCTBCodeMarkLine();         // set Bookmark and marker in 2D-View
                findFigureMarkSelection(markedBlockType, fCTBCodeClickedLineNow);
                fCTBCode.DoCaretVisible();
            }
            resetView = false;
        }

        private void fCTB_CheckUnknownCode()
        {   string curLine;
            string allowed = "ABCNGMFIJKLNPRSTUVWXYZOPLabcngmfijklnprstuvwxyzopl ";
            string number = " +-.0123456789";
            string cmt = "(;";
            string message = "";
            int messageCnt = 30;
            if (gcode.loggerTrace && logMain) Logger.Trace(" fCTB_CheckUnknownCode");
            
            fCTBCode.TextChanged -= fCTBCode_TextChanged;       // disable textChanged events
            for (int i = 0; i < fCTBCode.LinesCount; i++)
            {
                curLine = fCTBCode.Lines[i].Trim();
                if ((curLine.Length > 0) && (cmt.IndexOf(curLine[0]) >= 0))             // if comment, nothing to do
                {   if (curLine[0] == '(')
                    {   if (curLine.IndexOf(')') < 0)                               // if last ')' is missing
                        {   fCTBCode.Selection = fCTBCode.GetLine(i);
                            fCTBCode.SelectedText = curLine + " <- unknown command)";
                            if (--messageCnt > 0)   message += "Line " + i.ToString() + " : " + curLine + "\r\n";   
                        }
                    }
                }
                else if ((curLine.Length > 0) && (allowed.IndexOf(curLine[0]) < 0))     // if 1st char is unknown - no gcode
                {   fCTBCode.Selection = fCTBCode.GetLine(i);
                    fCTBCode.SelectedText = "(" + curLine + " <- unknown command)";
                    if (--messageCnt > 0)   message += "Line " + i.ToString() + " : " + curLine + "\r\n";
                }
                else if ((curLine.Length > 1) && (number.IndexOf(curLine[1]) < 0))  // if 1st known but 2nd not part of number
                {   fCTBCode.Selection = fCTBCode.GetLine(i);
                    fCTBCode.SelectedText = "(" + curLine + " <- unknown command)";
                    if (--messageCnt > 0)   message += "Line " + i.ToString() + " : " + curLine + "\r\n";
                }
                if  (messageCnt <= 0)
                    break;
            }
            fCTBCode.TextChanged += fCTBCode_TextChanged;       // enable textChanged events
            
            if (message.Length > 0)
            {   if (messageCnt <= 0)
                {
                    message = "Too many errors - stop fixing the code!!!\r\n\r\n" + message;
                    Logger.Debug("  fCTB_CheckUnknownCode -> Too many errors - stop fixing the code!!!");
                }
                MessageBox.Show(Localization.getString("mainUnknownCode") + message);
            }
        }

        private void setfCTBCodeText(string code)
        {   fCTBCode.Text = code;   }

        // mark clicked line in editor
        private int fCTBCodeClickedLineNow = 0;
        private int fCTBCodeClickedLineLast = 0;
        private void fCTBCode_Click(object sender, EventArgs e)         // click into FCTB with mouse
        {
//            if (gcode.loggerTrace && logMain) Logger.Trace("Event  fCTBCode_Click");
            fCTBCodeClickedLineNow = fCTBCode.Selection.ToLine;
//            statusStripSet2(string.Format("Clicked: {0}", fCTBCodeClickedLineNow),SystemColors.Control);
            markedBlockType = xmlMarkerType.none;
            if (manualEdit)
                return;

            enableBlockCommands(false);       // disable CMS-Menu block-move items 
            fCTBCode.DoCaretVisible();

            if (Panel.ModifierKeys == Keys.Alt)
            { findFigureMarkSelection(xmlMarkerType.Group, fCTBCodeClickedLineNow); }
            else if (Panel.ModifierKeys == Keys.Control)
            { findFigureMarkSelection(xmlMarkerType.Figure, fCTBCodeClickedLineNow); }

            else if (xmlMarker.isFoldingMarkerFigure(fCTBCodeClickedLineNow))
            { findFigureMarkSelection(xmlMarkerType.Figure, fCTBCodeClickedLineNow, (foldLevel>0)); }	// 2020-08-21 add , (foldLevel>0)
            else if (xmlMarker.isFoldingMarkerGroup(fCTBCodeClickedLineNow))
            { findFigureMarkSelection(xmlMarkerType.Group, fCTBCodeClickedLineNow, (foldLevel>0)); }	// 2020-08-21 add , (foldLevel>0)

            if (VisuGCode.codeBlocksAvailable())
            {
                statusStripSet(1, Localization.getString("statusStripeClickKeys"), Color.LightGreen);
   //             statusStripSet(2, fCTBCode.Lines[fCTBCodeClickedLineNow], Color.Orange);
            }

            fCTBCodeMarkLine(true);             // set Bookmark and marker in 2D-View
  //          statusStripSet(2, fCTBCode.Lines[fCTBCodeClickedLineNow], Color.Orange);
        }
        private void fCTBCode_KeyDown(object sender, KeyEventArgs e)    // key up down 
        {
//            if (gcode.loggerTrace && logMain) Logger.Trace("Event  fCTBCode_KeyDown {0} {1} {2}  clickedLine {3}", e.KeyValue, ModifierKeys, markedBlockType.ToString(), fCTBCodeClickedLineNow);
            int key = e.KeyValue;
            #region key up
            if (key == 38)                                  // up
            {   //if (!(fCTBCodeClickedLineNow > 0))
               //     return;
                if (Panel.ModifierKeys == Keys.Control)
                { moveSelectedCodeBlockUp(); }
                else if (Panel.ModifierKeys == (Keys.Control | Keys.Shift))
                { moveSelectedCodeBlockUp(true); }
                else
                {
                    if (markedBlockType == xmlMarkerType.Figure)
                    {
                        clearTextSelection(0);
                        markedBlockType = xmlMarkerType.none;
                        enableBlockCommands(false);                                 // disable CMS-Menu block-move items 
                        xmlMarker.GetFigure(xmlMarker.lastFigure.lineStart ,-1);    // find figure before
                        fCTBCodeClickedLineNow = xmlMarker.lastFigure.lineEnd;
                        if (gcode.loggerTrace && logMain) Logger.Trace("Figure up found {0}  {1}", xmlMarker.lastFigure.lineStart, xmlMarker.lastFigure.lineEnd);
                        findFigureMarkSelection(xmlMarkerType.Figure, fCTBCodeClickedLineNow);
                        enableBlockCommands(false);                                 // no idea why, code is selected, but not colored. Moving up/down of blocks doesn't work - better disable "isMarked"
   //                     statusStripSet(2, fCTBCode.Lines[fCTBCodeClickedLineNow], Color.Orange);
                    }
                    else if (markedBlockType == xmlMarkerType.Group)
                    {
                        clearTextSelection(0);
                        markedBlockType = xmlMarkerType.none;
                        enableBlockCommands(false);                                 // disable CMS-Menu block-move items 
                        xmlMarker.GetGroup(xmlMarker.lastGroup.lineStart, -1);    // find figure before
                        fCTBCodeClickedLineNow = xmlMarker.lastGroup.lineEnd;
                        if (gcode.loggerTrace && logMain) Logger.Trace("Group up found {0}  {1}", xmlMarker.lastGroup.lineStart, xmlMarker.lastGroup.lineEnd);
                        findFigureMarkSelection(xmlMarkerType.Group, fCTBCodeClickedLineNow);
                        enableBlockCommands(false);                                 // no idea why, code is selected, but not colored. Moving up/down of blocks doesn't work - better disable "isMarked"
      //                  statusStripSet(2, fCTBCode.Lines[fCTBCodeClickedLineNow], Color.Orange);
                    }
                    else
                    {   fCTBCodeClickedLineNow -= 1;
                        if (fCTBCodeClickedLineNow < 1) { fCTBCodeClickedLineNow = 1; }
                        while (fCTBCode.GetVisibleState(fCTBCodeClickedLineNow) == VisibleState.Hidden)
                            fCTBCodeClickedLineNow -= 1;
                        if (gcode.loggerTrace && logMain) Logger.Trace("Else up {0} ", markedBlockType.ToString());
                    }
                    fCTBCodeMarkLine();             // set Bookmark and marker in 2D-View
                    fCTBCode.DoCaretVisible();
                }
                statusStripClear(1);
                if (VisuGCode.codeBlocksAvailable() && figureIsMarked)
                {
                    statusStripSet(1, Localization.getString("statusStripeUpKeys"), Color.LightGreen);
                }
     //           else
     //               statusStripClear(2);
            }
            #endregion
            #region key down
            else if (key == 40)       // down
            {   //if (!(fCTBCodeClickedLineNow < (fCTBCode.Lines.Count - 1)))
               //     return;
                if (Panel.ModifierKeys == Keys.Control)
                { moveSelectedCodeBlockDown(); }
                else if (Panel.ModifierKeys == (Keys.Control | Keys.Shift))
                { moveSelectedCodeBlockDown(true); }
                else
                {
                    if (markedBlockType == xmlMarkerType.Figure)
                    {
                        clearTextSelection(0);
                        markedBlockType = xmlMarkerType.none;
                        enableBlockCommands(false);                                 // disable CMS-Menu block-move items 
                        xmlMarker.GetFigure(xmlMarker.lastFigure.lineStart ,1);
                        fCTBCodeClickedLineNow = xmlMarker.lastFigure.lineStart;
                        if (gcode.loggerTrace && logMain) Logger.Trace(" Figure down found {0}  {1}", xmlMarker.lastFigure.lineStart, xmlMarker.lastFigure.lineEnd);
                        findFigureMarkSelection(xmlMarkerType.Figure, fCTBCodeClickedLineNow);
                        enableBlockCommands(false);                                 // no idea why, code is selected, but not colored. Moving up/down of blocks doesn't work - better disable "isMarked"
        //                statusStripSet(2, fCTBCode.Lines[fCTBCodeClickedLineNow], Color.Orange);
                    }
                    else if (markedBlockType == xmlMarkerType.Group)
                    {
                        clearTextSelection(0);
                        markedBlockType = xmlMarkerType.none;
                        enableBlockCommands(false);                                 // disable CMS-Menu block-move items 
                        xmlMarker.GetGroup(xmlMarker.lastGroup.lineStart, 1);
                        fCTBCodeClickedLineNow = xmlMarker.lastGroup.lineStart;
                        if (gcode.loggerTrace && logMain) Logger.Trace(" Group down found {0}  {1}", xmlMarker.lastGroup.lineStart, xmlMarker.lastGroup.lineEnd);
                        findFigureMarkSelection(xmlMarkerType.Group, fCTBCodeClickedLineNow);
                        enableBlockCommands(false);                                 // no idea why, code is selected, but not colored. Moving up/down of blocks doesn't work - better disable "isMarked"
        //                statusStripSet(2, fCTBCode.Lines[fCTBCodeClickedLineNow], Color.Orange);
                    }
                    else
                    {   fCTBCodeClickedLineNow += 1;
                        if (fCTBCodeClickedLineNow >= fCTBCode.LinesCount) { fCTBCodeClickedLineNow = fCTBCode.LinesCount - 1; }
                        while (fCTBCode.GetVisibleState(fCTBCodeClickedLineNow) == VisibleState.Hidden)
                            fCTBCodeClickedLineNow += 1;
                    }
                    fCTBCodeMarkLine();                 // set Bookmark and marker in 2D-View
                    fCTBCode.DoCaretVisible();
                }
                statusStripClear(1);
                if (VisuGCode.codeBlocksAvailable() && figureIsMarked)
                {
                    statusStripSet(1, Localization.getString("statusStripeDownKeys"), Color.LightGreen);
 //                   statusStripSet(2, fCTBCode.Lines[fCTBCodeClickedLineNow], Color.Orange);
                }
  //              else
  //                  statusStripClear(2);
            }
            #endregion

            else if (!manualEdit && ((key == 8) || (key == 46)))    // delete
            {   unDo.setCode(fCTBCode.Text, "Delete", this);
                resetView = true;
            }
            else if (((key >= 32) && (key <= 127)) || (key == (char)13))
            {   setEditMode(true);}
        }

        private void fCTBCodeMarkLine(bool markAnyway=false)     // after click on gcode line, mark text and graphics
        {
            if ((fCTBCodeClickedLineNow <= fCTBCode.LinesCount) && (fCTBCodeClickedLineNow >= 0))
            {
                if ((fCTBCodeClickedLineNow != fCTBCodeClickedLineLast) || markAnyway)
                {
                    fCTBCode.UnbookmarkLine(fCTBCodeClickedLineLast);           // remove marker from old line
                    fCTBCode.BookmarkLine(fCTBCodeClickedLineNow);              // set new marker
                    fCTBCodeClickedLineLast = fCTBCodeClickedLineNow;
                    VisuGCode.setPosMarkerLine(fCTBCodeClickedLineNow, !isStreaming);
                    pictureBox1.Invalidate(); // avoid too much events
                    toolStrip_tb_StreamLine.Text = fCTBCodeClickedLineNow.ToString();
                }
            }
        }
        #endregion

        #region CMS Menu
        // context Menu on fastColoredTextBox
        private void cmsFCTB_ItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {
            if (e.ClickedItem.Name == "cmsCodeSelect")
                fCTBCode.SelectAll();
            else if (e.ClickedItem.Name == "cmsCodeCopy")
            {
                if (fCTBCode.SelectedText.Length > 0)
                    fCTBCode.Copy();
            }
            else if (e.ClickedItem.Name == "cmsCodePaste")
            {       //fCTBCode.Paste();
                IDataObject iData = Clipboard.GetDataObject();
                unDo.setCode(fCTBCode.Text, "Paste Code", this);
                fCTBCode.Text = (String)iData.GetData(DataFormats.Text);
                newCodeEnd();
                setLastLoadedFile("Data from Clipboard: Text", "");
                lbInfo.Text = "GCode from clipboard";
            }
            else if (e.ClickedItem.Name == "cmsEditMode")
            {   if (!manualEdit)
                {   setEditMode(true);  }
                else
                {   newCodeEnd(); }
            }
            else if (e.ClickedItem.Name == "cmsCodePasteSpecial1")  // Pen up
            {
                setEditMode(true);
                gcode.setup();
                StringBuilder gcodeString = new StringBuilder();
                gcode.PenUp(gcodeString, "pasted");
                setTextSelection(fCTBCodeClickedLineNow, fCTBCodeClickedLineNow);
                gcodeString.Append(fCTBCode.SelectedText.Replace("G01","G00")+" (modified)");
                Clipboard.SetText(gcodeString.ToString());
                fCTBCode.Paste();
                fCTBCodeMarkLine();
            }
            else if (e.ClickedItem.Name == "cmsCodePasteSpecial2")  // Pen down
            {
                setEditMode(true);
                gcode.setup();
                StringBuilder gcodeString = new StringBuilder();
                gcode.PenDown(gcodeString, "pasted");
                setTextSelection(fCTBCodeClickedLineNow, fCTBCodeClickedLineNow);
                gcodeString.Append(fCTBCode.SelectedText.Replace("G00", "G01")+" F"+gcode.gcodeXYFeed + " (modified)");
                Clipboard.SetText(gcodeString.ToString());
                fCTBCode.Paste();
                fCTBCodeMarkLine();
            }
            else if (e.ClickedItem.Name == "cmsCodeSendLine")
            {
                int clickedLine = fCTBCode.Selection.ToLine;
                sendCommand(fCTBCode.Lines[clickedLine], false);
            }
            else if (e.ClickedItem.Name == "cmsCommentOut")
                fCTB_CheckUnknownCode();
            else if (e.ClickedItem.Name == "cmsUpdate2DView")
                newCodeEnd();
            else if (e.ClickedItem.Name == "cmsReplaceDialog")
                fCTBCode.ShowReplaceDialog();
            else if (e.ClickedItem.Name == "cmsFindDialog")
                fCTBCode.ShowFindDialog();
            else if (e.ClickedItem.Name == "cmsEditorHotkeys")
            { showMessageForm(Properties.Resources.fctb_hotkeys); }
        }

        public void showMessageForm(string text)
        {
            if (_message_form == null)
            {
                _message_form = new MessageForm();
                _message_form.FormClosed += formClosed_MessageForm;
            }
            else
            {
                _message_form.Visible = false;
            }
            _message_form.showMessage("Information", text, 0);
            _message_form.Show(this);
            _message_form.WindowState = FormWindowState.Normal;
        }
        private void formClosed_MessageForm(object sender, FormClosedEventArgs e)
        { _message_form = null; }

        #endregion

        private void setEditMode(bool set)
        {
            manualEdit = set;
            fCTBCode.BackColor = set? Color.FromArgb(255, 255, 255, 100): Color.White;
            if (set)
                statusStripSet(1, Localization.getString("statusStripeEditModeOn"), Color.FromArgb(255, 255, 255, 100));
            else
                statusStripClear(1, 2);
        }

        private void enableCmsCodeBlocks(bool enable)
        {
            cmsCodeBlocksFold.Enabled = enable;
            cmsCodeBlocksMove.Enabled = enable;
            cmsCodeBlocksSort.Enabled = enable;
            cmsCodeBlocksRemoveAll.Enabled = enable;
            cmsCodeBlocksRemoveGroup.Enabled = enable;
        }

        #region find blocks
        private static bool figureIsMarked = false;
        private static xmlMarkerType markedBlockType = xmlMarkerType.none;
        private void findFigureMarkSelection(xmlMarkerType marker, int clickedLine, bool collapse=true)   // called by click on figure in 2D view
        {
            if (manualEdit)
                return;
            if (logDetailed) Logger.Trace(" findFigureMarkSelection marker:{0}  line:{1}  collapse:{2}",marker, clickedLine, collapse);
            fCTBCode.Selection.ColumnSelectionMode = false;
            statusStripClear(2);
            enableBlockCommands(false);
            markedBlockType = marker;
            Color highlight = Color.GreenYellow;

            if (marker == xmlMarkerType.Group)
            {
                if (xmlMarker.GetGroupCount() > 0)
                {   if (collapse) { fCTBCode.CollapseAllFoldingBlocks(); foldLevel = 1; }
                    if (xmlMarker.GetGroup(clickedLine))
                    {   fCTBCode.ExpandFoldedBlock(xmlMarker.lastGroup.lineStart);
                        enableBlockCommands(setTextSelection(xmlMarker.lastGroup.lineStart, xmlMarker.lastGroup.lineEnd));
                        VisuGCode.markSelectedGroup(xmlMarker.lastGroup.lineStart);
                        statusStripSet(2,string.Format("Marked: {0}", fCTBCode.Lines[xmlMarker.lastGroup.lineStart]), highlight);
                        pictureBox1.Invalidate();
                        fCTBCode.Invalidate();
                    }

                }
                else if (xmlMarker.GetFigureCount() > 0)
                {   if (collapse)
                        foldBlocks2();
                    markedBlockType = xmlMarkerType.Figure;
                    if (xmlMarker.GetFigure(clickedLine)) 
                    {   enableBlockCommands(setTextSelection(xmlMarker.lastFigure.lineStart, xmlMarker.lastFigure.lineEnd));
                        VisuGCode.setPosMarkerLine(fCTBCodeClickedLineNow, false);	//!isStreaming);	// 2020-08-24 don't highlight in setPosMarkerLine - was done (or deselect) before in setPosMarkerNearBy
                        statusStripSet(2,string.Format("Marked: {0}", fCTBCode.Lines[xmlMarker.lastFigure.lineStart]), highlight);
                        fCTBCode.Invalidate();
                    }
                }
            }
            else if (marker == xmlMarkerType.Figure)
            {   if (xmlMarker.GetGroupCount() > 0)
                {   if (collapse)
                    { foldBlocks2(); }
                }
                else
                {   if (collapse)
                    { fCTBCode.CollapseAllFoldingBlocks(); foldLevel = 1; }
                }
                if (xmlMarker.GetFigure(clickedLine))
                {   enableBlockCommands(setTextSelection(xmlMarker.lastFigure.lineStart, xmlMarker.lastFigure.lineEnd));
                    VisuGCode.setPosMarkerLine(fCTBCodeClickedLineNow, false);		//!isStreaming);	// 2020-08-24 don't highlight in setPosMarkerLine - was done (or deselect) before in setPosMarkerNearBy
                    statusStripSet(2,string.Format("Marked: {0}", fCTBCode.Lines[xmlMarker.lastFigure.lineStart]), highlight);
                    fCTBCode.Invalidate();
                }
                if (xmlMarker.lastFigure.lineStart < fCTBCode.LinesCount)
                    fCTBCode.ExpandFoldedBlock(xmlMarker.lastFigure.lineStart);   
            }
            else if (marker == xmlMarkerType.Line)
            {   if (xmlMarker.GetGroupCount() > 0)
                {   if (collapse)
                    { foldBlocks2(); }
                }
                else
                {   if (collapse)
                    { fCTBCode.CollapseAllFoldingBlocks(); foldLevel = 1; }
                }
                if (xmlMarker.GetFigure(clickedLine))
                {   fCTBCode.ExpandFoldedBlock(xmlMarker.lastFigure.lineStart);    }
                if (clickedLine == xmlMarker.lastFigure.lineStart)
                    clickedLine++;

                fCTBCodeClickedLineNow = clickedLine;

                setTextSelection(clickedLine, clickedLine);
                enableBlockCommands(false, true);
            }
            fCTBCodeClickedLineNow = xmlMarker.lastFigure.lineStart;
            fCTBCode.DoCaretVisible();
            this.Invalidate();
        }

        private void clearTextSelection(int line)
        {   if (line >= fCTBCode.LinesCount)
				return;
			Range mySelection = new Range(fCTBCode,0,line,0,line);
            fCTBCode.Selection = mySelection;
            fCTBCode.SelectionColor = Color.Green;
        }
        private bool setTextSelection(int start, int end)
        {
            Place selStart, selEnd;
            selStart.iLine = start;
            selStart.iChar = 0;
            Range mySelection = new Range(fCTBCode);
            mySelection.Start = selStart;
            selEnd.iLine = end;
            selEnd.iChar = fCTBCode.Lines[end].Length;
            mySelection.End = selEnd;
            if ((selStart == fCTBCode.Selection.Start) && (selEnd == fCTBCode.Selection.End))
            { return false; }    // was selected, now deselected

            fCTBCode.Selection = mySelection;
            fCTBCode.SelectionColor = Color.Red;
            return true;
        }

        private bool deleteMarkedCode = false;
        private void deletenotMarkToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (deleteMarkedCode)
            {   deleteMarkedCode = false;
                deletenotMarkToolStripMenuItem.Text = "Mark (not delete)";
            }
            else
            {   deleteMarkedCode = true;
                deletenotMarkToolStripMenuItem.Text = "Delete (not mark)";
            }
        }
        #endregion

        #region fold blocks
        private static int foldLevel = 0;
        private void foldBlocks1stToolStripMenuItem1_Click(object sender, EventArgs e)  // fold all blocks
        {   fCTBCode.CollapseAllFoldingBlocks(); foldLevel = 1; }

        private void foldBlocks2ndToolStripMenuItem1_Click(object sender, EventArgs e)  // fold 2nd level blocks
        { foldBlocks2(); }
        private void foldBlocks2()
        {   string tmp;
            fCTBCode.ExpandAllFoldingBlocks();
            for (int i = 0; i < fCTBCode.LinesCount; i++)
            {   tmp = fCTBCode.GetLine(i).Text;
                if (tmp.Contains(xmlMarker.figureStart) || tmp.Contains(xmlMarker.contourStart)|| tmp.Contains(xmlMarker.fillStart)|| tmp.Contains(xmlMarker.passStart))
                {    fCTBCode.CollapseFoldingBlock(i);  }
            }
            foldLevel = 2;
        }
        private void foldBlocks3rdToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            string tmp;
            fCTBCode.ExpandAllFoldingBlocks();
            for (int i = 0; i < fCTBCode.LinesCount; i++)
            {   tmp = fCTBCode.GetLine(i).Text;
                if (tmp.Contains(xmlMarker.contourStart) || tmp.Contains(xmlMarker.fillStart) || tmp.Contains(xmlMarker.passStart))
                { fCTBCode.CollapseFoldingBlock(i); }
            }
            foldLevel = 3;
        }

        private void expandCodeBlocksToolStripMenuItem_Click(object sender, EventArgs e)
        {   fCTBCode.ExpandAllFoldingBlocks(); foldLevel = 0; fCTBCode.DoCaretVisible(); }

        private void enableBlockCommands(bool tmp, bool keepSelection=false)
        {
            if (!tmp && !keepSelection)
                clearTextSelection(fCTBCodeClickedLineNow);
            figureIsMarked = tmp;
            cmsFCTBMoveSelectedCodeBlockMostUp.Enabled = tmp;
            cmsFCTBMoveSelectedCodeBlockUp.Enabled = tmp;
            cmsFCTBMoveSelectedCodeBlockDown.Enabled = tmp;
            cmsFCTBMoveSelectedCodeBlockMostDown.Enabled = tmp;

            cmsPicBoxDeletePath.Enabled = tmp;
            cmsPicBoxCropSelectedPath.Enabled = tmp;
            cmsPicBoxReverseSelectedPath.Enabled = tmp;
        }
        #endregion

        #region move blocks
        // Move code block upwards
        private void moveSelectedCodeBlockMostUpToolStripMenuItem_Click(object sender, EventArgs e)
        {   fCTBCode.UnbookmarkLine(fCTBCodeClickedLineNow);
			if (logDetailed) Logger.Trace("moveSelectedCodeBlockMostUpToolStripMenuItem_Click");
            moveSelectedCodeBlockUp(true);
			xmlMarker.listIDs();
        }
        private void moveSelectedCodeBlockUpToolStripMenuItem_Click(object sender, EventArgs e)
        {   fCTBCode.UnbookmarkLine(fCTBCodeClickedLineNow);
			if (logDetailed) Logger.Trace("moveSelectedCodeBlockUpToolStripMenuItem_Click");
            moveSelectedCodeBlockUp();
			xmlMarker.listIDs();
        }
        private void moveSelectedCodeBlockUp(bool mostTop=false)
        {   if (!figureIsMarked)
                return;
            xmlMarker.BlockData block;
            int insert = 0;
            if (markedBlockType == xmlMarkerType.Figure)
            {   block = xmlMarker.lastFigure;       // save current selection range
                if (mostTop)
                    insert = xmlMarker.FindInsertPositionFigureMostTop(block.lineStart);
                else
                    insert = xmlMarker.FindInsertPositionFigureTop(block.lineStart);
                if (moveBlockUp(block.lineStart, block.lineEnd, insert))
                {   newCodeEnd();
                    resetView = true;
                    fCTBCodeClickedLineNow = insert;
                }
            }
            else if (markedBlockType == xmlMarkerType.Group)
            {
                block = xmlMarker.lastGroup;       // save current selection range
                if (mostTop)
                    insert = xmlMarker.FindInsertPositionGroupMostTop(block.lineStart);
                else
                    insert = xmlMarker.FindInsertPositionGroupTop(block.lineStart);
                if (moveBlockUp(block.lineStart, block.lineEnd, insert))
                {   newCodeEnd();
                    resetView = true;
                    fCTBCodeClickedLineNow = insert;
                }
            }
        }
        private bool moveBlockUp(int blockStart, int blockEnd, int insert)
        {
            if (insert >= 0)
            {
                if (gcode.loggerTrace && logMain) Logger.Trace(" moveBlockUp blockStart:{0} Text:{1},  blockEnd:{2} insert:{3}  Text:{4}", blockStart, fCTBCode.Lines[blockStart].Substring(0, 10), blockEnd, insert, fCTBCode.Lines[insert].Substring(0, 10));
                Bookmarks tmp = new Bookmarks(fCTBCode);
                tmp.Clear();
                fCTBCode.Cut();
                List<int> remove = new List<int>();
                remove.Add(blockStart);
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


        private void moveSelectedCodeBlockMostDownToolStripMenuItem_Click(object sender, EventArgs e)
        {   fCTBCode.UnbookmarkLine(fCTBCodeClickedLineNow);
			if (logDetailed) Logger.Trace("moveSelectedCodeBlockMostDownToolStripMenuItem_Click");
            moveSelectedCodeBlockDown(true);
			xmlMarker.listIDs();
        }
        private void moveSelectedCodeBlockDownToolStripMenuItem_Click(object sender, EventArgs e)
        {   fCTBCode.UnbookmarkLine(fCTBCodeClickedLineNow);
			if (logDetailed) Logger.Trace("moveSelectedCodeBlockDownToolStripMenuItem_Click");
            moveSelectedCodeBlockDown();
			xmlMarker.listIDs();
        }

        private void moveSelectedCodeBlockDown(bool mostBottom = false)
        {
            if (!figureIsMarked)
                return;
            xmlMarker.BlockData block;
            int insert = 0;
            if (markedBlockType == xmlMarkerType.Figure)
            {   block = xmlMarker.lastFigure;       // save current selection range
                if (mostBottom)
                    insert = xmlMarker.FindInsertPositionFigureMostBottom(block.lineStart);
                else
                    insert = xmlMarker.FindInsertPositionFigureBottom(block.lineStart);
                if (moveBlockDown(block.lineStart, block.lineEnd, insert))
                {   newCodeEnd();
                    resetView = true;
                    fCTBCodeClickedLineNow = insert-1;
                }
            }
            else if (markedBlockType == xmlMarkerType.Group)
            {
                block = xmlMarker.lastGroup;       // save current selection range
                if (mostBottom)
                    insert = xmlMarker.FindInsertPositionGroupMostBottom(block.lineStart);
                else
                    insert = xmlMarker.FindInsertPositionGroupBottom(block.lineStart);
                if (moveBlockDown(block.lineStart, block.lineEnd, insert))
                {   newCodeEnd();
                    resetView = true;
                    fCTBCodeClickedLineNow = insert-1;
                }
            }
        }
        private bool moveBlockDown(int blockStart, int blockEnd, int insert)
        {
            if (insert >= blockEnd)
            {
                if (gcode.loggerTrace && logMain) Logger.Trace(" moveBlockDown blockStart:{0} Text:{1},  blockEnd:{2} insert:{3}  Text:{4}", blockStart, fCTBCode.Lines[blockStart].Substring(0,10), blockEnd, insert, fCTBCode.Lines[insert].Substring(0, 10));
                Bookmarks tmp = new Bookmarks(fCTBCode);
                tmp.Clear();
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
                List<int> remove = new List<int>();
                remove.Add(blockStart);
                fCTBCode.RemoveLines(remove);           // remove empty line after cut
                return true;
            }
            return false;
        }
        #endregion

        #region remove tags
        private void cmsCodeBlocksRemoveGroup_Click(object sender, EventArgs e)
        {   removeXMLTags(true); }
        private void cmsCodeBlocksRemoveAll_Click(object sender, EventArgs e)
        { removeXMLTags(); }
        private void removeXMLTags(bool groupsOnly=false)
        {   unDo.setCode(fCTBCode.Text, cmsCodeBlocksRemoveAll.Text, this);
            clearTextSelection(0);
            manualEdit = true;
            if (gcode.loggerTrace && logMain) Logger.Trace(" removeXMLTags groupsOnly:{0} ", groupsOnly);
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
            transformEnd();
        }
        #endregion

        #region sort blocks
		// GroupOptions { none = 0, ByColor = 1, ByWidth = 2, ByLayer = 3, ByType = 4, ByTile = 5};
		// 
        private void cmsCodeBlocksSortById_Click(object sender, EventArgs e)
        {   CodeBlocksSort(xmlMarker.sortOption.Id);}
        private void cmsCodeBlocksSortByColor_Click(object sender, EventArgs e)
        {   CodeBlocksSort(xmlMarker.sortOption.Color);}
        private void cmsCodeBlocksSortByWidth_Click(object sender, EventArgs e)
        {   CodeBlocksSort(xmlMarker.sortOption.Width);}
        private void cmsCodeBlocksSortByLayer_Click(object sender, EventArgs e)
        {   CodeBlocksSort(xmlMarker.sortOption.Layer);}
        private void cmsCodeBlocksSortByType_Click(object sender, EventArgs e)
        {   CodeBlocksSort(xmlMarker.sortOption.Type);}
        private void cmsCodeBlocksSortByGeometry_Click(object sender, EventArgs e)
        {   CodeBlocksSort(xmlMarker.sortOption.Geometry);}
        private void cmsCodeBlocksSortByToolNr_Click(object sender, EventArgs e)
        {   CodeBlocksSort(xmlMarker.sortOption.ToolNr);}
        private void cmsCodeBlocksSortByToolName_Click(object sender, EventArgs e)
        {   CodeBlocksSort(xmlMarker.sortOption.ToolName);}
        private void cmsCodeBlocksSortByCodeSize_Click(object sender, EventArgs e)
        {   CodeBlocksSort(xmlMarker.sortOption.CodeSize);}
        private void cmsCodeBlocksSortByCodeArea_Click(object sender, EventArgs e)
        {   CodeBlocksSort(xmlMarker.sortOption.CodeArea);}

        private void cmsCodeBlocksSortByDistance_Click(object sender, EventArgs e)
        {   CodeBlocksSort(xmlMarker.sortOption.Distance); }

		private void CodeBlocksSort(xmlMarker.sortOption tmp)
		{
			Cursor.Current = Cursors.WaitCursor; 
			xmlMarker.sortBy(tmp, cmsCodeBlocksSortReverse.Checked);
            getSortedCode(tmp); 
			Cursor.Current = Cursors.Default;
			xmlMarker.listIDs();
		}

        private void getSortedCode(xmlMarker.sortOption tmp)
        {   unDo.setCode(fCTBCode.Text, cmsCodeBlocksRemoveAll.Text, this);
            clearTextSelection(0);
            manualEdit = true;
            if (gcode.loggerTrace && logMain) Logger.Trace(" sortXMLTags by {0}",tmp.ToString());
            
            fCTBCode.Text = xmlMarker.getSortedCode(fCTBCode.Lines.ToArray<string>());
            transformEnd();
            if      (foldLevel == 1) { fCTBCode.CollapseAllFoldingBlocks(); foldLevel = 1; }
            else if (foldLevel == 2) foldBlocks2();
        }


        #endregion
    }
}
