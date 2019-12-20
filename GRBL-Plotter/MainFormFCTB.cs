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
        private bool commentOut;        // comment out unknown GCode to avoid errors from GRBL

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
            e.ChangedRange.SetFoldingMarkers(xmlMarker.fillStart, xmlMarker.fillEnd);
            e.ChangedRange.SetFoldingMarkers(xmlMarker.contourStart, xmlMarker.contourEnd);
            e.ChangedRange.SetFoldingMarkers(xmlMarker.passStart, xmlMarker.passEnd);
            e.ChangedRange.SetFoldingMarkers(xmlMarker.figureStart, xmlMarker.figureEnd);
  //          if (Properties.Settings.Default.importGCZIncEnable)
  //               fCTBCode.CollapseAllFoldingBlocks();
            e.ChangedRange.SetFoldingMarkers(xmlMarker.groupStart, xmlMarker.groupEnd);

            codeBlocksToolStripMenuItem.Enabled = visuGCode.codeBlocksAvailable();
        }

        private void fCTB_CheckUnknownCode()
        {   string curLine;
            string allowed = "ABCNGMFIJKLNPRSTUVWXYZOPLabcngmfijklnprstuvwxyzopl ";
            string number = " +-.0123456789";
            string cmt = "(;";
            string message = "";
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
                            message += "Line " + i.ToString() + " : " + curLine + "\r\n";
                        }
                    }
                }
                else if ((curLine.Length > 0) && (allowed.IndexOf(curLine[0]) < 0))     // if 1st char is unknown - no gcode
                {
                    fCTBCode.Selection = fCTBCode.GetLine(i);
                    fCTBCode.SelectedText = "(" + curLine + " <- unknown command)";
                    message += "Line " + i.ToString() + " : " + curLine + "\r\n";
                }
                else if ((curLine.Length > 1) && (number.IndexOf(curLine[1]) < 0))  // if 1st known but 2nd not part of number
                {
                    fCTBCode.Selection = fCTBCode.GetLine(i);
                    fCTBCode.SelectedText = "(" + curLine + " <- unknown command)";
                    message += "Line " + i.ToString() + " : " + curLine + "\r\n";
                }
            }
            if (message.Length > 0)
                MessageBox.Show(Localization.getString("mainUnknownCode") + message);
        }

        // mark clicked line in editor
        private int fCTBCodeClickedLineNow = 0;
        private int fCTBCodeClickedLineLast = 0;
        private void fCTBCode_Click(object sender, EventArgs e)
        {
            fCTBCodeClickedLineNow = fCTBCode.Selection.ToLine;
            Logger.Trace(" click {0}", fCTBCodeClickedLineNow);
            fCTBCodeMarkLine();
        }
        private void fCTBCode_KeyDown(object sender, KeyEventArgs e)    // up down arrow
        {
            int key = e.KeyValue;
            int fline = -1;
            int oldline = fCTBCodeClickedLineNow;
   //         MessageBox.Show(fCTBCodeClickedLineNow.ToString());
            if ((key == 38) && (fCTBCodeClickedLineNow > 0))        // up
            {
     /*           if (foldLevel > 0)
                {
                    if (fCTBCode.Lines[fCTBCodeClickedLineNow].Contains(xmlMarker.groupEnd))
                    { fline = findLine(fCTBCodeClickedLineNow - 2, xmlMarker.groupEnd, false); }
                    else if (fCTBCode.Lines[fCTBCodeClickedLineNow].Contains(xmlMarker.figureEnd))
                    { fline = findLine(fCTBCodeClickedLineNow - 2, xmlMarker.figureEnd, false); }
                    if (fline >= 0)
                    {   fCTBCodeClickedLineNow = fline;
               //         if (foldLevel == 1) foldBlocks1stToolStripMenuItem1_Click(sender, e);
               //         else if (foldLevel == 2) foldBlocks2ndToolStripMenuItem1_Click(sender, e);
               //         else if (foldLevel == 3) foldBlocks3rdToolStripMenuItem1_Click(sender, e);
                    }
                    Logger.Trace(" up old {0}  new {1}", oldline, fCTBCodeClickedLineNow);
                }
                else*/
                    fCTBCodeClickedLineNow -= 1;
                fCTBCodeMarkLine();
            }
            if ((key == 40) && (fCTBCodeClickedLineNow < (fCTBCode.Lines.Count - 1)))       // down
            {
      /*          if (foldLevel > 0)
                {
                    if (fCTBCode.Lines[fCTBCodeClickedLineNow].Contains(xmlMarker.groupEnd))
                    { fline = findLine(fCTBCodeClickedLineNow + 2, xmlMarker.groupEnd); }
                    else if (fCTBCode.Lines[fCTBCodeClickedLineNow].Contains(xmlMarker.figureEnd))
                    { fline = findLine(fCTBCodeClickedLineNow + 2, xmlMarker.figureEnd); }
                    if (fline >= 0)
                    {   fCTBCodeClickedLineNow = fline;
                //        if (foldLevel == 1) foldBlocks1stToolStripMenuItem1_Click(sender, e);
                //        else if (foldLevel == 2) foldBlocks2ndToolStripMenuItem1_Click(sender, e);
               //         else if (foldLevel == 3) foldBlocks3rdToolStripMenuItem1_Click(sender, e);
                    }
                    Logger.Trace(" down old {0}  new {1}", oldline, fCTBCodeClickedLineNow);
                }
                else*/
                    fCTBCodeClickedLineNow += 1;
                fCTBCodeMarkLine();
            }
        }
        private int findLine(int start, string txt, bool down = true)
        {   if (down)
            {   for (int i = start; i < fCTBCode.Lines.Count; i++)
                { if (fCTBCode.Lines[i].Contains(txt))
                        return i;
                }
                return -1;
            }
            else
            {   for (int i = start; i > 0; i--)
                {   if (fCTBCode.Lines[i].Contains(txt))
                        return i;
                }
                return -1;
            }
        }

        private Bookmark fCTBBookmark = null;
        private void fCTBCodeMarkLine()     // click in text
        {
            if ((fCTBCodeClickedLineNow <= fCTBCode.LinesCount) && (fCTBCodeClickedLineNow >= 0))
            {
                fCTBBookmark = new Bookmark(fCTBCode, "marked", fCTBCodeClickedLineNow);
                if (fCTBCodeClickedLineNow != fCTBCodeClickedLineLast)
                {
                    fCTBCode.UnbookmarkLine(fCTBCodeClickedLineLast);
                    fCTBCode.BookmarkLine(fCTBCodeClickedLineNow);
                    fCTBCodeClickedLineLast = fCTBCodeClickedLineNow;

                    // Set marker in drawing
                    Place tst = fCTBCode.Selection.Start;
                    visuGCode.setPosMarkerLine(fCTBCodeClickedLineNow,!isStreaming);

                    if (fCTBCode.Lines[tst.iLine].Contains(xmlMarker.groupStart))
                        visuGCode.markSelectedGroup(tst.iLine);

                    setFigureIsMarked(false);
                    int start = visuGCode.getLineOfFirstPointInFigure();
                    int end = visuGCode.getLineOfEndPointInFigure(start + 1);
//                    Logger.Trace("fCTBCodeMarkLine Start {0}  end {1}", start, end);
                    if ((start >= 0) && (end > start))
                    {
                        figureLastStart = start;
                        Place selStart, selEnd;
                        selStart.iLine = start;
                        selStart.iChar = 0;
                        Range mySelection = fCTBCode.Range;
                        mySelection.Start = selStart;
                        selEnd.iLine = end;
                        selEnd.iChar = 0;
                        mySelection.End = selEnd;
                        fCTBCode.Selection = mySelection;
                        fCTBCode.SelectionColor = Color.Red;// selectionColor;
//                        Logger.Trace(" Start {0}  end {1}", start, end);
                        setFigureIsMarked(true);
                    }
                    pictureBox1.Invalidate(); // avoid too much events
                    toolStrip_tb_StreamLine.Text = fCTBCodeClickedLineNow.ToString();
                }
            }
        }

            // context Menu on fastColoredTextBox
        private void cmsCode_ItemClicked(object sender, ToolStripItemClickedEventArgs e)
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
                newCodeStart();
                fCTBCode.Text = (String)iData.GetData(DataFormats.Text);
                newCodeEnd();
                setLastLoadedFile("Data from Clipboard: Text", "");
                lbInfo.Text = "GCode from clipboard";
            }
            else if (e.ClickedItem.Name == "cmsCodePasteSpecial1")
            {
                Place selStart, selEnd;
                Range mySelection = fCTBCode.Range;
                selStart.iLine = fCTBCodeClickedLineNow + 1;
                selStart.iChar = 0;
                selEnd.iLine = fCTBCodeClickedLineNow + 2;
                selEnd.iChar = 0;
                gcode.setup();
                StringBuilder gcodeString = new StringBuilder();
                gcodeString.Append("\r\n");
                gcode.PenUp(gcodeString, "pasted");
                string tmp = fCTBCode.Lines[fCTBCodeClickedLineNow + 1];
                if (tmp.Contains("G01"))
                {
                    mySelection.Start = selStart;
                    mySelection.End = selEnd;
                    fCTBCode.Selection = mySelection;
                    fCTBCode.SelectedText = (tmp.Replace("G01", "G00")) + "\r\n";
                }
                else
                    gcodeString.Append("G00\r\n");
                Clipboard.SetText(gcodeString.ToString());
                // restore old cursor pos. to insert code
                selStart.iLine = fCTBCodeClickedLineNow + 1;
                selEnd.iLine = fCTBCodeClickedLineNow + 1;
                mySelection.Start = selStart;
                mySelection.End = selEnd;
                fCTBCode.Selection = mySelection;
                fCTBCode.Paste();
                // set new cursor pos. for assumed PenDown command
                fCTBCodeClickedLineNow += gcodeString.ToString().Split(new[] { "\r\n", "\r", "\n" }, StringSplitOptions.None).Length; // 
                selStart.iLine = fCTBCodeClickedLineNow + 1;
                selEnd.iLine = fCTBCodeClickedLineNow + 1;
                mySelection.Start = selStart;
                mySelection.End = selEnd;
                fCTBCode.Selection = mySelection;
                fCTBCodeMarkLine();
            }
            else if (e.ClickedItem.Name == "cmsCodePasteSpecial2")
            {
                gcode.setup();
                StringBuilder gcodeString = new StringBuilder();
                gcode.PenDown(gcodeString, "pasted");
                string tmp = fCTBCode.Lines[fCTBCodeClickedLineNow + 1];
                if (!tmp.Contains("G01"))
                    gcodeString.Append("G01\r\n");
                gcodeString.Append("\r\n");
                Clipboard.SetText(gcodeString.ToString());
                fCTBCode.Paste();
                newCodeEnd();       // redraw 2D view
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
            _message_form.showMessage("Information",text);
            _message_form.Show(this);
            _message_form.WindowState = FormWindowState.Normal;
        }
        private void formClosed_MessageForm(object sender, FormClosedEventArgs e)
        { _message_form = null;  }

        #endregion
        private void moveToFirstPosToolStripMenuItem_Click(object sender, EventArgs e)
        {
            int start = findFigureMarkSelection(Color.Red);
            fCTBCodeClickedLineNow = start;
            fCTBCodeMarkLine();
            fCTBCode.DoCaretVisible();
            return;
        }
        private void cutOutSelectedPathToolStripMenuItem_Click(object sender, EventArgs e)
        {
            transformStart("Crop selection (Cut out)",false);
            fCTBCode.Text = visuGCode.cutOutFigure();
            transformEnd();
        }

        private static bool figureIsMarked = false;
        private static int figureLastStart = 0;
        private int findFigureMarkSelection(Color selectionColor)   // called by click on figure in 2D view
        {
            setFigureIsMarked(false);
            figureLastStart = 0;
            int start = visuGCode.getLineOfFirstPointInFigure();
            int end = visuGCode.getLineOfEndPointInFigure(start + 1);
            if ((start >= 0) && (end > start))
            {
//                Logger.Trace("findFigureMarkSelection start {0} end {1}",start, end);
                figureLastStart = start;
                Place selStart, selEnd;
                selStart.iLine = start;
                selStart.iChar = 0;
                selEnd.iLine = end;
                selEnd.iChar = 0;
                Range mySelection = fCTBCode.Range;
                mySelection.Start = selStart;
                mySelection.End = selEnd;
                fCTBCode.Selection = mySelection;
                fCTBCode.SelectionColor = selectionColor;
                setFigureIsMarked(true);
            }
            return start;
        }

        private void deletePathToolStripMenuItem_Click(object sender, EventArgs e)
        {
            int start = findFigureMarkSelection(Color.Red);
            if (start < 0)
                return;
            fCTBCodeClickedLineNow = start;
            fCTBCode.InsertText("( Figure removed )\r\n");
            fCTBCodeMarkLine();
            fCTBCode.DoCaretVisible();
            transformEnd();
            return;
        }

        private void deleteThisCodeLineToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (fCTBCode.LinesCount < 1) return;
            fCTBCodeClickedLineLast = 1;
            fCTBCodeMarkLine();
            if (deleteMarkedCode)
            {
                fCTBCode.Cut();
                fCTBCodeClickedLineNow--;
                fCTBCodeMarkLine();
            }
            fCTBCode.DoCaretVisible();
            newCodeEnd();
            return;
        }

        private bool deleteMarkedCode = false;
        private void deletenotMarkToolStripMenuItem_Click(object sender, EventArgs e)
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

        private static int foldLevel = 0;
        private void foldBlocks1stToolStripMenuItem1_Click(object sender, EventArgs e)  // fold all blocks
        {   fCTBCode.CollapseAllFoldingBlocks(); foldLevel = 1; }

        private void foldBlocks2ndToolStripMenuItem1_Click(object sender, EventArgs e)  // fold 2nd level blocks
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
        {   fCTBCode.ExpandAllFoldingBlocks(); foldLevel = 0; }

        private void moveSelectedCodeBlockUpToolStripMenuItem_Click(object sender, EventArgs e)
        {   // find figure-nr one line above
            int start = visuGCode.getLineOfFirstPointInFigure();        // 1st line of marked figure
            start = visuGCode.getLineOfFirstPointInFigure(start-2);     // now 1st line of prev figure
            fCTBCode.UnbookmarkLine(fCTBCodeClickedLineLast);
            if ((start >= 0) && figureIsMarked)
            {
                fCTBBookmark = null;
                Bookmarks tmp = new Bookmarks(fCTBCode);
                tmp.Clear();
                fCTBCode.Cut();
                Place selStart, selEnd;
                selStart.iLine = start;
                selStart.iChar = 0;
                Range mySelection = fCTBCode.Range;
                mySelection.Start = selStart;
                selEnd.iLine = start;
                selEnd.iChar = 0;
                mySelection.End = selEnd;
                fCTBCode.Selection = mySelection;
                
                fCTBCode.Paste();
                fCTBCodeClickedLineNow = start;
                transformEnd();

                if (foldLevel > 0)
                { foldBlocks2ndToolStripMenuItem1_Click(sender, e); }
                newCodeEnd();
            }
            setFigureIsMarked(false);
        }

        private void setFigureIsMarked(bool tmp)
        {   figureIsMarked = tmp;
            moveSelectedCodeBlockUpToolStripMenuItem1.Enabled = tmp;
            moveSelectedCodeBlockDownToolStripMenuItem1.Enabled = tmp;
        }

        private void moveSelectedCodeBlockDownToolStripMenuItem_Click(object sender, EventArgs e)
        {
            int start = visuGCode.getLineOfFirstPointInFigure();
            int end = visuGCode.getLineOfEndPointInFigure(start+1);
            fCTBCode.UnbookmarkLine(fCTBCodeClickedLineLast);
            start = visuGCode.getLineOfFirstPointInFigure(end+2);
            if (start <= 0)
                return;
            end = visuGCode.getLineOfEndPointInFigure(start + 1);
            if (start <= 0)
                return;
            if ((end > 0) && figureIsMarked)
            {
                fCTBBookmark = null;
                Bookmarks tmp = new Bookmarks(fCTBCode);
                tmp.Clear();
                fCTBCode.Copy();
                Range oldSelection = fCTBCode.Selection.Clone();
                Place selStart, selEnd;
                selStart.iLine = end;
                selStart.iChar = 0;
                Range mySelection = fCTBCode.Range;
                mySelection.Start = selStart;
                selEnd.iLine = end;
                selEnd.iChar = 0;
                mySelection.End = selEnd;
                fCTBCode.Selection = mySelection;

                fCTBCode.Paste();

                fCTBCode.Selection = oldSelection;
                fCTBCode.Cut();
                transformEnd();

                if (foldLevel > 0)
                { foldBlocks2ndToolStripMenuItem1_Click(sender, e); }
                newCodeEnd();
            }
            setFigureIsMarked(false);
        }
    }
}
