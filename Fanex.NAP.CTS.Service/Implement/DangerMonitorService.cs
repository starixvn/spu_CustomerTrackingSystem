namespace Fanex.NAP.CTS.Service.Implement
{
    using Fanex.NAP.CTS.Core.Extentions;
    using Fanex.NAP.CTS.Model;
    using Fanex.NAP.CTS.Model.Common;
    using Fanex.NAP.CTS.Service.Interface;
    using Fanex.NAP.CTS.ViewModel.DangerMonitor;
    using System.Collections.Generic;
    using System.Linq;
    using static Fanex.NAP.CTS.Resource.Consts.Consts;

    public class DangerMonitorService : IDangerMonitorService
    {
        private readonly ICommonService _commonService;

        public DangerMonitorService()
        {
            _commonService = new CommonService();
        }

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
    }
}
