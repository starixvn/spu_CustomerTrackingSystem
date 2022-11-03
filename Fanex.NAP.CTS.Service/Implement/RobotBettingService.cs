namespace Fanex.NAP.CTS.Service
{
    using Fanex.Data.Repository;
    using Fanex.NAP.CTS.Criteria.Account.RobotSetting;
    using Fanex.NAP.CTS.Model.Account.RobotBetting;
    using Fanex.NAP.CTS.Resource.Consts;
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;
    using static Fanex.NAP.CTS.Resource.Consts.Consts;

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

        public IEnumerable<RobotUserDetailModel> GetRobotDetectionDetails(long custID, int periodRangeType)
        {
            var criteria = new GetRobotUsersDetailsCriteria
            {
                CustID = custID,
                PeriodRangeType = periodRangeType
            };

            return repository.Fetch<RobotUserDetailModel>(criteria);
        }

        public IEnumerable<RobotUserDetailSceModel> GetRobotDetectionSCEDetails(long custID, int periodRangeType, RobotDetectionQueryType queryType, int? clusterId = null, DateTime? scannedTime = null)
        {
            var criteria = new GetRobotUsersSCEDetailsCriteria
            {
                CustID = custID,
                RobotType = periodRangeType,
                QueryType = (int)queryType,
                ScannedDate = scannedTime,
                ClusterID = clusterId
            };

            return repository.Fetch<RobotUserDetailSceModel>(criteria);
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

        public IEnumerable<string> GetTranIds(long custID, int periodRangeType, int clusterId, string createdTime)
        {
            bool isSCERobotDetection = (periodRangeType == (int)RobotDetectionReasonQueryType.ConsecutiveTicket || periodRangeType == (int)RobotDetectionReasonQueryType.UglyStakeTicket);

            if (isSCERobotDetection)
            {
                var datetime = DateTime.ParseExact(createdTime, FormatDateTimeToString_YYYYmmdd, CultureInfo.InvariantCulture);
                var robotUserDetailsSCE = GetRobotDetectionSCEDetails(custID, periodRangeType, RobotDetectionQueryType.RobotTicketDetail, clusterId, datetime);

                if (robotUserDetailsSCE != null && robotUserDetailsSCE.Any())
                {
                    return robotUserDetailsSCE.Select(c => c.TransList);
                }
            }
            else
            {
                var robotUserDetails = GetRobotDetectionDetails(custID, periodRangeType);

                if (robotUserDetails != null && robotUserDetails.Any())
                {
                    return robotUserDetails.Select(c => c.SuspiciousTransList);
                }
            }

            return Enumerable.Empty<string>();
        }


        public string GetTransFromList(IEnumerable<string> suspiciousTransList)
        {
            var tranIdList = new List<string>();

            foreach (var level1 in suspiciousTransList)
            {
                var susTranIdsLevel1 = level1.Split(',');

                foreach (var level2 in susTranIdsLevel1)
                {
                    var susTranIdsLevel2 = level2.Split('#');

                    if (susTranIdsLevel2.Length > 1)
                    {
                        tranIdList.AddRange(susTranIdsLevel2);
                    }
                    else
                    {
                        tranIdList.Add(level2);
                    }
                }
            }

            var tranIds = string.Join(",", tranIdList.Distinct());

            return tranIds.Replace(@".0", "");
        }
    }
}
