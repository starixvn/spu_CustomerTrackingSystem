namespace Fanex.NAP.CTS.Service
{
    using Fanex.Data.Repository;
    using Fanex.NAP.CTS.Criteria.Account.RobotSetting;
    using Fanex.NAP.CTS.Model.Account.RobotBetting;
    using Fanex.NAP.CTS.Model.MatchMonitor;
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

        public IEnumerable<RobotBettingModel> GetRobotBetting(long custID, out RobotDateAndTypeModel robotDateAndType)
        {          
            var criteria = new GetRobotDetectionCriteria()
            {
                CustID = custID 
            };

            var result = repository.Fetch<RobotBettingModel>(criteria);

            robotDateAndType = new RobotDateAndTypeModel
            {
                FirstRobotDate = criteria.FirstRobotDate,
                LastRobotDate = criteria.LastRobotDate,
                RobotTypeClassify = criteria.RobotTypeClassify
            };

            return result;
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

        public IEnumerable<RobotBettingTicketDetailModel> GetRobotBettingTicketDetails(string tranIdList)
        {
            if (!string.IsNullOrEmpty(tranIdList))
            {
                var criteria = new GetRobotDetectionTicketDetailsCriteria
                {
                    ListTransId = tranIdList
                };

                return repository.Fetch<RobotBettingTicketDetailModel>(criteria);
            }
            else
            {
                return Enumerable.Empty<RobotBettingTicketDetailModel>();
            }
        }

    }
}
