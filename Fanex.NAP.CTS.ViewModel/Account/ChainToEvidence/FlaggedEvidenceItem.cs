namespace Fanex.NAP.CTS.ViewModel.Account
{
    using Fanex.NAP.CTS.Model.Account;
    using System;

    [Serializable]
    public class FlaggedEvidenceItem
    {
        public FlaggedEvidenceItem() { }

        public FlaggedEvidenceItem(FlaggedEvidenceModel flaggedEvidenceModel)
        {
            if (flaggedEvidenceModel != null)
            {
                this.EvidenceID = flaggedEvidenceModel.EvidenceID;
                this.EvidenceCode = flaggedEvidenceModel.EvidenceCode;
                this.EvidenceName = flaggedEvidenceModel.EvidenceName;
                this.FlaggedDate = flaggedEvidenceModel.CreatedDate;
                this.Comment = flaggedEvidenceModel.Remark;
                this.Creator = flaggedEvidenceModel.UserName;
            }
        }

        public int EvidenceID { get; set; }

        public string EvidenceCode { get; set; }

        public string EvidenceName { get; set; }

        public string EvidenceDisplay { get { return this.EvidenceCode + " " + this.EvidenceName; } }

        public DateTime FlaggedDate { get; set; }        

        public string Comment { get; set; }

        public string Creator { get; set; }
    }
}