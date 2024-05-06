namespace Fanex.NAP.CTS.ViewModel.Account
{
    using Fanex.NAP.CTS.Model.Account;
    using Fanex.NAP.CTS.Resource.Consts;
    using System;    

    [Serializable]
    public class AffectedRetractEvidenceViewModel
    {
        public AffectedRetractEvidenceViewModel(AffectedEvidenceModel data, bool isRetract)
        {
            this.EvidenceID = data.EvidenceID;
            this.EvidenceCode = data.EvidenceCode;
            this.EvidenceName = data.EvidenceName;
            this.EvidenceDisplay = data.EvidenceCode + " " + data.EvidenceName;
            this.LoginID = string.IsNullOrEmpty(data.LoginID) ? data.UserName : data.LoginID;
            this.CTSCustID = data.CTSCustID;
            this.UserName = string.IsNullOrEmpty(data.UserName) ? data.LoginID : data.UserName;
            this.AssociationType = data.AssociationType;
            this.AffectedTime = data.CreatedDate.ToString(Consts.FormatDateToString_MMMddyyyyhhmmss);
            this.LastLoginTime = data.LastLoginTime.Year != 0001 ? data.LastLoginTime.ToString(Consts.FormatDateToString_MMMddyyyyhhmmss) : "None";
            this.IsRetract = isRetract;
            this.Creator = data.Creator;
            this.Comment = string.IsNullOrWhiteSpace(data.Remark) ? string.Empty : data.Remark;
            /*
              {
                    EvidenceID = x.EvidenceID,
                    EvidenceCode = x.EvidenceCode,
                    EvidenceName = x.EvidenceName,
                    EvidenceDisplay = x.EvidenceCode + " " + x.EvidenceName,
                    LoginID = string.IsNullOrEmpty(x.LoginID) ? x.UserName : x.LoginID,
                    CTSCustID = x.CTSCustID,
                    UserName = string.IsNullOrEmpty(x.UserName) ? x.LoginID : x.UserName,
                    AssociationType = x.AssociationType,
                    AffectedTime = x.CreatedDate.ToString(Consts.FormatDateToString_MMMddyyyyhhmmss),
                    LastLoginTime = x.LastLoginTime.ToString(Consts.FormatDateToString_MMMddyyyyhhmmss),
                    Creator = x.Creator,
                    Comment = string.IsNullOrWhiteSpace(x.Remark) ? string.Empty : x.Remark,
                    IsRetract = true
                }
             
             */
        }

        public int EvidenceID { get; set; }

        public string EvidenceCode { get; set; }

        public string EvidenceName { get; set; }

        public string EvidenceDisplay { get; set; }

        public string LoginID { get; set; }

        public Int64 CTSCustID { get; set; }

        public string UserName { get; set; }

        public string AssociationType { get; set; }

        public string AffectedTime { get; set; }

        public string LastLoginTime { get; set; }

        public string Creator { get; set; }

        public string Comment { get; set; }

        public bool IsRetract { get; set; }

    }
}