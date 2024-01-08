namespace Fanex.NAP.CTS.Service.Implement
{
    using Fanex.Data.Repository;
    using Fanex.NAP.CTS.Core.BaseClasses;
    using Fanex.NAP.CTS.Core.Extentions;
    using Fanex.NAP.CTS.Criteria.DangerMonitor;
    using Fanex.NAP.CTS.Model;
    using Fanex.NAP.CTS.Model.Common;
    using Fanex.NAP.CTS.Model.DangerMonitor;
    using Fanex.NAP.CTS.Resource.Consts;
    using Fanex.NAP.CTS.Service.Interface;
    using Fanex.NAP.CTS.ViewModel.DangerMonitor;
    using Fanex.NAP.CTS.ViewModel.MatchMonitor;
    using Fanex.NAP.TGRS.Core.ExcelUtility;
    using System.Collections.Generic;
    using System.Linq;
    using static Fanex.NAP.CTS.Resource.Consts.Consts;

    public class DangerMonitorService : IDangerMonitorService
    {
        private readonly ICommonService _commonService;
        private readonly IDynamicRepository _repository;

        public DangerMonitorService()
        {
            _commonService = new CommonService();
            _repository = new DynamicRepository();
        }

        #region public method
        public IEnumerable<DangerMonitorBetTypeViewModel> GetSportBettypes()
        {
            var betTypes = GetBetTypeSettings(null, false);

            var filter = new SportBettypeModel
            {
                ProductId = (int)ProductType.SportBook,
                BettypeList = string.Join(",", betTypes.Select(x => x.BetTypeID).Distinct())
            };

            var mainBetTypes = _commonService.GetMainSportBettype(filter);

            return mainBetTypes
                .GroupJoin(betTypes, main => main.Typeid, cts => cts.BetTypeID, (main, ctsGroup) => (main, ctsGroup))
                .SelectMany(item =>
                    item.ctsGroup.Select(group => new DangerMonitorBetTypeViewModel
                    {
                        SportType = item.main.SportType,
                        SportName = item.main.SportName,
                        BetTypeID = group.BetTypeID,
                        BetTypeName = group.BetTypeNameDisplay,
                        BetID = _commonService.GetBetID(group.BetIDPattern, group.NameDetailsX),
                        BetChoiceType = group.BetChoiceType,
                        BetChoiceHome = group.BetChoiceHome,
                        BetChoiceAway = group.BetChoiceAway,
                        PriorityOrder = group.PriorityOrder,
                        BetIDPattern = group.BetIDPattern
                    }));
        }

        public IEnumerable<DangerMonitorInfoViewModel> GetDangerMonitor(DangerMonitorSearchModel filter)
        {
            var result = Enumerable.Empty<DangerMonitorInfoViewModel>();

            var mainMatches = _commonService.GetMainMatchInfos(filter);

            var teamNameDictionary = mainMatches.SelectMany(x => new List<int> { x.HomeId, x.AwayId }).Distinct().ToDictionary(x => x, x => CachedRef.GetTeamName(x));
            var leagueNameDictionary = mainMatches.Select(x => x.LeagueId).Distinct().ToDictionary(x => x, x => CachedRef.GetLeagueName(x));
            var filterMatches = mainMatches.Select(x => new DangerMonitorInfoViewModel(x, teamNameDictionary, leagueNameDictionary))
                                            .Where(x =>
                                                x.MatchName.ToUpper().Contains((filter.Event ?? string.Empty).ToUpper()) ||
                                                x.LeagueName.ToUpper().Contains((filter.Event ?? string.Empty).ToUpper())).ToList();
            if (filterMatches?.Any() ?? false)
            {
                var matchIDs = string.Join(",", filterMatches.Select(x => x.MatchID));
                var mainSummaryTicketMatches = GetDangerMonitorSummary(filter, matchIDs);

                result = filterMatches
                    .GroupJoin(mainSummaryTicketMatches,
                        main => main.MatchID,
                        mainSumGroup => mainSumGroup.MatchId,
                        (main, mainSumGroup) => (main, mainSumGroup))
                    .SelectMany(item =>
                    {
                        return item.mainSumGroup.Select(x => new DangerMonitorInfoViewModel(item.main, x));
                    });

                // get more info
                var sportBettypes = GetSportBettypes();

                result = result.Join(sportBettypes, item => $"{item.SportType}_{item.BettypeID}_{item.BetID}", sportBettype => $"{sportBettype.SportType}_{sportBettype.BetTypeID}_{sportBettype.BetID}", (item, sportBettype) =>
                {
                    item.MappingBettypeSportInfo(sportBettype);
                    return item;
                });
            }

            return result;
        }

        public byte[] ExportExcelMMDetailSG(IEnumerable<ExExcelTicketInfoViewModel> items, string fileName, string templatePath)
        {
            var writer = new ExcelTemplateWriter(templatePath, fileName, "Sheet1");
            writer.CopyRows(0, 1);

            FillRowDataSG(items, ref writer);

            writer.SyncColumnsWidth(14);

            return writer.ToBytes();
        }

        public byte[] ExportExcelMMDetailDG(IEnumerable<ExExcelTicketInfoViewModel> homeMItems, IEnumerable<ExExcelTicketInfoViewModel> awayMItems
            , string fileName, string templatePath)
        {
            var writer = new ExcelTemplateWriter(templatePath, fileName, "Sheet1");
            writer.CopyRows(0, 1);

            FillRowDataDG(homeMItems, awayMItems, ref writer);

            writer.SyncColumnsWidth(29);

            return writer.ToBytes();
        }
        #endregion

        #region private method
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

        private IEnumerable<DangerMonitorTicketSummaryViewModel> GetDangerMonitorSummary(DangerMonitorSearchModel filter, string matchIds)
        {
            var criteria = new DangerMonitorTicketSummaryCriteria
            {
                BettypeList = filter.BetTypes,
                DangerList = filter.DangerList,
                MarketList = filter.Market,
                MatchIds = matchIds
            };

            return _repository.Fetch<DangerMonitorTicketSummaryViewModel>(criteria);
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
    }
}
