namespace Fanex.NAP.CTS.Service
{
    using Fanex.NAP.CTS.Core.Extentions;
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
    using Fanex.Logging;

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

        private IEnumerable<CustInfoModel> GetCustInfoByCustID(IEnumerable<long> custIds)
        {
            var custIdsToString = string.Join(",", custIds);
            if (!string.IsNullOrWhiteSpace(custIdsToString))
            {
                return this.GetCustInfoByCustID(custIdsToString);
            }

            return Enumerable.Empty<CustInfoModel>();
        }

        public IEnumerable<CustInfoModel> GetCustInfoByCustID(string custIDs)
        {
            var criteria = new CustInfoByCustIdGetCriteria()
            {
                CustIDs = custIDs
            };

            return repository.Fetch<CustInfoModel>(criteria);
        }

        public MMDetailFilterViewModel GetFraudFilterInfoDetail(MMDetailInfoViewModel para, TicketDetailViewModel fraudTicket)
        {            
            var fraudTicketMergeAGroup = MergeFraudReasonAGroup(fraudTicket);

            var suspiciousTransInfos = fraudTicket.GetSuspiciousTransInfo(fraudTicketMergeAGroup);

            var mmticketDetailInfo = this.GetMMTicketDetailInfo(para, suspiciousTransInfos);

            var allFilterResult = fraudTicket.GenerateDropdownFilterListByMainTicketInfo(mmticketDetailInfo.TicketInfoList, para.LiveIndicator, para.Sport);
            allFilterResult.MaxSequenceId = mmticketDetailInfo.MaxSequenceId;
            allFilterResult.RecordsTotal = mmticketDetailInfo.TotalTicketAll;

            return allFilterResult;
        }

        public MMDetailFilterViewModel GetAllFilterInfoDetail(MMDetailInfoViewModel para)
        {           
            var mmticketDetailInfo = this.GetMMTicketDetailInfo(para, null);

            var allFilterResult = new TicketDetailViewModel().GenerateDropdownFilterListByMainTicketInfo(mmticketDetailInfo.TicketInfoList, para.LiveIndicator, para.Sport);
            allFilterResult.MaxSequenceId = mmticketDetailInfo.MaxSequenceId;
            allFilterResult.RecordsTotal = mmticketDetailInfo.TotalTicketAll;

            return allFilterResult;
        }        

        public IEnumerable<MatchMonitorDetailItemViewModel> MergeFraudToAllMMDetail(IEnumerable<MatchMonitorDetailItemViewModel> mdItemList, IEnumerable<MatchMonitorDetailItemViewModel> fraudMDItemList)
        {
            if (fraudMDItemList != null && fraudMDItemList.Any())
            {
                var fraudItemDic = fraudMDItemList.ToDictionary(x => x.TransId, x => x);

                return mdItemList.Select(x => x.MappingFraudTicketToAllTicket(fraudItemDic));
            }

            return mdItemList;
        }

        public IEnumerable<MatchMonitorDetailItemViewModel> MergeFraudReasonAGroup(TicketDetailViewModel fraudTicket)
        {
            var mdResult = Enumerable.Empty<MatchMonitorDetailItemViewModel>();

            var matchDetailInfo = fraudTicket.MatchDetailInfo;

            if (matchDetailInfo != null && matchDetailInfo.MatchDetails.Any())
            {
                var mmStatic = new MatchMonitorStaticListModel();
                var ticketDetail = fraudTicket.TicketDetails;
                var mergeGroup = fraudTicket.MMDetailMergeGroup;

                mmStatic = this.GetMMStaticList();

                var custIDs = matchDetailInfo.Custs.Select(x => x.CustID).Distinct();
                var custInfo = GetCustInfoByCustID(custIDs);

                mdResult = DrawColorFraudMMDetailWithMergeReasonAGroup(matchDetailInfo.VerifiedTrans, custInfo, ticketDetail, mergeGroup, mmStatic.Reasons);
            }

            return mdResult;
        }

