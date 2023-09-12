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
    using Newtonsoft.Json;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using static Fanex.NAP.CTS.Resource.Consts.Consts;

    public class MatchMonitorService : IMatchMonitorService
    {
        private readonly IDynamicRepository repository;

        public MatchMonitorService()
        {
            repository = new DynamicRepository();
        }

        public IEnumerable<MatchFromMainInfoModel> GetMatchFromMainInfos(MatchMonitorSearchViewModel filter, string matchIDs)
        {
            var criteria = new GetMatchFromMainInfoCriteria
            {
                MatchIDs = matchIDs,
                FromDate = filter?.FromDate ?? DateTime.Now,
                ToDate = filter?.ToDate ?? DateTime.Now,
                MatchSection = filter.MatchSection
            };

            return repository.Fetch<MatchFromMainInfoModel>(criteria);
        }

        public IEnumerable<GetMatchMonitorModel> GetMatchMonitor(MatchMonitorSearchViewModel filter, string matchInfoFromMainJson)
        {
            var criteria = new MatchMonitorGetCriteria
            {
                FromDate = filter?.FromDate ?? DateTime.Now,
                ToDate = filter?.ToDate ?? DateTime.Now,
                SportType = filter.Sports,
                Market = filter.Market,
                BetTypes = filter.BetTypes,
                IsVerified = filter.IsVerified,
                Reasons = filter.Reasons,
                MatchInfosJson = matchInfoFromMainJson
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

        public MMDetailResultModel GetMatchMonitorDetail(MMDetailInfoViewModel filter)
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

                return new MMDetailResultModel
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

        public IEnumerable<TicketDetailModel> GetFraudTicketDetail(MMDetailInfoViewModel para, string transIds)
        {            
            var criteria = new TicketDetailGetCriteria()
            {
                QueryType = (int)MatchMonitorQueryType.TicketFraud,
                EventDate = para.EventDate,
                ListTransId = transIds,
                MatchID = para.MatchID,
                BetType = para.BetTypeID,
                BetID = para.BetID,
                IsLive = para.LiveIndicator ? 1 : 0
            };

            return repository.Fetch<TicketDetailModel>(criteria);
        }

        public TicketDetailViewModel GetAllTicketDetail(MMDetailInfoViewModel para, MMDetailFilterViewModel filter, int batchSize = 1000, string transIds = null)
        {                                   
            var scoreList = (filter.ScoreList == null || !para.LiveIndicator) ? null : string.Join(",", filter.ScoreList);

            var criteria = new TicketDetailGetCriteria()
            {
                QueryType = (int)MatchMonitorQueryType.TicketAll,
                EventDate = para.EventDate,
                LastScannedSequenceID = para.LastScannedSequenceID,
                IsLive = para.LiveIndicator ? 1 : 0,
                MatchID = para.MatchID,
                BetType = para.BetTypeID,
                BetID = para.BetID,
                BatchSize = batchSize,
                IsLicensee = para.Sites == MMSites.All ? null : (int?)(para.Sites == MMSites.Licensee ? 1 : 0),
                ListCurrency = filter?.CurrencyIdList == null ? null : string.Join(",", filter.CurrencyIdList),              
                ListScore = scoreList,
                ListDanger = GenerateDangerJson(filter),
                ListStatus = filter?.TicketStatusList == null ? null : string.Join(",", filter.TicketStatusList),
                ListTransId = transIds
            };

            if (para.BetTypeID == (int)MatchMonitorBetType.TotalGoal
                || para.BetTypeID == (int)MatchMonitorBetType.FirstTotalGoal
                || para.BetTypeID == (int)MatchMonitorBetType.CorrectScore
                || para.BetTypeID == (int)MatchMonitorBetType.FirstCorrectScore)
            {                
                criteria.ListHDPBetTeam = filter?.BetTeamList == null ? null : string.Join(",", filter.BetTeamList);
            }
            else
            {
                criteria.ListHDPBetTeam = filter?.HDPList == null ? null : string.Join(",", filter.HDPList);
            }

            if (filter?.AmountList?.Count >= 2)
            {
                criteria.FromStakeRM = filter.AmountList[0];
                criteria.ToStakeRM = filter.AmountList[1];
            }           

            var result = repository.Fetch<TicketDetailModel>(criteria);

            return new TicketDetailViewModel
            {
                TicketDetails = result,
                MinSequenceIDTotalAll = criteria.MinSequenceIDTotalAll
            };
        }

        private string GenerateDangerJson(MMDetailFilterViewModel filter)
        {           
            var jsonDanger = new List<MMDetailDangerModel>();

            if (filter.IsDangerNone)
                jsonDanger.Add(new MMDetailDangerModel { Danger1 = 0, Danger2 = 0, Danger3 = 0, Danger4 = 0, Danger5 = 0 });
            filter.Danger1List?.ForEach(x => { jsonDanger.Add(new MMDetailDangerModel { Danger1 = x }); });
            filter.Danger2List?.ForEach(x => { jsonDanger.Add(new MMDetailDangerModel { Danger2 = x }); });
            filter.Danger3List?.ForEach(x => { jsonDanger.Add(new MMDetailDangerModel { Danger3 = x }); });
            filter.Danger4List?.ForEach(x => { jsonDanger.Add(new MMDetailDangerModel { Danger4 = x }); });
            filter.Danger5List?.ForEach(x => { jsonDanger.Add(new MMDetailDangerModel { Danger5 = x }); });

            return JsonConvert.SerializeObject(jsonDanger);
        }
       
        public MMTicketDetailInfoModel GetMMTicketDetailInfo(MMDetailInfoViewModel para, IEnumerable<string> transIds)
        {
            var criteria = new MMTicketDetailInfoCriteria()
            {
                MatchID = para.MatchID,
                BetType = para.BetTypeID,
                BetID = para.BetID,
                IsLive = para.LiveIndicator ? 1 : 0,
                //ListTransId = string.Join(",", transIds)
                ListSuspiciousTransId = JsonConvert.SerializeObject(transIds)
            };
            
            var result = repository.Fetch<MMDetailFilterTicketInfoModel>(criteria);

            return new MMTicketDetailInfoModel
            {
                MaxSequenceId = criteria.MaxSequenceId,
                TotalTicketAll = criteria.TotalTicketAll,
                TotalTicketCredit = criteria.TotalTicketCredit,
                TotalTicketLicensee = criteria.TotalTicketLicensee,
                TicketInfoList = result               
            };
        }

        public IEnumerable<MMDetailMergeGroupModel> GetMMDetailMergeGroup(IEnumerable<MMDetailBetTeamModel> mmDetailBetTeams)
        {
            var transList = JsonConvert.SerializeObject(mmDetailBetTeams);

            var criteria = new MMDetailMergeGroupCriteria
            {
                TransList = transList
            };

            return repository.Fetch<MMDetailMergeGroupModel>(criteria);
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

        public byte[] ExportExcelMMDetail(IEnumerable<ExExcelTicketInfoViewModel> homeMItems, IEnumerable<ExExcelTicketInfoViewModel> awayMItems
            , string fileName, string templatePath)
        {
            var writer = new ExcelTemplateWriter(templatePath, fileName, "Sheet1");
            writer.CopyRows(0, 1);

            FillRowData(homeMItems, awayMItems, ref writer);

            writer.SyncColumnsWidth(33);

            return writer.ToBytes();
        }

        private void FillRowData(IEnumerable<ExExcelTicketInfoViewModel> homeMItems, IEnumerable<ExExcelTicketInfoViewModel> awayMItems, ref ExcelTemplateWriter writer)
        {
            var tblHome = homeMItems != null ? homeMItems.ToArray() : new ExExcelTicketInfoViewModel[0];
            var tblAway = awayMItems != null ? awayMItems.ToArray() : new ExExcelTicketInfoViewModel[0];
            var lengthH = tblHome.Length;
            var lengthA = tblAway.Length;

            if (lengthH > lengthA)
            {
                for (int i = 0; i < lengthH; i++)
                {
                    if (i < lengthA)
                        writer.WriteRow(1, NewRecordTblHomeAway(tblHome[i], tblAway[i]));
                    else
                        writer.WriteRow(1, NewRecordTblHome(tblHome[i]));
                }
            }
            else
            {
                for (int i = 0; i < lengthA; i++)
                {
                    if (i < lengthH)
                        writer.WriteRow(1, NewRecordTblHomeAway(tblHome[i], tblAway[i]));
                    else
                        writer.WriteRow(1, NewRecordTblAway(tblAway[i]));
                }
            }
        }

        private object[] NewRecordTblHome(ExExcelTicketInfoViewModel itemH)
        {
            return new object[]
                {
                    itemH.SubscriberType,
                    itemH.Username,
                    itemH.CustomerClass == -1 ? "-" : itemH.CustomerClass.ToString(),
                    (itemH.IsRobot ? "x" : "-"),
                    itemH.DangerToString,
                    itemH.Odds,
                    itemH.BetID.ToString(),
                    itemH.Currency,
                    (itemH.Amount % 1 == 0 ? itemH.Amount.ToString(Consts.FormatDecimalToString_Thousands) : itemH.Amount.ToString()),
                    itemH.HDP,
                    itemH.Score,
                    itemH.TransTimeToString,
                    itemH.BetFrom,
                    itemH.ADetectGroup,
                    itemH.MDetectGroup,
                    itemH.ReasonList,
                    (!string.IsNullOrEmpty(itemH.PICVerified) ? "x" : "-"),
                    null, null, null, null,
                    null, null, null,
                    null, null, null,
                    null, null, null, null,
                    null, null, null, null
                };
        }

        private object[] NewRecordTblAway(ExExcelTicketInfoViewModel itemA)
        {
            return new object[]
                {
                    null, null, null, null,
                    null, null, null,
                    null, null, null,
                    null, null, null, null,
                    null, null, null, null,
                    itemA.SubscriberType,
                    itemA.Username,
                    itemA.CustomerClass == -1 ? "-" : itemA.CustomerClass.ToString(),
                    (itemA.IsRobot ? "x" : "-"),
                    itemA.DangerToString,
                    itemA.Odds,
                    itemA.BetID.ToString(),
                    itemA.Currency,
                    (itemA.Amount % 1 == 0 ? itemA.Amount.ToString(Consts.FormatDecimalToString_Thousands) : itemA.Amount.ToString()),
                    itemA.HDP,
                    itemA.Score,
                    itemA.TransTimeToString,
                    itemA.BetFrom,
                    itemA.ADetectGroup,
                    itemA.MDetectGroup,
                    itemA.ReasonList,
                    (!string.IsNullOrEmpty(itemA.PICVerified) ? "x" : "-")
                };
        }

        private object[] NewRecordTblHomeAway(ExExcelTicketInfoViewModel itemH, ExExcelTicketInfoViewModel itemA)
        {
            return new object[]
                {
                    itemH.SubscriberType,
                    itemH.Username,
                    itemH.CustomerClass == -1 ? "-" : itemH.CustomerClass.ToString(),
                    (itemH.IsRobot ? "x" : "-"),
                    itemH.DangerToString,
                    itemH.Odds,
                    itemH.BetID.ToString(),
                    itemH.Currency,
                    (itemH.Amount % 1 == 0 ? itemH.Amount.ToString(Consts.FormatDecimalToString_Thousands) : itemH.Amount.ToString()),
                    itemH.HDP,
                    itemH.Score,
                    itemH.TransTimeToString,
                    itemH.BetFrom,
                    itemH.ADetectGroup,
                    itemH.MDetectGroup,
                    itemH.ReasonList,
                    (!string.IsNullOrEmpty(itemH.PICVerified) ? "x" : "-"),
                    "",
                    itemA.SubscriberType,
                    itemA.Username,
                    itemA.CustomerClass == -1 ? "-" : itemA.CustomerClass.ToString(),
                    (itemA.IsRobot ? "x" : "-"),
                    itemA.DangerToString,
                    itemA.Odds,
                    itemA.BetID.ToString(),
                    itemA.Currency,
                    (itemA.Amount % 1 == 0 ? itemA.Amount.ToString(Consts.FormatDecimalToString_Thousands) : itemA.Amount.ToString()),
                    itemA.HDP,
                    itemA.Score,
                    itemA.TransTimeToString,
                    itemA.BetFrom,
                    itemA.ADetectGroup,
                    itemA.MDetectGroup,
                    itemA.ReasonList,
                    (!string.IsNullOrEmpty(itemA.PICVerified) ? "x" : "-")
                };
        }

    }
}