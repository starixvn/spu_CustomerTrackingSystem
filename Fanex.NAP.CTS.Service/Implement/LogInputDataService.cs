namespace Fanex.NAP.CTS.Service
{
    using Fanex.Data.Repository;
    using Fanex.NAP.CTS.Core.Extentions;
    using Fanex.NAP.CTS.Core;
    using Fanex.NAP.CTS.Criteria.LogInputData;
    using Fanex.NAP.CTS.Model.CustomerClassification;
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
                            LogExtension.LogError($"LogInputData failed - FuncId: {(int)funcId}, inputData {JsonConvert.SerializeObject(content)} - Exception {result.ErrorMsg} - Datetime: {DateTime.Now}");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                LogExtension.LogError($"LogInputData failed - FuncId: {(int)funcId}, inputData {JsonConvert.SerializeObject(content)} - Exception {ex}- Datetime: {DateTime.Now}");
            }
        }

        private IEnumerable<LogInputDataModel> MappingLogInputData(LogInputViewModel content)
        {
            var logInputData = new List<LogInputDataModel>();

            if (content != null && content.RobotUser != null && content.RobotUser.Any())
            {
                content.RobotUser.ToList().ForEach(c =>
                {
                    logInputData.Add(new LogInputDataModel
                    {
                        CustId = c.CustID,
                        InputValue = new { c.RobotCounter }.ToString()
                    });
                });
            }

            if (content != null && content.LicenseeVIPBAModel != null && content.LicenseeVIPBAModel.Data != null)
            {
                logInputData.Add(new LogInputDataModel
                {
                    CustId = 0,
                    InputValue = new { content.LicenseeVIPBAModel.Data.CreatedBy, FromSystem = Enum.GetName(typeof(FromSystem), content.LicenseeVIPBAModel.FromSystem), content.LicenseeVIPBAModel.Data.CustIDs }.ToString()
                });
            }

            if (content != null && content.RemoveLicenseeBABySiteModel != null && content.RemoveLicenseeBABySiteModel.Data != null)
            {
                logInputData.Add(new LogInputDataModel
                {
                    CustId = 0,
                    InputValue = new { content.RemoveLicenseeBABySiteModel.Data.CreatedBy, FromSystem = Enum.GetName(typeof(FromSystem), content.RemoveLicenseeBABySiteModel.FromSystem), content.RemoveLicenseeBABySiteModel.Data.SiteName }.ToString()
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
                        LogExtension.LogError($"LogInputData failed - FuncId: {(int)funcId}, inputData {JsonConvert.SerializeObject(data)} - Exception {result.ErrorMsg} - Datetime: {DateTime.Now}");
                    }
                }
            }
            catch (Exception ex)
            {
                LogExtension.LogError($"LogInputData failed - FuncId: {(int)funcId}, inputData {JsonConvert.SerializeObject(data)} - Exception {ex}- Datetime: {DateTime.Now}");
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