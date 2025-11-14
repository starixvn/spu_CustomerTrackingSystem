namespace Fanex.NAP.CTS.Service.Interface.CustomerClassificationBySport
{
    using Fanex.NAP.CTS.Model.CustomerClassificationBySport;
    using System.Collections.Generic;
    using static Fanex.NAP.CTS.Resource.Consts.Consts;

    public interface IRescanCustomerClassificationBySportService
    {
        bool RescanCustomerClassificationBySport(IEnumerable<BaseCustomerInfoBySportModel> custInfos, BySport_FunctionId functionId, int batchSize);
    }
}
