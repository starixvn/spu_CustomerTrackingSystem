namespace Fanex.NAP.CTS.Service
{
    using NPOI.SS.UserModel;
    using NPOI.XSSF.UserModel;
    using NPOI.HSSF.UserModel;
    using System.Linq;
    using System.IO;
    using Fanex.NAP.CTS.Model.ImportEvidences;    
    using System.Collections.Generic;
    using Fanex.NAP.CTS.Core.Extentions;    
    using Fanex.NAP.CTS.Criteria.ImportEvidences;
    using Fanex.Data;
    using Fanex.NAP.CTS.Core;
    using Newtonsoft.Json;
    using Fanex.NAP.CTS.Resource;
    using Fanex.NAP.CTS.Core.ExportExcel;
    using System;
    using Fanex.Logging;
    using Fanex.NAP.CTS.Core.Utilities;

    public class ImportEvidencesService : IImportEvidencesService
    {
        private readonly List<CheckAccountEvidenceModel> _checkAccountEvidenceList;
        private readonly List<RawAccountEvidenceModel> _rawAccountEvidenceList;
        private string _fileErrorTemplate = string.Empty;

        public ImportEvidencesService()
        {
            _checkAccountEvidenceList = new List<CheckAccountEvidenceModel>();
            _rawAccountEvidenceList = new List<RawAccountEvidenceModel>();
        }

        public string ParseFileXls(Stream fileContent, string fileName)
        {
            using (fileContent)
            {
                var workbook = new HSSFWorkbook(fileContent);
               
                return ParseFile(workbook.GetSheetAt(0), fileName);
            }
        }

        public string ParseFileXlsx(Stream fileContent, string fileName)
        {
            using (fileContent)
            {
                var workbook = new XSSFWorkbook(fileContent);

                return ParseFile(workbook.GetSheetAt(0), fileName);
            }
        }

        private string ParseFile(ISheet sheet, string fileName)
        {
            var firstRowNum = 0;
            var lastRowNum = sheet.LastRowNum + 1;

            try
            {
                for (int i = firstRowNum; i < lastRowNum; i++)
                {
                    IRow row = sheet.GetRow(i);

                    if (row == null || row.Cells.All(d => d.CellType == CellType.Blank))
                        continue;

                    _checkAccountEvidenceList.Add(new CheckAccountEvidenceModel
                    {
                        RowId = i + 1,
                        Username = ParseCell(row.GetCell(0)),
                        EvidenceCode = ParseCell(row.GetCell(1)),
                    });

                    _rawAccountEvidenceList.Add(new RawAccountEvidenceModel
                    {
                        RowId = i + 1,
                        Username = ParseCell(row.GetCell(0)),
                        EvidenceCode = ParseCell(row.GetCell(1)),
                        Comment = ParseCell(row.GetCell(2)),
                    });
                }

                return string.Empty;
            }
            catch (Exception ex)
            {
                var errorMessage = string.Format(ResourcesManager.GetValue(ConstsResources.IMPORT_ACCOUNT_EVIDENCE_PARSE_FILE_ERROR), fileName);
                LogService.Error(errorMessage, ex);

                return errorMessage;
            }
        }

        private string ParseCell(object cellValue)
        {
            if (cellValue == null)
                return string.Empty;

            return cellValue.ToString();
        }

        public IEnumerable<RawAccountEvidenceModel> GetRawAccountEvidences()
           => _rawAccountEvidenceList;

        public byte[] DownloadTemplateExcel(string template)
        {
            var workbook = ExcelUtilities.GetWorkbook(template);

            return workbook.ToBytes();
        }

        public IEnumerable<ResultCheckAccountEvidenceModel> CheckRuleAddAccountEvidence()
        {
            var duplicateList = _checkAccountEvidenceList.GroupBy(x => new { x.Username, x.EvidenceCode })
                    .Where(g => g.Count() > 1)
                    .Select(x => x.Last())
                    .Select(x=> new ResultCheckAccountEvidenceModel {
                        CTSCustID = 0,
                        EvidenceID = 0,
                        RowId = x.RowId,
                        RowStatus = ResourcesManager.GetValue(ConstsResources.IMPORT_ACCOUNT_EVIDENCE_DUPLICATE)
                    });

            var noDuplicateList = _checkAccountEvidenceList.Distinct(x => new { x.Username, x.EvidenceCode });

            var jsonData = JsonConvert.SerializeObject(noDuplicateList);

            var criteria = new CheckAccountEvidenceCriteria
            {
                NewAccountEvidences = jsonData
            };

            using (var db = ObjectDbFactory.CreateInstance(criteria.GetSettingKey()))
            {
                var resultVerifyDbList = db.Query<ResultCheckAccountEvidenceModel>(criteria);

                return resultVerifyDbList
                    .Union(duplicateList)
                    .OrderBy(x=>x.RowId);
            }
        }
        
        public void InsertAccountEvidence(IEnumerable<ResultCheckAccountEvidenceModel> resultCheckAccountEvidenceList, string fileName, int userId, string fileErrorTemplate)
        {
            _fileErrorTemplate = fileErrorTemplate;

            var addAccountEvidenceList = resultCheckAccountEvidenceList
                .Join(_rawAccountEvidenceList, x => x.RowId, y => y.RowId,
                    (x, y) => new AddAccountEvidenceModel
                    {
                        CTSCustID = x.CTSCustID,
                        EvidenceID = x.EvidenceID,
                        Comment = y.Comment
                    });

            var loopAddAccountEvidenceList = addAccountEvidenceList.Chunk(ConfigSettings.ImportEvidenceChunkSize);

            foreach (var accountEvidence in loopAddAccountEvidenceList)
            {
                var jsonData = JsonConvert.SerializeObject(accountEvidence);

                try
                {
                    var criteria = new InsertAccountEvidenceCriteria
                    {
                        NewAccountEvidences = jsonData,
                        UserId = userId,
                        IsCreatedByMaster = true,
                        ErrorMessage = string.Empty
                    };

                    using (var db = ObjectDbFactory.CreateInstance(criteria.GetSettingKey()))
                    {
                        db.ExecuteNonQuery(criteria);
      
                        if (!string.IsNullOrWhiteSpace(criteria.ErrorMessage))
                        {
                            SendMail(resultCheckAccountEvidenceList, accountEvidence, userId);
                            LogError(fileName, userId, criteria.ErrorMessage, jsonData);
                        } 
                    }
                }
                catch (Exception ex)
                {
                    SendMail(resultCheckAccountEvidenceList, accountEvidence, userId);
                    LogError(fileName, userId, ex.Message, jsonData, ex);
                }
            }
        }

        private void LogError(string fileName, int userId, string error, string jsonData, Exception ex = null)
        {
            var errorMessage = string.Format("CTS - Import file name: {0} - userId: {2} - import error: {1} - content: {3}"
                                                       , fileName, userId, error, jsonData);
            if (ex == null)
                LogService.Error(errorMessage);
            else
                LogService.Error(errorMessage, ex);
        }

        private void SendMail(IEnumerable<ResultCheckAccountEvidenceModel> accountEvidenceCheckedList, IEnumerable<AddAccountEvidenceModel> errorAccountEvidenceModelList, int userId)
        {
            var attachmentFileName = ResourcesManager.GetValue(ConstsResources.IMPORT_ACCOUNT_EVIDENCE_SENDMAIL_ATTACHMENT_FILENAME);
            var subject = string.Format(ResourcesManager.GetValue(ConstsResources.IMPORT_ACCOUNT_EVIDENCE_SENDMAIL_SUBJECT),
                    ConfigSettings.EnvironmentName, userId, DateTime.Now);
            var body = ResourcesManager.GetValue(ConstsResources.IMPORT_ACCOUNT_EVIDENCE_SENDMAIL_BODY);

            try
            {
                var attachmentContent = ExportExcel(accountEvidenceCheckedList, errorAccountEvidenceModelList);
                IMailStationService mailService = new MailStationService(ConfigSettings.SPUMMailingListId)
                        .GenerateAttachment(attachmentFileName, attachmentContent)
                        .GenerateMail(subject, subject, body);

                mailService.SendMail();
            }
            catch (Exception ex)
            {
                LogService.Error(string.Format("CTS - Import file - send mail error: ", ex.Message), ex);
            }
        }

        private byte[] ExportExcel(IEnumerable<ResultCheckAccountEvidenceModel> accountEvidenceCheckedList, IEnumerable<AddAccountEvidenceModel> errorAccountEvidenceModelList)
        {
            var errorDetailList = errorAccountEvidenceModelList
                    .Join(accountEvidenceCheckedList, x => x.CTSCustID, y => y.CTSCustID, (x, y) => y)
                    .Join(_rawAccountEvidenceList, x => x.RowId, y => y.RowId, (x, y) => y);

            var workbook = ExcelUtilities.GetWorkbook(_fileErrorTemplate);

            if (errorDetailList.Any())
            {
                var sheet = workbook.GetSheetAt(0);
                do
                {
                    var rowOld = sheet.GetRow(sheet.LastRowNum);
                    if (rowOld != null)
                        sheet.RemoveRow(rowOld);
                } while (sheet.LastRowNum > 0);

                var rowTemplate = sheet.GetRow(sheet.LastRowNum);
                rowTemplate.GetCell(0).SetCellValue(string.Empty);
                rowTemplate.GetCell(1).SetCellValue(string.Empty);
                rowTemplate.GetCell(2).SetCellValue(string.Empty);

                foreach (var data in errorDetailList)
                {
                    sheet.ShiftRows(sheet.LastRowNum, sheet.LastRowNum, 1, true, false);

                    var newRowIndex = sheet.LastRowNum - 1;                    
                    var rowNew = sheet.CreateRow(newRowIndex);
                    ExcelUtilities.CopyRow(rowTemplate, rowNew);

                    rowNew.GetCell(0).SetCellValue(data.Username);
                    rowNew.GetCell(1).SetCellValue(data.EvidenceCode);
                    rowNew.GetCell(2).SetCellValue(data.Comment);
                }
            }

            return workbook.ToBytes();
        }

    }
}