        public IEnumerable<MatchMonitorDetailItemViewModel> DrawColorFraudMMDetailWithMergeReasonAGroup(IEnumerable<MatchMonitorDetailVerifyTransResultModel> verifiedTrans,
            IEnumerable<CustInfoModel> custInfo,
            IEnumerable<TicketDetailModel> ticket,
            MMDetailMergeGroupViewModel mergeGroup,
            IEnumerable<MatchMonitorReasonModel> reasons)
        {
            var ticketResult = Enumerable.Empty<MatchMonitorDetailItemViewModel>();

            if (custInfo == null || ticket == null || mergeGroup.TicketMergeGroups == null)
                return ticketResult;

            ticketResult = mergeGroup.TicketMergeGroups.Join(ticket, tg => tg.TransID, t => t.TransId,
                (tg, t) => new { tg = tg, t = t })
                .Join(custInfo, ttg => ttg.t.CustId, c => c.CustID,
                (ttg, c) => new { ttg = ttg, c = c }).Select(x =>
                {
                    var nTicket = new MatchMonitorDetailItemViewModel(x.ttg.tg.MergeGroupID, x.ttg.t, x.c);
                    var verifiedTransID = verifiedTrans.FirstOrDefault(y => y.VerifiedTransID == x.ttg.t.TransId)?.VerifiedTransID.ToString() ?? string.Empty;
                    var group = mergeGroup.Groups.FirstOrDefault(y => y.GroupID == x.ttg.tg.MergeGroupID);

                    nTicket.PICVerified = verifiedTransID;

                    nTicket.ReasonList = GetReasonName(reasons, x.ttg.tg.ReasonList);
                    nTicket.ReasonIDList = x.ttg.tg.ReasonList;
                    nTicket.ADetectGroupID = group?.GroupID ?? 0;
                    nTicket.ADetectGroup = group?.GroupName ?? "-";

                    return nTicket;
                });

            return ticketResult.Distinct(x=>x.TransId);
        }

        public string GetReasonName(IEnumerable<MatchMonitorReasonModel> reasonList, string reasonIDList)
        {
            if (!string.IsNullOrEmpty(reasonIDList))
            {
                var arrReasons = reasonIDList.Split(',').Select(x => Convert.ToInt32(x));
                var reasonNames = reasonList.Join(arrReasons,
                    r1 => r1.ReasonID,
                    r2 => r2,
                    (r1, r2) => r1.ReasonName);

                return string.Join(",", reasonNames);
            }

            return string.Empty;
        }

        public string GetReasonID(IEnumerable<MatchMonitorReasonModel> reasonList, IEnumerable<string> reasonNames)
        {
            var reasonIDs = new List<int>();
            
            if (reasonList == null)
            {
                var mmStatic = this.GetMMStaticList();
                reasonList = mmStatic.Reasons;
            }
            if (reasonNames != null && reasonNames.Any())
            {               
                reasonIDs = reasonList.Join(reasonNames,
                    r1 => r1.ReasonName,
                    r2 => r2,
                    (r1, r2) => r1.ReasonID).ToList();

                if (reasonNames.Any(x => x == "-") && !reasonIDs.Any(x=>x == 0))
                {
                    reasonIDs.Add(0);
                }
            }

            return string.Join(",", reasonIDs);            
        }

        public string GetTransFilterSuspicousMGr(MMDetailFilterViewModel filter, TicketDetailViewModel fraudTicket)
        {
            var transJson = string.Empty;

            var isGetAll = (filter.ReasonList != null && filter.ReasonList.Any(x => x.Equals("-"))
               && (filter.MGroupList != null && filter.MGroupList.Any(x => x.Equals("-") || x.Equals("0")))
               && (filter.AGroupList != null && filter.AGroupList.Any(x => x.Equals("-"))));

            if (!isGetAll || filter.IsFilterSuspicious)
            {
                var isExclude = (filter.ReasonList != null && filter.ReasonList.Any(x => x.Equals("-"))) &&
                        (filter.AGroupList != null && filter.AGroupList.Any(x => x.Equals("-")));

                if (fraudTicket != null)
                {
                    if (isExclude)
                    {
                        transJson = fraudTicket.GetTransByFilterExcludeSuspicous(filter, fraudTicket.AGroupReasonMerged);
                    }
                    else
                    {
                        transJson = fraudTicket.GetTransByFilterIncludeSuspicous(filter, fraudTicket.AGroupReasonMerged);
                    }
                }
                else
                {
                    fraudTicket = new TicketDetailViewModel();
                    if (isExclude)
                    {
                        transJson = fraudTicket.GetTransByFilterExcludeMGr(filter);
                    }
                    else
                    {
                        transJson = fraudTicket.GetTransByFilterIncludeMGr(filter);
                    }
                }
            }

            return transJson;
        }

        public TicketDetailViewModel GetTicketDetailFraud(MMDetailResultModel matchDetailInfo, MMDetailInfoViewModel para)
        {            
            var ticketMD = matchDetailInfo.MatchDetails.Select(x =>
                new FraudTransModel
                {
                    TransID = x.TransID,
                    Reason = x.Reason.ToString(),
                    AGroup = x.GroupID
                });

            var transJsons = JsonConvert.SerializeObject(ticketMD);            

            var result = this.GetTicketDetailByFraudTranIDs(para, null, transJsons);

            var mergeGroup = new MMDetailMergeGroupViewModel();
            var mMDetailBetTeams = matchDetailInfo.MatchDetails.Join(result.TicketDetails.Distinct(x => x.TransId),
                m => m.TransID,
                t => t.TransId, (m, t) => new MMDetailBetTeamModel(m, t));
        
            if (mMDetailBetTeams.Any())
            {
                var ticketMergeGroups = this.GetMMDetailMergeGroup(mMDetailBetTeams);
                mergeGroup.TicketMergeGroups = ticketMergeGroups;                

                mergeGroup.Groups = mergeGroup.GroupByGroupID();               
            }

            result.MMDetailMergeGroup = mergeGroup;

            return result;
        }

