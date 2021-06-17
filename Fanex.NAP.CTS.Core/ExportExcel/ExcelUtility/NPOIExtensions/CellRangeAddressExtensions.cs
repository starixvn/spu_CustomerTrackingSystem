// --------------------------------------------------------------------------
// <copyright file="CellRangeAddressExtensions.cs" company="Nexcel Solutions Vietnam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------
// <history>
//   <change who="Jason.Ly" date="2014.6.11">Created</change>
// </history>

namespace Fanex.NAP.TGRS.Core.ExcelUtility.NPOIExtensions
{
    using System.Diagnostics.CodeAnalysis;
    using NPOI.SS.Util;

    /// <summary>
    /// Cell Range Address Extensions.
    /// </summary>
    public static class CellRangeAddressExtensions
    {
        /// <summary>
        /// Clones cell range address.
        /// </summary>
        /// <param name="range">Current cell range address.</param>
        /// <returns>New cell range address instance.</returns>
        [SuppressMessage("Microsoft.Design", "CA1011:ConsiderPassingBaseTypesAsParameters", Justification = "Need to review here.")]
        public static CellRangeAddress Clone(this CellRangeAddress range)
        {
            var result = new CellRangeAddress(range.FirstRow, range.LastRow, range.FirstColumn, range.LastColumn);
            return result;
        }
    }
}