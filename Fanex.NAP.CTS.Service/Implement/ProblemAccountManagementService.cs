namespace Fanex.NAP.CTS.Service
{
    using Fanex.Data;
    using Fanex.Data.Repository;
    using Fanex.NAP.CTS.Criteria.ProblemAccountManagement;
    using Fanex.NAP.CTS.Criteria.ProblemAccountMonitor;
    using Fanex.NAP.CTS.Model.ProblemAccountManagement;
    using Fanex.NAP.CTS.Model.ProblemAccountMonitor;
    using Fanex.NAP.CTS.Resource.Consts;
    using Fanex.NAP.CTS.ViewModel.ProblemAccountManagement;
    using Fanex.NAP.TGRS.Core.ExcelUtility;
    using Fanex.NAP.TGRS.Core.ExcelUtility.RichTextExtensions;    
    using NPOI.SS.UserModel;
    using NPOI.XSSF.UserModel;    
    using System.Collections.Generic;
    using System.Linq;    

    public class ProblemAccountManagementService : IProblemAccountManagementService
    {
        public IEnumerable<SubscriberGroupModel> GetAllSites()
        {
            var criteria = new GetAllSubscriberGroupsCriteria();

            using (var db = ObjectDbFactory.CreateInstance(criteria.GetSettingKey()))
            {
                return db.Query<SubscriberGroupModel>(criteria);
            }
        }

        public IEnumerable<ProblemAccountModel> GetProblemAccounts(ProblemAccountFilterViewModel filter)
        {
            var criteria = new GetProblemAccountCriteria
            {
                FromDate = filter.FromDate,
                ToDate = filter.ToDate,
                Sites = filter.Sites,
                UserName = filter.UserName,
                HasDownline = filter.HasDownline
            };

            using (var db = ObjectDbFactory.CreateInstance(criteria.GetSettingKey()))
            {
                return db.Query<ProblemAccountModel>(criteria).ToList();
            }
        }

        public IEnumerable<ProblemAccountCategoryModel> GetProblemAccountCategories()
        {
            var criteria = new GetProblemAccountCategoryCriteria();

            using (var db = ObjectDbFactory.CreateInstance(criteria.GetSettingKey()))
            {
                return db.Query<ProblemAccountCategoryModel>(criteria).ToList();
            }
        }

        public IEnumerable<ProblemAccountValidatedModel> ValidateProblemAccounts(ValidateProblemAccountInputViewModel model)
        {
            var result = Enumerable.Empty<ProblemAccountValidatedModel>();
            var criteria = new ValidateUserNamesInputCriteria
            {
                UserNames = model.UserNames             
            };

            using (var db = ObjectDbFactory.CreateInstance(criteria.GetSettingKey()))
            {
                var data = db.Query<ProblemAccountValidatedModel>(criteria);

                if (data != null && data.Any())
                {
                    result = data.Select(x => new ProblemAccountValidatedModel
                    {
                        CTSCustID = x.CTSCustID,
                        SubscriberID = x.SubscriberID,
                        RoleID = x.RoleID,
                        CustID = x.CustID,                        
                        UserName = x.UserName,
                        IsLicensee = x.IsLicensee,
                        ActionID = x.ActionID,
                        ErrorMsg = x.ErrorMsg
                    });
                }
            }

            return result;
        }

        public IEnumerable<GetCustEvidenceByReasonIdModel> GetCustEvidenceByReasonIds()
        {
            var criteria = new GetCustEvidenceByReasonIdCriteria();

            using (var db = ObjectDbFactory.CreateInstance(criteria.GetSettingKey()))
            {
                return db.Query<GetCustEvidenceByReasonIdModel>(criteria).ToList();
            }
        }
     
        public byte[] ExportProblemAccount(IEnumerable<ProblemAccountViewModel> data, string fileName, string templatePath, ProblemAccountFilterViewModel filter)
        {
            var writer = new ExcelTemplateWriter(templatePath, fileName, "PAM");
            writer.CopyRows(0, 1);

            FilterBlock(filter, data.Count(), ref writer);
            writer.CopyRows(2, 1); // Fill Header

            FillRowData(data, ref writer);
            writer.SyncColumnsWidth(6);

            return writer.ToBytes();
        }

        private void FilterBlock(ProblemAccountFilterViewModel filter, int totalItems, ref ExcelTemplateWriter writer)
        {
            var boldFont = writer.GetFont(10, 0);
            var normalFont = writer.GetFont(11, 0);            

            var contentRender = new RichTextRender().Clear();
            AppendBlockContent(ref contentRender, "From", boldFont, filter.FromDate.ToString(Consts.FormatDateToString_MMMddyyyy), normalFont, "\n");
            WriteRickText(contentRender, ref writer, 4);

            contentRender = new RichTextRender().Clear();
            AppendBlockContent(ref contentRender, "To", boldFont, filter.ToDate.ToString(Consts.FormatDateToString_MMMddyyyy), normalFont, "\n");
            WriteRickText(contentRender, ref writer, 4);

            contentRender = new RichTextRender().Clear();
            AppendBlockContent(ref contentRender, "Site", boldFont, filter.IsAllSites ? "All" : filter.Sites, normalFont, "\n");
            WriteRickText(contentRender, ref writer, 4);

            contentRender = new RichTextRender().Clear();
            AppendBlockContent(ref contentRender, "Username", boldFont, " ", normalFont, "\n");
            WriteRickText(contentRender, ref writer, 4);

            contentRender = new RichTextRender().Clear();
            AppendBlockContent(ref contentRender, "Search downlines of this account", boldFont, filter.HasDownline ? "yes" : " ", normalFont, "\n");
            WriteRickText(contentRender, ref writer, 4);

            contentRender = new RichTextRender().Clear();
            AppendBlockContent(ref contentRender, "Total items", boldFont, totalItems.ToString(), normalFont, "\n");
            WriteRickText(contentRender, ref writer, 4);

            var dateTime = filter.DateTime.ToString(Consts.FormatDateToString_MMMddyyyyhhmmss2);
            contentRender = new RichTextRender().Clear();
            AppendBlockContent(ref contentRender, "Export date time", boldFont, dateTime, normalFont, "\n");             
            WriteRickText(contentRender, ref writer, 4);          
        }

        private void WriteRickText(RichTextRender contentRender, ref ExcelTemplateWriter writer, int idx)
        {
            var contentRichTextString = contentRender.XSSFRickRender();
            XSSFRichTextString[] contentRichTextArray = new XSSFRichTextString[1];
            contentRichTextArray[0] = contentRichTextString;
            writer.WriteRow(idx, contentRichTextArray);
        }

        private void FillRowData(IEnumerable<ProblemAccountViewModel> problemAccounts, ref ExcelTemplateWriter writer)
        {
            foreach (var item in problemAccounts)
            {
                var record = new object[]
                {
                     item.UserName,
                     item.Site,
                     item.Currency,
                     item.Category,
                     item.CreatedBy,
                     item.CreatedDateStr
                };

                writer.WriteRow(3, record);
            }
        }

        private void AppendBlockContent(ref RichTextRender content, string name, IFont nameFont, string value, IFont valueFont, string endString)
        {
            TextBlock nameBlock = new TextBlock(name + ": ", nameFont);
            content = content.AddText(nameBlock);

            TextBlock valueBlock = new TextBlock(value, valueFont);
            content = content.AddText(valueBlock);

            if (!string.IsNullOrEmpty(endString))
            {
                content = content.AddText(endString);
            }            
        }      
       
        public IEnumerable<long> RemoveProblemAccountEvidence(string custIds)
        {
            var repository = new DynamicRepository();
            var criteria = new RemoveProblemAccountEvidenceCriteria
            {
                CustIDs = custIds,
                SourceType = 1
            };

            return repository.Fetch<long>(criteria);
        }
    }
}
