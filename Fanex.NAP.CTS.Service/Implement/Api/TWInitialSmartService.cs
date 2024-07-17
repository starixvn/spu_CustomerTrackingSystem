namespace Fanex.NAP.CTS.Service.Implement.Api
{
    using Fanex.Data.Repository;
    using Fanex.NAP.CTS.Criteria.Api.TW;
    using Fanex.NAP.CTS.Model.API.TWInitialSmart;
    using Fanex.NAP.CTS.Service.Interface.Api;
    using Newtonsoft.Json;
    using System;
    using System.Collections.Generic;

    public class TWInitialSmartService : ITWInitialSmartService
    {
        private readonly IDynamicRepository repository;

        public TWInitialSmartService()
        {
            repository = new DynamicRepository();
        }

        public IEnumerable<long> AddCustTWInitialSmarts(IEnumerable<InitialSmartCustInfoModel> initialSmartCustInfos)
        {
            var criteria = new InsertTWInitialSmartCriteria
            {
                CustInfoJson = JsonConvert.SerializeObject(initialSmartCustInfos)
            };

            return repository.Fetch<long>(criteria);
        }

    }
}
