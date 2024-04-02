namespace Fanex.NAP.CTS.ViewModel.Account
{
    public class EvidenceViewModel
    {
        public EvidenceViewModel(FlaggedEvidenceViewModel data)
        {
            this.EvidenceID = data.EvidenceID;
            this.EvidenceCode = data.EvidenceCode;
            this.EvidenceDisplay = data.EvidenceCode + " " + data.EvidenceName;
        }

        public EvidenceViewModel(AffectedRetractEvidenceViewModel data)
        {
            this.EvidenceID = data.EvidenceID;
            this.EvidenceCode = data.EvidenceCode;
            this.EvidenceDisplay = data.EvidenceCode + " " + data.EvidenceName;
        }

        public int EvidenceID { get; set; }

        public string EvidenceCode { get; set; }        

        public string EvidenceDisplay { get; set; }
    }
}