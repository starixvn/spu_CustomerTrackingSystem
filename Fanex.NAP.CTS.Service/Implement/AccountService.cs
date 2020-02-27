namespace Fanex.NAP.CTS.Service
{
    using Fanex.Data;
    using Fanex.NAP.CTS.Core;
    using Fanex.NAP.CTS.Core.BaseClasses;
    using Fanex.NAP.CTS.Core.Extentions;
    using Fanex.NAP.CTS.Core.Helpers;
    using Fanex.NAP.CTS.Core.Helpers.Api;
    using Fanex.NAP.CTS.Criteria.Account;
    using Fanex.NAP.CTS.Criteria.Account.Associated;
    using Fanex.NAP.CTS.Criteria.Account.ChainToEvidence;
    using Fanex.NAP.CTS.Criteria.Account.GeneralInformation;
    using Fanex.NAP.CTS.Criteria.Home;
    using Fanex.NAP.CTS.Criteria.SubscriberAssignment;
    using Fanex.NAP.CTS.Model;
    using Fanex.NAP.CTS.Model.Account;
    using Fanex.NAP.CTS.Model.DTO;
    using Fanex.NAP.CTS.Model.GraphDB;
    using Fanex.NAP.CTS.Model.Home;
    using Fanex.NAP.CTS.Model.Subscriber;
    using Fanex.NAP.CTS.Resource;
    using Fanex.NAP.CTS.Resource.Consts;
    using Fanex.NAP.CTS.ViewModel.Account;
    using Fanex.NAP.CTS.ViewModel.Account.Associated;
    using Fanex.NAP.CTS.ViewModel.Account.GeneralInformation;
    using Fanex.NAP.CTS.ViewModel.Account.Search;
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
            generalInfo.AccountStatus = AccountStatus.NotYetDeposit;

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
        public IEnumerable<AssociatedAccountByAccount> GetAssociatedAccountByAccounts(AssociatedAccountByAccountFilter filter, out string msgError)
        {
            var accountCollected = new AssociatedAccountsCollected();

            accountCollected.CustomersFromAPI = GetCustomersFromAPI(filter);

            accountCollected.AccountsByCustomers = GetAssociatedAccountByCustomers(filter, accountCollected.CustomersFromAPI);

            accountCollected.CustomerFraudSuspicious = GetCustomerFraud(accountCollected.AccountsByCustomers);

            return GetAssociatedAccountByAccount(accountCollected, filter, out msgError);
        }

        private static IEnumerable<CustomerByGraphDB> GetCustomersFromAPI(AssociatedAccountByAccountFilter filter)
        {
            var customers = new List<CustomerByGraphDB>();
            var apiURLOneLevel = AppSettings.Get(Consts.GraphDBGetAssociationOneLevel);
            var apiURLMoreLevel = AppSettings.Get(Consts.GraphDBGetAssociationOneLevel);

            if (filter.DeviceAwayOne)
            {
                customers.AddRange(GetCustomersFromAPI(filter.CustId, apiURLOneLevel));
            }

            if (filter.DeviceAwayMore)
            {
                customers.AddRange(GetCustomersFromAPI(filter.CustId, apiURLMoreLevel));
            }

            return customers;
        }

        private static IEnumerable<CustomerByGraphDB> GetCustomersFromAPI(int custId, string apiURL)
        {
            var data = ApiHelper.Post<IEnumerable<CustomerByGraphDB>>(apiURL, custId);

            Task.WhenAll(data);

            var customersAPI = data.Result;

            return customersAPI ?? new List<CustomerByGraphDB>();
        }

        private static IEnumerable<AssociatedAccountByCustomer> GetAssociatedAccountByCustomers(AssociatedAccountByAccountFilter filter, IEnumerable<CustomerByGraphDB> customersFromAPI)
        {
            if (customersFromAPI.Any())
            {
                var customerIds = customersFromAPI.Select(c => c.CustId).Distinct().Join();

                var criteria = new GetAssociatedAccountByAccountCriteria
                {
                    custIds = customerIds,
                    rootAccountId = filter.AccountId,
                    userId = filter.userId,
                    isMaster = filter.isMasterSubscriberAssignment
                };

                return DataService.GetData<AssociatedAccountByCustomer>(criteria);
            }

            return new List<AssociatedAccountByCustomer>();
        }

        private CustomerFraudSuspicious GetCustomerFraud(IEnumerable<AssociatedAccountByCustomer> associatedCustomers)
        {
            if (associatedCustomers.Any())
            {
                var customerIds = associatedCustomers.Select(c => c.CustID).Join();

                var criteria = new GetCustomerFraudInfoCriteria
                {
                    custIds = customerIds
                };

                using (var db = ObjectDbFactory.CreateInstance(criteria.GetSettingKey()))
                {
                    using (var query = db.QueryMultiple(criteria))
                    {
                        var customerFrauds = query.Read<CustomerFraud>();

                        return new CustomerFraudSuspicious
                        {
                            CustomerFrauds = customerFrauds
                        };
                    }
                }
            }

            return new CustomerFraudSuspicious
            {
                CustomerFrauds = new List<CustomerFraud>()
            };
        }

        private static IEnumerable<AssociatedAccountByAccount> GetAssociatedAccountByAccount(AssociatedAccountsCollected accountsCollected, AssociatedAccountByAccountFilter filter, out string msgError)
        {
            try
            {
                msgError = string.Empty;

                return accountsCollected.AccountsByCustomers.AsEnumerable()
                                .Where(customer => customer.CompareEvidenceStatus(filter.EvidenceStatus))
                                .Join(accountsCollected.CustomersFromAPI.Distinct(c => c.CustId).AsEnumerable(),
                                    cusA => cusA.CustID,
                                    cusB => cusB.CustId,
                                    (cusA, cusB) => GetAssociatedAccountByGraph(cusA, cusB))
                                .Join(accountsCollected.CustomerFraudSuspicious.CustomerFrauds
                                    .Where(customer => customer.CompareStatus(filter.Status)),
                                    cusA => cusA.CustID,
                                    cusB => cusB.custid,
                                    (cusA, cusB) => GetAssociatedAccount(cusA, cusB)).ToList();
            }
            catch (Exception ex)
            {
                msgError = ex.Message;

                return new List<AssociatedAccountByAccount>();
            }
        }

        private static AssociatedAccountByAccount GetAssociatedAccountByGraph(AssociatedAccountByCustomer customerA, CustomerByGraphDB customerB)
        {
            return new AssociatedAccountByAccount
            {
                AccountID = customerA.AccountID,
                CustID = customerA.CustID,
                SubscriberName = "",
                Username = customerB.Username,
                Username2 = customerB.Username2,
                LastLogin = customerA.LastLogin,
                FirstLogin = customerA.FirstLogin,
                AssociatedWithRootAccountOn = customerA.AssociatedWithRootAccountOn,
                TotalLoginWithRootAccount = customerA.TotalLoginWithRootAccount,
                EvidenceLevel = customerA.EvidenceLevel
            };
        }

        private static AssociatedAccountByAccount GetAssociatedAccount(AssociatedAccountByAccount customerA, CustomerFraud customerB)
        {
            return new AssociatedAccountByAccount
            {
                AccountID = customerA.AccountID,
                CustID = customerA.CustID,
                SubscriberName = customerA.SubscriberName,
                Username = customerA.Username,
                Username2 = customerA.Username2,
                LastLogin = customerA.LastLogin,
                FirstLogin = customerA.FirstLogin,
                AssociatedWithRootAccountOn = customerA.AssociatedWithRootAccountOn,
                TotalLoginWithRootAccount = customerA.TotalLoginWithRootAccount,
                EvidenceLevel = customerA.EvidenceLevel,
                Status = customerB.GetStatusName()
            };
        }

        #endregion

        #region Associated device by account
        public IEnumerable<AssociatedDeviceByAccount> GetAssociatedDevicesByAccount(AssociatedDeviceByAccountFilter filter)
        {
            return GetAssociatedDevicesByAccount_Test(filter);
        }

        private static IEnumerable<AssociatedDeviceByAccount> GetAssociatedDevicesByAccount_Test(AssociatedDeviceByAccountFilter filter)
        {
            var lstAssociated = new List<AssociatedDeviceByAccount>();

            for (int i = 1; i < 6; i++)
            {
                var deviceAssociated = new AssociatedDeviceByAccount();
                deviceAssociated.DeviceNo = "08d99c65629142b9903ab56b2f3511ba";
                deviceAssociated.FirstUsed = DateTime.Now;
                deviceAssociated.LastUsed = DateTime.Now;
                deviceAssociated.AssociatedDate = DateTime.Now;
                deviceAssociated.TotalUses = 100 + i;
                deviceAssociated.GraphURL = "url 123456";
                if (i % 2 == 0)
                {
                    deviceAssociated.EvidenceStatus = EvidenceStatus.Affected.ToString();
                }
                else
                {
                    deviceAssociated.EvidenceStatus = EvidenceStatus.Clean.ToString();
                }

                lstAssociated.Add(deviceAssociated);
            }

            return lstAssociated;
        }


        //CTS new Implement
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

        public CustomerAssociationByCTSCustIdModel GetCustomerTotalAssociationByCTSCustId(long ctsCustId)
        {
            var criteria = new CustomerAssociationByCTSCustIdCriteria
            {
                CTSCustId = ctsCustId                
            };

            using (var db = ObjectDbFactory.CreateInstance(criteria.GetSettingKey()))
            {
                var result = db.QueryEntity<CustomerAssociationByCTSCustIdModel>(criteria);

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
                using (var result = db.QueryMultiple(criteria))
                {
                    var suspiciouseCustomer = result.ReadFirstOrDefault<SuspiciousCustomerModel>();
                    var fraudCaseList = result.Read<FraudCaseModel>().ToList();

                    return new CustomerProfileFraudInfoModel
                    {
                        SuspiciouseCustomer = suspiciouseCustomer,
                        FraudCaseList = fraudCaseList
                    };
                }
            }
        }

        //public List<CustomerAssociationByAccountDetailModel> GetDirectCustomerAssociatedByAccount(string custIds)
        //{
        //    var criteria = new GetDirectAssociationByAccountCriteria
        //    {
        //        CustIds = custIds
        //    };

        //    using (var db = ObjectDbFactory.CreateInstance(criteria.GetSettingKey()))
        //    {
        //        var result = db.Query<CustomerAssociationByAccountDetailModel>(criteria);

        //        return result.ToList();
        //    }
        //}

        public List<TagModel> GetDirectAssociatedAccountTagAffected(string custIdsAssInfo)
        {
            var criteria = new GetDirectAssociatedAccountTagAffectedCriteria
            {
                CustAssInfo = custIdsAssInfo
            };

            using (var db = ObjectDbFactory.CreateInstance(criteria.GetSettingKey()))
            {
                var result = db.Query<TagModel>(criteria);

                return result.ToList();
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

        public List<GraphAssociationModel> GetCustomerAssociationByGraph(string apiURL, long ctsCustId)
        {
            var data = ApiHelper.Post<List<GraphAssociationModel>>(apiURL, ctsCustId);

            Task.WhenAll(data);

            var customersAPI = data.Result;

            return customersAPI ?? new List<GraphAssociationModel>();
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
            var notYetDepositStatusName = accountStatusViewModel.GetAccountStatusName(Model.AccountStatus.NotYetDeposit);
            var notYetDepositStatusId = accountStatusViewModel.GetAccountStatusId(notYetDepositStatusName);

            customerAssociation = customerAssociation.Select(x =>
            {
                if (x.CustId == null)
                {
                    x.CustId = 0;
                    x.StatusId = notYetDepositStatusId;
                    x.StatusName = notYetDepositStatusName;
                }

                return x;
            }).ToList();

            var statusIdList = statusIds.Split(',').Select(Int32.Parse).ToList();

            var accountStatus = statusByCustIds.Select(x => new AccountStatusModel
            {
                CustId = x.CustId,
                Username = x.Username,
                StatusName = accountStatusViewModel.GetAccountStatusName(accountStatusViewModel.GetAccountStatus(x)),
                StatusId = accountStatusViewModel.GetAccountStatusId(accountStatusViewModel.GetAccountStatusName(accountStatusViewModel.GetAccountStatus(x)))
            }).Where(a => statusIdList.Contains(a.StatusId)).ToList();

            if (!statusIdList.Any(x => notYetDepositStatusId == x))
            {
                customerAssociation = customerAssociation.Where(x => x.StatusId != notYetDepositStatusId).ToList();
            }

            var result = customerAssociation.Select(x =>
            {
                if (x.StatusId == 0)
                {
                    var status = accountStatus.FirstOrDefault(a => a.CustId == x.CustId);

                    if (status != null)
                    {
                        x.StatusId = status.StatusId;
                        x.StatusName = status.StatusName;
                    }
                }

                return x;
            }).ToList();

            return result;

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
                            AssociationType = (g.Count() > 1 ? 0 : g.FirstOrDefault().AssociationType),
                            Username = g.FirstOrDefault().Username,
                            Username2 = g.FirstOrDefault().Username2,
                            AssociationDate = g.FirstOrDefault().AssociationDate,
                            SharedDevices = g.FirstOrDefault().SharedDevices,
                            SharedIPs = g.FirstOrDefault().SharedIPs,
                            StatusName = g.FirstOrDefault().StatusName,
                            StatusId = g.FirstOrDefault().StatusId,
                            SubscriberId = g.FirstOrDefault().SubscriberId,
                            CTSCustID = g.FirstOrDefault().CTSCustID
                        }).ToList();



                    return groupData;
                }

                return customerAssociation.Where(x => x.AssociationType == associationType).ToList();
            }

            return customerAssociation;
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
    }
}