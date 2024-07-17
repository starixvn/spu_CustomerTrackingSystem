namespace Fanex.NAP.CTS.Service.Interface.Api
{
    using Fanex.NAP.CTS.Model.API.TWInitialSmart;
    using System.Collections.Generic;

    public interface ITWInitialSmartService
    {
        IEnumerable<long> AddCustTWInitialSmarts(IEnumerable<InitialSmartCustInfoModel> initialSmartCustInfos);
    }
}
