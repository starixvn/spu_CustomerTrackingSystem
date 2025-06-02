namespace Fanex.NAP.CTS.Service.Implement.CustomerClassification
{
    using Fanex.NAP.CTS.Core.Service;
    using Fanex.NAP.CTS.Criteria.CustomerClassification.RescanNormalClassificationBySport;
    using Fanex.NAP.CTS.Model.CustomerClassification.RescanCustomerClassification;
    using Fanex.NAP.CTS.Service.Interface.CustomerClassification;
    using Newtonsoft.Json;
    using System.Collections.Generic;
    using System.Linq;
    using static Fanex.NAP.CTS.Resource.Consts.Consts;

    public class RescanCustomerClassificationBySportService : IRescanCustomerClassificationBySportService
    {
        private readonly ICriteriaService _repository;

        public RescanCustomerClassificationBySportService()
        {
            _repository = new CriteriaService();
        }

        public void ResanNormalBySportForNewCustomers(IEnumerable<RescanNormalBySportListCustModel> custIds, BySport_FunctionId functionId)
        {
            if (custIds != null && custIds.Any())
            {
                var custToString = JsonConvert.SerializeObject(custIds.Distinct());

                PreprocesRescanNormalClassificationBySport(custToString, functionId);
            }
        }

        private void PreprocesRescanNormalClassificationBySport(string custIds, BySport_FunctionId functionId)
        {
            if (!string.IsNullOrEmpty(custIds))
            {
                var criteria = new PreprocessRescanNormalClassificationBySportCriteria
                {
                    ListCustId = custIds,
                    FunctionId = (int)functionId,
                    QueryType = (int)BySport_RescanNormalType.Manual
                };

                _repository.Execute(criteria);
            }
        }
    }
}
