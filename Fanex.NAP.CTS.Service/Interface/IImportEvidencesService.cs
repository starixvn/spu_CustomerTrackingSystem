namespace Fanex.NAP.CTS.Service
{    
    using Fanex.NAP.CTS.Model.ImportEvidences;
    using System.Collections.Generic;
    using System.IO;

    public interface IImportEvidencesService
    {
        string ParseFileXls(Stream fileContent, string fileName);

        string ParseFileXlsx(Stream fileContent, string fileName);

        byte[] DownloadTemplateExcel(string template);

        IEnumerable<ResultCheckAccountEvidenceModel> CheckRuleAddAccountEvidence(bool isOverride);

        IEnumerable<RawAccountEvidenceModel> GetRawAccountEvidences();

        void InsertCustEvidenceFromFile(IEnumerable<ResultCheckAccountEvidenceModel> resultCheckAccountEvidenceList, string fileName, int userId, string fileErrorTemplate, bool isOverride);

    }
}
