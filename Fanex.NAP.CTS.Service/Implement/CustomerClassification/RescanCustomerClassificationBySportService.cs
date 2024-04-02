namespace Fanex.NAP.CTS.Service.Implement.CustomerClassification
{
    using Fanex.Data.Repository;
    using Fanex.NAP.CTS.Criteria.CustomerClassification.RescanNormalClassificationBySport;
    using Fanex.NAP.CTS.Service.Interface.CustomerClassification;
    using System.Collections.Generic;
    using System.Linq;
    using static Fanex.NAP.CTS.Resource.Consts.Consts;

    public class RescanCustomerClassificationBySportService : IRescanCustomerClassificationBySportService
    {
        private readonly IDynamicRepository _repository;

        public RescanCustomerClassificationBySportService()
        {
            _repository = new DynamicRepository();
        }

        public void ResanNormalBySportForNewCustomers(IEnumerable<long> custIds, BySport_FunctionId functionId)
        {
            if (custIds != null && custIds.Any())
            {
                var custToString = string.Join(",", custIds.Distinct());

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
                    FunctionId = (int)functionId
                };

                _repository.Execute(criteria);
            }
        }
    }
}
