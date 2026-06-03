/*  GRBL-Plotter. Another GCode sender for GRBL.
    This file is part of the GRBL-Plotter application.

    Copyright (C) 2015-2026 Sven Hasemann contact: svenhb@web.de

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
 * 2026-06-02 new helper to read .xlsx spreadsheets for Process Automation bulk jobs (uses ExcelDataReader)
 * 2026-06-02 keep multi-line cells (e.g. addresses) on one data line via "\n" placeholder
*/

using ExcelDataReader;
using NLog;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;

namespace GrblPlotter
{
    /// <summary>
    /// Reads the first worksheet of an .xlsx file and returns its rows as delimiter-joined
    /// strings - the same line format the Process Automation data list (TbData) already consumes,
    /// so column selection, GetDataText() and template substitution keep working unchanged.
    /// </summary>
    public static class XlsxReader
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        /// <summary>
        /// Read the first sheet of an .xlsx workbook into delimiter-joined text lines.
        /// Row 1 is returned via <paramref name="header"/>; every data row is padded to the
        /// header's column count so column positions stay stable even with trailing empty cells.
        /// </summary>
        /// <param name="path">Full path to the .xlsx file.</param>
        /// <param name="delimiter">Delimiter used to join the cells of a row.</param>
        /// <param name="header">Receives the first row (column titles). Never null.</param>
        /// <returns>One joined string per row (including the header row as element 0).</returns>
        public static string[] ReadFirstSheet(string path, char delimiter, out string[] header)
        {
            header = new string[0];
            List<string> lines = new List<string>();

            using (FileStream stream = File.Open(path, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
            using (IExcelDataReader reader = ExcelReaderFactory.CreateReader(stream))
            {
                // The first result set is the first worksheet.
                bool isHeader = true;
                int columnCount = 0;
                while (reader.Read())
                {
                    if (isHeader)
                    {
                        columnCount = reader.FieldCount;
                        header = ReadRow(reader, columnCount);
                        lines.Add(string.Join(delimiter.ToString(), header));
                        isHeader = false;
                    }
                    else
                    {
                        string[] cells = ReadRow(reader, columnCount);
                        lines.Add(string.Join(delimiter.ToString(), cells));
                    }
                }
            }

            Logger.Info("ReadFirstSheet '{0}' rows:{1} columns:{2}", path, lines.Count, header.Length);
            return lines.ToArray();
        }

        private static string[] ReadRow(IExcelDataReader reader, int columnCount)
        {
            if (columnCount < reader.FieldCount)
                columnCount = reader.FieldCount;
            string[] cells = new string[columnCount];
            for (int i = 0; i < columnCount; i++)
            {
                object value = (i < reader.FieldCount) ? reader.GetValue(i) : null;
                cells[i] = CellToString(value);
            }
            return cells;
        }

        // A cell line break is stored as the literal two chars "\n" so a multi-line cell (e.g. a
        // mailing address) stays on ONE data-list line. It is decoded back to a real line break
        // when the row is turned into text (see ProcessAutomation.ResolveTemplate).
        public const string NewlinePlaceholder = "\\n";

        private static string CellToString(object value)
        {
            string s;
            if (value == null)
                s = "";
            else if (value is double dbl)                           // numbers come back as double
                s = dbl.ToString(CultureInfo.InvariantCulture);
            else if (value is DateTime dt)
                s = dt.ToString(CultureInfo.InvariantCulture);
            else
                s = Convert.ToString(value, CultureInfo.InvariantCulture);

            // Collapse embedded line breaks so each spreadsheet row maps to exactly one data-list line.
            return s.Replace("\r\n", NewlinePlaceholder).Replace("\r", NewlinePlaceholder).Replace("\n", NewlinePlaceholder);
        }
    }
}
