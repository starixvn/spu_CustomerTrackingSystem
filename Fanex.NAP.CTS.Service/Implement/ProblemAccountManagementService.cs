namespace Fanex.NAP.CTS.Service
{
    using Fanex.Data;
    using Fanex.Data.Repository;
    using Fanex.NAP.CTS.Criteria.Account;
    using Fanex.NAP.CTS.Criteria.ProblemAccountManagement;
    using Fanex.NAP.CTS.Criteria.ProblemAccountMonitor;
    using Fanex.NAP.CTS.Model.Account;
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

        public IEnumerable<CategoryInfoModel> GetAllCategory()
        {
            var criteria = new GetCategoryInfoCriteria();

            using (var db = ObjectDbFactory.CreateInstance(criteria.GetSettingKey()))
            {
                return db.Query<CategoryInfoModel>(criteria);
            }
        }

        public IEnumerable<GetProblemAccountModel> GetProblemAccounts(ProblemAccountFilterViewModel filter)
        {
            var criteria = new GetProblemAccountCriteria
            {
                FromDate = filter.FromDate,
                ToDate = filter.ToDate,
                Sites = filter.Sites,
                CategoryIDs = filter.CategoryIDs,
                UserName = filter.UserName,
                HasDownline = filter.HasDownline
            };

            using (var db = ObjectDbFactory.CreateInstance(criteria.GetSettingKey()))
            {
                return db.Query<GetProblemAccountModel>(criteria).ToList();
            }
        }

        public IEnumerable<ProblemAccountCategoryModel> GetProblemAccountCategories()
        {
            var criteria = new GetProblemAccountCategoryCriteria();

            using (var db = ObjectDbFactory.CreateInstance(criteria.GetSettingKey()))
            {
                return db.Query<ProblemAccountCategoryModel>(criteria);
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
     
        public byte[] ExportProblemAccount(IEnumerable<GetProblemAccountViewModel> data, string fileName, string templatePath, ProblemAccountFilterViewModel filter)
        {
            var writer = new ExcelTemplateWriter(templatePath, fileName, "PAM");
            writer.CopyRows(0, 1);

            FilterBlock(filter, data.Count(), ref writer);
            writer.CopyRows(2, 1); // Fill Header

            FillRowData(data, ref writer);
            writer.SyncColumnsWidth(7);

            return writer.ToBytes();
        }

        private void FilterBlock(ProblemAccountFilterViewModel filter, int totalItems, ref ExcelTemplateWriter writer)
        {
            var boldFont = writer.GetFont(10, 0);
            var normalFont = writer.GetFont(11, 0);
            var lineCount = 1;
            var contentRender = new RichTextRender().Clear();

            AppendBlockContent(ref contentRender, ref lineCount, "From", boldFont, filter.FromDate.ToString(Consts.FormatDateToString_MMMddyyyy), normalFont, "\n");                   
            AppendBlockContent(ref contentRender, ref lineCount, "To", boldFont, filter.ToDate.ToString(Consts.FormatDateToString_MMMddyyyy), normalFont, "\n");

            if (filter.IsAllSites)
            {
                AppendBlockContent(ref contentRender, ref lineCount, "Site", boldFont, "All", normalFont, "\n");
            }
            else
            {
                AppendBlockContent(ref contentRender, ref lineCount, "Site", boldFont, " ", normalFont, "\n");

                if (!string.IsNullOrEmpty(filter.CreditSiteNames))                
                    AppendBlockContent(ref contentRender, ref lineCount, "- Credit", boldFont, filter.CreditSiteNames, normalFont, "\n");
                
                if (!string.IsNullOrEmpty(filter.LicenseeSiteNames))                
                    AppendBlockContent(ref contentRender, ref lineCount, "- Licensee", boldFont, filter.LicenseeSiteNames, normalFont, "\n");
                
                if (!string.IsNullOrEmpty(filter.CN88SiteNames))                
                    AppendBlockContent(ref contentRender, ref lineCount, "- CN88", boldFont, filter.CN88SiteNames, normalFont, "\n");                
            }

            AppendBlockContent(ref contentRender, ref lineCount, "Problem Category", boldFont, filter.IsAllCategories ? "All" : filter.CategoryNames, normalFont, "\n");
            
            var userName = string.IsNullOrEmpty(filter.UserName) ? " " : filter.UserName;
            AppendBlockContent(ref contentRender, ref lineCount, "Username", boldFont, userName, normalFont, "\n");                      
            AppendBlockContent(ref contentRender, ref lineCount, "Search downlines of this account", boldFont, filter.HasDownline ? "yes" : " ", normalFont, "\n");           
            AppendBlockContent(ref contentRender, ref lineCount, "Total items", boldFont, totalItems.ToString(), normalFont, "\n");
            
            var dateTime = filter.DateTime.ToString(Consts.FormatDateToString_MMMddyyyyhhmmss2);            
            AppendBlockContent(ref contentRender, ref lineCount, "Export date time", boldFont, dateTime, normalFont, "\n");       
            
            WriteRickText(contentRender, ref writer, 4);

            writer.SetRowHeight(1, 225 * lineCount);
        }

        private void WriteRickText(RichTextRender contentRender, ref ExcelTemplateWriter writer, int idx)
        {
            var contentRichTextString = contentRender.XSSFRickRender();
            XSSFRichTextString[] contentRichTextArray = new XSSFRichTextString[1];
            contentRichTextArray[0] = contentRichTextString;
            writer.WriteRow(idx, contentRichTextArray);
        }

        private void FillRowData(IEnumerable<GetProblemAccountViewModel> problemAccounts, ref ExcelTemplateWriter writer)
        {
            foreach (var item in problemAccounts)
            {
                var record = new object[]
                {
                     item.UserName,
                     item.Site,
                     item.Currency,
                     item.Category,
                     item.Comment,
                     item.CreatedDateStr,
                     item.CreatedBy                     
                };

                writer.WriteRow(3, record);
            }
        }

        private void AppendBlockContent(ref RichTextRender content, ref int lineCount, string name, IFont nameFont, string value, IFont valueFont, string endString)
        {
            TextBlock nameBlock = new TextBlock(name + ": ", nameFont);
            content = content.AddText(nameBlock);

            TextBlock valueBlock = new TextBlock(value, valueFont);
            content = content.AddText(valueBlock);

            if (!string.IsNullOrEmpty(endString))
            {
                content = content.AddText(endString);
            }

            lineCount += 1;
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
