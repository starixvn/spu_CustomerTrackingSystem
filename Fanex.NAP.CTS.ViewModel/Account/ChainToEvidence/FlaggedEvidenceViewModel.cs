namespace Fanex.NAP.CTS.ViewModel.Account
{
    using Fanex.NAP.CTS.Model.Account;
    using Fanex.NAP.CTS.Resource.Consts;
    using System;

    [Serializable]
    public class FlaggedEvidenceViewModel
    {
        public FlaggedEvidenceViewModel(FlaggedEvidenceModel data)
        {
            this.EvidenceID = data.EvidenceID;
            this.EvidenceCode = data.EvidenceCode;
            this.EvidenceName = data.EvidenceName;
            this.EvidenceDisplay = data.EvidenceCode + " " + data.EvidenceName;
            this.Comment = data.Remark;
            this.Creator = data.Creator;
            this.FlaggedDate = data.CreatedDate.ToString(Consts.FormatDateToString_MMMddyyyyhhmmss);
        }

        public int EvidenceID { get; set; }

        public string EvidenceCode { get; set; }

        public string EvidenceName { get; set; }

        public string EvidenceDisplay { get; set; }

        public string FlaggedDate { get; set; }

        public string Comment { get; set; }

        public string Creator { get; set; }
    }
}