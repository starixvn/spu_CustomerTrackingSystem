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
        private readonly List<CheckAccountEvidenceModel> checkAccountEvidenceList;
        private readonly List<RawAccountEvidenceModel> rawAccountEvidenceList;

        public ImportEvidencesService()
        {
            checkAccountEvidenceList = new List<CheckAccountEvidenceModel>();
            rawAccountEvidenceList = new List<RawAccountEvidenceModel>();
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

                    checkAccountEvidenceList.Add(new CheckAccountEvidenceModel
                    {
                        RowId = i + 1,
                        Username = ParseCell(row.GetCell(0)),
                        EvidenceCode = ParseCell(row.GetCell(1)),
                    });

                    rawAccountEvidenceList.Add(new RawAccountEvidenceModel
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
           => rawAccountEvidenceList;

        public byte[] DownloadTemplateExcel(string template)
        {
            var wb = ExcelUtilities.GetWorkbook(template);

            return wb.ToBytes();
        }

        public IEnumerable<ResultCheckAccountEvidenceModel> CheckRuleAddAccountEvidence()
        {
            var duplicateList = checkAccountEvidenceList.GroupBy(x => new { x.Username, x.EvidenceCode })
                    .Where(g => g.Count() > 1)
                    .Select(x => x.Last())
                    .Select(x=> new ResultCheckAccountEvidenceModel {
                        CTSCustID = 0,
                        EvidenceID = 0,
                        RowId = x.RowId,
                        RowStatus = ResourcesManager.GetValue(ConstsResources.IMPORT_ACCOUNT_EVIDENCE_DUPLICATE)
                    });

            var noDuplicateList = checkAccountEvidenceList.Distinct(x => new { x.Username, x.EvidenceCode });

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
        
        public void InsertAccountEvidence(IEnumerable<ResultCheckAccountEvidenceModel> resultCheckAccountEvidenceList, string fileName, int userId, bool isCreatedByMaster)
        {
            var addAccountEvidenceList = resultCheckAccountEvidenceList
                .Join(rawAccountEvidenceList,
                    x => x.RowId,
                    y => y.RowId,
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
                        IsCreatedByMaster = isCreatedByMaster,
                        ErrorMessage = string.Empty
                    };

                    using (var db = ObjectDbFactory.CreateInstance(criteria.GetSettingKey()))
                    {
                        db.ExecuteNonQuery(criteria);
      
                        if (!string.IsNullOrWhiteSpace(criteria.ErrorMessage))
                        {
                            var errorMessage = string.Format("File name: {0} - userId: {2} - import error: {1} - content: {3}"
                                                        , fileName, userId, criteria.ErrorMessage, jsonData);
                            LogService.Error(errorMessage);
                        } 
                    }
                }
                catch (Exception ex)
                {
                    var errorMessage = string.Format("File name: {0} - userId: {2} - import error: {1} - content: {3}"
                                                       , fileName, userId, ex.ToString(), jsonData);
                    LogService.Error(errorMessage, ex);
                }
            }
        }
    }
}