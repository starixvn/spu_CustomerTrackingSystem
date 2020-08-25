namespace Fanex.NAP.CTS.Service
{
    using Fanex.NAP.CTS.Core;
    using Fanex.NAP.CTS.Model.ImportEvidences;
    using Fanex.NAP.CTS.ViewModel.ImportEvidences;
    using System.Collections;
    using System.Collections.Generic;
    using System.IO;

    public interface IImportEvidencesService
    {
        string ParseFileXls(Stream fileContent, string fileName);

        string ParseFileXlsx(Stream fileContent, string fileName);

        byte[] DownloadTemplateExcel(string template);

        IEnumerable<ResultCheckAccountEvidenceModel> CheckRuleAddAccountEvidence(bool isOverwrite);

        void InsertAccountEvidence(IEnumerable<ResultCheckAccountEvidenceModel> resultCheckAccountEvidenceList, string fileName, int userId, string fileErrorTemplate, bool isOverwrite);

        IEnumerable<RawAccountEvidenceModel> GetRawAccountEvidences();

    }
}
