namespace Fanex.NAP.CTS.Service
{
    using Fanex.Data;
    using Fanex.Logging;
    using Fanex.NAP.CTS.Core;
    using Fanex.NAP.CTS.Core.BaseClasses;
    using Fanex.NAP.CTS.Core.Extentions;
    using Fanex.NAP.CTS.Core.Helpers;
    using Fanex.NAP.CTS.Core.Helpers.Api;
    using Fanex.NAP.CTS.Criteria.Account;
    using Fanex.NAP.CTS.Criteria.Account.Associated;
    using Fanex.NAP.CTS.Criteria.Account.BettingStatistic;
    using Fanex.NAP.CTS.Criteria.Account.ChainToEvidence;
    using Fanex.NAP.CTS.Criteria.Account.CustomerCategory;
    using Fanex.NAP.CTS.Criteria.Account.GeneralInformation;
    using Fanex.NAP.CTS.Criteria.Home;
    using Fanex.NAP.CTS.Model;
    using Fanex.NAP.CTS.Model.Account;
    using Fanex.NAP.CTS.Model.Account.CustomerCategory;
    using Fanex.NAP.CTS.Model.Association;
    using Fanex.NAP.CTS.Model.DTO;
    using Fanex.NAP.CTS.Model.GraphDB;
    using Fanex.NAP.CTS.Model.Home;
    using Fanex.NAP.CTS.Resource;
    using Fanex.NAP.CTS.Resource.Consts;
    using Fanex.NAP.CTS.ViewModel.Account;
    using Fanex.NAP.CTS.ViewModel.Account.Associated;
    using Fanex.NAP.CTS.ViewModel.Account.GeneralInformation;
    using NPOI.SS.Formula.Functions;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    public class AccountService : IAccountService
    {

        #region General information
        public AccountDetail GetAccountDetailGeneralInformation(GeneralInformationFilter filter)
        {
            var accountCollected = new GeneralAccountCollected();

            accountCollected.CustomerInformation = GetGeneralCustomerInformation(filter);

            accountCollected.EvidenceByTargets = GetEvidenceByAccount(filter);

            accountCollected.AssociatedByAccounts = GetAssociatedByAccounts(filter.AccountId);

            accountCollected.CustomerFraudSuspicious = GetCustomerFraudSuspicious(filter.CustId.ToString());

            return GetAccountDetailGeneralInformation(accountCollected, filter.AccountId, filter.CustId);
        }

        private static IEnumerable<CustomerInformation> GetGeneralCustomerInformation(GeneralInformationFilter filter)
        {
            var criteria = new GetCustomerGeneralInformationCriteria
            {
                ip_accountId = filter.AccountId,
                ip_isMaster = filter.IsMasterSubscriberAssignment,
                ip_userId = filter.UserId
            };

            return DataService.GetData<CustomerInformation>(criteria);
        }

        private static IEnumerable<EvidenceByTarget> GetEvidenceByAccount(GeneralInformationFilter filter)
        {
            var criteria = new GetEvidenceByTargetIdCriteria
            {
                targetId = filter.AccountId,
                targetType = false,
                userId = filter.UserId,
                isMaster = filter.IsMasterSubscriberAssignment
            };

            return DataService.GetData<EvidenceByTarget>(criteria);
        }

        private static IEnumerable<AssociatedByAccount> GetAssociatedByAccounts(int accountId)
        {
            var criteria = new GetAssociationByAccountCriteria
            {
                accountId = accountId
            };

            return DataService.GetData<AssociatedByAccount>(criteria);
        }

        private CustomerFraudSuspicious GetCustomerFraudSuspicious(string customerId)
        {
            var criteria = new GetCustomerFraudInfoCriteria
            {
                custIds = customerId
            };

            using (var db = ObjectDbFactory.CreateInstance(criteria.GetSettingKey()))
            {
                using (var query = db.QueryMultiple(criteria))
                {
                    var customerFrauds = query.Read<CustomerFraud>();
                    var customerSuspiciouses = query.Read<CustomerSuspicious>();

                    return new CustomerFraudSuspicious
                    {
                        CustomerFrauds = customerFrauds,
                        CustomerSuspiciouses = customerSuspiciouses
                    };
                }
            }
        }

        private static AccountDetail GetAccountDetailGeneralInformation(GeneralAccountCollected accountsCollected, int accountId, int custId)
        {
            var accountDetail = new AccountDetail();
            var generalInfo = new AccountGeneralInformation();

            generalInfo.AccountId = accountId;
            generalInfo.CustId = custId;

            if (accountsCollected.CustomerInformation.Any())
            {
                var customerInformation = accountsCollected.CustomerInformation.ToList()[0];

                generalInfo = GetLoginInformation(generalInfo, customerInformation);
            }

            if (accountsCollected.AssociatedByAccounts.Any())
            {
                var associated = accountsCollected.AssociatedByAccounts.ToList()[0];

                generalInfo = GetAssociatedInformation(generalInfo, associated);
            }

            generalInfo = GetEvidenceCodes(generalInfo, accountsCollected.EvidenceByTargets);

            generalInfo = GetFraudInformation(generalInfo, accountsCollected.CustomerFraudSuspicious);

            accountDetail.GeneralInfo = generalInfo;

            return accountDetail;
        }

        private static AccountGeneralInformation GetLoginInformation(AccountGeneralInformation generalInfo, CustomerInformation customerInformation)
        {
            generalInfo.FirstLogin = customerInformation.CreatedDate;
            generalInfo.LastLogin = customerInformation.LastLoginDate;
            generalInfo.FrequencyLogin = customerInformation.LoginCount;
            generalInfo.FirstLoginName = customerInformation.FirstLoginName;
            generalInfo.Username = customerInformation.Username;
            generalInfo.Username2 = customerInformation.Username2;

            if (customerInformation.AccountEvidenceStatus == 0)
            {
                generalInfo.AccountEvidenceStatus = AccountEvidenceStatus.CentralPoint;
            }
            else if (customerInformation.AccountEvidenceStatus == 1)
            {
                generalInfo.AccountEvidenceStatus = AccountEvidenceStatus.Clean;
            }
            else
            {
                generalInfo.AccountEvidenceStatus = AccountEvidenceStatus.Flagged;
            }
            generalInfo.AccountAge = customerInformation.AccountAge;
            generalInfo.SubscriberID = customerInformation.SubscriberID;
            generalInfo.SubscriberName = customerInformation.SubscriberName;

            return generalInfo;
        }

        private static AccountGeneralInformation GetAssociatedInformation(AccountGeneralInformation generalInfo, AssociatedByAccount associated)
        {
            generalInfo.AssociatedAccounts = associated.AssociatedAccount;
            generalInfo.AssociatedDevices = associated.AssociatedDevice;
            generalInfo.SharedDevices = associated.SharedDevices;
            generalInfo.NoSharedDevices = associated.NonShareDevices;
            generalInfo.AccountExceptions = associated.AccountException;

            return generalInfo;
        }

        private static AccountGeneralInformation GetEvidenceCodes(AccountGeneralInformation generalInfo, IEnumerable<EvidenceByTarget> evidenceByTargets)
        {
            var evidenceTypeCodes = evidenceByTargets.Select(e => e.EvidenceTypeCode).OrderBy(e => e);

            generalInfo.EvidenceTypeCodes = evidenceTypeCodes.Join(";");

            return generalInfo;
        }

        private static AccountGeneralInformation GetFraudInformation(AccountGeneralInformation generalInfo, CustomerFraudSuspicious customerFraudSuspicious)
        {
            generalInfo.AutoSuspend = string.Empty;
            generalInfo.AccountStatus = AccountStatus.NA;

            if (customerFraudSuspicious.CustomerFrauds.Any())
            {
                var fraud = customerFraudSuspicious.CustomerFrauds.ToList()[0];
                generalInfo.ProblemAccountStatus = fraud.ProblemAccount ? ProblemAccountStatus.Yes : ProblemAccountStatus.No;
                generalInfo.Abnormal = fraud.Abnormal;
                generalInfo.DangerousOriginal = fraud.danger;
                generalInfo.DangerousNonLive = fraud.danger2;
                generalInfo.DangerousOtherSports = fraud.danger3;
                generalInfo.AutoSuspend = GetAutoSuspend(fraud);
                generalInfo.AccountStatus = fraud.GetStatus();
            }

            var fraudCase = new StringBuilder();
            foreach (var item in customerFraudSuspicious.CustomerSuspiciouses)
            {
                fraudCase.Append(item.FraudCaseName + ", ");
            }
            generalInfo.CommitedFraudCase = fraudCase.ToString().TrimEnd(' ').TrimEnd(',');

            return generalInfo;
        }

        private static string GetAutoSuspend(CustomerFraud fraud)
        {
            var autoSuspend = new StringBuilder();

            if (fraud.autosuspend)
            {
                autoSuspend.Append(ResourcesManager.GetValue(ConstsResources.AUTO_SUSPEND_SOCCER) + ", ");
            }
            if (fraud.autosuspend2)
            {
                autoSuspend.Append(ResourcesManager.GetValue(ConstsResources.AUTO_SUSPEND_OTHER_SPORTS));
            }

            return autoSuspend.ToString().TrimEnd(' ').TrimEnd(',');
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

        public List<CustomerGeneralStatusModel> GetCustomerGeneralStatus(string custIds)
        {
            var criteria = new CustomerGeneralStatusCriteria
            {
                CustIds = custIds
            };

            using (var db = ObjectDbFactory.CreateInstance(criteria.GetSettingKey()))
            {
                var result = db.Query<CustomerGeneralStatusModel>(criteria);

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

        public CustomerAssociationByCTSCustIdModel GetCustomerTotalAssociationByCTSCustId(long ctsCustId, string apiUrl)
        {
            var apiPath = string.Format(apiUrl, ctsCustId);

            var data = ApiHelper.Get<CustomerAssociationByCTSCustIdModel>(apiPath);

            Task.WhenAll(data);

            return data.Result;
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

        public CustomerFlagInfoModel GetCustomerFlagInfo(Int64 ctsCustId, int subscriberId)
        {
            var criteria = new CustomerFlagInfoByCTSCustIdCriteria
            {
                CTSCustId = ctsCustId,
                SubscriberId = subscriberId
            };

            using (var db = ObjectDbFactory.CreateInstance(criteria.GetSettingKey()))
            {
                var result = db.QueryEntity<CustomerFlagInfoModel>(criteria);

                return result;
            }
        }

        public GraphAssociationPaging GetCustomerAssociationByGraph(string apiURL, long ctsCustId, int associationStatus, int skipNumber, int takeNumber)
        {
            var urlString = string.Format(apiURL + "?ctsCustId={0}&assStatus={3}&skipRow={1}&takeRow={2}", ctsCustId, skipNumber, takeNumber, associationStatus);

            var data = ApiHelper.Get<GraphAssociationPaging>(urlString);

            Task.WhenAll(data);

            var customersAPI = data.Result;

            return customersAPI ?? new GraphAssociationPaging();
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

        public List<GraphAssociationModel> FilterCustomerAssociationByStatus(string statusIds,
            List<GraphAssociationModel> customerAssociation,
            List<CustomerGeneralStatusModel> statusByCustIds)
        {
            var accountStatusViewModel = new CustomerGeneralStatusViewModel();
            var notAvailable = accountStatusViewModel.GetAccountStatusName(Model.AccountStatus.NA);
            var notAvailableId = accountStatusViewModel.GetAccountStatusId(notAvailable);

            customerAssociation = customerAssociation.Select(x =>
            {
                if (x.CustId == null || x.CustId == 0)
                {
                    x.CustId = 0;
                    x.StatusId = notAvailableId;
                    x.StatusName = notAvailable;
                }
                else
                {
                    var customerStatus = statusByCustIds.FirstOrDefault(a => a.CustId == x.CustId);

                    if (customerStatus != null)
                    {
                        var accountStatus = accountStatusViewModel.GetAccountStatus(customerStatus);
                        var statusName = accountStatusViewModel.GetAccountStatusName(accountStatus);

                        x.StatusId = accountStatusViewModel.GetAccountStatusId(statusName);
                        x.StatusName = statusName;
                    }
                    else
                    {
                        LogService.Error($"Account status is NULL on custid: { x.CustId }");
                    }
                }

                return x;
            }).ToList();

            //var statusIdList = statusIds.Split(',').Select(Int32.Parse).ToList();

            //var result = customerAssociation.Where(x => statusIdList.Contains(x.StatusId)).ToList();

            return customerAssociation;

        }

        public List<GraphAssociationModel> FilterCustomerAssociationByAssociationType(int associationType, List<GraphAssociationModel> customerAssociation)
        {
            if (customerAssociation != null && customerAssociation.Any())
            {
                if (associationType == 0)
                {
                    var groupData = customerAssociation.GroupBy(x => x.CTSCustID)
                        .Select(g => new GraphAssociationModel
                        {
                            CustId = g.FirstOrDefault().CustId,
                            AssociationType = g.Count() > 1 ? 0 : g.FirstOrDefault().AssociationType,
                            AssociationTypeName = CalculateAssociationType(g),
                            Username = g.FirstOrDefault().Username,
                            Username2 = g.FirstOrDefault().Username2,
                            AssociationDateGMT0 = g.FirstOrDefault().AssociationDateGMT0,
                            SharedDevices = g.FirstOrDefault().SharedDevices,
                            SharedIPs = g.FirstOrDefault().SharedIPs,
                            StatusName = g.FirstOrDefault().StatusName,
                            StatusId = g.FirstOrDefault().StatusId,
                            SubscriberId = g.FirstOrDefault().SubscriberId,
                            CTSCustID = g.FirstOrDefault().CTSCustID,
                            AssociationStatus = g.FirstOrDefault().AssociationStatus
                        }).ToList();

                    return groupData;
                }

                return customerAssociation.Where(x => x.AssociationType == associationType).Select(x =>
                {
                    x.AssociationTypeName = x.GetAssociationTypeName(x.AssociationType);
                    return x;
                }).ToList();
            }

            return customerAssociation;
        }

        private string CalculateAssociationType(IGrouping<long, GraphAssociationModel> groupData)
        {
            if (groupData.Count() > 1)
            {
                return groupData.Select(x => x.GetAssociationTypeName(x.AssociationType)).Join(", ");
            }

            return groupData.FirstOrDefault().GetAssociationTypeName(groupData.FirstOrDefault().AssociationType);
        }

        public List<AssociationCustomerModel> SearchAssociatedCustomer(string username)
        {
            var criteria = new GetCustomerByUsernameCriteria
            {
                Username = username
            };

            using (var db = ObjectDbFactory.CreateInstance(criteria.GetSettingKey()))
            {
                var result = db.Query<AssociationCustomerModel>(criteria);

                return result.ToList();
            }
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
            catch(Exception ex)
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
                SubscriberId = model.SubscriberId,
                EvidenceId = model.EvidenceId,
                Remark = model.Comment,
                UserId = model.UserId,
                IsCreatedByMaster = true
            };

            return DataService.UpdateData(criteria, "Place evidence");
        }

        public void FPSAddNewEvidence(string apiURL, UpdatePlaceEvidenceModel model)
        {
            var data  = ApiHelper.Post<AddNewEvidenceForFPSModel>(apiURL, new AddNewEvidenceForFPSModel
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

            return DataService.UpdateData(criteria, "Remove flagged evidence");
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

        public int AddAccountException(UpdateAccountExceptionModel model)
        {
            var criteria = new AddAccountExceptionCriteria
            {
                UserId = model.UserId,
                FromCTSCustId = model.FromCTSCustId,
                ToUserName = model.ToUserName,
                Comment = model.Comment,
                ErrorCode = 0
            };

            using (var db = ObjectDbFactory.CreateInstance(criteria.GetSettingKey()))
            {
                try
                {
                    db.ExecuteNonQuery(criteria);
                }
                catch (Exception ex)
                {
                    if (ex.Message != string.Empty && ex.Message.ToLower().Contains("duplicate"))
                        return 1062;
                }

                return criteria.ErrorCode;
            }
        }

        public int RemoveAccountException(UpdateAccountExceptionModel model)
        {
            var criteria = new RemoveAccountExceptionCriteria
            {
                UserId = model.UserId,
                SubscriberId = model.SubscriberId,
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

        public IEnumerable<CustomerClassificationModel> GetCustomerClassification(long custId)
        {
            var criteria = new GetCTSCustomerClassificationCriteria
            {
                CustId = custId
            };

            using (var db = ObjectDbFactory.CreateInstance(criteria.GetSettingKey()))
            {
                return db.Query<CustomerClassificationModel>(criteria);
            }
        }

        public DBResult AddCustomerClassification(long custId, long ctsCustId, int subscriberId, int categoryId, string sportGroupIds, string remark, int createdBy)
        {
            var criteria = new AddCTSCustomerClassificationCriteria
            {
                CustId = custId,
                CTSCustId = ctsCustId,
                SubscriberId = subscriberId,
                CategoryId = categoryId,
                SportGroupIds = sportGroupIds,
                CreatedBy = createdBy,
                Remark = remark,
                ErrorMessage = string.Empty
            };

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

            return new DBResult
            {
                ErrorCode = -1,
                ErrorMsg = criteria.ErrorMessage
            };
        }

        public DBResult UpdateCustomerClassification(long custId, long ctsCustId, int subscriberId, int categoryId, string sportGroupIds, string remark, int createdBy)
        {
            var criteria = new UpdateCTSCustomerClassificationCriteria
            {
                CustId = custId,
                CTSCustId = ctsCustId,
                SubscriberId = subscriberId,
                CategoryId = categoryId,
                SportGroupIds = sportGroupIds,
                CreatedBy = createdBy,
                Remark = remark,
                ErrorMessage = string.Empty
            };

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

            return new DBResult
            {
                ErrorCode = -1,
                ErrorMsg = criteria.ErrorMessage
            };
        }

        public DBResult RemoveCustomerClassification(long custId, long ctsCustId, int categoryId, int createdBy)
        {
            var criteria = new RemoveCTSCustomerClassificationCriteria
            {
                CustId = custId,
                CTSCustId = ctsCustId,
                CategoryId = categoryId,
                CreatedBy = createdBy,
                ErrorMessage = string.Empty
            };

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

            return new DBResult
            {
                ErrorCode = -1,
                ErrorMsg = criteria.ErrorMessage
            };
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
    }
}