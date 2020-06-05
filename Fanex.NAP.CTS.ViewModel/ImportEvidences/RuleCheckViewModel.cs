namespace Fanex.NAP.CTS.ViewModel.ImportEvidences
{
    public class RuleCheckViewModel
    {
        public int RowIndex { get; set; }

        public string UserName { get; set; }

        public string EvidenceCode { get; set; }

        public string Comment { get; set; }

        public bool IsViolate { get; set; }

        public string ReasonViolate { get; set; }

    }
}