        public TicketDetailViewModel GetTicketDetailByFraudTranIDs(MMDetailInfoViewModel para, MMDetailFilterViewModel filter, string transJson)
        {
            return this.GetMainTicketDetail(para, filter, null, transJson);
        }       

        public TicketDetailViewModel GetTicketDetailAll(MMDetailInfoViewModel info, MMDetailFilterViewModel filter, string transJson)
        {
            var batchSize = 5000;
            var ticketDetails = new List<TicketDetailModel>();
            var result = new TicketDetailViewModel();
            var isContinue = true;

            if (filter.CustAmountRM > 0)
            {
                result = this.GetMainTicketDetail(info, filter, -1, transJson);
                info.LastScannedSequenceID = result.MinSequenceId;
            }
            else
            {
                do
                {
                    result = this.GetMainTicketDetail(info, filter, batchSize, transJson);

                    result.TicketDetails = result.TicketDetails.OrderByDescending(x => x.SequenceID);

                    info.LastScannedSequenceID = result.MinSequenceId;

                    if (result.MinSequenceId != null && result.TicketDetails.Count() > 0)
                    {
                        ticketDetails.AddRange(result.TicketDetails);
                    }

                    if (result.MinSequenceId == null || ticketDetails.Count() >= batchSize)
                    {
                        result.TicketDetails = ticketDetails.ToList();

                        isContinue = false;
                    }

                } while (isContinue);
            }
            
            return result;
        }

        public TicketDetailViewModel GetMainTicketDetail(MMDetailInfoViewModel para, MMDetailFilterViewModel filter, int? batchSize, string transJson = null)
        {
            var criteria = new TicketDetailGetCriteria()
            {               
                QueryType = 2, // Get ticket for view MM report              
                EventDate = para.EventDate,
                LastScannedSequenceID = para.LastScannedSequenceID,
                IsLive = para.LiveIndicator ? 1 : 0,
                MatchID = para.MatchID,
                BetType = para.BetTypeID,
                BetID = para.BetID,
                TicketType = para.TypeOfTickets == MatchMonitorQueryType.TicketAll ? 0 : 1,
                BatchSize = batchSize,
                IsLicensee = (para.Sites == MMSites.All || para.Sites == null) ? null : (int?)(para.Sites == MMSites.Licensee ? 1 : 0),
                ListTransId = transJson
            };

            if (IsDisplaySingleGrid(para.BetTypeID))
            {
                criteria.ListHDPBetTeam = filter?.BetTeamList == null ? null : string.Join(",", filter.BetTeamList);
            }
            else
            {
                criteria.ListHDPBetTeam = filter?.HDPList == null ? null : string.Join(",", filter.HDPList);
            }

            if (filter?.CustAmountRM > 0)
            {
                criteria.CustAmountRM = filter?.CustAmountRM;
            }

            if (filter != null)
            {
                criteria.ListScore = (filter.ScoreList == null || !para.LiveIndicator || filter.SportType == 2) ? null : string.Join(",", filter.ScoreList);
                criteria.ListCurrency = filter.CurrencyIdList == null ? null : string.Join(",", filter.CurrencyIdList);
                criteria.ListDanger = GenerateDangerJson(filter);
                criteria.ListStatus = filter.TicketStatusList == null ? null : string.Join(",", filter.TicketStatusList);
                criteria.ListCustomerClass = filter.CustomerClassList == null ? null : string.Join(",", filter.CustomerClassList);             
                criteria.ListReason = filter.ReasonList == null ? null : string.Join(",", filter.ReasonList).Replace('-', '0');
                criteria.ListAGroup = filter.AGroupList == null ? null : string.Join(",", filter.AGroupList).Replace('-', '0');
                criteria.ListMGroup = filter.MGroupList == null ? null : string.Join(",", filter.MGroupList).Replace('-', '0');
            }

            var result = repository.Fetch<TicketDetailModel>(criteria);          

            return new TicketDetailViewModel
            {
                TicketDetails = result,
                MinSequenceId = criteria.MinSequenceId,
                TotalTicket = criteria.TotalTicket
            };
        }

