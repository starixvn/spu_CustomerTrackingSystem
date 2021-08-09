namespace Fanex.NAP.CTS.Model.Account
{
    public class CustomerGeneralInfoModel
    {
        public int CustId { get; set; }

        public string Username { get; set; }

        public bool Autosuspend { get; set; }

        public bool Autosuspend2 { get; set; }

        public bool IsLicensee { get; set; }
    }
}
