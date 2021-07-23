namespace Fanex.NAP.CTS.Service
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Fanex.Data;
    using Fanex.Data.Repository;
    using Fanex.Logging;
    using Fanex.NAP.CTS.Core;
    using Fanex.NAP.CTS.Core.Extentions;
    using Fanex.NAP.CTS.Core.Utilities;
    using Fanex.NAP.CTS.Criteria;
    using Fanex.NAP.CTS.Criteria.Account.ChainToEvidence;
    using Fanex.NAP.CTS.Criteria.CustomerClassify;
    using Fanex.NAP.CTS.Criteria.ProblemAccountMonitor;
    using Fanex.NAP.CTS.Model;
    using Fanex.NAP.CTS.Model.CustomerClassify;
    using Fanex.NAP.CTS.Model.SyncCustomerToMainDb;
    using Fanex.NAP.CTS.Resource.Consts;
    using Newtonsoft.Json;
    using static Fanex.NAP.CTS.Resource.Consts.Consts;

    public class CustomerClassifyService : ICustomerClassifyService
    {
        private readonly IPushDataToExternalService _pushDataToExternalService;
        private readonly bool _isWriteLogInfo;

        public CustomerClassifyService()
        {
            _pushDataToExternalService = new PushDataToExternalService();
            _isWriteLogInfo = ConfigSettings.IsWriteLogInfo;
        }

        public void RescanCustomerClassify(List<int> custIds, bool isCheckPAFromNAP, bool isPushCCToSoon88, FunctionId funcId)
        {
            var custIdList = new List<int>();

            //Rescan PA - Get SMA CustId
            var roleCusomters = GetAgentCusts(custIds);
            custIdList.AddRange(roleCusomters);

            // Rescan PA - Get PA From Evidence in CTS
            var problemAccountsFromEvi = SyncCustomerEvidenceToCategory(custIds);
            custIdList.AddRange(problemAccountsFromEvi);

            // Rescan PA - Get PA From RobotUser & NAP PA
            var problemAccounts = GetProblemAccounts(custIds, isCheckPAFromNAP);

            if (problemAccounts != null && problemAccounts.Any())
            {
                custIdList.AddRange(InsertProblemAccount(problemAccounts));
            }

            //Rescan PA - Push CC & CN88
            var custIdsToSyncCC = custIdList.Distinct().ToList();

            if (custIdsToSyncCC != null && custIdsToSyncCC.Any())
            {
                SyncCustomerClassToExternal(custIdsToSyncCC, isPushCCToSoon88);
            }

            // Rescan Normal 
            custIds.RemoveAll(x => custIdsToSyncCC.Contains(x));

            if (_isWriteLogInfo)
                LogService.Info($"Web - Normal Rescan - with data: {JsonConvert.SerializeObject(custIds)}");

            if (custIds != null && custIds.Any())
            {
                RescanNormalAccount(custIds, funcId);
            }
        }

        private List<int> SyncCustomerEvidenceToCategory(List<int> custIds)
        {
            var custIdsToSyncCC = new List<int>();

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

        private InsertCategoryResultModel SyncCustomerEvidenceToCategory(List<CustEvidenceModel> custEvidences)
        {
            try
            {
                if (custEvidences == null || !custEvidences.Any())
                {
                    return new InsertCategoryResultModel() { ErrorMessage = string.Empty, CustIds = null };
                }

                var evidencesChunk = custEvidences.Chunk(ConfigSettings.UpdateInternalChunkSize);

                var custIds = new List<int>();

                foreach (var evidences in evidencesChunk)
                {
                    custIds.AddRange(SyncCustomerEvidenceToCategory(JsonConvert.SerializeObject(evidences), true));
                }

                return new InsertCategoryResultModel { ErrorMessage = string.Empty, CustIds = custIds };
            }
            catch (Exception ex)
            {
                LogService.Error($"Web - CustomerClassifyService - SyncCustomerEvidenceToCategory exception: {ex.Message} - Data: {JsonConvert.SerializeObject(custEvidences)}", ex);

                return new InsertCategoryResultModel { ErrorMessage = ex.Message, CustIds = null };
            }
        }

        private List<int> SyncCustomerEvidenceToCategory(string custEvidencesJson, bool isRescan)
        {
            var custIds = new List<int>();
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
                custIds = repository.Fetch<int>(criteria).ToList();
            }

            if (!string.IsNullOrEmpty(criteria.ErrorMessage))
            {
                LogService.Error($"Web - CustomerClassifyService - SyncCustomerEvidenceToCategory failed: {criteria.ErrorMessage} - Data: {custEvidencesJson}");
            }

            if (_isWriteLogInfo)
                LogService.Info($"Web - CustomerClassifyService - SyncCustomerEvidenceToCategory custEvidencesJson:  {custEvidencesJson}");

            return (custIds != null) ? custIds : new List<int>();
        }

        private List<MultipleProblemAccountsModel> GetProblemAccounts(List<int> custIds, bool isCheckPAFromNAP)
        {
            var problemAccounts = new List<MultipleProblemAccountsModel>();

            if (custIds != null && custIds.Any())
            {
                //PA - RobotUser
                var robotUsers = SyncRobotUserToUpdateCategory(custIds);

                if (robotUsers != null && robotUsers.Any())
                {
                    robotUsers.ForEach(c =>
                    {
                        problemAccounts.Add(new MultipleProblemAccountsModel()
                        {
                            CustID = c,
                            ReasonID = (int)CATEGORY.ROBOT
                        });
                    });

                    LogInputData(new LogInputViewModel { ProblemAccounts = problemAccounts }, FunctionId.RescanRobotUser);
                }

                if (_isWriteLogInfo)
                    LogService.Info($"PA Rescan - Get RobotUsers - with data: {JsonConvert.SerializeObject(problemAccounts)}");

                //PA - NAP PA
                if (isCheckPAFromNAP)
                {
                    var existedProblemAccounts = GetExistedProblemAccountsFromNAPPA(string.Join(",", custIds)).ToList();
                    problemAccounts.AddRange(existedProblemAccounts);

                    LogInputData(new LogInputViewModel { ProblemAccounts = existedProblemAccounts }, FunctionId.RescanPAFromNAPPA);

                    if (_isWriteLogInfo)
                        LogService.Info($"PA Rescan - Get PA Custs - with data: {JsonConvert.SerializeObject(existedProblemAccounts)}");
                }
            }

            return problemAccounts;
        }

        private List<int> InsertProblemAccount(List<MultipleProblemAccountsModel> problemAccounts)
        {
            var custIdsToSyncCC = new List<int>();

            if (problemAccounts != null && problemAccounts.Any())
            {
                var result = InsertProblemAccountToCTS(problemAccounts);

                if(string.IsNullOrEmpty(result.ErrorMessage) && result.CustIds != null)
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
            var problemAccounts = new List<MultipleProblemAccountsModel>();

            if (string.IsNullOrEmpty(custIds))
            {
                return problemAccounts;
            }

            var problemAccountsNAPCM = GetProblemAccountsFromNAP(custIds, null).ToList();
            var problemAccountsCN88 = GetProblemAccountsFromNAP_CN88(custIds, null).ToList();

            if (problemAccountsNAPCM != null && problemAccountsNAPCM.Any())
            {
                problemAccounts.AddRange(problemAccountsNAPCM);
            }

            if (problemAccountsCN88 != null && problemAccountsCN88.Any())
            {
                problemAccounts.AddRange(problemAccountsCN88);
            }

            return problemAccounts.Distinct();
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

        private InsertCategoryResultModel InsertProblemAccountToCTS(List<MultipleProblemAccountsModel> problemAccounts)
        {
            try
            {
                if (problemAccounts == null || !problemAccounts.Any())
                {
                    return new InsertCategoryResultModel() { ErrorMessage = string.Empty, CustIds = null };
                }

                var probalemAccountsChunk = problemAccounts.Chunk(ConfigSettings.UpdateInternalChunkSize);

                var custIds = new List<int>();

                foreach (var problemAccs in probalemAccountsChunk)
                {
                    var result = InsertProblemAccount(JsonConvert.SerializeObject(problemAccs), true);

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

        private List<int> InsertProblemAccount(string problemAccountJson, bool isRescan)
        {
            var repository = new DynamicRepository();
            var custIds = new List<int>();

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
                custIds = repository.Fetch<int>(criteria).ToList();
            }

            if (!string.IsNullOrEmpty(criteria.ErrorMessage))
            {
                LogService.Error($"Web - CustomerClassifyService - InsertProblemAccount failed: {criteria.ErrorMessage} - Data: {problemAccountJson}");
            }

            if(_isWriteLogInfo)
                LogService.Info($"Web - CustomerClassifyService - InsertProblemAccount problemAccountJson:  {problemAccountJson}");

            return (custIds != null) ? custIds : new List<int>();
        }

        private void SyncCustomerClassToExternal(List<int> custIdsToSyncCC, bool isPushCCToSoon88)
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

        public void LogInputData(LogInputViewModel content, FunctionId funcId)
        {
            try
            {
                if (content == null)
                {
                    return;
                }

                var inputData = MappingLogInputData(content);

                if (inputData != null && inputData.Any())
                {
                    var result = LogInputDataAccessLayer(inputData, funcId);

                    if (!string.IsNullOrEmpty(result.ErrorMsg))
                    {
                        LogService.Error($"LogInputData failed - FuncId: {funcId}, inputData {content} - Datetime: {DateTime.Now}");
                    }
                }
            }
            catch (Exception ex)
            {
                LogService.Error($"LogInputData failed - FuncId: {funcId}, inputData {content} - Exception {ex}- Datetime: {DateTime.Now}");
            }
        }

        private IEnumerable<LogInputDataModel> MappingLogInputData(LogInputViewModel content)
        {
            var logInputData = new List<LogInputDataModel>();

            if (content?.PlaceEvidences != null && !string.IsNullOrEmpty(content.PlaceEvidences.CustIds))
            {
                var custIds = content.PlaceEvidences.CustIds.Split(',').Select(long.Parse);

                custIds?.ToList().ForEach(c =>
                {
                    logInputData.Add(new LogInputDataModel
                    {
                        CustId = c,
                        InputValue = content.PlaceEvidences.EvidenceId
                    });
                });
            }

            if (content?.PlaceMultiEvidences != null &&
                !string.IsNullOrEmpty(content.PlaceMultiEvidences.CustIdList) &&
                !string.IsNullOrEmpty(content.PlaceMultiEvidences.EvidenceCodeList))
            {
                var custIds = content.PlaceMultiEvidences.CustIdList.Split(',').Select(long.Parse);
                var evidences = content.PlaceMultiEvidences.EvidenceCodeList.Split(',').Select(int.Parse);

                custIds?.ToList().ForEach(c =>
                {
                    evidences?.ToList().ForEach(e =>
                    {
                        logInputData.Add(new LogInputDataModel
                        {
                            CustId = c,
                            InputValue = e
                        });
                    });
                });
            }

            return logInputData;
        }

        private void RescanNormalAccount(List<int> customers, FunctionId funcId)
        {
            if (customers == null || !customers.Any())
            {
                return;
            }

            var custIdsChunk = customers.Chunk(ConfigSettings.PushCCToExternalChunkSize).ToList();

            foreach (var custIds in custIdsChunk)
            {
                var custIdsToBeRescaned = string.Join(",", custIds);

                ClassifyCustomerGeneralCategoryDaily(custIdsToBeRescaned, funcId, false);
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

        private List<int> GetAgentCusts(List<int> custIds)
        {
            var agentIds = new List<int>();

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

        private List<int> SyncRobotUserToUpdateCategory(List<int> custIds)
        {
            var robotUser = new List<int>();

            var custs = GetCustByRobotSiteRule(string.Join(",", custIds)).ToList();

            var getRobotUser = GetExistedRobotUser(string.Join(",", custs));

            if (!string.IsNullOrEmpty(getRobotUser))
            {
                robotUser = getRobotUser.Split(',').Select(int.Parse).ToList();
            }

            return robotUser;
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

        private string GetExistedRobotUser(string custIds)
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
        

        public DBResult LogInputDataAccessLayer(IEnumerable<LogInputDataModel> content, FunctionId funcId)
        {
            var repository = new DynamicRepository();

            var criteria = new LogInputDataCriteria
            {
                InputData = JsonConvert.SerializeObject(content),
                FunctionID = (int)funcId
            };

            repository.Execute(criteria);

            return new DBResult
            {
                ErrorCode = string.IsNullOrEmpty(criteria.ErrorMessage) ? 0 : 1,
                ErrorMsg = criteria.ErrorMessage
            };
        }

    }
}
