namespace Fanex.NAP.CTS.Service
{
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
    using Fanex.NAP.CTS.Service.Implement;
    using Fanex.NAP.CTS.Service.Interface;
    using Newtonsoft.Json;
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

        //public void RescanCustomerClassification(RescanCustClassificationModel para)
        //{
        //    var custIdsPushCC = RescanProblemAccont(para);

        //    RescanNormalAccount(para.CustIds?.ToList(), custIdsPushCC, para.FunctionId);
        //}

        //public bool RescanCustomerClassification(RescanCustClassificationModel parameters)
        //{
        //    if (parameters.CustIds != null && parameters.CustIds.Any())
        //    {
        //        if (parameters.IsRescanPA)
        //        {
        //            GetExistedProblemAccounts(parameters.CustIds)
        //           .GetProbAccountLosingClassification(parameters.FunctionId)
        //           .InsertProblemAccountLogics(parameters.ServiceName)
        //           .GetRobotUsers(parameters.CustIds).LogRescanPAData(robotUsers, parameters.FuncId_CheckRobotUser, parameters.ServiceName)
        //           .InsertRobot(parameters)
        //           .PushExternalData(parameters);
        //        }

        //        GetAgentCusts(parameters.CustIds)
        //       .RescanNormalAccount(parameters);
        //    }

        //    return isSuccess;
        //}

        #region Rescan PA

        private IEnumerable<long> RescanProblemAccont(RescanCustClassificationModel parameters)
        {
            var custIdsCheckPA = new List<long>();

            //Evidence & PACM
            var existedProblemAccounts = problemAccountService.GetExistedProblemAccounts(parameters.CustIds);
            var paLosing = problemAccountService.GetProblemAccountLosingClassification(parameters.CustIds, parameters.FunctionId);

           // custIdsCheckPA.AddRange(problemAccountService.InsertProblemAccountLogics(existedProblemAccounts, paLosing, InsertPA_ByType.Category, parameters.ServiceName));
           //custIdsCheckPA.AddRange(SyncCustomerEvidenceToCategory(existEvidences, paLosing, parameters.FunctionId));
            custIdsCheckPA.AddRange(GetRobotUsers(parameters));

            //Rescan PA - Push CC & CN88
            var custIdsPushCC = custIdsCheckPA.Distinct().ToList();

            if (custIdsPushCC != null && custIdsPushCC.Any())
            {
                Parallel.Invoke(
                () =>
                {
                    PushCCToExternal(custIdsPushCC, parameters.IsPushCCToSoon88, parameters.IsUpdateDangerToMainDB, parameters.DangerLevelType);
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
                var data = GetRoleCustomer(string.Join(",", custIds));

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


        private List<long> GetRobotUsers(RescanCustClassificationModel para)
        {
            var custIdsToSyncCC = new List<long>();

            if (para.CustIds != null && para.CustIds.Any())
            {
                var existedRobotUsers = GetExistedRobotUser(para.CustIds);

                if (existedRobotUsers != null && existedRobotUsers.Any())
                {
                    logInputDataService.LogInputData(new LogInputViewModel { ProblemAccounts = existedRobotUsers?.ToList() }, para.FuncId_CheckRobotUser);

                    //var result = InsertRobot(existedRobotUsers);

                    //if (string.IsNullOrEmpty(result.ErrorMessage) && result.CustIds != null)
                    //{
                    //    custIdsToSyncCC = result.CustIds.ToList();
                    //}
                }
            }

            return custIdsToSyncCC;
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
                            ReasonID = (int)CATEGORY.ROBOT,
                            Creator = Convert.ToInt32(ConfigurationManager.AppSettings["StarixITUserId"])
                        });
                    });
                }
            }

            return existedRobots;
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

        //public IRescanCustClassificationService GetExistedProblemAccounts(IEnumerable<long> custIds)
        //{
        //    if (custIds != null && custIds.Any())
        //    {
        //        var result = problemAccountService.GetExistedProblemAccounts(custIds);

        //        if (result.Any())
        //        {
        //            problemAccounts = result.Where(c => !c.IsRobot).Select(c => new InsertProblemAccountEntity
        //            {
        //                CustID = c.CustID,
        //                CTSCustID = c.CTSCustID,
        //                CategoryID = c.CategoryID,
        //                CategoryGroup = c.CategoryGroup,
        //                IsDirectPA = c.IsDirectPA,
        //                Remark = c.Remark,
        //                IsLicensee = c.IsLicensee,
        //                RoleID = c.RoleID,
        //                SubscriberID = c.SubscriberID,
        //                CreatedBy = StarixITId
        //            });

        //            robotClassificationPerformance = result.Where(c => c.IsRobot).Select(c => new InsertRobotEntity
        //            {
        //                CustID = c.CustID,
        //                CategoryID = c.CategoryID,
        //                IsFromTW = false,
        //                ActiveDays = c.ActiveDays,
        //                TurnoverRM = c.Turnover,
        //                BetCount = c.BetCount,
        //                WinlossRM = c.Winloss,
        //                CreatedBy = StarixITId
        //            });
        //        }
        //    }

        //    return this;
        //}





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
   
        private IEnumerable<RoleCustomerModel> GetRoleCustomer(string custIds)
        {
            var criteria = new GetRoleCustomerCriteria
            {
                CustIds = custIds
            };

            return repository.Fetch<RoleCustomerModel>(criteria);
        }

        private IEnumerable<RobotUserModel> GetExistedRobotUserDbAccess(string custIds)
        {
            var criteria = new GetExistedRobotUserCriteria
            {
                CustIds = custIds
            };

            return repository.Fetch<RobotUserModel>(criteria);
        }

        #endregion DataAccess Layer
    }
}