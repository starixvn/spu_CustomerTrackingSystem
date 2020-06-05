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
        IEnumerable<AddEvidenceModel> ParseFileXls(Stream fileContent);

        IEnumerable<AddEvidenceModel> ParseFileXlsx(Stream fileContent);

        IEnumerable<RuleCheckViewModel> CheckRule(IEnumerable<AddEvidenceModel> addEvidenceList);

        DBResult AddEvidenceList(IEnumerable<AddEvidenceModel> evidenceList);
    }
}