// --------------------------------------------------------------------------
// <copyright file="WorkbookExtensions.cs" company="Nexcel Solutions Vietnam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------
// <history>
//   <change who="Jason.Ly" date="2014.6.11">Created</change>
//   <change who="Aaron.Cao" date="2016.1.29">Created</change>
// </history>

namespace Fanex.NAP.TGRS.Core.ExcelUtility.NPOIExtensions
{
    using System.Web;
    using NPOI.SS.UserModel;

    public static class WorkbookExtensions
    {
        public static ISheet CloneSheet(this IWorkbook workbook, string sourceSheetName, string newSheetName)
        {
            var sheetIndex = workbook.GetSheetIndex(sourceSheetName);
            var newSheet = workbook.CloneSheet(sheetIndex);
            var newSheetIndex = workbook.GetSheetIndex(newSheet);
            workbook.SetSheetName(newSheetIndex, newSheetName);
            return newSheet;
        }

        public static void ToReponseStream(this IWorkbook workbook, string fileName, bool isXlsx = false)
        {
            var response = HttpContext.Current.Response;

            response.Clear();
            response.ContentType = "application/vnd.ms-excel";
            response.AddHeader(
                "Content-Disposition", isXlsx ? string.Format("attachment; filename={0}.xlsx", fileName) : string.Format("attachment; filename={0}.xls", fileName));

            workbook.Write(response.OutputStream);
        }

        public static byte[] ToResponseBytes(this IWorkbook workbook)
        {
            System.IO.MemoryStream memoryStream = new System.IO.MemoryStream();

            workbook.Write(memoryStream);

            return memoryStream.ToArray();
        }
    }
}