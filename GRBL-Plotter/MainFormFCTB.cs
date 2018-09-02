/* MainFormFCTB
 * FCTB (fast colored text box) related methods
 * fCTBCode_Click
 * fCTBCode_KeyDown
 * fCTBCode_TextChanged
 * fCTBCode_TextChangedDelayed
 * */

using System;
using System.Drawing;
using System.Windows.Forms;
using FastColoredTextBoxNS;

namespace GRBL_Plotter
{
    public partial class MainForm
    {
        private bool commentOut;        // comment out unknown GCode to avoid errors from GRBL

        #region fCTB FastColoredTextBox related
        // highlight code in editor
        Style StyleComment = new TextStyle(Brushes.Gray, null, FontStyle.Italic);
        Style StyleGWord = new TextStyle(Brushes.Blue, null, FontStyle.Bold);
        Style StyleMWord = new TextStyle(Brushes.SaddleBrown, null, FontStyle.Regular);
        Style StyleFWord = new TextStyle(Brushes.Red, null, FontStyle.Regular);
        Style StyleSWord = new TextStyle(Brushes.OrangeRed, null, FontStyle.Regular);
        Style StyleTool = new TextStyle(Brushes.Black, null, FontStyle.Regular);
        Style StyleXAxis = new TextStyle(Brushes.Green, null, FontStyle.Bold);
        Style StyleYAxis = new TextStyle(Brushes.BlueViolet, null, FontStyle.Bold);
        Style StyleZAxis = new TextStyle(Brushes.Red, null, FontStyle.Bold);

        private void fCTBCode_TextChanged(object sender, FastColoredTextBoxNS.TextChangedEventArgs e)
        {
            e.ChangedRange.ClearStyle(StyleComment);
            e.ChangedRange.SetStyle(StyleComment, "(\\(.*\\))", System.Text.RegularExpressions.RegexOptions.Compiled);
            e.ChangedRange.SetStyle(StyleGWord, "(G\\d{1,2})", System.Text.RegularExpressions.RegexOptions.Compiled);
            e.ChangedRange.SetStyle(StyleMWord, "(M\\d{1,2})", System.Text.RegularExpressions.RegexOptions.Compiled);
            e.ChangedRange.SetStyle(StyleFWord, "(F\\d+)", System.Text.RegularExpressions.RegexOptions.Compiled);
            e.ChangedRange.SetStyle(StyleSWord, "(S\\d+)", System.Text.RegularExpressions.RegexOptions.Compiled);
            e.ChangedRange.SetStyle(StyleTool, "(T\\d{1,2})", System.Text.RegularExpressions.RegexOptions.Compiled);
            e.ChangedRange.SetStyle(StyleXAxis, "[XIxi]{1}-?\\d+(.\\d+)?", System.Text.RegularExpressions.RegexOptions.Compiled);
            e.ChangedRange.SetStyle(StyleYAxis, "[YJyj]{1}-?\\d+(.\\d+)?", System.Text.RegularExpressions.RegexOptions.Compiled);
            e.ChangedRange.SetStyle(StyleZAxis, "[Zz]{1}-?\\d+(.\\d+)?", System.Text.RegularExpressions.RegexOptions.Compiled);
        }

        //bool showChangedMessage = true;     // show Message if TextChanged
        private void fCTBCode_TextChangedDelayed(object sender, TextChangedEventArgs e)
        {   //showChangedMessage = true;
            if (fCTBCode.LinesCount > 2)            // only redraw if GCode is available, otherwise startup picture disappears
            {   if (commentOut)
                {   fCTB_CheckUnknownCode(); }
                pictureBox1.BackgroundImage = null;
                if (!blockFCTB_Events)
                {   redrawGCodePath();
     //               MessageBox.Show("textchangedelayed");
                }
                blockFCTB_Events = false;
            }
        }

        private void fCTB_CheckUnknownCode()
        {   string curLine;
            string allowed = "NGMFIJKLNPRSTUVWXYZOPLngmfijklnprstuvwxyzopl ";
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
        int fCTBCodeClickedLineNow = 0;
        int fCTBCodeClickedLineLast = 0;
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
                fCTBCode.Selection = fCTBCode.GetLine(fCTBCodeClickedLineNow);
                fCTBCodeMarkLine();
            }
            if ((key == 40) && (fCTBCodeClickedLineNow < (fCTBCode.Lines.Count - 1)))
            {
                fCTBCodeClickedLineNow += 1;
                fCTBCode.Selection = fCTBCode.GetLine(fCTBCodeClickedLineNow);
                fCTBCodeMarkLine();
            }
        }
        private void fCTBCodeMarkLine()
        {
            if ((fCTBCodeClickedLineNow <= fCTBCode.LinesCount) && (fCTBCodeClickedLineNow >= 0))
            {
                if (fCTBCodeClickedLineNow != fCTBCodeClickedLineLast)
                {
                    fCTBCode.UnbookmarkLine(fCTBCodeClickedLineLast);
                    fCTBCode.BookmarkLine(fCTBCodeClickedLineNow);
                    Range selected = fCTBCode.GetLine(fCTBCodeClickedLineNow);
                    fCTBCode.Selection = selected;
                    fCTBCode.SelectionColor = Color.Orange;
                    fCTBCodeClickedLineLast = fCTBCodeClickedLineNow;
                    // Set marker in drawing
                    //visuGCode.setMarkerOnDrawing(fCTBCode.SelectedText);
                    visuGCode.setPosMarkerLine(fCTBCodeClickedLineNow);
                    pictureBox1.Invalidate(); // avoid too much events
                    if (_camera_form != null)
                    { _camera_form.setPosMarker(visuGCode.GetPosMarker());// X(), visuGCode.GetPosMarkerY());
                        //MessageBox.Show("x "+visuGCode.GetPosMarkerX().ToString()+ "  y "+ visuGCode.GetPosMarkerY().ToString());
                    }
                }
            }
        }

