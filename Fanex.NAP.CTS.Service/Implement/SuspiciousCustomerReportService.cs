namespace Fanex.NAP.CTS.Service
{
    using Criteria.SuspiciousCustomer;
    using ExcelConfig.SuspiciousCustomer;
    using Fanex.NAP.CTS.Core.BaseClasses;
    using Fanex.NAP.CTS.Core.ExportExcel;
    using Model.SuspiciousCustomer;
    using System.Collections.Generic;
    using System.Text;

    public class SuspiciousCustomerReportService : ISuspiciousCustomerReportService
    {
        public IEnumerable<SuspiciousCustomerReportModel> GetSuspiciousCustomerReport(SuspiciousCustomerReportGetCriteria criteria)
        {
            return GetData(criteria);
        }

        public IEnumerable<SuspiciousCustomerNoteModel> GetNotesData(int custId)
        {
            var criteria = new SuspiciousCustomerNoteGetCriteria(custId);

            return DataService.GetData<SuspiciousCustomerNoteModel>(criteria);
        }

        public byte[] ExportExcel(SuspiciousCustomerReportGetCriteria criteria, string fraudCaseName, string assignee, string template, IEnumerable<SuspiciousCustomerReportModel> data)
        {
            var wb = ExcelUtilities.GetWorkbook(template)
                                    .RenderSheet(new SuspiciousCustomerConfig(fraudCaseName, assignee), criteria, data, null);

            return wb.ToBytes();
        }

        public string ExportCSV(IEnumerable<SuspiciousCustomerReportModel> data, SuspiciousCustomerReportGetCriteria criteria, string fraudCaseName, string assignee, string classification, string verified)
        {
            var content = new StringBuilder();

            content.AppendLine("SUSPICIOUS CUSTOMER");

            content.AppendLine($"From Date: {criteria.FromDate.ToString("MM/dd/yyyy")}, ToDate: {criteria.ToDate.ToString("MM/dd/yyyy")}, Member: {criteria.Username}, Fraud Case: {fraudCaseName}, Verified: {verified}, Classification: {classification}, Assignees: {assignee}");

            content.AppendLine("#,Dirty,Delinquent,Hedge,Verified,Notes,Member,Total Violations,Violated Cases,#Tickets,Assignee");
            var counter = 0;           
            foreach (var item in data)
            {
                counter++;
                content.AppendLine($"{counter}," +
                    $"{(item.IsDirty ? "Yes" : "No")}," +
                    $"{(item.IsDelinquent ? "Yes": "No")}," +
                    $"{(item.IsHedge ? "Yes" : "No")}," +
                    $"{(item.IsVerified ? "Yes" : "No")}," +
                    $"{(item.HasNote ? "Yes" : "No")}," +
                    $"{item.Username}," +
                    $"{item.TotalViolation}," +
                    $"{item.ViolatedCases}," +
                    $"{item.BetCount}," +
                    $"{item.Assignee}");
            }

            return content.ToString();
        }

        public IEnumerable<SuspiciousCustomerReportModel> GetData(SuspiciousCustomerReportGetCriteria criteria)
        {
            return DataService.GetData<SuspiciousCustomerReportModel>(criteria);
        }
    }
}