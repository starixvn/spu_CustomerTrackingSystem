namespace Fanex.NAP.CTS.Service.Implement.RobotReport
{
    using System;
    using System.Linq;
    using System.Collections.Generic;
    using Fanex.NAP.TGRS.Core.ExcelUtility;
    using Fanex.NAP.CTS.Service.Implement.ExportExcel;
    using Fanex.NAP.CTS.Service.Interface.RobotReport;
    using Fanex.NAP.CTS.Model.RobotReport;
    using Fanex.NAP.CTS.ViewModel.RobotReport;
    using static Fanex.NAP.CTS.Resource.Consts.Consts;

    public class ExportRobotReportService : ExportExcelService, IExportRobotReportService
    {
        private const string _sheetName = "Robot Report";
        private const string _totalDistinctAccount = "Total Distinct Account";

        #region public method
        public byte[] ExportRobotReport(string templatePath, string fileName, RobotReportViewModel data, RobotReportParamModel param)
        {
            _lineCountFilterBlock = 0;
            var blocks = PrepareBlockData(param);
            var rowDatas = PrepareRowData(data, param.IsMonthly);
            var footer = PrepareFooterData(data);

            var writer = OrderActionExport(templatePath, fileName, blocks, rowDatas, footer);
            writer.SyncColumnsWidth((int)ROBOT_REPORT.ROBOT_EXPORT_NUMBER_COLUMN);

            return writer.ToBytes();
        }

        public byte[] ExportAIRobotReport(string templatePath, string fileName, AIRobotReportViewModel data, RobotReportParamModel param)
        {
            _lineCountFilterBlock = 0;
            var blocks = PrepareBlockData(param);
            var rowDatas = PrepareRowData(data, param.IsMonthly);
            var footer = PrepareFooterData(data);

            var writer = OrderActionExport(templatePath, fileName, blocks, rowDatas, footer);
            writer.SyncColumnsWidth((int)ROBOT_REPORT.AIROBOT_EXPORT_NUMBER_COLUMN);

            return writer.ToBytes();
        }
        #endregion

        #region private method
        private List<(string, string)> PrepareBlockData(RobotReportParamModel param)
        {
            var blocks = new List<(string, string)>();
            var site = param.Sites == "A" ? "All" : (param.Sites == "L" ? "Licensee" : "Credit");
            var dateTime = param.IsMonthly ? ("Month", $"{GetMonthlyTime(param.FromDate)} - {GetMonthlyTime(param.ToDate)}") : ("Date", $"{GetDailyTime(param.FromDate)} - {GetDailyTime(param.ToDate)}");

            blocks.Add(("Site", site));
            blocks.Add(dateTime);

            return blocks;
        }

        private IEnumerable<object[]> PrepareRowData(RobotReportViewModel data, bool isMonthly)
        {
            return data.Items.Select(item => new object[]
            {
                isMonthly ? item.GetMonthlyTime : item.GetDailyTime,
                item.AI,
                item.IMPERVA,
                item.COUNTER,
                item.TotalAccount
            });
        }

        private IEnumerable<object[]> PrepareRowData(AIRobotReportViewModel data, bool isMonthly)
        {
            return data.Items.Select(item => new object[]
            {
                isMonthly ? item.GetMonthlyTime : item.GetDailyTime,
                item.TI,
                item.SCE,
                item.TD,
                item.OCRD,
                item.TotalAccount,
            });
        }

        private object[] PrepareFooterData(RobotReportViewModel data)
        {
            return new object[]
            {
                _totalDistinctAccount,
                data.TFoot.AI,
                data.TFoot.IMPERVA,
                data.TFoot.COUNTER,
            };
        }

        private object[] PrepareFooterData(AIRobotReportViewModel data)
        {
            return new object[]
            {
                _totalDistinctAccount,
                data.TFoot.TI,
                data.TFoot.SCE,
                data.TFoot.TD,
                data.TFoot.OCRD,
            };
        }

        private ExcelTemplateWriter OrderActionExport(string templatePath, string fileName, List<(string, string)> blocks, IEnumerable<object[]> rowDatas, object[] footer)
        {
            var writer = new ExcelTemplateWriter(templatePath, fileName, _sheetName);
            writer.CopyRows(0, 1);
            FilterBlock(ref writer, blocks);
            FillHeader(ref writer);
            if (rowDatas != null && rowDatas.Any())
            {
                FillRowData(rowDatas, ref writer);
                FillFooter(footer, ref writer);
            }

            return writer;
        }

        protected override int CountLine(string line)
        {
            int charPerLine = (int)ROBOT_REPORT.EXPORT_NUMBER_CHAR_PER_LINE;
            return Convert.ToInt32(Math.Ceiling((float)line.Length / charPerLine));
        }

        protected override void FilterBlock(ref ExcelTemplateWriter writer, List<(string, string)> items)
        {
            base.FilterBlock(ref writer, items);
            writer.SetRowHeight(1, (int)ROBOT_REPORT.EXPORT_POINT_PIXEL * _lineCountFilterBlock);
        }

        private string GetMonthlyTime(DateTime dateTime) => dateTime.ToString(FormatDateTimeToString_yyyyMM);
        private string GetDailyTime(DateTime dateTime) => dateTime.ToString(FormatDateTimeToString_yyyyMMdd);
        #endregion
    }
}
