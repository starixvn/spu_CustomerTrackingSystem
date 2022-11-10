namespace Fanex.NAP.CTS.Service
{
    using Fanex.Data;
    using Fanex.Data.Repository;
    using Fanex.NAP.CTS.Core.Extentions;
    using Fanex.NAP.CTS.Core.Helpers.Xml;
    using Fanex.NAP.CTS.Core.Utilities;
    using Fanex.NAP.CTS.Criteria;
    using Fanex.NAP.CTS.Criteria.Account.CustomerCategory;
    using Fanex.NAP.CTS.Criteria.CustomerClassification;
    using Fanex.NAP.CTS.Model;
    using Fanex.NAP.CTS.Model.Account;
    using Fanex.NAP.CTS.Model.CustomerClassification;
    using Fanex.NAP.CTS.Model.CustomerClassification.RescanCustomerClassification;
    using Fanex.NAP.CTS.Model.LogInputData;
    using Fanex.NAP.CTS.Model.RescanCustClassificationModel;
    using Fanex.NAP.CTS.Model.SyncCustomerToMainDb;
    using Fanex.NAP.CTS.Service.Implement;
    using Fanex.NAP.CTS.Service.Interface;
    using Fanex.NAP.SPU.CTS.CC.Features.CustomerClassification.ProblemAccountInsert.Entities;
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

            if (parameters.IsRescanPA)
            {
                result = RescanProblemAccount(parameters);
            }

            if (result.IsSuccess)
            {
                var smCustIds = GetAgentCusts(parameters.CustIds);
                result.IsSuccess = RescanNormalAccount(parameters, result.CustIds, smCustIds);
            }

            return result.IsSuccess;
        }

        public IEnumerable<RescanCategoryNormalModel> GetExistingCustClassificationNormal(string custIDList)
        {
            var criteria = new GetExistingCustClassificationNormalCriteria
            {
                CustIDList = custIDList
            };

            return repository.Fetch<RescanCategoryNormalModel>(criteria);            
        }

        #region Private Methods

        private RescanResultModel RescanProblemAccount(RescanCustClassificationModel parameters)
        {
            var rescanResult = new RescanResultModel();

            if (parameters != null && parameters.CustIds != null && parameters.CustIds.Any())
            {
                var existedProbAccounts = GetExistedProblemAccounts(parameters.CustIds, parameters.IsDataChanged);

                var custTORemovedOldPA = InsertProblemAccountLogics(existedProbAccounts.ProblemAccounts, parameters.FunctionId, ref rescanResult, parameters.IsToQueue);

                if (rescanResult.IsSuccess)
                {
                    var robotUsers = GetTWRobotUsers(parameters);

                    InsertRobot(robotUsers, existedProbAccounts.RobotClassification, ref rescanResult);
                }

                if (rescanResult.IsSuccess && rescanResult.CustIds.Any())
                {
                    rescanResult.IsSuccess = PushDataToExternalPackge(rescanResult.CustIds.Distinct(), parameters);
                }

                if (rescanResult.IsSuccess && custTORemovedOldPA.Any())
                {
                    problemAccountService.RemoveOldProblemAccount(custTORemovedOldPA);
                }
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

                        var custNormalInfos = GetExistingCustClassificationNormal(custToString);

                        if (custNormalInfos.Any())
                        {
                            ClassifyCustomerGeneralCategoryDaily(custNormalInfos.Distinct().ToXmlString(), parameters.FunctionId);
                        }
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
                        IsFromTW = c.IsFromTW,
                        IsDataChanged = isDataChanged,
                        CreatedBy = Convert.ToInt32(ConfigurationManager.AppSettings["StarixITUserId"])
                    });

                    existedPA.RobotClassification = result.Where(c => c.IsRobot).Select(c => new InsertRobotModel
                    {
                        CustID = c.CustID,
                        IsFromTW = c.IsFromTW,
                        IsFromCTS = c.IsFromCTS,
                        IsFromAI = c.IsFromAI,
                        IsFromTVS = c.IsFromTVS,
                        TVSRequestID = c.TVSRequestID,
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

        private void ClassifyCustomerGeneralCategoryDaily(string customersXML, FunctionId funcId)
        {
            var criteria = new NormalAccountsClassificationGeneralCategorizeDailyCriteria
            {
                CustomersXML = customersXML,              
                FunctionId = (int)funcId
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

        #endregion Private Methods
    }
}