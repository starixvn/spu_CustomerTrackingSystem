namespace Fanex.NAP.CTS.Service.Implement
{
    using Fanex.Data.Repository;
    using Fanex.NAP.CTS.Core.BaseClasses;
    using Fanex.NAP.CTS.Core.Extentions;
    using Fanex.NAP.CTS.Criteria.DangerMonitor;
    using Fanex.NAP.CTS.Model;
    using Fanex.NAP.CTS.Model.Common;
    using Fanex.NAP.CTS.Model.DangerMonitor;
    using Fanex.NAP.CTS.Service.Interface;
    using Fanex.NAP.CTS.ViewModel.DangerMonitor;
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
                        BetTypeId = group.BetTypeID,
                        BetTypeName = group.BetTypeNameDisplay,
                        BetID = _commonService.GetBetID(group.BetIDPattern, group.NameDetailsX),
                        BetChoiceType = group.BetChoiceType,
                        BetChoiceHome = group.BetChoiceHome,
                        BetChoiceAway = group.BetChoiceAway,
                        PriorityOrder = group.PriorityOrder
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
                                                x.MatchName.Contains(filter.Event ?? string.Empty) ||
                                                x.LeagueName.Contains(filter.Event ?? string.Empty)).ToList();
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

                result = result.Join(sportBettypes, item => $"{item.SportType}_{item.BettypeID}_{item.BetID}", sportBettype => $"{sportBettype.SportType}_{sportBettype.BetTypeId}_{sportBettype.BetID}", (item, sportBettype) =>
                {
                    item.MappingBettypeSportInfo(sportBettype);
                    return item;
                });
            }

            return result;
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
        #endregion
    }
}
