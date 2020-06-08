namespace Fanex.NAP.CTS.ViewModel.ImportEvidences
{
    using System.Collections.Generic;

    public class ResultImportViewModel
    {
        public string FileName { get; set; }

        public IEnumerable<ResultCheckAccountEvidenceViewModel> ResultCheckAccountEvidenceList { get; set; }
    }
}