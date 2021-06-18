namespace Fanex.NAP.CTS.Service.Implement
{
    using Fanex.Data;
    using Fanex.NAP.CTS.Core.ExportExcel;
    using Fanex.NAP.CTS.Core.Helpers.Api;
    using Fanex.NAP.CTS.Criteria.ProblemCaseMonitor;
    using Fanex.NAP.CTS.Model.ProblemCaseMonitor;
    using Fanex.NAP.CTS.Service.Interface;
    using Fanex.NAP.TGRS.Core.ExcelUtility;
    using Fanex.NAP.TGRS.Core.ExcelUtility.RichTextExtensions;
    using NPOI.SS.UserModel;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    public class ProblemCaseMonitorService : IProblemCaseMonitorService
    {
        public IEnumerable<ProblemCasesFilterModel> GetProblemCasesFilter(int listId, bool status)
        {
            var criteria = new ProblemCaseFilterGetCriteria()
            {
                ListId = listId,
                Status = status
            };

            using (var db = ObjectDbFactory.CreateInstance(criteria.GetSettingKey()))
            {
                return db.Query<ProblemCasesFilterModel>(criteria).ToList();
            }
        }

        public IEnumerable<ProblemCasesMonitorModel> GetProblemCaseMonitor(string apiUrl, ProblemCasesMonitorFilterModel para)
        {
            var param = new
            {
                FromDate = para.FromDate,
                ToDate = para.ToDate,
                SubscriberIDList = para.SubscriberIds,
                ProblemCaseIDList = para.ProblemCaseIds,
            };

            var data = ApiHelper.Post<IEnumerable<ProblemCasesMonitorModel>>(apiUrl, param);

            Task.WhenAll(data);

            return data.Result ?? new List<ProblemCasesMonitorModel>();
        }

        public byte[] ExportProblemCaseMonitor(List<ProblemCasesMonitorModel> data, string fileName, string templatePath, string sheetName, ProblemCasesMonitorFilterModel filter)
        {
            var writer = new ExcelTemplateWriter(templatePath, fileName, sheetName);
            writer.CopyRows(0, 1);
            FilterBlock(data, ref writer, filter);
            //FillRowData(data, ref writer);
            writer.SyncColumnsWidth(6);
            writer.ToBytes();

            return writer.ToBytes();
        }

        private void FilterBlock(List<ProblemCasesMonitorModel> data, ref ExcelTemplateWriter writer, ProblemCasesMonitorFilterModel filter)
        {
            RichTextRender contentRender = new RichTextRender().Clear();
            IFont boldFont = writer.GetFont(10, 0);
            IFont normalFont = writer.GetFont(11, 0);
            int lineCount = 0;

            AppendBlockContent(ref contentRender, ref lineCount, "Subscriber", boldFont, filter.SubscriberIds, normalFont, "\n");
            AppendBlockContent(ref contentRender, ref lineCount, "Problem Case", boldFont, filter.ProblemCaseIds, normalFont, "\n");

            IRichTextString contentRichTextString = contentRender.Render();
            IRichTextString[] contentRichTextArray = new IRichTextString[1];
            contentRichTextArray[0] = contentRichTextString;
            writer.WriteRow(3, contentRichTextArray);
            writer.SetRowHeight(1, 225 * lineCount); //PointInPixel
        }

        private void FillRowData(List<ProblemCasesMonitorModel> data, ref ExcelTemplateWriter writer)
        {
            int index = 1;
            var cellStyle = writer.GetCellStyle(0, 1);

            foreach (var item in data)
            {
                var record = new object[]
                {
                       item.UserName,
                       item.SubscriberName,
                       item.RobotToString,
                       item.SuspiciousAssociationToString,
                       item.ProPunterToString
                };

                var row = writer.CreateRow(index++, record, 5);
                writer.SetCellStyle(row, cellStyle, 0, 5);
            }
        }

        private void AppendBlockContent(ref RichTextRender content, ref int lineCount, string name, IFont nameFont, string value, IFont valueFont, string endString)
        {
            if (string.IsNullOrEmpty(value))
            {
                return;
            }

            // Name
            TextBlock nameBlock = new TextBlock(name + ":  ", nameFont);
            content = content.AddText(nameBlock);

            // Value
            TextBlock valueBlock = new TextBlock(value, valueFont);
            content = content.AddText(valueBlock);

            if (!string.IsNullOrEmpty(endString))
            {
                content = content.AddText(endString);
            }

            lineCount += CountLine(name + value);
        }

        private int CountLine(string line)
        {
            int charPerLine = 216; // Number char per line in filter block - use to calculate number of line
            return Convert.ToInt32(Math.Ceiling((float)line.Length / charPerLine));
        }
    }
}
