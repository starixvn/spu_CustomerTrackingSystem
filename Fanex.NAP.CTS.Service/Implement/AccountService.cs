namespace Fanex.NAP.CTS.Service
{
    using Fanex.Data;
    using Fanex.Data.Repository;
    using Fanex.Logging;
    using Fanex.NAP.CTS.Core;
    using Fanex.NAP.CTS.Core.BaseClasses;
    using Fanex.NAP.CTS.Core.Extentions;
    using Fanex.NAP.CTS.Core.Helpers;
    using Fanex.NAP.CTS.Criteria.Account;
    using Fanex.NAP.CTS.Criteria.Account.Associated;
    using Fanex.NAP.CTS.Criteria.Account.BettingStatistic;
    using Fanex.NAP.CTS.Criteria.Account.ChainToEvidence;
    using Fanex.NAP.CTS.Criteria.Account.CustomerCategory;
    using Fanex.NAP.CTS.Criteria.Account.CustomerCategoryBySport;
    using Fanex.NAP.CTS.Criteria.Account.CustomerInfo;
    using Fanex.NAP.CTS.Criteria.Home;
    using Fanex.NAP.CTS.Criteria.Transaction;
    using Fanex.NAP.CTS.Model;
    using Fanex.NAP.CTS.Model.Account;
    using Fanex.NAP.CTS.Model.Account.AssociatedAccount;
    using Fanex.NAP.CTS.Model.Account.CustomerCategoryBySport;
    using Fanex.NAP.CTS.Model.Account.CustomerInfo;
    using Fanex.NAP.CTS.Model.Association;
    using Fanex.NAP.CTS.Model.CustomerClassification;
    using Fanex.NAP.CTS.Model.DTO;
    using Fanex.NAP.CTS.Model.Home;
    using Fanex.NAP.CTS.Model.Transaction;
    using Fanex.NAP.CTS.Resource.Consts;
    using Fanex.NAP.CTS.ViewModel.Account.Associated;
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;
    using static Fanex.NAP.CTS.Resource.Consts.Consts;

    public class AccountService : IAccountService
    {
        private readonly IDynamicRepository repository;

        public AccountService()
        {
            repository = new DynamicRepository();
        }

        #region Customer Profile

        public IEnumerable<CustInfoModel> GetCustInfoByCustID(string custIDs)
        {
            var criteria = new CustInfoByCustIdGetCriteria()
            {
                CustIDs = custIDs
            };

            return repository.Fetch<CustInfoModel>(criteria);
        }

        public CustomerInfoByCTSCustIdModel GetCustomerInfoByCTSCustId(Int64 ctsCustId)
        {
            var criteria = new CustomerInfoByCTSCustIdCriteria
            {
                CTSCustId = ctsCustId
            };

            using (var db = ObjectDbFactory.CreateInstance(criteria.GetSettingKey()))
            {
                var result = db.QueryEntity<CustomerInfoByCTSCustIdModel>(criteria);

                return result;
            }
        }

        public CustomerInfoByCustIdsModel GetCustomerInfoDangerous(long custId)
        {
            var criteria = new GetCustomerInfoDangerousCriteria
            {
                CustIdList = custId.ToString(),
            };

            using (var db = ObjectDbFactory.CreateInstance(criteria.GetSettingKey()))
            {
                var result = db.QueryEntity<CustomerInfoByCustIdsModel>(criteria);

                return result;
            }
        }

        public CustomerInfoAssociationAccountModel GetCustomerInfoAssociationAccount(long ctsCustId)
        {
            var criteria = new GetCustomerInfoAssociatedAccountCriteria
            {
                CTSCustId = ctsCustId
            };

            using (var db = ObjectDbFactory.CreateInstance(criteria.GetSettingKey()))
            {
                return db.QueryEntity<CustomerInfoAssociationAccountModel>(criteria);
            }
        }

        public IEnumerable<CustomerInfoEvidenceModel> GetCustomerInfoEvidence(long ctsCustId)
        {
            var criteria = new GetCustomerInfoEvidenceCriteria
            {
                CTSCustId = ctsCustId
            };

            using (var db = ObjectDbFactory.CreateInstance(criteria.GetSettingKey()))
            {
                return db.Query<CustomerInfoEvidenceModel>(criteria);
            }
        }

        public CustomerInfoLoginStatsModel GetCustomerInfoLoginStats(long ctsCustId, DateTime fromDate, DateTime toDate)
        {
            var criteria = new GetCustomerInfoLoginStatsCriteria
            {
                CTSCustID = ctsCustId,
                FromDate = fromDate,
                ToDate = toDate
            };

            using (var db = ObjectDbFactory.CreateInstance(criteria.GetSettingKey()))
            {
                return db.QueryEntity<CustomerInfoLoginStatsModel>(criteria);
            }
        }

        public CustomerInfoDateModel GetCustProfileDates(long ctsCustId)
        {
            var criteria = new GetCustomerInfoDateCriteria
            {
                CTSCustID = ctsCustId
            };

            using (var db = ObjectDbFactory.CreateInstance(criteria.GetSettingKey()))
            {
                return db.QueryEntity<CustomerInfoDateModel>(criteria);
            }
        }

        #endregion

        #region Associated account by account
        public List<SearchResultModel> SearchAccounts(string username)
        {
            var criteria = new SearchResultCriteria
            {
                Username = username
            };

            using (var db = ObjectDbFactory.CreateInstance(criteria.GetSettingKey()))
            {
                var result = db.Query<SearchResultModel, Currency, Role, Subscriber, SearchResultModel>((model, currency, role, subscriber) =>
                {
                    model.Currency = currency;
                    model.Role = role;
                    model.Subscriber = subscriber;

                    return model;
                },
                "CurrencyId, RoleId, SubscriberId",
                criteria).ToList();

                return result;
            }
        }

        public List<CustomerEvidenceByCTSCustIdModel> GetCustomerEvidenceByCTSCustId(long ctsCustId)
        {
            var criteria = new CustomerEvidenceByCTSCustIdCriteria
            {
                CTSCustId = ctsCustId
            };

            using (var db = ObjectDbFactory.CreateInstance(criteria.GetSettingKey()))
            {
                var result = db.Query<CustomerEvidenceByCTSCustIdModel>(criteria);

                return result.ToList();
            }
        }

        public List<CustomerStaticStatusModel> GetCustomerStaticStatus(int listId, bool status)
        {
            var criteria = new CustomerStaticAccountStatusCriteria
            {
                ListId = listId,
                Status = status
            };

            using (var db = ObjectDbFactory.CreateInstance(criteria.GetSettingKey()))
            {
                var result = db.Query<CustomerStaticStatusModel>(criteria);

                return result.ToList();
            }
        }

        public CustomerProfileFraudInfoModel GetCustomerProfileFraudInfo(long custId)
        {
            var criteria = new CustomerProfileFraudInfoCriteria
            {
                CustId = custId
            };

            using (var db = ObjectDbFactory.CreateInstance(criteria.GetSettingKey()))
            {
                var fraudCaseList = db.Query<FraudCaseModel>(criteria).ToList();

                return new CustomerProfileFraudInfoModel
                {
                    FraudCaseList = fraudCaseList
                };
            }
        }

        public List<TagModel> GetDirectAssociatedAccountTagFlagged(string custIdsAssInfo)
        {
            var criteria = new GetDirectAssociatedAccountTagFlaggedCriteria
            {
                CustAssInfo = custIdsAssInfo
            };

            using (var db = ObjectDbFactory.CreateInstance(criteria.GetSettingKey()))
            {
                var result = db.Query<TagModel>(criteria);

                return result.ToList();
            }
        }   

        public CustomerFlagInfoModel GetCustomerFlagInfo(Int64 ctsCustId)
        {
            var criteria = new CustomerFlagInfoByCTSCustIdCriteria
            {
                CTSCustId = ctsCustId
            };

            using (var db = ObjectDbFactory.CreateInstance(criteria.GetSettingKey()))
            {
                var result = db.QueryEntity<CustomerFlagInfoModel>(criteria);

                return result;
            }
        }

        public AssociatedAccountPagingModel GetAssociatedAccount(AssociatedFilterModel filterModel)
        {
            var criteria = new GetAssociationProfileCriteria
            {
                CTSCustId = filterModel.CTSCustId,
                AssociationStatus = filterModel.AssociationStatus,
                AccountStatusIds = filterModel.AccountStatusIds,
                AssociationType = filterModel.AssociationType,
                CategoryIDs = filterModel.CategoryIds,
                SubscriberIDs = filterModel.SubscriberIds,
                Skip = filterModel.start,
                Take = filterModel.length,
                TotalItems = 0
            };

            using (var db = ObjectDbFactory.CreateInstance(criteria.GetSettingKey()))
            {
                var result = db.Query<AssociatedAccountModel>(criteria);

                return new AssociatedAccountPagingModel
                {
                    AssociationList = result.ToList(),
                    TotalItems = criteria.TotalItems
                };
            }
        }

        public List<AccountStaticStatusModel> GetAccountStaticStatuses(int listId, bool status)
        {
            var groupStatuses = new List<AccountStaticStatusModel>();

            var accountStatuses = GetCustomerStaticStatus(listId, status)?.OrderBy(o => o.PriorityOrder);

            if (accountStatuses != null && accountStatuses.Any())
            {
                groupStatuses.Add(new AccountStaticStatusModel
                {
                    Ids = string.Join(",", accountStatuses.Select(s => s.ItemId).ToList()),
                    Name = Enum.GetName(typeof(ACCOUNT_STATUS), ACCOUNT_STATUS.All)
                });

                accountStatuses.GroupBy(g => g.PriorityOrder).ToList().ForEach(c =>
                {
                    groupStatuses.Add(new AccountStaticStatusModel
                    {
                        Ids = string.Join(",", c.Select(s => s.ItemId).ToList()),
                        Name = Enum.GetName(typeof(ACCOUNT_STATUS), c.Key)
                    });
                });
            }

            return groupStatuses;
        }       

        public IEnumerable<ValidateManualAssociationMultipleModel> ValidateManualAssociationMultiple(long fromCTSCustId, string toUserNameList, out bool isException)
        {
            isException = false;

            var criteria = new ValidateManualAssociationMultipleCriteria
            {
                ToUserNameList = toUserNameList,
                FromCTSCustID = fromCTSCustId
            };

            try
            {
                using (var db = ObjectDbFactory.CreateInstance(criteria.GetSettingKey()))
                {
                    return db.Query<ValidateManualAssociationMultipleModel>(criteria).ToList();
                }
            }
            catch (Exception ex)
            {
                isException = true;
                LogService.Error("CTS - Validate manual association multiple - exception: " + ex.Message, ex);

                return GenerateAssociationErrorList(fromCTSCustId, toUserNameList);
            }
        }

        private IEnumerable<ValidateManualAssociationMultipleModel> GenerateAssociationErrorList(long fromCTSCustId, string toUserNameList)
        {
            var errorList = new List<ValidateManualAssociationMultipleModel>();
            var userNameList = toUserNameList.Split(',').AsEnumerable();

            errorList.AddRange(userNameList.Select(x => new ValidateManualAssociationMultipleModel
            {
                CTSCustID = 0,
                SubscriberID = 0,
                UserName = x,
                IsDuplicated = false
            }));

            return errorList;
        }

        public DBResult InsertManualAssociationMultiple(string associationListJson)
        {
            var criteria = new InsertManualAssociationMultipleCriteria
            {
                AssociationListJson = associationListJson
            };

            using (var db = ObjectDbFactory.CreateInstance(criteria.GetSettingKey()))
            {
                try
                {
                    db.ExecuteNonQuery(criteria);

                    return new DBResult
                    {
                        ErrorCode = 0,
                        ErrorMsg = string.Empty
                    };
                }
                catch (Exception ex)
                {
                    LogService.Error("CTS - Insert manual association multiple - exception: " + ex.Message, ex);

                    return new DBResult
                    {
                        ErrorCode = -1,
                        ErrorMsg = ex.Message
                    };
                }
            }
        }

        public DBResult ActiveRemoveAssociation(int fromSubscriberId, long fromCTSCustId, int toSubscriberId, long toCTSCustId, string remark, int createdBy, bool isRemoveAction)
        {
            var criteria = new ActiveRemoveAssociationCriteria
            {
                FromSubscriberId = fromSubscriberId,
                FromCTSCustId = fromCTSCustId,
                ToSubscriberId = toSubscriberId,
                ToCTSCustId = toCTSCustId,
                CreatedBy = createdBy,
                Remark = remark,
                IsActionRemove = isRemoveAction,
                ErrorMessage = string.Empty
            };

            using (var db = ObjectDbFactory.CreateInstance(criteria.GetSettingKey()))
            {
                db.ExecuteNonQuery(criteria);

                return new DBResult
                {
                    ErrorCode = string.IsNullOrWhiteSpace(criteria.ErrorMessage) ? 0 : -1,
                    ErrorMsg = criteria.ErrorMessage
                };
            }
        }
      
        public MatchesAssociationByAIModel GetGBAssociationFromAI(long custID1, long custID2, string groupList)
        {
            var result = new MatchesAssociationByAIModel();

            var criteria = new GetMatchesAssociationByAICriteria
            {
                CustID1 = custID1,
                CustID2 = custID2,
                GroupList = groupList
            };
            result.MatchesInfo = repository.Fetch<GBAssociationAIModel>(criteria) ?? Enumerable.Empty<GBAssociationAIModel>();
            result.ErrorMessage = criteria.ErrorMessage;
            result.ErrorCode = string.IsNullOrEmpty(criteria.ErrorMessage) ? 0 : -1;
            result.TotalMatchCount = (int)criteria.TotalMatchCount;

            return result;
        }

        public IEnumerable<MatchesModel> GetMatchInfosByIDs(string matchIDs)
        {
            var criteria = new GetMatchesInfoCriteria
            {
                MatchIds = matchIDs
            };

            using (var db = ObjectDbFactory.CreateInstance(criteria.GetSettingKey()))
            {
                return db.Query<MatchesModel>(criteria);
            }
        }

        public IEnumerable<MatchesInfoModel> GetBettingPatternMatchesInfo(string matchIDs, string custIds)
        {
            var criteria = new GetBettingPatternMatchesInfoCriteria
            {
                MatchIds = matchIDs,
                CustIds = custIds
            };

            using (var db = ObjectDbFactory.CreateInstance(criteria.GetSettingKey()))
            {
                return db.Query<MatchesInfoModel>(criteria);
            }
        }

        public IEnumerable<CustomerGroupModel> GetCustGroupAssociation(string originalGroupIDs)
        {
            if (!string.IsNullOrEmpty(originalGroupIDs))
            {
                var criteria = new GetCustomerGroupCriteria
                {
                    GroupList = originalGroupIDs
                };

                return repository.Fetch<CustomerGroupModel>(criteria) ?? Enumerable.Empty<CustomerGroupModel>();
            }

            return Enumerable.Empty<CustomerGroupModel>();
        }

        public IEnumerable<MatchesDetailInfoModel> GetTicketDetails(MatchesAssociationDetailViewModel macthInfo, string custIds, string betTypes)
        {
            if (!string.IsNullOrEmpty(custIds))
            {
                var criteria = new GetMatchesDetailInfoCriteria
                {
                    EventDate = macthInfo.EventDate,
                    Bettypes = betTypes,
                    CustIds = custIds,
                    MatchId = macthInfo.MatchId
                };
                return repository.Fetch<MatchesDetailInfoModel>(criteria) ?? Enumerable.Empty<MatchesDetailInfoModel>();
            }

            return Enumerable.Empty<MatchesDetailInfoModel>();
        }

        public IEnumerable<MatchesDetailInfoViewModel> MergeCustGroupTicketInfo(IEnumerable<MatchCustomerGGBViewModel> custGroups, IEnumerable<MatchesDetailInfoModel> ticketDetails, bool isGGB)
        {
            var result = new List<MatchesDetailInfoViewModel>();

            if (isGGB && custGroups != null && custGroups.Any() && ticketDetails != null && ticketDetails.Any())
            {
                result = custGroups.Join(ticketDetails,
                                    c => c.CustID,
                                    t => t.CustID,
                                    (c, t) => new MatchesDetailInfoViewModel
                                    {
                                        IsGGB = isGGB,
                                        GroupID = c.GroupID,
                                        AdditionalInfo = c.AdditionalInfo,
                                        UserName = t.UserName,
                                        TransDate = t.TransDate,
                                        TransDateString = t.TransDate.ToString(Consts.FormatDateToString_MMMddyyyyhhmmss),
                                        TransId = t.TransId.ToString(),
                                        BetType = t.TypeName,
                                        BetTeam = t.BetTeam,
                                        Stake = t.Stake,
                                        Odds = t.Odds,
                                        LiveScore = t.LiveHomeScore + "-" + t.LiveAwayScore,
                                        Currency = t.CurrencyName,
                                        StakeRM = Math.Truncate(t.StakeRM),
                                        IP = t.IP
                                    })?.OrderBy(c => c.GroupID).ThenByDescending(c => c.TransDate).ToList();
            }
            else if (!isGGB && ticketDetails != null && ticketDetails.Any())
            {
                ticketDetails.ToList().ForEach(c =>
                {
                    result.Add(new MatchesDetailInfoViewModel
                    {
                        IsGGB = isGGB,
                        GroupID = 0,
                        UserName = c.UserName,
                        TransDate = c.TransDate,
                        TransDateString = c.TransDate.ToString(Consts.FormatDateToString_MMMddyyyyhhmmss),
                        TransId = c.TransId.ToString(),
                        BetType = c.TypeName,
                        BetTeam = c.BetTeam,
                        Stake = c.Stake,
                        Odds = c.Odds,
                        LiveScore = c.LiveHomeScore + "-" + c.LiveAwayScore,
                        Currency = c.CurrencyName,
                        StakeRM = Math.Truncate(c.StakeRM),
                        IP = c.IP
                    });
                });

                result?.OrderBy(x => x.TransDate);
            }

            return result ?? Enumerable.Empty<MatchesDetailInfoViewModel>();
        }

        public IEnumerable<MatchCustomerGGBViewModel> MergeCustMatchInfo(IEnumerable<GBAssociationAIModel> matchInfo, IEnumerable<CustomerGroupModel> groupInfo)
        {
            if (matchInfo != null && matchInfo.Any() && groupInfo != null && groupInfo.Any())
            {
                return matchInfo.Join(groupInfo,
                                    m => m.GroupID,
                                    g => g.OriginGroupID,
                                    (m, g) => new MatchCustomerGGBViewModel
                                    {
                                        CustID = g.CustID,
                                        GroupID = g.GroupID,
                                        AdditionalInfo = m.AdditionalInfo
                                    }) ?? Enumerable.Empty<MatchCustomerGGBViewModel>();
            }

            return Enumerable.Empty<MatchCustomerGGBViewModel>();
        }

        public IEnumerable<CategoryInfoModel> GetAllCategory()
        {
            var criteria = new GetCategoryInfoCriteria();

            using (var db = ObjectDbFactory.CreateInstance(criteria.GetSettingKey()))
            {
                return db.Query<CategoryInfoModel>(criteria);
            }
        }

        public AssociationByIPInfoModel GetAssociationByIPInfo(long custId1, long custId2)
        {
            var criteria = new GetAssociationByIPInfoCriteria
            {
                CustId1 = custId1,
                CustId2 = custId2
            };

            using (var db = ObjectDbFactory.CreateInstance(criteria.GetSettingKey()))
            {
                return new AssociationByIPInfoModel
                {
                    ItemList = db.Query<AssociationByIPItemModel>(criteria),
                    ErrorMessage = criteria.ErrorMessage
                };
            }
        }

        public IEnumerable<int> GetAssociationByIPType(long custID1, long custID2)
        {
            var criteria = new GetAssociationByIPTypeCriteria
            {
                CustID1 = custID1,
                CustID2 = custID2
            };

            using (var db = ObjectDbFactory.CreateInstance(criteria.GetSettingKey()))
            {
                return db.Query<int>(criteria);
            }
        }

        public IEnumerable<AssByIPTicketStakeModel> GetAssociationBy5GIPDetail(long custID1, long custID2)
        {
            var criteria = new GetAssociationBy5GIPDetailCriteria
            {
                CustID1 = custID1,
                CustID2 = custID2
            };

            using (var db = ObjectDbFactory.CreateInstance(criteria.GetSettingKey()))
            {
                return db.Query<AssByIPTicketStakeModel>(criteria);
            }
        }

        public CoupleInfoViewModel GetCoupleInfos(long custID1, long custID2)
        {
            var criteria = new GetCoupleInfoCriteria
            {
                CustId1 = custID1,
                CustId2 = custID2
            };

            var result = new CoupleInfoViewModel();

            using (var db = ObjectDbFactory.CreateInstance(criteria.GetSettingKey()))
            {
                result.CoupleInfo = db.Query<CoupleInfoModel>(criteria);
            }

            return result;
        }

        public IEnumerable<DateTime> GetAIFristAssociationDate(long custID1, long custID2, AccountAssociation_ByAIPattern pattern)
        {
            var criteria = new GetAI5G3SFirstAssociationCriteria
            {
                CustId1 = custID1,
                CustId2 = custID2,
                AssType = (int)pattern
            };

            return repository.Fetch<DateTime>(criteria);
        }

        public IEnumerable<CoupleFraudDetailModel> GetCoupleFraudDetails(string coupleInfoJson)
        {
            var criteria = new GetCoupleFraudDetailCriteria
            {
                CoupleInfoDetails = coupleInfoJson
            };

            return repository.Fetch<CoupleFraudDetailModel>(criteria);
        }

        public IEnumerable<CoupleIPFraudDetailModel> GetCoupleIPFraudDetails(string coupleInfoJson)
        {
            var criteria = new GetCoupleIPFraudDetailCriteria
            {
                CoupleInfoDetails = coupleInfoJson
            };
            using (var db = ObjectDbFactory.CreateInstance(criteria.GetSettingKey()))
            {
                return db.Query<CoupleIPFraudDetailModel>(criteria);
            }
        }

        public IEnumerable<TicketDetailsModel> GetAIAssociationTicketDetails(string transIds, int? transSize, int? matchSize)
        {
            var result = new List<TicketDetailsModel>();

            var criteria = new GetTicketDetailsCriteria()
            {
                ListTransId = transIds,
                TransSize = transSize,
                MatchSize = matchSize
            };
            using (var db = ObjectDbFactory.CreateInstance(criteria.GetSettingKey()))
            {
                return db.Query<TicketDetailsModel>(criteria);
            }
        }

        public AssociationOTGBViewModel GetAssociationByOTGB(long custID1, long custID2, AccountAssociationByOTGB fraudType)
        {
            var result = new AssociationOTGBViewModel();

            var criteria = new GetAssociationByOTGBCriteria
            {
                CustId1 = custID1,
                CustId2 = custID2,
                FraudType = (int)fraudType
            };

            var db = ObjectDbFactory.CreateInstance(criteria.GetSettingKey());

            var data = db.QueryMultiple(criteria);

            if (fraudType != AccountAssociationByOTGB.CheckExist)
            {
                result.AssociationOTGBRoun1 = data?.Read<AssociationOTGBModel>() ?? Enumerable.Empty<AssociationOTGBModel>();
                result.AssociationOTGBRoun2 = data?.Read<AssociationOTGBModel>() ?? Enumerable.Empty<AssociationOTGBModel>();
                result.AssociationOTGBRoun3 = data?.Read<AssociationOTGBModel>() ?? Enumerable.Empty<AssociationOTGBModel>();
            }
            else
            {
                result.ScoreRound1 = data?.Read<bool>().FirstOrDefault() ?? false;
                result.ScoreRound2 = data?.Read<bool>().FirstOrDefault() ?? false;
                result.ScoreRound3 = data?.Read<bool>().FirstOrDefault() ?? false;
            }

            return result;
        }

        #endregion

        #region Chain To Evidence
        public IEnumerable<EvidenceModel> GetEvidenceList()
        {
            var criteria = new GetAllEvidenceCriteria();

            using (var db = ObjectDbFactory.CreateInstance(criteria.GetSettingKey()))
            {
                return db.Query<EvidenceModel>(criteria);
            }
        }

        public IEnumerable<FlaggedEvidenceModel> GetFlaggedEvidences(Int64 ctsCustId, string evidenceCodes)
        {
            var criteria = new GetFlaggedEvidenceCriteria
            {
                CTSCustId = ctsCustId,
                EvidenceCodes = evidenceCodes
            };

            using (var db = ObjectDbFactory.CreateInstance(criteria.GetSettingKey()))
            {
                return db.Query<FlaggedEvidenceModel>(criteria);
            }
        }

        public DBResult RemoveFlaggedEvidence(RemoveEvidenceModel model)
        {
            var criteria = new RemoveFlaggedEvidenceCriteria
            {
                CTSCustId = model.CTSCustId,
                EvidenceId = model.EvidenceId,
                UserId = model.UserId
            };

            using (var db = ObjectDbFactory.CreateInstance(criteria.GetSettingKey()))
            {
                try
                {
                    db.ExecuteNonQuery(criteria);

                    if (!string.IsNullOrEmpty(criteria.ErrorMessage))
                    {
                        return new DBResult { ErrorCode = -1, ErrorMsg = criteria.ErrorMessage };
                    }
                    else
                    {
                        return new DBResult { ErrorCode = 0, ErrorMsg = string.Empty };
                    }
                }
                catch (Exception ex)
                {
                    LogService.Error("CTS - Remove Flagged Evidence Manual throws exception: " + ex.Message, ex);

                    return new DBResult { ErrorCode = -1, ErrorMsg = ex.Message };
                }
            }
        }

        public List<AccountExceptionsModel> GetAccountExceptions(Int64 ctsCustId)
        {
            var criteria = new GetAccountExceptionsCriteria
            {
                CTSCustId = ctsCustId
            };

            using (var db = ObjectDbFactory.CreateInstance(criteria.GetSettingKey()))
            {
                return db.Query<AccountExceptionsModel>(criteria).ToList();
            }
        }

        public DBResult AddAccountException(UpdateAccountExceptionModel model)
        {
            var criteria = new AddAccountExceptionCriteria
            {
                UserId = model.UserId,
                FromCTSCustId = model.FromCTSCustId,
                ToUserName = model.ToUserName,
                Comment = model.Comment,
                ErrorCode = 0
            };

            try
            {
                repository.Execute(criteria);
            }
            catch (Exception ex)
            {
                if (ex.Message != string.Empty && ex.Message.ToLower().Contains("duplicate"))
                {
                    return new DBResult { ErrorCode = 1062 };
                }
            }

            return new DBResult
            {
                ErrorCode = criteria.ErrorCode,
                OutParam = new Hashtable { { "toCTSCustId", criteria.ToCTSCustId } }
            };
        }

        public int RemoveAccountException(UpdateAccountExceptionModel model)
        {
            var criteria = new RemoveAccountExceptionCriteria
            {
                UserId = model.UserId,
                FromCTSCustId = model.FromCTSCustId,
                ToCTSCustId = model.ToCTSCustId
            };

            using (var db = ObjectDbFactory.CreateInstance(criteria.GetSettingKey()))
            {
                db.ExecuteNonQuery(criteria);

                return 0;
            }
        }

        public AffectedRetractEvidenceModel GetAffectedRetractEvidences(Int64 ctsCustId, string evidenceCodes)
        {
            var criteria = new GetAffectedEvidenceCriteria
            {
                CTSCustId = ctsCustId,
                EvidenceCodes = evidenceCodes
            };

            using (var db = ObjectDbFactory.CreateInstance(criteria.GetSettingKey()))
            {
                using (var result = db.QueryMultiple(criteria))
                {
                    return new AffectedRetractEvidenceModel
                    {
                        AffectedEvidenceList = result.Read<AffectedEvidenceModel>().ToList(),
                        RetractEvidenceList = result.Read<AffectedEvidenceModel>().ToList()
                    };
                }
            }
        }

        public int RetractAffectedEvidence(RemoveEvidenceModel model)
        {
            var criteria = new RetractAffectedEvidenceCriteria
            {
                CTSCustId = model.CTSCustId,
                EvidenceId = model.EvidenceId,
                Comment = model.Comment,
                UserId = model.UserId
            };

            var result = DataService.UpdateData(criteria, "Retract affected evidence");

            return result.ErrorCode;
        }

        public int RestoreAffectedEvidence(RemoveEvidenceModel model)
        {
            var criteria = new RestoreAffectedEvidenceCriteria
            {
                CTSCustId = model.CTSCustId,
                EvidenceId = model.EvidenceId,
                UserId = model.UserId
            };

            var result = DataService.UpdateData(criteria, "Restore affected evidence");

            return result.ErrorCode;
        }

        public IEnumerable<AffectedEvidenceListBetweenCustsModel> GetAffectedEvidenceListBetweenCusts(Int64 fromCTSCustID, Int64 toCTSCustID)
        {
            var criteria = new GetAffectedEvidenceBetweenCustsCriteria
            {
                FromCTSCustID = fromCTSCustID,
                ToCTSCustID = toCTSCustID
            };

            using (var db = ObjectDbFactory.CreateInstance(criteria.GetSettingKey()))
            {
                return db.Query<AffectedEvidenceListBetweenCustsModel>(criteria);
            }
        }

        #endregion

        #region Customer Category           

        public IEnumerable<GetCustomerClassificationModel> GetCustomerClassification(long ctsCustId, out bool isExcludedTagAssPA)
        {         
            isExcludedTagAssPA = false;

            var criteria = new GetCTSCustomerClassificationCriteria
            {
                CTSCustID = ctsCustId
            };

            using (var db = ObjectDbFactory.CreateInstance(criteria.GetSettingKey()))
            {
                using (var result = db.QueryMultiple(criteria))
                {                   
                    var excludedTagAssPA = result.Read<bool>();
                    var categories = result.Read<GetCustomerClassificationModel>();
                   
                    if (excludedTagAssPA != null && excludedTagAssPA.Any())
                    {
                        isExcludedTagAssPA = excludedTagAssPA.ToList()[0];
                    }

                    return categories;
                }
            }
        }

        public IEnumerable<GetCustomerClassificationModel> GetCustomerClassification(long ctsCustId)
        {
            var criteria = new GetCTSCustomerClassificationCriteria
            {
                CTSCustID = ctsCustId
            };

            using (var db = ObjectDbFactory.CreateInstance(criteria.GetSettingKey()))
            {
                using (var result = db.QueryMultiple(criteria))
                {                   
                    result.Read<bool>(); // Excluded Tag Ass PA

                    return result.Read<GetCustomerClassificationModel>();
                }
            }
        }

        public IEnumerable<CustomerClassificationHistoryModel> GetCustomerClassificationHistory(long ctsCustId)
        {
            var criteria = new GetCTSCustomerClassificationHistoryCriteria
            {
                CTSCustID = ctsCustId
            };

            try
            {
                using (var db = ObjectDbFactory.CreateInstance(criteria.GetSettingKey()))
                {
                    return db.Query<CustomerClassificationHistoryModel>(criteria);
                }
            }
            catch (Exception ex)
            {
                LogService.Error("CTS - AccountService - GetCustomerClassificationHistory exception: " + ex.Message, ex);
            }

            return new List<CustomerClassificationHistoryModel>();
        }

        public IEnumerable<GetCustomerClassificationModel> GetCustomerClassificationForAPI(string custIdList, string parentCateIdList)
        {
            var criteria = new GetCTSCustomerClassificationForAPICriteria
            {
                CustIdList = custIdList,
                ParentCateIdList = parentCateIdList
            };

            using (var db = ObjectDbFactory.CreateInstance(criteria.GetSettingKey()))
            {
                return db.Query<GetCustomerClassificationModel>(criteria);
            }
        }     

        public DBResult AddQueueAffectDownlinesCustomerCategory(CustomerCategoryModel categoryModel)
        {
            var criteria = new InsertQueueCustClassificationForDownlineCriteria
            {
                CustId = categoryModel.CustId,
                CTSCustId = categoryModel.CTSCustId,
                SubscriberId = categoryModel.SubscriberId,
                CategoryId = categoryModel.CategoryId,
                RoleId = categoryModel.RoleId,
                CreatedBy = categoryModel.CreatedBy,
                Remark = categoryModel.Remark,
                ActionType = (int)categoryModel.ActionType
            };

            var repository = new DynamicRepository();

            try
            {
                repository.Execute(criteria);
            }
            catch (Exception ex)
            {
                LogService.Error($"CTS - AddQueueCustomerCategory exception: { ex.Message }", ex);

                return new DBResult { ErrorCode = -1, ErrorMsg = ex.Message };
            }

            return new DBResult { ErrorCode = 0, ErrorMsg = string.Empty };
        }
      
        public DBResult ChangeExcludedTagAssPA(long ctsCustId, int subscriberId, bool isExcludedTagAssPA, int createdBy)
        {
            var criteria = new ChangeUpdateTaggingAssWithPACriteria
            {
                CTSCustId = ctsCustId,
                SubscriberId = subscriberId,
                IsExcludedTagAssPA = isExcludedTagAssPA,
                CreatedBy = createdBy
            };

            using (var db = ObjectDbFactory.CreateInstance(criteria.GetSettingKey()))
            {
                db.ExecuteNonQuery(criteria);

                if (string.IsNullOrWhiteSpace(criteria.ErrorMessage))
                {
                    return new DBResult { ErrorCode = 0, ErrorMsg = criteria.ErrorMessage };
                }
            }

            return new DBResult { ErrorCode = -1, ErrorMsg = criteria.ErrorMessage };
        }

        public IEnumerable<CustomerClassificationGeneralInfoModel> GetCustomerClassificationByCategory(string categoryIdList)
        {
            var criteria = new GetCustomerCategoryByCategoryIdsCriteria
            {
                CategoryIDList = categoryIdList
            };

            using (var db = ObjectDbFactory.CreateInstance(criteria.GetSettingKey()))
            {
                return db.Query<CustomerClassificationGeneralInfoModel>(criteria);
            }
        }
        #endregion

        #region Customer Category By Sport
        public IEnumerable<GetCustomerCategoryBySportModel> GetCustomerCategoryBySport(long ctsCustID)
        {
            var criteria = new GetCustomerClassificationBySportCriteria { CTSCustID = ctsCustID };

            using (var db = ObjectDbFactory.CreateInstance(criteria.GetSettingKey()))
            {
                return db.Query<GetCustomerCategoryBySportModel>(criteria);
            }
        }

        public IEnumerable<GetCustomerCategoryHistoryBySportModel> GetCustomerCategoryHistoryBySport(long ctsCustID, int sportID)
        {
            var criteria = new GetCustomerClassificationHistoryBySportCriteria
            {
                CTSCustID = ctsCustID,
                SportID = sportID
            };

            using (var db = ObjectDbFactory.CreateInstance(criteria.GetSettingKey()))
            {
                return db.Query<GetCustomerCategoryHistoryBySportModel>(criteria);
            }
        }
        #endregion

        #region Betting Statistic
        public IEnumerable<PerformanceBySportModel> GetPerformanceBySport(long custId, int getTop)
        {
            var criteria = new BettingPerformanceBySportGetCriteria
            {
                CustID = custId,
                GetTop = getTop
            };

            using (var db = ObjectDbFactory.CreateInstance(criteria.GetSettingKey()))
            {
                return db.Query<PerformanceBySportModel>(criteria);
            }
        }

        public IEnumerable<PerformanceByBettypeModel> GetPerformanceByBettype(long custId, int getTop)
        {
            var criteria = new BettingStaisticByBetTypeGetCriteria
            {
                CustID = custId,
                GetTop = getTop
            };

            using (var db = ObjectDbFactory.CreateInstance(criteria.GetSettingKey()))
            {
                return db.Query<PerformanceByBettypeModel>(criteria);
            }
        }

        public IEnumerable<PerformanceDailyModel> GetPerformanceDaily(long custId)
        {
            var criteria = new BettingStatictisDaily
            {
                CustId = custId
            };

            using (var db = ObjectDbFactory.CreateInstance(criteria.GetSettingKey()))
            {
                return db.Query<PerformanceDailyModel>(criteria);
            }
        }
        #endregion

        public IEnumerable<CTSCustomer> CheckCustomerNotExisted(string custIds, DateTime fromDate, DateTime toDate)
        {
            var criteria = new CheckCustNotExistedByCustIdsCriteria
            {
                CustIds = custIds,
                FromDate = fromDate,
                ToDate = toDate
            };

            return repository.Fetch<CTSCustomer>(criteria);
        }

    }
}