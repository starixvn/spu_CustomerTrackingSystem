namespace Fanex.NAP.CTS.Service.Implement
{
    using Fanex.Data;
    using Fanex.Data.Repository;
    using Fanex.Logging;
    using Fanex.NAP.CTS.Criteria.Common;
    using Fanex.NAP.CTS.Model.Common;
    using Fanex.NAP.CTS.Model.DangerMonitor;
    using Fanex.NAP.CTS.Service.Interface;
    using Fanex.NAP.CTS.ViewModel.Common;
    using Newtonsoft.Json;
    using System;
    using System.Collections.Generic;
    using System.Linq;

    public class CommonService : ICommonService
    {
        private readonly IDynamicRepository repository;

        public CommonService()
        {
            repository = new DynamicRepository();
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

            using (var db = ObjectDbFactory.CreateInstance(criteria.GetSettingKey()))
            {
                return db.Query<SubscriberGroupModel>(criteria);
            }
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
                using (var db = ObjectDbFactory.CreateInstance(criteria.GetSettingKey()))
                {
                    return db.Query<ValidateUserNameModel>(criteria);
                }
            }
            catch (Exception ex)
            {
                isException = true;
                LogService.Error(ex.Message, ex);

                return Enumerable.Empty<ValidateUserNameModel>();
            }
        }

        public IEnumerable<SiteModel> GetSites(bool isNestedCalled)
        {
            var criteria = new GetSitesCriteria { IsNestedCalled = isNestedCalled };

            using (var db = ObjectDbFactory.CreateInstance(criteria.GetSettingKey()))
            {
                return db.Query<SiteModel>(criteria);
            }
        }

        public IEnumerable<MainMatchModel> GetMainMatchInfosByIDs(string matchIDs)
        {
            var criteria = new GetMatchesInfoCriteria
            {
                MatchIds = matchIDs
            };

            using (var db = ObjectDbFactory.CreateInstance(criteria.GetSettingKey()))
            {
                return db.Query<MainMatchModel>(criteria);
            }
        }

        public IEnumerable<MainTicketDetailModel> GetMainTicketDetails(string transIds, int? transSize, int? matchSize)
        {           
            var criteria = new GetTicketDetailsCriteria
            {
                ListTransId = transIds,
                TransSize = transSize,
                MatchSize = matchSize
            };

            using (var db = ObjectDbFactory.CreateInstance(criteria.GetSettingKey()))
            {
                return db.Query<MainTicketDetailModel>(criteria);
            }
        }

        public IEnumerable<CCGroupModel> GetGroupCustomerClass()
        {
            var groupCCList = Enumerable.Empty<CCGroupModel>();
            var criteria = new GetCommonCustomerClassCriteria();             

            using (var db = ObjectDbFactory.CreateInstance(criteria.GetSettingKey()))
            {
                var data = db.Query<CommonCustomerClassModel>(criteria);

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

            return repository.Fetch<StaticListModel>(criteria);
        }

        public IEnumerable<BetTypeSettingModel> GetBetTypeSettings(BetTypeSettingParaModel infos)
        {
            var criteria = new GetCommonBetTypeSettingCriteria
            {
                FunctionID = infos.FunctionID,
                BettypeIDList = infos.BettypeIDList,
                SportTypeIDList = infos.SportTypeIDList,
                IsFilterMMDetection = infos.IsFilterMMDetection,
                IsShowDetailsX = infos.IsShowDetailsX,
                Status = infos.Status
            };

            return repository.Fetch<BetTypeSettingModel>(criteria);
        }

        public IEnumerable<SportBettypeViewModel> GetMainSportBettype(SportBettypeModel model)
        {
            var criteria = new GetCommonSportBettypeCriteria
            {
                ProductId = model.ProductId,
                BettypeList = model.BettypeList
            };

            return repository.Fetch<SportBettypeViewModel>(criteria);
        }

        public IEnumerable<LeagueGroupViewModel> GetLeagueGroup()
        {
            //var criteria = new GetCommonLeagueGroupCriteria();

            //return repository.Fetch<LeagueGroupViewModel>(criteria);

            var json = "[{\"GroupID\":0,\"GroupName\":\"None\",\"SportType\":0,\"IsActive\":true},{\"GroupID\":1,\"GroupName\":\"MajorLeague\",\"SportType\":0,\"IsActive\":true},{\"GroupID\":2,\"GroupName\":\"MinorLeague1\",\"SportType\":0,\"IsActive\":true},{\"GroupID\":3,\"GroupName\":\"MinorLeague2\",\"SportType\":0,\"IsActive\":true},{\"GroupID\":4,\"GroupName\":\"MinorLeague3\",\"SportType\":0,\"IsActive\":true},{\"GroupID\":5,\"GroupName\":\"MinorLeague4\",\"SportType\":0,\"IsActive\":true},{\"GroupID\":6,\"GroupName\":\"Others\",\"SportType\":0,\"IsActive\":true},{\"GroupID\":7,\"GroupName\":\"Others1\",\"SportType\":1,\"IsActive\":true},{\"GroupID\":8,\"GroupName\":\"Others2\",\"SportType\":1,\"IsActive\":true},{\"GroupID\":9,\"GroupName\":\"Others3\",\"SportType\":1,\"IsActive\":true},{\"GroupID\":10,\"GroupName\":\"Others4\",\"SportType\":1,\"IsActive\":true},{\"GroupID\":11,\"GroupName\":\"Others5\",\"SportType\":1,\"IsActive\":true},{\"GroupID\":12,\"GroupName\":\"Specials\",\"SportType\":1,\"IsActive\":true},{\"GroupID\":13,\"GroupName\":\"Specific10min\",\"SportType\":1,\"IsActive\":true},{\"GroupID\":14,\"GroupName\":\"Specific15min\",\"SportType\":1,\"IsActive\":true},{\"GroupID\":16,\"GroupName\":\"ArenaofValor\",\"SportType\":43,\"IsActive\":true},{\"GroupID\":17,\"GroupName\":\"Artifact\",\"SportType\":43,\"IsActive\":true},{\"GroupID\":18,\"GroupName\":\"CallofDuty\",\"SportType\":43,\"IsActive\":true},{\"GroupID\":19,\"GroupName\":\"CS:GO\",\"SportType\":43,\"IsActive\":true},{\"GroupID\":20,\"GroupName\":\"Dota2\",\"SportType\":43,\"IsActive\":true},{\"GroupID\":21,\"GroupName\":\"FIFA\",\"SportType\":43,\"IsActive\":true},{\"GroupID\":22,\"GroupName\":\"Hearthstone\",\"SportType\":43,\"IsActive\":true},{\"GroupID\":23,\"GroupName\":\"HeroesoftheStorm\",\"SportType\":43,\"IsActive\":true},{\"GroupID\":24,\"GroupName\":\"KingofGlory\",\"SportType\":43,\"IsActive\":true},{\"GroupID\":25,\"GroupName\":\"LeagueofLegends\",\"SportType\":43,\"IsActive\":true},{\"GroupID\":26,\"GroupName\":\"NBA2k\",\"SportType\":43,\"IsActive\":true},{\"GroupID\":27,\"GroupName\":\"Overwatch\",\"SportType\":43,\"IsActive\":true},{\"GroupID\":28,\"GroupName\":\"PUBG\",\"SportType\":43,\"IsActive\":true},{\"GroupID\":29,\"GroupName\":\"RainbowSix\",\"SportType\":43,\"IsActive\":true},{\"GroupID\":30,\"GroupName\":\"RocketLeague\",\"SportType\":43,\"IsActive\":true},{\"GroupID\":31,\"GroupName\":\"Starcraft\",\"SportType\":43,\"IsActive\":true},{\"GroupID\":32,\"GroupName\":\"StarCraft2\",\"SportType\":43,\"IsActive\":true},{\"GroupID\":33,\"GroupName\":\"StreetFighterV\",\"SportType\":43,\"IsActive\":true},{\"GroupID\":34,\"GroupName\":\"Warcraft3\",\"SportType\":43,\"IsActive\":true},{\"GroupID\":35,\"GroupName\":\"WorldofWarcraft\",\"SportType\":43,\"IsActive\":true},{\"GroupID\":36,\"GroupName\":\"Paladins\",\"SportType\":43,\"IsActive\":true},{\"GroupID\":37,\"GroupName\":\"MobileLegends\",\"SportType\":43,\"IsActive\":true},{\"GroupID\":38,\"GroupName\":\"ClashRoyale\",\"SportType\":43,\"IsActive\":true},{\"GroupID\":39,\"GroupName\":\"Fortnite\",\"SportType\":43,\"IsActive\":true},{\"GroupID\":40,\"GroupName\":\"GameForPeace\",\"SportType\":43,\"IsActive\":true},{\"GroupID\":41,\"GroupName\":\"Esoccer\",\"SportType\":1,\"IsActive\":true},{\"GroupID\":42,\"GroupName\":\"Virtual\",\"SportType\":0,\"IsActive\":true},{\"GroupID\":43,\"GroupName\":\"3X31\",\"SportType\":2,\"IsActive\":true},{\"GroupID\":44,\"GroupName\":\"3X32\",\"SportType\":2,\"IsActive\":true},{\"GroupID\":45,\"GroupName\":\"3X33\",\"SportType\":2,\"IsActive\":true},{\"GroupID\":46,\"GroupName\":\"AFRICA5\",\"SportType\":2,\"IsActive\":true},{\"GroupID\":47,\"GroupName\":\"AMERICAS1\",\"SportType\":2,\"IsActive\":true},{\"GroupID\":48,\"GroupName\":\"AMERICAS2\",\"SportType\":2,\"IsActive\":true},{\"GroupID\":49,\"GroupName\":\"AMERICAS3\",\"SportType\":2,\"IsActive\":true},{\"GroupID\":50,\"GroupName\":\"AMERICAS4\",\"SportType\":2,\"IsActive\":true},{\"GroupID\":51,\"GroupName\":\"AMERICAS5\",\"SportType\":2,\"IsActive\":true},{\"GroupID\":52,\"GroupName\":\"ASIA1\",\"SportType\":2,\"IsActive\":true},{\"GroupID\":53,\"GroupName\":\"ASIA2\",\"SportType\":2,\"IsActive\":true},{\"GroupID\":54,\"GroupName\":\"ASIA3\",\"SportType\":2,\"IsActive\":true},{\"GroupID\":55,\"GroupName\":\"ASIA4\",\"SportType\":2,\"IsActive\":true},{\"GroupID\":56,\"GroupName\":\"ASIA5\",\"SportType\":2,\"IsActive\":true},{\"GroupID\":57,\"GroupName\":\"EUROPE1\",\"SportType\":2,\"IsActive\":true},{\"GroupID\":58,\"GroupName\":\"EUROPE2\",\"SportType\":2,\"IsActive\":true},{\"GroupID\":59,\"GroupName\":\"EUROPE3\",\"SportType\":2,\"IsActive\":true},{\"GroupID\":60,\"GroupName\":\"EUROPE4\",\"SportType\":2,\"IsActive\":true},{\"GroupID\":61,\"GroupName\":\"EUROPE5\",\"SportType\":2,\"IsActive\":true},{\"GroupID\":62,\"GroupName\":\"INTERNATIONAL1\",\"SportType\":2,\"IsActive\":true},{\"GroupID\":63,\"GroupName\":\"INTERNATIONAL2\",\"SportType\":2,\"IsActive\":true},{\"GroupID\":64,\"GroupName\":\"INTERNATIONAL3\",\"SportType\":2,\"IsActive\":true},{\"GroupID\":65,\"GroupName\":\"INTERNATIONAL4\",\"SportType\":2,\"IsActive\":true},{\"GroupID\":66,\"GroupName\":\"INTERNATIONAL5\",\"SportType\":2,\"IsActive\":true},{\"GroupID\":67,\"GroupName\":\"SPECIALS\",\"SportType\":2,\"IsActive\":true},{\"GroupID\":68,\"GroupName\":\"USA1\",\"SportType\":2,\"IsActive\":true},{\"GroupID\":69,\"GroupName\":\"USA2\",\"SportType\":2,\"IsActive\":true},{\"GroupID\":70,\"GroupName\":\"USA3\",\"SportType\":2,\"IsActive\":true},{\"GroupID\":71,\"GroupName\":\"USAOTHER\",\"SportType\":2,\"IsActive\":true},{\"GroupID\":72,\"GroupName\":\"VirtualGamer\",\"SportType\":0,\"IsActive\":true},{\"GroupID\":73,\"GroupName\":\"Futsal\",\"SportType\":1,\"IsActive\":true},{\"GroupID\":74,\"GroupName\":\"VirtualSportBacarrat\",\"SportType\":1,\"IsActive\":true},{\"GroupID\":75,\"GroupName\":\"Grandslam\",\"SportType\":5,\"IsActive\":true},{\"GroupID\":76,\"GroupName\":\"Valorant\",\"SportType\":43,\"IsActive\":true},{\"GroupID\":77,\"GroupName\":\"MajorTournament1\",\"SportType\":1,\"IsActive\":true},{\"GroupID\":78,\"GroupName\":\"MajorTournament2\",\"SportType\":1,\"IsActive\":true},{\"GroupID\":79,\"GroupName\":\"ClubMajorQualifier\",\"SportType\":1,\"IsActive\":true},{\"GroupID\":80,\"GroupName\":\"ClubMinorQualifier\",\"SportType\":1,\"IsActive\":true},{\"GroupID\":81,\"GroupName\":\"InternationalMajorQualifiers\",\"SportType\":1,\"IsActive\":true},{\"GroupID\":82,\"GroupName\":\"InternationalMinorQualifiers\",\"SportType\":1,\"IsActive\":true},{\"GroupID\":83,\"GroupName\":\"CSGOTier2\",\"SportType\":43,\"IsActive\":true},{\"GroupID\":84,\"GroupName\":\"CSGOTier3\",\"SportType\":43,\"IsActive\":true},{\"GroupID\":85,\"GroupName\":\"CSGOTier4\",\"SportType\":43,\"IsActive\":true},{\"GroupID\":86,\"GroupName\":\"CSGOTier5\",\"SportType\":43,\"IsActive\":true},{\"GroupID\":87,\"GroupName\":\"Dota2Tier2\",\"SportType\":43,\"IsActive\":true},{\"GroupID\":88,\"GroupName\":\"Dota2Tier3\",\"SportType\":43,\"IsActive\":true},{\"GroupID\":89,\"GroupName\":\"Dota2Tier4\",\"SportType\":43,\"IsActive\":true},{\"GroupID\":90,\"GroupName\":\"LeagueOfLegendsTier2\",\"SportType\":43,\"IsActive\":true},{\"GroupID\":91,\"GroupName\":\"LeagueOfLegendsTier3\",\"SportType\":43,\"IsActive\":true},{\"GroupID\":92,\"GroupName\":\"LeagueOfLegendsTier4\",\"SportType\":43,\"IsActive\":true},{\"GroupID\":93,\"GroupName\":\"Dota2Tier5\",\"SportType\":43,\"IsActive\":true},{\"GroupID\":94,\"GroupName\":\"LeagueOfLegendsTier5\",\"SportType\":43,\"IsActive\":true},{\"GroupID\":95,\"GroupName\":\"KingofGloryTier2\",\"SportType\":43,\"IsActive\":true},{\"GroupID\":96,\"GroupName\":\"KingofGloryTier3\",\"SportType\":43,\"IsActive\":true},{\"GroupID\":97,\"GroupName\":\"KingofGloryTier4\",\"SportType\":43,\"IsActive\":true},{\"GroupID\":98,\"GroupName\":\"KingofGloryTier5\",\"SportType\":43,\"IsActive\":true},{\"GroupID\":99,\"GroupName\":\"TestingLeagueGroup\",\"SportType\":0,\"IsActive\":true},{\"GroupID\":100,\"GroupName\":\"WorldCup\",\"SportType\":1,\"IsActive\":true},{\"GroupID\":101,\"GroupName\":\"EURO\",\"SportType\":1,\"IsActive\":true},{\"GroupID\":102,\"GroupName\":\"WorldCupSpecials\",\"SportType\":1,\"IsActive\":true},{\"GroupID\":103,\"GroupName\":\"WorldCupSpecific10min\",\"SportType\":1,\"IsActive\":true},{\"GroupID\":104,\"GroupName\":\"WorldCupSpecific15min\",\"SportType\":1,\"IsActive\":true},{\"GroupID\":105,\"GroupName\":\"EUROSpecials\",\"SportType\":1,\"IsActive\":true},{\"GroupID\":106,\"GroupName\":\"EUROSpecific10min\",\"SportType\":1,\"IsActive\":true},{\"GroupID\":107,\"GroupName\":\"EUROSpecific15min\",\"SportType\":1,\"IsActive\":true},{\"GroupID\":108,\"GroupName\":\"VirtualFutsal\",\"SportType\":1,\"IsActive\":true},{\"GroupID\":109,\"GroupName\":\"CrossFire\",\"SportType\":43,\"IsActive\":true},{\"GroupID\":111,\"GroupName\":\"LeagueofLegends:WildRift\",\"SportType\":43,\"IsActive\":true},{\"GroupID\":112,\"GroupName\":\"Streamer\",\"SportType\":43,\"IsActive\":true},{\"GroupID\":113,\"GroupName\":\"SABAPinGoal\",\"SportType\":1,\"IsActive\":true},{\"GroupID\":122,\"GroupName\":\"SABABasketballPinGoal\",\"SportType\":2,\"IsActive\":true}]";

            var result = JsonConvert.DeserializeObject<List<LeagueGroupViewModel>>(json);

            return result;
        }

        public void CheckConnection()
        {
            using (var db = ObjectDbFactory.CreateInstance(new HealthCheckCriteria().GetSettingKey()))
            {
                db.CreateDbConnection().Open();
            }
        }
    }
}