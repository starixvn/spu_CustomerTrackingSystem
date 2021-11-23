namespace Fanex.NAP.CTS.Service.Implement.Api
{
    using Fanex.Data.Repository;
    using Fanex.NAP.CTS.Criteria.Api.UpdateCategoryWhenPAUpdated;
    using Fanex.NAP.CTS.Model.API.ProblemCaseMonitor;
    using Fanex.NAP.CTS.Model.API.UpdateCategoryWhenPAUpdated;
    using Fanex.NAP.CTS.Model.CustomerClassify;
    using Fanex.NAP.CTS.Service.Interface.Api;
    using Newtonsoft.Json;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using static Fanex.NAP.CTS.Resource.Consts.Consts;

    public class UpdateCategoryService : IUpdateCategoryService
    {
        private readonly DynamicRepository _repository;
        private readonly ILogInputDataService _logInputDataService;

        public UpdateCategoryService()
        {
            _repository = new DynamicRepository();
            _logInputDataService = new LogInputDataService();
        }

        public IEnumerable<UpdateCategoryModel> UpdateCategoryToCTSWhenPAUpdated(UpdateCategoryParamsModel model)
        {            
            this.LogInputData(model, FunctionId.UpdateCategory);

            var custInfoJson = JsonConvert.SerializeObject(model.CustInfos);

            return this.UpdateCategoryToCTSWhenPAUpdated(custInfoJson, model.UserId);
        }

        public void LogInputData(UpdateCategoryParamsModel model, FunctionId funcId)
        {
            var data = new List<LogInputDataModel>();

            foreach (var cust in model.CustInfos)
            {
                data.Add(new LogInputDataModel
                {
                    CustId = cust.CustID,
                    InputValue = "FromReason: " + cust.FromReasonID + ", ToReason: " + cust.ToReasonID
                });
            }

            this._logInputDataService.LogInputData(data, funcId);
        }

        public IEnumerable<UpdateCategoryModel> UpdateCategoryToCTSWhenPAUpdated(string custInfoJson, int userId)
        {
            var criteria = new UpdateCategoryToCTSWhenPAUpdatedCriteria
            {
                CustInfo = custInfoJson,
                UserId = userId
            };

            return _repository.Fetch<UpdateCategoryModel>(criteria);
        }
    }
}
