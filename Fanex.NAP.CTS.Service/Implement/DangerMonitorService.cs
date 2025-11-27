namespace Fanex.NAP.CTS.Service.Implement
{
    using Fanex.NAP.CTS.Core.Extentions;
    using Fanex.NAP.CTS.Core.Helpers;
    using Fanex.NAP.CTS.Core.Service;
    using Fanex.NAP.CTS.Criteria.DangerMonitor;
    using Fanex.NAP.CTS.Model;
    using Fanex.NAP.CTS.Model.Common;
    using Fanex.NAP.CTS.Model.DangerMonitor;
    using Fanex.NAP.CTS.Model.DangerMonitor.Entities;
    using Fanex.NAP.CTS.Model.MatchMonitor;
    using Fanex.NAP.CTS.Resource.Consts;
    using Fanex.NAP.CTS.Service.Interface;
    using Fanex.NAP.CTS.ViewModel.DangerMonitor;
    using Fanex.NAP.CTS.ViewModel.MatchMonitor;
    using Fanex.NAP.TGRS.Core.ExcelUtility;
    using Newtonsoft.Json;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using static Fanex.NAP.CTS.Resource.Consts.Consts;

    public class DangerMonitorService : IDangerMonitorService
    {
        private readonly ICommonService _commonService;
        private readonly ICriteriaService _repository;
        private readonly IMemoryCacheService _memoryCacheService;
        private readonly IMatchMonitorService _matchMonitorService;

        private readonly string UserID = "MMDetailUserID";
        private readonly string Region = "MMDetailRegion";

        private const int mainBatchSize = 5000;
        private const int detailBatchSize = 5000;

        public DangerMonitorService(string region, string userID, ICriteriaService repository = null)
        {
            _commonService = new CommonService();
            _repository = repository ?? new CriteriaService();
            _matchMonitorService = new MatchMonitorService(MMQueryType.DangerMMReport);

            _memoryCacheService = new MemoryCacheService();

            Region = region;
            UserID = userID;
        }

        #region public method
        public IEnumerable<DangerMonitorSporTypeBetTypeViewModel> GetSportBettypesFromCache(bool isGetNew = false)
        {
            var result = default(IEnumerable<DangerMonitorSporTypeBetTypeViewModel>);

            var cacheKey = $"{nameof(GetSportBettypesFromCache)}_{Region}";

            lock (cacheKey)
            {
                result = isGetNew ? null : _memoryCacheService.GetValue<IEnumerable<DangerMonitorSporTypeBetTypeViewModel>>(cacheKey);

                if (result == null)
                {
                    result = GetSportBettypes();

                    _memoryCacheService.PushValue(cacheKey, result);
                }
            }

            return result;
        }

        public IEnumerable<DangerMonitorSporTypeBetTypeViewModel> GetBettypesFromCache()
        {
            return GetSportBettypesFromCache()?.GroupBy(item => $"{item.SportType}_{item.BetTypeID}_{item.BetID}")?.Select(x => x.FirstOrDefault());
        }

        public IEnumerable<DangerMonitorLeagueGroupViewModel> GetLeagueGroups()
        {
            var leagueGroups = _commonService.GetLeagueGroup();

            return leagueGroups?.Select(x => new DangerMonitorLeagueGroupViewModel
            {
                GroupID = x.GroupID,
                GroupName = x.GroupName,
                SportType = x.SportType,
                SportGroup = x.SportGroup
            }) ?? new List<DangerMonitorLeagueGroupViewModel>();
        }

        public DangerMonitorValidateUsernameViewModel ValidateUsernameForMain(string usernames)
        {
            var result = new DangerMonitorValidateUsernameViewModel();

            if (usernames.IsNotNullOrEmpty())
            {
                var validateUsernames = _commonService.ValidateUsernameForMain(usernames);

                var arrUsername = usernames.Split(',');


                var set = new HashSet<string>(validateUsernames?.Select(x => x.UserName) ?? new List<string>(), StringComparer.OrdinalIgnoreCase);

                result.ValidateUsernames = arrUsername?.Where(x => set.Contains(x)).ToList() ?? new List<string>();
                result.InvalidateUsernames = arrUsername?.Where(x => !set.Contains(x)).ToList() ?? new List<string>();
            }

            return result;
        }

        public GetDangerMonitorResultViewModel GetDangerMonitorWithNoAssociation(DangerMonitorSearchModel filter)
        {
            var result = Enumerable.Empty<GetDangerMonitorItemViewModel>();

            var dangerMMItems = GetDangerMonitorInfoWithNoAssociation(filter);

            result = dangerMMItems != null ? MappingDangerMonitorWithSportBettype(dangerMMItems) : null;

            return new GetDangerMonitorResultViewModel
            {
                Items = result ?? new List<GetDangerMonitorItemViewModel>()
            };
        }

        public GetDangerMonitorResultViewModel GetDangerMonitorWithAssociationByRootUser(DangerMonitorSearchModel filter, (string assByAIIPDevice, string assByAgent) associationData)
        {
            var result = Enumerable.Empty<GetDangerMonitorItemViewModel>();

            if (associationData.assByAIIPDevice.HasRealValue() || associationData.assByAgent.HasRealValue())
            {
                var dangerMMItems = GetDangerMonitorInfoWithAssociationRootUser(filter, associationData);
                result = dangerMMItems != null ? MappingDangerMonitorWithSportBettype(dangerMMItems) : null;
            }

            return new GetDangerMonitorResultViewModel
            {
                Items = result ?? new List<GetDangerMonitorItemViewModel>()
            };
        }

        public IEnumerable<CustInfoModel> GetCustInfoByCustID(string custIDs)
        {
            if (custIDs.IsNullOrEmpty())
            {
                return Enumerable.Empty<CustInfoModel>();
            }

            return _matchMonitorService.GetCustInfoByCustID(custIDs);
        }

        public byte[] ExportExcelMMDetailSG(IEnumerable<ExExcelTicketInfoViewModel> items, string fileName, string templatePath, int sportType)
        {
            var writer = new ExcelTemplateWriter(templatePath, fileName, "Sheet1");
            writer.CopyRows(0, 1);

            if (sportType == (int)MatchMonitorSportType.Cricket)
            {
                FillRowDataCricketSG(items, ref writer);
                writer.SyncColumnsWidth(15);
            }
            else
            {
                FillRowDataSG(items, ref writer);
                writer.SyncColumnsWidth(14);
            }

            return writer.ToBytes();
        }

        public byte[] ExportExcelMMDetailDG(IEnumerable<ExExcelTicketInfoViewModel> homeMItems, IEnumerable<ExExcelTicketInfoViewModel> awayMItems
            , string fileName, string templatePath, int sportType)
        {
            var writer = new ExcelTemplateWriter(templatePath, fileName, "Sheet1");
            writer.CopyRows(0, 1);

            if (ConditionUseSetExportExcel.Contains(sportType))
            {
                FillRowDataBadmintonDG(homeMItems, awayMItems, ref writer);
                writer.SyncColumnsWidth(31);
            }
            else
            {
                FillRowDataDG(homeMItems, awayMItems, ref writer);
                writer.SyncColumnsWidth(29);
            }

            return writer.ToBytes();
        }
        #endregion

        #region private method
        private IEnumerable<GetDangerMonitorItemViewModel> MappingDangerMonitorWithSportBettype(IEnumerable<DangerMMTicketInfoModel> dangerMonitors)
        {
            var sportBettypes = GetBettypesFromCache();

            return dangerMonitors.Join(sportBettypes,
                                        item => $"{item.SportType}_{item.Bettype}_{item.BetID}",
                                        sportBettype => $"{sportBettype.SportType}_{sportBettype.BetTypeID}_{sportBettype.BetID}",
                                        (item, sportBettype) => new GetDangerMonitorItemViewModel(item, sportBettype));
        }

        private IEnumerable<BetTypeSettingModel> GetBetTypeSettings(string sportIDs, bool isFilterMMDetection)
        {
            var paras = new BetTypeSettingParaModel
            {
                FunctionID = (int)BetTypeSettingsFunctionID.DgrBetType,
                BettypeIDList = null,
                SportTypeIDList = sportIDs,
                IsFilterMMDetection = isFilterMMDetection,
                IsShowDetailsX = true,
                Status = 1
            };

            return _commonService.GetBetTypeSettings(paras);
        }

        private IEnumerable<DangerMMTicketInfoModel> GetDangerMonitorInfoWithNoAssociation(DangerMonitorSearchModel filter)
        {
            var criteria = new DangerMonitorSearchMatchInfoCriteria
            {
                FromDate = filter.FromDate,
                ToDate = filter.ToDate,
                SportList = filter.SportList?.Where(x => x.Contains("null"))?.Select(x => x.Split('_')[0])?.Join(),
                SportSabaLgGroup = filter.SportList?.Where(x => !x.Contains("null"))?.Join(),
                LeagueEvent = filter.Event ?? string.Empty,
                BettypeList = filter.BetTypes,
                MatchSection = filter.MatchSection,
                MarketList = filter.Market,
                QueryDanger = filter.QueryDanger ?? string.Empty,
                UserNameList = filter.UserNameList ?? string.Empty,
                LeagueGroupList = filter.LeagueGroupList,
                TotalAmount = filter.TotalAmount,
                UserAmount = filter.UserAmount
            };

            var result = _repository.Fetch<DangerMMTicketInfoEntity>(criteria);

            return result?.Select(x => new DangerMMTicketInfoModel(x)) ?? Enumerable.Empty<DangerMMTicketInfoModel>();
        }

        private IEnumerable<DangerMMTicketInfoModel> GetDangerMonitorInfoWithAssociationRootUser(DangerMonitorSearchModel filter, (string assByAIIPDevice, string assByAgent) associatedData)
        {
            var criteria = new DangerMonitorSearchMatchInfoWithAssociationRootUserCriteria
            {
                FromDate = filter.FromDate,
                ToDate = filter.ToDate,
                SportList = filter.SportList?.Where(x => x.Contains("null"))?.Select(x => x.Split('_')[0])?.Join(),
                SportSabaLgGroup = filter.SportList?.Where(x => !x.Contains("null"))?.Join(),
                LeagueEvent = filter.Event ?? string.Empty,
                BettypeList = filter.BetTypes,
                MatchSection = filter.MatchSection,
                MarketList = filter.Market,
                LeagueGroupList = filter.LeagueGroupList,
                TotalAmount = filter.TotalAmount,
                UserAmount = filter.UserAmount,
                AssociationByAIDeviceIP = associatedData.assByAIIPDevice,
                AssociationByAgent = associatedData.assByAgent,
            };

            var result = _repository.Fetch<DangerMMTicketInfoEntity>(criteria);

            return result?.Select(x => new DangerMMTicketInfoModel(x)) ?? Enumerable.Empty<DangerMMTicketInfoModel>();
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
                };

                writer.WriteRow(1, obj);
            }
        }

        private void FillRowDataCricketSG(IEnumerable<ExExcelTicketInfoViewModel> items, ref ExcelTemplateWriter writer)
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
                    item.DangerToString,
                     (item.Game == 0 ? "-" : item.Game.ToString()),
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
                };

                writer.WriteRow(1, obj);
            }
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

        private void FillRowDataBadmintonDG(IEnumerable<ExExcelTicketInfoViewModel> homeMItems, IEnumerable<ExExcelTicketInfoViewModel> awayMItems, ref ExcelTemplateWriter writer)
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
                        writer.WriteRow(1, NewRecordTblBadmintonHomeAwayDG(tblHome[i], tblAway[i]));
                    else
                        writer.WriteRow(1, NewRecordTblBadmintonHomeDG(tblHome[i]));
                }
            }
            else
            {
                for (int i = 0; i < lengthA; i++)
                {
                    if (i < lengthH)
                        writer.WriteRow(1, NewRecordTblBadmintonHomeAwayDG(tblHome[i], tblAway[i]));
                    else
                        writer.WriteRow(1, NewRecordTblBadmintonAwayDG(tblAway[i]));
                }
            }
        }

        private object[] NewRecordTblBadmintonHomeDG(ExExcelTicketInfoViewModel itemH)
        {
            return new object[]
                {
                    itemH.SubscriberType,
                    itemH.Username,
                    itemH.CustomerClass == -1 ? "-" : itemH.CustomerClass.ToString(),
                    itemH.DangerToString,
                     (itemH.Game == 0 ? "-" : itemH.Game.ToString()),
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
                    null, null, null, null,
                    null, null, null, null,
                    null, null, null, null,
                    null, null, null, null,
                    null
                };
        }

        private object[] NewRecordTblBadmintonAwayDG(ExExcelTicketInfoViewModel itemA)
        {
            return new object[]
                {
                    null, null, null, null,
                    null, null, null, null,
                    null, null, null, null,
                    null, null, null, null,
                    null,
                    itemA.SubscriberType,
                    itemA.Username,
                    itemA.CustomerClass == -1 ? "-" : itemA.CustomerClass.ToString(),
                    itemA.DangerToString,
                    (itemA.Game == 0 ? "-" : itemA.Game.ToString()),
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
                };
        }

        private object[] NewRecordTblBadmintonHomeAwayDG(ExExcelTicketInfoViewModel itemH, ExExcelTicketInfoViewModel itemA)
        {
            return new object[]
                {
                    itemH.SubscriberType,
                    itemH.Username,
                    itemH.CustomerClass == -1 ? "-" : itemH.CustomerClass.ToString(),
                    itemH.DangerToString,
                     (itemH.Game == 0 ? "-" : itemH.Game.ToString()),
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
                    "",
                    itemA.SubscriberType,
                    itemA.Username,
                    itemA.CustomerClass == -1 ? "-" : itemA.CustomerClass.ToString(),
                    itemA.DangerToString,
                    (itemA.Game == 0 ? "-" : itemA.Game.ToString()),
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
                };
        }

        private object[] NewRecordTblHomeDG(ExExcelTicketInfoViewModel itemH)
        {
            return new object[]
                {
                    itemH.SubscriberType,
                    itemH.Username,
                    itemH.CustomerClass == -1 ? "-" : itemH.CustomerClass.ToString(),
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
                    null, null, null, null,
                    null, null, null, null,
                    null, null, null, null,
                    null, null, null, null,
                };
        }

        private object[] NewRecordTblAwayDG(ExExcelTicketInfoViewModel itemA)
        {
            return new object[]
                {
                    null, null, null, null,
                    null, null, null, null,
                    null, null, null, null,
                    null, null, null, null,
                    itemA.SubscriberType,
                    itemA.Username,
                    itemA.CustomerClass == -1 ? "-" : itemA.CustomerClass.ToString(),
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
                };
        }

        private object[] NewRecordTblHomeAwayDG(ExExcelTicketInfoViewModel itemH, ExExcelTicketInfoViewModel itemA)
        {
            return new object[]
                {
                    itemH.SubscriberType,
                    itemH.Username,
                    itemH.CustomerClass == -1 ? "-" : itemH.CustomerClass.ToString(),
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
                    "",
                    itemA.SubscriberType,
                    itemA.Username,
                    itemA.CustomerClass == -1 ? "-" : itemA.CustomerClass.ToString(),
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
                };
        }
        #endregion

        #region mmdetail
        public MMDetailTicketListViewModel GetMatchMonitorDetail(MMDetailInfoViewModel para, MMDetailFilterViewModel filter)
        {
            var mdResult = default(MMDetailTicketListViewModel);

            switch (para.TypeOfTickets)
            {
                case MMViewModeFromUI.TicketFraud:
                    {
                        var fraudTicket = GetFraudMMDetailForCache(para);

                        if (fraudTicket != null)
                        {
                            mdResult = GetFraudMMDetail(para, filter, fraudTicket);
                        }

                        break;
                    }
                case MMViewModeFromUI.TicketAll when para.IsSupportShowAllTickets:
                    {
                        mdResult = GetAllMMDetail(para, filter);

                        if (mdResult != null)
                        {
                            var ctsCustIDHomeIsZero = mdResult.HomeMatchItemList.Where(x => x.CTSCustID == 0).Select(x => x);
                            var ctsCustIDAwayIsZero = mdResult.AwayMatchItemList.Where(x => x.CTSCustID == 0).Select(x => x);

                            if (ctsCustIDHomeIsZero != null && ctsCustIDHomeIsZero.Any())
                            {
                                LogExtension.LogError($"{Region} Get Match Monitor Detail with CTSCustIDHome=0: {JsonConvert.SerializeObject(ctsCustIDHomeIsZero)}");
                            }

                            if (ctsCustIDAwayIsZero != null && ctsCustIDAwayIsZero.Any())
                            {
                                LogExtension.LogError($"{Region} Get Match Monitor Detail with CTSCustIDAway=0: {JsonConvert.SerializeObject(ctsCustIDAwayIsZero)}");
                            }
                        }

                        break;
                    }
            }

            return mdResult;
        }

        public MMDetailFilterViewModel GetMatchMonitorFilter(MMDetailInfoViewModel para)
        {
            var result = new MMDetailFilterViewModel();

            if (para.IsImportNewTicket)
            {
                result = GetMatchMonitorFilterImportNew(para);
            }
            else
            {
                if (para.TypeOfTickets == MMViewModeFromUI.TicketFraud)
                {
                    result = GetFraudMMFilter(para);
                }
                else if (para.IsSupportShowAllTickets)
                {
                    result = GetAllMMFilter(para);
                }
            }

            return result;
        }
        #endregion

        #region private method
        private TicketDetailViewModel GetFraudMMDetailForCache(MMDetailInfoViewModel filter)
        {
            var fraudMMDetailResult = new TicketDetailViewModel() { TicketType = filter.TicketType };
            var viewModeName = Enum.GetName(typeof(MMViewModeFromUI), MMViewModeFromUI.TicketFraud);

            try
            {
                var matchDetailInfo = new MMDetailResultModel();
                var fraudTicketDetails = new List<TicketDetailModel>();

                filter.BetID?.Split(',').ToList().ForEach((betID) =>
                {
                    var para = filter.Clone();
                    para.TypeOfTickets = MMViewModeFromUI.TicketFraud;
                    para.BetID = betID;

                    var matchDetailForCache = _matchMonitorService.GetMatchMonitorDetailForCache(para, Region, UserID, viewModeName);

                    var ticketDetails = _matchMonitorService.GetSingleTicketDetailForCache(matchDetailForCache, para, Region, UserID, viewModeName);

                    matchDetailInfo.Custs.AddRange(matchDetailForCache.Custs);
                    matchDetailInfo.MatchDetails.AddRange(matchDetailForCache.MatchDetails);
                    matchDetailInfo.VerifiedTrans.AddRange(matchDetailForCache.VerifiedTrans);

                    fraudTicketDetails.AddRange(ticketDetails.TicketDetails);
                });

                fraudMMDetailResult = new TicketDetailViewModel() { TicketType = filter.TicketType };
                var ticketDetailModel = new TicketDetailViewModel() { TicketDetails = fraudTicketDetails };

                fraudMMDetailResult = _matchMonitorService.BuildSingleTicketDetailFraudMergeGroup(matchDetailInfo, filter, ticketDetailModel);

            }
            catch (Exception ex)
            {
                ex.LogError($"{Region} Get Fraud MM Detail For Cache Error: " + ex.Message + ex.StackTrace + "\n"
                   + ex.InnerException.Message + ex.InnerException.Source + ex.InnerException
                   + ex.InnerException.StackTrace);
            }

            return fraudMMDetailResult;
        }

        private MMDetailTicketListViewModel GetFraudMMDetail(MMDetailInfoViewModel para, MMDetailFilterViewModel filter, TicketDetailViewModel fraudTicket)
        {
            var mdResult = new MMDetailTicketListViewModel();
            var matchDetailInfo = fraudTicket.MatchDetailInfo;
            fraudTicket.TicketType = para.TicketType;

            if (matchDetailInfo != null && matchDetailInfo.MatchDetails.Any())
            {
                var mmStatic = new MatchMonitorStaticListModel();
                var ticketDetail = fraudTicket.TicketDetails;
                var mergeGroup = fraudTicket.MMDetailMergeGroup;

                var reasons = _commonService.GetStaticLists((int)StaticListID.Reason, true);

                var custIDs = matchDetailInfo.Custs.Select(x => x.CustID).Distinct();
                var custInfo = GetCustInfoByCustID(custIDs);

                var ticketMergeGroups = fraudTicket.AGroupReasonMerged;

                var transJson = fraudTicket.GetTransByFilterIncludeSuspicous(filter, ticketMergeGroups);

                var ticketDetailGroup = _matchMonitorService.GetTicketDetailByFraudTranIDs(para, filter, transJson);

                custIDs = ticketDetailGroup.TicketDetails.Select(ticket => ticket.CustId).Distinct();

                custInfo = GetCustInfoByCustID(custIDs);

                var mdItemList = _matchMonitorService.DrawColorFraudMMDetailWithMergeReasonAGroup(matchDetailInfo.VerifiedTrans, custInfo, ticketDetailGroup.TicketDetails, mergeGroup, reasons);

                if (para.BetChoiceType == 1)
                {
                    mdResult.ItemList = mdItemList.OrderByDescending(x => x.TransTime).ThenByDescending(x => x.TransId).ToList();
                }
                else
                {
                    mdResult.HomeMatchItemList = mdItemList.Where(x => x.ChoiceOrder == 10).OrderByDescending(x => x.TransTime).ThenByDescending(x => x.TransId).ToList();
                    mdResult.AwayMatchItemList = mdItemList.Where(x => x.ChoiceOrder == 20).OrderByDescending(x => x.TransTime).ThenByDescending(x => x.TransId).ToList();
                }

                mdResult.RecordsTotal = mdItemList?.Count() ?? 0;
            }

            return mdResult;
        }

        private MMDetailTicketListViewModel GetAllMMDetail(MMDetailInfoViewModel info, MMDetailFilterViewModel filter)
        {
            var para = info.Clone();
            para.TypeOfTickets = MMViewModeFromUI.TicketAll;
            var mdResult = new MMDetailTicketListViewModel();
            var ticketDetailGroup = new TicketDetailViewModel();

            try
            {
                if (info.BetChoiceType != 1)
                {
                    var matchDetailInfo = _matchMonitorService.GetMatchMonitorDetail(para);

                    if (matchDetailInfo != null && matchDetailInfo.MatchDetails.Any())
                    {
                        var fraudTicket = GetFraudMMDetailForCache(para);
                        fraudTicket.TicketType = para.TicketType;

                        var transJson = _matchMonitorService.GetTransFilterSuspicousMGr(filter, fraudTicket);

                        ticketDetailGroup = GetTicketDetailAll(para, filter, transJson);

                        mdResult.SetLastMark(para);

                        var custIDs = ticketDetailGroup.TicketDetails.Select(ticket => ticket.CustId).Distinct();

                        var custInfo = GetCustInfoByCustID(custIDs);

                        var mdItemList = DrawColorAllMMDetailAndUpdateValue(matchDetailInfo.VerifiedTrans, custInfo, ticketDetailGroup.TicketDetails, para.BetTypeID);

                        mdItemList = _matchMonitorService.MergeFraudToAllMMDetail(mdItemList, fraudTicket?.AGroupReasonMerged);

                        mdResult.HomeMatchItemList = mdItemList?.Where(x => x.ChoiceOrder == 10).OrderByDescending(x => x.TransTime).ThenByDescending(x => x.TransId).ToList();

                        mdResult.AwayMatchItemList = mdItemList?.Where(x => x.ChoiceOrder == 20).OrderByDescending(x => x.TransTime).ThenByDescending(x => x.TransId).ToList();

                        mdResult.RecordsTotal = mdItemList?.Count() ?? 0;
                    }
                    else
                    {
                        mdResult = GetTicketsFromMainDb(para, filter);
                    }
                }
                else if (para.TypeOfTickets == MMViewModeFromUI.TicketAll)
                {
                    mdResult = GetTicketsFromMainDb(para, filter);
                }

                return mdResult;
            }
            catch (Exception ex)
            {
                ex.LogError($"{Region} Get All Match Monitor Detail Error: " + ex.Message + ex.StackTrace + "\n"
                  + ex.InnerException.Message + ex.InnerException.Source + ex.InnerException
                  + ex.InnerException.StackTrace);
            }

            return mdResult;
        }

        private TicketDetailViewModel GetTicketDetailAll(MMDetailInfoViewModel info, MMDetailFilterViewModel filter, string transJson)
        {
            var ticketDetails = new List<TicketDetailModel>();
            var result = new TicketDetailViewModel();
            var isContinue = true;

            if (filter.CustAmountRM > 0)
            {
                result = _matchMonitorService.GetMainTicketDetail(info, filter, -1, transJson);
                info.SetLastMark(result);
            }
            else
            {
                do
                {
                    result = _matchMonitorService.GetMainTicketDetail(info, filter, detailBatchSize, transJson);

                    result.TicketDetails = result.TicketDetails.OrderByDescending(x => x.TransDate);

                    info.SetLastMark(result);

                    if (!StopLoopCallDataMMCondition(result) && result.TicketDetails.Count() > 0)
                    {
                        ticketDetails.AddRange(result.TicketDetails);
                    }

                    if (StopLoopCallDataMMCondition(result) || ticketDetails.Count() >= detailBatchSize)
                    {
                        result.TicketDetails = ticketDetails.ToList();

                        isContinue = false;
                    }

                } while (isContinue);
            }

            return result;
        }

        private bool StopLoopCallDataMMCondition(TicketDetailViewModel result)
        {
            return result.MinSequenceId is null && result.MinTransDate is null;
        }

        private IEnumerable<MatchMonitorDetailItemViewModel> DrawColorAllMMDetailAndUpdateValue(IEnumerable<MatchMonitorDetailVerifyTransResultModel> verifiedTrans,
            IEnumerable<CustInfoModel> custInfos,
            IEnumerable<TicketDetailModel> tickets,
            int betTypeID)
        {
            var ticketResult = new List<MatchMonitorDetailItemViewModel>();
            var custInfoDictionary = custInfos.ToDictionary(x => x.CustID, x => x);

            if (custInfos == null || tickets == null)
                return ticketResult;

            foreach (var t in tickets)
            {
                var verifiedTransID = verifiedTrans.FirstOrDefault(y => y.VerifiedTransID == t.TransId)?.VerifiedTransID.ToString() ?? string.Empty;
                var nTicket = custInfoDictionary.TryGetValue(t.CustId, out var custInfo) ? new MatchMonitorDetailItemViewModel(t, custInfo) : new MatchMonitorDetailItemViewModel(t);

                nTicket.PICVerified = verifiedTransID;
                nTicket.BetTypeID = betTypeID;
                nTicket.ADetectGroup = "-";
                nTicket.ReasonList = "-";
                nTicket.ReasonIDList = string.Empty;
                nTicket.TypeOfTicket = MMViewModeFromUI.TicketAll;

                ticketResult.Add(nTicket);
            }

            return ticketResult;
        }

        private MMDetailTicketListViewModel GetTicketsFromMainDb(MMDetailInfoViewModel para, MMDetailFilterViewModel filter)
        {
            var verifiedTrans = new List<MatchMonitorDetailVerifyTransResultModel>();
            var ticketFromMainDb = new MMDetailTicketListViewModel();

            var transJson = _matchMonitorService.GetTransFilterSuspicousMGr(filter, null, para.TicketType);

            var ticketDetailGroup = GetTicketDetailAll(para, filter, transJson);

            var custInfo = GetCustInfoByCustID(ticketDetailGroup.TicketDetails.Select(ticket => ticket.CustId).Distinct());

            var mdItemList = DrawColorAllMMDetailAndUpdateValue(verifiedTrans, custInfo, ticketDetailGroup.TicketDetails, para.BetTypeID);

            if (para.BetChoiceType == 1)
            {
                ticketFromMainDb.ItemList = mdItemList.OrderByDescending(x => x.TransTime).ThenByDescending(x => x.TransId).ToList();
            }
            else
            {
                ticketFromMainDb.HomeMatchItemList = mdItemList?.Where(x => x.ChoiceOrder == 10).OrderByDescending(x => x.TransTime).ThenByDescending(x => x.TransId).ToList();
                ticketFromMainDb.AwayMatchItemList = mdItemList?.Where(x => x.ChoiceOrder == 20).OrderByDescending(x => x.TransTime).ThenByDescending(x => x.TransId).ToList();
            }

            ticketFromMainDb.RecordsTotal = mdItemList?.Count() ?? 0;
            ticketFromMainDb.SetLastMark(para);

            return ticketFromMainDb;
        }

        private MMDetailFilterViewModel GetMatchMonitorFilterImportNew(MMDetailInfoViewModel para)
        {
            var result = new MMDetailFilterViewModel();

            var removeKeyFraud = para.BuildCacheKey(string.Empty, Region, UserID, Enum.GetName(typeof(MMViewModeFromUI), MMViewModeFromUI.TicketFraud));
            _memoryCacheService.RemoveKeyContainsValue(removeKeyFraud);

            para.BetID?.Split(',').ToList().ForEach(betId =>
            {
                var removeKey = para.BuildCacheKeyByBetID(string.Empty, Region, UserID, betId, Enum.GetName(typeof(MMViewModeFromUI), MMViewModeFromUI.TicketFraud));
                _memoryCacheService.RemoveKeyContainsValue(removeKey);
            });

            var removeKeyAll = para.BuildCacheKey(string.Empty, Region, UserID, Enum.GetName(typeof(MMViewModeFromUI), MMViewModeFromUI.TicketAll));
            _memoryCacheService.RemoveKeyContainsValue(removeKeyAll);

            switch (para.TypeOfTickets)
            {
                case MMViewModeFromUI.TicketFraud:
                    {
                        result = GetFraudMMFilter(para);

                        if (para.IsSupportShowAllTickets)
                        {
                            Task.Run(() =>
                            {
                                para.TypeOfTickets = MMViewModeFromUI.TicketAll;

                                _ = GetAllMMFilter(para);
                            });
                        }

                        break;
                    }
                case MMViewModeFromUI.TicketAll:
                    {
                        para.TypeOfTickets = MMViewModeFromUI.TicketFraud;

                        GetFraudMMDetailForCache(para);

                        if (para.IsSupportShowAllTickets)
                        {
                            para.TypeOfTickets = MMViewModeFromUI.TicketAll;

                            result = GetAllMMFilter(para);
                        }

                        break;
                    }
            }

            return result;
        }

        private MMDetailFilterViewModel GetFraudMMFilter(MMDetailInfoViewModel para)
        {
            var fraudFilterResult = new MMDetailFilterViewModel();
            para.TypeOfTickets = MMViewModeFromUI.TicketFraud;
            var viewModeName = Enum.GetName(typeof(MMViewModeFromUI), para.TypeOfTickets);

            var key = para.BuildCacheKey(nameof(GetFraudMMFilter), Region, UserID, viewModeName);

            lock (key)
            {
                fraudFilterResult = _memoryCacheService.GetValue<MMDetailFilterViewModel>(key);

                if (fraudFilterResult == null)
                {
                    var fraudTicket = GetFraudMMDetailForCache(para);

                    if (fraudTicket != null)
                    {
                        var reasons = _commonService.GetStaticLists((int)StaticListID.Reason, true);

                        fraudFilterResult = _matchMonitorService.GetFraudFilterInfoDetail(para, fraudTicket, reasons);

                        _memoryCacheService.PushValueAsync(key, fraudFilterResult);
                    }
                }
            }

            fraudFilterResult = fraudFilterResult ?? new MMDetailFilterViewModel();
            fraudFilterResult.IsDisplaySingleGrid = para.BetChoiceType == 1;

            return fraudFilterResult;
        }

        private MMDetailFilterViewModel GetAllMMFilter(MMDetailInfoViewModel para)
        {
            var allFilterResult = new MMDetailFilterViewModel();
            var viewModeName = Enum.GetName(typeof(MMViewModeFromUI), para.TypeOfTickets);
            var key = para.BuildCacheKey(nameof(GetAllMMFilter), Region, UserID, viewModeName);

            try
            {
                allFilterResult = _memoryCacheService.GetValue<MMDetailFilterViewModel>(key);

                if (allFilterResult == null)
                {
                    var fraudTicket = GetFraudMMDetailForCache(para);

                    if (fraudTicket != null)
                    {
                        var reasons = _commonService.GetStaticLists((int)StaticListID.Reason, true);

                        allFilterResult = _matchMonitorService.GetFraudFilterInfoDetail(para, fraudTicket, reasons);

                        _memoryCacheService.PushValueAsync(key, allFilterResult);
                    }
                    else
                    {
                        allFilterResult = _matchMonitorService.GetAllFilterInfoDetail(para);
                    }
                }
            }
            catch (Exception ex)
            {
                ex.LogError($"{Region} Get All MM Filter Error: " + ex.Message + ex.StackTrace + "\n"
                     + ex.InnerException.Message + ex.InnerException.Source + ex.InnerException
                     + ex.InnerException.StackTrace);

                throw;
            }

            allFilterResult = allFilterResult ?? new MMDetailFilterViewModel();
            allFilterResult.IsDisplaySingleGrid = para.BetChoiceType == 1;

            return allFilterResult;
        }

        private IEnumerable<CustInfoModel> GetCustInfoByCustID(IEnumerable<long> custIds)
        {
            var custIdsToString = string.Join(",", custIds);
            if (!string.IsNullOrWhiteSpace(custIdsToString))
            {
                return _matchMonitorService.GetCustInfoByCustID(custIdsToString);
            }

            return Enumerable.Empty<CustInfoModel>();
        }

        private IEnumerable<DangerMonitorSporTypeBetTypeViewModel> GetSportBettypes()
        {
            var betTypes = GetBetTypeSettings(null, false);

            var filter = new SportBettypeModel
            {
                ProductId = (int)ProductType.SportBook,
                SportBettypeList = JsonConvert.SerializeObject(betTypes.Select(x => x.BetTypeID).Distinct())
            };

            var mainBetTypes = _commonService.GetMainSportBettype(filter);
            DangerMonitorSporTypeBetTypeViewModel Project(
                    dynamic main,
                    dynamic bt
                ) => new DangerMonitorSporTypeBetTypeViewModel
                {
                    SportGroup = main.SportGroup,
                    SportNameDisplay = main.SportNameDisplay,
                    LeagueGroupID = main.LeagueGroupID,
                    SportType = main.SporttypeId,
                    SportName = main.SportName,
                    BetTypeID = bt.BetTypeID,
                    BetTypeName = bt.BetTypeNameDisplay,
                    BetID = _commonService.GetBetID(bt.BetIDPattern, bt.NameDetailsX),
                    BetChoiceType = bt.BetChoiceType,
                    BetChoiceHome = bt.BetChoiceHome,
                    BetChoiceAway = bt.BetChoiceAway,
                    SportOrder = bt.SportOrder,
                    BetIDPattern = bt.BetIDPattern,
                    BettypeOrder = bt.BettypeOrder
                };

            var result1 = mainBetTypes
               .Join(
                   betTypes.Where(x => x.SportTypeID == 0),
                   main => main.BettypeId,
                   bt => bt.BetTypeID,
                   (main, bt) => Project(main, bt)
               );

            var result2 = mainBetTypes
                .Join(
                    betTypes.Where(x => x.SportTypeID > 0),
                    main => new { main.BettypeId, main.SporttypeId },
                    bt => new { BettypeId = bt.BetTypeID, SporttypeId = bt.SportTypeID },
                    (main, bt) => Project(main, bt)
                );

            return result1.Union(result2);
        }
        #endregion
    }
}
