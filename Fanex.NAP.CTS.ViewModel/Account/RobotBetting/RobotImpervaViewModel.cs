namespace Fanex.NAP.CTS.ViewModel.Account.RobotBetting
{
    using Fanex.NAP.CTS.Model.Account.RobotBetting;
    using static Fanex.NAP.CTS.Resource.Consts.Consts;

    public class RobotImpervaViewModel
    {
        public RobotImpervaViewModel() { }       

        public RobotImpervaViewModel(RobotBettingImpervaModel data)
        {
            CustID = data.CustID;
            CreatedDate = data.CreateTime.ToString(FormatDateToString_ddMMMyyyy);
            PlatformName = data.Platform == 1 ? Platform.Desktop : Platform.Mobile;
        }

        public long CustID { get; set; }

        public string CreatedDate { get; set; }        

        public string PlatformName { get; set; }

    }

}
