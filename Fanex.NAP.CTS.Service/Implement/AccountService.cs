namespace Fanex.NAP.CTS.Service
{
    using Fanex.Data;
    using Fanex.Data.Repository;
    using Fanex.Logging;
    using Fanex.NAP.CTS.Core;
    using Fanex.NAP.CTS.Core.BaseClasses;
    using Fanex.NAP.CTS.Core.Extentions;
    using Fanex.NAP.CTS.Core.Helpers;
    using Fanex.NAP.CTS.Core.Helpers.Api;
    using Fanex.NAP.CTS.Core.Utilities;
    using Fanex.NAP.CTS.Criteria.Account;
    using Fanex.NAP.CTS.Criteria.Account.Associated;
    using Fanex.NAP.CTS.Criteria.Account.BettingStatistic;
    using Fanex.NAP.CTS.Criteria.Account.ChainToEvidence;
    using Fanex.NAP.CTS.Criteria.Account.CustomerCategory;
    using Fanex.NAP.CTS.Criteria.CustomerClassify;
    using Fanex.NAP.CTS.Criteria.Home;
    using Fanex.NAP.CTS.Model;
    using Fanex.NAP.CTS.Model.Account;
    using Fanex.NAP.CTS.Model.Association;
    using Fanex.NAP.CTS.Model.CustomerClassify;
    using Fanex.NAP.CTS.Model.DTO;
    using Fanex.NAP.CTS.Model.GraphDB;
    using Fanex.NAP.CTS.Model.Home;
    using Fanex.NAP.CTS.Model.SyncCustomerToMainDb;
    using Fanex.NAP.CTS.Resource.Consts;
    using Fanex.NAP.CTS.Service.Implement;
    using Fanex.NAP.CTS.ViewModel.Account.Associated;
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using static Fanex.NAP.CTS.Resource.Consts.Consts;

    public class AccountService : IAccountService
    {
        private readonly ICustomerEvidenceService customerEvidenceService;
        private readonly IDynamicRepository repository;

        public AccountService()
        {
            customerEvidenceService = new CustomerEvidenceService();
            repository = new DynamicRepository();
        }

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

        public List<CustomerGeneralInfoModel> GetCustomerGeneralInfo(string custIds)
        {
            var criteria = new CustomerGeneralInfoCriteria
            {
                CustIds = custIds
            };

            using (var db = ObjectDbFactory.CreateInstance(criteria.GetSettingKey()))
            {
                var result = db.Query<CustomerGeneralInfoModel>(criteria);

                return result.ToList();
            }
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

        public CustomerAssociationByCTSCustIdModel GetCustomerTotalAssociationByCTSCustId(long ctsCustId, string apiUrl, bool fromMySQL = false)
        {
            if (fromMySQL)
            {
                var criteria = new GetAssociationProfileStatictisCriteria
                {
                    CTSCustId = ctsCustId
                };

                using (var db = ObjectDbFactory.CreateInstance(criteria.GetSettingKey()))
                {
                    var result = db.QueryEntity<CustomerAssociationByCTSCustIdModel>(criteria);

                    return result;
                }
            }
            else
            {
                var apiPath = string.Format(apiUrl, ctsCustId);

                var data = ApiHelper.Get<CustomerAssociationByCTSCustIdModel>(apiPath);

                Task.WhenAll(data);

                return data.Result;
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

        public CustomerPoliciesModel GetCustomerProfilePoliciesInfo(long custId)
        {
            var criteria = new CustomerProfilePoliciesInfoCriteria
            {
                CustID = custId
            };

            using (var db = ObjectDbFactory.CreateInstance(criteria.GetSettingKey()))
            {
                var result = db.QueryEntity<CustomerPoliciesModel>(criteria);

                return result;
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

        public CustomerLastTicketDateModel GetCustomerLastTicketDate(long custId)
        {
            var criteria = new CustomerLastTicketDateCriteria
            {
                CustId = custId
            };

            using (var db = ObjectDbFactory.CreateInstance(criteria.GetSettingKey()))
            {
                var result = db.QueryEntity<CustomerLastTicketDateModel>(criteria);

                return result;
            }
        }

        public CustomerEsportAutoSuspendStatusModel GetCustomerEsportAutoSuspendedStatus(long custId)
        {
            var criteria = new CustomerEsportAutoSuspendStatusCriteria
            {
                CustId = custId
            };

            using (var db = ObjectDbFactory.CreateInstance(criteria.GetSettingKey()))
            {
                var result = db.QueryEntity<CustomerEsportAutoSuspendStatusModel>(criteria);

                return result;
            }
        }

        public CustomerRobotUserStatusModel GetCustomerRobotUserStatus(long custId)
        {
            var criteria = new CustomerRobotUserStatusCriteria
            {
                CustId = custId
            };

            using (var db = ObjectDbFactory.CreateInstance(criteria.GetSettingKey()))
            {
                var result = db.QueryEntity<CustomerRobotUserStatusModel>(criteria);

                return result;
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

        public GraphAssociationPaging GetCustomerAssociationByGraph(AssociatedFilterModel filterModel, bool fromMySQL = false)
        {
            if (fromMySQL)
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
                    var result = db.Query<GraphAssociationModel>(criteria);

                    return new GraphAssociationPaging
                    {
                        AssociationList = result.ToList(),
                        TotalItems = criteria.TotalItems
                    };
                }
            }
            else
            {
                try
                {
                    var param = new AssociationProfileParameterModel
                    {
                        CTSCustId = filterModel.CTSCustId,
                        AssociationStatus = filterModel.AssociationStatus,
                        AccountStatusIds = filterModel.AccountStatusIds,
                        AssociationType = filterModel.AssociationType,
                        CategoryIDs = filterModel.CategoryIds,
                        SubscriberIDs = filterModel.SubscriberIds,
                        Skip = filterModel.start,
                        Take = filterModel.length,
                    };

                    var data = ApiHelper.Post<GraphAssociationPaging>(ConfigSettings.GetDirectAssociationProfile, param);

                    Task.WhenAll(data);

                    return data.Result ?? new GraphAssociationPaging();
                }
                catch (Exception ex)
                {
                    LogService.Info("Web - Association Tab Profile - Exception: " + ex.Message + ex.StackTrace + "\n"
                        + ex.InnerException.Message + ex.InnerException.Source + ex.InnerException
                        + ex.InnerException.StackTrace);
                }
            }

            return new GraphAssociationPaging();
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

        public List<GraphDirectAssociationModel> GetDirectAssocationByGraph(string apiURL, List<long> ctsCustIds)
        {
            var data = ApiHelper.Post<List<GraphDirectAssociationModel>>(apiURL, ctsCustIds);

            Task.WhenAll(data);

            var customersAPI = data.Result;

            return customersAPI ?? new List<GraphDirectAssociationModel>();
        }

        public ShortestPathOfAccount GetShortestPathAssociationByGraph(string apiURL, int custIdFrom, int custIdTo)
        {
            var data = ApiHelper.Get<ShortestPathOfAccount>(string.Format(apiURL, custIdFrom, custIdTo));
            Task.WhenAll(data);

            return data.Result ?? new ShortestPathOfAccount();
        }

        public string MappingAssociationType(GraphAssociationModel data)
        {
            if (data.AssociationType != null)
            {
                return GetAssociationTypeName(data.AssociationType).Select(x => x).Join(", ");
            }

            return string.Empty;
        }

        public List<string> GetAssociationTypeName(List<int> associationType)
        {
            var assTypeName = new List<string>();

            foreach (var i in associationType)
            {
                assTypeName.Add(GetAssociationTypeName(i));
            }

            return assTypeName;
        }

        public string GetAssociationTypeName(int associationType)
        {
            string x = Enum.GetName(typeof(AssociationType), associationType);

            if (!string.IsNullOrEmpty(x) && x.Equals("BettingPattern"))
            {
                return Consts.AssociationTypeName_BettingPattern;
            }

            return x;
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

        public IEnumerable<long> GetAssociationException(string apiURL, long ctsCustId)
        {
            var urlString = string.Format(apiURL + "?ctsCustId={0}", ctsCustId);

            var data = ApiHelper.Get<IEnumerable<long>>(urlString);

            Task.WhenAll(data);

            return data.Result ?? new List<long>();
        }

        public MatchesAssociationByAIModel GetMatchesAssociationInfo(long custID1, long custID2)
        {
            var result = new MatchesAssociationByAIModel();

            var criteria = new GetMatchesAssociationByAICriteria
            {
                CustID1 = custID1,
                CustID2 = custID2,
            };

            using (var db = ObjectDbFactory.CreateInstance(criteria.GetSettingKey()))
            {
                result.MatchesInfo = db.Query<MatchesInfoModel>(criteria).ToList();
            }

            result.ErrorMessage = criteria.ErrorMessage;
            result.TotalMatchCount = (int)criteria.TotalMatchCount;

            return result;
        }

        public IEnumerable<MatchesInfoModel> GetMatchesInfo(string matchIDs)
        {
            var criteria = new GetMatchesInfoCriteria
            {
                MatchIds = matchIDs
            };

            using (var db = ObjectDbFactory.CreateInstance(criteria.GetSettingKey()))
            {
                return db.Query<MatchesInfoModel>(criteria);
            }
        }

        public IEnumerable<MatchesDetailInfoModel> GetMatchesDetailInfo(MatchesAssociationDetailViewModel macthInfo)
        {
            var criteria = new GetMatchesDetailInfoCriteria
            {
                EventDate = macthInfo.EventDate,
                Bettypes = macthInfo.Bettypes,
                CustId1 = macthInfo.CustId1,
                CustId2 = macthInfo.CustId2,
                MatchId = macthInfo.MatchId
            };

            using (var db = ObjectDbFactory.CreateInstance(criteria.GetSettingKey()))
            {
                return db.Query<MatchesDetailInfoModel>(criteria);
            }
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
                    ErrorMessage = criteria.ErrorMessage,
                    TotalIPCount = criteria.TotalIPCount,
                    TotalMatchCount = criteria.TotalMatchCount
                };
            }
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

        public DBResult PlaceEvidence(UpdatePlaceEvidenceModel model)
        {
            var criteria = new PlaceEvidenceCriteria
            {
                CTSCustId = model.CTSCustId,
                EvidenceId = model.EvidenceId,
                Remark = model.Comment,
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
                    LogService.Error("CTS - Place Evidence Manual throws exception: " + ex.Message, ex);

                    return new DBResult { ErrorCode = -1, ErrorMsg = ex.Message };
                }
            }
        }

        public DBResult ProcessPlaceEvidence(UpdatePlaceEvidenceModel model)
        {
            var result = PlaceEvidence(model);

            if (result.ErrorCode == 0)
            {
                customerEvidenceService.UpdateBetLimitOfFlaggedCustomer(model.CustId.ToString());

                Task.Run(() =>
                {

                    try
                    {
                        FPSAddNewEvidence(ConfigSettings.AddNewEvidence, model);
                    }
                    catch (Exception ex)
                    {
                        LogService.Error(ex.Message);
                    }
                });
            }

            return result;
        }

        public void FPSAddNewEvidence(string apiURL, UpdatePlaceEvidenceModel model)
        {
            var data = ApiHelper.Post<AddNewEvidenceForFPSModel>(apiURL, new AddNewEvidenceForFPSModel
            {
                CustIds = model.CustId.ToString(),
                EviCodes = model.EvidenceCode,
                CreatedBy = model.UserId.ToString(),
                Comment = model.Comment
            });

            Task.WhenAll(data);
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

        public DBResult SyncCustEvidenceToCategory(string custEvidencesJson, bool isRescan, out IEnumerable<long> custIds)
        {
            custIds = new List<long>();

            var criteria = new CustEvidenceSyncToCategoryCriteria
            {
                CustEvidencesJson = custEvidencesJson,
                IsRescan = isRescan
            };

            try
            {
                var repository = new DynamicRepository();

                custIds = repository.Fetch<long>(criteria);

                if (!string.IsNullOrEmpty(criteria.ErrorMessage))
                {
                    LogService.Error("CTS - AccountService SyncCustEvidenceToCategory failed: " + criteria.ErrorMessage);

                    return new DBResult { ErrorCode = -1, ErrorMsg = criteria.ErrorMessage };
                }

                return new DBResult { ErrorCode = 0, ErrorMsg = string.Empty };
            }
            catch (Exception ex)
            {
                LogService.Error("CTS - AccountService SyncCustEvidenceToCategory exception: " + ex.Message, ex);

                return new DBResult { ErrorCode = -1, ErrorMsg = ex.Message };
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
        public IEnumerable<SportGroup> GetAllSportGroup()
        {
            var criteria = new GetAllSportGroupCriteria();

            using (var db = ObjectDbFactory.CreateInstance(criteria.GetSettingKey()))
            {
                return db.Query<SportGroup>(criteria);
            }
        }

        public IEnumerable<CustomerCategory> GetCustomerCategoryByCTSCustId(long ctsCustId)
        {
            var criteria = new GetCustomerCategoryByCTSCustId
            {
                CTSCustId = ctsCustId
            };

            using (var db = ObjectDbFactory.CreateInstance(criteria.GetSettingKey()))
            {
                return db.Query<CustomerCategory>(criteria);
            }
        }

        public IEnumerable<CustomerClassificationModel> GetCustomerClassification(long ctsCustId, out bool isPinCategory, out bool isExcludedTagAssPA)
        {
            isPinCategory = false;
            isExcludedTagAssPA = false;

            var criteria = new GetCTSCustomerClassificationCriteria
            {
                CTSCustID = ctsCustId
            };

            using (var db = ObjectDbFactory.CreateInstance(criteria.GetSettingKey()))
            {
                using (var result = db.QueryMultiple(criteria))
                {
                    var pinCategories = result.Read<bool>();
                    var excludedTagAssPA = result.Read<bool>();
                    var categories = result.Read<CustomerClassificationModel>();

                    if (pinCategories != null && pinCategories.Any())
                    {
                        isPinCategory = pinCategories.ToList()[0];
                    }

                    if (excludedTagAssPA != null && excludedTagAssPA.Any())
                    {
                        isExcludedTagAssPA = excludedTagAssPA.ToList()[0];
                    }

                    return categories;
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

        public IEnumerable<CustomerClassificationModel> GetCustomerClassificationForAPI(string custIdList, string parentCateIdList)
        {
            var criteria = new GetCTSCustomerClassificationForAPICriteria
            {
                CustIdList = custIdList,
                ParentCateIdList = parentCateIdList
            };

            using (var db = ObjectDbFactory.CreateInstance(criteria.GetSettingKey()))
            {
                return db.Query<CustomerClassificationModel>(criteria);
            }
        }

        public IEnumerable<Model.API.CustomerClassModel> GetCustomerClassByCustId(string custIdList)
        {
            var criteria = new GetCustomerClassByCustIdCriteria
            {
                CustIds = custIdList
            };

            using (var db = ObjectDbFactory.CreateInstance(criteria.GetSettingKey()))
            {
                return db.Query<Model.API.CustomerClassModel>(criteria);
            }
        }

        public IEnumerable<LatestClassification4ReportModel> GetCustomerClassFromCTSByCustId(string custIdList)
        {
            var criteria = new GetLastestClassificationCriteria
            {
                CustIds = custIdList
            };

            using (var db = ObjectDbFactory.CreateInstance(criteria.GetSettingKey()))
            {
                return db.Query<LatestClassification4ReportModel>(criteria);
            }
        }

        public DBResult AddCustomerCategory(AddCustomerCategoryModel addCategoryModel, int createdBy, out IEnumerable<LatestClassificationModel> latestClassifications)
        {
            var criteria = new AddCTSCustomerClassificationCriteria
            {
                CustId = addCategoryModel.CustId,
                CTSCustId = addCategoryModel.CTSCustId,
                SubscriberId = addCategoryModel.SubscriberId,
                CategoryId = addCategoryModel.CategoryId,
                CreatedBy = createdBy,
                Remark = addCategoryModel.Remark
            };

            var repository = new DynamicRepository();

            latestClassifications = repository.Fetch<LatestClassificationModel>(criteria);

            if (!string.IsNullOrEmpty(criteria.ErrorMessage))
            {
                LogService.Error("CTS - AccountService AddCustomerCategory failed: " + criteria.ErrorMessage);

                return new DBResult { ErrorCode = -1, ErrorMsg = criteria.ErrorMessage };
            }

            return new DBResult { ErrorCode = 0, ErrorMsg = string.Empty };
        }

        public ResultInsertRemoveCustomerClassificationModel RemoveCustomerCategory(long custId, long ctsCustId, int categoryId, int createdBy)
        {
            var insertCTSResult = new ResultInsertRemoveCustomerClassificationModel();
            var criteria = new RemoveCTSCustomerClassificationCriteria
            {
                CustId = custId,
                CTSCustId = ctsCustId,
                CategoryId = categoryId,
                CreatedBy = createdBy
            };

            var repository = new DynamicRepository();
            insertCTSResult.CustomerClassList = repository.Fetch<CustomerClassForMainDb>(criteria);
            insertCTSResult.OpErrorMessage = criteria.ErrorMessage;

            return insertCTSResult;
        }

        public DBResult PinCategoryCustomerClassification(long ctsCustId, int subscriberId, bool isPinCategory, int createdBy)
        {
            var criteria = new PinCTSCategoryCustomerClassificationCriteria
            {
                CTSCustId = ctsCustId,
                SubscriberId = subscriberId,
                IsPinCategory = isPinCategory,
                CreatedBy = createdBy
            };

            try
            {
                using (var db = ObjectDbFactory.CreateInstance(criteria.GetSettingKey()))
                {
                    db.ExecuteNonQuery(criteria);

                    if (string.IsNullOrWhiteSpace(criteria.ErrorMessage))
                    {
                        return new DBResult
                        {
                            ErrorCode = 0,
                            ErrorMsg = criteria.ErrorMessage
                        };
                    }
                }
            }
            catch (Exception ex)
            {
                LogService.Error("CTS - Pin Category - PinCategoryCustomerClassification exception: " + ex.Message, ex);

                return new DBResult
                {
                    ErrorCode = -1,
                    ErrorMsg = ex.Message
                };
            }

            return new DBResult
            {
                ErrorCode = -1,
                ErrorMsg = criteria.ErrorMessage
            };
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