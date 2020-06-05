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

    public class ImportEvidencesService : IImportEvidencesService
    {
        public IEnumerable<AddEvidenceModel> ParseFileXls(Stream fileContent)
        {
            using (fileContent)
            {
                var workbook = new HSSFWorkbook(fileContent);
               
                return ParseFile(workbook.GetSheetAt(0));
            }
        }

        public IEnumerable<AddEvidenceModel> ParseFileXlsx(Stream fileContent)
        {
            using (fileContent)
            {
                var workbook = new XSSFWorkbook(fileContent);

                return ParseFile(workbook.GetSheetAt(0));
            }
        }

        private IEnumerable<AddEvidenceModel> ParseFile(ISheet sheet)
        {
            var firstRowNum = 0;
            var lastRowNum = sheet.LastRowNum;
            var evidenceAddList = new List<AddEvidenceModel>();

            for (int i = firstRowNum; i < lastRowNum; i++)
            {
                IRow row = sheet.GetRow(i);

                if (row == null || row.Cells.All(d => d.CellType == CellType.Blank))
                    continue;

                evidenceAddList.Add(new AddEvidenceModel
                {
                    UserName = row.GetCell(0).ToString(),
                    EvidenceCode = row.GetCell(1).ToString(),
                    Comment = row.GetCell(2).ToString(),
                });
            }

            return evidenceAddList;
        }

        public IEnumerable<RuleCheckViewModel> CheckRule(IEnumerable<AddEvidenceModel> addEvidenceList)
        {
            return new List<RuleCheckViewModel>();
        }

        public DBResult AddEvidenceList(IEnumerable<AddEvidenceModel> evidenceList)
        {
            var criteria = new AddEvidenceCriteria
            {
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