// --------------------------------------------------------------------------
// <copyright file="SheetExtensions.cs" company="Nexcel Solutions Vietnam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------
// <history>
//   <change who="Jason.Ly" date="2014.6.11">Created</change>
// </history>

namespace Fanex.NAP.TGRS.Core.ExcelUtility.NPOIExtensions
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using NPOI.SS.UserModel;
    using NPOI.SS.Util;

    /// <summary>
    /// Sheet Extensions.
    /// </summary>
    public static class SheetExtensions
    {
        /// <summary>
        /// Copies a number of rows from another sheet.
        /// </summary>
        /// <param name="sheet">Current sheet.</param>
        /// <param name="sourceSheet">Source sheet.</param>
        /// <param name="sourceRowIndex">Start row index of the source sheet.</param>
        /// <param name="rowIndex">Start row index of the current sheet.</param>
        /// <param name="total">Number of rows that will be copied.</param>
        public static void CopyRows(this ISheet sheet, ISheet sourceSheet, int sourceRowIndex, int rowIndex, int total)
        {
            for (int i = 0; i < total; i++)
            {
                sheet.CopyRow(sourceSheet, sourceRowIndex + i, rowIndex + i);
            }
        }

        /// <summary>
        /// Copies a row from the source to the current sheet.
        /// </summary>
        /// <param name="sheet">Current sheet.</param>
        /// <param name="sourceSheet">Source sheet.</param>
        /// <param name="sourceRowIndex">The index of source row.</param>
        /// <param name="rowIndex">The index of the row where data will be copied to.</param>
        /// <param name="firstColumnIndex">First column index of copying area.</param>
        /// <param name="lastColumnIndex">Last column index of copying area.</param>
        [SuppressMessage("StyleCop.CSharp.StylecopPlus.Rules", "SP2101:MethodMustNotContainMoreLinesThan", Justification = "Need to review here.")]
        public static void CopyRow(this ISheet sheet, ISheet sourceSheet, int sourceRowIndex, int rowIndex, int firstColumnIndex, int lastColumnIndex)
        {
            // Expand the sheet to satisfy the number of needed rows.
            for (int i = sheet.LastRowNum + 1; i <= rowIndex; i++)
            {
                sheet.CreateRow(i);
            }

            // Get source row that will be copied.
            IRow sourceRow = sourceSheet.GetRow(sourceRowIndex);
            if (sourceRow != null)
            {
                // Get the row at a specific index of the current sheet.
                IRow newRow = sheet.GetRow(rowIndex);
                newRow = newRow ?? sheet.CreateRow(rowIndex);
                newRow.Height = sourceRow.Height;

                // Get the first and last available columns of the copying area.
                int firstCellNum = Math.Min(sourceRow.FirstCellNum, firstColumnIndex);
                int lastCellNum = Math.Min(sourceRow.LastCellNum, lastColumnIndex + 1);

                // Loop through each cell of the source row to copy its data to the new row.
                for (int i = firstCellNum; i < lastCellNum; i++)
                {
                    // Get the source cell.
                    ICell sourceCell = sourceRow.GetCell(i);

                    // If the source cell is null then jump to next cell
                    if (sourceCell == null)
                    {
                        continue;
                    }

                    // Create a new cell at current sheet, and clone the source cell data and style to it.
                    ICell newCell = newRow.CreateCell(i);
                    CloneCell(newCell, sourceCell);

                    // Copy merged area.
                    CellRangeAddress sourceRange = null;
                    for (int j = 0; j < sourceSheet.NumMergedRegions; j++)
                    {
                        CellRangeAddress range = sourceSheet.GetMergedRegion(j);
                        if (range.FirstColumn == sourceCell.ColumnIndex && range.FirstRow == sourceCell.RowIndex)
                        {
                            sourceRange = range;
                            break;
                        }
                    }

                    if (sourceRange != null)
                    {
                        var newRange = new CellRangeAddress(
                            newCell.RowIndex,
                            newCell.RowIndex + sourceRange.LastRow - sourceRange.FirstRow,
                            newCell.ColumnIndex,
                            newCell.ColumnIndex + sourceRange.LastColumn - sourceRange.FirstColumn);
                        sheet.AddMergedRegion(newRange);
                    }
                }
            }
        }

        /// <summary>
        /// Copies a row from the source to the current sheet.
        /// </summary>
        /// <param name="sheet">Current sheet.</param>
        /// <param name="sourceSheet">Source sheet.</param>
        /// <param name="sourceRowIndex">The index of source row.</param>
        /// <param name="rowIndex">The index of the row where data will be copied to.</param>
        public static void CopyRow(this ISheet sheet, ISheet sourceSheet, int sourceRowIndex, int rowIndex)
        {
            sheet.CopyRow(sourceSheet, sourceRowIndex, rowIndex, 0, int.MaxValue - 1);
        }

        /// <summary>
        /// Removes a row at a specific index.
        /// </summary>
        /// <param name="sheet">Current sheet.</param>
        /// <param name="rowIndex">Index of row that needs to be removed.</param>
        public static void RemoveRow(this ISheet sheet, int rowIndex)
        {
            sheet.RemoveRow(sheet.GetRow(rowIndex));
        }

        /// <summary>
        /// Removes a list of columns from the sheet.
        /// </summary>
        /// <param name="sheet">Current sheet.</param>
        /// <param name="columns">List of column indexes.</param>
        public static void RemoveColumns(this ISheet sheet, List<int> columns)
        {
            columns.Sort();
            for (int i = columns.Count - 1; i >= 0; i--)
            {
                RemoveColumn(sheet, columns[i]);
            }
        }

        /// <summary>
        /// Removes a column from the sheet at a specific index.
        /// </summary>
        /// <param name="sheet">Current sheet.</param>
        /// <param name="columnIndex">Index of column that needs to be removed.</param>
        [SuppressMessage("StyleCop.CSharp.StylecopPlus.Rules", "SP2101:MethodMustNotContainMoreLinesThan", Justification = "Need to review here.")]
        public static void RemoveColumn(this ISheet sheet, int columnIndex)
        {
            bool adjustColumn = true;
            for (int rowIndex = sheet.FirstRowNum; rowIndex <= sheet.LastRowNum; rowIndex++)
            {
                IRow row = sheet.GetRow(rowIndex);

                // If no row exists here then skip to next row.
                if (row != null)
                {
                    // Get the last available column of current row.
                    int lastColumn = row.LastCellNum;

                    if (lastColumn >= columnIndex)
                    {
                        for (int currentCol = columnIndex; currentCol < lastColumn; currentCol++)
                        {
                            // Remove old cell.
                            ICell oldCell = row.GetCell(currentCol);
                            if (oldCell != null)
                            {
                                row.RemoveCell(oldCell);
                            }

                            // Move cell.
                            ICell nextCell = row.GetCell(currentCol + 1);
                            if (nextCell != null)
                            {
                                ICell newCell = row.CreateCell(currentCol, nextCell.CellType);
                                if (adjustColumn)
                                {
                                    sheet.SetColumnWidth(newCell.ColumnIndex, sheet.GetColumnWidth(nextCell.ColumnIndex));
                                }

                                CloneCell(newCell, nextCell);
                            }
                        }

                        // Remove last cell.
                        ICell removeCell = row.GetCell(lastColumn);
                        if (removeCell != null)
                        {
                            row.RemoveCell(removeCell);
                        }
                    }

                    // Adjust merged region
                    for (int i = sheet.NumMergedRegions - 1; i >= 0; i--)
                    {
                        CellRangeAddress mergedRegion = sheet.GetMergedRegion(i);

                        if (rowIndex == mergedRegion.FirstRow)
                        {
                            if ((columnIndex == mergedRegion.FirstColumn)
                                && (columnIndex == mergedRegion.LastColumn))
                            {
                                sheet.RemoveMergedRegion(i);
                            }
                            else if (mergedRegion.FirstColumn > columnIndex)
                            {
                                mergedRegion.FirstColumn = mergedRegion.FirstColumn - 1;
                                mergedRegion.LastColumn = mergedRegion.LastColumn - 1;
                            }
                            else if (columnIndex <= mergedRegion.LastColumn)
                            {
                                mergedRegion.LastColumn = mergedRegion.LastColumn - 1;
                            }
                        }
                    }

                    adjustColumn = false;
                }
            }
        }

        /// <summary>
        /// Clones a cell.
        /// </summary>
        /// <param name="sheet">Current sheet.</param>
        /// <param name="newCell">New cell instance.</param>
        /// <param name="sourceCell">Source cell instance.</param>
        [SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", Justification = "Need to review here.")]
        public static void CloneCell(this ISheet sheet, ICell newCell, ICell sourceCell)
        {
            CloneCell(newCell, sourceCell);
        }

        /// <summary>
        /// Gets a cell at specific row and column.
        /// </summary>
        /// <param name="sheet">Current sheet.</param>
        /// <param name="rowIndex">Row index.</param>
        /// <param name="colIndex">Column index.</param>
        /// <returns>Cell instance at the passed position.</returns>
        public static ICell GetCell(this ISheet sheet, int rowIndex, int colIndex)
        {
            return sheet.GetRow(rowIndex).GetCell(colIndex);
        }

        /// <summary>
        /// Merges cells into a cell range.
        /// </summary>
        /// <param name="sheet">Current sheet.</param>
        /// <param name="firstRow">First row index of region needs to be merged.</param>
        /// <param name="firstCol">First column index of region needs to be merged.</param>
        /// <param name="lastRow">Last row index of region needs to be merged.</param>
        /// <param name="lastCol">Last column index of region needs to be merged.</param>
        /// <returns>Index of merged cells.</returns>
        public static int MergeCells(this ISheet sheet, int firstRow, int firstCol, int lastRow, int lastCol)
        {
            CellRangeAddress rangeAddress = new CellRangeAddress(firstRow, lastRow, firstCol, lastCol);
            return sheet.AddMergedRegion(rangeAddress);
        }

        /// <summary>
        /// Removes merged regions from the sheet.
        /// </summary>
        /// <param name="sheet">Current sheet.</param>
        /// <param name="firstRow">First row index of region needs to be removed.</param>
        /// <param name="firstCol">First column index of region needs to be removed.</param>
        /// <param name="lastRow">Last row index of region needs to be removed.</param>
        /// <param name="lastCol">Last column index of region needs to be removed.</param>
        public static void RemoveMergedRegions(this ISheet sheet, int firstRow, int firstCol, int lastRow, int lastCol)
        {
            for (int i = sheet.NumMergedRegions - 1; i >= 0; i--)
            {
                CellRangeAddress mergedRegion = sheet.GetMergedRegion(i);
                if ((mergedRegion.FirstRow >= firstRow)
                    && (mergedRegion.LastRow <= lastRow)
                    && (mergedRegion.FirstColumn >= firstCol)
                    && (mergedRegion.LastColumn <= lastCol))
                {
                    sheet.RemoveMergedRegion(i);
                }
            }
        }

        /// <summary>
        /// Creates a merged cell.
        /// </summary>
        /// <param name="sheet">Current sheet.</param>
        /// <param name="rowIndex">Row index.</param>
        /// <param name="colIndex">Column index.</param>
        /// <param name="rowSpan">Number of row span.</param>
        /// <param name="colSpan">Number of column span.</param>
        /// <returns>New cell instance.</returns>
        public static ICell CreateCell(this ISheet sheet, int rowIndex, int colIndex, int rowSpan, int colSpan)
        {
            ICell cell = sheet.CreateCell(rowIndex, colIndex);

            if ((rowSpan > 0) || (colSpan > 0))
            {
                MergeCells(sheet, rowIndex, colIndex, rowIndex + rowSpan, colIndex + colSpan);
            }

            return cell;
        }

        /// <summary>
        /// Creates a merged cell.
        /// </summary>
        /// <param name="sheet">Current sheet.</param>
        /// <param name="rowIndex">Row index.</param>
        /// <param name="colIndex">Column index.</param>
        /// <param name="rowSpan">Number of row span.</param>
        /// <param name="colSpan">Number of column span.</param>
        /// <param name="cellStyle">Cell style.</param>
        /// <returns>New cell instance.</returns>
        public static ICell CreateCell(this ISheet sheet, int rowIndex, int colIndex, int rowSpan, int colSpan, ICellStyle cellStyle)
        {
            ICell cell = sheet.CreateCell(rowIndex, colIndex, rowSpan, colSpan);
            cell.CellStyle = cellStyle;
            return cell;
        }

        /// <summary>
        /// Creates new cell at specific row and column index.
        /// </summary>
        /// <param name="sheet">Current sheet.</param>
        /// <param name="rowIndex">Row index.</param>
        /// <param name="colIndex">Column index.</param>
        /// <returns>New cell instance.</returns>
        public static ICell CreateCell(this ISheet sheet, int rowIndex, int colIndex)
        {
            IRow row = sheet.GetRow(rowIndex);
            row = row ?? sheet.CreateRow(rowIndex);

            ICell cell = row.GetCell(colIndex);
            cell = cell ?? row.CreateCell(colIndex);

            return cell;
        }

        /// <summary>
        /// Clones value and style of a cell.
        /// </summary>
        /// <param name="newCell">New cell instance.</param>
        /// <param name="sourceCell">Source cell instance.</param>
        [SuppressMessage("StyleCop.CSharp.StylecopPlus.Rules", "SP2101:MethodMustNotContainMoreLinesThan", Justification = "Need to review here.")]
        private static void CloneCell(ICell newCell, ICell sourceCell)
        {
            if (sourceCell != null)
            {
                if (sourceCell.CellComment != null)
                {
                    newCell.CellComment = sourceCell.CellComment;
                }

                if (sourceCell.Hyperlink != null)
                {
                    newCell.Hyperlink = sourceCell.Hyperlink;
                }

                if (sourceCell.CellStyle != null)
                {
                    newCell.CellStyle = sourceCell.CellStyle;
                }

                newCell.SetCellType(sourceCell.CellType);

                switch (sourceCell.CellType)
                {
                    case CellType.Boolean:
                        {
                            newCell.SetCellValue(sourceCell.BooleanCellValue);
                            break;
                        }

                    case CellType.Numeric:
                        {
                            newCell.SetCellValue(sourceCell.NumericCellValue);
                            break;
                        }

                    case CellType.String:
                        {
                            newCell.SetCellValue(sourceCell.StringCellValue);
                            break;
                        }

                    case CellType.Error:
                        {
                            newCell.SetCellValue(sourceCell.ErrorCellValue);
                            break;
                        }

                    case CellType.Formula:
                        {
                            newCell.CellFormula = sourceCell.CellFormula;
                            break;
                        }
                }
            }
        }
    }
}