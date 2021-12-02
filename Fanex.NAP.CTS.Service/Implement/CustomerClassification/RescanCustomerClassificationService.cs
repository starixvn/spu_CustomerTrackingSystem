namespace Fanex.NAP.CTS.Service
{
    using Fanex.Data;
    using Fanex.Data.Repository;
    using Fanex.Logging;
    using Fanex.Logging.Extensions;
    using Fanex.Logging.Extensions.Database;
    using Fanex.NAP.CTS.Core.Extentions;
    using Fanex.NAP.CTS.Core.Utilities;
    using Fanex.NAP.CTS.Criteria;
    using Fanex.NAP.CTS.Criteria.Account.ChainToEvidence;
    using Fanex.NAP.CTS.Criteria.CustomerClassify;
    using Fanex.NAP.CTS.Criteria.ProblemAccountMonitor;
    using Fanex.NAP.CTS.Model;
    using Fanex.NAP.CTS.Model.CustomerClassify;
    using Fanex.NAP.CTS.Model.ImportEvidences;
    using Fanex.NAP.CTS.Model.LogInputData;
    using Fanex.NAP.CTS.Model.RescanCustClassificationModel;
    using Fanex.NAP.CTS.Model.SyncCustomerToMainDb;
    using Fanex.NAP.CTS.Resource.Consts;
    using Newtonsoft.Json;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using static Fanex.NAP.CTS.Resource.Consts.Consts;

    public class RescanCustomerClassificationService : IRescanCustomerClassificationService
    {
        private readonly ICustomerClassificationService custClassificationService;
        private readonly ILogInputDataService logInputDataService;
        private readonly IAntiFraudCenterService antiFraudCenterService;
        private readonly bool _isWriteLogInfo;
        private readonly IDynamicRepository repository;

        public RescanCustomerClassificationService()
        {
            custClassificationService = new CustomerClassificationService();
            _isWriteLogInfo = ConfigSettings.IsWriteLogInfo;
            logInputDataService = new LogInputDataService();
            repository = new DynamicRepository();
            antiFraudCenterService = new AntiFraudCenterService();
        }

        public void RescanCustomerClassify(RescanCustClassificationModel para)
        {
            var custIdsPushCC = RescanProblemAccont(para);

            RescanNormalAccount(para.CustIds?.ToList(), custIdsPushCC, para.FunctionId);
        }

        #region Rescan PA

        private IEnumerable<long> RescanProblemAccont(RescanCustClassificationModel para)
        {
            var custIdsCheckPA = new List<long>();

            //Rescan PA - Get SMA CustId
            custIdsCheckPA.AddRange(GetAgentCusts(para.CustIds));

            // Rescan PA - Get PA From Evidence in CTS
            custIdsCheckPA.AddRange(GetExistedEvidences(para.CustIds));

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
                Parallel.Invoke(() =>
                {
                    PushCCToExternal(custIdsPushCC, para.IsPushCCToSoon88, para.IsUpdateDangerToMainDB, para.DangerLevelType);
                },
                () =>
                {
                    antiFraudCenterService.InsertAFCAccounts(custIdsPushCC);
                });
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

        private List<long> GetExistedEvidences(IEnumerable<long> custIds)
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

                var custIdList = new List<long>();

                foreach (var evidences in evidencesChunk)
                {
                    var assCustForRollbacks = Enumerable.Empty<NewPAAssociationCustomerForRollbackModel>();

                    var custIds = SyncCustEvidenceToCategoryDbAccess(JsonConvert.SerializeObject(evidences), true, out assCustForRollbacks);

                    custIdList.AddRange(custIds);

                    CheckNewPAAssociationCustomerForRollback(assCustForRollbacks);
                }

                return new InsertCategoryResultModel { ErrorMessage = string.Empty, CustIds = custIdList };
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
                return GetProblemAccountsFromNAP(custIds, null)?.Distinct() ?? new List<MultipleProblemAccountsModel>();
            }

            if (_isWriteLogInfo)
                LogService.Info($"PA Rescan - Get PA Custs - with data: {JsonConvert.SerializeObject(probAccFromNap)}");

            return new List<MultipleProblemAccountsModel>();
        }

        private IEnumerable<MultipleProblemAccountsModel> GetExistedRobotUser(IEnumerable<long> custIds)
        {
            var existedRobots = new List<MultipleProblemAccountsModel>();

            if (custIds != null && custIds.Any())
            {
                var robotUsers = GetExistedRobotUserDbAccess(string.Join(",", custIds));

                if (robotUsers != null && robotUsers.Any())
                {
                    robotUsers.ToList().ForEach(c =>
                    {
                        existedRobots.Add(new MultipleProblemAccountsModel()
                        {
                            CustID = c.CustId,
                            RobotCounter = c.Counter,
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

        private void PushCCToExternal(List<long> custIdsToSyncCC, bool isPushCCToSoon88, bool isUpdateDanger, DangerLevel dangerLevelType)
        {
            custClassificationService.PushDataToExternal(custIdsToSyncCC, new PushDataToExternalModel
            {
                ExternalBatchSize = ConfigSettings.PushCCToExternalChunkSize,
                IsPushCCToSoon88 = isPushCCToSoon88,
                IsUpdateDangerToMainDB = isUpdateDanger,
                DangerLevelType = dangerLevelType
            });

            if (_isWriteLogInfo)
                LogService.Info($"PA Rescan - Cust to syncCC - with data: {JsonConvert.SerializeObject(custIdsToSyncCC)}");
        }

        public int CheckNewPAAssociationCustomerForRollback(IEnumerable<NewPAAssociationCustomerForRollbackModel> assCustForRollbacks)
        {
            var result = 0;

            try
            {
                if (assCustForRollbacks != null && assCustForRollbacks.Any())
                {
                    var custInfoJson = JsonConvert.SerializeObject(assCustForRollbacks);

                    var criteria = new CheckNewPAAssociationCustomerForRollbackCriteria
                    {
                        CustInfoJson = custInfoJson
                    };

                    result = repository.Execute(criteria);
                }
            }
            catch (Exception ex)
            {
                var logMessage = new LogMessage(ex.Message)
                   .AddSection("Data Info", DatabaseInfoGetters.GetDbExceptionInfo(ex))
                   .AddSection("Exception Info", CommonInfoGetters.GetExceptionInfo(ex)).ToString();

                LogService.Error(logMessage);

                result = -1;
            }

            return result;
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

                if(custIds.Any())
                {
                    Parallel.Invoke(() =>
                    {
                        var custIdsChunk = custIds.Chunk(ConfigSettings.PushCCToExternalChunkSize).ToList();

                        foreach (var custRescanNormal in custIdsChunk)
                        {
                            var custToString = string.Join(",", custRescanNormal);

                            ClassifyCustomerGeneralCategoryDaily(custToString, funcId, false);
                        }
                    },
                    () =>
                    {
                        antiFraudCenterService.DeleteAFCAccounts(custIds);
                    });
                }    
            }
        }

        #endregion Rescan Normal

        #region DataAccess Layer

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

        private List<long> SyncCustEvidenceToCategoryDbAccess(string custEvidencesJson
            , bool isRescan
            , out IEnumerable<NewPAAssociationCustomerForRollbackModel> assCustForRollbacks)
        {
            var custIds = new List<long>();
            assCustForRollbacks = new List<NewPAAssociationCustomerForRollbackModel>();
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
                assCustForRollbacks = repository.Fetch<NewPAAssociationCustomerForRollbackModel>(criteria);

                if (assCustForRollbacks != null && assCustForRollbacks.Any())
                {
                    custIds = assCustForRollbacks.Select(x => (long)x.CustID).ToList();
                }
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

        private IEnumerable<RobotUserModel> GetExistedRobotUserDbAccess(string custIds)
        {
            var repository = new DynamicRepository();

            var criteria = new GetExistedRobotUserCriteria
            {
                CustIds = custIds
            };

            return repository.Fetch<RobotUserModel>(criteria);
        }

        #endregion DataAccess Layer
    }
}