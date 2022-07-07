namespace Fanex.NAP.CTS.Service.Implement
{
    using Fanex.Data.Repository;
    using Fanex.Logging;
    using Fanex.Logging.Extensions;
    using Fanex.Logging.Extensions.Database;
    using Fanex.NAP.CTS.Core;
    using Fanex.NAP.CTS.Criteria.AssociatedGroupMonitor;
    using Fanex.NAP.CTS.Model.AssociatedGroupMonitor;
    using Fanex.NAP.CTS.Service.Interface;
    using Fanex.NAP.CTS.ViewModel.AssociatedGroupMonitor;
    using Fanex.NAP.TGRS.Core.ExcelUtility;
    using Fanex.NAP.TGRS.Core.ExcelUtility.RichTextExtensions;
    using NPOI.SS.UserModel;
    using NPOI.XSSF.UserModel;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    public class AssociatedGroupMonitorService : IAssociatedGroupMonitorService
    {
        private readonly IDynamicRepository repository;

        public AssociatedGroupMonitorService()
        {
            repository = new DynamicRepository();
        }

        public IEnumerable<AssociatedGroupMonitorModel> GetAssociatedGroupMonitor()
        {
            var criteria = new GetAssociatedGroupMonitorCriteria();

            return repository.Fetch<AssociatedGroupMonitorModel>(criteria);
        }

        public IEnumerable<AssociatedAccountMonitorModel> GetAssociatedAccountMonitor(long groupID)
        {
            var criteria = new GetAssociatedAccountCriteria
            {
                GroupID = groupID
            };

            return repository.Fetch<AssociatedAccountMonitorModel>(criteria);
        }

        public DBResult AddAssociatedAccount(AddAssociatedAccountViewModel model)
        {
            var criteria = new AddAssociatedAccountCriteria
            {
                GroupID = model.GroupID,
                CTSCustIDs = model.CTSCustIDs,
                Remark = model.Comment,
                CreatedBy = model.CreatedBy
            };

            try
            {
                repository.Execute(criteria);

                return new DBResult { ErrorCode = 0 };
            }
            catch (Exception ex)
            {
                var logMessage = new LogMessage(ex.Message)
                 .AddSection("Data Info", DatabaseInfoGetters.GetDbExceptionInfo(ex))
                 .AddSection("Exception Info", CommonInfoGetters.GetExceptionInfo(ex)).ToString();

                LogService.Error($"Add associated account Exception { logMessage }", ex);

                return new DBResult { ErrorCode = -1, ErrorMsg = ex.Message };
            }
        }

        public DBResult AddAssociatedGroupMonitor(AddAssociatedGroupViewModel model)
        {
            var criteria = new AddAssociatedGroupMonitorCriteria
            {
                GroupName = model.GroupName,
                ABI = model.ABI,
                Remark = model.Comment,
                CreatedBy = model.CreatedBy
            };

            try
            {
                repository.Execute(criteria);

                return new DBResult { ErrorCode = 0 };
            }
            catch (Exception ex)
            {
                var logMessage = new LogMessage(ex.Message)
                 .AddSection("Data Info", DatabaseInfoGetters.GetDbExceptionInfo(ex))
                 .AddSection("Exception Info", CommonInfoGetters.GetExceptionInfo(ex)).ToString();

                LogService.Error($"Add associated group monitor Exception { logMessage }", ex);

                return new DBResult { ErrorCode = -1, ErrorMsg = ex.Message };
            }
        }      

        public DBResult RemoveAssociatedGroupMonitor(RemoveAssociatedGroupViewModel model)
        {
            var criteria = new RemoveAssociatedGroupMonitorCriteria
            {
                GroupID = model.GroupID,
                CreatedBy = model.CreatedBy
            };

            try
            {
                repository.Execute(criteria);

                return new DBResult { ErrorCode = 0 };
            }
            catch (Exception ex)
            {
                var logMessage = new LogMessage(ex.Message)
                 .AddSection("Data Info", DatabaseInfoGetters.GetDbExceptionInfo(ex))
                 .AddSection("Exception Info", CommonInfoGetters.GetExceptionInfo(ex)).ToString();

                LogService.Error($"Remove associated group monitor Exception { logMessage }", ex);

                return new DBResult { ErrorCode = -1, ErrorMsg = ex.Message };
            }
        }

        public byte[] ExportExcelAssociatedAccount(IEnumerable<AssociatedAccountMonitorModel> associatedAccounts
            , string fileName
            , string templatePath
            , ExportToExcelAssociatedAccountViewModel model)
        {
            var writer = new ExcelTemplateWriter(templatePath, fileName, "GMR");
            writer.CopyRows(0, 1);

            //FilterBlock(filter, data.Count(), ref writer);
            //writer.CopyRows(2, 1); // Fill Header

            FillRowData(associatedAccounts, ref writer);
            writer.SyncColumnsWidth(6);

            return writer.ToBytes();
        }        

        private void FilterBlock(ExportToExcelAssociatedAccountViewModel model, int totalItems, ref ExcelTemplateWriter writer)
        {
            var boldFont = writer.GetFont(10, 0);
            var normalFont = writer.GetFont(11, 0);

            //var contentRender = new RichTextRender().Clear();
            //AppendBlockContent(ref contentRender, "From", boldFont, filter.FromDate.ToString(Consts.FormatDateToString_MMMddyyyy), normalFont, "\n");
            //WriteRickText(contentRender, ref writer, 4);
        }

        private void AppendBlockContent(ref RichTextRender content, string name, IFont nameFont, string value, IFont valueFont, string endString)
        {
            TextBlock nameBlock = new TextBlock(name + ": ", nameFont);
            content = content.AddText(nameBlock);

            TextBlock valueBlock = new TextBlock(value, valueFont);
            content = content.AddText(valueBlock);

            if (!string.IsNullOrEmpty(endString))
                content = content.AddText(endString);
        }

        private void WriteRickText(RichTextRender contentRender, ref ExcelTemplateWriter writer, int idx)
        {
            var contentRichTextString = contentRender.XSSFRickRender();
            XSSFRichTextString[] contentRichTextArray = new XSSFRichTextString[1];
            contentRichTextArray[0] = contentRichTextString;
            writer.WriteRow(idx, contentRichTextArray);
        }

        private void FillRowData(IEnumerable<AssociatedAccountMonitorModel> associatedAccounts, ref ExcelTemplateWriter writer)
        {
            foreach (var item in associatedAccounts)
            {
                var record = new object[]
                {
                    item.Username,
                    item.AccountStatus,
                    item.AccountCreated,
                    item.Site,
                    item.Currency,
                    item.ABI,
                    item.Ori,
                    item.Category,
                    item.Remark,
                    item.DetectedBy,
                    item.DetectedDate
                };

                writer.WriteRow(3, record);
            }
        }

    }
}
