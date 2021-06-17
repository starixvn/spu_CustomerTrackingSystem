namespace Fanex.NAP.CTS.Service.Implement
{
    using Fanex.Data;
    using Fanex.NAP.CTS.Core.ExportExcel;
    using Fanex.NAP.CTS.Core.Helpers.Api;
    using Fanex.NAP.CTS.Criteria.ProblemCaseMonitor;
    using Fanex.NAP.CTS.Model.ProblemCaseMonitor;
    using Fanex.NAP.CTS.Service.Interface;
    using Fanex.NAP.TGRS.Core.ExcelUtility;
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

        public byte[] ExportProblemCaseMonitor(List<ProblemCasesMonitorModel> data, string fileName, string templatePath, string sheetName)
        {
            var writer = new ExcelTemplateWriter(templatePath, fileName, sheetName);
            writer.CopyRows(0, 1);
            FillRowData(data, ref writer);
            writer.SyncColumnsWidth(6);
            writer.ToBytes();

            return writer.ToBytes();
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
    }
}
