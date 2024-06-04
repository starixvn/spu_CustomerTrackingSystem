namespace Fanex.NAP.CTS.Service
{
    using Fanex.Data.Repository;
    using Fanex.NAP.CTS.Core.Extentions;
    using Fanex.NAP.CTS.Core.Utilities;
    using Fanex.NAP.CTS.Criteria;
    using Fanex.NAP.CTS.Criteria.CustomerClassification;
    using Fanex.NAP.CTS.Criteria.CustomerClassification.RescanPotentialPA;
    using Fanex.NAP.CTS.Model;
    using Fanex.NAP.CTS.Model.CustomerClassification;
    using Fanex.NAP.CTS.Model.CustomerClassification.RescanCustomerClassification;
    using Fanex.NAP.CTS.Model.LogInputData;
    using Fanex.NAP.CTS.Model.RescanCustClassificationModel;
    using Fanex.NAP.CTS.Model.SyncCustomerToMainDb;
    using Fanex.NAP.CTS.Resource.Consts;
    using Fanex.NAP.CTS.Service.Implement;
    using Fanex.NAP.CTS.Service.Interface;
    using Fanex.NAP.SPU.CTS.CC.Features.CustomerClassification.ProblemAccountInsert.Entities;
    using Newtonsoft.Json;
    using System;
    using System.Collections.Generic;
    using System.Configuration;
    using System.Linq;
    using static Fanex.NAP.CTS.Resource.Consts.Consts;

    public class RescanCustomerClassificationService : IRescanCustomerClassificationService
    {
        private readonly ICustomerClassificationService custClassificationService;
        private readonly IProblemAccountClassificationService problemAccountService;
        private readonly ILogInputDataService logInputDataService;
        private readonly bool _isWriteLogInfo;
        private readonly IDynamicRepository repository;

        public RescanCustomerClassificationService()
        {
            custClassificationService = new CustomerClassificationService();
            problemAccountService = new ProblemAccountClassificationService();
            _isWriteLogInfo = ConfigSettings.IsWriteLogInfo;
            logInputDataService = new LogInputDataService();
            repository = new DynamicRepository();
        }

        public bool RescanCustomerClassification(RescanCustClassificationModel parameters)
        {
            var result = new RescanResultModel();
            var custToRescanPA = new List<long>();

            if (parameters.IsRescanPA)
            {
                result = RescanProblemAccount(parameters);
                custToRescanPA.AddRange(result.CustIds);
            }

            if (parameters.IsRescanPotentialPA)
            {
                result = RescanPotentialProblemAccount(parameters.CustIds.ToList(), custToRescanPA, parameters);
                custToRescanPA.AddRange(result.CustIds);
            }

            if (result.IsSuccess)
            {
                var smCustIds = GetAgentCusts(parameters.CustIds);
                result.IsSuccess = RescanNormalAccount(parameters, custToRescanPA, smCustIds);
            }

            return result.IsSuccess;
        }

        #region Private Methods

        private RescanResultModel RescanProblemAccount(RescanCustClassificationModel parameters)
        {
            var rescanResult = new RescanResultModel();

            if (parameters != null && parameters.CustIds != null && parameters.CustIds.Any())
            {
                var existedProbAccounts = GetExistedProblemAccounts(parameters.CustIds, parameters.IsDataChanged);

                InsertProblemAccountLogics(existedProbAccounts.ProblemAccounts, parameters.FunctionId, ref rescanResult, parameters.IsToQueue);

                if (rescanResult.IsSuccess)
                {
                    var robotUsers = GetTWRobotUsers(parameters);

                    InsertRobot(robotUsers, existedProbAccounts.RobotClassification, ref rescanResult);
                }

                if (rescanResult.IsSuccess && rescanResult.CustIds.Any())
                {
                    rescanResult.IsSuccess = PushDataToExternalPackge(rescanResult.CustIds.Distinct(), parameters);

                    if (rescanResult.IsSuccess)
                    {
                        problemAccountService.RemoveOldProblemAccount(rescanResult.CustIds);
                    }
                }               
            }

            return rescanResult;
        }

        public RescanResultModel RescanPotentialProblemAccount(List<long> custIds, IEnumerable<long> custIdsToRescanPA, RescanCustClassificationModel parameters)
        {
            var rescanResult = new RescanResultModel();

            if (custIds.Any())
            {                
                custIds.RemoveAll(x => custIdsToRescanPA.Distinct().Contains(x));

                var rescanPotentialPA = string.Join(",", custIds);

                if (rescanPotentialPA != string.Empty)
                {
                    var rescanDangerousScorePA = RescanDangerousPA(rescanPotentialPA, parameters);

                    if (rescanDangerousScorePA.IsSuccess)
                    {
                        rescanResult.CustIds.AddRange(rescanDangerousScorePA.CustIds);
                    }

                    var rescanInitialSmartPA = RescanInitialSmartPA(rescanPotentialPA, parameters);

                    if (rescanInitialSmartPA.IsSuccess)
                    {
                        rescanResult.CustIds.AddRange(rescanInitialSmartPA.CustIds);
                    }

                    var rescanPaTWGB = RescanInitialTWGroupBettingPA(rescanPotentialPA, parameters);

                    if (rescanPaTWGB.IsSuccess)
                    {
                        rescanResult.CustIds.AddRange(rescanPaTWGB.CustIds);
                    }
                }               
            }

            return rescanResult;
        }

        public RescanResultModel RescanDangerousPA(string custIDs, RescanCustClassificationModel parameters)
        {
            var rescanResult = new RescanResultModel();

            var rescanDangerous = this.RescanDangerousPA(custIDs);

            if (rescanDangerous != null && rescanDangerous.Any())
            {
                var paDangerous = rescanDangerous.Select(x => new InsertProblemAccountModel(x));

                var custPaDangerous = InsertProblemAccountLogics(paDangerous, parameters.FunctionId, ref rescanResult, parameters.IsToQueue);

                rescanResult.IsSuccess = PushDataToExternalPackge(custPaDangerous.Distinct(), parameters);
            }

            return rescanResult;
        }

        public RescanResultModel RescanInitialSmartPA(string custIDs, RescanCustClassificationModel parameters)
        {
            var rescanResult = new RescanResultModel();

            var rescanInitialSmart = this.CheckExistInitialSmartPA(custIDs);

            if (rescanInitialSmart != null && rescanInitialSmart.Any())
            {
                var pasInitialSmart = rescanInitialSmart.Select(x => new InsertProblemAccountModel(x));

                var custPaInitialSmart = InsertProblemAccountLogics(pasInitialSmart, parameters.FunctionId, ref rescanResult, parameters.IsToQueue);

                rescanResult.IsSuccess = PushDataToExternalPackge(custPaInitialSmart.Distinct(), parameters);
            }

            return rescanResult;
        }

        public RescanResultModel RescanInitialTWGroupBettingPA(string custIDs, RescanCustClassificationModel parameters)
        {
            var rescanResult = new RescanResultModel();

            var rescanInitialTWGB = this.GetInitialTWGroupBetting(custIDs);

            if (rescanInitialTWGB != null && rescanInitialTWGB.Any())
            {
                var custInfos = this.GetCustInfos(custIDs);

                var paTWGBs = rescanInitialTWGB.Join(custInfos, twGBCust => twGBCust.CustID, ctsCust => ctsCust.CustID,
                               (twGBCust, ctsCust) => new InsertProblemAccountModel(twGBCust, ctsCust));

                var custPaTWGB = InsertProblemAccountLogics(paTWGBs, parameters.FunctionId, ref rescanResult, parameters.IsToQueue);

                rescanResult.IsSuccess = PushDataToExternalPackge(custPaTWGB.Distinct(), parameters);
            }

            return rescanResult;
        }

        public bool RescanNormalAccount(RescanCustClassificationModel parameters, IEnumerable<long> custIdsToRescanPA, IEnumerable<long> smaCustIds)
        {
            if (parameters != null && parameters.CustIds != null && parameters.CustIds.Any())
            {
                var custToRescanNormal = parameters.CustIds.Distinct().ToList();
                custToRescanNormal.RemoveAll(x => custIdsToRescanPA.Distinct().Contains(x));

                custToRescanNormal.RemoveAll(x => smaCustIds.Distinct().Contains(x));

                if (custToRescanNormal.Any())
                {
                    var custIdsChunk = custToRescanNormal.Chunk(ConfigSettings.PushCCToExternalChunkSize);

                    foreach (var custIds in custIdsChunk)
                    {
                        var custToString = string.Join(",", custIds);

                        ClassifyNormalCategory(custToString, parameters.FunctionId, RescanNormalType.Manual);
                    }
                }

                if (smaCustIds.Any())
                {
                    var custToRemovePA = smaCustIds.Except(custIdsToRescanPA);
                    PushDataToExternalPackge(custToRemovePA?.Distinct(), parameters);
                }
            }

            return true;
        }

        public IEnumerable<long> GetAgentCusts(IEnumerable<long> custIds)
        {
            if (custIds != null && custIds.Any())
            {
                var criteria = new GetRoleCustomerCriteria
                {
                    CustIds = string.Join(",", custIds.Distinct())
                };

                var data = repository.Fetch<RoleCustomerModel>(criteria);

                if (data != null && data.Any())
                {
                    return data.Where(c => c.RoleID == (int)ROLE_CUSTOMER.AGENT
                                        || c.RoleID == (int)ROLE_CUSTOMER.MASTER
                                        || c.RoleID == (int)ROLE_CUSTOMER.SUPER).Select(c => c.CustID);
                }
            }

            return Enumerable.Empty<long>();
        }

        private bool PushDataToExternalPackge(IEnumerable<long> custIdsToSyncCC, RescanCustClassificationModel parameters)
        {
            bool isSuccess = true;

            if (custIdsToSyncCC.Any())
            {
                isSuccess = custClassificationService.PushDataToExternalPackge(custIdsToSyncCC.Distinct(), new PushDataToExternalModel
                {
                    ExternalBatchSize = ConfigSettings.PushCCToExternalChunkSize,
                    IsPushCCToSoon88 = parameters.IsPushCCToSoon88,
                    IsUpdateDangerToMainDB = parameters.IsUpdateDangerToMainDB,
                    IsSyncAFC = ConfigSettings.IsSyncAFCData
                }, FromFlow.CTSWEB);
            }

            return isSuccess;
        }

        private ExistedPAResultModel GetExistedProblemAccounts(IEnumerable<long> custIds, bool isDataChanged)
        {
            var existedPA = new ExistedPAResultModel();

            if (custIds != null && custIds.Any())
            {
                var result = problemAccountService.GetExistedProblemAccounts(custIds);

                if (result.Any())
                {
                    existedPA.ProblemAccounts = result.Where(c => !c.IsRobot).Select(c => new InsertProblemAccountModel
                    {
                        CustID = c.CustID,
                        CTSCustID = c.CTSCustID,
                        RoleID = c.RoleID,
                        SubscriberID = c.SubscriberID,
                        CategoryID = c.CategoryID,
                        CategoryGroup = c.CategoryGroup,
                        IsLicensee = c.IsLicensee,
                        LastModifiedDate = c.LastModifiedDate,
                        Remark = c.Remark,
                        IsDirectPA = c.IsDirect,
                        IsFromCTS = c.IsFromCTS,
                        IsFromTVS = c.IsFromTVS,
                        TVSRequestID = c.TVSRequestID,
                        IsParlay = c.IsParlay,
                        SportType = c.SportType,
                        IssueTypeID = c.IssueTypeID,
                        IsFromTW = c.IsFromTW,
                        IsDataChanged = isDataChanged,
                        CreatedBy = Convert.ToInt32(ConfigurationManager.AppSettings["StarixITUserId"])
                    });

                    existedPA.RobotClassification = result.Where(c => c.IsRobot).Select(c => new InsertRobotModel
                    {
                        CustID = c.CustID,
                        CategoryID = c.CategoryID,
                        IsFromTW = c.IsFromTW,
                        IsFromCTS = c.IsFromCTS,
                        IsFromAI = c.IsFromAI,
                        IsFromImperva = c.IsFromImperva,
                        IsFromTVS = c.IsFromTVS,
                        TVSRequestID = c.TVSRequestID,
                        IsParlay = c.IsParlay,
                        SportType = c.SportType,
                        IssueTypeID = c.IssueTypeID,
                        IsDirect = c.IsDirect,
                        Remark = c.Remark,
                        CreatedBy = Convert.ToInt32(ConfigurationManager.AppSettings["StarixITUserId"])
                    });
                }
            }

            return existedPA;
        }

        private IEnumerable<long> InsertProblemAccountLogics(IEnumerable<InsertProblemAccountModel> problemAccounts, FunctionId funcId, ref RescanResultModel rescanResultModel, bool isToQueue)
        {
            if (problemAccounts != null && problemAccounts.Any())
            {
                var result = problemAccountService.InsertProblemAccountLogics(problemAccounts, funcId, InsertPA_ByType.Category, isToQueue);

                if (result != null && result.Any())
                {
                    var custIds = result.Select(c => c.CustID).Distinct();

                    rescanResultModel.CustIds.AddRange(custIds);

                    return custIds;
                }
            }

            return Enumerable.Empty<long>();
        }

        public void ClassifyNormalCategory(string custList, FunctionId functionId, RescanNormalType type)
        {
            var criteria = new NormalClassificationRescanCriteria
            {
                ListCustId = custList,
                FunctionId = (int)functionId,
                RescanTypeId = (int)type

            };

            repository.Execute(criteria);
        }

        private IEnumerable<InsertRobotModel> GetTWRobotUsers(RescanCustClassificationModel para)
        {
            if (para.CustIds != null && para.CustIds.Any())
            {
                var robotUsers = problemAccountService.GetExistedTWRobotUser(para.CustIds);

                if (robotUsers != null && robotUsers.Any())
                {
                    logInputDataService.LogInputData(new LogInputViewModel { RobotUser = robotUsers }, para.FuncId_CheckRobotUser);

                    return robotUsers;
                }
            }

            return Enumerable.Empty<InsertRobotModel>();
        }

        private void InsertRobot(IEnumerable<InsertRobotModel> twRobotUsers, IEnumerable<InsertRobotModel> robotClassification, ref RescanResultModel rescanResultModel)
        {
            var robotToInsert = new List<InsertRobotModel>();

            if (twRobotUsers.Any())
            {
                robotToInsert.AddRange(twRobotUsers);
            }

            if (robotClassification.Any())
            {
                robotToInsert.AddRange(robotClassification);
            }

            var customerInfo = problemAccountService.InsertRobotAccountPackge(robotToInsert, true);

            if (customerInfo.Any())
            {
                var custIds = customerInfo.Select(c => c.CustID).Distinct();

                rescanResultModel.CustIds.AddRange(custIds);
            }
        }

        public IEnumerable<RescanPADangerousModel> RescanDangerousPA(string custIDs)
        {
            var criteria = new RescanPADangerousCriteria
            {
                CustIDs = custIDs
            };

            return repository.Fetch<RescanPADangerousModel>(criteria);
        }

        public IEnumerable<RescanTWInitialSmartPAModel> CheckExistInitialSmartPA(string custIDs)
        {
            var criteria = new CheckExistTWInitialSmartPACriteria
            {
                CustIDs = custIDs
            };

            return repository.Fetch<RescanTWInitialSmartPAModel>(criteria);
        }

        public IEnumerable<GetInitialTWGroupBettingModel> GetInitialTWGroupBetting(string custIDs)
        {
            var criteria = new RescanInitialTWGroupBettingCriteria() { CustIDs = custIDs };

            return repository.Fetch<GetInitialTWGroupBettingModel>(criteria);
        }

        public IEnumerable<PotentialPACustInfoModel> GetCustInfos(string custIDs)
        {
            var criteria = new GetCustInfoByCustIDCriteria
            {
                CustIDs = custIDs
            };

            return repository.Fetch<PotentialPACustInfoModel>(criteria);
        }

        #endregion Private Methods
    }
}