        // context Menu on fastColoredTextBox
        private void cmsCode_ItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {
            if (e.ClickedItem.Name == "cmsCodeSelect")
            {
                fCTBCode.SelectAll();
            }
            if (e.ClickedItem.Name == "cmsCodeCopy")
            {
                if (fCTBCode.SelectedText.Length > 0)
                    fCTBCode.Copy();
            }
            if (e.ClickedItem.Name == "cmsCodePaste")
            {
                fCTBCode.Paste();
            }
            if (e.ClickedItem.Name == "cmsCodeSendLine")
            {
                int clickedLine = fCTBCode.Selection.ToLine;
                sendCommand(fCTBCode.Lines[clickedLine], false);
                //MessageBox.Show(fCTBCode.Lines[clickedLine]);
            }
            if (e.ClickedItem.Name == "cmsCommentOut")
            {
                fCTB_CheckUnknownCode();
            }
        }

        #endregion
                
        private int findEndOfPath(int startLine, bool toEnd)
        {
            int endVal = fCTBCode.LinesCount;
            int lineNr = startLine;
            int lastNr = lineNr;
            string curLine;
            if (endVal < 2) return -1;
            if (toEnd)
            { if (startLine > endVal) return -1; }
            else
            {
                endVal = 0;
                if (startLine < endVal) return -1;
            }
            do
            {
                curLine = fCTBCode.Lines[lineNr];
                if ((curLine.IndexOf("X") >= 0) || (curLine.IndexOf("Y") >= 0))
                { lastNr = lineNr; }
                if ((curLine.IndexOf("Z") >= 0) || (curLine.IndexOf("G0") >= 0) || (curLine.IndexOf("M30") >= 0) || (curLine.IndexOf("F") >= 0))
                {
                    if (toEnd)
                        lastNr++;
                    return lastNr;
                }
                if (toEnd)
                { lineNr++; }
                else
                { lineNr--; }
            } while ((lineNr <= fCTBCode.LinesCount) || (lineNr > 0));
            return -1;
        }

        private void moveToFirstPosToolStripMenuItem_Click(object sender, EventArgs e)
        {   // rotate coordinates until marked line first
            int lineNr = fCTBCodeClickedLineNow;
            Range mySelection = fCTBCode.Range;
            Place selStart, selEnd;
            selStart.iLine = fCTBCodeClickedLineNow;
            selStart.iChar = 0;
            mySelection.Start = selStart;
            selEnd.iLine = lineNr;
            selEnd.iChar = 0;
            // select from marked line until end of this path - needs to be moved to front
            lineNr = findEndOfPath(fCTBCodeClickedLineNow, true);            // find end
            if (lineNr > 0)
            {
                selEnd.iLine = lineNr;
                selEnd.iChar = 0;
                mySelection.End = selEnd;
                fCTBCode.Selection = mySelection;
                fCTBCode.SelectionColor = Color.Red;
                // find current begin of path, to insert selected code
                lineNr = findEndOfPath(fCTBCodeClickedLineNow, false);      // find start
                if (lineNr > 0)
                {
                    if (deleteMarkedCode)
                    {
                        fCTBCode.Cut();
                        selStart.iLine = lineNr;
                        selStart.iChar = 0;
                        selEnd.iLine = lineNr;
                        selEnd.iChar = 0;
                        mySelection.Start = selStart;
                        mySelection.End = selEnd;
                        fCTBCode.Selection = mySelection;
                        fCTBCode.Paste();
                        fCTBCodeClickedLineNow = lineNr;
                        fCTBCodeMarkLine();
                    }
                    fCTBCode.DoCaretVisible();
                    redrawGCodePath();
                    return;
                }
            }
            MessageBox.Show("Path start / end could not be identified");
        }

        private void deletePathToolStripMenuItem_Click(object sender, EventArgs e)
        {   // mark start to end of path and delete
            int lineNr = fCTBCodeClickedLineNow;
            Range mySelection = fCTBCode.Range;
            Place selStart, selEnd;
            selStart.iLine = fCTBCodeClickedLineNow;
            selStart.iChar = 0;
            mySelection.Start = selStart;
            selEnd.iLine = lineNr;
            selEnd.iChar = 0;
            // find start of path
            lineNr = findEndOfPath(fCTBCodeClickedLineNow, false);
            if (lineNr > 0)
            {
                if (fCTBCode.Lines[lineNr].IndexOf("Z") >= 0) { lineNr--; }
                selStart.iLine = lineNr;
                selStart.iChar = 0;
                mySelection.Start = selStart;
                // find end of path
                lineNr = findEndOfPath(fCTBCodeClickedLineNow, true);
                if (lineNr > 0)
                {
                    if (fCTBCode.Lines[lineNr].IndexOf("Z") >= 0) { lineNr++; }
                    selEnd.iLine = lineNr;
                    selEnd.iChar = 0;
                    mySelection.End = selEnd;
                    fCTBCode.Selection = mySelection;
                    fCTBCode.SelectionColor = Color.Red;
                    fCTBCodeClickedLineNow = selStart.iLine;

                    if (deleteMarkedCode)
                    {
                        fCTBCode.Cut();
                        fCTBCodeMarkLine();
                    }
                    fCTBCode.DoCaretVisible();
                    redrawGCodePath();
                    return;
                }
            }
            MessageBox.Show("Path start / end could not be identified");
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
            redrawGCodePath();
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
