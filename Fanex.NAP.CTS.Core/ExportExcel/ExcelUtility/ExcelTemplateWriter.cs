// --------------------------------------------------------------------------
// <copyright file="ExcelTemplateWriter.cs" company="Nexcel Solutions Vietnam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------
// <history>
//   <change who="Jason.Ly" date="2014.6.11">Created</change>
// </history>

namespace Fanex.NAP.TGRS.Core.ExcelUtility
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Reflection;
    using NPOI.HSSF.UserModel;
    using NPOI.SS.UserModel;
    using NPOI.SS.Util;
    using NPOI.XSSF.UserModel;
    using NPOIExtensions;

    /// <summary>
    /// Excel Template Writer.
    /// </summary>
    public class ExcelTemplateWriter
    {
        #region Attributes

        private string _fileName;
        private string _sheetName;
        private ISheet _templateSheet;
        private ISheet _sheet;
        private int _rowId;
        private bool _isXlsx;

        #endregion Attributes

        #region Constructors

        public ExcelTemplateWriter(string filePath, string excelFileName = "Report.xls", string sheetName = "Result")
        {
            var fileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read);
            this._isXlsx = Path.GetExtension(fileStream.Name).Contains("xlsx");

            if (_isXlsx)
            {
                var template = new XSSFWorkbook(fileStream);
                this._fileName = excelFileName;
                this._sheetName = sheetName;
                this._templateSheet = template.GetSheetAt(0);
                this._sheet = template.CreateSheet(this._sheetName);
                this._sheet.Workbook.RemoveSheetAt(0);
            }
            else
            {
                var template = new HSSFWorkbook(fileStream);
                this._fileName = excelFileName;
                this._sheetName = sheetName;
                this._templateSheet = template.GetSheetAt(0);
                this._sheet = template.CreateSheet(this._sheetName);
                this._sheet.Workbook.RemoveSheetAt(0);
            }

            fileStream.Close();
        }

        #endregion Constructors

        #region Properties

        public int RowId
        {
            get
            {
                return this._rowId;
            }
        }

        #endregion Properties

        /// <summary>
        /// Renders Excel file to stream.
        /// </summary>
        public void Render()
        {
            this._sheet.Workbook.ToReponseStream(this._fileName, _isXlsx);
        }

        public byte[] ToBytes()
        {
            return this._sheet.Workbook.ToResponseBytes();
        }

        /// <summary>
        /// Freeze excel's pane by column index and row index.
        /// </summary>
        /// <param name="colSplit">Column index.</param>
        /// <param name="rowSplit">Row index.</param>
        public void FreezeRow(int colSplit, int rowSplit)
        {
            this._sheet.CreateFreezePane(colSplit, rowSplit);
        }

        public void SetColumnHidden(int colIndex)
        {
            this._sheet.SetColumnHidden(colIndex, true);
        }

        /// <summary>
        /// Writes cell range value.
        /// </summary>
        /// <param name="rowIndex">Start row index.</param>
        /// <param name="totalRow">Number of rows that will be spanned.</param>
        /// <param name="colIndex">Start column index.</param>
        /// <param name="totalCol">Number of columns that will be spanned.</param>
        /// <param name="value">Value that will be filled into the cell range.</param>
        public void WriteCellRange(int rowIndex, int totalRow, int colIndex, int totalCol, object value)
        {
            var cellRange = new CellRangeAddress(rowIndex, rowIndex + totalRow - 1, colIndex, colIndex + totalCol - 1);
            this._sheet.AddMergedRegion(cellRange);

            ICell cell = this._sheet.GetCell(rowIndex, colIndex);
            cell.SetCellValue(value);
        }

        #region Auto resize columns' width

        /// <summary>
        /// Resizes columns' width automatically.
        /// </summary>
        /// <param name="from">Start column index.</param>
        /// <param name="to">End column index.</param>
        public void AutosizeColumns(int from, int to)
        {
            for (int i = from; i <= to; i++)
            {
                this._sheet.AutoSizeColumn(i, false);
            }
        }

        /// <summary>
        /// Resizes columns' width automatically.
        /// </summary>
        /// <param name="total">Number of columns that will be resized automatically.</param>
        public void AutosizeColumns(int total)
        {
            this.AutosizeColumns(0, total - 1);
        }

        #endregion Auto resize columns' width

        #region Synchronize columns' width

        /// <summary>
        /// Synchronizes columns' width of the templates with the current sheet.
        /// </summary>
        /// <param name="from">Start column index.</param>
        /// <param name="to">End column index.</param>
        public void SyncColumnsWidth(int from, int to)
        {
            for (int i = from; i <= to; i++)
            {
                this._sheet.SetColumnWidth(i, this._templateSheet.GetColumnWidth(i));
            }
        }

        /// <summary>
        /// Synchronizes row' height of the templates with the current sheet.
        /// </summary>
        /// <param name="index">The row index.</param>
        /// /// <param name="height">Row height.</param>
        public void SetRowHeight(int index, int height)
        {
            this._sheet.GetRow(index).Height = Convert.ToInt16(height);
        }

        /// <summary>
        /// Synchronizes columns' width of the templates with the current sheet.
        /// </summary>
        /// <param name="total">Number of columns that will be synchronized.</param>
        public void SyncColumnsWidth(int total)
        {
            this.SyncColumnsWidth(0, total - 1);
        }

        #endregion Synchronize columns' width

        #region Cell related methods

        /// <summary>
        /// Gets the cell style at a specific position.
        /// </summary>
        /// <param name="rowIndex">Index of the row.</param>
        /// <param name="columnIndex">Index of the column.</param>
        /// <returns>The cell style of cell at the passed position.</returns>
        public ICellStyle GetCellStyle(int rowIndex, int columnIndex)
        {
            ICell cell = this._templateSheet.GetCell(rowIndex, columnIndex);
            ICellStyle cellStyle = null;

            if (cell != null)
            {
                cellStyle = cell.CellStyle;
            }

            return cellStyle;
        }

        /// <summary>
        /// Gets the font at a specific position.
        /// </summary>
        /// <param name="rowIndex">Index of the row.</param>
        /// <param name="columnIndex">Index of the column.</param>
        /// <returns>The font of cell at the passed position.</returns>
        public IFont GetFont(int rowIndex, int columnIndex)
        {
            ICellStyle cellStyle = this.GetCellStyle(rowIndex, columnIndex);
            IFont font = null;

            if (cellStyle != null)
            {
                font = cellStyle.GetFont(this._templateSheet.Workbook);
            }

            return font;
        }

        #endregion Cell related methods

        #region Row related methods

        /// <summary>
        /// Writes values to a row of sheet.
        /// </summary>
        /// <param name="rowIndex">Index of original row that the style will be copied.</param>
        /// <param name="values">Value that will be filled into the row.</param>
        public void WriteRow(int rowIndex, object[] values)
        {
            IRow row = this.CreateRow(rowIndex);
            row.WriteCells(0, values);
        }

        /// <summary>
        /// Writes values to a number of rows of the sheet.
        /// </summary>
        /// <param name="rowIndex">Index of original row that the style will be copied.</param>
        /// <param name="arrayList">List of values which each of its items using to filled into a row.</param>
        public void WriteRows(int rowIndex, List<object[]> arrayList)
        {
            int total = arrayList.Count;
            IRow row = this.CreateRows(rowIndex, total);
            for (int i = 0; i < total; i++)
            {
                row.WriteCells(0, arrayList[i]);
                row = this._sheet.GetRow(row.RowNum + 1);
            }
        }

        /// <summary>
        /// Writes values to a row in the sheet.
        /// </summary>
        /// <param name="rowIndex">Index of original row that the style will be copied.</param>
        /// <param name="value">Value that will be filled into the row.</param>
        public void WriteRow(int rowIndex, object value)
        {
            var list = new List<object>();

            foreach (PropertyInfo propertyInfo in value.GetType().GetProperties())
            {
                list.Add(propertyInfo.GetValue(value, null));
            }

            WriteRow(rowIndex, list.ToArray());
        }

        /// <summary>
        /// Writes number of rows to the sheet.
        /// </summary>
        /// <param name="rowId">Index of original row that the style will be copied.</param>
        /// <param name="recordList">List of values that will be filled into rows.</param>
        public void WriteRows(int rowId, IEnumerable<object> recordList)
        {
            foreach (object record in recordList)
            {
                this.WriteRow(rowId, record);
            }
        }

        /// <summary>
        /// Copies rows.
        /// </summary>
        /// <param name="rowIndex">Index of original row that the style will be copied.</param>
        /// <param name="total">Number of rows that will be created.</param>
        /// <returns>First instance in the list of created rows.</returns>
        public IRow CopyRows(int rowIndex, int total)
        {
            return CreateRows(rowIndex, total);
        }

        /// <summary>
        /// Creates a row.
        /// </summary>
        /// <param name="rowId">Index of original row that the style will be copied.</param>
        /// <returns>Row instance.</returns>
        private IRow CreateRow(int rowId)
        {
            return CreateRows(rowId, 1);
        }

        /// <summary>
        /// Creates number of rows.
        /// </summary>
        /// <param name="rowIndex">Index of original row that the style will be copied.</param>
        /// <param name="total">Number of rows that will be created.</param>
        /// <returns>First instance in the list of created rows.</returns>
        private IRow CreateRows(int rowIndex, int total)
        {
            this._sheet.CopyRows(this._templateSheet, rowIndex, this._rowId, total);
            this._rowId += total;
            return this._sheet.GetRow(this._rowId - total);
        }

        public IRow CreateRow(int rowIndex, object[] values, int cellCopyIndex)
        {
            IRow row = this.CreateRow(rowIndex);

            return SetRowValue(row, values, cellCopyIndex);
        }

        public IRow SetRowValue(IRow row, object[] values, int cellCopyIndex)
        {
            var cellCopy = row.GetCell(cellCopyIndex);

            for (int i = 0; i < values.Length; i++)
            {
                var cell = row.GetCell(i);

                if (cell == null)
                {
                    cell = row.CreateCell(i);
                }

                if (i > cellCopyIndex)
                {
                    CellExtensions.CopyFrom(cell, cellCopy);
                }

                cell.SetCellValue(values[i]);
            }

            return row;
        }

        public void SetCellStyle(IRow row, ICellStyle cellStyle, int cellStartIndex, int cellEndIndex)
        {
            for (int j = cellStartIndex; j < cellEndIndex; j++)
            {
                var cell = row.GetCell(j);
                if (cell != null)
                {
                    cell.CellStyle = cellStyle;
                }
            }
        }

        #endregion Row related methods
    }
}