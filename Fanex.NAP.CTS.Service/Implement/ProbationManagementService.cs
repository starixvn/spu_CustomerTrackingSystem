namespace Fanex.NAP.CTS.Service.Implement
{
    using Fanex.Data;
    using Fanex.Data.Repository;
    using Fanex.Logging;
    using Fanex.NAP.CTS.Core;
    using Fanex.NAP.CTS.Core.ExportExcel;    
    using Fanex.NAP.CTS.Criteria.ProbationManagement;
    using Fanex.NAP.CTS.Model.ProbationManagement;
    using Fanex.NAP.CTS.Model.SyncCustomerToMainDb;
    using Fanex.NAP.CTS.Resource.Consts;
    using Fanex.NAP.CTS.Service.Interface;
    using Fanex.NAP.CTS.ViewModel.ProbationManagement;
    using NPOI.SS.UserModel;
    using NPOI.XSSF.UserModel;
    using System;
    using System.Collections.Generic;    
    using System.Text;

    public class ProbationManagementService : IProbationManagementService
    {
        public List<AccountStaticStatusModel> GetAccountStaticStatuses()
        {
            var statusList = new List<AccountStaticStatusModel>();

            statusList.Add(new AccountStaticStatusModel
            {
                Ids = "1,11", // 1:Open, 11:Active
                Name = "Open"
            });

            statusList.Add(new AccountStaticStatusModel
            {
                Ids = "2,3,4,12,13,14", // 2:Disabled, 3:Closed, 4:Suspended, 12:Inactive, 13:View Only, 14:Suspended
                Name = "Closed"
            });

            return statusList;
        }
      
        public IEnumerable<ProbationCategoryModel> GetProbationCategory(string username, int daysLeft, int categoryMode)
        {
            var criteria = new GetProbationCategoryCriteria
            {
                UserName = username,
                FromDaysLeft = daysLeft,
                CategoryMode = categoryMode
            };

            using (var db = ObjectDbFactory.CreateInstance(criteria.GetSettingKey()))
            {
                return db.Query<ProbationCategoryModel>(criteria);
            }
        }

        public IEnumerable<ProbationCategoryModel> GetProbationCategory(ExportProbationCategoryFilter filter, out DBResult dBResult)
        {
            dBResult = new DBResult { ErrorCode = 0, ErrorMsg = string.Empty };

            var criteria = new GetProbationCategoryCriteria
            {
                UserName = filter.Username,
                SubscriberID = filter.SubscriberID,
                FromDaysLeft = filter.FromDaysLeft,
                ToDaysLeft = filter.ToDaysLeft,
                StatusIDs = filter.StatusIDs,
                IsAssociatedPA = filter.IsAssociatedPA,
                CategoryMode = filter.CategoryMode
            };

            try
            {
                using (var db = ObjectDbFactory.CreateInstance(criteria.GetSettingKey()))
                {
                    return db.Query<ProbationCategoryModel>(criteria);
                }
            }
            catch (Exception ex)
            {
                dBResult = new DBResult { ErrorCode = -1, ErrorMsg = ex.Message };

                var errorMessage = "CTS - Probation Management - GetProbationCategory exception: " + ex.Message;

                LogService.Error(errorMessage, ex);
            }

            return new List<ProbationCategoryModel>();
        }

        public IEnumerable<AccumulatedInfoModel> GetAccummulatedInfo(string customersXML)
        {
            try
            {
                var criteria = new GetAccummulatedInfoCriteria
                {
                    CustomersXML = customersXML
                };

                using (var db = ObjectDbFactory.CreateInstance(criteria.GetSettingKey()))
                {
                    return db.Query<AccumulatedInfoModel>(criteria);
                }
            }
            catch (Exception ex)
            {
                var errorMessage = "CTS - Probation Management - GetAccummulatedInfo exception: " + ex.Message;

                LogService.Error(errorMessage, ex);

                return new List<AccumulatedInfoModel>();
            }
        }

        public ResultChangeCategoryToCts ChangeCategory(string jsonData, string remark, int createdBy)
        {
            var result = new ResultChangeCategoryToCts
            {
                CustomerClassList = new List<CustomerClassForMainDb>()
            };

            var criteria = new UpdateProbationCategoryCriteria
            {
                CustIdList = jsonData,
                Remark = remark,
                CreatedBy = createdBy
            };

            try
            {
                var repository = new DynamicRepository();
                var customerClass = repository.Fetch<CustomerClassForMainDb>(criteria);

                if (string.IsNullOrWhiteSpace(criteria.ErrorMessage))
                {
                    result.DBResult = new DBResult { ErrorCode = 0, ErrorMsg = criteria.ErrorMessage };
                    result.CustomerClassList = customerClass;

                    return result;
                }
                result.DBResult = new DBResult { ErrorCode = -1, ErrorMsg = criteria.ErrorMessage };
                return result;
            }
            catch (Exception ex)
            {
                var errorMessage = "CTS - Probation Management - ChangeCategory exception: " + ex.Message;

                LogService.Error(errorMessage, ex);

                result.DBResult = new DBResult { ErrorCode = -1, ErrorMsg = ex.Message };
                return result;
            }
        }

        public string ExportCSV(IEnumerable<ProbationCategoryViewModel> probationCategoryList, ExportProbationCategoryFilter filter)
        {
            var content = new StringBuilder();

            try
            {
                content.AppendLine("PROBATION MANAGEMENT");
                content.AppendLine($"Username: {filter.Username}");
                content.AppendLine($"Site: {filter.SubscriberName}");
                content.AppendLine($"Status: {filter.StatusNames}");
                content.AppendLine($"Probation Day Left: {filter.DaysLeft}");
                content.AppendLine($"Associated With PA: {filter.AssociatedWithPA}");
                content.AppendLine($"Current WinLoss: {filter.MinWinLoss} - {filter.MaxWinLoss}");
                content.AppendLine($"Current Margin: {filter.MinMargin} - {filter.MaxMargin}");
                content.AppendLine("#,Username,Site,Status,Associated With PA,Tagged Date,Current WinLoss,First Margin(%),Current Margin(%),Probation Day Left");

                var rowIdx = 1;

                foreach (var item in probationCategoryList)
                {
                    content.AppendLine($"{rowIdx}," +
                       $"{(item.UserName)}," +
                       $"{(item.SiteName)}," +
                       $"{(item.Status)}," +
                       $"{(item.AssociatedWithPA == true ? "Yes" : "No")}," +
                       $"{(item.TaggedDate)}," +
                       $"{item.CurrentWinLossString}," +
                       $"{item.FirstMargin.ToString(Consts.FormatDecimalToString_2Places)}," +
                       $"{item.CurrentMarginString}," +
                       $"{item.DaysLeft}");

                    rowIdx++;
                }
            }
            catch (Exception ex)
            {
                var errorMessage = "CTS - Probation Management - ExportCSV exception: " + ex.Message;

                LogService.Error(errorMessage, ex);
            }

            return content.ToString();
        }

        public byte[] ExportExcel(IEnumerable<ProbationCategoryViewModel> probationCategoryList, ExportProbationCategoryFilter filter, string template)
        {
            try
            {
                var workbook = ExcelUtilities.GetWorkbook(template);
                var sheet = workbook.GetSheetAt(0);

                var rowTemplate = sheet.GetRow(sheet.LastRowNum);

                for (int i = 0; i < 10; i++)
                {
                    rowTemplate.GetCell(i).SetCellValue(string.Empty);
                }

                FillFilterBlock(filter, sheet);

                FillRowData(probationCategoryList, rowTemplate, sheet);

                sheet.RemoveRow(rowTemplate);

                return workbook.ToBytes();
            }
            catch (Exception ex)
            {
                var errorMessage = "CTS - Probation Management - ExportExcel exception: " + ex.Message;

                LogService.Error(errorMessage, ex);

                return new byte[] { };
            }
        }

        private void FillFilterBlock(ExportProbationCategoryFilter filter, ISheet sheet)
        {
            var rowFilter = sheet.GetRow(1);

            new XSSFRichTextString()
                .WriteBold("Username: ")
                .Write($"{filter.Username}\n")
                .WriteBold("Site: ")
                .Write($"{filter.SubscriberName}\n")
                .WriteBold("Status: ")
                .Write($"{filter.StatusNames}\n")
                .WriteBold("Probation Day Left: ")
                .Write($"{filter.DaysLeft}\n")
                .WriteBold("Associated With PA: ")
                .Write($"{filter.AssociatedWithPA}\n")
                .WriteBold("Current WinLoss: ")
                .Write($"{filter.MinWinLoss} - {filter.MaxWinLoss}\n")
                .WriteBold("Current Margin: ")
                .Write($"{filter.MinMargin} - {filter.MaxMargin}\n")
                .SetValue(rowFilter.GetCell(0));
        }

        private void FillRowData(IEnumerable<ProbationCategoryViewModel> probationCategoryList, IRow rowTemplate, ISheet sheet)
        {
            var rowIdx = 1;

            foreach (var item in probationCategoryList)
            {
                var columnIdx = 0;
                sheet.ShiftRows(sheet.LastRowNum, sheet.LastRowNum, 1, true, false);

                var rowNew = sheet.CreateRow(sheet.LastRowNum - 1);
                ExcelUtilities.CopyRow(rowTemplate, rowNew);

                rowNew.GetCell(columnIdx++).SetCellValue(rowIdx);
                rowNew.GetCell(columnIdx++).SetCellValue(item.UserName);
                rowNew.GetCell(columnIdx++).SetCellValue(item.SiteName);
                rowNew.GetCell(columnIdx++).SetCellValue(item.Status);
                rowNew.GetCell(columnIdx++).SetCellValue(item.AssociatedWithPA == true ? "Yes" : "No");
                rowNew.GetCell(columnIdx++).SetCellValue(item.TaggedDate);
                rowNew.GetCell(columnIdx++).SetCellValue(item.CurrentWinLossString);
                rowNew.GetCell(columnIdx++).SetCellValue(item.FirstMargin.ToString(Consts.FormatDecimalToString_Thousands2Places));
                rowNew.GetCell(columnIdx++).SetCellValue(item.CurrentMarginString);
                rowNew.GetCell(columnIdx).SetCellValue(item.DaysLeft);

                rowIdx++;
            }
        }
    }
}
