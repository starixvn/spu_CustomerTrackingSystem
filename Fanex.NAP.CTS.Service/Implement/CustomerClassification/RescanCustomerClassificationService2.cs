namespace Fanex.NAP.CTS.Service
{
    using Fanex.Data.Repository;
    using Fanex.NAP.CTS.Core.Extentions;
    using Fanex.NAP.CTS.Core.Utilities;
    using Fanex.NAP.CTS.Criteria;
    using Fanex.NAP.CTS.Criteria.CustomerClassification;
    using Fanex.NAP.CTS.Model;
    using Fanex.NAP.CTS.Model.CustomerClassification;
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
    using System.Threading.Tasks;
    using static Fanex.NAP.CTS.Resource.Consts.Consts;

    public class RescanCustomerClassificationService2 : IRescanCustomerClassificationService2
    {
        private readonly ICustomerClassificationService custClassificationService;
        private readonly IProblemAccountClassificationService problemAccountService;
        private readonly ILogInputDataService logInputDataService;
        private readonly IAntiFraudCenterService antiFraudCenterService;
        private readonly bool _isWriteLogInfo;
        private readonly IDynamicRepository repository;

        public RescanCustomerClassificationService2()
        {
            custClassificationService = new CustomerClassificationService();
            problemAccountService = new ProblemAccountClassificationService();
            _isWriteLogInfo = ConfigSettings.IsWriteLogInfo;
            logInputDataService = new LogInputDataService();
            repository = new DynamicRepository();
            antiFraudCenterService = new AntiFraudCenterService();
        }

        public bool RescanCustomerClassification(RescanCustClassificationModel parameters)
        {
            var result = new RescanResultModel();

            if (parameters.IsRescanPA)
            {
                result = RescanProblemAccount(parameters);
            }

            if(result.IsSuccess)
            {
                var smCustIds = GetAgentCusts(parameters.CustIds);
                result.IsSuccess = RescanNormalAccount(parameters, result.CustIds, smCustIds);
            }

            return result.IsSuccess;
        }

        #region Rescan PA

        private RescanResultModel RescanProblemAccount(RescanCustClassificationModel parameters)
        {
            var rescanResult = new RescanResultModel();

            if( parameters != null && parameters.CustIds != null && parameters.CustIds.Any())
            {
                var existedProbAccounts = GetExistedProblemAccounts(parameters.CustIds);
                var paLosing = problemAccountService.GetProblemAccountLosingClassification(parameters.CustIds, parameters.FunctionId);

                InsertProblemAccountLogics(existedProbAccounts.ProblemAccounts, paLosing, parameters.ServiceName, ref rescanResult);

                if (rescanResult.IsSuccess)
                {
                    var robotUsers = GetTWRobotUsers(parameters);

                    InsertRobot(robotUsers, existedProbAccounts.RobotClassificationPerformance, ref rescanResult);
                }

                if (rescanResult.IsSuccess && rescanResult.CustIds.Any())
                {
                    rescanResult.IsSuccess = PushExternalData(rescanResult.CustIds, parameters);
                }
            }

            return rescanResult;
        }

        private bool RescanNormalAccount(RescanCustClassificationModel parameters, IEnumerable<long> custIdsToRescanPA, IEnumerable<long> smaCustIds)
        {
            if (parameters != null && parameters.CustIds != null && parameters.CustIds.Any())
            {
                var custToRescanNormal = parameters.CustIds.Distinct().ToList();
                custToRescanNormal.RemoveAll(x => custIdsToRescanPA.Contains(x));
                custToRescanNormal.RemoveAll(x => smaCustIds.Contains(x));

                if (custToRescanNormal.Any())
                {
                    var custIdsChunk = custToRescanNormal.Chunk(ConfigSettings.PushCCToExternalChunkSize);

                    foreach (var custIds in custIdsChunk)
                    {
                        var custToString = string.Join(",", custIds);
                        ClassifyCustomerGeneralCategoryDaily(custToString, parameters.FunctionId, false);
                    }
                }
            }

            return true;
        }

        private IEnumerable<long> GetAgentCusts(IEnumerable<long> custIds)
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

        public bool PushExternalData(IEnumerable<long> custIdsToSyncCC, RescanCustClassificationModel parameters)
        {
            bool isSuccess = true;

            if (custIdsToSyncCC.Any())
            {
                Parallel.Invoke(() =>
                {
                    isSuccess = custClassificationService.PushDataToExternal(custIdsToSyncCC, new PushDataToExternalModel
                    {
                        ExternalBatchSize = ConfigSettings.PushCCToExternalChunkSize,
                        IsPushCCToSoon88 = parameters.IsPushCCToSoon88,
                        IsUpdateDangerToMainDB = parameters.IsUpdateDangerToMainDB,
                        DangerLevelType = parameters.DangerLevelType
                    });
                },
                () =>
                {
                    antiFraudCenterService.InsertAFCAccounts(custIdsToSyncCC, FromFlow.CTSWEB);
                });
            }

            return isSuccess;
        }

        private ExistedPAResultModel GetExistedProblemAccounts(IEnumerable<long> custIds)
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
                        CategoryID = c.CategoryID,
                        CategoryGroup = c.CategoryGroup,
                        IsDirectPA = c.IsDirectPA,
                        Remark = c.Remark,
                        IsLicensee = c.IsLicensee,
                        RoleID = c.RoleID,
                        SubscriberID = c.SubscriberID,
                        CreatedBy = Convert.ToInt32(ConfigurationManager.AppSettings["StarixITUserId"])
                    });

                    existedPA.RobotClassificationPerformance = result.Where(c => c.IsRobot).Select(c => new InsertRobotModel
                    {
                        CustID = c.CustID,
                        CategoryID = c.CategoryID,
                        IsFromTW = false,
                        ActiveDays = c.ActiveDays,
                        TurnoverRM = c.Turnover,
                        BetCount = c.BetCount,
                        WinlossRM = c.Winloss,
                        CreatedBy = Convert.ToInt32(ConfigurationManager.AppSettings["StarixITUserId"])
                    });
                }
            }

            return existedPA;
        }

        private void InsertProblemAccountLogics(IEnumerable<InsertProblemAccountModel> problemAccounts, IEnumerable<ProblemAccountLosingClassificationModel> paLosing, string serviceName, ref RescanResultModel rescanResultModel)
        {
            if(problemAccounts.Any())
            {
                var result = problemAccountService.InsertProblemAccountLogics(problemAccounts, paLosing, InsertPA_ByType.Category, serviceName);
                
                rescanResultModel.IsSuccess = (result.ErrorCode == 0);

                if (result.ErrorCode == 0 && result.CustomerInfo.Any())
                {
                    rescanResultModel.CustIds.AddRange(result.CustomerInfo.Select(c => c.CustID).Distinct());
                }
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

        public void InsertRobot(IEnumerable<InsertRobotModel> twRobotUsers, IEnumerable<InsertRobotModel> robotClassification, ref RescanResultModel rescanResultModel)
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

            var customerInfo = problemAccountService.InsertRobotAccount(robotToInsert);

            if (customerInfo.Any())
            {
                var custIds = customerInfo.Select(c => c.CustID).Distinct();

                rescanResultModel.CustIds.AddRange(custIds);
            }
        }


        #endregion DataAccess Layer
    }
}