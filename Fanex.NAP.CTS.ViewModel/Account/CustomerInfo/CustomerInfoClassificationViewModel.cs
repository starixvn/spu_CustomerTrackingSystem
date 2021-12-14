namespace Fanex.NAP.CTS.ViewModel.Account.CustomerInfo
{
    public class CustomerInfoClassificationViewModel
    {
        public int Class { get; set; }        

        public string FlagEvidenceCodes { get; set; }

        public string AffectedEvidenceCodes { get; set; }

        public bool IsVIP { get; set; }

        public string LicVIP
        {
            get
            {
                return IsVIP ? "Yes" : "No";
            }
        }
    }
}
