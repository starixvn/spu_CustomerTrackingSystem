namespace Fanex.NAP.CTS.Service
{
    using Fanex.Data;
    using Fanex.Data.Repository;
    using Fanex.Logging;
    using Fanex.NAP.CTS.Core.Extentions;
    using Fanex.NAP.CTS.Core.Utilities;
    using Fanex.NAP.CTS.Criteria;
    using Fanex.NAP.CTS.Criteria.Account.ChainToEvidence;
    using Fanex.NAP.CTS.Criteria.CustomerClassify;
    using Fanex.NAP.CTS.Criteria.ProblemAccountMonitor;
    using Fanex.NAP.CTS.Model;
    using Fanex.NAP.CTS.Model.CustomerClassify;
    using Fanex.NAP.CTS.Model.LogInputData;
    using Fanex.NAP.CTS.Model.RescanCustClassificationModel;
    using Fanex.NAP.CTS.Model.SyncCustomerToMainDb;
    using Fanex.NAP.CTS.Resource.Consts;
    using Newtonsoft.Json;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using static Fanex.NAP.CTS.Resource.Consts.Consts;

    public class CustomerClassifyService : ICustomerClassifyService
    {
        private readonly IPushDataToExternalService _pushDataToExternalService;
        private readonly ILogInputDataService logInputDataService;
        private readonly bool _isWriteLogInfo;

        public CustomerClassifyService()
        {
            _pushDataToExternalService = new PushDataToExternalService();
            _isWriteLogInfo = ConfigSettings.IsWriteLogInfo;
            logInputDataService = new LogInputDataService();
        }

        public void RescanCustomerClassify(RescanCustClassificationModel para)
        {
            var custIdsPushCC = RescanProblemAccont(para);

            RescanNormalAccount(para.CustIds?.ToList(), custIdsPushCC, para.FunctionId);
        }

        private void SyncCustomerClassToExternal(List<long> custIdsToSyncCC, bool isPushCCToSoon88)
        {
            var custIdsChunk = custIdsToSyncCC.Chunk(ConfigSettings.PushCCToExternalChunkSize);

            if (_isWriteLogInfo)
                LogService.Info($"PA Rescan - Cust to syncCC - with data: {JsonConvert.SerializeObject(custIdsToSyncCC)}");

            foreach (var custids in custIdsChunk)
            {
                var lastestClassifications = GetLatestClassification(string.Join(",", custids));

                if (lastestClassifications != null && lastestClassifications.Any())
                {
                    bool isSyncMainSuccessfully = _pushDataToExternalService.PushCustomerClassificationToMain(lastestClassifications, 3);

                    if (isSyncMainSuccessfully)
                        UpdateAppliedCustomerClassification(JsonConvert.SerializeObject(lastestClassifications));

                    if (isPushCCToSoon88)
                        _pushDataToExternalService.PushCustomerClassificationToSoon88(lastestClassifications);
                }
            }
        }

        #region Rescan PA

        private IEnumerable<long> RescanProblemAccont(RescanCustClassificationModel para)
        {
            var custIdsCheckPA = new List<long>();

            //Rescan PA - Get SMA CustId
            var roleCusomters = GetAgentCusts(para.CustIds);
            custIdsCheckPA.AddRange(roleCusomters);

            // Rescan PA - Get PA From Evidence in CTS
            var problemAccountsFromEvi = SyncCustomerEvidenceToCategory(para.CustIds);
            custIdsCheckPA.AddRange(problemAccountsFromEvi);

            // Rescan PA - Get PA From RobotUser & NAP PA
            var problemAccounts = GetProblemAccounts(para);

            if (problemAccounts != null && problemAccounts.Any())
            {
                custIdsCheckPA.AddRange(InsertProblemAccount(problemAccounts));
            }

            //Rescan PA - Push CC & CN88
            var custIdsPushCC = custIdsCheckPA.Distinct().ToList();

            if (custIdsPushCC != null && custIdsPushCC.Any())
            {
                SyncCustomerClassToExternal(custIdsPushCC, para.IsPushCCToSoon88);
            }

            return custIdsPushCC;
        }

        private List<long> GetAgentCusts(IEnumerable<long> custIds)
        {
            var agentIds = new List<long>();

            if (custIds != null && custIds.Any())
            {
                var data = GetRoleCustomer(string.Join(",", custIds)).ToList();

                if (data != null && data.Any())
                {
                    agentIds = data.Where(c => c.RoleID == (int)Consts.ROLE_CUSTOMER.AGENT
                                            || c.RoleID == (int)Consts.ROLE_CUSTOMER.MASTER
                                            || c.RoleID == (int)Consts.ROLE_CUSTOMER.SUPER)
                               .Select(c => c.CustID).ToList();
                }
            }

            if (_isWriteLogInfo)
                LogService.Info($"WEB - PA Rescan - GetAgentCusts - with data: {JsonConvert.SerializeObject(agentIds)}");

            return agentIds;
        }

        private List<int> SyncRobotUserToUpdateCategory(IEnumerable<long> custIds)
        {
            var robotUser = new List<int>();

            var custs = GetCustByRobotSiteRule(string.Join(",", custIds)).ToList();

            var getRobotUser = GetExistedRobotUserDbAccess(string.Join(",", custs));

            if (!string.IsNullOrEmpty(getRobotUser))
            {
                robotUser = getRobotUser.Split(',').Select(int.Parse).ToList();
            }

            return robotUser;
        }

        private List<long> SyncCustomerEvidenceToCategory(IEnumerable<long> custIds)
        {
            var custIdsToSyncCC = new List<long>();

            var existedCustEvidences = GetExistedCustomerEvidences(string.Join(",", custIds)).ToList();

            var result = SyncCustomerEvidenceToCategory(existedCustEvidences);

            if (string.IsNullOrEmpty(result.ErrorMessage) && result.CustIds != null && result.CustIds.Any())
            {
                custIdsToSyncCC.AddRange(result.CustIds);
            }

            if (_isWriteLogInfo)
                LogService.Info($"Web - RescanPA - Get Evidence Custs - with data:  {JsonConvert.SerializeObject(existedCustEvidences)}");

            return custIdsToSyncCC;
        }

        private InsertCategoryResultModel SyncCustomerEvidenceToCategory(List<CustEvidenceModel> custEvidences)
        {
            try
            {
                if (custEvidences == null || !custEvidences.Any())
                {
                    return new InsertCategoryResultModel() { ErrorMessage = string.Empty, CustIds = null };
                }

                var evidencesChunk = custEvidences.Chunk(ConfigSettings.UpdateInternalChunkSize);

                var custIds = new List<long>();

                foreach (var evidences in evidencesChunk)
                {
                    custIds.AddRange(SyncCustEvidenceToCategoryDbAccess(JsonConvert.SerializeObject(evidences), true));
                }

                return new InsertCategoryResultModel { ErrorMessage = string.Empty, CustIds = custIds };
            }
            catch (Exception ex)
            {
                LogService.Error($"Web - CustomerClassifyService - SyncCustomerEvidenceToCategory exception: {ex.Message} - Data: {JsonConvert.SerializeObject(custEvidences)}", ex);

                return new InsertCategoryResultModel { ErrorMessage = ex.Message, CustIds = null };
            }
        }

        private List<MultipleProblemAccountsModel> GetProblemAccounts(RescanCustClassificationModel para)
        {
            var problemAccounts = new List<MultipleProblemAccountsModel>();

            if (para.CustIds != null && para.CustIds.Any())
            {
                var existedRobotUsers = GetExistedRobotUser(para.CustIds);
                problemAccounts.AddRange(existedRobotUsers);

                logInputDataService.LogInputData(new LogInputViewModel { ProblemAccounts = existedRobotUsers?.ToList() }, para.FuncId_CheckRobotUser);

                //PA - NAP PA
                if (para.IsCheckPAFromNAP)
                {
                    var existedProblemAccounts = GetExistedProblemAccountsFromNAPPA(string.Join(",", para.CustIds)).ToList();
                    problemAccounts.AddRange(existedProblemAccounts);

                    logInputDataService.LogInputData(new LogInputViewModel { ProblemAccounts = existedProblemAccounts }, para.FuncId_CheckPA);
                }
            }

            return problemAccounts;
        }

        private List<long> InsertProblemAccount(List<MultipleProblemAccountsModel> problemAccounts)
        {
            var custIdsToSyncCC = new List<long>();

            if (problemAccounts != null && problemAccounts.Any())
            {
                var result = InsertProblemAccountToCTS(problemAccounts);

                if (string.IsNullOrEmpty(result.ErrorMessage) && result.CustIds != null)
                {
                    custIdsToSyncCC = result.CustIds.ToList();
                }
            }

            if (_isWriteLogInfo)
                LogService.Info($"PA Rescan - Get PA Custs (Robot & NAPPA) - with data: {JsonConvert.SerializeObject(custIdsToSyncCC)}");

            return custIdsToSyncCC;
        }

        private IEnumerable<MultipleProblemAccountsModel> GetExistedProblemAccountsFromNAPPA(string custIds)
        {
            var probAccFromNap = new List<MultipleProblemAccountsModel>();

            if (!string.IsNullOrEmpty(custIds))
            {
                var problemAccountsNAPCM = GetProblemAccountsFromNAP(custIds, null).ToList();
                var problemAccountsCN88 = GetProblemAccountsFromNAP_CN88(custIds, null).ToList();

                if (problemAccountsNAPCM != null && problemAccountsNAPCM.Any())
                {
                    probAccFromNap.AddRange(problemAccountsNAPCM);
                }

                if (problemAccountsCN88 != null && problemAccountsCN88.Any())
                {
                    probAccFromNap.AddRange(problemAccountsCN88);
                }
            }

            if (_isWriteLogInfo)
                LogService.Info($"PA Rescan - Get PA Custs - with data: {JsonConvert.SerializeObject(probAccFromNap)}");

            return probAccFromNap.Distinct();
        }

        private IEnumerable<MultipleProblemAccountsModel> GetExistedRobotUser(IEnumerable<long> custIds)
        {
            var existedRobots = new List<MultipleProblemAccountsModel>();

            if (custIds != null && custIds.Any())
            {
                var robotUsers = SyncRobotUserToUpdateCategory(custIds);

                if (robotUsers != null && robotUsers.Any())
                {
                    robotUsers.ForEach(c =>
                    {
                        existedRobots.Add(new MultipleProblemAccountsModel()
                        {
                            CustID = c,
                            ReasonID = (int)CATEGORY.ROBOT
                        });
                    });
                }
            }

            if (_isWriteLogInfo)
                LogService.Info($"PA Rescan - Get RobotUsers - with data: {JsonConvert.SerializeObject(existedRobots)}");

            return existedRobots;
        }

        private InsertCategoryResultModel InsertProblemAccountToCTS(List<MultipleProblemAccountsModel> problemAccounts)
        {
            try
            {
                if (problemAccounts == null || !problemAccounts.Any())
                {
                    return new InsertCategoryResultModel() { ErrorMessage = string.Empty, CustIds = null };
                }

                var probalemAccountsChunk = problemAccounts.Chunk(ConfigSettings.UpdateInternalChunkSize);

                var custIds = new List<long>();

                foreach (var problemAccs in probalemAccountsChunk)
                {
                    var result = InsertProblemAccountDbAccess(JsonConvert.SerializeObject(problemAccs), true);

                    custIds.AddRange(result);
                }

                return new InsertCategoryResultModel() { ErrorMessage = string.Empty, CustIds = custIds };
            }
            catch (Exception ex)
            {
                LogService.Error($"Web - Rescan PA - InsertProblemAccountToCTS exception: {ex.Message} - Data: { JsonConvert.SerializeObject(problemAccounts)}", ex);

                return new InsertCategoryResultModel { ErrorMessage = ex.Message, CustIds = null };
            }
        }

        #endregion Rescan PA

        #region Rescan Normal

        private void RescanNormalAccount(List<long> custIds, IEnumerable<long> custRescanPA, FunctionId funcId)
        {
            if (custIds != null && custIds.Any())
            {
                custIds.RemoveAll(x => custRescanPA.Contains(x));

                if (_isWriteLogInfo)
                    LogService.Info($"PA Rescan - Cust to syncCC - with data: {JsonConvert.SerializeObject(custIds)}");

                var custIdsChunk = custIds.Chunk(ConfigSettings.PushCCToExternalChunkSize).ToList();

                foreach (var custRescanNormal in custIdsChunk)
                {
                    var custToString = string.Join(",", custRescanNormal);

                    ClassifyCustomerGeneralCategoryDaily(custToString, funcId, false);
                }
            }
        }

        private void ClassifyCustomerGeneralCategoryDaily(string customers, FunctionId funcId, bool isCheckLast14Days)
        {
            var repository = new DynamicRepository();

            var criteria = new NormalAccountsClassificationGeneralCategorizeDailyCriteria
            {
                ListCustIds = customers,
                IsCheckLast14Days = isCheckLast14Days,
                FunctionId = (int)funcId
            };

            repository.Execute(criteria);
        }

        #endregion Rescan Normal

        #region DataAccess Layer

        private IEnumerable<CustEvidenceModel> GetExistedCustomerEvidences(string custIds)
        {
            var existedEvidences = new List<CustEvidenceModel>();

            if (string.IsNullOrEmpty(custIds))
            {
                return existedEvidences;
            }

            var criteria = new GetExistedEvidencesCriteria
            {
                CustIds = custIds
            };

            using (var db = ObjectDbFactory.CreateInstance(criteria.GetSettingKey()))
            {
                existedEvidences = db.Query<CustEvidenceModel>(criteria).ToList();
            }

            return existedEvidences ?? new List<CustEvidenceModel>();
        }

        private List<long> SyncCustEvidenceToCategoryDbAccess(string custEvidencesJson, bool isRescan)
        {
            var custIds = new List<long>();
            var repository = new DynamicRepository();

            if (string.IsNullOrEmpty(custEvidencesJson))
            {
                return custIds;
            }

            var criteria = new CustEvidenceSyncToCategoryCriteria
            {
                CustEvidencesJson = custEvidencesJson,
                IsRescan = isRescan
            };

            using (var db = ObjectDbFactory.CreateInstance(criteria.GetSettingKey()))
            {
                custIds = repository.Fetch<long>(criteria).ToList();
            }

            if (!string.IsNullOrEmpty(criteria.ErrorMessage))
            {
                LogService.Error($"Web - CustomerClassifyService - SyncCustomerEvidenceToCategory failed: {criteria.ErrorMessage} - Data: {custEvidencesJson}");
            }

            if (_isWriteLogInfo)
                LogService.Info($"Web - CustomerClassifyService - SyncCustomerEvidenceToCategory custEvidencesJson:  {custEvidencesJson}");

            return (custIds != null) ? custIds : new List<long>();
        }

        private IEnumerable<MultipleProblemAccountsModel> GetProblemAccountsFromNAP(string custIDs, DateTime? dateTime)
        {
            var criteria = new GetProblemAccountsCriteria
            {
                CustIds = custIDs,
                Datetime = dateTime
            };

            using (var db = ObjectDbFactory.CreateInstance(criteria.GetSettingKey()))
            {
                return db.Query<MultipleProblemAccountsModel>(criteria).ToList();
            }
        }

        private IEnumerable<MultipleProblemAccountsModel> GetProblemAccountsFromNAP_CN88(string custIDs, DateTime? dateTime)
        {
            var criteria = new GetProblemAccountsCN88Criteria
            {
                CustIds = custIDs,
                Datetime = dateTime
            };

            using (var db = ObjectDbFactory.CreateInstance(criteria.GetSettingKey()))
            {
                return db.Query<MultipleProblemAccountsModel>(criteria).ToList();
            }
        }

        private List<long> InsertProblemAccountDbAccess(string problemAccountJson, bool isRescan)
        {
            var repository = new DynamicRepository();
            var custIds = new List<long>();

            if (string.IsNullOrEmpty(problemAccountJson))
            {
                return custIds;
            }

            var criteria = new InsertProblemAccountCriteria
            {
                ProblemAccountJson = problemAccountJson,
                IsRescan = isRescan
            };

            using (var db = ObjectDbFactory.CreateInstance(criteria.GetSettingKey()))
            {
                custIds = repository.Fetch<long>(criteria).ToList();
            }

            if (!string.IsNullOrEmpty(criteria.ErrorMessage))
            {
                LogService.Error($"Web - CustomerClassifyService - InsertProblemAccount failed: {criteria.ErrorMessage} - Data: {problemAccountJson}");
            }

            if (_isWriteLogInfo)
                LogService.Info($"Web - CustomerClassifyService - InsertProblemAccount problemAccountJson:  {problemAccountJson}");

            return (custIds != null) ? custIds : new List<long>();
        }

        public IEnumerable<LatestClassificationModel> GetLatestClassification(string custIds)
        {
            if (string.IsNullOrEmpty(custIds))
            {
                return new List<LatestClassificationModel>();
            }

            var criteria = new GetLastestClassificationCriteria
            {
                CustIds = custIds
            };

            using (var db = ObjectDbFactory.CreateInstance(criteria.GetSettingKey()))
            {
                return db.Query<LatestClassificationModel>(criteria);
            }
        }

        public void UpdateAppliedCustomerClassification(string jsonData)
        {
            var repository = new DynamicRepository();

            var criteria = new UpdateAppliedCustomerClassificationCriteria
            {
                ProblemCustomerJson = jsonData
            };

            using (var db = ObjectDbFactory.CreateInstance(criteria.GetSettingKey()))
            {
                repository.Fetch<string>(criteria);

                if (!string.IsNullOrEmpty(criteria.ErrorMessage))
                {
                    LogService.Error($"Web - CustomerClassifyService - UpdateAppliedCustomerClassification failed {criteria.ErrorMessage} with data: {jsonData}");
                }
            }
        }

        private IEnumerable<RoleCustomerModel> GetRoleCustomer(string custIds)
        {
            var roleCus = new List<RoleCustomerModel>();

            var criteria = new GetRoleCustomerCriteria
            {
                CustIds = custIds
            };

            using (var db = ObjectDbFactory.CreateInstance(criteria.GetSettingKey()))
            {
                roleCus = db.Query<RoleCustomerModel>(criteria).ToList();
            }

            if (_isWriteLogInfo)
                LogService.Info($"PA Rescan - GetAgentCusts - with data: {JsonConvert.SerializeObject(roleCus)}");

            return roleCus;
        }

        private IEnumerable<int> GetCustByRobotSiteRule(string custIds)
        {
            var roleCus = new List<int>();

            var criteria = new GetCustByRobotSiteRuleCriteria
            {
                CustIds = custIds
            };

            using (var db = ObjectDbFactory.CreateInstance(criteria.GetSettingKey()))
            {
                roleCus = db.Query<int>(criteria).ToList();
            }

            return roleCus;
        }

        private string GetExistedRobotUserDbAccess(string custIds)
        {
            var criteria = new GetExistedRobotUserCriteria
            {
                CustIds = custIds
            };

            using (var db = ObjectDbFactory.CreateInstance(criteria.GetSettingKey()))
            {
                db.ExecuteNonQuery(criteria);
            }

            return criteria.RobotList;
        }

        #endregion DataAccess Layer
    }
}