        public int GetTotalTicketDetail(MMDetailInfoViewModel para, MMDetailFilterViewModel filter, int? batchSize, string transIDsJson)
        {
            var ticketResult = this.GetMainTicketDetail(para, filter, batchSize, transIDsJson);

            return (int)ticketResult.TotalTicket;
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
       
        public MMTicketDetailInfoModel GetMMTicketDetailInfo(MMDetailInfoViewModel para, IEnumerable<SuspiciousTransInfoModel> suspiciousTranInfos)
        {
            var criteria = new MMTicketDetailInfoCriteria()
            {
                MatchID = para.MatchID,
                EventDate = para.EventDate,
                BetType = para.BetTypeID,
                IsLive = para.LiveIndicator ? 1 : 0,
                ListSuspiciousTransId = suspiciousTranInfos != null ? JsonConvert.SerializeObject(suspiciousTranInfos) : null,               
                TicketType = para.TypeOfTickets == MatchMonitorQueryType.TicketAll ? 0 : 1,
                IsExclTestCurr = false,
                BetID = para.BetID
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

        public bool IsDisplaySingleGrid(int betTypeID)
        {
            return betTypeID == (int)MatchMonitorBetType.Full1X2
                || betTypeID == (int)MatchMonitorBetType.First1X2
                || betTypeID == (int)MatchMonitorBetType.CorrectScore
                || betTypeID == (int)MatchMonitorBetType.FirstCorrectScore
                || betTypeID == (int)MatchMonitorBetType.TotalGoal
                || betTypeID == (int)MatchMonitorBetType.FirstTotalGoal
                || betTypeID == (int)MatchMonitorBetType.FirstTotalGoal;
        }

        public byte[] ExportExcelMMDetailSG(IEnumerable<ExExcelTicketInfoViewModel> items, string fileName, string templatePath)
        {
            var writer = new ExcelTemplateWriter(templatePath, fileName, "Sheet1");
            writer.CopyRows(0, 1);

            FillRowDataSG(items, ref writer);

            writer.SyncColumnsWidth(16);

            return writer.ToBytes();
        }

        private void FillRowDataSG(IEnumerable<ExExcelTicketInfoViewModel> items, ref ExcelTemplateWriter writer)
        {
            var arrItems = items != null ? items.ToArray() : new ExExcelTicketInfoViewModel[0];           
            var lengthItems = arrItems.Length;

            for (int i = 0; i < lengthItems; i++)
            {
                var item = arrItems[i];
                var obj = new object[] {
                    item.SubscriberType,
                    item.Username,
                    item.CustomerClass == -1 ? "-" : item.CustomerClass.ToString(),
                    (item.IsRobot ? "x" : "-"),
                    item.DangerToString,
                    item.Odds,
                    item.BetID.ToString(),
                    item.Currency,
                    (item.Amount % 1 == 0 ? item.Amount.ToString(Consts.FormatDecimalToString_Thousands) : item.Amount.ToString()),
                    item.BetTeam,
                    item.Score,
                    item.TransTimeToString,
                    item.BetFrom,
                    item.ADetectGroup,
                    item.MDetectGroup,
                    item.ReasonList,
                    (!string.IsNullOrEmpty(item.PICVerified) ? "x" : "-")
                };

                writer.WriteRow(1, obj);
            }
        }      

        public byte[] ExportExcelMMDetailDG(IEnumerable<ExExcelTicketInfoViewModel> homeMItems, IEnumerable<ExExcelTicketInfoViewModel> awayMItems
            , string fileName, string templatePath)
        {
            var writer = new ExcelTemplateWriter(templatePath, fileName, "Sheet1");
            writer.CopyRows(0, 1);

            FillRowDataDG(homeMItems, awayMItems, ref writer);

            writer.SyncColumnsWidth(33);

            return writer.ToBytes();
        }

        private void FillRowDataDG(IEnumerable<ExExcelTicketInfoViewModel> homeMItems, IEnumerable<ExExcelTicketInfoViewModel> awayMItems, ref ExcelTemplateWriter writer)
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
                        writer.WriteRow(1, NewRecordTblHomeAwayDG(tblHome[i], tblAway[i]));
                    else
                        writer.WriteRow(1, NewRecordTblHomeDG(tblHome[i]));
                }
            }
            else
            {
                for (int i = 0; i < lengthA; i++)
                {
                    if (i < lengthH)
                        writer.WriteRow(1, NewRecordTblHomeAwayDG(tblHome[i], tblAway[i]));
                    else
                        writer.WriteRow(1, NewRecordTblAwayDG(tblAway[i]));
                }
            }
        }

        private object[] NewRecordTblHomeDG(ExExcelTicketInfoViewModel itemH)
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

        private object[] NewRecordTblAwayDG(ExExcelTicketInfoViewModel itemA)
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

        private object[] NewRecordTblHomeAwayDG(ExExcelTicketInfoViewModel itemH, ExExcelTicketInfoViewModel itemA)
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