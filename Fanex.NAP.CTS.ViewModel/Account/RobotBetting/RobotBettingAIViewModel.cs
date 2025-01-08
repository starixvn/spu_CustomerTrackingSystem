using Fanex.NAP.CTS.Model.Account.RobotBetting;

namespace Fanex.NAP.CTS.ViewModel.Account.RobotBetting
{
    public class RobotBettingAIViewModel
    {
        public RobotBettingAIViewModel() { }

        public RobotBettingAIViewModel(RobotBettingAIModel data)
        {
            CustID = data.CustID;
            RangeType = data.PeriodRangeType;
            RangeName = data.PeriodRangeName;
        }

        public long CustID { get; set; }

        public int RangeType { get; set; }

        public string RangeName { get; set; }
    }
}
