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
*/

#define debuginfo

using System;
using System.Drawing;
using System.Windows.Forms;
using FastColoredTextBoxNS;

namespace GRBL_Plotter
{
    public partial class MainForm : Form
    {
        private bool commentOut;        // comment out unknown GCode to avoid errors from GRBL

        #region fCTB FastColoredTextBox related
        // highlight code in editor
        private Style StyleComment = new TextStyle(Brushes.Gray, null, FontStyle.Italic);
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

        private void fCTBCode_TextChanged(object sender, FastColoredTextBoxNS.TextChangedEventArgs e)
        {
#if (debuginfo)
            log.Add("MainFormFCTB event fCTBCode_TextChanged");
#endif
            e.ChangedRange.ClearStyle(StyleComment);
            e.ChangedRange.SetStyle(StyleComment, "(\\(.*\\))", System.Text.RegularExpressions.RegexOptions.Compiled);
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
                MessageBox.Show("Fixed some unknown GCode:\r\n" + message);
        }

        // mark clicked line in editor
        private int fCTBCodeClickedLineNow = 0;
        private int fCTBCodeClickedLineLast = 0;
        private void fCTBCode_Click(object sender, EventArgs e)
        {
            fCTBCodeClickedLineNow = fCTBCode.Selection.ToLine;
            fCTBCodeMarkLine();
            //           MessageBox.Show(visuGCode.getLineInfo(fCTBCodeClickedLineNow));
            //            fCTBCode.t  (visuGCode.getLineInfo(fCTBCodeClickedLineNow));
        }
        private void fCTBCode_KeyDown(object sender, KeyEventArgs e)
        {
            int key = e.KeyValue;
            if ((key == 38) && (fCTBCodeClickedLineNow > 0))
            {
                fCTBCodeClickedLineNow -= 1;
                fCTBCodeMarkLine();
            }
            if ((key == 40) && (fCTBCodeClickedLineNow < (fCTBCode.Lines.Count - 1)))
            {
                fCTBCodeClickedLineNow += 1;
                fCTBCodeMarkLine();
            }
        }
        private void fCTBCodeMarkLine()
        {
#if (debuginfo)
            log.Add("MainFormFCTB fCTBCodeMarkLine() now: "+ fCTBCodeClickedLineNow.ToString() + " last: "+ fCTBCodeClickedLineLast.ToString());
#endif
            if ((fCTBCodeClickedLineNow <= fCTBCode.LinesCount) && (fCTBCodeClickedLineNow >= 0))
            {
                if (fCTBCodeClickedLineNow != fCTBCodeClickedLineLast)
                {
                    fCTBCode.UnbookmarkLine(fCTBCodeClickedLineLast);
                    fCTBCode.BookmarkLine(fCTBCodeClickedLineNow);
                    fCTBCodeClickedLineLast = fCTBCodeClickedLineNow;
                    // Set marker in drawing
                    visuGCode.setPosMarkerLine(fCTBCodeClickedLineNow,!isStreaming);
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
                fCTBCode.Paste();
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

        private void showMessageForm(string text)
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
            _message_form.showMessage(text);
            _message_form.Show(this);
        }
        private void formClosed_MessageForm(object sender, FormClosedEventArgs e)
        { _message_form = null;  }


        #endregion

        private void moveToFirstPosToolStripMenuItem_Click(object sender, EventArgs e)
        {
            int start = visuGCode.getLineOfFirstPointInFigure();
            int end = visuGCode.getLineOfEndPointInFigure(start);
            if ((start >=0) && (end > start))
            {
                Place selStart, selEnd;
                selStart.iLine = start;
                selStart.iChar = 0;
                Range mySelection = fCTBCode.Range;
                mySelection.Start = selStart;
                selEnd.iLine = end;
                selEnd.iChar = 0;
                mySelection.End = selEnd;
                fCTBCode.Selection = mySelection;
                fCTBCode.SelectionColor = Color.Red;
            }
            fCTBCodeClickedLineNow = start;
            fCTBCodeMarkLine();
            fCTBCode.DoCaretVisible();
            return;
        }

        private void deletePathToolStripMenuItem_Click(object sender, EventArgs e)
        {   int start = visuGCode.getLineOfFirstPointInFigure();
            int end   = visuGCode.getLineOfEndPointInFigure(start);
            if ((start < 0) || (end <= start))
                return;
            Place selStart, selEnd;
            selStart.iLine = start;
            selStart.iChar = 0;
            Range mySelection = fCTBCode.Range;
            mySelection.Start = selStart;
            selEnd.iLine = end;
            selEnd.iChar = 0;
            mySelection.End = selEnd;
            fCTBCode.Selection = mySelection;
            fCTBCode.SelectionColor = Color.Red;
            fCTBCodeClickedLineNow = start;
            fCTBCode.InsertText("( Figure removed )\r\n");
            fCTBCodeMarkLine();
            fCTBCode.DoCaretVisible();
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

    }
}
