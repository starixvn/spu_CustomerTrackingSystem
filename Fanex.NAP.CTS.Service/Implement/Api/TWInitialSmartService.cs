namespace Fanex.NAP.CTS.Service.Implement.Api
{
    using Fanex.Data.Repository;
    using Fanex.NAP.CTS.Model.API.TWInitialSmart;
    using Fanex.NAP.CTS.Service.Interface.Api;
    using System;
    using System.Collections.Generic;

    public class TWInitialSmartService : ITWInitialSmartService
    {
        private readonly IDynamicRepository repository;

        public TWInitialSmartService()
        {
            repository = new DynamicRepository();
        }

        public bool AddCustTWInitialSmarts(IEnumerable<InitialSmartCustInfoModel> initialSmartCustInfos)
        {
            throw new NotImplementedException();
        }

    }
}
