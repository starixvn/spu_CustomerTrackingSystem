namespace Fanex.NAP.CTS.Service
{
    using Fanex.Data;
    using Fanex.Data.Repository;
    using Fanex.NAP.CTS.Core;
    using Fanex.NAP.CTS.Criteria.MatchMonitor;
    using Fanex.NAP.CTS.Model.MatchMonitor;
    using Fanex.NAP.CTS.Resource.Consts;
    using Fanex.NAP.CTS.ViewModel.MatchMonitor;
    using Fanex.NAP.TGRS.Core.ExcelUtility;
    using System;
    using System.Collections.Generic;
    using System.Linq;

    public class MatchMonitorService : IMatchMonitorService
    {
        private readonly IDynamicRepository repository;

        public MatchMonitorService()
        {
            repository = new DynamicRepository();
        }

        public IEnumerable<GetMatchMonitorModel> GetMatchMonitor(MatchMonitorFilterViewModel filter)
        {
            var criteria = new MatchMonitorGetCriteria
            {
                DateType = filter.DateType,
                FromDate = filter?.FromDate ?? DateTime.Now,
                ToDate = filter?.ToDate ?? DateTime.Now,
                SportType = filter.Sports,
                Market = filter.Market,
                MatchSection = filter.MatchSection,
                BetTypes = filter.BetTypes,
                IsVerified = filter.IsVerified,
                Reasons = filter.Reasons
            };

            return repository.Fetch<GetMatchMonitorModel>(criteria);
        }

        public DBResult VerifyTransMatchMonitorDetail(MatchMonitorDetailVerifyTransParaModel para)
        {
            var criteria = new MatchMonitorDetailVerifyCriteria
            {
                MatchID = para.MatchId,
                LiveIndicator = para.LiveIndicator,
                BettypeID = para.BettypeID,
                BetID = para.BetID,
                TranIds = para.TranIds,
                VerifiedBy = para.VerifiedBy                
            };

            repository.Fetch<DBResult>(criteria);

            return new DBResult { ErrorCode = 0, ErrorMsg = string.Empty };
        }

        public IEnumerable<MatchGroupModel> GetMatchGroup(Int64 matchID, int betTypeID, bool liveIndicator, Int64 betId, int reason)
        {
            var criteria = new MatchGroupGetCriteria()
            {
                BettypeID = betTypeID,
                BetID = betId,
                MatchID = matchID,
                LiveIndicator = liveIndicator,
                Reason = reason
            };

            return repository.Fetch<MatchGroupModel>(criteria);
        }

        public IEnumerable<MatchScoreModel> GetMatchScore(Int64 matchID, int betTypeID, bool liveIndicator, Int64 betId, int reason)
        {
            var criteria = new MatchScoreGetCriteria()
            {
                BettypeID = betTypeID,
                BetID = betId,
                MatchID = matchID,
                LiveIndicator = liveIndicator,
                Reason = reason
            };

            return repository.Fetch<MatchScoreModel>(criteria);
        }

        public MatchMonitorDetailResultModel GetMatchMonitorDetail(MatchMonitorDetailFilterViewModel filter)
        {
            var criteria = new MatchDetailMonitorGetCriteria
            {
                MatchID = filter.MatchID,
                LiveIndicator = filter.LiveIndicator,
                BettypeID = filter.BetTypeID,
                BetID = filter.BetID
            };

            using (var db = ObjectDbFactory.CreateInstance(criteria.GetSettingKey()))
            {
                var result = db.QueryMultiple(criteria);

                return new MatchMonitorDetailResultModel
                {
                    Custs = result?.Read<MMDetailCustModel>() ?? new List<MMDetailCustModel>(),
                    MatchDetails = result?.Read<MatchMonitorDetailModel>() ?? new List<MatchMonitorDetailModel>(),
                    VerifiedTrans = result?.Read<MatchMonitorDetailVerifyTransResultModel>() ?? new List<MatchMonitorDetailVerifyTransResultModel>()
                };
            }
        }

        public IEnumerable<CustInfoModel> GetCustInfoByCustID(string custIDs)
        {
            var criteria = new CustInfoByCustIdGetCriteria()
            {
                CustIDs = custIDs
            };

            return repository.Fetch<CustInfoModel>(criteria);
        }

        public IEnumerable<TicketDetailModel> GetTicketDetail(DateTime eventDate, string transIds)
        {
            var criteria = new TicketDetailGetCriteria()
            {
                QueryType = Consts.MatchMonitor_ReportQueryType,
                EventDate = eventDate,
                ListTransId = transIds
            };

            return repository.Fetch<TicketDetailModel>(criteria);
        }

        public IEnumerable<RobotStatusModel> GetRobotStatus(string transJson)
        {
            var criteria = new RobotStatusByTransGetCriteria()
            {
                TransJson = transJson
            };

            return repository.Fetch<RobotStatusModel>(criteria);
        }
       
        public MatchMonitorStaticListModel GetMMStaticList()
        {
            var criteria = new GetMatchMonitorStaticListCriteria();

            var result = repository.FetchMultiple<MatchMonitorReasonModel, MatchMonitorSportModel, MatchMonitorBetTypeModel>(criteria);

            return new MatchMonitorStaticListModel
            {
                Reasons = result.firstResultSet,
                Sports = result.secondResultSet,
                BetTypes = result.thirdResultSet
            };
        }

        public byte[] ExportExcelMMDetail(MatchMonitorDetailListViewModel mmDetail, string fileName, string templatePath)
        {
            var writer = new ExcelTemplateWriter(templatePath, fileName, "Sheet1");
            writer.CopyRows(0, 1);

            FillRowData(mmDetail, ref writer);

            writer.SyncColumnsWidth(31);

            return writer.ToBytes();
        }

        private void FillRowData(MatchMonitorDetailListViewModel mmDetail, ref ExcelTemplateWriter writer)
        {
            var tblHome = mmDetail.HomeMatchItemList.ToArray();
            var tblAway = mmDetail.AwayMatchItemList.ToArray();
            var lengthH = tblHome.Length;
            var lengthA = tblAway.Length;

            if (lengthH > lengthA)
            {
                for (int i = 0; i < lengthH; i++)
                {                   
                    if (i < lengthA)
                    {                        
                        writer.WriteRow(1, NewRecordTblHomeAway(tblHome[i], tblAway[i]));
                    }
                    else
                    {
                        writer.WriteRow(1, NewRecordTblHome(tblHome[i]));
                    }
                }
            }
            else
            {
                for (int i = 0; i < lengthA; i++)
                {
                    if (i < lengthH)
                    {                       
                        writer.WriteRow(1, NewRecordTblHomeAway(tblHome[i], tblAway[i]));
                    }
                    else
                    {
                        writer.WriteRow(1, NewRecordTblAway(tblAway[i]));
                    }
                }
            }
        }

        private object[] NewRecordTblHome(MatchMonitorDetailItemViewModel itemH)
        {
            return new object[]
                {
                    itemH.SubscriberType,
                    itemH.Username,
                    itemH.CustomerClass,
                    (itemH.IsRobot ? "x" : "-"),
                    itemH.DangerToString,
                    itemH.Odds,
                    itemH.BetID.ToString(),
                    itemH.Currency,
                    itemH.Amount,
                    itemH.HDP,
                    itemH.Score,
                    itemH.TransTimeToString,
                    itemH.Group,
                    itemH.ReasonList,
                    (!string.IsNullOrEmpty(itemH.PICVerified) ? "x" : "-")
                };
        }

        private object[] NewRecordTblAway(MatchMonitorDetailItemViewModel itemA)
        {
            return new object[]
                {
                    "", "", "",
                    "", "", "",
                    "", "", "",
                    "", "", "",
                    "", "", "", "",
                    itemA.SubscriberType,
                    itemA.Username,
                    itemA.CustomerClass,
                    (itemA.IsRobot ? "x" : "-"),
                    itemA.DangerToString,
                    itemA.Odds,
                    itemA.BetID.ToString(),
                    itemA.Currency,
                    itemA.Amount,
                    itemA.HDP,
                    itemA.Score,
                    itemA.TransTimeToString,
                    itemA.Group,
                    itemA.ReasonList,
                    (!string.IsNullOrEmpty(itemA.PICVerified) ? "x" : "-")
                };
        }

        private object[] NewRecordTblHomeAway(MatchMonitorDetailItemViewModel itemH, MatchMonitorDetailItemViewModel itemA)
        {
            return new object[]
                {
                    itemH.SubscriberType,                    
                    itemH.Username,
                    itemH.CustomerClass,
                    (itemH.IsRobot ? "x" : "-"),
                    itemH.DangerToString,
                    itemH.Odds,
                    itemH.BetID.ToString(),
                    itemH.Currency,
                    itemH.Amount,
                    itemH.HDP,
                    itemH.Score,
                    itemH.TransTimeToString,
                    itemH.Group,
                    itemH.ReasonList,                    
                    (!string.IsNullOrEmpty(itemH.PICVerified) ? "x" : "-"),
                    "",
                    itemA.SubscriberType,
                    itemA.Username,
                    itemA.CustomerClass,
                    (itemA.IsRobot ? "x" : "-"),
                    itemA.DangerToString,
                    itemA.Odds,
                    itemA.BetID.ToString(),
                    itemA.Currency,
                    itemA.Amount,
                    itemA.HDP,
                    itemA.Score,
                    itemA.TransTimeToString,
                    itemA.Group,
                    itemA.ReasonList,
                    (!string.IsNullOrEmpty(itemA.PICVerified) ? "x" : "-")
                };
        }

    }
}