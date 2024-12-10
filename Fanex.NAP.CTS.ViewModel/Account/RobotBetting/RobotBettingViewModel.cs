namespace Fanex.NAP.CTS.ViewModel.Account.RobotBetting
{
    using Fanex.NAP.CTS.Model.Account.RobotBetting;
    using Fanex.NAP.CTS.Resource.Consts;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using static Fanex.NAP.CTS.Resource.Consts.Consts;

    public class RobotBettingViewModel
    {
        public RobotBettingViewModel(RobotBettingModel rbData)
        {
            var isRobotBettingAIValid = rbData?.RobotBettingAI?.Any() ?? false;
            var isRobotImpervasValid = rbData?.RobotBettingImperva?.Any() ?? false;
            var isFirstLastRobotDateValid = !rbData.FirstLastRobotDate.FirstRobotDate.Equals(DateTime.MinValue) &&
                                            !rbData.FirstLastRobotDate.LastRobotDate.Equals(DateTime.MinValue);

            RobotAILP = RobotBettingAI = isRobotBettingAIValid ? rbData.RobotBettingAI.Select(x => new RobotBettingAIViewModel
            {
                CustID = x.CustID,
                RangeType = x.PeriodRangeType,
                RangeName = x.PeriodRangeName
            }) : Enumerable.Empty<RobotBettingAIViewModel>();

            RobotBettingAI = isRobotBettingAIValid ? rbData.RobotBettingAI.Select(x => new RobotBettingAIViewModel
            {
                CustID = x.CustID,
                RangeType = x.PeriodRangeType,
                RangeName = x.PeriodRangeName
            }) : Enumerable.Empty<RobotBettingAIViewModel>();

            RobotImpervas = isRobotImpervasValid ? rbData.RobotBettingImperva.OrderBy(x => x.CreateTime).Select(x => new RobotImpervaViewModel
            {
                CustID = x.CustID,
                CreatedDate = x.CreateTime.ToString(Consts.FormatDateToString_ddMMMyyyy),
                PlatformName = x.Platform == 1 ? Platform.Desktop : Platform.Mobile
            }) : Enumerable.Empty<RobotImpervaViewModel>();

            FirstRobotDate = isFirstLastRobotDateValid ? rbData.FirstLastRobotDate.FirstRobotDate.ToString(FormatDatetimeToString_ddMMMyyyyHHmmssfff) : string.Empty;
            LastRobotDate = isFirstLastRobotDateValid ? rbData.FirstLastRobotDate.LastRobotDate.ToString(FormatDatetimeToString_ddMMMyyyyHHmmssfff) : string.Empty;
        }

        public string FirstRobotDate { get; set; }

        public string LastRobotDate { get; set; }

        public IEnumerable<RobotBettingAIViewModel> RobotAILP { get; set; }

        public IEnumerable<RobotBettingAIViewModel> RobotBettingAI { get; set; }

        public IEnumerable<RobotImpervaViewModel> RobotImpervas { get; set; }
    }
}
