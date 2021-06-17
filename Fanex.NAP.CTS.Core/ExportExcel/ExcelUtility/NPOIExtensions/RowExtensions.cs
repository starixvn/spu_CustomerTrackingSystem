// --------------------------------------------------------------------------
// <copyright file="RowExtensions.cs" company="Nexcel Solutions Vietnam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------
// <history>
//   <change who="Jason.Ly" date="2014.6.11">Created</change>
// </history>

namespace Fanex.NAP.TGRS.Core.ExcelUtility.NPOIExtensions
{
    using NPOI.SS.UserModel;

    /// <summary>
    /// Row Extensions.
    /// </summary>
    public static class RowExtensions
    {
        /// <summary>
        /// Creates cell at a specific column.
        /// </summary>
        /// <param name="row">Current row.</param>
        /// <param name="colIndex">Index of column where the cell is created.</param>
        /// <param name="cellStyle">Style that will apply for the cell.</param>
        /// <returns>New cell instance.</returns>
        public static ICell CreateCell(this IRow row, int colIndex, ICellStyle cellStyle)
        {
            ICell cell = row.CreateCell(colIndex);

            cell.CellStyle = cellStyle;

            return cell;
        }

        /// <summary>
        /// Fill values to row, start at a specific column.
        /// </summary>
        /// <param name="row">Current row.</param>
        /// <param name="colIndex">Start column index.</param>
        /// <param name="values">List of values.</param>
        public static void WriteCells(this IRow row, int colIndex, object[] values)
        {
            int id = colIndex;
            foreach (object value in values)
            {
                row.GetCell(id).SetCellValue(value);
                id++;
            }
        }
    }
}