// --------------------------------------------------------------------------
// <copyright file="CellExtensions.cs" company="Nexcel Solutions Vietnam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------
// <history>
//   <change who="Jason.Ly" date="2014.6.11">Created</change>
// </history>

namespace Fanex.NAP.TGRS.Core.ExcelUtility.NPOIExtensions
{
    using System;
    using System.Diagnostics.CodeAnalysis;
    using NPOI.SS.UserModel;
    using NPOI.SS.Util;

    /// <summary>
    /// Cell Extensions.
    /// </summary>
    public static class CellExtensions
    {
        /// <summary>
        /// Set cell value.
        /// </summary>
        /// <param name="cell">Current cell.</param>
        /// <param name="value">Cell value.</param>
        [SuppressMessage("Microsoft.Performance", "CA1800:DoNotCastUnnecessarily", Justification = "Need to review here.")]
        public static void SetCellValue(this ICell cell, object value)
        {
            if (value == null)
            {
                cell.SetCellValue(string.Empty);
            }
            else
            {
                if (value is string)
                {
                    cell.SetCellValue((string)value);
                }
                else if (value is int)
                {
                    cell.SetCellValue((int)value);
                }
                else if (value is long)
                {
                    cell.SetCellValue((long)value);
                }
                else if (value is double)
                {
                    cell.SetCellValue((double)value);
                }
                else if (value is decimal)
                {
                    cell.SetCellValue((decimal)value);
                }
                else if (value is bool)
                {
                    cell.SetCellValue((bool)value);
                }
                else if (value is System.DateTime)
                {
                    cell.SetCellValue((System.DateTime)value);
                }
                else if (value is IRichTextString)
                {
                    cell.SetCellValue((IRichTextString)value);
                }
                else
                {
                    cell.SetCellValue(string.Empty);
                }
            }
        }

        /// <summary>
        /// Set cell value.
        /// </summary>
        /// <param name="cell">Current cell.</param>
        /// <param name="value">Cell value.</param>
        public static void SetCellValue(this ICell cell, double? value)
        {
            value = value ?? 0;
            cell.SetCellValue((double)value);
        }

        /// <summary>
        /// Set cell value.
        /// </summary>
        /// <param name="cell">Current cell.</param>
        /// <param name="value">Cell value.</param>
        public static void SetCellValue(this ICell cell, decimal value)
        {
            cell.SetCellValue(Convert.ToDouble(value));
        }

        /// <summary>
        /// Set cell value.
        /// </summary>
        /// <param name="cell">Current cell.</param>
        /// <param name="value">Cell value.</param>
        public static void SetCellValue(this ICell cell, decimal? value)
        {
            value = value ?? 0;
            cell.SetCellValue((decimal)value);
        }

        /// <summary>
        /// Set cell value.
        /// </summary>
        /// <param name="cell">Current cell.</param>
        /// <param name="value">Cell value.</param>
        public static void SetCellValue(this ICell cell, int value)
        {
            cell.SetCellValue(Convert.ToDouble(value));
        }

        /// <summary>
        /// Set cell value.
        /// </summary>
        /// <param name="cell">Current cell.</param>
        /// <param name="value">Cell value.</param>
        public static void SetCellValue(this ICell cell, int? value)
        {
            value = value ?? 0;
            cell.SetCellValue((int)value);
        }

        /// <summary>
        /// Set cell value.
        /// </summary>
        /// <param name="cell">Current cell.</param>
        /// <param name="value">Cell value.</param>
        public static void SetCellValue(this ICell cell, long value)
        {
            cell.SetCellValue(Convert.ToDouble(value));
        }

        /// <summary>
        /// Set cell value.
        /// </summary>
        /// <param name="cell">Current cell.</param>
        /// <param name="value">Cell value.</param>
        public static void SetCellValue(this ICell cell, long? value)
        {
            value = value ?? 0;
            cell.SetCellValue((long)value);
        }

        /// <summary>
        /// Copies cell value.
        /// </summary>
        /// <param name="cell">Current cell.</param>
        /// <param name="sourceCell">Source cell.</param>
        public static void CopyFrom(this ICell cell, ICell sourceCell)
        {
            // Copy merged cells.
            CellRangeAddress sourceRange = null;

            for (int i = 0; i < sourceCell.Sheet.NumMergedRegions; i++)
            {
                CellRangeAddress range = sourceCell.Sheet.GetMergedRegion(i);

                if ((range.FirstColumn <= sourceCell.ColumnIndex)
                    && (range.LastColumn >= sourceCell.ColumnIndex)
                    && (range.FirstRow <= sourceCell.RowIndex)
                    && (range.LastRow >= sourceCell.RowIndex))
                {
                    sourceRange = range;
                    break;
                }
            }

            if (sourceRange != null)
            {
                cell.Sheet.AddMergedRegion(sourceRange.Clone());
            }

            // Copy style
            cell.CellStyle = sourceCell.CellStyle;
            
        }
    }
}