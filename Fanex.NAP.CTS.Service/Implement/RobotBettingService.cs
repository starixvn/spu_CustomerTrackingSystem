namespace Fanex.NAP.CTS.Service
{
    using Fanex.Data;
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

        public RobotBettingModel GetRobotBetting(long custID)
        {
            var criteria = new GetRobotDetectionCriteria
            {
                CustID = custID
            };
           
            using (var db = ObjectDbFactory.CreateInstance(criteria.GetSettingKey()))
            {
                using (var result = db.QueryMultiple(criteria))
                {
                    var robotAIs = result.Read<RobotBettingAIModel>();
                    var robotImpervas = result.Read<RobotBettingImpervaModel>();
                    var typeModel = new RobotDateAndTypeModel
                    {
                        FirstRobotDate = criteria.FirstRobotDate,
                        LastRobotDate = criteria.LastRobotDate,
                    };

                    return new RobotBettingModel
                    {
                        TypeModel = typeModel,
                        RobotBettingAI = robotAIs,
                        RobotBettingImperva = robotImpervas
                    };                   
                }
            }
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

        public IEnumerable<RobotUserDetailSceModel> GetRobotDetectionSCEDetails(RobotBettingSceDetailParaModel parameters)
        {
            var criteria = new GetRobotUsersSCEDetailsCriteria
            {
                CustID = parameters.CustId,
                RobotTypeOrigin = parameters.RobotTypeOrigin,
                RobotTypeDisplay = parameters.RobotTypeDisplay,
                QueryType = (int)parameters.QueryType,
                ScannedDate = parameters.ScannedTime,
                ClusterID = parameters.ClusterId
            };

            return repository.Fetch<RobotUserDetailSceModel>(criteria);
        }

        public IEnumerable<RobotBettingTicketDetailModel> GetRobotBettingTicketDetails(string tranIdList, bool isSCETicketDetails)
        {
            if (!string.IsNullOrEmpty(tranIdList))
            {
                var criteria = new GetRobotDetectionTicketDetailsCriteria
                {
                    ListTransId = tranIdList,
                    MatchBatchSize = isSCETicketDetails ? 1000 : 50
                };

                return repository.Fetch<RobotBettingTicketDetailModel>(criteria);
            }
            else
            {
                return Enumerable.Empty<RobotBettingTicketDetailModel>();
            }
        }

        public IEnumerable<string> GetTranIds(long custID, int robotTypeOrigin, int? robotTypeDisplay, int? clusterId, string createdTime)
        {
            bool isSCERobotDetection = (robotTypeOrigin == (int)RobotDetectionReasonQueryType.ConsecutiveTicket || robotTypeOrigin == (int)RobotDetectionReasonQueryType.UglyStakeTicket);

            if (isSCERobotDetection)
            {
                var inputPara = new RobotBettingSceDetailParaModel
                {
                    CustId = custID,
                    ClusterId = clusterId,
                    QueryType = RobotDetectionQueryType.RobotTicketDetail,
                    RobotTypeDisplay = robotTypeDisplay.GetValueOrDefault(),
                    RobotTypeOrigin = robotTypeOrigin,
                    ScannedTime = DateTime.ParseExact(createdTime, FormatDateTimeToString_YYYYmmdd, CultureInfo.InvariantCulture)
                };

                var robotUserDetailsSCE = GetRobotDetectionSCEDetails(inputPara);

                if (robotUserDetailsSCE != null && robotUserDetailsSCE.Any())
                {
                    return robotUserDetailsSCE.Select(c => c.TransList);
                }
            }
            else
            {
                var robotUserDetails = GetRobotDetectionDetails(custID, robotTypeOrigin);

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
