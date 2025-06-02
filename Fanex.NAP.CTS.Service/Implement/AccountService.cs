namespace Fanex.NAP.CTS.Service
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;
    using Fanex.NAP.CTS.Core;
    using Fanex.NAP.CTS.Core.Service;
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
    using Fanex.NAP.CTS.Model.Home;
    using Fanex.NAP.CTS.Model.Transaction;
    using Fanex.NAP.CTS.Resource.Consts;
    using Fanex.NAP.CTS.ViewModel.Account.Associated;
    using static Fanex.NAP.CTS.Resource.Consts.Consts;

    public class AccountService : IAccountService
    {
        private readonly ICriteriaService _repository;

        public AccountService()
        {
            _repository = new CriteriaService();
        }

        #region Customer Profile

        public IEnumerable<CustInfoModel> GetCustInfoByCustID(string custIDs)
        {
            var criteria = new CustInfoByCustIdGetCriteria()
            {
                CustIDs = custIDs
            };

            return _repository.Fetch<CustInfoModel>(criteria);
        }

        public CustomerInfoByCtsCustIdModel GetCustomerInfoByCTSCustId(Int64 ctsCustId)
        {
            var criteria = new CustomerInfoByCTSCustIdCriteria
            {
                CTSCustId = ctsCustId
            };

            return _repository.Get<CustomerInfoByCtsCustIdModel>(criteria);
        }

        public CustomerInfoByCustIdsModel GetCustomerInfoDangerous(long custId)
        {
            var criteria = new GetCustomerInfoDangerousCriteria
            {
                CustIdList = custId.ToString(),
            };

            return _repository.Get<CustomerInfoByCustIdsModel>(criteria);
        }

        public CustomerInfoAssociationAccountModel GetCustomerInfoAssociationAccount(long ctsCustId)
        {
            var criteria = new GetCustomerInfoAssociatedAccountCriteria
            {
                CTSCustId = ctsCustId
            };

            return _repository.Get<CustomerInfoAssociationAccountModel>(criteria);
        }

        public IEnumerable<CustomerInfoEvidenceModel> GetCustomerInfoEvidence(long ctsCustId)
        {
            var criteria = new GetCustomerInfoEvidenceCriteria
            {
                CTSCustId = ctsCustId
            };

            return _repository.Fetch<CustomerInfoEvidenceModel>(criteria);
        }

        public CustomerInfoLoginStatsModel GetCustomerInfoLoginStats(long ctsCustId, DateTime fromDate, DateTime toDate)
        {
            var criteria = new GetCustomerInfoLoginStatsCriteria
            {
                CTSCustID = ctsCustId,
                FromDate = fromDate,
                ToDate = toDate
            };

            return _repository.Get<CustomerInfoLoginStatsModel>(criteria);
        }

        public CustomerInfoDateModel GetCustProfileDates(long ctsCustId, ROLE_CUSTOMER roleID)
        {
            var criteria = new GetCustomerInfoDateCriteria
            {
                CTSCustID = ctsCustId,
                RoleID = roleID
            };

            return _repository.Get<CustomerInfoDateModel>(criteria);
        }

        #endregion

        #region Associated account by account
        public List<SearchResultModel> SearchAccounts(string username)
        {
            var criteria = new SearchResultCriteria
            {
                Username = username
            };

            var result = _repository.Fetch<SearchAccountResultModel>(criteria);

            return result?.Select(x => new SearchResultModel(x)).ToList() ?? new List<SearchResultModel>();
        }

        public List<CustomerEvidenceByCTSCustIdModel> GetCustomerEvidenceByCTSCustId(long ctsCustId)
        {
            var criteria = new CustomerEvidenceByCTSCustIdCriteria
            {
                CTSCustId = ctsCustId
            };

            return _repository.Fetch<CustomerEvidenceByCTSCustIdModel>(criteria)?.ToList() ?? new List<CustomerEvidenceByCTSCustIdModel>();
        }

        public List<CustomerStaticStatusModel> GetCustomerStaticStatus(int listId, bool status)
        {
            var criteria = new CustomerStaticAccountStatusCriteria
            {
                ListId = listId,
                Status = status
            };

            return _repository.Fetch<CustomerStaticStatusModel>(criteria)?.ToList() ?? new List<CustomerStaticStatusModel>();
        }

        public CustomerProfileFraudInfoModel GetCustomerProfileFraudInfo(long custId)
        {
            var criteria = new CustomerProfileFraudInfoCriteria
            {
                CustId = custId
            };

            return _repository.Get<CustomerProfileFraudInfoModel>(criteria);
        }

        public List<TagModel> GetDirectAssociatedAccountTagFlagged(string custIdsAssInfo)
        {
            var criteria = new GetDirectAssociatedAccountTagFlaggedCriteria
            {
                CustAssInfo = custIdsAssInfo
            };

            return _repository.Fetch<TagModel>(criteria)?.ToList() ?? new List<TagModel>();
        }

        public CustomerFlagInfoModel GetCustomerFlagInfo(Int64 ctsCustId)
        {
            var criteria = new CustomerFlagInfoByCTSCustIdCriteria
            {
                CTSCustId = ctsCustId
            };

            return _repository.Get<CustomerFlagInfoModel>(criteria);
        }

        public AssociatedAccountPagingModel GetAssociatedAccount(AssociatedFilterModel filterModel)
        {
            var criteria = new GetAssociationProfileCriteria
            {
                CTSCustId = filterModel.CTSCustId,
                RoleIDs = filterModel.RoleIDs,
                AssociationStatus = filterModel.AssociationStatus,
                AccountStatusIds = filterModel.AccountStatusIds,
                AssociationType = filterModel.AssociationType,
                CategoryIDs = filterModel.CategoryIds,
                SubscriberIDs = filterModel.SubscriberIds,
                Skip = filterModel.start,
                Take = filterModel.length,
                TotalItems = 0
            };

            var result = _repository.Fetch<AssociatedAccountModel>(criteria);

            return new AssociatedAccountPagingModel
            {
                AssociationList = result?.ToList() ?? new List<AssociatedAccountModel>(),
                TotalItems = criteria.TotalItems
            };
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
                return _repository.Fetch<ValidateManualAssociationMultipleModel>(criteria);
            }
            catch (Exception ex)
            {
                isException = true;
                LogExtension.LogError("CTS - Validate manual association multiple - exception: " + ex.Message, ex);

                return GenerateAssociationErrorList(toUserNameList);
            }
        }

        private IEnumerable<ValidateManualAssociationMultipleModel> GenerateAssociationErrorList(string toUserNameList)
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

            try
            {
                _repository.Execute(criteria);

                return new DBResult
                {
                    ErrorCode = 0,
                    ErrorMsg = string.Empty
                };
            }
            catch (Exception ex)
            {
                LogExtension.LogError("CTS - Insert manual association multiple - exception: " + ex.Message, ex);

                return new DBResult
                {
                    ErrorCode = -1,
                    ErrorMsg = ex.Message
                };
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

            _repository.Execute(criteria);

            return new DBResult
            {
                ErrorCode = string.IsNullOrWhiteSpace(criteria.ErrorMessage) ? 0 : -1,
                ErrorMsg = criteria.ErrorMessage
            };
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
            result.MatchesInfo = _repository.Fetch<GBAssociationAIModel>(criteria) ?? Enumerable.Empty<GBAssociationAIModel>();
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

            return _repository.Fetch<MatchesModel>(criteria);
        }

        public IEnumerable<MatchesInfoModel> GetBettingPatternMatchesInfo(string matchIDs, string custIds)
        {
            var criteria = new GetBettingPatternMatchesInfoCriteria
            {
                MatchIds = matchIDs,
                CustIds = custIds
            };


            return _repository.Fetch<MatchesInfoModel>(criteria);
        }

        public IEnumerable<CustomerGroupModel> GetCustGroupAssociation(string originalGroupIDs)
        {
            if (!string.IsNullOrEmpty(originalGroupIDs))
            {
                var criteria = new GetCustomerGroupCriteria
                {
                    GroupList = originalGroupIDs
                };

                return _repository.Fetch<CustomerGroupModel>(criteria) ?? Enumerable.Empty<CustomerGroupModel>();
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
                return _repository.Fetch<MatchesDetailInfoModel>(criteria) ?? Enumerable.Empty<MatchesDetailInfoModel>();
            }

            return Enumerable.Empty<MatchesDetailInfoModel>();
        }

        public IEnumerable<MatchesDetailInfoViewModel> MergeCustGroupTicketInfo(IEnumerable<MatchCustomerGgbViewModel> custGroups, IEnumerable<MatchesDetailInfoModel> ticketDetails, bool isGGB)
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

        public IEnumerable<MatchCustomerGgbViewModel> MergeCustMatchInfo(IEnumerable<GBAssociationAIModel> matchInfo, IEnumerable<CustomerGroupModel> groupInfo)
        {
            if (matchInfo != null && matchInfo.Any() && groupInfo != null && groupInfo.Any())
            {
                return matchInfo.Join(groupInfo,
                                    m => m.GroupID,
                                    g => g.OriginGroupID,
                                    (m, g) => new MatchCustomerGgbViewModel
                                    {
                                        CustID = g.CustID,
                                        GroupID = g.GroupID,
                                        AdditionalInfo = m.AdditionalInfo
                                    }) ?? Enumerable.Empty<MatchCustomerGgbViewModel>();
            }

            return Enumerable.Empty<MatchCustomerGgbViewModel>();
        }

        public IEnumerable<CategoryInfoModel> GetAllCategory()
        {
            var criteria = new GetCategoryInfoCriteria();

            return _repository.Fetch<CategoryInfoModel>(criteria);
        }

        public AssociationByIPInfoModel GetAssociationByIPInfo(long custId1, long custId2)
        {
            var criteria = new GetAssociationByIPInfoCriteria
            {
                CustId1 = custId1,
                CustId2 = custId2
            };

            return new AssociationByIPInfoModel
            {
                ItemList = _repository.Fetch<AssociationByIPItemModel>(criteria),
                ErrorMessage = criteria.ErrorMessage
            };
        }

        public IEnumerable<int> GetAssociationByIPType(long custID1, long custID2)
        {
            var criteria = new GetAssociationByIPTypeCriteria
            {
                CustID1 = custID1,
                CustID2 = custID2
            };

            return _repository.Fetch<int>(criteria);
        }

        public IEnumerable<AssByIPTicketStakeModel> GetAssociationBy5GIPDetail(long custID1, long custID2)
        {
            var criteria = new GetAssociationBy5GIPDetailCriteria
            {
                CustID1 = custID1,
                CustID2 = custID2
            };

            return _repository.Fetch<AssByIPTicketStakeModel>(criteria);
        }

        public CoupleInfoViewModel GetCoupleInfos(long custID1, long custID2)
        {
            var criteria = new GetCoupleInfoCriteria
            {
                CustId1 = custID1,
                CustId2 = custID2
            };

            var result = new CoupleInfoViewModel();

            result.CoupleInfo = _repository.Fetch<CoupleInfoModel>(criteria);

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

            return _repository.Fetch<DateTime>(criteria);
        }

        public IEnumerable<CoupleFraudDetailModel> GetCoupleFraudDetails(string coupleInfoJson)
        {
            var criteria = new GetCoupleFraudDetailCriteria
            {
                CoupleInfoDetails = coupleInfoJson
            };

            return _repository.Fetch<CoupleFraudDetailModel>(criteria);
        }

        public IEnumerable<CoupleIPFraudDetailModel> GetCoupleIPFraudDetails(string coupleInfoJson)
        {
            var criteria = new GetCoupleIPFraudDetailCriteria
            {
                CoupleInfoDetails = coupleInfoJson
            };

            return _repository.Fetch<CoupleIPFraudDetailModel>(criteria);
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

            return _repository.Fetch<TicketDetailsModel>(criteria);
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

            if (fraudType != AccountAssociationByOTGB.CheckExist)
            {
                (result.AssociationOTGBRoun1, result.AssociationOTGBRoun2, result.AssociationOTGBRoun3) = _repository.Fetch<AssociationOTGBModel, AssociationOTGBModel, AssociationOTGBModel>(criteria);
            }
            else
            {
                (var scoreRound1s, var scoreRound2s, var scoreRound3s) = _repository.Fetch<bool, bool, bool>(criteria);
                result.ScoreRound1 = scoreRound1s?.FirstOrDefault() ?? false;
                result.ScoreRound2 = scoreRound2s?.FirstOrDefault() ?? false;
                result.ScoreRound3 = scoreRound3s?.FirstOrDefault() ?? false;
            }

            return result;
        }

        #endregion

        #region Chain To Evidence
        public IEnumerable<EvidenceModel> GetEvidenceList()
        {
            var criteria = new GetAllEvidenceCriteria();

            return _repository.Fetch<EvidenceModel>(criteria);
        }

        public IEnumerable<FlaggedEvidenceModel> GetFlaggedEvidences(Int64 ctsCustId, string evidenceCodes)
        {
            var criteria = new GetFlaggedEvidenceCriteria
            {
                CTSCustId = ctsCustId,
                EvidenceCodes = evidenceCodes
            };

            return _repository.Fetch<FlaggedEvidenceModel>(criteria);
        }

        public DBResult RemoveFlaggedEvidence(RemoveEvidenceModel model)
        {
            var criteria = new RemoveFlaggedEvidenceCriteria
            {
                CTSCustId = model.CTSCustId,
                EvidenceId = model.EvidenceId,
                UserId = model.UserId
            };

            try
            {
                _repository.Execute(criteria);

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
                LogExtension.LogError("CTS - Remove Flagged Evidence Manual throws exception: " + ex.Message, ex);

                return new DBResult { ErrorCode = -1, ErrorMsg = ex.Message };
            }
        }

        public List<AccountExceptionsModel> GetAccountExceptions(Int64 ctsCustId)
        {
            var criteria = new GetAccountExceptionsCriteria
            {
                CTSCustId = ctsCustId
            };

            return _repository.Fetch<AccountExceptionsModel>(criteria)?.ToList() ?? new List<AccountExceptionsModel>();
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
                _repository.Execute(criteria);
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

            _repository.Execute(criteria);

            return 0;
        }

        public AffectedRetractEvidenceModel GetAffectedRetractEvidences(Int64 ctsCustId, string evidenceCodes)
        {
            var criteria = new GetAffectedEvidenceCriteria
            {
                CTSCustId = ctsCustId,
                EvidenceCodes = evidenceCodes
            };

            (var affectedEvidenceList, var retractEvidenceList) = _repository.Fetch<AffectedEvidenceModel, AffectedEvidenceModel>(criteria);

            return new AffectedRetractEvidenceModel
            {
                AffectedEvidenceList = affectedEvidenceList?.ToList() ?? new List<AffectedEvidenceModel>(),
                RetractEvidenceList = retractEvidenceList?.ToList() ?? new List<AffectedEvidenceModel>()
            };
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

            var result = _repository.UpdateData(criteria, "Retract affected evidence");

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

            var result = _repository.UpdateData(criteria, "Restore affected evidence");

            return result.ErrorCode;
        }

        public IEnumerable<AffectedEvidenceListBetweenCustsModel> GetAffectedEvidenceListBetweenCusts(Int64 fromCTSCustID, Int64 toCTSCustID)
        {
            var criteria = new GetAffectedEvidenceBetweenCustsCriteria
            {
                FromCTSCustID = fromCTSCustID,
                ToCTSCustID = toCTSCustID
            };

            return _repository.Fetch<AffectedEvidenceListBetweenCustsModel>(criteria);
        }

        #endregion

        #region Customer Category           

        public IEnumerable<GetCustomerClassificationModel> GetCustomerClassification(long ctsCustId, ROLE_CUSTOMER roleID, out bool isExcludedTagAssPA)
        {
            var criteria = new GetCTSCustomerClassificationCriteria
            {
                CTSCustID = ctsCustId,
                RoleID = roleID
            };

            (var excludedTagAssPA, var categories) = _repository.Fetch<bool, GetCustomerClassificationModel>(criteria);

            isExcludedTagAssPA = excludedTagAssPA.IsNotNullOrEmpty() ? excludedTagAssPA.ToList()[0] : false;

            return categories;
        }

        public IEnumerable<CustomerClassificationHistoryModel> GetCustomerClassificationHistory(long ctsCustId, ROLE_CUSTOMER roleID)
        {
            var criteria = new GetCTSCustomerClassificationHistoryCriteria
            {
                CTSCustID = ctsCustId,
                RoleID = roleID
            };

            try
            {
                return _repository.Fetch<CustomerClassificationHistoryModel>(criteria);
            }
            catch (Exception ex)
            {
                LogExtension.LogError("CTS - AccountService - GetCustomerClassificationHistory exception: " + ex.Message, ex);
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

            return _repository.Fetch<GetCustomerClassificationModel>(criteria);
        }

        public DBResult AddQueueAffectDownlinesCustomerCategory(CustomerCategoryModel categoryModel)
        {
            var criteria = new InsertQueueCustClassificationForDownlineCriteria
            {
                CustId = categoryModel.CustId,
                CTSCustId = categoryModel.CTSCustId,
                SubscriberId = categoryModel.SubscriberId,
                CategoryId = categoryModel.CategoryId,
                RoleId = (int)categoryModel.RoleId,
                CreatedBy = categoryModel.CreatedBy,
                Remark = categoryModel.Remark,
                ActionType = (int)categoryModel.ActionType
            };

            try
            {
                _repository.Execute(criteria);
            }
            catch (Exception ex)
            {
                LogExtension.LogError($"CTS - AddQueueCustomerCategory exception: {ex.Message}", ex);

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

            _repository.Execute(criteria);

            return new DBResult { ErrorCode = string.IsNullOrWhiteSpace(criteria.ErrorMessage) ? 0 : -1, ErrorMsg = criteria.ErrorMessage };
        }

        public IEnumerable<CustomerClassificationGeneralInfoModel> GetCustomerClassificationByCategory(string categoryIdList)
        {
            var criteria = new GetCustomerCategoryByCategoryIdsCriteria
            {
                CategoryIDList = categoryIdList
            };

            return _repository.Fetch<CustomerClassificationGeneralInfoModel>(criteria);
        }
        #endregion

        #region Customer Category By Sport
        public IEnumerable<GetCustomerCategoryBySportModel> GetCustomerCategoryBySport(long ctsCustID)
        {
            var criteria = new GetCustomerClassificationBySportCriteria { CTSCustID = ctsCustID };

            return _repository.Fetch<GetCustomerCategoryBySportModel>(criteria);
        }

        public IEnumerable<GetCustomerCategoryHistoryBySportModel> GetCustomerCategoryHistoryBySport(long ctsCustID, int sportID)
        {
            var criteria = new GetCustomerClassificationHistoryBySportCriteria
            {
                CTSCustID = ctsCustID,
                SportID = sportID
            };

            return _repository.Fetch<GetCustomerCategoryHistoryBySportModel>(criteria);
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

            return _repository.Fetch<PerformanceBySportModel>(criteria);
        }

        public IEnumerable<PerformanceByBettypeModel> GetPerformanceByBettype(long custId, int getTop)
        {
            var criteria = new BettingStaisticByBetTypeGetCriteria
            {
                CustID = custId,
                GetTop = getTop
            };

            return _repository.Fetch<PerformanceByBettypeModel>(criteria);
        }

        public IEnumerable<PerformanceDailyModel> GetPerformanceDaily(long custId)
        {
            var criteria = new BettingStatictisDaily
            {
                CustId = custId
            };

            return _repository.Fetch<PerformanceDailyModel>(criteria);
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

            return _repository.Fetch<CTSCustomer>(criteria);
        }

    }
}