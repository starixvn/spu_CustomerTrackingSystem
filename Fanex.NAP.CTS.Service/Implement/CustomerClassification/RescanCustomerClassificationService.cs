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
    using Fanex.NAP.CTS.Model.ImportEvidences;
    using Fanex.NAP.CTS.Model.LogInputData;
    using Fanex.NAP.CTS.Model.RescanCustClassificationModel;
    using Fanex.NAP.CTS.Model.SyncCustomerToMainDb;
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

            custIdsCheckPA.AddRange(GetExistedEvidences(para.CustIds, para.FunctionId));

            custIdsCheckPA.AddRange(GetProblemAccountFromNAP(para));

            custIdsCheckPA.AddRange(GetRobotUsers(para));

            //Rescan PA - Push CC & CN88
            var custIdsPushCC = custIdsCheckPA.Distinct().ToList();

            if (custIdsPushCC != null && custIdsPushCC.Any())
            {
                Parallel.Invoke(
                () =>
                {
                    PushCCToExternal(custIdsPushCC, para.IsPushCCToSoon88, para.IsUpdateDangerToMainDB, para.DangerLevelType);
                },
                () =>
                {
                    Task.Run(() =>
                    {
                        antiFraudCenterService.InsertAFCAccounts(custIdsPushCC, FromFlow.CTSWEB);
                    });
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
                    agentIds = data.Where(c => c.RoleID == (int)ROLE_CUSTOMER.AGENT
                                            || c.RoleID == (int)ROLE_CUSTOMER.MASTER
                                            || c.RoleID == (int)ROLE_CUSTOMER.SUPER)
                               .Select(c => c.CustID).ToList();
                }
            }

            return agentIds;
        }

        private List<long> GetExistedEvidences(IEnumerable<long> custIds, FunctionId functionId)
        {
            var custIdsToSyncCC = new List<long>();

            var existedCustEvidences = GetExistedCustomerEvidences(string.Join(",", custIds)).ToList();

            var result = SyncCustomerEvidenceToCategory(existedCustEvidences, functionId);

            if (string.IsNullOrEmpty(result.ErrorMessage) && result.CustIds != null && result.CustIds.Any())
            {
                custIdsToSyncCC.AddRange(result.CustIds);
            }

            if (_isWriteLogInfo)
                LogService.Info($"Web - RescanPA - Get Evidence Custs - with data:  {JsonConvert.SerializeObject(existedCustEvidences)}");

            return custIdsToSyncCC;
        }

        private InsertEvidenceCategoryResultModel SyncCustomerEvidenceToCategory(List<CustEvidenceModel> custEvidences, FunctionId functionId)
        {
            try
            {
                if (custEvidences == null || !custEvidences.Any())
                {
                    return new InsertEvidenceCategoryResultModel() { ErrorMessage = string.Empty, CustIds = null };
                }

                var evidencesChunk = custEvidences.Chunk(ConfigSettings.UpdateInternalChunkSize);

                var custIdList = new List<long>();

                foreach (var evidences in evidencesChunk)
                {
                    var assCustForRollbacks = Enumerable.Empty<CheckPACustomerAssociationForRollbackModel>();

                    var probAccountClassification = GetProblemAccountLosing(evidences, functionId);

                    var custIds = SyncCustEvidenceToCategoryDbAccess(JsonConvert.SerializeObject(probAccountClassification), true, out assCustForRollbacks);

                    custIdList.AddRange(custIds);

                    CheckNewPAAssociationCustomerForRollback(assCustForRollbacks);
                }

                return new InsertEvidenceCategoryResultModel { ErrorMessage = string.Empty, CustIds = custIdList };
            }
            catch (Exception ex)
            {
                LogService.Error($"Web - CustomerClassifyService - SyncCustomerEvidenceToCategory exception: {ex.Message} - Data: {JsonConvert.SerializeObject(custEvidences)}", ex);

                return new InsertEvidenceCategoryResultModel { ErrorMessage = ex.Message, CustIds = null };
            }
        }

        private List<long> GetRobotUsers(RescanCustClassificationModel para)
        {
            var custIdsToSyncCC = new List<long>();

            if (para.CustIds != null && para.CustIds.Any())
            {
                var existedRobotUsers = GetExistedRobotUser(para.CustIds);

                if (existedRobotUsers != null && existedRobotUsers.Any())
                {
                    logInputDataService.LogInputData(new LogInputViewModel { ProblemAccounts = existedRobotUsers?.ToList() }, para.FuncId_CheckRobotUser);

                    var result = InsertRobot(existedRobotUsers);

                    if (string.IsNullOrEmpty(result.ErrorMessage) && result.CustIds != null)
                    {
                        custIdsToSyncCC = result.CustIds.ToList();
                    }
                }
            }

            return custIdsToSyncCC;
        }

        private List<long> GetProblemAccountFromNAP(RescanCustClassificationModel para)
        {
            var custIdsToSyncCC = new List<long>();

            if (para.CustIds != null && para.CustIds.Any() && para.IsCheckPAFromNAP)
            {
                var problemAccounts = GetExistedProblemAccountsFromNAPPA(string.Join(",", para.CustIds)).ToList();

                if (problemAccounts != null && problemAccounts.Any())
                {
                    logInputDataService.LogInputData(new LogInputViewModel { ProblemAccounts = problemAccounts }, para.FuncId_CheckPA);

                    var result = InsertProblemAccount(problemAccounts, para.FunctionId);

                    if (string.IsNullOrEmpty(result.ErrorMessage) && result.CustIds != null)
                    {
                        custIdsToSyncCC = result.CustIds.ToList();
                    }
                }
            }

            return custIdsToSyncCC;
        }

        private IEnumerable<MultipleProblemAccountsModel> GetExistedProblemAccountsFromNAPPA(string custIds)
        {
            if (!string.IsNullOrEmpty(custIds))
            {
                return GetProblemAccountsFromNAP(custIds, null)?.Distinct() ?? new List<MultipleProblemAccountsModel>();
            }

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

        private InsertEvidenceCategoryResultModel InsertProblemAccount(IEnumerable<MultipleProblemAccountsModel> problemAccounts, FunctionId functionId)
        {
            var custIds = new List<long>();

            try
            {
                if (problemAccounts != null && problemAccounts.Any())
                {
                    var probalemAccountsChunk = problemAccounts.Chunk(ConfigSettings.UpdateInternalChunkSize);

                    foreach (var problemAccs in probalemAccountsChunk)
                    {
                        var paLosing = GetProblemAccountLosing(problemAccs, functionId);

                        var result = InsertProblemAccountDbAccess(JsonConvert.SerializeObject(paLosing), true);

                        if (result.ErrorCode == -1)
                        {
                            LogService.Error($"Web - CustomerClassifyService - InsertProblemAccount failed: {result.ErrorMessage} - Data: {paLosing}");
                        }
                        else
                        {
                            CheckNewPAAssociationCustomerForRollback(result.NewPAAssociationCustomerForRollback);

                            custIds.AddRange(result.CustIds);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                LogService.Error($"Web - Rescan PA - InsertProblemAccount exception: {ex.Message} - Data: { JsonConvert.SerializeObject(problemAccounts)}", ex);

                return new InsertEvidenceCategoryResultModel { ErrorMessage = ex.Message, CustIds = null };
            }

            return new InsertEvidenceCategoryResultModel() { ErrorMessage = string.Empty, CustIds = custIds };
        }

        private InsertEvidenceCategoryResultModel InsertRobot(IEnumerable<MultipleProblemAccountsModel> robotUsers)
        {
            var custIds = new List<long>();

            try
            {
                if (robotUsers != null && robotUsers.Any())
                {
                    var robotUserChunk = robotUsers.Chunk(ConfigSettings.UpdateInternalChunkSize);

                    foreach (var robots in robotUserChunk)
                    {
                        var result = InsertProblemAccountDbAccess(JsonConvert.SerializeObject(robotUsers), true);

                        if (result.ErrorCode == -1)
                        {
                            LogService.Error($"Web - CustomerClassifyService - InsertRobot failed: {result.ErrorMessage} - Data: {robotUsers}");
                        }
                        else
                        {
                            CheckNewPAAssociationCustomerForRollback(result.NewPAAssociationCustomerForRollback);

                            custIds.AddRange(result.CustIds);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                LogService.Error($"Web - Rescan PA - InsertRobot exception: {ex.Message} - Data: { JsonConvert.SerializeObject(robotUsers)}", ex);

                return new InsertEvidenceCategoryResultModel { ErrorMessage = ex.Message, CustIds = null };
            }

            return new InsertEvidenceCategoryResultModel() { ErrorMessage = string.Empty, CustIds = custIds };
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

        public IEnumerable<InsertProblemAccountModel> GetProblemAccountLosing(IEnumerable<MultipleProblemAccountsModel> problemAccounts, FunctionId functionId)
        {
            if (problemAccounts != null && problemAccounts.Any())
            {
                var custIds = string.Join(",", problemAccounts.Select(c => c.CustID).Distinct());

                var probAccountLosing = GetProblemAccountLosingClassificationDbAccess(custIds, functionId);

                return problemAccounts.GroupJoin(probAccountLosing,
                                                            pa => pa.CustID,
                                                            paLosing => paLosing.CustId,
                                                            (pa, paLosing) => new { pa, paLosing })
                                                            .SelectMany(
                                                                 group => group.paLosing.DefaultIfEmpty(),
                                                                 (group, temp) => new InsertProblemAccountModel
                                                                 {
                                                                     CustID = group.pa.CustID,
                                                                     ReasonID = group.pa.ReasonID,
                                                                     RobotCounter = group.pa.RobotCounter,
                                                                     BetCount = temp?.BetCount,
                                                                     ActiveDays = temp?.ActiveDays,
                                                                     TurnoverRM = temp?.TurnoverRM,
                                                                     WinlossRM = temp?.WinlossRM,
                                                                     WinlossStatus = temp?.WinlossStatus
                                                                 });
            }

            return Enumerable.Empty<InsertProblemAccountModel>();
        }

        public IEnumerable<CustEvidenceInsertModel> GetProblemAccountLosing(IEnumerable<CustEvidenceModel> custEvidences, FunctionId functionId)
        {
            if (custEvidences != null && custEvidences.Any())
            {
                var custIds = string.Join(",", custEvidences.Select(c => c.CustId).Distinct());

                var probAccountLosing = GetProblemAccountLosingClassificationDbAccess(custIds, functionId);

                return custEvidences.GroupJoin(probAccountLosing,
                                            pa => pa.CustId,
                                            paLosing => paLosing.CustId,
                                            (pa, paLosing) => new { pa, paLosing })
                                            .SelectMany(
                                                 group => group.paLosing.DefaultIfEmpty(),
                                                 (group, temp) => new CustEvidenceInsertModel
                                                 {
                                                     CustId = group.pa.CustId,
                                                     CTSCustId = group.pa.CTSCustId,
                                                     EvidenceId = group.pa.EvidenceId,
                                                     SubscriberId = group.pa.SubscriberId,
                                                     ActionType = group.pa.ActionType,
                                                     BetCount = temp?.BetCount,
                                                     ActiveDays = temp?.ActiveDays,
                                                     TurnoverRM = temp?.TurnoverRM,
                                                     WinlossRM = temp?.WinlossRM,
                                                     WinlossStatus = temp?.WinlossStatus
                                                 });
            }

            return Enumerable.Empty<CustEvidenceInsertModel>();
        }

        public void CheckNewPAAssociationCustomerForRollback(IEnumerable<CheckPACustomerAssociationForRollbackModel> assCustForRollbacks)
        {
            if (assCustForRollbacks != null && assCustForRollbacks.Any())
            {
                var custInfoJson = JsonConvert.SerializeObject(assCustForRollbacks);

                var criteria = new CheckNewPAAssociationCustomerForRollbackCriteria
                {
                    CustInfoJson = custInfoJson
                };

                repository.Execute(criteria);
            }
        }

        #endregion Rescan PA

        #region Rescan Normal

        private void RescanNormalAccount(List<long> custIds, IEnumerable<long> custRescanPA, FunctionId funcId)
        {
            if (custIds != null && custIds.Any())
            {
                custIds.RemoveAll(x => custRescanPA.Contains(x));

                if (custIds.Any())
                {
                    var custIdsChunk = custIds.Chunk(ConfigSettings.PushCCToExternalChunkSize).ToList();

                    foreach (var custRescanNormal in custIdsChunk)
                    {
                        var custToString = string.Join(",", custRescanNormal);

                        ClassifyCustomerGeneralCategoryDaily(custToString, funcId, false);
                    }
                }
            }
        }

        #endregion Rescan Normal

        #region DataAccess Layer

        private void ClassifyCustomerGeneralCategoryDaily(string customers, FunctionId funcId, bool isCheckLast14Days)
        {
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

        private List<long> SyncCustEvidenceToCategoryDbAccess(string custEvidencesJson, bool isRescan, out IEnumerable<CheckPACustomerAssociationForRollbackModel> assCustForRollbacks)
        {
            var custIds = new List<long>();
            assCustForRollbacks = new List<CheckPACustomerAssociationForRollbackModel>();

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
                assCustForRollbacks = repository.Fetch<CheckPACustomerAssociationForRollbackModel>(criteria);

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

        private InsertProblemAccountResultModel InsertProblemAccountDbAccess(string problemAccountJson, bool isRescan)
        {
            var criteria = new InsertProblemAccountCriteria
            {
                ProblemAccountJson = problemAccountJson,
                IsRescan = isRescan
            };

            var assCustForRollback = repository.Fetch<CheckPACustomerAssociationForRollbackModel>(criteria);

            return new InsertProblemAccountResultModel
            {
                ErrorMessage = criteria.ErrorMessage,
                ErrorCode = string.IsNullOrEmpty(criteria.ErrorMessage) ? 0 : -1,
                CustIds = assCustForRollback?.Select(x => x.CustID) ?? Enumerable.Empty<long>(),
                NewPAAssociationCustomerForRollback = assCustForRollback
            };
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
            var criteria = new GetExistedRobotUserCriteria
            {
                CustIds = custIds
            };

            return repository.Fetch<RobotUserModel>(criteria);
        }

        public IEnumerable<ProblemAccountLosingClassificationModel> GetProblemAccountLosingClassificationDbAccess(string custIds, FunctionId funcId)
        {
            var criteria = new ProblemAccountLosingClassificationGetCriteria()
            {
                ListCustIds = custIds,
                FunctionId = (int)funcId
            };

            return repository.Fetch<ProblemAccountLosingClassificationModel>(criteria) ?? Enumerable.Empty<ProblemAccountLosingClassificationModel>();
        }

        #endregion DataAccess Layer
    }
}