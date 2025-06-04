namespace Fanex.NAP.CTS.Service.Implement
{
    using Fanex.NAP.CTS.Core.Extentions;
    using Fanex.NAP.CTS.Core.Service;
    using Fanex.NAP.CTS.Criteria.Common;
    using Fanex.NAP.CTS.Model.Common;
    using Fanex.NAP.CTS.Model.Common.Entities;
    using Fanex.NAP.CTS.Service.Interface;
    using Fanex.NAP.CTS.ViewModel.Common;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using static Fanex.NAP.CTS.Resource.Consts.Consts;

    public class CommonService : ICommonService
    {
        private readonly ICriteriaService _repository;

        public CommonService(ICriteriaService repository = null)
        {
            _repository = repository ?? new CriteriaService();
        }

        public IEnumerable<SubscriberGroupViewModel> GetSubscriberGroups()
        {
            var subscriberGroups = Enumerable.Empty<SubscriberGroupViewModel>();
            var data = GetSubscribers();

            if (data != null)
            {
                subscriberGroups = data
                    .GroupBy(g => new { g.SubscriberGroupID, g.SubscriberGroupName })
                    .Select(x => new { Group = x.Key, SubscriberList = x.ToList() })
                    .Select(x => new SubscriberGroupViewModel
                    {
                        SubscriberGroupID = x.Group.SubscriberGroupID,
                        SubscriberGroupName = x.Group.SubscriberGroupName,
                        SubscriberList = x.SubscriberList.Select(y => new SubscriberViewModel
                        {
                            SubscriberID = y.SubscriberID,
                            SubscriberName = y.SubscriberName
                        })
                    });
            }

            return subscriberGroups;
        }

        public IEnumerable<SubscriberGroupModel> GetSubscribers()
        {
            var criteria = new GetSubscriberGroupCriteria();

            return _repository.Fetch<SubscriberGroupModel>(criteria);
        }

        public IEnumerable<ValidateUserNameModel> ValidateUsernames(string userNames, out bool isException)
        {
            isException = false;

            var criteria = new ValidateUsernameCriteria
            {
                userNames = userNames
            };

            try
            {
                return _repository.Fetch<ValidateUserNameModel>(criteria);
            }
            catch (Exception ex)
            {
                isException = true;
                LogExtension.LogError(ex.Message, ex);

                return Enumerable.Empty<ValidateUserNameModel>();
            }
        }

        public IEnumerable<SiteModel> GetSites(bool isNestedCalled)
        {
            var criteria = new GetSitesCriteria { IsNestedCalled = isNestedCalled };

            return _repository.Fetch<SiteModel>(criteria);
        }

        public IEnumerable<MainMatchModel> GetMainMatchInfosByIDs(string matchIDs)
        {
            var criteria = new GetMatchesInfoCriteria
            {
                MatchIds = matchIDs
            };

            return _repository.Fetch<MainMatchModel>(criteria);
        }

        public IEnumerable<MainTicketDetailModel> GetMainTicketDetails(string transIds, int? transSize, int? matchSize)
        {
            var criteria = new GetTicketDetailsCriteria
            {
                ListTransId = transIds,
                TransSize = transSize,
                MatchSize = matchSize
            };

            return _repository.Fetch<MainTicketDetailModel>(criteria);
        }

        public IEnumerable<CCGroupModel> GetGroupCustomerClass()
        {
            var groupCCList = Enumerable.Empty<CCGroupModel>();
            var criteria = new GetCommonCustomerClassCriteria();

            var data = _repository.Fetch<CommonCustomerClassModel>(criteria);

            if (data != null && data.Any())
            {
                groupCCList = data.GroupBy(g => g.CustomerClassGroup)
                     .Select(x => new { CCGroupName = x.Key, CCList = x.ToList() })
                     .Select(x => new CCGroupModel
                     {
                         CCGroupName = x.CCGroupName,
                         CCList = x.CCList.OrderBy(y => y.CustomerClass).Select(y => y.CustomerClass)?.ToList()
                     });
            }

            return groupCCList;
        }

        public int GetBetID(string betIDPattern, string nameDetailsX)
        {
            var betID = 0;

            if (!string.IsNullOrEmpty(betIDPattern) && !string.IsNullOrEmpty(nameDetailsX))
            {
                var strBetID = betIDPattern.ToUpper().Replace("X", nameDetailsX);

                int.TryParse(strBetID, out betID);
            }

            return betID;
        }

        public IEnumerable<StaticListModel> GetStaticLists(int listID, bool isStatusActive)
        {
            var criteria = new GetCommonStaticListCriteria
            {
                ListID = listID,
                IsStatusActive = isStatusActive
            };

            var result = _repository.Fetch<StaticListEntity>(criteria);

            return result?.Select(x => new StaticListModel(x)) ?? Enumerable.Empty<StaticListModel>();
        }

        public IEnumerable<BetTypeSettingModel> GetBetTypeSettings(BetTypeSettingParaModel infos)
        {
            var criteria = new GetCommonBetTypeSettingCriteria
            {
                FunctionID = infos.FunctionID,
                BettypeIDList = infos.BettypeIDList,
                SportTypeIDList = infos.SportTypeIDList,
                IsFilterMMDetection = infos.IsFilterMMDetection,
                IsFilterMMParlayDetection = infos.IsFilterMMParlayDetection,
                IsShowDetailsX = infos.IsShowDetailsX,
                Status = infos.Status
            };

            return _repository.Fetch<BetTypeSettingModel>(criteria);
        }

        public IEnumerable<SportBettypeViewModel> GetMainSportBettype(SportBettypeModel model)
        {
            var criteria = new GetCommonSportBettypeCriteria
            {
                ProductId = model.ProductId,
                SportBettypeList = model.SportBettypeList
            };

            return _repository.Fetch<SportBettypeViewModel>(criteria);
        }

        public IEnumerable<LeagueGroupViewModel> GetLeagueGroup()
        {
            var criteria = new GetCommonLeagueGroupCriteria();

            return _repository.Fetch<LeagueGroupViewModel>(criteria);
        }

        public IEnumerable<ValidateUserNameModel> ValidateUsernameForMain(string userName)
        {
            var criteria = new ValidateUsernameForMainCriteria { UsernameList = userName };

            return _repository.Fetch<ValidateUserNameModel>(criteria);
        }

        public IEnumerable<CustomerRoleViewModel> GetRoles()
        {
            return Enum.GetValues(typeof(ROLE_CUSTOMER))
                        .Cast<int>()
                        .Select(x => new CustomerRoleViewModel { RoleID = x });
        }
    }
}