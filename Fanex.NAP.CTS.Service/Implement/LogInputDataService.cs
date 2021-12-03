namespace Fanex.NAP.CTS.Service
{
    using Fanex.Data.Repository;
    using Fanex.Logging;
    using Fanex.NAP.CTS.Core;
    using Fanex.NAP.CTS.Criteria.LogInputData;
    using Fanex.NAP.CTS.Model.CustomerClassify;
    using Fanex.NAP.CTS.Model.LogInputData;
    using Newtonsoft.Json;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using static Fanex.NAP.CTS.Resource.Consts.Consts;

    public class LogInputDataService : ILogInputDataService
    {
        public LogInputDataService()
        {
        }

        public void LogInputData(LogInputViewModel content, FunctionId funcId)
        {
            try
            {
                if (content != null)
                {
                    var inputData = MappingLogInputData(content);

                    if (inputData != null && inputData.Any())
                    {
                        var result = LogInputDataAccessLayer(inputData, funcId);

                        if (!string.IsNullOrEmpty(result.ErrorMsg))
                        {
                            LogService.Error($"LogInputData failed - FuncId: {(int)funcId}, inputData {JsonConvert.SerializeObject(content)} - Exception {result.ErrorMsg} - Datetime: {DateTime.Now}");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                LogService.Error($"LogInputData failed - FuncId: {(int)funcId}, inputData {JsonConvert.SerializeObject(content)} - Exception {ex}- Datetime: {DateTime.Now}");
            }
        }

        private IEnumerable<LogInputDataModel> MappingLogInputData(LogInputViewModel content)
        {
            var logInputData = new List<LogInputDataModel>();

            logInputData.AddRange(PrepareEvidenceInputData(content));
            logInputData.AddRange(PreparePAInputData(content));

            return logInputData;
        }

        private IEnumerable<LogInputDataModel> PrepareEvidenceInputData(LogInputViewModel content)
        {
            var logInputData = new List<LogInputDataModel>();

            if (content.PlaceEvidences != null && !string.IsNullOrEmpty(content.PlaceEvidences.CustIds))
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

            if (content.PlaceMultiEvidences != null &&
                !string.IsNullOrEmpty(content.PlaceMultiEvidences.CustIdList) &&
                !string.IsNullOrEmpty(content.PlaceMultiEvidences.EvidenceCodeList))
            {
                var custIds = content.PlaceMultiEvidences.CustIdList.Split(',').Select(long.Parse);
                var evidences = content.PlaceMultiEvidences.EvidenceCodeList.Split(',');

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

        private IEnumerable<LogInputDataModel> PreparePAInputData(LogInputViewModel content)
        {
            var logInputData = new List<LogInputDataModel>();

            if (content.ProblemAccounts != null && content.ProblemAccounts.Any())
            {
                content.ProblemAccounts.ToList().ForEach(e =>
                {
                    logInputData.Add(new LogInputDataModel
                    {
                        CustId = e.CustID,
                        InputValue = e.ReasonID
                    });
                });
            }

            return logInputData;
        }

        public void LogInputData(IEnumerable<LogInputDataModel> data, FunctionId funcId)
        {
            try
            {
                if (data != null && data.Any())
                {
                    var result = LogInputDataAccessLayer(data, funcId);

                    if (!string.IsNullOrEmpty(result.ErrorMsg))
                    {
                        LogService.Error($"LogInputData failed - FuncId: {(int)funcId}, inputData {JsonConvert.SerializeObject(data)} - Exception {result.ErrorMsg} - Datetime: {DateTime.Now}");
                    }
                }
            }
            catch (Exception ex)
            {
                LogService.Error($"LogInputData failed - FuncId: {(int)funcId}, inputData {JsonConvert.SerializeObject(data)} - Exception {ex}- Datetime: {DateTime.Now}");
            }
        }

        #region DataAccess Layer

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

        #endregion DataAccess Layer
    }
}