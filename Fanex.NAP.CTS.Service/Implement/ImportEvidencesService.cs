namespace Fanex.NAP.CTS.Service
{
    using NPOI.SS.UserModel;
    using NPOI.XSSF.UserModel;
    using NPOI.HSSF.UserModel;
    using System.Linq;
    using System.IO;
    using Fanex.NAP.CTS.Model.ImportEvidences;    
    using System.Collections.Generic;
    using Fanex.NAP.CTS.ViewModel.ImportEvidences;
    using Fanex.NAP.CTS.Criteria.ImportEvidences;
    using Fanex.Data;
    using Fanex.NAP.CTS.Core;
    using Newtonsoft.Json;

    public class ImportEvidencesService : IImportEvidencesService
    {
        private readonly List<CheckAccountEvidenceModel> checkAccountEvidenceList;
        private readonly List<RawAccountEvidenceModel> rawAccountEvidenceList;

        public ImportEvidencesService()
        {
            checkAccountEvidenceList = new List<CheckAccountEvidenceModel>();
            rawAccountEvidenceList = new List<RawAccountEvidenceModel>();
        }

        public void ParseFileXls(Stream fileContent)
        {
            using (fileContent)
            {
                var workbook = new HSSFWorkbook(fileContent);
               
                ParseFile(workbook.GetSheetAt(0));
            }
        }

        public void ParseFileXlsx(Stream fileContent)
        {
            using (fileContent)
            {
                var workbook = new XSSFWorkbook(fileContent);

                ParseFile(workbook.GetSheetAt(0));
            }
        }

        private void ParseFile(ISheet sheet)
        {
            var firstRowNum = 0;
            var lastRowNum = sheet.LastRowNum;

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
        }

        private string ParseCell(object cellValue)
        {
            if (cellValue == null)
                return string.Empty;

            return cellValue.ToString();
        }

        public IEnumerable<ResultCheckAccountEvidenceModel> CheckRuleAddAccountEvidence()
        {
            var jsonData = JsonConvert.SerializeObject(checkAccountEvidenceList);

            var criteria = new CheckAccountEvidenceCriteria
            {
                NewAccountEvidences = jsonData
            };

            using (var db = ObjectDbFactory.CreateInstance(criteria.GetSettingKey()))
            {
                return db.Query<ResultCheckAccountEvidenceModel>(criteria);
            }
        }

        public DBResult InsertAccountEvidence(IEnumerable<ResultCheckAccountEvidenceModel> resultCheckAccountEvidenceList, int userId, bool isCreatedByMaster)
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

            var jsonData = JsonConvert.SerializeObject(addAccountEvidenceList);

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

                return new DBResult
                {
                    ErrorCode = string.IsNullOrWhiteSpace(criteria.ErrorMessage) ? 0 : -1,
                    ErrorMsg = criteria.ErrorMessage
                };
            }
        }
    }
}