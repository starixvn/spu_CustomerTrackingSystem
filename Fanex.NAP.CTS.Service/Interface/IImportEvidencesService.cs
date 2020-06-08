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
        void ParseFileXls(Stream fileContent);

        void ParseFileXlsx(Stream fileContent);

        IEnumerable<ResultCheckAccountEvidenceModel> CheckRuleAddAccountEvidence();

        DBResult InsertAccountEvidence(IEnumerable<ResultCheckAccountEvidenceModel> resultCheckAccountEvidenceList, int userId, bool isCreatedByMaster);
    }
}