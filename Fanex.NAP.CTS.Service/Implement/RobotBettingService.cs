namespace Fanex.NAP.CTS.Service
{
    using Fanex.Data.Repository;
    using Fanex.NAP.CTS.Criteria.Account.RobotSetting;
    using Fanex.NAP.CTS.Model.Account.RobotBetting;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    public class RobotBettingService : IRobotBettingService
    {
        private readonly IDynamicRepository repository;

        public RobotBettingService()
        {
            repository = new DynamicRepository();
        }

        public IEnumerable<RobotBettingModel> GetRobotBetting(long custID)
        {
            var criteria = new GetRobotDetectionCriteria()
            {
                CustID = custID 
            };

            return repository.Fetch<RobotBettingModel>(criteria);
        }

        public IEnumerable<RobotUserDetailModel> GetRobotUserDetails(long custID, int periodRangeType)
        {
            var criteria = new GetRobotUsersDetailsCriteria
            {
                CustID = custID,
                PeriodRangeType = periodRangeType
            };

            return repository.Fetch<RobotUserDetailModel>(criteria);
        }

    }
}
