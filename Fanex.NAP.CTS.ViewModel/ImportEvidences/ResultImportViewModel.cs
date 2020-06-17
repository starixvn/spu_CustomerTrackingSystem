namespace Fanex.NAP.CTS.ViewModel.ImportEvidences
{
    using System.Collections.Generic;

    public class ResultImportViewModel
    {
        public string FileName { get; set; }

        public IEnumerable<ResultCheckAccountEvidenceViewModel> ResultCheckAccountEvidenceList { get; set; }

        public int ErrorCode { get; set; }

        public string ErrorMessage { get; set; }
    }
}
