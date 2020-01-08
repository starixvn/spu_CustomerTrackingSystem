namespace Fanex.NAP.CTS.Model.Account
{
    public class CustomerGeneralStatusModel
    {
        public int CustId { get; set; }

        public string Username { get; set; }

        /// <summary>
        /// 1: Inactive, 2: Suspended, 3: ViewOnly, 4: Active
        /// -1: customer isn't CN88
        /// </summary>
        public int CN88Status { get; set; }

        /// <summary>
        /// suspend (reject=true) only for MBC
        /// </summary>
        public bool Reject { get; set; }

        /// <summary>
        /// CN88: locked (closed=1) || MBC: disabled (closed=1) active (closed=0)
        /// </summary>
        public bool Closed { get; set; }

        public int Danger { get; set; }

        public int Danger2 { get; set; }

        public int Danger3 { get; set; }

        public bool Autosuspend { get; set; }

        public bool Autosuspend2 { get; set; }

        public int Abnormal { get; set; }

        public bool ProblemAccountWithoutBetLimit { get; set; }

        public bool ProblemAccountWithBetLimit { get; set; }

        public bool BetRestrictedAccount { get; set; }

        public bool BetRestrictedCurrency { get; set; }

        public bool BetRestrictedSite { get; set; }

        public bool BetRestrictedSiteAndCurrency { get; set; }

        public bool SmartPunter { get; set; }

        public int IsLicensee { get; set; }

        public bool Enabled { get; set; }
    }
}
